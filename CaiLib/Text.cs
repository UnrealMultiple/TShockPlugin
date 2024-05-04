using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TShockAPI;

namespace CaiLib
{
    public static class CaiText
    {
        public static readonly string[] gradienColor = new string[] {
            "[c/40c900:{0}]",
            "[c/00c427:{0}]",
            "[c/00be3b:{0}]",
            "[c/00b650:{0}]",
            "[c/00ad5f:{0}]",
            "[c/00a569:{0}]",
            "[c/009d70:{0}]",
            "[c/009575:{0}]",
            "[c/008d78:{0}]",
            "[c/008579:{0}]",
            "[c/007e7a:{0}]",
            "[c/007779:{0}]",
            "[c/007078:{0}]",
            "[c/006976:{0}]",
            "[c/006373:{0}]",
            "[c/005c71:{0}]",
            "[c/00556e:{0}]",
            "[c/004f6b:{0}]",
            "[c/004765:{0}]",
            "[c/00405c:{0}]"
        };

        public static void SendGradientMessage(this TSPlayer player, string msg)
        {
            player.SendSuccessMessage(GetGradientText(msg));
        }
        public static string GetGradientText(string text)
        {
            string result = "";
            if (new Regex("\\[i(tem)?(?:\\/s(?<Stack>\\d{1,4}))?(?:\\/p(?<Prefix>\\d{1,3}))?:(?<NetID>-?\\d{1,4})\\]").Match(result).Success)
            {
                return text;
            }
            else
            {
                //Random random = new Random();

                //foreach (char c in args.Text)
                //{
                //    Chat += "[c/" + random.Next(0, 16777215).ToString("x8") + ":" + c.ToString() + "]";
                //}
                int index = 0;
                bool isRight = true;
                for (int i = 0; i < text.Length; i++)
                {
                    result += string.Format(gradienColor[index], text[i]);
                    if (isRight && index == gradienColor.Length - 1)
                    {
                        isRight = false;
                        index--;
                    }
                    else if (!isRight && index == 0)
                    {
                        isRight = true;
                        index++;
                    }
                    else if (isRight)
                    {
                        index++;
                    }
                    else
                    {
                        index--;
                    }
                }
            }
            return result;
        }
    }
}
