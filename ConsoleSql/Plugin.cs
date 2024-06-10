using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace ConsoleSql
{
    [ApiVersion(2, 1)]
    public class ConsoleSql : TerrariaPlugin
    {
        //定义插件的作者名称
        public override string Author => "Cai";

        //插件的一句话描述
        public override string Description => "让控制台可以执行SQL操作数据库";

        //插件的名称
        public override string Name => "ConsoleSql";

        //插件的版本
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        //插件的构造器
        public ConsoleSql(Main game) : base(game)
        {
        }

        public static IDbConnection database => TShock.DB;
        //插件加载时执行的代码
        public override void Initialize()
        {
            //恋恋给出的模板代码中展示了如何为TShock添加一个指令
            Commands.ChatCommands.Add(new Command(
                permissions: new List<string> { "ConsoleSql.Use", },
                cmd: Cmd,
                "sql"));
        }

        //插件卸载时执行的代码

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //移除所有由本插件添加的所有指令

                var asm = Assembly.GetExecutingAssembly();
                Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method?.DeclaringType?.Assembly == asm);
            }
            base.Dispose(disposing);
        }

        //执行指令时对指令进行处理的方法
        private void Cmd(CommandArgs args)
        {
            if (args.Player.RealPlayer)
            {
                args.Player.SendErrorMessage("此命令仅支持控制台(BOT)执行!");
                return;
            }
            if (args.Parameters.Count == 0)
            {
                args.Player.SendErrorMessage("格式错误!正确格式: sql <SQL语句>;");
                args.Player.SendWarningMessage("常用SQL语句:");
                args.Player.SendWarningMessage("->Sqlite列出表格:SELECT name FROM sqlite_master WHERE type='table';");
                args.Player.SendWarningMessage("->Mysql列出表格:SHOW TABLES;");
                args.Player.SendWarningMessage("->清空表格:DROP TABLE <表格名字>;");
                args.Player.SendWarningMessage("->删除表格:DELETE FROM <表格名字>;");
                args.Player.SendWarningMessage("->删除记录:DELETE FROM <表格名字> WHERE <条件>;");
                args.Player.SendWarningMessage("->查询表格内容:SELECT * FROM <表格名字>;");
                args.Player.SendWarningMessage("->查询表格内容扩展:SELECT * FROM <表格名字> WHERE <条件 > LIMIT <返回行数>;");
                args.Player.SendWarningMessage("->修改数据表指定内容:UPDATE <表格名字> SET <更新列名> = '更新值' WHERE <条件>");
                args.Player.SendWarningMessage("*详细教程：https://www.runoob.com/sql/sql-tutorial.html");

            }
            else
            {
                try
                {
                    args.Player.SendInfoMessage(string.Join(" ", args.Parameters));
                    Stopwatch stopwatch = Stopwatch.StartNew(); // 开始计时
                    using (var reader = database.QueryReader(string.Join(" ", args.Parameters)))
                    {
                        
                        DataTable dt = new DataTable();
                        dt.Load(reader.Reader);
                        stopwatch.Stop(); // 停止计时
                        TimeSpan ts = stopwatch.Elapsed; // 获取经过的时间
                        StringBuilder sb = new StringBuilder();
                        if (dt.Columns.Count==0)
                        {
                            sb.AppendLine($"执行成功!");
                            sb.AppendLine($"影响{reader.Reader.RecordsAffected}行 ({ts.TotalSeconds.ToString("F2")}秒)");
                            args.Player.SendInfoMessage(sb.ToString());
                            return;
                        }


                        // 添加查询时间和查询到的条数
                        sb.Append("+");
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("----------------------------+");
                        }
                        sb.AppendLine();

                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.AppendFormat("| {0,-26} ", column.ColumnName);
                        }
                        sb.AppendLine("|");

                        sb.Append("+");
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("----------------------------+");
                        }
                        sb.AppendLine();

                        // 添加每一行
                        foreach (DataRow row in dt.Rows)
                        {
                            foreach (DataColumn column in dt.Columns)
                            {
                                sb.AppendFormat("| {0,-26} ", row[column]);
                            }
                            sb.AppendLine("|");

                            sb.Append("+");
                            foreach (DataColumn column in dt.Columns)
                            {
                                sb.Append("----------------------------+");
                            }
                            sb.AppendLine();
                        }
                        sb.AppendLine($"查询到{dt.Rows.Count}行 ({ts.TotalSeconds.ToString("F2")}秒)");
                        args.Player.SendInfoMessage(sb.ToString());
                    }

                }
                catch (Exception ex)
                {
                    args.Player.SendErrorMessage("SQL执行失败!\n" +
                        $"原因:{ex.Message}");
                }

            }
            
        }
    }
}