namespace EssentialsPlus.Db;

public class Home
{
    public string Name { get; private set; }
    public int UserID { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }

    public Home(int userId, string name, float x, float y)
    {
        this.Name = name;
        this.UserID = userId;
        this.X = x;
        this.Y = y;
    }
}