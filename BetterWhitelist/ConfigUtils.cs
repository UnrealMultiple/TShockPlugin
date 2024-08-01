using System;
using System.Collections.Generic;

namespace BetterWhitelist
{
	public class Translation
	{
		public static Translation Load(string path)
		{
			return new Translation
			{
				language = 
				{
					{
                        "删除成功提示",
						"删除成功!"
					},
					{
                        "添加成功提示",
						"添加成功!"
					},
					{
                        "启用成功提示",
						"启用成功!"
					},
					{
                        "禁用成功提示",
						"禁用成功!"
					},
					{
                        "重载成功提示",
						"重载成功!"
					},
					{
						"添加失败提示",
						"添加失败! 该玩家已经在白名单中"
					},
					{
						"删除失败提示",
						"删除失败 ! 该玩家不在白名单中"
					},
					{
						"启用失败提示",
						"启用失败 ! 插件已打开"
					},
					{
						"禁用失败提示",
						"禁用失败 ! 插件已关闭"
					},
					{
						"删除白名单后断开玩家连接提示",
						"你已被移出白名单!"
					},
					{
						"未知指令提示文本",
						"用法: 输入 /bwl help 显示帮助信息."
					},
					{
						"未启用插件提示",
						"插件开关已被禁用，请检查配置文件!"
					},
					{
						"全部的指令帮助提示",
                        "/bwl help, 显示帮助信息\n/bwl add {name}, 添加玩家名到白名单中\n/bwl del {name}, 将玩家移出白名单\n/bwl list, 显示白名单上的全部玩家\n/bwl true, 启用插件\n/bwl false, 关闭插件\n/bwl reload, 重载插件"
                    },
					{
						"连接时不在白名单提示",
						"你不在服务器白名单中！"
					}
				}
			};
		}

		public Dictionary<string, string> language = new Dictionary<string, string>();
	}
}
