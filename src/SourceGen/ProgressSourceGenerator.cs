using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgressTypeGenerator;

[Generator]
public class ProgressSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var comp = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => node is EnumDeclarationSyntax,
            transform: static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) as INamedTypeSymbol)
            .Where(symbol => symbol?.Name == "ProgressType");
        context.RegisterImplementationSourceOutput(comp, (context, enumSymbol) =>
        {
            var metadata = this.GatherProgressTypeMetadata(enumSymbol);
            var source = this.GenerateSourceCode(metadata);
            context.AddSource("ProgressTypeHelper.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    private List<EnumMemberMetadata> GatherProgressTypeMetadata(INamedTypeSymbol? enumSymbol)
    {
        var metadata = new List<EnumMemberMetadata>();
        if (enumSymbol == null)
        {
            return metadata;
        }
        foreach (var member in enumSymbol.GetMembers().OfType<IFieldSymbol>())
        {
            if (member.ConstantValue == null)
            {
                continue;
            }
            var memberMetadata = new EnumMemberMetadata
            {
                MemberName = member.Name,
                MemberValue = (int) member.ConstantValue
            };

            foreach (var attr in member.GetAttributes())
            {
                switch (attr.AttributeClass?.Name)
                {
                    case "ProgressNameAttribute":
                        memberMetadata.ProgressNames = attr.ConstructorArguments[0].Values
                        .Select(v => v.Value?.ToString())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList()!;
                        break;
                    case "ProgressMapIDAttribute":
                        if (attr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
                        {
                            memberMetadata.MapIDs.AddRange(attr.ConstructorArguments[0].Values
                                .Select(v => (int) v.Value!));
                        }
                        else
                        {
                            memberMetadata.MapIDs.Add((int) attr.ConstructorArguments[0].Value!);
                        }
                        break;
                    case "ProgressMapAttribute":
                        var fieldName = attr.ConstructorArguments[0].Value!.ToString();
                        var value = attr.ConstructorArguments[2].Value;
                        if (!string.IsNullOrEmpty(fieldName) && attr.ConstructorArguments[1].Value is INamedTypeSymbol targetType)
                        {
                            memberMetadata.MapInfo = new MapInfo
                            {
                                FieldName = fieldName,
                                TargetType = targetType.ToDisplayString(),
                                Value = this.FormatValue(value)
                            };
                        }
                        break;
                    default:
                        break;
                }
            }
            metadata.Add(memberMetadata);
        }
        return metadata;
    }

    private string FormatValue(object? value)
    {
        return value switch
        {
            bool b => b ? "true" : "false",
            int i => i.ToString(),
            string s => $"\"{s}\"",
            _ => value?.ToString() ?? "null"
        };
    }

    private string GenerateSourceCode(List<EnumMemberMetadata> metadata)
    {
        var sb = new StringBuilder();
        sb.AppendLine("""""
            #nullable enable
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using LazyAPI.Enums;
            using Terraria;
            using LazyAPI.Utility;

            namespace ProgressHelper;

            public interface IProgressMap
            {
                public bool GetStatus(Player? ply = null);
            }

            public class ProgressMatchAttribute(ProgressType type, params string[] names) : Attribute
            {
                public ProgressType Type { get; } = type;
                public string[] Names { get; } = names;
            }

            """"");
        foreach (var member in metadata)
        {
            sb.AppendLine($"[ProgressMatch(ProgressType.{member.MemberName}, {string.Join(", ", member.ProgressNames.Select(n => $"\"{n}\""))})]");
            sb.AppendLine($"internal class {member.MemberName}Map : IProgressMap");
            sb.AppendLine("{");
            sb.AppendLine($"    public bool GetStatus(Player? ply = null)");
            sb.AppendLine("    {");
            if (member.MemberName == "EvilBoss")
            {
                sb.AppendLine($"        return GameProgress.InBestiaryDB(Terraria.ID.NPCID.EaterofWorldsHead);");
            }
            else if (member.MemberName == "Brainof")
            {
                sb.AppendLine($"        return GameProgress.InBestiaryDB(Terraria.ID.NPCID.BrainofCthulhu);");
            }
            else if (member.MapInfo.TargetType == "Terraria.Player")
            {
                sb.AppendLine($"        return ply?.{member.MapInfo.FieldName} == {member.MapInfo.Value};");
            }
            else
            {
                sb.AppendLine($"        return {member.MapInfo.TargetType}.{member.MapInfo.FieldName} == {member.MapInfo.Value};");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private class EnumMemberMetadata
    {
        public string MemberName { get; set; } = string.Empty;

        public int MemberValue { get; set; }

        public List<string> ProgressNames { get; set; } = [];

        public List<int> MapIDs { get; set; } = [];

        public MapInfo MapInfo { get; set; } = new();
    }

    private class MapInfo
    {
        public string FieldName { get; set; } = "";

        public string TargetType { get; set; } = "";

        public string Value { get; set; } = "";
    }
}