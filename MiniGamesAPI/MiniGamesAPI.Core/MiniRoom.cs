using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesAPI
{
    public class MiniRoom
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int WaitingTime { get; set; }

        public int GamingTime { get; set; }

        public int SeletingTime { get; set; }

        public int MaxPlayer { get; set; }

        public int MinPlayer { get; set; }

        public Enum.RoomStatus Status { get; set; }

        public MiniRoom(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
