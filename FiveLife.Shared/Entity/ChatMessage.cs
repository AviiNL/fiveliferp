using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Shared.Entity
{
    public class ChatMessage
    {
        public Shared.Entity.Character Character { get; set; }
        public string Message { get; set; }
        public Vector3 Position { get; set; }
    }
}
