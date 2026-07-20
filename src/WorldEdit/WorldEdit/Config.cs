using System.IO;
using Newtonsoft.Json;

namespace WorldEdit
{
    /// <summary>
    /// 世界编辑配置类，负责管理和持久化系统设置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 魔法棒工具可操作的最大方块数量限制
        /// </summary>
        public int MagicWandTileLimit { get; set; } = 10000;

        /// <summary>
        /// 最大撤销操作历史记录数量
        /// </summary>
        public int MaxUndoCount { get; set; } = 50;

        /// <summary>
        /// 是否对未认证玩家禁用撤销系统
        /// </summary>
        public bool DisableUndoSystemForUnrealPlayers { get; set; } = false;

        /// <summary>
        /// 是否在原理图文件名前添加创建者用户ID
        /// </summary>
        public bool StartSchematicNamesWithCreatorUserID { get; set; } = false;

        /// <summary>
        /// 原理图文件存储文件夹路径
        /// </summary>
        public string SchematicFolderPath { get; set; } = "schematics";

        /// <summary>
        /// 从指定文件路径读取配置
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>配置实例</returns>
        public static Config Read(string configFilePath)
        {
            try
            {
                if (!File.Exists(configFilePath))
                {
                    return new Config().Write(configFilePath);
                }

                string jsonContent = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<Config>(jsonContent) ?? new Config();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取配置文件失败: {ex.Message}");
                return new Config();
            }
        }

        /// <summary>
        /// 将当前配置写入指定文件
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>当前配置实例</returns>
        public Config Write(string configFilePath)
        {
            try
            {
                // 确保目录存在
                string directory = Path.GetDirectoryName(configFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string jsonContent = JsonConvert.SerializeObject(
                    this,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    });

                File.WriteAllText(configFilePath, jsonContent);
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入配置文件失败: {ex.Message}");
                return this;
            }
        }
    }
}