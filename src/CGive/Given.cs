using TShockAPI;
using TShockAPI.DB;

namespace CGive;

public class Given
{
    public string Name { get; set; } = "";

    public int Id { get; set; }

    public void Save()
    {
        Data.Command($"INSERT INTO Given(name) VALUES (@0)", this.Name);
    }

    public void Del()
    {
        Data.Command($"DELETE FROM Given WHERE name=@0 and id=@1", this.Name, this.Id);
    }

    public bool IsGiven()
    {
        var dB = TShock.DB;
        using var queryResult = dB.QueryReader($"SELECT * FROM Given WHERE name=@0 and id=@1", this.Name, this.Id);
        return  queryResult.Read();
    }
}