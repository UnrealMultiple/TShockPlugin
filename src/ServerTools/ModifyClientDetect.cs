using IL.Terraria.GameContent.LeashedEntities;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;

namespace ServerTools;

public class ModifyClientDetect
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal sealed class HiddenValueAttribute(int mask1, int mask2) : Attribute
    {
        public int Mask1 { get; } = mask1;
        public int Mask2 { get; } = mask2;
    }

    [HiddenValue(0x5A5A5A5A, 0x5A5A5A93)]
    public static class HiddenChecker
    {
        private static readonly int _targetValue;

        static HiddenChecker()
        {
            var attr = typeof(HiddenChecker).GetCustomAttribute<HiddenValueAttribute>()!;
            _targetValue = attr.Mask1 ^ attr.Mask2;
        }

        public static void CheckModify(MessageBuffer instance, int value)
        {
            var player = TShockAPI.TShock.Players[instance.whoAmI];
            if (CheckPacket(value) && player != null)
            {
                var text = $"[警告]玩家 {player.Name} 使用修改后的客户端进入服务器！";
                TShockAPI.TShock.Log.ConsoleWarn(text);
                TShockAPI.TShock.Utils.Broadcast(text, Microsoft.Xna.Framework.Color.Red);
                if(Config.Instance.KickCheater)
                {
                    player.Kick("使用了修改后的客户端！");
                }
            }
        }

        private static bool CheckPacket(int value)
        {
            var exceptionCheck = false;
            try
            {
                var dummy = 100 / (value - _targetValue);
            }
            catch (DivideByZeroException)
            {
                exceptionCheck = true;
            }
            var unsafeCheck = UnsafeMemoryEquals(value, _targetValue);
            var exprCheck = ExpressionTreeEquals(value, _targetValue);
            var dynamicCheck = DynamicILComparer(value, _targetValue);
            var recursionCheck = RecursiveChaos(value, _targetValue);
            return exceptionCheck && unsafeCheck && exprCheck && dynamicCheck && recursionCheck;
        }

        private static unsafe bool UnsafeMemoryEquals(int a, int b)
        {
            var pa = (byte*) &a;
            var pb = (byte*) &b;
            for (var i = 0; i < sizeof(int); i++)
            {
                if (pa[i] != pb[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ExpressionTreeEquals(int x, int target)
        {
            var param = Expression.Parameter(typeof(int), "x");
            var targetConst = Expression.Constant(target, typeof(int));
            var diff = Expression.Subtract(param, targetConst);
            var sum = Expression.Add(param, targetConst);
            var useless = Expression.Divide(Expression.Multiply(diff, sum), Expression.Add(param, Expression.Constant(1)));
            var condition = Expression.Equal(diff, Expression.Constant(0));
            var block = Expression.Block(condition);
            var func = Expression.Lambda<Func<int, bool>>(block, param).Compile();
            return func(x);
        }

        private static bool DynamicILComparer(int x, int target)
        {
            var dynMethod = new DynamicMethod("SecretILComparer", typeof(bool), new[] { typeof(int), typeof(int) }, typeof(HiddenChecker).Module);
            var il = dynMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Ret);
            var comparer = (Func<int, int, bool>) dynMethod.CreateDelegate(typeof(Func<int, int, bool>));
            return comparer(x, target);
        }

        private static bool RecursiveChaos(int x, int target)
        {
            return DeepRecursive(x, target, 1000);
        }

        private static bool DeepRecursive(int x, int target, int depth)
        {
            return depth > 0 && (x == target || (depth % 2 == 0 ? DeepRecursive(x + 1, target, depth - 1) : DeepRecursive(x - 1, target, depth - 1)));
        }
    }
}
