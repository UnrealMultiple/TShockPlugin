using System.Data;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.Task;

public class PlayerFinishTask
{
    public int TaskID { get; set; }

    public string Name { get; set; }

    public TaskStatus Status { get; set; }

    public PlayerFinishTask(int id, string name, TaskStatus status = TaskStatus.Success)
    {
        TaskID = id;
        Name = name;
        Status = status;
    }
}

public class AccountException : Exception
{
    public AccountException(string message)
        : base(message) { }
}

public class TaskFinishManager
{
    private readonly IDbConnection database;

    public List<PlayerFinishTask> Tasks;

    public TaskFinishManager()
    {
        database = TShock.DB;
        var table = new SqlTable("Task",
            new SqlColumn("TaskID", MySqlDbType.Int32) { Unique = true },
            new SqlColumn("Name", MySqlDbType.VarChar) { Unique = true, Length = 100 },
            new SqlColumn("Status", MySqlDbType.Int32)
            );

        var List = new SqlTableCreator(database, database.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(table);
        Tasks = GetPlayerTasks();
    }

    private List<PlayerFinishTask> GetPlayerTasks()
    {
        List<PlayerFinishTask> tasks = new();
        using var read = database.QueryReader("select * from `Task`");
        while (read.Read())
        {
            var taskid = read.Get<int>("TaskID");
            var Name = read.Get<string>("Name");
            var status = (TaskStatus)read.Get<int>("Status");
            tasks.Add(new(taskid, Name, status));
            if (status == TaskStatus.Ongoing)
                UserTaskData.Add(Name, taskid);
        }
        return tasks;
    }


    public bool HasFinishTask(int ID, string Name, TaskStatus status = TaskStatus.Success)
    {
        return Tasks.Any(x => x.TaskID == ID && x.Name == Name && x.Status == status);
    }

    public void Add(int id, string name, TaskStatus status)
    {
        database.Query("INSERT INTO `Task` (`TaskID`, `Name`, `Status`) VALUES (@0, @1, @2)", id, name, status);
        Tasks.Add(new PlayerFinishTask(id, name, status));
    }

    public void Update(int id, string name, TaskStatus status)
    {
        var task = Tasks.Find(x => x.TaskID == id && x.Name == name);
        if (task != null)
        {
            database.Query("UPDATE `Task` SET `Status` = @0 WHERE `Task`.`TaskID` = @1 AND `Task`.`Name` = @2", status, id, name);
            task.Status = status;
        }
    }


    public List<PlayerFinishTask> GetTaksByName(string name, TaskStatus status = TaskStatus.Success)
    {
        return Tasks.FindAll(x => x.Name == name && x.Status == status);
    }

    public List<PlayerFinishTask> GetTasksByID(int id, TaskStatus status = TaskStatus.Success)
    {
        return Tasks.FindAll(x => x.TaskID == id && x.Status == status);
    }

    public void RemoveAll()
    {
        if (database.GetSqlType() == SqlType.Sqlite)
        {
            database.Query("delete from Task");
        }
        else
        {
            database.Query("TRUNCATE Table Task");
        }
        Tasks.Clear();
    }

    public void Remove(int id, string name)
    {

        database.Query("DELETE FROM `Task` WHERE `Task`.`TaskID` = @0 AND `Task`.`Name` = @1", id, name);
        Tasks.RemoveAll(f => f.TaskID == id && f.Name == name);
    }
}
