# LazyAPI 基础库

- 作者: [cc004](https://github.com/cc004), UnrealMultiple 成员
- 原始出处: [Github](https://github.com/cc004/LazyUtils)
- 插件基础库

## 命令行参数
- `-culture zh|en` 此命令行参数用于切换生成配置文件的语言，默认跟随ts语言。

## 自动注册指令详解

### 类声明
如此声明一个类，给类添加一个CommandAttribute，LazyAPI会在插件实例化阶段进行检测并进行下一步操作。
```csharp
[Command("test")]
public class TestCommand
{

}
```

## 指令方法
声明的方法必须为静态且公开的，第一个参数类型必须是`CommandArgs`，否则不会加载或出错!
```csharp
[Command("test")]
public class TestCommand
{
	public static void Run(CommandArgs args)
	{
		...方法内部实现...
	}
}
```

### 子命令
以下示例，在被加载后`test`命令会出现两个子命令`/test run` 和 `/test go`，默认情况下子命令以小写的方法名命名，第二条则是因为添加了`AliasAttribute`
```csharp
[Command("test")]
public class TestCommand
{
	public static void Run(CommandArgs args)
	{
		...方法内部实现...
	}

	[Alias("go")]
	public static void Execute(CommandArgs args)
	{
		...方法内部实现...
	}
}
```

如果不希望其成为子命令则只需要添加一个`MainAttribute`，这样当你运行`/test`就不存在子命令可以直接运行
```csharp
[Command("test")]
public class TestCommand
{
	[Main]
	public static void Execute(CommandArgs args)
	{
		...方法内部实现...
	}
}
```

### 其他Attribute
`PermissionsAttribute`为指令附加了权限，如此一来便不需要再使用HasPermission(...)来进行判断<br>
`RealPlayerAttribute`则是为了防止机器人或其他非玩家的用户执行指令，LazyAPI会自动判断并阻止执行。<br>
`FlexibleAttribute`的存在则是为了处理翻页Page的情况，后续将会提到。
```csharp
[Command("test")]
[Permissions("test")]
public class TestCommand
{
	[Flexible]
	[RealPlayer]
	[Permissions("test.use")]
	public static void Execute(CommandArgs args)
	{
		...方法内部实现...
	}
}
```
### 参数匹配
LazyAPI的指令系统拥有参数匹配功能<br>
以下示例中，方法多了一个`string`和`int`类型参数，LazyAPI在处理指令时，会从args.Parameters中找到这些参数并尝试转换。<br>
并且在默认情况下，参数是严格匹配的，例如输入指令的参数和声明函数参数数量不同时不会执行，若参数转换失败同样如此。<br>
可自动处理的参数类型:`int`，`long`，`TSPlayer`，`bool`，`string`，`UserAccount`，`DateTime`
```csharp
[Command("test")]
[Permissions("test")]
public class TestCommand
{
	[Permissions("test.use")]
	public static void Execute(CommandArgs args, string name, int age)
	{
		...方法内部实现...
	}
}
```
### FlexibleAttribute
为了处理那些特殊指令，例如:`test list [页码]`，因为参数匹配限制，所以添加了这个Attribute，你只需给方法添加它，那么LazyAPI就不会那么严格要求参数数量，只要大于方法参数数量即可。
```csharp
[Command("test")]
[Permissions("test")]
public class TestCommand
{
	[Flexible]
	[Permissions("test.list")]
	public static void List(CommandArgs args)
	{
		var OnlineInfo = PlayerOnline.GetOnlineRank().Select(online => GetString($"{online.Name} 在线时长: {Math.Ceiling(Convert.ToDouble(online.Duration * 1.0f / 60))}分钟").Color(TShockAPI.Utils.GreenHighlight)).ToList();
        args.SendPage(OnlineInfo, 1, new PaginationTools.Settings
        {
            MaxLinesPerPage = 30,
            NothingToDisplayString = GetString("当前没有玩家在线数据"),
            HeaderFormat = GetString("在线排行 ({0}/{1})："),
            FooterFormat = GetString("输入 {0}在线排行 {{0}} 查看更多").SFormat(Commands.Specifier)
        });
	}
}
```

## Rest
关于Rest LazyAPI的处理与指令基本相同，可以参考ServerTool中的示例。

## 更新日志

### v1.0.3.0
- 升级Linq2db -> v6

### v1.0.2.0
- 修复在构造函数阶段使用TShock.Log
- 添加FindCommand
- 添加RestPathAttribute，FindAttribute
- RestHelper可以自定义method路径

### v1.0.1.0
- 修复`TShockAPI.I18n.TranslationCultureInfo`为空时, 无法正确生成配置

### v1.0.0.7
- 添加游戏进度工具类

### v1.0.0.6
- Database/DB.cs 工具集设置 tableName 为可选
- 添加配置文件重载提示

### v1.0.0.4
- 支持西班牙语，俄语

### v1.0.0.2
- 支持生成默认配置，多语言更多操作的支持!

### v1.0.0.1
- 初步支持多语言配置文件

### v1.0.0.0
- 初始上传

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
