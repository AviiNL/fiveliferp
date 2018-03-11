using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizenFX.Core
{
    public static class PedExtension
    {

        public static async Task Apply(this Ped self, FiveLife.Shared.Entity.Character obj)
        {
            #region Model
            if ((PedHash)self.Model.NativeValue != (PedHash)obj.ModelHash)
            {
                var model = new Model((PedHash)obj.ModelHash);
                model.Request();
                while (!model.IsLoaded) await BaseScript.Delay(0);
                await Game.Player.ChangeModel(model);
            }
            #endregion

            #region PedComponents
            if (API.IsPedComponentVariationValid(self.Handle,(int)PedComponents.Hair, obj.Hair, obj.HairTexture))
                self.Style[PedComponents.Hair].SetVariation(obj.Hair, obj.HairTexture);

            if(API.IsPedHairColorValid(obj.HairColor) && API.IsPedHairColorValid(obj.HairHighlight))
                Function.Call(Hash._SET_PED_HAIR_COLOR, self, obj.HairColor, obj.HairHighlight);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Face, obj.Face, obj.FaceTexture))
                self.Style[PedComponents.Face].SetVariation(obj.Face, obj.FaceTexture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Head, obj.Head, obj.HeadTexture))
                self.Style[PedComponents.Head].SetVariation(obj.Head, obj.HeadTexture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Torso, obj.Torso, obj.TorsoTexture))
                self.Style[PedComponents.Torso].SetVariation(obj.Torso, obj.TorsoTexture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Legs, obj.Legs, obj.LegsTexture))
                self.Style[PedComponents.Legs].SetVariation(obj.Legs, obj.LegsTexture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Hands, obj.Hands, obj.HandsTexture))
                self.Style[PedComponents.Hands].SetVariation(obj.Hands, obj.HandsTexture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Shoes, obj.Shoes, obj.ShoesTexture))
                self.Style[PedComponents.Shoes].SetVariation(obj.Shoes, obj.ShoesTexture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Special1, obj.Special1, obj.Special1Texture))
                self.Style[PedComponents.Special1].SetVariation(obj.Special1, obj.Special1Texture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Special2, obj.Special2, obj.Special2Texture))
                self.Style[PedComponents.Special2].SetVariation(obj.Special2, obj.Special2Texture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Special3, obj.Special3, obj.Special3Texture))
                self.Style[PedComponents.Special3].SetVariation(obj.Special3, obj.Special3Texture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Textures, obj.Textures, obj.TexturesTexture))
                self.Style[PedComponents.Textures].SetVariation(obj.Textures, obj.TexturesTexture);

            if (API.IsPedComponentVariationValid(self.Handle, (int)PedComponents.Torso2, obj.Torso2, obj.Torso2Texture))
                self.Style[PedComponents.Torso2].SetVariation(obj.Torso2, obj.Torso2Texture);
            #endregion

            #region PedProps
            if (API.IsPedPropValid(self.Handle, (int)PedProps.Hats, obj.Hats, obj.HatsTexture))
                self.Style[PedProps.Hats].SetVariation(obj.Hats, obj.HatsTexture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.Glasses, obj.Glasses, obj.GlassesTexture))
                self.Style[PedProps.Glasses].SetVariation(obj.Glasses, obj.GlassesTexture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.EarPieces, obj.EarPieces, obj.EarPiecesTexture))
                self.Style[PedProps.EarPieces].SetVariation(obj.EarPieces, obj.EarPiecesTexture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.Unknown3, obj.Unknown3, obj.Unknown3Texture))
                self.Style[PedProps.Unknown3].SetVariation(obj.Unknown3, obj.Unknown3Texture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.Unknown4, obj.Unknown4, obj.Unknown4Texture))
                self.Style[PedProps.Unknown4].SetVariation(obj.Unknown4, obj.Unknown4Texture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.Unknown5, obj.Unknown5, obj.Unknown5Texture))
                self.Style[PedProps.Unknown5].SetVariation(obj.Unknown5, obj.Unknown5Texture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.Watches, obj.Watches, obj.WatchesTexture))
                self.Style[PedProps.Watches].SetVariation(obj.Watches, obj.WatchesTexture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.Wristbands, obj.Wristbands, obj.WristbandsTexture))
                self.Style[PedProps.Wristbands].SetVariation(obj.Wristbands, obj.WristbandsTexture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.Unknown8, obj.Unknown8, obj.Unknown8Texture))
                self.Style[PedProps.Unknown8].SetVariation(obj.Unknown8, obj.Unknown8Texture);

            if (API.IsPedPropValid(self.Handle, (int)PedProps.Unknown9, obj.Unknown9, obj.Unknown9Texture))
                self.Style[PedProps.Unknown9].SetVariation(obj.Unknown9, obj.Unknown9Texture);
            #endregion

            #region FaceFeatures
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 0, obj.NoseWidth);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 1, obj.NosePeakHight);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 2, obj.NosePeakLenght);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 3, obj.NoseBoneHigh);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 4, obj.NosePeakLowering);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 5, obj.NoseBoneTwist);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 6, obj.EyeBrownHigh);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 7, obj.EyeBrownForward);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 8, obj.CheeksBoneHigh);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 9, obj.CheeksBoneWidth);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 10, obj.CheeksWidth);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 11, obj.EyesOpenning);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 12, obj.LipsThickness);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 13, obj.JawBoneWidth);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 14, obj.JawBoneBackLenght);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 15, obj.ChimpBoneLowering);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 16, obj.ChimpBoneLenght);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 17, obj.ChimpBoneWidth);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 18, obj.ChimpHole);
            Function.Call(Hash._SET_PED_FACE_FEATURE, self, 19, obj.NeckThikness);
            #endregion

            #region Face
            Function.Call(Hash.SET_PED_HEAD_BLEND_DATA, self,
                obj.HeadShape1,
                obj.HeadShape2,
                obj.HeadShape3,
                obj.HeadSkin1,
                obj.HeadSkin2,
                obj.HeadSkin3,
                obj.HeadShapeMix,
                obj.HeadSkinMix,
                obj.ThirdMix,
                false
            );

            Function.Call(Hash._SET_PED_EYE_COLOR, self, obj.EyeColor);
            #endregion

            #region Health and Armor
            self.MaxHealth = 100;
            self.Health = obj.Health;
            self.Armor = obj.Armor;
            #endregion

            #region Weapons
            self.DropsWeaponsOnDeath = false;
            self.Weapons.RemoveAll();

            // @todo[inventory]: Add weapons owned by player
            #endregion
            
        }
    }
}
