using Lagrange.XocMat.Adapter.Attributes;
using Lagrange.XocMat.Adapter.Enumerates;
using Lagrange.XocMat.Adapter.Extension;
using Lagrange.XocMat.Adapter.Model.Action.Receive;
using Lagrange.XocMat.Adapter.Model.Internet;
using ProtoBuf;
using Rests;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.IO;
using Terraria.Map;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace Lagrange.XocMat.Adapter;

internal class Utils
{
    public static byte[] ExportPlayer(Player player, long time = 0L)
    {
        var playerFileData = new PlayerFileData
        {
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            Player = player,
            _isCloudSave = false
        };
        playerFileData.SetPlayTime(new TimeSpan(time * 10000000L));
        Main.LocalFavoriteData.ClearEntry(playerFileData);
        using var stream = new MemoryStream();
        using var cryptoStream = new CryptoStream(stream, Aes.Create().CreateEncryptor(Player.ENCRYPTION_KEY, Player.ENCRYPTION_KEY), CryptoStreamMode.Write);
        using var binaryWriter = new BinaryWriter(cryptoStream);
        binaryWriter.Write(279);
        playerFileData.Metadata.Write(binaryWriter);
        Player.Serialize(playerFileData, player, binaryWriter);
        binaryWriter.Flush();
        cryptoStream.FlushFinalBlock();
        return stream.ToArray();
    }

    public static MemoryStream SerializeObj<T>(T obj)
    {
        MemoryStream stream = new();
        Serializer.Serialize(stream, obj);
        return stream;
    }
    public static bool SetGameProgress(string type, bool enable)
    {
        var fields = typeof(ProgressType).GetFields().Where(x => x.FieldType == typeof(ProgressType));
        foreach (var field in fields)
        {
            var match = field.GetCustomAttribute<ProgressMatch>();
            if (match?.Name == type)
            {
                var target = match.Type.GetField(match.FieldName);
                if (target != null)
                {
                    target.SetValue(null, enable);
                    return true;
                }
            }
        }
        return false;
    }

    public static Dictionary<string, bool> GetGameProgress()
    {
        var prog = new Dictionary<string, bool>();
        var fields = typeof(ProgressType).GetFields().Where(x => x.FieldType == typeof(ProgressType));
        foreach (var field in fields)
        {
            var match = field.GetCustomAttribute<ProgressMatch>();
            if (match != null)
            {
                var val = match.Type.GetField(match.FieldName)?.GetValue(null);
                if (val != null)
                {
                    prog[match.Name] = Convert.ToBoolean(val);
                }
            }
        }
        return prog;
    }

    /// <summary>
    /// 设置旅途模式的难度
    /// </summary>
    /// <param name="diffName">难度</param>
    /// <returns></returns>
    public static bool SetJourneyDiff(string diffName)
    {
        float diff;
        switch (diffName.ToLower())
        {
            case "master":
                diff = 1f;
                break;
            case "journey":
                diff = 0f;
                break;
            case "normal":
                diff = 0.33f;
                break;
            case "expert":
                diff = 0.66f;
                break;
            default:
                return false;
        }
        var power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
        power._sliderCurrentValueCache = diff;
        power.UpdateInfoFromSliderValueCache();
        power.OnPlayerJoining(0);
        return true;
    }


    /// <summary>
    /// 匹配此程序集的方法
    /// </summary>
    /// <typeparam name="T">返回特性类型</typeparam>
    /// <param name="paramType">参数类型</param>
    /// <returns></returns>
    public static Dictionary<MethodInfo, (object?, T)> MatchAssemblyMethodByAttribute<T>(params Type[] paramType) where T : Attribute
    {
        var methods = new Dictionary<MethodInfo, (object?, T)>();
        Dictionary<Type, object?> types = new();
        Assembly.GetExecutingAssembly().GetTypes().ForEach(x =>
        {
            if (!x.IsAbstract && !x.IsInterface)
            {
                var flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public;
                x.GetMethods(flag).ForEach(m =>
                {
                    if (m.ParamsMatch(paramType))
                    {
                        var attribute = m.GetCustomAttribute<T>();
                        if (attribute != null)
                        {
                            if (!m.IsStatic)
                            {
                                var instance = types.TryGetValue(x, out var obj) && obj != null ? obj : Activator.CreateInstance(x);
                                types[x] = instance;
                                var method = instance?.GetType().GetMethod(m.Name, flag);
                                if (method != null)
                                {
                                    methods.Add(method, (instance, attribute));
                                }
                            }
                            else
                            {
                                methods.Add(m, (null, attribute));
                            }
                        }
                    }
                });
            }
        });
        return methods;
    }

    /// <summary>
    /// 加载指令
    /// </summary>
    public static void MapingCommand()
    {
        var methods = MatchAssemblyMethodByAttribute<CommandMatch>(typeof(CommandArgs));
        foreach (var (method, tuple) in methods)
        {
            var (Instance, attr) = tuple;
            Commands.ChatCommands.Add(new(attr.Permission, method.CreateDelegate<CommandDelegate>(Instance), attr.Name));
        }
    }

    /// <summary>
    /// 加载rest API
    /// </summary>
    public static void MapingRest()
    {
        var methods = MatchAssemblyMethodByAttribute<RestMatch>(typeof(RestRequestArgs));
        foreach (var (method, tuple) in methods)
        {
            var (Instance, attr) = tuple;
            TShock.RestApi.Register(new(attr.ApiPath, method.CreateDelegate<RestCommandD>(Instance)));
        }
    }

    /// <summary>
    /// 将Terraria Item[] 转为Model.item[]
    /// </summary>
    /// <param name="items">物品数组</param>
    /// <param name="slots">背包格</param>
    /// <returns></returns>
    public static Model.Internet.Item[] GetInventoryData(Terraria.Item[] items, int slots)
    {
        var info = new Model.Internet.Item[slots];
        for (var i = 0; i < slots; i++)
        {
            info[i] = new Model.Internet.Item(items[i].netID, items[i].prefix, items[i].stack);
        }
        return info;
    }


    public static Model.Internet.PlayerData? BInvSee(string playerName)
    {
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
                return null;
            }
            playerName = offline.Name;
            var data = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), offline.ID);
            tsplayer = CreateAPlayer(playerName, data);
        }
        var retObject = new Model.Internet.PlayerData
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
        for (var i = 0; i < tsplayer.Loadouts.Length; i++)
        {
            retObject.Loadout[i] = i == tsplayer.CurrentLoadoutIndex
                ? new Suits()
                {
                    armor = Utils.GetInventoryData(tsplayer.armor, NetItem.ArmorSlots),
                    dye = Utils.GetInventoryData(tsplayer.dye, NetItem.DyeSlots),
                }
                : new Suits()
                {
                    armor = Utils.GetInventoryData(tsplayer.Loadouts[i].Armor, tsplayer.Loadouts[i].Armor.Length),
                    dye = Utils.GetInventoryData(tsplayer.Loadouts[i].Dye, tsplayer.Loadouts[i].Dye.Length)
                };
        }
        //垃圾桶
        retObject.trashItem = new Model.Internet.Item[1]
        {
            new(tsplayer.trashItem.netID, tsplayer.trashItem.prefix, tsplayer.trashItem.stack)
        };
        //猪猪存钱罐
        retObject.Piggiy = GetInventoryData(tsplayer.bank.item, NetItem.PiggySlots);
        //保险箱
        retObject.safe = GetInventoryData(tsplayer.bank2.item, NetItem.SafeSlots);
        //护卫熔炉
        retObject.Forge = GetInventoryData(tsplayer.bank3.item, NetItem.ForgeSlots);
        //虚空保险箱
        retObject.VoidVault = GetInventoryData(tsplayer.bank4.item, NetItem.VoidSlots);

        return retObject;
    }


    public static Player CreateAPlayer(string name, TShockAPI.PlayerData data)
    {
        Player player = new()
        {
            name = name,
            statLife = data.health,
            statLifeMax = data.maxHealth,
            statMana = data.mana,
            statManaMax = data.maxMana,
            extraAccessory = data.extraSlot == 1,
            skinVariant = data.skinVariant ?? default,
            hair = data.hair ?? default,
            hairDye = data.hairDye,
            hairColor = data.hairColor ?? default,
            pantsColor = data.pantsColor ?? default,
            shirtColor = data.shirtColor ?? default,
            underShirtColor = data.underShirtColor ?? default,
            shoeColor = data.shoeColor ?? default,
            hideVisibleAccessory = data.hideVisuals,
            skinColor = data.skinColor ?? default,
            eyeColor = data.eyeColor ?? default,
            anglerQuestsFinished = data.questsCompleted,
            UsingBiomeTorches = data.usingBiomeTorches == 1,
            happyFunTorchTime = data.happyFunTorchTime == 1,
            unlockedBiomeTorches = data.unlockedBiomeTorches == 1,
            ateArtisanBread = data.ateArtisanBread == 1,
            usedAegisCrystal = data.usedAegisCrystal == 1,
            usedAegisFruit = data.usedAegisFruit == 1
        };
        player.usedAegisCrystal = data.usedArcaneCrystal == 1;
        player.usedGalaxyPearl = data.usedGalaxyPearl == 1;
        player.usedGummyWorm = data.usedGummyWorm == 1;
        player.usedAmbrosia = data.usedAmbrosia == 1;
        player.unlockedSuperCart = data.unlockedSuperCart == 1;
        player.enabledSuperCart = data.enabledSuperCart == 1;
        //正常同步
        if (data.currentLoadoutIndex == player.CurrentLoadoutIndex)
        {
            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                if (i < NetItem.InventoryIndex.Item2)
                {
                    player.inventory[i] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.inventory[i].stack = data.inventory[i].Stack;
                    player.inventory[i].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.ArmorIndex.Item2)
                {
                    var num = i - NetItem.ArmorIndex.Item1;
                    player.armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.armor[num].stack = data.inventory[i].Stack;
                    player.armor[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.DyeIndex.Item2)
                {
                    var num2 = i - NetItem.DyeIndex.Item1;
                    player.dye[num2] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.dye[num2].stack = data.inventory[i].Stack;
                    player.dye[num2].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.MiscEquipIndex.Item2)
                {
                    var num3 = i - NetItem.MiscEquipIndex.Item1;
                    player.miscEquips[num3] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.miscEquips[num3].stack = data.inventory[i].Stack;
                    player.miscEquips[num3].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.MiscDyeIndex.Item2)
                {
                    var num4 = i - NetItem.MiscDyeIndex.Item1;
                    player.miscDyes[num4] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.miscDyes[num4].stack = data.inventory[i].Stack;
                    player.miscDyes[num4].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.PiggyIndex.Item2)
                {
                    var num5 = i - NetItem.PiggyIndex.Item1;
                    player.bank.item[num5] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.bank.item[num5].stack = data.inventory[i].Stack;
                    player.bank.item[num5].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.SafeIndex.Item2)
                {
                    var num6 = i - NetItem.SafeIndex.Item1;
                    player.bank2.item[num6] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.bank2.item[num6].stack = data.inventory[i].Stack;
                    player.bank2.item[num6].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.TrashIndex.Item2)
                {
                    player.trashItem = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.trashItem.stack = data.inventory[i].Stack;
                    player.trashItem.prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.ForgeIndex.Item2)
                {
                    var num7 = i - NetItem.ForgeIndex.Item1;
                    player.bank3.item[num7] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.bank3.item[num7].stack = data.inventory[i].Stack;
                    player.bank3.item[num7].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.VoidIndex.Item2)
                {
                    var num8 = i - NetItem.VoidIndex.Item1;
                    player.bank4.item[num8] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.bank4.item[num8].stack = data.inventory[i].Stack;
                    player.bank4.item[num8].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout1Armor.Item2)
                {
                    var num9 = i - NetItem.Loadout1Armor.Item1;
                    player.Loadouts[0].Armor[num9] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.Loadouts[0].Armor[num9].stack = data.inventory[i].Stack;
                    player.Loadouts[0].Armor[num9].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout1Dye.Item2)
                {
                    var num10 = i - NetItem.Loadout1Dye.Item1;
                    player.Loadouts[0].Dye[num10] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.Loadouts[0].Dye[num10].stack = data.inventory[i].Stack;
                    player.Loadouts[0].Dye[num10].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout2Armor.Item2)
                {
                    var num11 = i - NetItem.Loadout2Armor.Item1;
                    player.Loadouts[1].Armor[num11] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.Loadouts[1].Armor[num11].stack = data.inventory[i].Stack;
                    player.Loadouts[1].Armor[num11].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout2Dye.Item2)
                {
                    var num12 = i - NetItem.Loadout2Dye.Item1;
                    player.Loadouts[1].Dye[num12] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.Loadouts[1].Dye[num12].stack = data.inventory[i].Stack;
                    player.Loadouts[1].Dye[num12].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout3Armor.Item2)
                {
                    var num13 = i - NetItem.Loadout3Armor.Item1;
                    player.Loadouts[2].Armor[num13] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.Loadouts[2].Armor[num13].stack = data.inventory[i].Stack;
                    player.Loadouts[2].Armor[num13].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout3Dye.Item2)
                {
                    var num14 = i - NetItem.Loadout3Dye.Item1;
                    player.Loadouts[2].Dye[num14] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.Loadouts[2].Dye[num14].stack = data.inventory[i].Stack;
                    player.Loadouts[2].Dye[num14].prefix = data.inventory[i].PrefixId;
                }
            }
        }
        //异常同步
        else
        {
            var notselected = 0;
            for (var i = 0; i < 3; i++)
            {
                if (player.CurrentLoadoutIndex != i && data.currentLoadoutIndex != i)
                {
                    notselected = i;
                }
            }
            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                if (i < NetItem.InventoryIndex.Item2)
                {
                    player.inventory[i] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.inventory[i].stack = data.inventory[i].Stack;
                    player.inventory[i].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.ArmorIndex.Item2)
                {
                    var num = i - NetItem.ArmorIndex.Item1;
                    player.Loadouts[data.currentLoadoutIndex].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.Loadouts[data.currentLoadoutIndex].Armor[num].stack = data.inventory[i].Stack;
                    player.Loadouts[data.currentLoadoutIndex].Armor[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.DyeIndex.Item2)
                {
                    var num = i - NetItem.DyeIndex.Item1;
                    player.Loadouts[data.currentLoadoutIndex].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.Loadouts[data.currentLoadoutIndex].Dye[num].stack = data.inventory[i].Stack;
                    player.Loadouts[data.currentLoadoutIndex].Dye[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.MiscEquipIndex.Item2)
                {
                    var num = i - NetItem.MiscEquipIndex.Item1;
                    player.miscEquips[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.miscEquips[num].stack = data.inventory[i].Stack;
                    player.miscEquips[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.MiscDyeIndex.Item2)
                {
                    var num = i - NetItem.MiscDyeIndex.Item1;
                    player.miscDyes[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.miscDyes[num].stack = data.inventory[i].Stack;
                    player.miscDyes[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.PiggyIndex.Item2)
                {
                    var num = i - NetItem.PiggyIndex.Item1;
                    player.bank.item[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.bank.item[num].stack = data.inventory[i].Stack;
                    player.bank.item[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.SafeIndex.Item2)
                {
                    var num = i - NetItem.SafeIndex.Item1;
                    player.bank2.item[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.bank2.item[num].stack = data.inventory[i].Stack;
                    player.bank2.item[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.TrashIndex.Item2)
                {
                    player.trashItem = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.trashItem.stack = data.inventory[i].Stack;
                    player.trashItem.prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.ForgeIndex.Item2)
                {
                    var num = i - NetItem.ForgeIndex.Item1;
                    player.bank3.item[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.bank3.item[num].stack = data.inventory[i].Stack;
                    player.bank3.item[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.VoidIndex.Item2)
                {
                    var num = i - NetItem.VoidIndex.Item1;
                    player.bank4.item[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                    player.bank4.item[num].stack = data.inventory[i].Stack;
                    player.bank4.item[num].prefix = data.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout1Armor.Item2)
                {
                    var num = i - NetItem.Loadout1Armor.Item1;
                    if (data.currentLoadoutIndex != 0)
                    {
                        if (notselected == 0)
                        {
                            player.Loadouts[0].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[0].Armor[num].stack = data.inventory[i].Stack;
                            player.Loadouts[0].Armor[num].prefix = data.inventory[i].PrefixId;
                        }
                        else if (player.CurrentLoadoutIndex != 0)
                        {
                            player.Loadouts[0].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[0].Armor[num].stack = data.inventory[i].Stack;
                            player.Loadouts[0].Armor[num].prefix = data.inventory[i].PrefixId;
                        }
                        else
                        {
                            player.Loadouts[0].Armor[num].TurnToAir(false);
                            player.armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.armor[num].stack = data.inventory[i].Stack;
                            player.armor[num].prefix = data.inventory[i].PrefixId;
                        }

                    }
                }
                else if (i < NetItem.Loadout1Dye.Item2)
                {
                    var num = i - NetItem.Loadout1Dye.Item1;
                    if (data.currentLoadoutIndex != 0)
                    {
                        if (notselected == 0)
                        {
                            player.Loadouts[0].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[0].Dye[num].stack = data.inventory[i].Stack;
                            player.Loadouts[0].Dye[num].prefix = data.inventory[i].PrefixId;
                        }
                        else if (player.CurrentLoadoutIndex != 0)
                        {
                            player.Loadouts[0].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[0].Dye[num].stack = data.inventory[i].Stack;
                            player.Loadouts[0].Dye[num].prefix = data.inventory[i].PrefixId;
                        }
                        else
                        {
                            player.Loadouts[0].Dye[num].TurnToAir(false);
                            player.dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.dye[num].stack = data.inventory[i].Stack;
                            player.dye[num].prefix = data.inventory[i].PrefixId;
                        }
                    }
                }
                else if (i < NetItem.Loadout2Armor.Item2)
                {
                    var num = i - NetItem.Loadout2Armor.Item1;
                    if (data.currentLoadoutIndex != 1)
                    {
                        if (notselected == 1)
                        {
                            player.Loadouts[1].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[1].Armor[num].stack = data.inventory[i].Stack;
                            player.Loadouts[1].Armor[num].prefix = data.inventory[i].PrefixId;
                        }
                        else if (player.CurrentLoadoutIndex != 1)
                        {
                            player.Loadouts[1].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[1].Armor[num].stack = data.inventory[i].Stack;
                            player.Loadouts[1].Armor[num].prefix = data.inventory[i].PrefixId;
                        }
                        else
                        {
                            player.Loadouts[1].Armor[num].TurnToAir(false);
                            player.armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.armor[num].stack = data.inventory[i].Stack;
                            player.armor[num].prefix = data.inventory[i].PrefixId;
                        }
                    }
                }
                else if (i < NetItem.Loadout2Dye.Item2)
                {
                    var num = i - NetItem.Loadout2Dye.Item1;
                    if (data.currentLoadoutIndex != 1)
                    {
                        if (notselected == 1)
                        {
                            player.Loadouts[1].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[1].Dye[num].stack = data.inventory[i].Stack;
                            player.Loadouts[1].Dye[num].prefix = data.inventory[i].PrefixId;
                        }
                        else if (player.CurrentLoadoutIndex != 1)
                        {
                            player.Loadouts[1].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[1].Dye[num].stack = data.inventory[i].Stack;
                            player.Loadouts[1].Dye[num].prefix = data.inventory[i].PrefixId;
                        }
                        else
                        {
                            player.Loadouts[1].Dye[num].TurnToAir(false);
                            player.dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.dye[num].stack = data.inventory[i].Stack;
                            player.dye[num].prefix = data.inventory[i].PrefixId;
                        }
                    }
                }
                else if (i < NetItem.Loadout3Armor.Item2)
                {
                    var num = i - NetItem.Loadout3Armor.Item1;
                    if (data.currentLoadoutIndex != 2)
                    {
                        if (notselected == 2)
                        {
                            player.Loadouts[2].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[2].Armor[num].stack = data.inventory[i].Stack;
                            player.Loadouts[2].Armor[num].prefix = data.inventory[i].PrefixId;
                        }
                        else
                        {
                            if (player.CurrentLoadoutIndex != 2)
                            {
                                player.Loadouts[2].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[2].Armor[num].stack = data.inventory[i].Stack;
                                player.Loadouts[2].Armor[num].prefix = data.inventory[i].PrefixId;
                            }
                            else
                            {
                                player.Loadouts[2].Armor[num].TurnToAir(false);
                                player.armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.armor[num].stack = data.inventory[i].Stack;
                                player.armor[num].prefix = data.inventory[i].PrefixId;
                            }
                        }
                    }
                }
                else if (i < NetItem.Loadout3Dye.Item2)
                {
                    var num = i - NetItem.Loadout3Dye.Item1;
                    if (data.currentLoadoutIndex != 2)
                    {
                        if (notselected == 2)
                        {
                            player.Loadouts[2].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[2].Dye[num].stack = data.inventory[i].Stack;
                            player.Loadouts[2].Dye[num].prefix = data.inventory[i].PrefixId;
                        }
                        else if (player.CurrentLoadoutIndex != 2)
                        {
                            player.Loadouts[2].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.Loadouts[2].Dye[num].stack = data.inventory[i].Stack;
                            player.Loadouts[2].Dye[num].prefix = data.inventory[i].PrefixId;
                        }
                        else
                        {
                            player.Loadouts[2].Dye[num].TurnToAir(false);
                            player.dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                            player.dye[num].stack = data.inventory[i].Stack;
                            player.dye[num].prefix = data.inventory[i].PrefixId;
                        }
                    }
                }
            }
        }
        return player;
    }

    public static Terraria.Item NetItem2Item(NetItem netItem)
    {
        var item = new Terraria.Item
        {
            netID = netItem.NetId,
            stack = netItem.Stack,
            prefix = netItem.PrefixId
        };
        return item;
    }
    public static byte[] CreateMapBytes(ImageType type)
    {
        var image = CreateMapImage();
        using var stream = new MemoryStream();
        image.Save(stream, type == ImageType.Png ? new PngEncoder() : new JpegEncoder());
        return stream.ToArray();
    }
    public static SixLabors.ImageSharp.Image CreateMapImage()
    {
        Image<Rgba32> image = new(Main.maxTilesX, Main.maxTilesY);

        MapHelper.Initialize();
        Main.Map ??= new WorldMap(0, 0);
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                var tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);
                var col = MapHelper.GetMapTileXnaColor(ref tile);
                image[x, y] = new Rgba32(col.R, col.G, col.B, col.A);
            }
        }

        return image;
    }

    public static void ReStarServer(string startArgs, bool save = false)
    {
        if (save)
        {
            TShock.Utils.StopServer(true);
        }
        else
        {
            Netplay.SaveOnServerExit = false;
            Netplay.Disconnect = true;
        }
        var currentProcess = Process.GetCurrentProcess();
        Process.Start(currentProcess.MainModule!.FileName!, startArgs);
        Environment.Exit(0);
    }

    public static void RestServer(ResetServerArgs args)
    {
        WorldFile.SaveWorld();
        ClearDB();
        foreach (var cmd in Plugin.Config.ResetConfig.Commands)
        {
            Commands.HandleCommand(TSPlayer.Server, cmd);
        }
        if (TShock.Config.Settings.RestApiEnabled)
        {
            //关闭rest
            TShock.RestApi.Stop();
        }
        //关闭Tshock日志
        TShock.Log.Dispose();
        //关闭terrariaApi日志
        var obj = (ServerLogWriter) ServerApi.LogWriter
            .GetType()
            .GetProperty("DefaultLogWriter", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(ServerApi.LogWriter)!;
        obj.Dispose();
        if (Plugin.Config.ResetConfig.ClearLogs)
        {
            new DirectoryInfo(TShock.Config.Settings.LogPath)
                .GetFiles()
                .ForEach(x => x.Delete());
        }

        if (Plugin.Config.ResetConfig.ClearMap && File.Exists(Main.worldPathName))
        {
            File.Delete(Main.worldPathName);
        }

        var dir = Path.GetDirectoryName(Main.worldPathName)!;
        if (args.UseFile)
        {
            File.WriteAllBytes(Path.Combine(dir, args.FileName), args.FileBuffer);
        }
        ReStarServer(args.StartArgs);
    }

    public static void ClearDB()
    {
        Plugin.Config.ResetConfig.ClearTable.ForEach(x =>
        {
            try
            {
                TShock.DB.Query($"delete from {x}");
            }
            catch { }
        });
    }

    internal static void HandleCommandLine(string[] param)
    {
        var args = Terraria.Utils.ParseArguements(param);
        foreach (var (key, value) in args)
        {
            switch (key.ToLower())
            {
                case "-seed":
                    Main.AutogenSeedName = value;
                    break;
                case "-mode":

                    if (int.TryParse(value, out var mode))
                    {
                        Plugin.Channeler.Writer.TryWrite(mode);
                    }

                    break;
            }
        }
    }
}
