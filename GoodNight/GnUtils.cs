using Newtonsoft.Json;

namespace Goodnight
{
    internal class GnUtils
    {
        //获取断连豁免名单中的名字
        internal bool Exempt(string Name) => Goodnight.Config.PlayersList.Contains(Name);

        //列出豁免名单
        public string GetList() => JsonConvert.SerializeObject(Goodnight.Config.PlayersList, (Formatting)1);

        //添加豁免名单名字
        public bool Add(string name)
        {
            if (Exempt(name))
            {
                return false;
            }
            Goodnight.Config.PlayersList.Add(name);
            Goodnight.Config.Write(Configuration.FilePath);
            return true;
        }

        //移除豁免名单名字
        public bool Del(string name)
        {
            if (Exempt(name))
            {
                Goodnight.Config.PlayersList.Remove(name);
                Goodnight.Config.Write(Configuration.FilePath);
                return true;
            }
            return false;
        }

    }
}
