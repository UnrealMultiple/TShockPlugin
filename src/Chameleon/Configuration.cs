using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace Chameleon;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "Chameleon";
    [LocalizedPropertyName(CultureType.Chinese, "验证登录IP是否一致", Order = -3)]
    [LocalizedPropertyName(CultureType.English, "VerifyloginIP", Order = -3)]
    public bool VerifyloginIP { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "等待列表长度")]
    [LocalizedPropertyName(CultureType.English, "AwaitBufferSize")]
    public ushort AwaitBufferSize { get; set; } = Chameleon.Size;

    [LocalizedPropertyName(CultureType.Chinese, "启用强制提示显示")]
    [LocalizedPropertyName(CultureType.English, "EnableForcedHint")]
    public bool EnableForcedHint { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "强制提示欢迎语")]
    [LocalizedPropertyName(CultureType.English, "Greeting")]
    public string Greeting { get; set; } = "   欢迎来到茑萝服务器！";

    [LocalizedPropertyName(CultureType.Chinese, "验证失败提示语")]
    [LocalizedPropertyName(CultureType.English, "VerificationFailedMessage")]
    public string VerificationFailedMessage { get; set; } = "         账户密码错误。\r\n\r\n         若你第一次进服，请换一个人物名；\r\n         若忘记密码，请联系管理。";

    [LocalizedPropertyName(CultureType.Chinese, "强制提示文本")]
    [LocalizedPropertyName(CultureType.English, "Hints")]
    public string[] Hints { get; set; } = new[]
    {
        " ↓↓ 请看下面的提示以进服 ↓↓",
        " \r\n         看完下面的再点哦→",
        " 1. 在\"服务器密码\"中输入自己的密码, 以后加服时输入这个密码即可\n相当于为你自己的账号设置密码",
        " 2. 阅读完毕后请重新加入"
    };

    [LocalizedPropertyName(CultureType.Chinese, "同步注册服务器")]
    [LocalizedPropertyName(CultureType.English, "SyncServerReg")]
    public List<RESTser> SyncServerReg { get; set; } = new List<RESTser>
    {
        new RESTser
        {
            Name = "测试服务器",
            IPAddress = "http://127.0.0.1:7878/",
            Token = "TOKENTEST"
        }
    };

    public class RESTser
    {
        [LocalizedPropertyName(CultureType.Chinese, "名称")]
        [LocalizedPropertyName(CultureType.English, "Name")]
        public string Name { get; set; } = "";

        [LocalizedPropertyName(CultureType.Chinese, "地址")]
        [LocalizedPropertyName(CultureType.English, "IpAddress")]
        public string IPAddress { get; set; } = "";

        public string Token { get; set; } = "";
    }

    protected override void SetDefault()
    {
        this.AwaitBufferSize = Chameleon.Size;
        this.EnableForcedHint = true;
        this.Greeting = "   欢迎来到茑萝服务器！";
        this.VerificationFailedMessage = "         账户密码错误。\r\n\r\n         若你第一次进服，请换一个人物名；\r\n         若忘记密码，请联系管理。";
        this.Hints = new[]
        {
            " ↓↓ 请看下面的提示以进服 ↓↓",
            " \r\n         看完下面的再点哦→",
            " 1. 在\"服务器密码\"中输入自己的密码, 以后加服时输入这个密码即可，相当于为你自己的账号设置密码",
            " 2. 阅读完毕后请重新加入"
        };
        this.SyncServerReg = new List<RESTser>
        {
            new RESTser
            {
                Name = "测试服务器",
                IPAddress = "http://127.0.0.1:7878/",
                Token = "TOKENTEST"
            }
        };
    }

}