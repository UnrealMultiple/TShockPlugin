using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace CGive;

public class Given
{
    public string Name { get; set; }

    public int id { get; set; }

    public void Save()
    {
        Data.Command($"insert into Given(name,id)values(@0,@1)", this.Name, this.id);
    }

    public void Del()
    {
        Data.Command($"delete from Given where name=@0 and id=@1", this.Name, this.id);
    }

    public bool IsGiven()
    {
        var result = false;
        var dB = TShock.DB;
        using (var queryResult = dB.QueryReader($"select * from Given where name=@0 and id=@1", this.Name, this.id))
        {
            result = queryResult.Read();
        }
        return result;
    }
}
