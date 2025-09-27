using LazyAPI.Cil;
using System.Reflection;
using System.Reflection.Metadata;

namespace LazyAPI.Hooks;

public sealed class ActionHook : HookBase
{
    public Action RegisterAction { get; }
    public Action? UnregisterAction { get; }
    private readonly Delegate? delegateeObject;
    private readonly MethodInfo? removeMethod;
    internal ActionHook(Action registerMethod, Action unregisterMethod, HookLoadType loadType = HookLoadType.Auto) : base(loadType)
    {
        ArgumentNullException.ThrowIfNull(registerMethod);
        ArgumentNullException.ThrowIfNull(unregisterMethod);
        this.RegisterAction = registerMethod;
        this.UnregisterAction = unregisterMethod;
        this.delegateeObject = null;
        this.removeMethod = null;
    }
    public ActionHook(Action registerAction, HookLoadType loadType = HookLoadType.Auto) : base(loadType)
    {
        ArgumentNullException.ThrowIfNull(registerAction);
        this.RegisterAction = registerAction;
        var registerMethod = registerAction.Method;
        var module = registerAction.Method.Module;
        var bytes = registerMethod.GetMethodBody()!.GetILAsByteArray()!;
        var instructions = CilUtils.GetInstructionsFromBytes(bytes);
        var ldftnIns = instructions.Single(x => x.OpCode == ILOpCode.Ldftn);
        var newobjIns = instructions.Single(x => x.OpCode == ILOpCode.Newobj);
        if (instructions.IndexOf(ldftnIns) + 1 != instructions.IndexOf(newobjIns))
        {
            throw new Exception("MSIL is not [ldftn, newobj]");
        }
        var targetOpCode = instructions[instructions.IndexOf(ldftnIns) - 1].OpCode;
        if (targetOpCode is not ILOpCode.Ldnull or ILOpCode.Ldarg_0)
        {
            throw new Exception($"methodName: [{registerMethod.DeclaringType!.FullName}.{registerMethod.Name}] first instruction is not Ldnull or Ldarg_0");
        }
        var callIns = instructions.Single(x => x.OpCode == ILOpCode.Call);
        var callMethod = module.ResolveMethod((int) callIns.Operand!)!;
        if (!callMethod.Name.StartsWith("add_"))
        {
            throw new Exception("methodName is not startwith 'add_'");
        }
        if (!callMethod.IsStatic)
        {
            throw new Exception("method is not Static");
        }
        var removeMethodName = string.Concat("remove_", callMethod.Name.AsSpan("add_".Length));
        var removeMethod = callMethod.DeclaringType!.GetMethod(removeMethodName) ?? throw new Exception($"can't find remove method '{removeMethodName}'");
        var ldftnMethod = module.ResolveMethod((int) ldftnIns.Operand!)!;
        var newobjMethod = module.ResolveMethod((int) newobjIns.Operand!)!;
        this.delegateeObject = targetOpCode == ILOpCode.Ldnull
            ? Delegate.CreateDelegate(newobjMethod.DeclaringType!, (MethodInfo)ldftnMethod)
            : Delegate.CreateDelegate(newobjMethod.DeclaringType!, registerAction.Target, (MethodInfo)ldftnMethod);
        this.removeMethod = removeMethod;
    }
    protected override void DoRegister()
    {
        this.RegisterAction();
    }

    protected override void DoUnregister()
    {
        if (this.UnregisterAction is null)
        {
            this.removeMethod!.Invoke(null, [this.delegateeObject]);
        }
        else
        {
            this.UnregisterAction();
        }
    }
}