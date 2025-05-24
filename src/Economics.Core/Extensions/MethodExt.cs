using System.Reflection;

namespace Economics.Core.Extensions;

internal static class MethodExt
{
    public static bool ParamsMatch(this MethodInfo method, params Type[] args)
    {
        if (method == null)
        {
            return false;
        }

        var methodParams = method.GetParameters();
        if (methodParams.Length != args.Length)
        {
            return false;
        }

        for (var i = 0; i < methodParams.Length; i++)
        {
            if (methodParams[i].ParameterType != args[i])
            {
                return false;
            }
        }
        return true;
    }

    public static bool ResultMatch(this MethodInfo method, Type result)
    {
        return method == null ? false : method.ReturnType == result;
    }
}