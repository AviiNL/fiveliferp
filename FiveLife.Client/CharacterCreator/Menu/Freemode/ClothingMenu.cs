using FiveLife.NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.CharacterCreator.Menu.Freemode
{
    public class ClothingMenu : UIMenu
    {
        Menu menu;

        public ClothingMenu(Menu menu)
            : base("", "Clothing")
        {
            this.menu = menu;

        }
    }
}
