using LinqToDB;
using LinqToDB.Mapping;
using Newtonsoft.Json;

namespace CaiBotLite.Moulds;

[Table("cai_mail")]
public class Mail
{
    [PrimaryKey] [Identity] [Column("id")] public int Id;
    [Column("account_name")] public string AccountName = null!;

    [Column("name")] public string Name = "";

    [Column("hasReceived")] public bool HasReceived;

    [Column("is_command")] public bool IsCommand;
    
    

    [Column("commands", DataType = DataType.NVarChar)]
    [ValueConverter(ConverterType = typeof(JsonConverter<List<string>>))]
    public List<string> Commands = [];
    
    [Column("items", DataType = DataType.NVarChar)]
    [ValueConverter(ConverterType = typeof(JsonConverter<List<MailItem>>))]
    public List<MailItem> Items = [];
    
    public static List<Mail> GetByPlayerName(string name)
    {
        using var db = Database.Db;

        var mails = db
            .GetTable<Mail>()
            .Where(c => c.AccountName == name)
            .ToList();
        return mails;
    }

    public void CreatOrUpdate()
    {
        using var db = Database.Db;

        db.InsertOrReplace(this);
    }

    public static void CleanAll()
    {
        using var db = Database.Db;
        db.GetTable<Mail>().Delete();
    }
    
    
}

[Serializable]
public class MailItem
{
    [JsonProperty("type")]
    public int Type;
    [JsonProperty("stack")]
    public int Stack;
    [JsonProperty("prefix")]
    public int Prefix;
}