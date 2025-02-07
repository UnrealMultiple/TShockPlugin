using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;
using LazyAPI.Database;
using LinqToDB;
using TShockAPI;

namespace NoteWall.DB;

[Table("NoteWall")]
public class Note : RecordBase<Note>
{
    [PrimaryKey, Identity]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.Now;

    private const string TableName = "NoteWall";

    public static List<Note> GetNotesByUsername(string username)
    {
        using (var db = Db.Context<Note>(TableName))
        {
            return db.Records
                     .Where(n => n.Username.ToLower() == username.ToLower())
                     .ToList();
        }
    }

    public static Note? GetNoteById(int id)
    {
        using (var db = Db.Context<Note>(TableName))
        {
            return db.Records.FirstOrDefault(n => n.Id == id);
        }
    }

    public static List<Note> GetAllNotes()
    {
        using (var db = Db.Context<Note>(TableName))
        {
            return db.Records.ToList();
        }
    }

    public static Note? GetRandomNote()
    {
        using (var db = Db.Context<Note>(TableName))
        {
            var count = db.Records.Count();
            if (count == 0)
            {
                return null;
            }

            var rnd = new Random();
            return db.Records.Skip(rnd.Next(count)).FirstOrDefault();
        }
    }

    public static Note? AddNote(string username, string content)
    {
        using (var db = Db.Context<Note>(TableName))
        {
            var hasExceededLimit = db.Records
                .Any(n => n.Username.ToLower() == username.ToLower()
                    && db.Records.Count(n2 => n2.Username.ToLower() == username.ToLower()) >= 5);

            if (hasExceededLimit)
            {
                return null;
            }

            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    var note = new Note
                    {
                        Username = username,
                        Content = content,
                        Timestamp = DateTime.Now
                    };

                    var insertedId = db.InsertWithInt32Identity(note);
                    if (insertedId <= 0)
                    {
                        transaction.Rollback();
                        return null;
                    }

                    note.Id = insertedId;
                    transaction.Commit();
                    return note;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TShock.Log.ConsoleError($"留言添加失败: {ex.Message}");
                    return null;
                }
            }
        }
    }

    public static Note? DeleteNoteById(int id, TSPlayer player)
    {
        using (var db = Db.Context<Note>(TableName))
        {
            var query = db.Records.Where(n => n.Id == id);

            if (!player.HasPermission("notewall.admin"))
            {
                query = query.Where(n => n.Username.ToLower() == player.Name.ToLower());
            }

            var note = query.FirstOrDefault();
            if (note == null)
            {
                return null;
            }

            db.Delete(note);
            return note;
        }
    }

    public static bool UpdateNote(int id, string content, string username)
    {
        using (var db = Db.Context<Note>(TableName))
        {
            var note = db.Records.FirstOrDefault(n => n.Id == id && n.Username.ToLower() == username.ToLower());
            if (note == null)
            {
                return false;
            }

            var oldContent = note.Content;
            note.Content = content;
            note.Timestamp = DateTime.Now;
            db.Update(note);

            return true;
        }
    }


    public static int DeleteNotesByUsername(string username)
    {
        using (var db = Db.Context<Note>(TableName))
        {
            return db.Records.Delete(n => n.Username.ToLower() == username.ToLower());
        }
    }
}
