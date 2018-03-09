using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Shared.Entity
{
    public class Player : IEntity
    {
        public int Id { get; set; }
        public string SteamId { get; set; }
        public virtual List<Character> Characters { get; set; }
        public ERank Rank { get; set; }
        public int Priority { get; set; }
        public string Notes { get; set; }


        // Operators
        //public override int GetHashCode()
        //{
        //    return Id;
        //}

        //public override bool Equals(object obj)
        //{
        //    var player = obj as Player;
        //    return player != null &&
        //           Id == player.Id;
        //}

        //public static bool operator ==(Player a, Player b)
        //{
        //    return a.Id == b.Id;
        //}

        //public static bool operator !=(Player a, Player b)
        //{
        //    return a.Id != b.Id;
        //}
    }
}
