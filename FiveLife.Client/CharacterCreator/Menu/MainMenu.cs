using FiveLife.Client.CharacterCreator.Elements;
using FiveLife.NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.CharacterCreator.Menu
{
    public class MainMenu : UIMenu
    {
        public delegate void PedChanged(CitizenFX.Core.Ped ped);
        public event PedChanged OnPedChanged;
        private Menu menu;

        public MainMenu(Menu menu)
            : base("Character Creator", "")
        {
            this.menu = menu;

            var pedList = new PedList(this);

            var customize = new UIMenuItem("Customize");
            var play = new UIMenuItem("Play");
            var cancel = new UIMenuItem("Cancel");

            customize.Activated += Customize_Activated;

            AddItem(pedList);
            AddItem(customize);
            AddItem(play);
            AddItem(cancel);
        }

        private void Customize_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (!Visible) return;
            menu.Open("customize");
        }

        public void PedChange(CitizenFX.Core.Ped ped)
        {
            if (!Visible) return;
            OnPedChanged?.Invoke(ped);
        }

    }
}
