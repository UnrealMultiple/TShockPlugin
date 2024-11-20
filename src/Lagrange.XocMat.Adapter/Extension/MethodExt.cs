using System.Reflection;

namespace Lagrange.XocMat.Adapter.Extension;

internal static class MethodExt
{
    public static bool ParamsMatch(this MethodInfo method, params Type[] args)
    {
        if (method == null)
            return false;
        var methodParams = method.GetParameters();
        if (methodParams.Length != args.Length)
            return false;
        for (int i = 0; i < methodParams.Length; i++)
        {
            if (methodParams[i].ParameterType != args[i])
                return false;
        }
        return true;
    }
}
