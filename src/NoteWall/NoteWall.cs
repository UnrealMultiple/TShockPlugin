using System;
using System.Linq;
using LazyAPI;
using TShockAPI;
using Terraria;
using NoteWall.DB;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;

namespace NoteWall;

[ApiVersion(2, 1)]
public class NoteWall : LazyPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "肝帝熙恩";
    public override Version Version => new Version(1, 0, 2);
    public override string Description => GetString("留言墙");

    public NoteWall(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("notewall.user.add", this.AddNote, "addnote"));
        Commands.ChatCommands.Add(new Command("notewall.user.view", this.ViewNote, "viewnote","vinote"));
        Commands.ChatCommands.Add(new Command("notewall.user.page", this.ViewNotesPage,"notewall"));
        Commands.ChatCommands.Add(new Command("notewall.user.random", this.RandomNote, "randomnote","rdnote"));
        Commands.ChatCommands.Add(new Command("notewall.user.update", this.UpdateNote, "updatenote","upnote"));
        Commands.ChatCommands.Add(new Command("notewall.admin.delete", this.DeleteNote, "deletenote","delnote"));
        Commands.ChatCommands.Add(new Command("notewall.user.my", this.MyNotes, "mynote"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.AddNote || x.CommandDelegate == this.ViewNote || x.CommandDelegate == this.ViewNotesPage || x.CommandDelegate == this.RandomNote || x.CommandDelegate == this.UpdateNote || x.CommandDelegate == this.DeleteNote || x.CommandDelegate == this.MyNotes);
        }
        base.Dispose(disposing);
    }

    private void AddNote(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("请输入留言内容！输入/notewall help 查看帮助"));
            return;
        }

        var content = string.Join(" ", args.Parameters);

        if (content.Length > Configuration.Instance.MaxNoteLength)
        {
            args.Player.SendErrorMessage(GetString($"留言内容不能超过 {Configuration.Instance.MaxNoteLength} 字！"));
            return;
        }

        if (Configuration.Instance.BannedWords.Any(bannedWord => content.Contains(bannedWord, StringComparison.OrdinalIgnoreCase)))
        {
            args.Player.SendErrorMessage(GetString("留言内容包含不允许的词汇！"));
            return;
        }

        var note = Note.AddNote(args.Player.Name, content);

        if (note != null)
        {
            args.Player.SendSuccessMessage(GetString($"留言成功！\n[{note.Id}] {note.Username}: {note.Content} (时间：{note.Timestamp})"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString($"你最多只能留言 {Configuration.Instance.MaxNotesPerPlayer} 条！"));
        }
    }

    private void UpdateNote(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendErrorMessage(GetString("请输入留言序号和新的留言内容！输入/notewall help 查看帮助"));
            return;
        }

        if (!int.TryParse(args.Parameters[0], out var id))
        {
            args.Player.SendErrorMessage(GetString("无效的留言序号！输入/notewall help 查看帮助"));
            return;
        }

        var newContent = string.Join(" ", args.Parameters.Skip(1));

        if (newContent.Length > Configuration.Instance.MaxNoteLength)
        {
            args.Player.SendErrorMessage(GetString($"修改后的留言内容不能超过 {Configuration.Instance.MaxNoteLength} 字！"));
            return;
        }

        if (Configuration.Instance.BannedWords.Any(bannedWord => newContent.Contains(bannedWord, StringComparison.OrdinalIgnoreCase)))
        {
            args.Player.SendErrorMessage(GetString("修改后的留言内容包含不允许的词汇！"));
            return;
        }

        var note = Note.GetNoteById(id);

        if (note != null && Note.UpdateNote(id, newContent, args.Player.Name))
        {
            args.Player.SendSuccessMessage(GetString($"留言修改成功！\n修改前：{note.Content}\n修改后：{newContent}（留言时间已更新）"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("只能修改你自己的留言，或留言不存在！输入/notewall help 查看帮助"));
        }
    }



    private void DeleteNote(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("请输入留言序号或用户名！输入/notewall help 查看帮助"));
            return;
        }

        var param = args.Parameters[0];
        if (int.TryParse(param, out var id))
        {
            var note = Note.DeleteNoteById(id);
            if (note != null)
            {
                args.Player.SendSuccessMessage(GetString($"管理员删除留言 [{note.Id}] - {note.Username}: {note.Content}"));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("留言不存在！"));
            }
        }
        else
        {
            var deletedCount = Note.DeleteNotesByUsername(param);
            if (deletedCount > 0)
            {
                args.Player.SendSuccessMessage(GetString($"成功删除 {deletedCount} 条 {param} 的留言！"));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("该玩家没有留言或删除失败！"));
            }
        }
    }

    private void ViewNote(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("请输入留言序号或用户名！输入/notewall help 查看帮助"));
            return;
        }

        var param = args.Parameters[0];
        if (int.TryParse(param, out var id))
        {
            var note = Note.GetNoteById(id);
            if (note != null)
            {
                args.Player.SendInfoMessage(GetString($"留言 [{note.Id}] - {note.Username}: {note.Content} (时间：{note.Timestamp})"));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("找不到该留言！"));
            }
        }
        else
        {
            var notes = Note.GetNotesByUsername(param);
            if (notes.Count > 0)
            {
                foreach (var note in notes)
                {
                    args.Player.SendInfoMessage(GetString($"留言 [{note.Id}] - {note.Username}: {note.Content} (时间：{note.Timestamp})"));
                }
            }
            else
            {
                args.Player.SendErrorMessage(GetString("该玩家没有留言！"));
            }
        }
    }

    private void ViewNotesPage(CommandArgs args)
    {
        // 如果输入 "help" 显示帮助信息
        if (args.Parameters.Count > 0 && args.Parameters[0].ToLower() == "help")
        {
            args.Player.SendInfoMessage(GetString("===== 留言墙 插件指令 ====="));
            args.Player.SendInfoMessage(GetString("/addnote <内容>  留下留言"));
            args.Player.SendInfoMessage(GetString("/vinote <序号/玩家名字>  查看某个留言"));
            args.Player.SendInfoMessage(GetString("/notewall <页码/help> 查看留言墙"));
            args.Player.SendInfoMessage(GetString("/rdnote  查看一条随机留言"));
            args.Player.SendInfoMessage(GetString("/upnote <序号> <新内容> 修改你自己的留言"));
            args.Player.SendInfoMessage(GetString("/delnote <序号/玩家名字>  删除留言"));
            args.Player.SendInfoMessage(GetString("/mynote  查看你自己的留言"));
            return;
        }

        // 如果参数不为 "help"，继续分页显示留言
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("请输入页码！例如：/notewall 1"));
            return;
        }

        if (!int.TryParse(args.Parameters[0], out var page))
        {
            args.Player.SendErrorMessage(GetString("请输入有效的页码！输入/notewall help 查看帮助"));
            return;
        }

        var allNotes = Note.GetAllNotes();
        var totalNotes = allNotes.Count;
        var notesPerPage = 10;
        var totalPages = (int) Math.Ceiling((double) totalNotes / notesPerPage);

        if (page <= 0 || page > totalPages)
        {
            args.Player.SendErrorMessage(GetString($"页码无效！共有 {totalPages} 页。"));
            return;
        }

        var skip = (page - 1) * notesPerPage;
        var paginatedNotes = allNotes.Skip(skip).Take(notesPerPage).ToList();

        if (paginatedNotes.Count > 0)
        {
            args.Player.SendInfoMessage(GetString("[i:531]========== 留言墙 ==========[i:531]"));
            args.Player.SendMessage(GetString($"第 {page} 页，共 {totalPages} 页"), Color.Orange);
            foreach (var note in paginatedNotes)
            {
                args.Player.SendInfoMessage(GetString($"[{note.Id}] {note.Username}: {note.Content} (时间：{note.Timestamp})"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("没有更多留言了！"));
        }
    }


    private void RandomNote(CommandArgs args)
    {
        var note = Note.GetRandomNote();
        if (note != null)
        {
            args.Player.SendInfoMessage(GetString($"随机留言 [{note.Id}] - {note.Username}: {note.Content} (时间：{note.Timestamp})"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("目前没有留言！输入/notewall help 查看帮助"));
        }
    }

    private void MyNotes(CommandArgs args)
    {
        var notes = Note.GetNotesByUsername(args.Player.Name);
        if (notes.Count > 0)
        {
            foreach (var note in notes)
            {
                args.Player.SendInfoMessage(GetString($"留言 [{note.Id}] - {note.Username}: {note.Content} (时间：{note.Timestamp})"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("你还没有留言！输入/notewall help 查看帮助"));
        }
    }
}
