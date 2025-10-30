using System;
using System.ComponentModel;
using System.Reflection;

namespace CurrencyTracker.Helpers;

public static class EnumHelper
{
    public static string GetDescription<TEnum>(TEnum value) where TEnum : Enum
    {
        var field = typeof(TEnum).GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}
