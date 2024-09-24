

using System.ComponentModel;

namespace AutoPluginManager;
public enum Platforms 
{

    [Description("win-x64")]
    WindowsX64,

    [Description("linux-x64")]
    Linux64,

    [Description("win-x86")]
    WindowX86,

    [Description("linux-arm")]
    LinuxArm,

    [Description("linux-arm64")]
    LinuxArm64,

    [Description("osx-arm")]
    OsxArm,

    [Description("osx-arm64")]
    OsxArm64,

    [Description("linux-musl-arm64")]
    LinuxMuslArm64,

    [Description("linux-musl-x64")]
    LinuxMuslX64
}
