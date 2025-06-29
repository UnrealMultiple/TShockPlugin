using System.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace TrCDK;

public class CDK
{
    public string Cdkname { get; set; } = "";
    public int Usetime { get; set; }
    public long Utiltime { get; set; }
    public string Grouplimit { get; set; } = "";
		public string Playerlimit { get; set; } = "";
		public string Used { get; set; } = "";
		public string Cmds { get; set; } = "";
	}
internal class Data
{
    public static SqliteConnection? DB;
    const string path = "tshock/TrCDK.sqlite";
    public static void Init()
    {
        DB = new SqliteConnection($"Data Source={path};");
        DB.Open();
        Command("create table if not exists Data(CDKname text,Usetime int(32),Utiltime int(64),Grouplimit text,Playerlimit text,Used text, Cmds text)");
    }
    public static SqliteDataReader Command(string cmd)
    {
        return new SqliteCommand(cmd, DB).ExecuteReader();
    }
    public static bool Insert(string CDKname, int Usetime, long Utiltime, string Grouplimit, string Playerlimit, string Cmds)
    {
			using (var reader = Command($"select * from Data where CDKname='{CDKname}'"))
			{
				while (reader.Read())
				{
					if (reader.GetString(0) == CDKname)
                {
                    return false;
                }
            }
			}
			Command($"insert into Data(CDKname,Usetime,Utiltime,Grouplimit,Playerlimit,Used,Cmds)values('{CDKname}','{Usetime}','{Utiltime}','{Grouplimit}','{Playerlimit}','','{Cmds}')");
			return true;
    }
		public static void Update(string CDKname, int Usetime, long Utiltime, string Grouplimit, string Playerlimit, string Used, string Cmds)
		{
			using (var reader = Command($"select * from Data where CDKname='{CDKname}'"))
			{
				while (reader.Read())
				{
					if (reader.GetString(0) == CDKname)
                {
                    Command($"UPDATE Data SET Usetime='{Usetime}',Utiltime='{Utiltime}',Grouplimit='{Grouplimit}',Playerlimit='{Playerlimit}',Used='{Used}',Cmds='{Cmds}' WHERE CDKname='{CDKname}'");
                }
            }
			}
		}
		public static CDK GetData(string name)
    {
			var Res = new CDK();
        using(var reader=Command($"select * from Data where CDKname='{name}'"))
        {
            while (reader.Read()) 
            {
                Res = new CDK(){ Cdkname = reader.GetString(0), Usetime = reader.GetInt32(1), Utiltime = reader.GetInt64(2), Grouplimit = reader.GetString(3), Playerlimit = reader.GetString(4), Used = reader.GetString(5), Cmds = reader.GetString(6)};

				}
        }
        return Res;
    }
		public static CDK[] GetAllData()
		{
			var Res = new List<CDK>();
			using (var reader = Command("select * from Data"))
			{
				while (reader.Read())
				{
					Res.Add (new CDK() { Cdkname = reader.GetString(0), Usetime = reader.GetInt32(1), Utiltime = reader.GetInt64(2), Grouplimit = reader.GetString(3), Playerlimit = reader.GetString(4), Used = reader.GetString(5), Cmds = reader.GetString(6) });

				}
			}
			return Res.ToArray();
		}
		public static bool DelCDK(string name)
		{
			Command($"DELETE FROM Data where CDKname='{name}'");
			return true;
		}
	}
