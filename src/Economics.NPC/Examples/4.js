// 克苏鲁的右眼 (netID 4) - 扇形参数可配置，格式化清晰版
var TShock = importNamespace("TShockAPI");

// ========== 基础配置 ==========
var BASE_LIFE          = 15000;
var PER_PLAYER         = 1641;
var AURA_RADIUS        = 100;
var AURA_BUFFS         = [22, 33];
var DEATH_SPAWN_ID     = 181;
var DEATH_SPAWN_COUNT  = 10;

// ========== 定时弹幕配置 ==========
// 每个弹幕可配置 count（弹幕数量）和 spread（总散射角度，弧度），
// 若不指定则默认为单发（count=1, spread=0）
var EVENTS = [
    {
        interval: 15,
        delay: 0,
        chat: "哼，感受下克苏鲁军团的威力吧",
        shots: [
            { t: 465, d: 35 }
        ]
    },
    {
        interval: 3,
        delay: 0,
        chat: "",
        shots: [
            { t: 96, d: 3, lr: 100, ls: 8, count: 3, spread: Math.PI / 12 }   // 扇形3发，总角15°
        ]
    },
    {
        interval: 2,
        delay: 0,
        chat: "",
        shots: [
            { t: 44, d: 35, lr: 200, ls: 3.5, count: 5, spread: Math.PI / 12 } // 扇形5发，总角15°
        ]
    }
];

// ========== 血量事件配置 ==========
var HP_EVENT_THRESHOLD = 0.5;
var HP_EVENT_CHAT      = "黑暗的仆人啊，助我杀敌!";
var HP_EVENT_SHOTS = [
    { t: 465, d: 35, vx:  3, vy:  0 },
    { t: 465, d: 35, vx: -3, vy:  0 },
    { t: 465, d: 35, vx:  0, vy: -3 },
    { t: 465, d: 35, vx:  0, vy:  3 }
];

var state = {};   // 每个 NPC 实例的状态


// 计算朝向目标的速度向量
function getAim(npc, target, speed) {
    var dx = target.TPlayer.Center.X - npc.Center.X;
    var dy = target.TPlayer.Center.Y - npc.Center.Y;
    var len = Math.sqrt(dx * dx + dy * dy) || 0.001;
    return { x: dx / len * speed, y: dy / len * speed };
}

// 查找范围内最近的玩家
function findTarget(npc, rangeTiles) {
    var rangePx = rangeTiles * 16;
    var best = null;
    var bestDist = rangePx * rangePx;

    for (var i = 0; i < TShock.TShock.Players.length; i++) {
        var p = TShock.TShock.Players[i];
        if (!p || !p.active) continue;

        var dx = p.TPlayer.Center.X - npc.Center.X;
        var dy = p.TPlayer.Center.Y - npc.Center.Y;
        var dist = dx * dx + dy * dy;

        if (dist <= bestDist) {
            bestDist = dist;
            best = p;
        }
    }
    return best;
}

// 发射单发或扇形弹幕（依据 sh 中的 count / spread）
function fireSpread(npc, index, sh, target) {
    var base = target ? getAim(npc, target, sh.ls) : { x: sh.vx || 0, y: sh.vy || 0 };
    var count = sh.count || 1;
    var spread = sh.spread || 0;

    var baseAngle = Math.atan2(base.y, base.x);
    var speed = Math.sqrt(base.x * base.x + base.y * base.y);
    var step = count > 1 ? spread / (count - 1) : 0;

    for (var i = 0; i < count; i++) {
        var angle = baseAngle - spread / 2 + i * step;
        var vx = Math.cos(angle) * speed;
        var vy = Math.sin(angle) * speed;

        SpawnProjectile(
            index,
            npc.Center.X + (sh.ox || 0),
            npc.Center.Y + (sh.oy || 0),
            vx, vy,
            sh.t,
            sh.d || 30,
            5
        );
    }
}

// ========== 怪物生命周期函数 ==========

function onSpawn(npc) {
    var players = ActivePlayerCount();
    var life = BASE_LIFE + PER_PLAYER * players;

    if (life > npc.lifeMax) {
        npc.lifeMax = life;
        npc.life = life;
        npc.netUpdate = true;
    }

    Say("克苏鲁的右眼睁开了！(血量 " + npc.lifeMax + ")");
}

function ai(npc, index, time, struck) {
    // ---------- 光环效果 ----------
    for (var i = 0; i < TShock.TShock.Players.length; i++) {
        var p = TShock.TShock.Players[i];
        if (!p || !p.active) continue;

        var dx = p.TPlayer.Center.X - npc.Center.X;
        var dy = p.TPlayer.Center.Y - npc.Center.Y;

        if (dx * dx + dy * dy <= AURA_RADIUS * AURA_RADIUS) {
            for (var b = 0; b < AURA_BUFFS.length; b++) {
                p.SetBuff(AURA_BUFFS[b], 60, false);
            }
        }
    }

    // ---------- 定时弹幕 ----------
    var st = state[index] || (state[index] = { next: [], hpFired: false });

    for (var e = 0; e < EVENTS.length; e++) {
        var cfg = EVENTS[e];
        var next = st.next[e];
        if (next === undefined) next = cfg.delay;

        if (time >= next) {
            st.next[e] = time + cfg.interval;

            if (cfg.chat) Broadcast(cfg.chat, 255, 255, 255);

            for (var s = 0; s < cfg.shots.length; s++) {
                var sh = cfg.shots[s];

                // 锁定弹幕（带有 lr 和 ls）
                if (sh.lr && sh.ls) {
                    var target = findTarget(npc, sh.lr);
                    if (target) {
                        fireSpread(npc, index, sh, target);
                    } else {
                        // 无目标时按默认方向单发
                        SpawnProjectile(
                            index,
                            npc.Center.X + (sh.ox || 0),
                            npc.Center.Y + (sh.oy || 0),
                            sh.vx || 0, sh.vy || 0,
                            sh.t,
                            sh.d || 30,
                            5
                        );
                    }
                } else {
                    // 非锁定弹幕直接发射
                    SpawnProjectile(
                        index,
                        npc.Center.X + (sh.ox || 0),
                        npc.Center.Y + (sh.oy || 0),
                        sh.vx || 0, sh.vy || 0,
                        sh.t,
                        sh.d || 30,
                        5
                    );
                }
            }
        }
    }

    // ---------- 血量事件（50% 血量） ----------
    if (!st.hpFired && npc.life / npc.lifeMax <= HP_EVENT_THRESHOLD) {
        st.hpFired = true;
        Broadcast(HP_EVENT_CHAT, 255, 255, 255);

        for (var h = 0; h < HP_EVENT_SHOTS.length; h++) {
            var hp = HP_EVENT_SHOTS[h];
            SpawnProjectile(
                index,
                npc.Center.X,
                npc.Center.Y,
                hp.vx || 0, hp.vy || 0,
                hp.t,
                hp.d || 30,
                5
            );
        }
    }
}

function onKill(npc) {
    // 死亡召唤
    for (var j = 0; j < DEATH_SPAWN_COUNT; j++) {
        SpawnNpc(
            DEATH_SPAWN_ID,
            npc.Center.X + (j % 5 - 2) * 30,
            npc.Center.Y + (j % 3 - 1) * 30
        );
    }

    delete state[npc.whoAmI];
    Broadcast("克苏鲁的右眼陨落了！", 255, 255, 255);
}