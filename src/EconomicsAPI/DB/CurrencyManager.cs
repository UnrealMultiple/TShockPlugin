﻿using EconomicsAPI.Configured;
using EconomicsAPI.Enumerates;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using EconomicsAPI.Events;
using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace EconomicsAPI.DB;

public class CurrencyManager
{
    public class PlayerCurrency
    {
        public long Number { get; set; }

        public string CurrencyType { get; set; }

        public string PlayerName { get; set; }

        public PlayerCurrency(long num, string player, string type)
        {
            this.Number = num;
            this.CurrencyType = type;
            this.PlayerName = player;
        }

    }
    private readonly List<PlayerCurrency> Currencys = new();

    public IDbConnection database;

    internal CurrencyManager()
    {
        this.database = TShock.DB;
        var Skeleton = new SqlTable("economics",
            new SqlColumn("id", MySqlDbType.Int32) { Length = 8, Primary = true, Unique = true, AutoIncrement = true },
            new SqlColumn("username", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("currency", MySqlDbType.Int64) { Length = 255 },
            new SqlColumn("type", MySqlDbType.Text) { Length = 255 });
        var List = new SqlTableCreator(this.database, this.database.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(Skeleton);
        using (var reader = this.database.QueryReader("SELECT * FROM economics"))
        {
            while (reader.Read())
            {
                var username = reader.Get<string>("username");
                var currency = reader.Get<long>("currency");
                var type = reader.Get<string>("type");
                this.Currencys.Add(new PlayerCurrency(currency, username, type));
            }
        }
    }

    public void UpdataAll()
    {
        foreach (var curr in this.Currencys)
        {
            if (!this.Update(curr))
            {
                this.Write(curr);
            }
        }
    }

    public bool Update(string UserName, string type)
    {
        return this.database.Query("UPDATE `economics` SET `currency` = @0 WHERE `economics`.`username` = @1 AND `economics`.`type` = @2", this.GetUserCurrency(UserName, type).Number, UserName, type) == 1;
    }
    public bool Update(PlayerCurrency currency)
    {
        return this.Update(currency.PlayerName, currency.CurrencyType);
    }

    public bool Write(string UserName, string type)
    {
        return this.database.Query("INSERT INTO `economics` (`id`, `username`, `currency`, `type`) VALUES (NULL, @0, @1, @2)", UserName, this.GetUserCurrency(UserName, type).Number, type) == 1;
    }

    public bool Write(PlayerCurrency currency)
    {
        return this.Write(currency.PlayerName, currency.CurrencyType);
    }

    public PlayerCurrency GetUserCurrency(string name, string type)
    {
        return this.Currencys.Find(x => x.PlayerName == name && x.CurrencyType == type) ?? new(0, name, type);
    }

    public void AddUserCurrency(string name, params RedemptionRelationshipsOption[] options)
    {
        foreach (var option in options)
        {
            this.AddUserCurrency(name, option.Number, option.CurrencyType);
        }
    }

    public void AddUserCurrency(string name, long amount, string type)
    {
        var player = Economics.ServerPlayers.Find(x => x.Name == name && x.Active);
        if (player != null)
        {
            var args = new PlayerCurrencyUpdateArgs()
            {
                Change = amount,
                CurrentType = CurrencyUpdateType.Added,
                Player = player,
                Current = this.GetUserCurrency(name, type).Number
            };
            if (PlayerHandler.PlayerCurrencyUpdate(args))
            {
                return;
            }

            amount = args.Change;
        }
        this.Add(name, amount, type);
    }

    private void Add(string name, long amount, string type)
    {
        var playercurr = this.Currencys.Find(x => x.PlayerName == name && x.CurrencyType == type);
        if (playercurr is null)
        {
            playercurr = new PlayerCurrency(amount, name, type);
            this.Currencys.Add(playercurr);
        }
        else
        {
            playercurr.Number += amount;
        }
    }

    public void ClearUserCurrency(string name, string type)
    {
        this.GetUserCurrency(name, type).Number = 0;
    }

    private bool Deduct(string name, long amount, string type)
    {
        if (amount == 0)
        {
            return true;
        }
        var playerCurrency = this.GetUserCurrency(name, type);
        if (playerCurrency.Number >= amount)
        {
            playerCurrency.Number -= amount;
            return true;
        }
        return false;
    }

    [Obsolete("use DeductUserCurrency instead")]
    public bool DelUserCurrency(string name, long amount, string type)
    {
        return this.DeductUserCurrency(name, amount, type);
    }

    public bool DeductUserCurrency(string name, IEnumerable<RedemptionRelationshipsOption> RedemptionRelationships, int count = 1)
    {
        var success = RedemptionRelationships.Where(r => this.GetUserCurrency(name, r.CurrencyType).Number * count >= r.Number);
        if (success.Count() != RedemptionRelationships.Count())
        {
            return false;
        }
        foreach (var rro in success)
        {
            this.DeductUserCurrency(name, rro.Number, rro.CurrencyType);
        }
        return true;
    }

    public bool DeductUserCurrency(string name, params RedemptionRelationshipsOption[] options)
    {
        var success = options.Where(r => this.GetUserCurrency(name, r.CurrencyType).Number >= r.Number);
        if (success.Count() != options.Count())
        {
            return false;
        }
        foreach (var rro in success)
        {
            this.DeductUserCurrency(name, rro.Number, rro.CurrencyType);
        }
        return true;
    }

    public bool DeductUserCurrency(string name, long amount, string type)
    {
        var player = Economics.ServerPlayers.Find(x => x.Name == name && x.Active);
        if (player != null)
        {
            var args = new PlayerCurrencyUpdateArgs()
            {
                Change = amount,
                CurrentType = CurrencyUpdateType.Delete,
                Player = player,
                Current = this.GetUserCurrency(name, type).Number
            };
            if (PlayerHandler.PlayerCurrencyUpdate(args))
            {
                return true;
            }

            amount = args.Change;
        }
        return this.Deduct(name, amount, type);
    }

    public void Reset()
    {
        this.database.Query("delete from Economics");
        this.Currencys.Clear();
    }
}