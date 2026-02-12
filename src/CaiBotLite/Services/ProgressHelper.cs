using System;
using System.Collections.Generic;
using System.Linq;
using CaiBotLite.Attributes;
using CaiBotLite.Enums;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Events;

namespace CaiBotLite.Services;

public static class ProgressHelper
{
    // 获取当前满足条件的枚举列表
    public static List<ProgressType> GetCurrentProgresses()
    {
        var result = new List<ProgressType>();
        var type = typeof(ProgressType);
        
        foreach (ProgressType progress in Enum.GetValues(type))
        {
            if (IsConditionMet(progress))
            {
                result.Add(progress);
            }
        }
        
        return result;
    }

    /// <summary>
    /// 检查给定的进度枚举列表，返回不满足条件的进度
    /// </summary>
    /// <param name="requiredProgresses">需要检查的进度列表</param>
    /// <returns>不满足条件的进度列表，如果全部满足则返回空列表</returns>
    public static List<ProgressType> CheckProgresses(IEnumerable<ProgressType> requiredProgresses)
    {
        var unmetProgresses = new List<ProgressType>();
    
        foreach (var progress in requiredProgresses)
        {
            if (!IsConditionMet(progress))
            {
                unmetProgresses.Add(progress);
            }
        }
    
        return unmetProgresses;
    }
    
    private static bool IsConditionMet(ProgressType progress)
    {
        var fieldInfo = typeof(ProgressType).GetField(progress.ToString());
        var mapAttribute = fieldInfo!.GetCustomAttribute<ProgressMapAttribute>();
        
        if (mapAttribute == null)
        {
            return false;
        }
        
        var fieldParts = mapAttribute.FieldName.Split('.');
        object? currentObj = null;
        
        foreach (var part in fieldParts)
        {
            if (currentObj == null)
            {
                var type = GetTypeContainingMember(part);
                {
                    var member = type.GetMember(part, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault();
                    if (member is FieldInfo fi)
                    {
                        currentObj = fi.GetValue(null);
                    }
                    else if (member is PropertyInfo pi)
                    {
                        currentObj = pi.GetValue(null);
                    }
                }
            }
            else
            {
                var type = currentObj.GetType();
                var member = type.GetMember(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault();
                
                if (member is FieldInfo fi)
                {
                    currentObj = fi.GetValue(currentObj);
                }
                else if (member is PropertyInfo pi)
                {
                    currentObj = pi.GetValue(currentObj);
                }
            }

            if (currentObj == null)
            {
                return false;
            }
        }
        
        if (currentObj is bool boolValue && mapAttribute.ExpectedValue is bool expectedBool)
        {
            return boolValue == expectedBool;
        }
        else if (currentObj is int intValue && mapAttribute.ExpectedValue is int expectedInt)
        {
            return intValue == expectedInt;
        }

        return Equals(currentObj, mapAttribute.ExpectedValue);
    }

    // 辅助方法：获取包含指定成员的Type
    private static Type GetTypeContainingMember(string memberName)
    {
        // 检查Terraria.Main中的成员
        if (typeof(Main).GetMember(memberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Length != 0)
        {
            return typeof(Main);
        }
        
        // 检查Terraria.NPC中的成员
        if (typeof(NPC).GetMember(memberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Length != 0)
        {
            return typeof(NPC);
        }
        
        // 检查Terraria.Player中的成员
        if (typeof(Player).GetMember(memberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Length != 0)
        {
            return typeof(Player);
        }
        
        // 检查Terraria.GameContent.Events.DD2Event中的成员
        if (typeof(DD2Event).GetMember(memberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Length != 0)
        {
            return typeof(DD2Event);
        }
        
        // 检查BirthdayParty中的成员
        if (typeof(BirthdayParty).GetMember(memberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Length != 0)
        {
            return typeof(BirthdayParty);
        }

        return null!;
    }

    // 获取枚举值的所有名称
    public static string[] GetProgressNames(ProgressType progress)
    {
        var fieldInfo = typeof(ProgressType).GetField(progress.ToString());
        var nameAttribute = fieldInfo!.GetCustomAttribute<ProgressNameAttribute>();
        
        return nameAttribute?.Names ?? new[] { progress.ToString() };
    }
}