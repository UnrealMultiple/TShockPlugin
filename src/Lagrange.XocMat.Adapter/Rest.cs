using Lagrange.XocMat.Adapter.Attributes;
using Lagrange.XocMat.Adapter.Enumerates;
using Lagrange.XocMat.Adapter.Protocol.Internet;
using Rests;
using Terraria;
using TShockAPI;
using Item = Lagrange.XocMat.Adapter.Protocol.Internet.Item;

namespace Lagrange.XocMat.Adapter;

internal class Rest
{
    #region 查询进度api
    [RestMatch("/Progress")]
    public object GameProgress(RestRequestArgs args)
    {
        RestObject obj = new()
        {
            Status = "200",
            Response = "查询成功"
        };
        obj["data"] = Utils.GetGameProgress();
        return obj;
    }
    #endregion

    #region 查背包API
    [RestMatch("/beanInvsee")]
    private object BInvSee(RestRequestArgs args)
    {
        var playerName = args.Parameters["name"];
        var tsplayer = new Player();
        var players = TSPlayer.FindByNameOrID(playerName);
        if (players.Count != 0)
        {
            tsplayer = players[0].TPlayer;
        }
        else
        {
            var offline = TShock.UserAccounts.GetUserAccountByName(playerName);
            if (offline == null)
            {
                return new RestObject("201") { { "response", "无效用户" } };
            }
            playerName = offline.Name;
            var data = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), offline.ID);
            tsplayer = Utils.CreateAPlayer(playerName, data);
        }
        var retObject = new Protocol.Internet.PlayerData
        {
            //在线状态
            OnlineStatu = false,
            //玩家名称
            Username = playerName,
            //最大生命
            statLifeMax = tsplayer.statLifeMax,
            //当前生命
            statLife = tsplayer.statLife,
            //最大法力
            statManaMax = tsplayer.statManaMax,
            //当前法力
            statMana = tsplayer.statMana,
            //buff
            buffType = tsplayer.buffType,
            //buff 时间
            buffTime = tsplayer.buffTime,
            //背包
            inventory = Utils.GetInventoryData(tsplayer.inventory, NetItem.InventorySlots),
            //宠物坐骑的染料
            miscDye = Utils.GetInventoryData(tsplayer.miscDyes, NetItem.MiscDyeSlots),
            //宠物坐骑等
            miscEquip = Utils.GetInventoryData(tsplayer.miscEquips, NetItem.MiscEquipSlots),
            //套装
            Loadout = new Suits[tsplayer.Loadouts.Length]
        };
        for (int i = 0; i < tsplayer.Loadouts.Length; i++)
        {
            if (i == tsplayer.CurrentLoadoutIndex)
            {
                retObject.Loadout[i] = new Suits()
                {
                    armor = Utils.GetInventoryData(tsplayer.armor, NetItem.ArmorSlots),
                    dye = Utils.GetInventoryData(tsplayer.dye, NetItem.DyeSlots),
                };
            }
            else
            {
                retObject.Loadout[i] = new Suits()
                {
                    armor = Utils.GetInventoryData(tsplayer.Loadouts[i].Armor, tsplayer.Loadouts[i].Armor.Length),
                    dye = Utils.GetInventoryData(tsplayer.Loadouts[i].Dye, tsplayer.Loadouts[i].Dye.Length)
                };
            }
        }
        //垃圾桶
        retObject.trashItem =
        [
            new Item(tsplayer.trashItem.type, tsplayer.trashItem.prefix, tsplayer.trashItem.stack)
        ];
        //猪猪存钱罐
        retObject.Piggiy = Utils.GetInventoryData(tsplayer.bank.item, NetItem.PiggySlots);
        //保险箱
        retObject.safe = Utils.GetInventoryData(tsplayer.bank2.item, NetItem.SafeSlots);
        //护卫熔炉
        retObject.Forge = Utils.GetInventoryData(tsplayer.bank3.item, NetItem.ForgeSlots);
        //虚空保险箱
        retObject.VoidVault = Utils.GetInventoryData(tsplayer.bank4.item, NetItem.VoidSlots);

        return new RestObject()
        {
            { "response", "查询成功" },
            { "data", retObject }
        };
    }
    #endregion

    #region 在线排行API
    [RestMatch("/onlineDuration")]
    public object OnlineDuration(RestRequestArgs args)
    {
        var data = Plugin.Onlines.OrderByDescending(x => x.Value).Select(x => new { name = x.Key, duration = x.Value });
        return new RestObject()
        {
            { "response", "查询成功" },
            { "data", data }
        };
    }
    #endregion

    #region 死亡排行
    [RestMatch("/deathrank")]
    private object DeadRank(RestRequestArgs args)
    {
        var data = Plugin.Deaths
            .OrderByDescending(x => x.Value)
            .Select(x => new { Name = x.Key, Count = x.Value })
            .ToList();
        return new RestObject("200")
        {
            ["response"] = "获取成功",
            ["data"] = data
        };
    }
    #endregion

    #region 获取图片Base64
    [RestMatch("/generatemap")]
    public object GenerateMapApi(RestRequestArgs args)
    {
        try
        {
            var bytes = Utils.CreateMapBytes(ImageType.Png);
            return new RestObject("200") { Response = Convert.ToBase64String(bytes) };
        }
        catch (Exception ex)
        {
            return new RestObject("404") { Response = ex.Message };
        }
    }
    #endregion
}
