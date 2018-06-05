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

            foreach (var menu in menus.Values)
            {
                menu.AddInstructionalButton(new InstructionalButton(CitizenFX.Core.Control.CursorScrollUp, "Zoom"));
                pool.Add(menu);
            }
        }

        public void Open(string menu = null)
        {
            if(menu == null)
            {
                Close();
                current = menus["main"];
                current.Visible = true;
                return;
            }

            Close();
            current.Visible = false;
            menus[menu].ParentMenu = current;
            current = menus[menu];
            current.Visible = true;
        }

        public void Back()
        {
            var temp = current; // this logic should be in the menu itself, at least partly
            current.Visible = false;
            if (current.ParentMenu != null)
            {
                current = current.ParentMenu;
                temp.ParentMenu = null;
            }
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
