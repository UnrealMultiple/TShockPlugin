using TShockAPI;

namespace CGive;

public class Given
{
    public string Name { get; set; } = "";

    public int Id { get; set; }

    public void Save()
    {
        Data.Command("INSERT INTO Given(name, id) VALUES (@0, @1)", this.Name, this.Id);
    }

    public void Del()
    {
        Data.Command("DELETE FROM Given WHERE name=@0 AND id=@1", this.Name, this.Id);
    }

    public bool IsGiven()
    {
        using var queryResult = Data.QueryReader("SELECT * FROM Given WHERE name=@0 AND id=@1", this.Name, this.Id);
        return queryResult.Read();
    }
}