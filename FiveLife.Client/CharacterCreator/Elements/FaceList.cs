using CitizenFX.Core;
using FiveLife.NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.CharacterCreator.Elements
{
    public class FaceList : UIMenuListItem
    {
        public FaceList(UIMenu parent)
            :base("Face")
        {
            UpdateList();
            this.OnListChanged += FaceList_OnListChanged;
            this.OnListSelected += FaceList_OnListSelected;
        }

        private void FaceList_OnListSelected(UIMenuListItem sender, UIMenuListItemItem item)
        {
            var character = CitizenFX.Core.Game.Player.Character;

            if ((uint)character.Model.Hash == (uint)PedHash.FreemodeFemale01 || (uint)character.Model.Hash == (uint)PedHash.FreemodeMale01)
            {
                Debug.WriteLine("freemode, open submenu");
            }
        }

        private void UpdateList()
        {
            _items.Clear();
            Index = 0;

            var character = CitizenFX.Core.Game.Player.Character;

            if((uint)character.Model.Hash == (uint)PedHash.FreemodeFemale01 || (uint)character.Model.Hash == (uint)PedHash.FreemodeMale01)
            {
                // Debug.WriteLine("Freemode model selected");
                return;
            }

            Enabled = true;

            var face = character.Style[PedComponents.Face];

            for (var i = 0; i < face.Count; i++) {
                _items.Add(new UIMenuListItemItem(i));
            }
        }

        private void FaceList_OnListChanged(UIMenuListItem sender, UIMenuListItemItem item)
        {
            var character = CitizenFX.Core.Game.Player.Character;

            character.Style[PedComponents.Face].Index = (int)item.Value;
        }

    }
}
