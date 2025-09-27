using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace LazyAPI.Cil;

[DebuggerDisplay("OpCode = {OpCode}, Offset = {Offset}")]
public class Instruction
{
    public ILOpCode OpCode;
    public object? Operand;
    public int Offset;
    public bool IsBranchTarget;

    public Instruction(ILOpCode opCode, object? operand = null)
    {
        this.OpCode = opCode;
        this.Operand = operand;
    }
    public Instruction(ILOpCode opCode, object? operand, int offset) : this(opCode, operand)
    {
        this.Offset = offset;
    }
    public int GetSize()
    {
        var opcode = CilUtils.GetOpCode(this.OpCode);
        var size = opcode.Size;
        return opcode.OperandType switch
        {
            OperandType.InlineSwitch => size + (1 + ((Array) this.Operand!).Length) * 4,
            OperandType.InlineI8 or OperandType.InlineR => size + 8,
            OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or OperandType.InlineMethod or OperandType.InlineString or OperandType.InlineTok or OperandType.InlineType or OperandType.ShortInlineR or OperandType.InlineSig => size + 4,
            OperandType.InlineVar => size + 2,
            OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => size + 1,
            _ => size,
        };
    }
}