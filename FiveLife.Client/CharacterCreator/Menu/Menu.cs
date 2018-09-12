using FiveLife.NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.CharacterCreator.Menu
{
    public class Menu
    {
        private MenuPool pool;

        private Dictionary<string, UIMenu> menus = new Dictionary<string, UIMenu>();
        private UIMenu current = null;

        public Menu()
        {
            pool = new MenuPool();

            menus.Add("main", new MainMenu(this));
            menus.Add("customize", new CustomizeMenu(this));
            menus.Add("freemode_heritage", new Freemode.HeritageMenu(this));
            menus.Add("freemode_clothing", new Freemode.ClothingMenu(this));

            foreach (var menu in menus.Values)
            {
                menu.AddInstructionalButton(new InstructionalButton(CitizenFX.Core.Control.CursorScrollUp, "Zoom"));
                menu.OnMenuBack += Menu_OnMenuBack;
                pool.Add(menu);
            }
        }

        private void Menu_OnMenuBack(UIMenu sender)
        {
            if(sender.ParentMenu != null)
                current = sender.ParentMenu;
        }

        public void Open(string menu = null)
        {
            if (menu == null)
            {
                Close();
                current = menus["main"];
                current.OnActivated();
                current.Visible = true;
                return;
            }

            Close();
            current.Visible = false;
            menus[menu].ParentMenu = current;
            current = menus[menu];
            current.OnActivated();
            current.Visible = true;
        }

        public void Close()
        {
            pool.CloseAllMenus();
        }

        public bool IsOpen()
        {
            return pool.IsAnyMenuOpen();
        }

        public void Update()
        {
            pool.Draw();
            pool.ProcessControl();
        }

    }
}
