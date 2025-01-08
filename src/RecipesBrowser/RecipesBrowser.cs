using Terraria;
using Terraria.Localization;
using Terraria.Map;
using TerrariaApi.Server;
using TShockAPI;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 7);

    public override string Author => "棱镜 羽学适配";

    public override string Description => GetString("通过指令获取物品合成表");

    public MainPlugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("", this.FindRecipe, "查", "find", "fd"));
        MapHelper.Initialize();
        BuildMapAtlas();
    }

    public static void BuildMapAtlas()
    {
        Lang._mapLegendCache = (LocalizedText[]) (object) new LocalizedText[MapHelper.LookupCount()];
        for (var i = 0; i < Lang._mapLegendCache.Length; i++)
        {
            Lang._mapLegendCache[i] = LocalizedText.Empty;
        }
        Lang._mapLegendCache[MapHelper.TileToLookup(4, 0)] = Lang._itemNameCache[8];
        Lang._mapLegendCache[MapHelper.TileToLookup(4, 1)] = Lang._itemNameCache[8];
        Lang._mapLegendCache[MapHelper.TileToLookup(5, 0)] = Language.GetText("MapObject.Tree");
        Lang._mapLegendCache[MapHelper.TileToLookup(6, 0)] = Language.GetText("MapObject.Iron");
        Lang._mapLegendCache[MapHelper.TileToLookup(7, 0)] = Language.GetText("MapObject.Copper");
        Lang._mapLegendCache[MapHelper.TileToLookup(8, 0)] = Language.GetText("MapObject.Gold");
        Lang._mapLegendCache[MapHelper.TileToLookup(9, 0)] = Language.GetText("MapObject.Silver");
        Lang._mapLegendCache[MapHelper.TileToLookup(10, 0)] = Language.GetText("MapObject.Door");
        Lang._mapLegendCache[MapHelper.TileToLookup(11, 0)] = Language.GetText("MapObject.Door");
        Lang._mapLegendCache[MapHelper.TileToLookup(12, 0)] = Lang._itemNameCache[29];
        Lang._mapLegendCache[MapHelper.TileToLookup(13, 0)] = Lang._itemNameCache[31];
        Lang._mapLegendCache[MapHelper.TileToLookup(14, 0)] = Language.GetText("MapObject.Table");
        Lang._mapLegendCache[MapHelper.TileToLookup(15, 0)] = Language.GetText("MapObject.Chair");
        Lang._mapLegendCache[MapHelper.TileToLookup(16, 0)] = Language.GetText("MapObject.Anvil");
        Lang._mapLegendCache[MapHelper.TileToLookup(17, 0)] = Lang._itemNameCache[33];
        Lang._mapLegendCache[MapHelper.TileToLookup(18, 0)] = Lang._itemNameCache[36];
        Lang._mapLegendCache[MapHelper.TileToLookup(20, 0)] = Language.GetText("MapObject.Sapling");
        Lang._mapLegendCache[MapHelper.TileToLookup(21, 0)] = Lang._itemNameCache[48];
        Lang._mapLegendCache[MapHelper.TileToLookup(22, 0)] = Language.GetText("MapObject.Demonite");
        Lang._mapLegendCache[MapHelper.TileToLookup(26, 0)] = Language.GetText("MapObject.DemonAltar");
        Lang._mapLegendCache[MapHelper.TileToLookup(26, 1)] = Language.GetText("MapObject.CrimsonAltar");
        Lang._mapLegendCache[MapHelper.TileToLookup(27, 0)] = Lang._itemNameCache[63];
        Lang._mapLegendCache[MapHelper.TileToLookup(407, 0)] = Language.GetText("MapObject.Fossil");
        Lang._mapLegendCache[MapHelper.TileToLookup(412, 0)] = Lang._itemNameCache[3549];
        for (var j = 0; j < 9; j++)
        {
            Lang._mapLegendCache[MapHelper.TileToLookup(28, j)] = Language.GetText("MapObject.Pot");
        }
        Lang._mapLegendCache[MapHelper.TileToLookup(37, 0)] = Lang._itemNameCache[116];
        Lang._mapLegendCache[MapHelper.TileToLookup(29, 0)] = Lang._itemNameCache[87];
        Lang._mapLegendCache[MapHelper.TileToLookup(31, 0)] = Lang._itemNameCache[115];
        Lang._mapLegendCache[MapHelper.TileToLookup(31, 1)] = Lang._itemNameCache[3062];
        Lang._mapLegendCache[MapHelper.TileToLookup(32, 0)] = Language.GetText("MapObject.Thorns");
        Lang._mapLegendCache[MapHelper.TileToLookup(33, 0)] = Lang._itemNameCache[105];
        Lang._mapLegendCache[MapHelper.TileToLookup(34, 0)] = Language.GetText("MapObject.Chandelier");
        Lang._mapLegendCache[MapHelper.TileToLookup(35, 0)] = Lang._itemNameCache[1813];
        Lang._mapLegendCache[MapHelper.TileToLookup(36, 0)] = Lang._itemNameCache[1869];
        Lang._mapLegendCache[MapHelper.TileToLookup(42, 0)] = Language.GetText("MapObject.Lantern");
        Lang._mapLegendCache[MapHelper.TileToLookup(48, 0)] = Lang._itemNameCache[147];
        Lang._mapLegendCache[MapHelper.TileToLookup(49, 0)] = Lang._itemNameCache[148];
        Lang._mapLegendCache[MapHelper.TileToLookup(50, 0)] = Lang._itemNameCache[149];
        Lang._mapLegendCache[MapHelper.TileToLookup(51, 0)] = Language.GetText("MapObject.Web");
        Lang._mapLegendCache[MapHelper.TileToLookup(55, 0)] = Lang._itemNameCache[171];
        Lang._mapLegendCache[MapHelper.TileToLookup(63, 0)] = Lang._itemNameCache[177];
        Lang._mapLegendCache[MapHelper.TileToLookup(64, 0)] = Lang._itemNameCache[178];
        Lang._mapLegendCache[MapHelper.TileToLookup(65, 0)] = Lang._itemNameCache[179];
        Lang._mapLegendCache[MapHelper.TileToLookup(66, 0)] = Lang._itemNameCache[180];
        Lang._mapLegendCache[MapHelper.TileToLookup(67, 0)] = Lang._itemNameCache[181];
        Lang._mapLegendCache[MapHelper.TileToLookup(68, 0)] = Lang._itemNameCache[182];
        Lang._mapLegendCache[MapHelper.TileToLookup(69, 0)] = Language.GetText("MapObject.Thorn");
        Lang._mapLegendCache[MapHelper.TileToLookup(72, 0)] = Language.GetText("MapObject.GiantMushroom");
        Lang._mapLegendCache[MapHelper.TileToLookup(77, 0)] = Lang._itemNameCache[221];
        Lang._mapLegendCache[MapHelper.TileToLookup(78, 0)] = Lang._itemNameCache[222];
        Lang._mapLegendCache[MapHelper.TileToLookup(79, 0)] = Lang._itemNameCache[224];
        Lang._mapLegendCache[MapHelper.TileToLookup(80, 0)] = Lang._itemNameCache[276];
        Lang._mapLegendCache[MapHelper.TileToLookup(81, 0)] = Lang._itemNameCache[275];
        Lang._mapLegendCache[MapHelper.TileToLookup(82, 0)] = Lang._itemNameCache[313];
        Lang._mapLegendCache[MapHelper.TileToLookup(82, 1)] = Lang._itemNameCache[314];
        Lang._mapLegendCache[MapHelper.TileToLookup(82, 2)] = Lang._itemNameCache[315];
        Lang._mapLegendCache[MapHelper.TileToLookup(82, 3)] = Lang._itemNameCache[316];
        Lang._mapLegendCache[MapHelper.TileToLookup(82, 4)] = Lang._itemNameCache[317];
        Lang._mapLegendCache[MapHelper.TileToLookup(82, 5)] = Lang._itemNameCache[318];
        Lang._mapLegendCache[MapHelper.TileToLookup(82, 6)] = Lang._itemNameCache[2358];
        Lang._mapLegendCache[MapHelper.TileToLookup(83, 0)] = Lang._itemNameCache[313];
        Lang._mapLegendCache[MapHelper.TileToLookup(83, 1)] = Lang._itemNameCache[314];
        Lang._mapLegendCache[MapHelper.TileToLookup(83, 2)] = Lang._itemNameCache[315];
        Lang._mapLegendCache[MapHelper.TileToLookup(83, 3)] = Lang._itemNameCache[316];
        Lang._mapLegendCache[MapHelper.TileToLookup(83, 4)] = Lang._itemNameCache[317];
        Lang._mapLegendCache[MapHelper.TileToLookup(83, 5)] = Lang._itemNameCache[318];
        Lang._mapLegendCache[MapHelper.TileToLookup(83, 6)] = Lang._itemNameCache[2358];
        Lang._mapLegendCache[MapHelper.TileToLookup(84, 0)] = Lang._itemNameCache[313];
        Lang._mapLegendCache[MapHelper.TileToLookup(84, 1)] = Lang._itemNameCache[314];
        Lang._mapLegendCache[MapHelper.TileToLookup(84, 2)] = Lang._itemNameCache[315];
        Lang._mapLegendCache[MapHelper.TileToLookup(84, 3)] = Lang._itemNameCache[316];
        Lang._mapLegendCache[MapHelper.TileToLookup(84, 4)] = Lang._itemNameCache[317];
        Lang._mapLegendCache[MapHelper.TileToLookup(84, 5)] = Lang._itemNameCache[318];
        Lang._mapLegendCache[MapHelper.TileToLookup(84, 6)] = Lang._itemNameCache[2358];
        Lang._mapLegendCache[MapHelper.TileToLookup(85, 0)] = Lang._itemNameCache[321];
        Lang._mapLegendCache[MapHelper.TileToLookup(86, 0)] = Lang._itemNameCache[332];
        Lang._mapLegendCache[MapHelper.TileToLookup(87, 0)] = Lang._itemNameCache[333];
        Lang._mapLegendCache[MapHelper.TileToLookup(88, 0)] = Lang._itemNameCache[334];
        Lang._mapLegendCache[MapHelper.TileToLookup(89, 0)] = Lang._itemNameCache[335];
        Lang._mapLegendCache[MapHelper.TileToLookup(90, 0)] = Lang._itemNameCache[336];
        Lang._mapLegendCache[MapHelper.TileToLookup(91, 0)] = Language.GetText("MapObject.Banner");
        Lang._mapLegendCache[MapHelper.TileToLookup(92, 0)] = Lang._itemNameCache[341];
        Lang._mapLegendCache[MapHelper.TileToLookup(93, 0)] = Language.GetText("MapObject.FloorLamp");
        Lang._mapLegendCache[MapHelper.TileToLookup(94, 0)] = Lang._itemNameCache[352];
        Lang._mapLegendCache[MapHelper.TileToLookup(95, 0)] = Lang._itemNameCache[344];
        Lang._mapLegendCache[MapHelper.TileToLookup(96, 0)] = Lang._itemNameCache[345];
        Lang._mapLegendCache[MapHelper.TileToLookup(97, 0)] = Lang._itemNameCache[346];
        Lang._mapLegendCache[MapHelper.TileToLookup(98, 0)] = Lang._itemNameCache[347];
        Lang._mapLegendCache[MapHelper.TileToLookup(100, 0)] = Lang._itemNameCache[349];
        Lang._mapLegendCache[MapHelper.TileToLookup(101, 0)] = Lang._itemNameCache[354];
        Lang._mapLegendCache[MapHelper.TileToLookup(102, 0)] = Lang._itemNameCache[355];
        Lang._mapLegendCache[MapHelper.TileToLookup(103, 0)] = Lang._itemNameCache[356];
        Lang._mapLegendCache[MapHelper.TileToLookup(104, 0)] = Lang._itemNameCache[359];
        Lang._mapLegendCache[MapHelper.TileToLookup(105, 0)] = Language.GetText("MapObject.Statue");
        Lang._mapLegendCache[MapHelper.TileToLookup(105, 2)] = Language.GetText("MapObject.Vase");
        Lang._mapLegendCache[MapHelper.TileToLookup(106, 0)] = Lang._itemNameCache[363];
        Lang._mapLegendCache[MapHelper.TileToLookup(107, 0)] = Language.GetText("MapObject.Cobalt");
        Lang._mapLegendCache[MapHelper.TileToLookup(108, 0)] = Language.GetText("MapObject.Mythril");
        Lang._mapLegendCache[MapHelper.TileToLookup(111, 0)] = Language.GetText("MapObject.Adamantite");
        Lang._mapLegendCache[MapHelper.TileToLookup(114, 0)] = Lang._itemNameCache[398];
        Lang._mapLegendCache[MapHelper.TileToLookup(125, 0)] = Lang._itemNameCache[487];
        Lang._mapLegendCache[MapHelper.TileToLookup(128, 0)] = Lang._itemNameCache[498];
        Lang._mapLegendCache[MapHelper.TileToLookup(129, 0)] = Lang._itemNameCache[502];
        Lang._mapLegendCache[MapHelper.TileToLookup(132, 0)] = Lang._itemNameCache[513];
        Lang._mapLegendCache[MapHelper.TileToLookup(411, 0)] = Lang._itemNameCache[3545];
        Lang._mapLegendCache[MapHelper.TileToLookup(133, 0)] = Lang._itemNameCache[524];
        Lang._mapLegendCache[MapHelper.TileToLookup(133, 1)] = Lang._itemNameCache[1221];
        Lang._mapLegendCache[MapHelper.TileToLookup(134, 0)] = Lang._itemNameCache[525];
        Lang._mapLegendCache[MapHelper.TileToLookup(134, 1)] = Lang._itemNameCache[1220];
        Lang._mapLegendCache[MapHelper.TileToLookup(136, 0)] = Lang._itemNameCache[538];
        Lang._mapLegendCache[MapHelper.TileToLookup(137, 0)] = Language.GetText("MapObject.Trap");
        Lang._mapLegendCache[MapHelper.TileToLookup(138, 0)] = Lang._itemNameCache[540];
        Lang._mapLegendCache[MapHelper.TileToLookup(139, 0)] = Lang._itemNameCache[576];
        Lang._mapLegendCache[MapHelper.TileToLookup(142, 0)] = Lang._itemNameCache[581];
        Lang._mapLegendCache[MapHelper.TileToLookup(143, 0)] = Lang._itemNameCache[582];
        Lang._mapLegendCache[MapHelper.TileToLookup(144, 0)] = Language.GetText("MapObject.Timer");
        Lang._mapLegendCache[MapHelper.TileToLookup(149, 0)] = Language.GetText("MapObject.ChristmasLight");
        Lang._mapLegendCache[MapHelper.TileToLookup(166, 0)] = Language.GetText("MapObject.Tin");
        Lang._mapLegendCache[MapHelper.TileToLookup(167, 0)] = Language.GetText("MapObject.Lead");
        Lang._mapLegendCache[MapHelper.TileToLookup(168, 0)] = Language.GetText("MapObject.Tungsten");
        Lang._mapLegendCache[MapHelper.TileToLookup(169, 0)] = Language.GetText("MapObject.Platinum");
        Lang._mapLegendCache[MapHelper.TileToLookup(170, 0)] = Language.GetText("MapObject.PineTree");
        Lang._mapLegendCache[MapHelper.TileToLookup(171, 0)] = Lang._itemNameCache[1873];
        Lang._mapLegendCache[MapHelper.TileToLookup(172, 0)] = Language.GetText("MapObject.Sink");
        Lang._mapLegendCache[MapHelper.TileToLookup(173, 0)] = Lang._itemNameCache[349];
        Lang._mapLegendCache[MapHelper.TileToLookup(174, 0)] = Lang._itemNameCache[713];
        Lang._mapLegendCache[MapHelper.TileToLookup(178, 0)] = Lang._itemNameCache[181];
        Lang._mapLegendCache[MapHelper.TileToLookup(178, 1)] = Lang._itemNameCache[180];
        Lang._mapLegendCache[MapHelper.TileToLookup(178, 2)] = Lang._itemNameCache[177];
        Lang._mapLegendCache[MapHelper.TileToLookup(178, 3)] = Lang._itemNameCache[179];
        Lang._mapLegendCache[MapHelper.TileToLookup(178, 4)] = Lang._itemNameCache[178];
        Lang._mapLegendCache[MapHelper.TileToLookup(178, 5)] = Lang._itemNameCache[182];
        Lang._mapLegendCache[MapHelper.TileToLookup(178, 6)] = Lang._itemNameCache[999];
        Lang._mapLegendCache[MapHelper.TileToLookup(191, 0)] = Language.GetText("MapObject.LivingWood");
        Lang._mapLegendCache[MapHelper.TileToLookup(204, 0)] = Language.GetText("MapObject.Crimtane");
        Lang._mapLegendCache[MapHelper.TileToLookup(207, 0)] = Language.GetText("MapObject.WaterFountain");
        Lang._mapLegendCache[MapHelper.TileToLookup(209, 0)] = Lang._itemNameCache[928];
        Lang._mapLegendCache[MapHelper.TileToLookup(211, 0)] = Language.GetText("MapObject.Chlorophyte");
        Lang._mapLegendCache[MapHelper.TileToLookup(212, 0)] = Language.GetText("MapObject.Turret");
        Lang._mapLegendCache[MapHelper.TileToLookup(213, 0)] = Lang._itemNameCache[965];
        Lang._mapLegendCache[MapHelper.TileToLookup(214, 0)] = Lang._itemNameCache[85];
        Lang._mapLegendCache[MapHelper.TileToLookup(215, 0)] = Lang._itemNameCache[966];
        Lang._mapLegendCache[MapHelper.TileToLookup(216, 0)] = Language.GetText("MapObject.Rocket");
        Lang._mapLegendCache[MapHelper.TileToLookup(217, 0)] = Lang._itemNameCache[995];
        Lang._mapLegendCache[MapHelper.TileToLookup(218, 0)] = Lang._itemNameCache[996];
        Lang._mapLegendCache[MapHelper.TileToLookup(219, 0)] = Language.GetText("MapObject.SiltExtractinator");
        Lang._mapLegendCache[MapHelper.TileToLookup(220, 0)] = Lang._itemNameCache[998];
        Lang._mapLegendCache[MapHelper.TileToLookup(221, 0)] = Language.GetText("MapObject.Palladium");
        Lang._mapLegendCache[MapHelper.TileToLookup(222, 0)] = Language.GetText("MapObject.Orichalcum");
        Lang._mapLegendCache[MapHelper.TileToLookup(223, 0)] = Language.GetText("MapObject.Titanium");
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 0)] = Lang._itemNameCache[1107];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 1)] = Lang._itemNameCache[1108];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 2)] = Lang._itemNameCache[1109];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 3)] = Lang._itemNameCache[1110];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 4)] = Lang._itemNameCache[1111];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 5)] = Lang._itemNameCache[1112];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 6)] = Lang._itemNameCache[1113];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 7)] = Lang._itemNameCache[1114];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 8)] = Lang._itemNameCache[3385];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 9)] = Lang._itemNameCache[3386];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 10)] = Lang._itemNameCache[3387];
        Lang._mapLegendCache[MapHelper.TileToLookup(227, 11)] = Lang._itemNameCache[3388];
        Lang._mapLegendCache[MapHelper.TileToLookup(228, 0)] = Lang._itemNameCache[1120];
        Lang._mapLegendCache[MapHelper.TileToLookup(231, 0)] = Language.GetText("MapObject.Larva");
        Lang._mapLegendCache[MapHelper.TileToLookup(232, 0)] = Lang._itemNameCache[1150];
        Lang._mapLegendCache[MapHelper.TileToLookup(235, 0)] = Lang._itemNameCache[1263];
        Lang._mapLegendCache[MapHelper.TileToLookup(236, 0)] = Lang._itemNameCache[1291];
        Lang._mapLegendCache[MapHelper.TileToLookup(237, 0)] = Lang._itemNameCache[1292];
        Lang._mapLegendCache[MapHelper.TileToLookup(238, 0)] = Language.GetText("MapObject.PlanterasBulb");
        Lang._mapLegendCache[MapHelper.TileToLookup(239, 0)] = Language.GetText("MapObject.MetalBar");
        Lang._mapLegendCache[MapHelper.TileToLookup(240, 0)] = Language.GetText("MapObject.Trophy");
        Lang._mapLegendCache[MapHelper.TileToLookup(240, 2)] = Lang._npcNameCache[21];
        Lang._mapLegendCache[MapHelper.TileToLookup(240, 3)] = Language.GetText("MapObject.ItemRack");
        Lang._mapLegendCache[MapHelper.TileToLookup(240, 4)] = Lang._itemNameCache[2442];
        Lang._mapLegendCache[MapHelper.TileToLookup(241, 0)] = Lang._itemNameCache[1417];
        Lang._mapLegendCache[MapHelper.TileToLookup(242, 0)] = Language.GetText("MapObject.Painting");
        Lang._mapLegendCache[MapHelper.TileToLookup(242, 1)] = Language.GetText("MapObject.AnimalSkin");
        Lang._mapLegendCache[MapHelper.TileToLookup(243, 0)] = Lang._itemNameCache[1430];
        Lang._mapLegendCache[MapHelper.TileToLookup(244, 0)] = Lang._itemNameCache[1449];
        Lang._mapLegendCache[MapHelper.TileToLookup(245, 0)] = Language.GetText("MapObject.Picture");
        Lang._mapLegendCache[MapHelper.TileToLookup(246, 0)] = Language.GetText("MapObject.Picture");
        Lang._mapLegendCache[MapHelper.TileToLookup(247, 0)] = Lang._itemNameCache[1551];
        Lang._mapLegendCache[MapHelper.TileToLookup(254, 0)] = Lang._itemNameCache[1725];
        Lang._mapLegendCache[MapHelper.TileToLookup(269, 0)] = Lang._itemNameCache[1989];
        Lang._mapLegendCache[MapHelper.TileToLookup(270, 0)] = Lang._itemNameCache[1993];
        Lang._mapLegendCache[MapHelper.TileToLookup(271, 0)] = Lang._itemNameCache[2005];
        Lang._mapLegendCache[MapHelper.TileToLookup(275, 0)] = Lang._itemNameCache[2162];
        Lang._mapLegendCache[MapHelper.TileToLookup(276, 0)] = Lang._itemNameCache[2163];
        Lang._mapLegendCache[MapHelper.TileToLookup(277, 0)] = Lang._itemNameCache[2164];
        Lang._mapLegendCache[MapHelper.TileToLookup(278, 0)] = Lang._itemNameCache[2165];
        Lang._mapLegendCache[MapHelper.TileToLookup(279, 0)] = Lang._itemNameCache[2166];
        Lang._mapLegendCache[MapHelper.TileToLookup(280, 0)] = Lang._itemNameCache[2167];
        Lang._mapLegendCache[MapHelper.TileToLookup(281, 0)] = Lang._itemNameCache[2168];
        Lang._mapLegendCache[MapHelper.TileToLookup(282, 0)] = Lang._itemNameCache[250];
        Lang._mapLegendCache[MapHelper.TileToLookup(413, 0)] = Language.GetText("MapObject.OrangeSquirrelCage");
        Lang._mapLegendCache[MapHelper.TileToLookup(283, 0)] = Lang._itemNameCache[2172];
        Lang._mapLegendCache[MapHelper.TileToLookup(285, 0)] = Lang._itemNameCache[2174];
        Lang._mapLegendCache[MapHelper.TileToLookup(286, 0)] = Lang._itemNameCache[2175];
        Lang._mapLegendCache[MapHelper.TileToLookup(287, 0)] = Lang._itemNameCache[2177];
        Lang._mapLegendCache[MapHelper.TileToLookup(288, 0)] = Lang._itemNameCache[2178];
        Lang._mapLegendCache[MapHelper.TileToLookup(289, 0)] = Lang._itemNameCache[2179];
        Lang._mapLegendCache[MapHelper.TileToLookup(290, 0)] = Lang._itemNameCache[2180];
        Lang._mapLegendCache[MapHelper.TileToLookup(291, 0)] = Lang._itemNameCache[2181];
        Lang._mapLegendCache[MapHelper.TileToLookup(292, 0)] = Lang._itemNameCache[2182];
        Lang._mapLegendCache[MapHelper.TileToLookup(293, 0)] = Lang._itemNameCache[2183];
        Lang._mapLegendCache[MapHelper.TileToLookup(294, 0)] = Lang._itemNameCache[2184];
        Lang._mapLegendCache[MapHelper.TileToLookup(295, 0)] = Lang._itemNameCache[2185];
        Lang._mapLegendCache[MapHelper.TileToLookup(296, 0)] = Lang._itemNameCache[2186];
        Lang._mapLegendCache[MapHelper.TileToLookup(297, 0)] = Lang._itemNameCache[2187];
        Lang._mapLegendCache[MapHelper.TileToLookup(298, 0)] = Lang._itemNameCache[2190];
        Lang._mapLegendCache[MapHelper.TileToLookup(299, 0)] = Lang._itemNameCache[2191];
        Lang._mapLegendCache[MapHelper.TileToLookup(300, 0)] = Lang._itemNameCache[2192];
        Lang._mapLegendCache[MapHelper.TileToLookup(301, 0)] = Lang._itemNameCache[2193];
        Lang._mapLegendCache[MapHelper.TileToLookup(302, 0)] = Lang._itemNameCache[2194];
        Lang._mapLegendCache[MapHelper.TileToLookup(303, 0)] = Lang._itemNameCache[2195];
        Lang._mapLegendCache[MapHelper.TileToLookup(304, 0)] = Lang._itemNameCache[2196];
        Lang._mapLegendCache[MapHelper.TileToLookup(305, 0)] = Lang._itemNameCache[2197];
        Lang._mapLegendCache[MapHelper.TileToLookup(306, 0)] = Lang._itemNameCache[2198];
        Lang._mapLegendCache[MapHelper.TileToLookup(307, 0)] = Lang._itemNameCache[2203];
        Lang._mapLegendCache[MapHelper.TileToLookup(308, 0)] = Lang._itemNameCache[2204];
        Lang._mapLegendCache[MapHelper.TileToLookup(309, 0)] = Lang._itemNameCache[2206];
        Lang._mapLegendCache[MapHelper.TileToLookup(310, 0)] = Lang._itemNameCache[2207];
        Lang._mapLegendCache[MapHelper.TileToLookup(316, 0)] = Lang._itemNameCache[2439];
        Lang._mapLegendCache[MapHelper.TileToLookup(317, 0)] = Lang._itemNameCache[2440];
        Lang._mapLegendCache[MapHelper.TileToLookup(318, 0)] = Lang._itemNameCache[2441];
        Lang._mapLegendCache[MapHelper.TileToLookup(319, 0)] = Lang._itemNameCache[2490];
        Lang._mapLegendCache[MapHelper.TileToLookup(320, 0)] = Lang._itemNameCache[2496];
        Lang._mapLegendCache[MapHelper.TileToLookup(323, 0)] = Language.GetText("MapObject.PalmTree");
        Lang._mapLegendCache[MapHelper.TileToLookup(314, 0)] = Lang._itemNameCache[2340];
        Lang._mapLegendCache[MapHelper.TileToLookup(353, 0)] = Lang._itemNameCache[2996];
        Lang._mapLegendCache[MapHelper.TileToLookup(354, 0)] = Lang._itemNameCache[2999];
        Lang._mapLegendCache[MapHelper.TileToLookup(355, 0)] = Lang._itemNameCache[3000];
        Lang._mapLegendCache[MapHelper.TileToLookup(356, 0)] = Lang._itemNameCache[3064];
        Lang._mapLegendCache[MapHelper.TileToLookup(365, 0)] = Lang._itemNameCache[3077];
        Lang._mapLegendCache[MapHelper.TileToLookup(366, 0)] = Lang._itemNameCache[3078];
        Lang._mapLegendCache[MapHelper.TileToLookup(373, 0)] = Language.GetText("MapObject.DrippingWater");
        Lang._mapLegendCache[MapHelper.TileToLookup(374, 0)] = Language.GetText("MapObject.DrippingLava");
        Lang._mapLegendCache[MapHelper.TileToLookup(375, 0)] = Language.GetText("MapObject.DrippingHoney");
        Lang._mapLegendCache[MapHelper.TileToLookup(377, 0)] = Lang._itemNameCache[3198];
        Lang._mapLegendCache[MapHelper.TileToLookup(372, 0)] = Lang._itemNameCache[3117];
    }

    private void FindRecipe(CommandArgs args)
    {

        // 检查玩家是否具有“查合成表”权限
        if (!args.Player.HasPermission("RecipesBrowser"))
        {
            // 如果没有权限，则向该玩家发送错误消息并退出方法
            args.Player.SendErrorMessage("你没有使用合成表指令的权限");
            return;
        }

        var itemByIdOrName = TShock.Utils.GetItemByIdOrName(args.Parameters[0]);
        if (itemByIdOrName.Count > 1)
        {
            args.Player.SendInfoMessage("检索到多个物品匹配");
            var text = "";
            for (var i = 0; i < itemByIdOrName.Count; i++)
            {
                text = text + Lang.GetItemNameValue(itemByIdOrName[i].type) + ",";
                if ((i + 1) % 5 == 0)
                {
                    text += "\n";
                }
            }
            text = text.Trim(',');
            args.Player.SendInfoMessage(text);
            return;
        }
        if (itemByIdOrName.Count < 1)
        {
            args.Player.SendErrorMessage("未找到物品");
            return;
        }
        var item = itemByIdOrName[0];
        var text2 = (args.Parameters.Count > 1) ? args.Parameters[1] : "c";
        switch (text2.ToLower())
        {
            case "r":
                args.Player.SendInfoMessage(this.GetRecipeStringByRequired(item));
                return;
        }
        var list = Main.recipe.ToList().FindAll((Recipe r) => r.createItem.type == item.type);
        var text3 = $"物品:{Lang.GetItemNameValue(item.type)}[i:{item.type}]\n";
        if (list.Count > 1)
        {
            for (var j = 0; j < list.Count; j++)
            {
                text3 += $"配方{j + 1}:\n";
                text3 += this.GetRecipeStringByResult(list[j]);
            }
        }
        else
        {
            if (list.Count != 1)
            {
                args.Player.SendErrorMessage("此物品无配方");
                return;
            }
            text3 += this.GetRecipeStringByResult(list[0]);
        }
        args.Player.SendInfoMessage(text3);
    }

    //释放
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.FindRecipe);
            MapHelper.Initialize();
            BuildMapAtlas();
        }
        base.Dispose(disposing);
    }

    private string GetRecipeStringByResult(Recipe recipe)
    {
        var text = "材料：";
        foreach (var item in recipe.requiredItem.Where((Item r) => r.stack > 0))
        {
            text += string.Format("{0}{1}[i/s{2}:{3}] ", Lang.GetItemNameValue(item.type), (item.maxStack == 1 || item.stack == 0) ? "" : ("*" + item.stack), item.stack, item.type);
        }
        text += "\n";
        text += "需要的合成站：";
        foreach (var item2 in recipe.requiredTile.Where((int i) => i >= 0))
        {
            text += $"{Lang._mapLegendCache[MapHelper.tileLookup[item2]]} ";
        }
        if (text.Last() == '：')
        {
            text += "徒手";
        }
        text += "\n";
        if (recipe.needHoney)
        {
            text += "需要蜂蜜\n";
        }
        if (recipe.needWater)
        {
            text += "需要水\n";
        }
        if (recipe.needLava)
        {
            text += "需要岩浆\n";
        }
        return text + "\n";
    }

    private string GetRecipeStringByRequired(Item item)
    {
        var text = "可合成的物品:\n";
        var source = from r in Main.recipe
                     where r.requiredItem.Select((Item i) => i.type).Contains(item.type)
                     select r.createItem;
        for (var j = 1; j <= source.Count(); j++)
        {
            var val = source.ElementAt(j - 1);
            text += string.Format("{0}{1}[i/s{2}:{3}],{4}", Lang.GetItemNameValue(val.type), (val.maxStack > 1) ? ("*" + val.stack) : "", val.stack, val.type, (j % 5 == 0) ? "\n" : "");
        }
        text.Trim(',');
        return text + "\n";
    }
}