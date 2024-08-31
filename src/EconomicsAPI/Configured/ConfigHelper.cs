using Newtonsoft.Json;

namespace EconomicsAPI.Configured;

public class ConfigHelper
{
    public static T Read<T>(string Path) where T : new()
    {
        return !File.Exists(Path) ? new T() : JsonConvert.DeserializeObject<T>(File.ReadAllText(Path)) ?? new T();
    }


    public static void Write<T>(string PATH, T obj)
    {
        var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
        File.WriteAllText(PATH, data);
    }


    public static T LoadConfig<T>(string PATH) where T : new()
    {
        T obj = new();
        if (File.Exists(PATH))
        {
            try
            {
                obj = Read<T>(PATH);
                Write(PATH, obj);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
        }
        return obj;
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    /// <param name="PATH"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T LoadConfig<T>(string PATH, T obj) where T : new()
    {
        if (File.Exists(PATH))
        {
            try
            {
                obj = Read<T>(PATH);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
        }
        Write(PATH, obj);
        return obj;
    }
}