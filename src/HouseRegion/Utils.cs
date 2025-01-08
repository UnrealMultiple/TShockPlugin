using Microsoft.Xna.Framework;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using TShockAPI;
using TShockAPI.DB;


namespace HouseRegion;

class Utils//专属工具包
{
    public static int MaxCount(TSPlayer ply)//通过正则获取权限中自定义房屋数量最大值
    {
        for (var i = 0; i < ply.Group.permissions.Count; i++)
        {
            var perm = ply.Group.permissions[i];
            var Match = Regex.Match(perm, @"^house\.count\.(\d{1,9})$");
            if (Match.Success)
            {
                return Convert.ToInt32(Match.Groups[1].Value);
            }
        }
        return Config.Instance.HouseMaxNumber;
    }
    public static int MaxSize(TSPlayer ply)//取最大尺寸
    {
        for (var i = 0; i < ply.Group.permissions.Count; i++)
        {
            var perm = ply.Group.permissions[i];
            var Match = Regex.Match(perm, @"^house\.size\.(\d{1,9})$");//正则
            if (Match.Success)
            {
                return Convert.ToInt32(Match.Groups[1].Value);
            }
        }
        return Config.Instance.HouseMaxSize;//没有权限指定则返回配置的 内容
    }
    public static House? GetHouseByName(string name)//取得指定名字的房子
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        for (var i = 0; i < HousingPlugin.Houses.Count; i++)
        {
            var house = HousingPlugin.Houses[i];
            if (house == null)
            {
                continue;
            }

            if (house.Name == name)
            {
                return house;
            }
        }
        return null;
    }

    public static bool OwnsHouse(UserAccount U, string housename)//判断是否为房屋的所有者
    {
        return U != null && OwnsHouse(U.ID.ToString(), housename);
    }

    public static bool OwnsHouse(UserAccount U, House house)//判断是否为房屋的所有者
    {
        return U != null && OwnsHouse(U.ID.ToString(), house);
    }

    public static bool OwnsHouse(string UserID, string housename)//判断是否为房屋的所有者
    {
        if (string.IsNullOrWhiteSpace(UserID) || UserID == "0" || string.IsNullOrEmpty(housename))
        {
            return false;
        }

        var H = GetHouseByName(housename);//各种排错之后看看这个房子在不在
        return H != null && OwnsHouse(UserID, H);
    }
    public static bool OwnsHouse(string UserID, House house)//判断是否为房屋的所有者
    {
        if (!string.IsNullOrEmpty(UserID) && UserID != "0" && house != null)
        {
            try
            {
                return house.Owners.Contains(UserID);
            }
            catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误超标错误:") + ex.ToString()); return false; }
        }
        return false;
    }
    public static bool CanUseHouse(string UserID, House house)//是否为使用者
    {
        return !string.IsNullOrEmpty(UserID) && UserID != "0" && house.Users.Contains(UserID);
    }

    public static bool CanUseHouse(UserAccount U, House house)//形式不一样
    {
        return U != null && U.ID != 0 && house.Users.Contains(U.ID.ToString());
    }
    public static string? InAreaHouseName(int x, int y)//指定位置的房子名字
    {
        for (var i = 0; i < HousingPlugin.Houses.Count; i++)
        {
            var house = HousingPlugin.Houses[i];
            if (house == null)
            {
                continue;
            }

            if (x >= house.HouseArea.Left && x < house.HouseArea.Right &&
                y >= house.HouseArea.Top && y < house.HouseArea.Bottom)
            {
                return house.Name;
            }
        }
        return null;
    }
    public static House? InAreaHouse(int x, int y)//指定位置的房子
    {
        for (var i = 0; i < HousingPlugin.Houses.Count; i++)
        {
            var house = HousingPlugin.Houses[i];
            if (house == null)
            {
                continue;
            }

            if (x >= house.HouseArea.Left && x < house.HouseArea.Right &&
                y >= house.HouseArea.Top && y < house.HouseArea.Bottom)
            {
                return house;
            }
        }
        return null;
    }
}
public class HouseManager//房屋管理
{
    const string cols = "Name, TopX, TopY, BottomX, BottomY, Author, Owners, WorldID, Locked, Users";
    public static bool AddHouse(int tx, int ty, int width, int height, string housename, string author)
    {
        var locked = 1;
        if (Utils.GetHouseByName(housename) != null)
        {
            return false;
        }

        try
        {
            TShock.DB.Query("INSERT INTO HousingDistrict (" + cols + ") VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9);", housename, tx, ty, width, height, author, "", Main.worldID.ToString(), locked, "");
        }
        catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误数据库写入错误:") + ex.ToString()); return false; }
        HousingPlugin.Houses.Add(new House(new Rectangle(tx, ty, width, height), author, new List<string>(), housename, locked == 1, new List<string>()));
        return true;
    }
    public static bool AddNewOwner(string houseName, string id)
    {
        var house = Utils.GetHouseByName(houseName);
        if (house == null)
        {
            return false;
        }

        var sb = new StringBuilder();//某些人
        var count = 0; house.Owners.Add(id);
        for (var i = 0; i < house.Owners.Count; i++)
        {
            var owner = house.Owners[i];
            count++; sb.Append(owner);
            if (count != house.Owners.Count)
            {
                sb.Append(',');//添加分隔符
            }
        }
        try
        {
            var query = "UPDATE HousingDistrict SET Owners=@0 WHERE Name=@1";
            TShock.DB.Query(query, sb.ToString(), houseName);
        }
        catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误数据库修改错误:") + ex.ToString()); return false; }
        return true;
    }
    public static bool AddNewUser(string houseName, string id)//添加使用者
    {
        var house = Utils.GetHouseByName(houseName);
        if (house == null)
        {
            return false;
        }

        var sb = new StringBuilder();//某些人
        var count = 0; house.Users.Add(id);
        for (var i = 0; i < house.Users.Count; i++)
        {
            var user = house.Users[i];
            count++; sb.Append(user);
            if (count != house.Users.Count)
            {
                sb.Append(',');//添加分隔符
            }
        }
        try
        {
            var query = "UPDATE HousingDistrict SET Users=@0 WHERE Name=@1";
            TShock.DB.Query(query, sb.ToString(), houseName);
        }
        catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误数据库修改错误:") + ex.ToString()); return false; }
        return true;
    }
    public static bool DeleteOwner(string houseName, string id)//删除所有者
    {
        var house = Utils.GetHouseByName(houseName);
        if (house == null)
        {
            return false;
        }

        var sb = new StringBuilder();
        var count = 0; house.Owners.Remove(id);
        for (var i = 0; i < house.Owners.Count; i++)
        {
            var owner = house.Owners[i];
            count++; sb.Append(owner);
            if (count != house.Owners.Count)
            {
                sb.Append(',');
            }
        }
        try
        {
            var query = "UPDATE HousingDistrict SET Owners=@0 WHERE Name=@1";
            TShock.DB.Query(query, sb.ToString(), houseName);
        }
        catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误数据库修改错误:") + ex.ToString()); return false; }
        return true;
    }
    public static bool DeleteUser(string houseName, string id)//删除使用者
    {
        var house = Utils.GetHouseByName(houseName);
        if (house == null)
        {
            return false;
        }

        var sb = new StringBuilder();
        var count = 0; house.Users.Remove(id);
        for (var i = 0; i < house.Users.Count; i++)
        {
            var users = house.Users[i];
            count++; sb.Append(users);
            if (count != house.Users.Count)
            {
                sb.Append(',');
            }
        }
        try
        {
            var query = "UPDATE HousingDistrict SET Users=@0 WHERE Name=@1";
            TShock.DB.Query(query, sb.ToString(), houseName);
        }
        catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误数据库修改错误:") + ex.ToString()); return false; }
        return true;
    }
    public static bool RedefineHouse(int tx, int ty, int width, int height, string housename)
    {
        try
        {
            var house = Utils.GetHouseByName(housename);
            var houseName = house!.Name;
            try
            {
                var query = "UPDATE HousingDistrict SET TopX=@0, TopY=@1, BottomX=@2, BottomY=@3, WorldID=@4 WHERE Name=@5";
                TShock.DB.Query(query, tx, ty, width, height, Main.worldID.ToString(), house.Name);
            }
            catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误数据库修改错误:") + ex.ToString()); return false; }
            house.HouseArea = new Rectangle(tx, ty, width, height);
        }
        catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误重新定义房屋时出错:") + ex.ToString()); return false; }
        return true;
    }
    public static bool ChangeLock(House house)
    {
        house.Locked = !house.Locked;

        try
        {
            var query = "UPDATE HousingDistrict SET Locked=@0 WHERE Name=@1";
            TShock.DB.Query(query, house.Locked ? 1 : 0, house.Name);
        }
        catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误修改锁房屋时出错:") + ex.ToString()); return false; }
        return true;
    }
}