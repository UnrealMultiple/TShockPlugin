using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace LazyAPI.Cil;

public static class CilUtils
{
    private static OpCode?[]? shortOpCodes;
    private static OpCode?[]? largeOpCodes;
    public static OpCode?[] ShortOpCodes
    {
        get
        {
            if (shortOpCodes is null)
            {
                InitOpcodes();
            }
            return shortOpCodes;
        }
    }
    public static OpCode?[] LargeOpCodes
    {
        get
        {
            if (largeOpCodes is null)
            {
                InitOpcodes();
            }
            return largeOpCodes;
        }
    }
    private static Func<Module, object, object>?[]? shortOpcodeTransformFunc;
    public static Func<Module, object, object>?[] ShortOpcodeTransformFunc
    {
        get
        {
            if (shortOpcodeTransformFunc is null)
            {
                InitOpcodes();
            }
            return shortOpcodeTransformFunc;
        }
    }
    private static Func<Module, object, object>?[]? largeOpcodeTransformFunc;
    public static Func<Module, object, object>?[] LargeOpcodeTransformFunc
    {
        get
        {
            if (largeOpcodeTransformFunc is null)
            {
                InitOpcodes();
            }
            return largeOpcodeTransformFunc;
        }
    }
    [MemberNotNull(nameof(shortOpCodes), nameof(largeOpCodes), nameof(shortOpcodeTransformFunc), nameof(largeOpcodeTransformFunc))]
    private static void InitOpcodes()
    {
        var opcodes = typeof(OpCodes).GetFields().Where(x => x.FieldType == typeof(OpCode)).Select(x => (OpCode) x.GetValue(null)!).ToArray();
        Array.Sort(opcodes, OpCodeComparer.Instance);
        var index = Array.FindIndex(opcodes, opcode => opcode.Value < 0);
        shortOpCodes = new OpCode?[256];
        largeOpCodes = new OpCode?[opcodes.Where(x => x.Value < 0).Select(x => (byte) (ushort) x.Value).Max() + 1];
        for (var i = 0; i < index; i++)
        {
            shortOpCodes[opcodes[i].Value] = opcodes[i];
        }
        for (var i = index; i < opcodes.Length; i++)
        {
            largeOpCodes[(byte) (ushort) opcodes[i].Value] = opcodes[i];
        }
        shortOpcodeTransformFunc = new Func<Module, object, object>?[shortOpCodes.Length];
        largeOpcodeTransformFunc = new Func<Module, object, object>?[largeOpCodes.Length];
        var resolveField = static (Module module, object operand) => module.ResolveField((int) operand)!;
        var resolveMember = static (Module module, object operand) => module.ResolveMember((int) operand)!;
        var resolveMethod = static (Module module, object operand) => module.ResolveMethod((int) operand)!;
        var resolveSignature = static (Module module, object operand) => module.ResolveSignature((int) operand)!;
        var resolveString = static (Module module, object operand) => module.ResolveString((int) operand)!;
        var resolveType = static (Module module, object operand) => module.ResolveType((int) operand)!;
        foreach (var code in new ILOpCode[] { ILOpCode.Ldfld, ILOpCode.Ldflda, ILOpCode.Ldsfld, ILOpCode.Ldsflda })
        {
            shortOpcodeTransformFunc[(int) code] = resolveField;
        }
        foreach (var code in new ILOpCode[] { ILOpCode.Call, ILOpCode.Callvirt })
        {
            shortOpcodeTransformFunc[(int) code] = resolveMethod;
        }
    }
    public static List<Instruction> GetInstructionsFromBytes(byte[] bytes)
    {
        var ilByteArray = bytes;
        var ms = new MemoryStream(ilByteArray);
        var br = new BinaryReader(ms);
        var result = new List<Instruction>();
        while (ms.Position != ms.Length)
        {
            var instruction = new Instruction(default, null, (int) ms.Position);
            var opcodeValue = br.ReadByte();
            var opcode = opcodeValue == 0xFE ? LargeOpCodes[br.ReadByte()]!.Value : ShortOpCodes[opcodeValue]!.Value;
            instruction.OpCode = (ILOpCode) opcode.Value;
            switch (opcode.OperandType)
            {
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineMethod:
                case OperandType.InlineI:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                    instruction.Operand = br.ReadInt32();
                    break;
                case OperandType.InlineI8:
                    instruction.Operand = br.ReadInt64();
                    break;
                case OperandType.InlineNone:
                    break;
#pragma warning disable CS0618 // 类型或成员已过时
                case OperandType.InlinePhi:
                    throw new NotSupportedException(nameof(OperandType.InlinePhi));
#pragma warning restore CS0618 // 类型或成员已过时
                case OperandType.InlineR:
                    instruction.Operand = br.ReadDouble();
                    break;
                case OperandType.InlineSwitch:
                    var count = br.ReadUInt32();
                    var targets = new int[count];
                    for (var i = 0; i < count; i++)
                    {
                        targets[i] = br.ReadInt32();
                    }
                    instruction.Operand = targets;
                    break;
                case OperandType.InlineVar:
                    instruction.Operand = br.ReadUInt16();
                    break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                    instruction.Operand = br.ReadSByte();
                    break;
                case OperandType.ShortInlineR:
                    instruction.Operand = br.ReadSingle();
                    break;
                case OperandType.ShortInlineVar:
                    instruction.Operand = br.ReadByte();
                    break;
                default:
                    throw new NotSupportedException(opcode.OperandType.ToString());
            }
            result.Add(instruction);
        }
        return result;
    }
    public static OpCode GetOpCode(ILOpCode code)
    {
        return (ushort) code >= 0xFE00 ? LargeOpCodes[(ushort) code - 0xFE00]!.Value : ShortOpCodes[(int) code]!.Value;
    }
    private class OpCodeComparer : IComparer<OpCode>
    {
        public static OpCodeComparer Instance = new OpCodeComparer();
        private OpCodeComparer() { }
        public int Compare(OpCode x, OpCode y)
        {
            return ((ushort) x.Value).CompareTo((ushort) y.Value);
        }
    }
}