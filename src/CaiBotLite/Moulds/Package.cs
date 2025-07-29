using CaiBotLite.Enums;
using CaiBotLite.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using On.Terraria.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CaiBotLite.Moulds;

[Serializable]
public class Package(Direction direction, PackageType type, bool isRequest, string? requestId)
{
    // {
    //     "version": "0.1.0",       // 数据包版本
    //     "direction": "to_server", // 数据包方向
    //     "type": "self_kick",      // 数据包类型
    //     "is_request": true,       // 是否为请求
    //     "request_id": "...",      // 请求ID
    //     "payload": {              // 数据 Dict[str, Any]
    //         ...
    //     }
    // }

    [JsonProperty("version")] public Version Version => this.Type.GetVersion();

    [JsonProperty("direction")] public Direction Direction = direction;

    [JsonProperty("type")] public PackageType Type = type;

    [JsonProperty("is_request")] public bool IsRequest = isRequest;

    [JsonProperty("request_id")] public string? RequestId = requestId;

    [JsonProperty("payload")] public Payload Payload = new ();

    public T Read<T>(string key)
    {
        if (!this.Payload.TryGetValue(key, out var value))
        {
            throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
        }

        // 处理枚举类型
        if (typeof(T).IsEnum && value is string stringValue)
        {
            return HandleEnumConversion<T>(stringValue);
        }

        // 处理 List<Enum> 类型
        if (IsListOfEnum(typeof(T), out var enumType) && value is Newtonsoft.Json.Linq.JArray jArray)
        {
            return HandleListOfEnumConversion<T>(enumType, jArray);
        }

        // 处理普通泛型列表
        if (value is Newtonsoft.Json.Linq.JArray array && typeof(T).IsGenericType)
        {
            var listType = typeof(T).GetGenericTypeDefinition();
            if (listType == typeof(List<>) || listType == typeof(IList<>) || listType == typeof(IEnumerable<>))
            {
                try
                {
                    return array.ToObject<T>(JsonSerializer.CreateDefault())!;
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException($"Cannot convert JArray to {typeof(T).Name}", ex);
                }
            }
        }

        // 默认情况
        if (value is not T targetValue)
        {
            throw new InvalidCastException($"Expected type {typeof(T).Name}, but got {value.GetType().Name}");
        }

        return targetValue;
    }

// 处理枚举转换
    private static T HandleEnumConversion<T>(string stringValue)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>($"\"{stringValue}\"", Converter)!;
        }
        catch (JsonException ex)
        {
            if (Enum.GetNames(typeof(T)).Any(name => string.Equals(name, "Unknown", StringComparison.OrdinalIgnoreCase)))
            {
                return (T) Enum.Parse(typeof(T), "Unknown", true);
            }

            throw new InvalidCastException($"Cannot convert string '{stringValue}' to enum {typeof(T).Name}", ex);
        }
    }

// 处理 List<Enum> 转换
    private static T HandleListOfEnumConversion<T>(Type enumType, Newtonsoft.Json.Linq.JArray jArray)
    {
        try
        {
            var listType = typeof(List<>).MakeGenericType(enumType);
            var result = Activator.CreateInstance(listType) as System.Collections.IList;

            foreach (var item in jArray)
            {
                if (item.Type == Newtonsoft.Json.Linq.JTokenType.String)
                {
                    var enumValue = Enum.Parse(enumType, item.ToString(), true);
                    result!.Add(enumValue);
                }
                else
                {
                    throw new InvalidCastException($"Expected string value for enum, but got {item.Type}");
                }
            }

            return (T) result!;
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert JArray to List<{enumType.Name}>", ex);
        }
    }

// 检查是否是 List<Enum> 类型
    private static bool IsListOfEnum(Type type, out Type enumType)
    {
        enumType = null!;

        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
        {
            return false;
        }

        var elementType = type.GetGenericArguments()[0];
        if (elementType.IsEnum)
        {
            enumType = elementType;
            return true;
        }

        return false;
    }

    private static readonly StringEnumConverter Converter = new (new SnakeCaseNamingStrategy());

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.None, Converter);
    }

    public static Package Parse(string json)
    {
        return JsonConvert.DeserializeObject<Package>(json, Converter)!;
    }
}

public enum Direction
{
    [JsonProperty("to_server")] ToServer,
    [JsonProperty("to_bot")] ToBot
}

public class Payload : Dictionary<string, object>
{
    public new object this[string key]
    {
        get => this.TryGetValue(key, out _);
        set
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, value);
            }
            else
            {
                base[key] = value;
            }
        }
    }
}