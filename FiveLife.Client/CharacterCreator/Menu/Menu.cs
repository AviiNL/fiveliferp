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

        private MainMenu mainMenu = new MainMenu();

        public Menu()
        {
            pool = new MenuPool();

            pool.Add(mainMenu);

        }

        public void Open()
        {
            Close();
            mainMenu.Visible = true;
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
