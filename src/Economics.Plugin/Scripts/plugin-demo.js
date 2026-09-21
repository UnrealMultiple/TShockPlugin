var TShockAPI = importNamespace("TShockAPI");

var cmdHello, cmdItem, cmdPing;

function init() {
    cmdHello = command('jhello', '', '打个招呼: /jhello <名字>', onHello);
    cmdItem  = command('jitem',  '', '给自己一个物品: /jitem <物品ID> [数量]', onGiveItem);
    cmdPing  = command('jping',  '', '查看在线人数: /jping', onPing);
    if (cmdHello) Commands.Add(cmdHello);
    if (cmdItem)  Commands.Add(cmdItem);
    if (cmdPing)  Commands.Add(cmdPing);

    // 订阅 TSAPI 事件（正确方式）
    ServerHooks.ServerJoin.Register(Plugin, onServerJoin);
    ServerHooks.ServerLeave.Register(Plugin, onServerLeave);
    ServerHooks.ServerChat.Register(Plugin, onServerChat);
    ServerHooks.NpcKilled.Register(Plugin, onNpcKilled);
    ServerHooks.NetGetData.Register(Plugin, onNetGetData);
}

function unload() {
    // 反注册自己 Add 的命令与钩子（宿主不追踪，必须在这里清理）
    if (cmdHello) Commands.Remove(cmdHello);
    if (cmdItem)  Commands.Remove(cmdItem);
    if (cmdPing)  Commands.Remove(cmdPing);

    ServerHooks.ServerJoin.Deregister(Plugin, onServerJoin);
    ServerHooks.ServerLeave.Deregister(Plugin, onServerLeave);
    ServerHooks.ServerChat.Deregister(Plugin, onServerChat);
    ServerHooks.NpcKilled.Deregister(Plugin, onNpcKilled);
    ServerHooks.NetGetData.Deregister(Plugin, onNetGetData);

    log('[demo] unload: 命令与钩子已反注册');
}

// ---------- 命令回调：收到一个 args 对象（友好值） ----------
function onHello(args) {
    var who = args.Parameters.length > 0 ? args.Parameters[0] : args.PlayerName;
    sendMessage(args.PlayerName, '你好, ' + who + ' !(来自 JS 脚本)');
}

function onGiveItem(args) {
    if (args.Parameters.length < 1) {
        sendError(args.PlayerName, '用法: /jitem <物品ID> [数量]');
        return;
    }
    var id = parseInt(args.Parameters[0], 10);
    var count = args.Parameters.length > 1 ? parseInt(args.Parameters[1], 10) : 1;
    if (isNaN(id)) {
        sendError(args.PlayerName, '物品ID 必须为数字');
        return;
    }
    giveItem(args.PlayerName, id, count);
    sendMessage(args.PlayerName, '已给你 ' + count + ' 个物品 (ID=' + id + ')');
}

function onPing(args) {
    sendMessage(args.PlayerName, '当前在线: ' + getOnlinePlayers());
}

// ---------- TSAPI 事件钩子回调：参数是 TSAPI 的事件参数对象 ----------
function onServerJoin(args) {
    broadcast(playerName(args.Who) + ' 加入了服务器!(来自 JS)');
}

function onServerLeave(args) {
    log('[demo] ' + playerName(args.Who) + ' 离开了服务器');
}

function onServerChat(args) {
    if (args.Text && args.Text.indexOf('hello') >= 0) {
        var who = playerName(args.Who);
        sendMessage(who, '你好呀 ' + who + '!');
    }
}

function onNpcKilled(args) {
    log('[demo] 击杀了 ' + args.npc.FullName);
}

function onNetGetData(args) {
    log('[demo] 收包: 类型=' + args.MsgID + ' 玩家=' + TShockAPI.TShock.Players[args.Msg.whoAmI].Name + ' 长度=' + args.Length);
}
