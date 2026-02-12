using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System.Text;
using Terraria;
using Terraria.Map;
using TerrariaApi.Server;
using TShockAPI;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => "RecipesBrowser";
    public override Version Version => new Version(1, 1, 3);

    public override string Author => "棱镜,羽学适配,Cai优化";

    public override string Description => GetString("通过指令获取物品合成表");

    public Plugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("RecipesBrowser", this.FindRecipe, "find", "fd", "查"));
        IL.Terraria.Lang.BuildMapAtlas += this.LangOnBuildMapAtlas;
        MapHelper.Initialize();
        Lang.BuildMapAtlas();
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.FindRecipe);
            IL.Terraria.Lang.BuildMapAtlas -= this.LangOnBuildMapAtlas;
        }
        base.Dispose(disposing);
    }

    private void LangOnBuildMapAtlas(ILContext il)
    {
        il.Instrs.RemoveAt(0);
        il.Instrs.RemoveAt(0);
        il.Instrs.RemoveAt(0);
    }

    private void FindRecipe(CommandArgs args)
    {
        if (args.Parameters.Count==0)
        {
            args.Player.SendErrorMessage(GetString("格式错误!正确格式: /find <物品ID|物品名>"));
            return;
        }
        
        var itemByIdOrName = TShock.Utils.GetItemByIdOrName(args.Parameters[0]);
        
        if (itemByIdOrName.Count == 0)
        {
            args.Player.SendErrorMessage(GetString("未找到物品"));
            return;
        }
        
        if (itemByIdOrName.Count > 1)
        {
            args.Player.SendMultipleMatchError(itemByIdOrName.Select(x=> $"{x.Name}({x.type})"));
            return;
        }
        
        var item = itemByIdOrName[0];
        var mode = args.Parameters.Count > 1 ? args.Parameters[1] : "c";
        if (mode.ToLower() == "r")
        {
            args.Player.SendSuccessMessage(this.GetRecipeStringByRequired(item));
            return;
        }

        var list = Main.recipe.ToList().FindAll(r => r.createItem.type == item.type);
        var result = new StringBuilder();
        result.AppendLine(GetString($"物品:{TShock.Utils.ItemTag(item)}"));
        if (list.Count == 0)
        {
            args.Player.SendErrorMessage(GetString("此物品无配方"));
            return;
        }
        if (list.Count >= 1)
        {
            
            for (var i = 0; i < list.Count; i++)
            {
                var numberIcons = i.ToString()
                    .Select(x => 2703 +int.Parse(x.ToString()))
                    .Select(x=> $"[i:{x}]");
                
                result.AppendLine(GetString($"{string.Join("",numberIcons)}配方{i + 1}:"));
                result.AppendLine(GetRecipeStringByResult(list[i]));
            }
        }
        args.Player.SendWarningMessage(result.ToString().Trim('\n'));
    }
    


    private static string GetRecipeStringByResult(Recipe recipe)
    {
        
        var result = new StringBuilder();
        result.Append(GetString("材料："));
        foreach (var item in recipe.requiredItem.Where(r => r.stack > 0))
        {
            result.Append($"{TShock.Utils.ItemTag(item)}{item.Name}{(item.maxStack == 1 || item.stack == 0 ? "" : "x" + item.stack)} ");
        }
        result.AppendLine();
        
        result.Append(GetString("合成站："));
        if (recipe.requiredTile != -1)
        {
            result.Append($"{Lang._mapLegendCache[MapHelper.tileLookup[recipe.requiredTile]]} ");
        }
        if (recipe.needHoney)
        {
            result.AppendLine(GetString("[i:1134]蜂蜜 "));
        }
        if (recipe.needWater)
        {
            result.AppendLine(GetString("[i:126]水 "));
        }
        if (recipe.needLava)
        {
            result.AppendLine(GetString("[i:4825]岩浆 "));
        }

        if (recipe.needSnowBiome)
        {
            result.AppendLine(GetString("[i:593]雪原群系 "));
        }
        if (recipe.needMechdusa)
        {
            result.AppendLine(GetString("[i:4956]天顶种子 "));
        }

        if (recipe.needGraveyardBiome)
        {
            result.AppendLine(GetString("[i:321]墓地 "));
        }
        return result.ToString().Trim('\n');
    }

    private string GetRecipeStringByRequired(Item item)
    {
        var result = new StringBuilder();
        result.AppendLine(GetString("可合成的物品:\n"));
        var source = Main.recipe.Where(r => r.requiredItem.Select(i => i.type).Contains(item.type)).Select(r => r.createItem).ToArray();
        for (var j = 1; j <= source.Length; j++)
        {
            var sourceItem = source.ElementAt(j - 1);
            result.Append($"{TShock.Utils.ItemTag(sourceItem)}{sourceItem.Name}{(sourceItem.maxStack > 1 ? "x" + sourceItem.stack : "")},{(j % 5 == 0 ? "\n" : "")}");
        }
        return result.ToString().Trim(',').Trim('\n');
    }
}