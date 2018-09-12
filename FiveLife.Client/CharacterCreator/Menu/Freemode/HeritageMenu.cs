using CitizenFX.Core;
using FiveLife.Client.CharacterCreator.Elements;
using FiveLife.NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.CharacterCreator.Menu.Freemode
{
    public class HeritageMenu : UIMenu
    {
        Menu menu;

        UIMenuListItem fatherShape;
        UIMenuListItem motherShape;
        UIMenuListItem mixShape;

        private List<UIMenuListItemItem> maleList = new List<UIMenuListItemItem>()
            {
                new UIMenuListItemItem(0, "Dayton"),
                new UIMenuListItemItem(1, "Ernest"),
                new UIMenuListItemItem(2, "Sam"),
                new UIMenuListItemItem(3, "Roderick"),
                new UIMenuListItemItem(4, "Ryland"),
                new UIMenuListItemItem(5, "Cruz"),
                new UIMenuListItemItem(6, "Myles"),
                new UIMenuListItemItem(7, "Wade"),
                new UIMenuListItemItem(8, "Francis"),
                new UIMenuListItemItem(9, "Samson"),
                new UIMenuListItemItem(10, "Marques"),
                new UIMenuListItemItem(11, "Kyson"),
                new UIMenuListItemItem(12, "August"),
                new UIMenuListItemItem(13, "Cedric"),
                new UIMenuListItemItem(14, "Jayvon"),
                new UIMenuListItemItem(15, "Bennett"),
                new UIMenuListItemItem(16, "Uriah"),
                new UIMenuListItemItem(17, "Eden"),
                new UIMenuListItemItem(18, "Jabari"),
                new UIMenuListItemItem(19, "Mario"),
                new UIMenuListItemItem(20, "Jeramiah"),
                new UIMenuListItemItem(42, "Alexis"),
                new UIMenuListItemItem(43, "Vaughn"),
                new UIMenuListItemItem(44, "Johan")
            };
        private List<UIMenuListItemItem> femaleList = new List<UIMenuListItemItem>() {
                new UIMenuListItemItem(21, "Paola"),
                new UIMenuListItemItem(22, "Karen"),
                new UIMenuListItemItem(23, "Aliza"),
                new UIMenuListItemItem(24, "Aurora"),
                new UIMenuListItemItem(25, "Desirae"),
                new UIMenuListItemItem(26, "Nathaly"),
                new UIMenuListItemItem(27, "Lena"),
                new UIMenuListItemItem(28, "Aryana"),
                new UIMenuListItemItem(29, "Luciana"),
                new UIMenuListItemItem(30, "Kendal"),
                new UIMenuListItemItem(31, "Rubi"),
                new UIMenuListItemItem(32, "Brenna"),
                new UIMenuListItemItem(33, "Piper"),
                new UIMenuListItemItem(34, "Christina"),
                new UIMenuListItemItem(35, "Adelyn"),
                new UIMenuListItemItem(36, "Bailee"),
                new UIMenuListItemItem(37, "Ayla"),
                new UIMenuListItemItem(38, "Kenya"),
                new UIMenuListItemItem(39, "Janet"),
                new UIMenuListItemItem(40, "Jaylin"),
                new UIMenuListItemItem(41, "Krystal"),
                new UIMenuListItemItem(45, "Jacqueline")
            };
        private List<UIMenuListItemItem> mixRange = new List<UIMenuListItemItem>();

        public HeritageMenu(Menu menu)
            : base("", "Heritage")
        {
            this.menu = menu;
        }

        public override void OnActivated()
        {
            MenuItems.Clear();

            for (float value = 0f; value <= 1.0f; value += 0.01f)
            {
                var a = (float)Math.Round(value, 2);
                mixRange.Add(new UIMenuListItemItem(a, a.ToString()));
            }

            motherShape = new UIMenuListItem("Mother Shape", femaleList);
            fatherShape = new UIMenuListItem("Father Shape", maleList);
            mixShape = new UIMenuListItem("Mix Shape", mixRange);

            motherShape.OnListChanged += MotherShape_OnListChanged;
            fatherShape.OnListChanged += FatherShape_OnListChanged;
            mixShape.OnListChanged += MixValue_OnListChanged;

            AddItem(motherShape);
            AddItem(fatherShape);
            AddItem(mixShape);

            motherShape.Index = femaleList.FindIndex(x => (int)x.Value == Game.Data.Character.HeadShape1);
            fatherShape.Index = maleList.FindIndex(x => (int)x.Value == Game.Data.Character.HeadShape2);
            mixShape.Index = mixRange.FindIndex(x => (float)x.Value == Game.Data.Character.HeadShapeMix);
        }

        private async void MixValue_OnListChanged(UIMenuListItem sender, UIMenuListItemItem item)
        {
            Game.Data.Character.HeadShapeMix = (float)item.Value;
            await CitizenFX.Core.Game.Player.Character.Apply(Game.Data.Character);
        }

        private async void FatherShape_OnListChanged(UIMenuListItem sender, UIMenuListItemItem item)
        {
            Game.Data.Character.HeadShape2 = (int)item.Value;
            await CitizenFX.Core.Game.Player.Character.Apply(Game.Data.Character);
        }

        private async void MotherShape_OnListChanged(UIMenuListItem sender, UIMenuListItemItem item)
        {
            Game.Data.Character.HeadShape1 = (int)item.Value;
            await CitizenFX.Core.Game.Player.Character.Apply(Game.Data.Character);
        }
    }
}