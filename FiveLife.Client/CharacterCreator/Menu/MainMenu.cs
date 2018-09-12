using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
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
        private Menu menu;

        public MainMenu(Menu menu)
            : base("", "Character Creator")
        {
            this.menu = menu;

            var pedList = new PedList(this);

            var customize = new UIMenuItem("Customize");
            var finish = new UIMenuItem("Finish");
            var cancel = new UIMenuItem("Cancel");

            customize.Activated += Customize_Activated;
            finish.Activated += Finish_Activated;
            cancel.Activated += Cancel_Activated;

            AddItem(pedList);
            AddItem(customize);
            AddItem(finish);
            AddItem(cancel);
        }

        private void Cancel_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (!Visible) return;
            Function.Call(Hash.ACTIVATE_FRONTEND_MENU, -1171018317, 0, 42);
        }

        private void Finish_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (!Visible) return;

            Screen.Effects.Start(ScreenEffect.SwitchHudIn, 0, false);
            NUI.Open(NUI.Page.IDForm, Game.Data.Character);

            // open NUI set name and stuff
        }

        private void Customize_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (!Visible) return;
            menu.Open("customize");
        }

    }
}
