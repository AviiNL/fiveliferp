using CitizenFX.Core;
using FiveLife.NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.CharacterCreator.Menu
{
    class CustomizeMenu : UIMenu
    {
        Menu menu;
        Ped selectedPed;

        public CustomizeMenu(Menu menu)
           : base("", "Customize")
        {
            this.menu = menu;
        }


        public override void OnActivated()
        {
            MenuItems.Clear();

            selectedPed = CitizenFX.Core.Game.Player.Character;
            Debug.WriteLine(((PedHash)selectedPed.Model.Hash).ToString());

            if (((PedHash)selectedPed.Model.Hash).ToString() == "FreemodeMale01" || ((PedHash)selectedPed.Model.Hash).ToString() == "FreemodeFemale01")
            {
                Debug.WriteLine("Freemode");

                var heritageMenu = new UIMenuItem("Heritage");
                heritageMenu.Activated += HeritageMenu_Activated;
                AddItem(heritageMenu);

                var clothingMenu = new UIMenuItem("Clothing");
                clothingMenu.Activated += ClothingMenu_Activated;
                AddItem(clothingMenu);

            }
            else
            {
                // Check if ped has variants, add items. (make sure to set default value to previously selected value!!!!!!)
                Debug.WriteLine("Ped");
                // add menus for ped customization
            }


        }

        private void HeritageMenu_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            menu.Open("freemode_heritage");
        }

        private void ClothingMenu_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            menu.Open("freemode_clothing");
        }
    }
}
