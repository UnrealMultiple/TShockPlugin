using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using TerrariaApi.Server;

using TShockAPI;

namespace GroundCraft;

public sealed partial class GroundCraft
{
    private void UpdateCraftAnimations()
    {
        for (int i = _craftAnimations.Count - 1; i >= 0; i--)
        {
            CraftAnimation animation = _craftAnimations[i];
            animation.Age++;

            if (!AnimationItemsStillValid(animation))
            {
                CancelCraftAnimation(animation);
                _craftAnimations.RemoveAt(i);
                continue;
            }

            int duration = AnimationDuration(animation);
            float progress = Math.Clamp(animation.Age / (float)duration, 0f, 1f);
            Vector2 target = CraftTarget(animation);

            for (int j = 0; j < animation.Ingredients.Count; j++)
                MoveAnimatedIngredient(animation, animation.Ingredients[j], j, animation.Ingredients.Count, progress, target, animation.Age, duration);

            if (animation.Age < duration)
                continue;

            CompleteCraftAnimation(animation, target);
            _craftAnimations.RemoveAt(i);
        }
    }

    private static int AnimationDuration(CraftAnimation animation)
    {
        return animation.IsZenith ? ZenithAnimationTicks : CraftAnimationTicks;
    }

    private static Vector2 CraftTarget(CraftAnimation animation)
    {
        float lift = animation.IsZenith ? ZenithAnimationLiftPixels : CraftAnimationLiftPixels;
        return animation.Center + new Vector2(0f, -lift);
    }

    private static bool AnimationItemsStillValid(CraftAnimation animation)
    {
        foreach (AnimatedIngredient ingredient in animation.Ingredients)
        {
            if (ingredient.Index < 0 || ingredient.Index >= Main.item.Length)
                return false;

            WorldItem item = Main.item[ingredient.Index];
            if (!item.active || item.type != ingredient.Type || item.stack != ingredient.Stack)
                return false;
        }

        return true;
    }

    private static void MoveAnimatedIngredient(
        CraftAnimation animation,
        AnimatedIngredient ingredient,
        int order,
        int total,
        float progress,
        Vector2 target,
        int age,
        int duration)
    {
        WorldItem item = Main.item[ingredient.Index];
        Vector2 visualCenter = AnimatedCenter(animation, ingredient, order, total, progress, target);
        int syncEvery = AnimationSyncInterval(animation);
        float lookaheadTicks = Math.Max(1, syncEvery);
        float lookaheadProgress = Math.Min(progress + lookaheadTicks / duration, 1f);
        Vector2 predictedCenter = AnimatedCenter(animation, ingredient, order, total, lookaheadProgress, target);

        item.position = visualCenter - new Vector2(ingredient.Width / 2f, ingredient.Height / 2f);
        item.velocity = (predictedCenter - visualCenter) / lookaheadTicks;
        LockAnimatedItem(item);

        if (animation.IsZenith)
            SpawnZenithItemTrail(visualCenter, item.velocity, order, age);

        if (age == 1 || age % syncEvery == 0)
            SyncItemNoGrab(ingredient.Index);
    }

    private static int AnimationSyncInterval(CraftAnimation animation)
    {
        return animation.IsZenith ? ZenithAnimationSyncEveryTicks : CraftAnimationSyncEveryTicks;
    }

    private static Vector2 AnimatedCenter(CraftAnimation animation, AnimatedIngredient ingredient, int order, int total, float progress, Vector2 target)
    {
        return animation.IsZenith
            ? ZenithAnimatedCenter(animation, ingredient, order, total, progress, target)
            : DefaultAnimatedCenter(ingredient, order, total, progress, target);
    }

    private static Vector2 DefaultAnimatedCenter(AnimatedIngredient ingredient, int order, int total, float progress, Vector2 target)
    {
        float eased = EaseOutCubic(progress);
        float phase = MathHelper.TwoPi * (CraftAnimationTurns * progress + order / Math.Max(1f, total));
        float radius = CraftAnimationOrbitRadiusPixels * (1f - eased);
        Vector2 spiral = new(MathF.Cos(phase) * radius, MathF.Sin(phase) * radius * 0.55f);
        return Vector2.Lerp(ingredient.StartCenter, target, eased) + spiral;
    }

    private static Vector2 ZenithAnimatedCenter(CraftAnimation animation, AnimatedIngredient ingredient, int order, int total, float progress, Vector2 target)
    {
        float eased = EaseInOutCubic(progress);
        float gather = EaseOutCubic(Math.Clamp(progress * 1.6f, 0f, 1f));
        float phase = MathHelper.TwoPi * (ZenithAnimationTurns * progress + order / Math.Max(1f, total));
        float radius = ZenithAnimationOrbitRadiusPixels * (1f - progress * progress);
        Vector2 spiral = new(MathF.Cos(phase) * radius, MathF.Sin(phase) * radius * 0.42f);
        float parabola = ZenithAnimationParabolaPixels * 4f * progress * (1f - progress);
        Vector2 orbitCenter = Vector2.Lerp(animation.Center, target, eased) + new Vector2(0f, -parabola);
        Vector2 orbitPoint = orbitCenter + spiral;

        return Vector2.Lerp(ingredient.StartCenter, orbitPoint, gather);
    }

    private static float EaseOutCubic(float value)
    {
        float inverse = 1f - value;
        return 1f - inverse * inverse * inverse;
    }

    private static float EaseInOutCubic(float value)
    {
        return value < 0.5f
            ? 4f * value * value * value
            : 1f - MathF.Pow(-2f * value + 2f, 3f) / 2f;
    }

    private void CompleteCraftAnimation(CraftAnimation animation, Vector2 target)
    {
        foreach (AnimatedIngredient ingredient in animation.Ingredients)
        {
            WorldItem item = Main.item[ingredient.Index];
            item.TurnToAir(true);
            ClearConsumedItem(ingredient.Index);
            _lockedItemIndexes.Remove(ingredient.Index);
            _stableScans.Remove(ingredient.Index);
        }

        SpawnDeferredLeftovers(animation);
        SpawnItem(animation.Recipe.OutputType, animation.OutputStack, target);
        if (animation.IsZenith)
            SpawnZenithFinale(target);
        else
            SpawnCraftEffect(target);

        _runtime.CraftBatches++;
        _runtime.Crafts += animation.CraftCount;
        TShock.Log.ConsoleInfo(GetString($"[GroundCraft] 螺旋融合完成：{ItemName(animation.Recipe.OutputType)} x{animation.OutputStack}。"));
        NotifyNearby(target, animation.Recipe, animation.OutputStack, animation.ConsumedStacks);
    }

    private void CancelCraftAnimation(CraftAnimation animation)
    {
        foreach (AnimatedIngredient ingredient in animation.Ingredients)
        {
            _lockedItemIndexes.Remove(ingredient.Index);
            if (ingredient.Index < 0 || ingredient.Index >= Main.item.Length)
                continue;

            WorldItem item = Main.item[ingredient.Index];
            if (!item.active)
                continue;

            item.noGrabDelay = 0;
            item.playerIndexTheItemIsReservedFor = 255;
            item.velocity = Vector2.Zero;
            SyncItem(ingredient.Index);
            NetMessage.SendData(MessageID.ItemOwner, -1, -1, null, ingredient.Index);
        }

        SpawnDeferredLeftovers(animation);
    }

    private static void SpawnDeferredLeftovers(CraftAnimation animation)
    {
        foreach (DeferredLeftover leftover in animation.Leftovers)
            SpawnItem(leftover.Type, leftover.Stack, leftover.Center);
    }

    private static void SpawnZenithItemTrail(Vector2 center, Vector2 velocity, int order, int age)
    {
        if ((age + order * ZenithTrailOrderOffsetTicks) % ZenithTrailEveryTicks != 0)
            return;

        Vector2 trailPoint = center;
        if (velocity.LengthSquared() > 0.01f)
            trailPoint -= Vector2.Normalize(velocity) * 14f;

        NetMessage.SendData(MessageID.PoofOfSmoke, -1, -1, null, (int)trailPoint.X, trailPoint.Y);
    }

    private void ReleaseCraftAnimations()
    {
        foreach (CraftAnimation animation in _craftAnimations)
            CancelCraftAnimation(animation);

        _craftAnimations.Clear();
        _lockedItemIndexes.Clear();
    }

    private void OnGetData(GetDataEventArgs args)
    {
        if (args.Handled || !IsLockedItemPacket(args))
            return;

        int index = BitConverter.ToInt16(args.Msg.readBuffer, args.Index);
        if (!_lockedItemIndexes.Contains(index))
            return;

        args.Handled = true;
        ReassertLockedItem(index);
    }

    private static bool IsLockedItemPacket(GetDataEventArgs args)
    {
        int msgId = (int)args.MsgID;
        if (args.Length < 2)
            return false;

        return msgId == MessageID.SyncItem
            || msgId == MessageID.ItemOwner
            || msgId == MessageID.ReleaseItemOwnership
            || msgId == MessageID.SyncItemDespawn;
    }

    private static void LockAnimatedItem(int index)
    {
        WorldItem item = Main.item[index];
        LockAnimatedItem(item);
    }

    private static void LockAnimatedItem(WorldItem item)
    {
        item.noGrabDelay = CraftAnimationNoGrabDelay;
        item.playerIndexTheItemIsReservedFor = 255;
        item.ownIgnore = -1;
        item.ownTime = 0;
        item.beingGrabbed = false;
        item.keepTime = Math.Max(item.keepTime, 10);
    }

    private void ReassertLockedItem(int index)
    {
        if (index < 0 || index >= Main.item.Length)
            return;

        WorldItem item = Main.item[index];
        if (!item.active)
            return;

        LockAnimatedItem(item);
        SyncItemNoGrab(index);
        NetMessage.SendData(MessageID.ItemOwner, -1, -1, null, index);
    }
}
