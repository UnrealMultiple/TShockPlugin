namespace AutoClear
{
    internal enum AutoClearItemCategory
    {
        None,
        Throwable,
        Swinging,
        Regular,
        Equipment,
        Vanity,
    }

    internal static class AutoClearItemRules
    {
        internal static AutoClearItemCategory Classify(int damage, int maxStack)
        {
            if (damage > 0)
            {
                return maxStack > 1
                    ? AutoClearItemCategory.Throwable
                    : AutoClearItemCategory.Swinging;
            }

            if (damage < 0)
            {
                return maxStack > 1
                    ? AutoClearItemCategory.Regular
                    : AutoClearItemCategory.Vanity;
            }

            return maxStack == 1
                ? AutoClearItemCategory.Equipment
                : AutoClearItemCategory.None;
        }

        internal static bool IsEnabled(
            AutoClearItemCategory category,
            AutoClearConfiguration configuration)
        {
            return category switch
            {
                AutoClearItemCategory.Throwable => configuration.SweepThrowable,
                AutoClearItemCategory.Swinging => configuration.SweepSwinging,
                AutoClearItemCategory.Regular => configuration.SweepRegular,
                AutoClearItemCategory.Equipment => configuration.SweepEquipment,
                AutoClearItemCategory.Vanity => configuration.SweepVanity,
                _ => false,
            };
        }
    }
}
