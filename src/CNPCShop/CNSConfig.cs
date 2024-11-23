using Newtonsoft.Json;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace CNPCShop;

public class CNSConfig
{
    [JsonProperty("更新间隔")]
    public int UpdateTime = 500;
    [JsonProperty("总列表")]
    public List<ShopContainer> Shops = new List<ShopContainer>();
    public static void Load()
    {
        CNSPlugin.AviliableShops.Clear();
        var path = Path.Combine(TShock.SavePath, "CNPCShop.json");
        if (!File.Exists(path))
        {
            CNSPlugin.Config = new CNSConfig()
            {
                Shops = new List<ShopContainer>()
                {
                    new ShopContainer()
                    {
                        Enabled = true,
                        WorldID = -1,
                        Shops = new Shop[]
                        {
                            new Shop()
                            {
                                Enabled = true,
                                Groups = new List<string>(){ "guest", "default", "superadmin"},
                                OpenMessage = new List<string>(){ "商人: 想买点啥?", "商人: 大甩卖!", "商人: 你好啊 {name}"},
                                CloseMessage = new List<string>(){ "商人: 欢迎再来" },
                                NPC = NPCID.Merchant,
                                Items = new CNSItem[]
                                {
                                    new CNSItem()
                                    {
                                        NetID = ItemID.Torch,
                                        Stack = 100,
                                        Prefix = 0,
                                        Price = new Price()
                                        {
                                            Copper = 99,
                                            Silver = 3,
                                            Gold = 0,
                                            Platinum = 0
                                        }
                                    },
                                    new CNSItem()
                                    {
                                        NetID = ItemID.LesserHealingPotion,
                                        Prefix = 0,
                                        Stack = 10,
                                        Price = new Price()
                                        {
                                            Copper = 99,
                                            Silver = 12,
                                            Gold = 0,
                                            Platinum = 0
                                        }
                                    },
                                    new CNSItem()
                                    {
                                        NetID = ItemID.IronskinPotion,
                                        Stack = 10,
                                        Prefix = 0,
                                        Price = new Price()
                                        {
                                            Copper = 99,
                                            Silver = 49,
                                            Gold = 0,
                                            Platinum = 0
                                        }
                                    },
                                    new CNSItem()
                                    {
                                        NetID = ItemID.RecallPotion,
                                        Stack = 10,
                                        Prefix = 0,
                                        Price = new Price()
                                        {
                                            Copper = 99,
                                            Silver = 99,
                                            Gold = 0,
                                            Platinum = 0
                                        }
                                    }
                                }
                            },
                            new Shop()
                            {
                                Enabled = false,
                                NPC = NPCID.Merchant,
                                Groups = new List<string>(){ "" },
                                OpenMessage = new List<string>(){ "" },
                                CloseMessage = new List<string>(){ "" },
                                Items = new CNSItem[]
                                {
                                    new CNSItem()
                                    {
                                        NetID = ItemID.IronPickaxe,
                                        Stack = 1,
                                        Prefix = 0,
                                        Price = new Price()
                                        {
                                            Copper = 99,
                                            Silver = 9,
                                            Gold = 0,
                                            Platinum = 0
                                        }
                                    },
                                    new CNSItem()
                                    {
                                        NetID = ItemID.DirtBlock,
                                        Stack = 999,
                                        Prefix = 0,
                                        Price = new Price()
                                        {
                                            Copper = 5,
                                            Silver = 0,
                                            Gold = 0,
                                            Platinum = 0
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            File.WriteAllText(path, JsonConvert.SerializeObject(CNSPlugin.Config, Formatting.Indented));
        }
        else
        {
            CNSPlugin.Config = JsonConvert.DeserializeObject<CNSConfig>(File.ReadAllText(path))!;
        }

        foreach (var shopContainer in CNSPlugin.Config.Shops)
        {
            if ((shopContainer.WorldID != Main.worldID && shopContainer.WorldID != -1) || !shopContainer.Enabled)
            {
                continue;
            }

            for (var s = 0; s < shopContainer.Shops.Length; s++)
            {
                var shop = shopContainer.Shops[s];
                if (!shop.Enabled)
                {
                    continue;
                }

                shop.RawData = new byte[40][];
                var count = (byte) shop.Items.Length;
                for (byte i = 0; i < count; i++)
                {
                    if (i >= 40)
                    {
                        break;
                    }

                    var item = shop.Items[i];
                    var idBytes = BitConverter.GetBytes(item.NetID);
                    var stackBytes = BitConverter.GetBytes(item.Stack);
                    var priceBytes = BitConverter.GetBytes(Item.buyPrice(item.Price.Platinum,
                        item.Price.Gold, item.Price.Silver, item.Price.Copper));
                    shop.RawData[i] = new byte[]
                    {
                        14, 0, 104, i, idBytes[0], idBytes[1], stackBytes[0], stackBytes[1], shop.Items[i].Prefix,
                        priceBytes[0], priceBytes[1], priceBytes[2], priceBytes[3], 0
                    };
                }
                for (var i = count; i < 40; i++)
                {
                    shop.RawData[i] = new byte[] { 14, 0, 104, i, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                }

                CNSPlugin.AviliableShops.Add(shop);
            }
        }
    }
    #region ShopContainer

    public struct ShopContainer
    {
        [JsonProperty("启用")]
        public bool Enabled;
        [JsonProperty("世界ID[-1则不限制世界]")]
        public int WorldID;
        [JsonProperty("商店列表")]
        public Shop[] Shops;
    }

    #endregion
    #region Shop

    public struct Shop
    {
        [JsonProperty("启用")]
        public bool Enabled;
        [JsonProperty("用户组[留空则允许所有]")]
        public List<string> Groups;
        [JsonProperty("进入消息")]
        public List<string> OpenMessage;
        [JsonProperty("关闭消息")]
        public List<string> CloseMessage;
        [JsonProperty("NPCID")]
        public int NPC;
        [JsonProperty("商品")]
        public CNSItem[] Items;
        [JsonIgnore]
        public byte[][] RawData;
    }

    #endregion
    #region CNSItem

    public struct CNSItem
    {
        [JsonProperty("物品ID")]
        public short NetID;
        [JsonProperty("堆叠")]
        public int Stack;
        [JsonProperty("前缀")]
        public byte Prefix;
        [JsonProperty("价格")]
        public Price Price;
    }

    #endregion
    #region Price

    public struct Price
    {
        [JsonProperty("铜")]
        public byte Copper;
        [JsonProperty("银")]
        public byte Silver;
        [JsonProperty("金")]
        public byte Gold;
        [JsonProperty("铂")]
        public short Platinum;
    }

    #endregion
}