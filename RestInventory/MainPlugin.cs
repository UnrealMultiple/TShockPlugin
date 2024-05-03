using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Rests;

namespace RestInventory
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        
        public MainPlugin(Main game) : base(game){}
        public override string Name => "BeanInvsee";
        public override Version Version => new Version(1, 0, 0, 1);
        public override string Author => "少司命";
        public override string Description => "rest查询背包";
        public RetObject retObject = new RetObject();
        public override void Initialize()
        {
            TShock.RestApi.Register(new SecureRestCommand("/beanInvsee", BInvSee, "beanInvsee.use"));
        }


        private object BInvSee(RestRequestArgs args)
        {
            string playerName = args.Parameters["name"];
            var tsplayer = new Player();
            var players = TSPlayer.FindByNameOrID(playerName);
            if (players.Count != 0)
            {
                tsplayer = players[0].TPlayer;
            }
            else 
            {
                var offline = TShock.UserAccounts.GetUserAccountByName(playerName);
                if ( offline == null)
                {
                    return new RestObject("201") { { "response", "无效用户" } };
                }
                playerName = offline.Name;
                var data = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), offline.ID);
                tsplayer = Utils.ModifyData(playerName, data);
            }
            //在线状态
            retObject.OnlineStatu = false;
            //玩家名称
            retObject.Username = playerName;
            //最大生命
            retObject.statLifeMax = tsplayer.statLifeMax;
            //当前生命
            retObject.statLife = tsplayer.statLife;
            //最大法力
            retObject.statManaMax = tsplayer.statManaMax;
            //当前法力
            retObject.statMana = tsplayer.statMana;
            //buff
            retObject.buffType = tsplayer.buffType;
            //buff 时间
            retObject.buffTime = tsplayer.buffTime;
            //背包
            retObject.inventory = Utils.GetInventoryData(tsplayer.inventory, NetItem.InventorySlots);
            //宠物坐骑的染料
            retObject.miscDye = Utils.GetInventoryData(tsplayer.miscDyes, NetItem.MiscDyeSlots);
            //宠物坐骑等
            retObject.miscEquip = Utils.GetInventoryData(tsplayer.miscEquips, NetItem.MiscEquipSlots);
            //套装
            retObject.Loadout = new Suits[tsplayer.Loadouts.Length];
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
            retObject.trashItem = new InventoryData[1] { new InventoryData(tsplayer.trashItem.netID, tsplayer.trashItem.prefix, tsplayer.trashItem.stack) };
            //猪猪存钱罐
            retObject.Piggiy = Utils.GetInventoryData(tsplayer.bank.item, NetItem.PiggySlots);
            //保险箱
            retObject.safe = Utils.GetInventoryData(tsplayer.bank2.item, NetItem.SafeSlots);
            //护卫熔炉
            retObject.Forge = Utils.GetInventoryData(tsplayer.bank3.item, NetItem.ForgeSlots);
            //虚空保险箱
            retObject.VoidVault = Utils.GetInventoryData(tsplayer.bank4.item, NetItem.VoidSlots);

            return new RestObject() { { "response", "查询成功" }, { "data", retObject } };
        }

       

        protected override void Dispose(bool disposing)
        {



        }
    }
}
