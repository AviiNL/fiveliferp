using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Shared.Entity
{
    public class Room : IEntity
    {
        public Room ()
        {
            this.CurrentlyInside = new List<int>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Character Owner { get; set; }
        public List<int> CurrentlyInside { get; set; }
        public bool IsPrivate { get; set; }
        public virtual ICollection<Character> Allowed { get; set; }
        
        public string IPL { get; set; }

        public float InsideX {get;set;}
        public float InsideY { get; set; }
        public float InsideZ { get; set; }
        public float InsideHeading { get; set; }

        public float OutsideX { get; set; }
        public float OutsideY { get; set; }
        public float OutsideZ { get; set; }
        public float OutsideHeading { get; set; }

        public float InsideSpawnX { get; set; }
        public float InsideSpawnY { get; set; }
        public float InsideSpawnZ { get; set; }
        public float InsideSpawnHeading { get; set; }

        public float OutsideSpawnX { get; set; }
        public float OutsideSpawnY { get; set; }
        public float OutsideSpawnZ { get; set; }
        public float OutsideSpawnHeading { get; set; }

        // public bool HasBlip { get; set; } ?
        public int BlipSprite { get; set; }
        public int BlipColor { get; set; }

        // Operators
        //public override int GetHashCode()
        //{
        //    return Id;
        //}

        //public override bool Equals(object obj)
        //{
        //    var room = obj as Room;
        //    return room != null &&
        //           Id == room.Id;
        //}

        //public static bool operator ==(Room a, Room b)
        //{
        //    return a.Id == b.Id;
        //}

        //public static bool operator !=(Room a, Room b)
        //{
        //    return a.Id != b.Id;
        //}
    }
}

