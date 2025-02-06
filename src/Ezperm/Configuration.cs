using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace Ezperm;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "ezperm";
    internal class GroupInfo
    {
        [LocalizedPropertyName(CultureType.Chinese, "组名字")]
        [LocalizedPropertyName(CultureType.English, "Name")]
        public string Name { get; set; } = "";

        [LocalizedPropertyName(CultureType.Chinese, "父组")]
        [LocalizedPropertyName(CultureType.English, "Parent")]
        public string Parent { get; set; } = "";

        [LocalizedPropertyName(CultureType.Chinese, "添加的权限")]
        [LocalizedPropertyName(CultureType.English, "AddPermissions")]
        public List<string> AddPermissions { get; set; } = new List<string>();

        [LocalizedPropertyName(CultureType.Chinese, "删除的权限")]
        [LocalizedPropertyName(CultureType.English, "DelPermissions")]
        public List<string> DelPermissions { get; set; } = new List<string>();
    }

    [LocalizedPropertyName(CultureType.Chinese, "组列表", Order = -3)]
    [LocalizedPropertyName(CultureType.English, "Groups", Order = -3)]
    public List<GroupInfo> Groups { get; set; } = new List<GroupInfo>();

    protected override void SetDefault()
    {
        this.Groups = new List<GroupInfo>
        {
            new GroupInfo
            {
                Name = "default",
                Parent = "guest",
                AddPermissions = new List<string>
                {
                    "tshock.world.movenpc",
                    "tshock.world.time.usesundial",
                    "tshock.tp.pylon",
                    "tshock.tp.demonconch",
                    "tshock.tp.magicconch",
                    "tshock.tp.tppotion",
                    "tshock.tp.rod",
                    "tshock.tp.wormhole",
                    "tshock.npc.startdd2",
                    "tshock.npc.spawnpets",
                    "tshock.npc.summonboss",
                    "tshock.npc.startinvasion",
                    "tshock.npc.hurttown"
                },
                DelPermissions = new List<string> { "tshock.admin" }
            }
        };
    }
}