using Terraria;
using TShockAPI;


namespace WorldModify
{
    public class BackupHelper
    {
        public static string BackupPath { get; set; }

        public static void Backup(TSPlayer op, string notes = "")
        {
            Thread thread = new Thread((ThreadStart)delegate
            {
                DoBackup(op, notes);
            })
            {
                Name = "[wm]Backup Thread"
            };
            thread.Start();
        }

        private static void DoBackup(TSPlayer op, string notes)
        {
            try
            {
                string worldPathName = Main.worldPathName;
                string fileName = Path.GetFileName(worldPathName);
                if (string.IsNullOrEmpty(notes))
                {
                    Main.ActiveWorldFileData._path = Path.Combine(BackupPath, $"{fileName}.{DateTime.Now:yyyyMMddHHmmss}.bak");
                }
                else
                {
                    Main.ActiveWorldFileData._path = Path.Combine(BackupPath, $"{fileName}.{DateTime.Now:yyyyMMddHHmmss}_{notes}.bak");
                }
                string directoryName = Path.GetDirectoryName(Main.worldPathName);
                if (directoryName != null && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                TShock.Log.Info("[wm]正在备份地图...");
                op.SendInfoMessage("正在备份地图...");
                TShock.Utils.SaveWorld();
                TShock.Log.Info("[wm]世界已备份 (" + Main.worldPathName + ")");
                op.SendInfoMessage("世界已备份");
                Main.ActiveWorldFileData._path = worldPathName;
            }
            catch (Exception ex)
            {
                TShock.Log.Error("[wm]备份失败!");
                op.SendInfoMessage("备份地图失败！请手动查看日志文件");
                TShock.Log.Error(ex.ToString());
            }
        }
    }
}
