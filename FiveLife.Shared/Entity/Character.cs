using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Shared.Entity
{
    public class Character : IEntity
    {

        // Indentifier
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }

        // Name
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Date of Birth
        public DateTime DateOfBirth { get; set; }

        // Position
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Heading { get; set; }

        // Stats
        public int Health { get; set; } = 100;
        public int Armor { get; set; } = 0;
        public int Stamina { get; set; }
        public int Stealth { get; set; }
        public int LungCapacity { get; set; }
        public int Flying { get; set; }
        public int Shooting { get; set; }
        public int Strength { get; set; }
        public int Wheelie { get; set; }

        // Appearance
        public long ModelHash { get; set; }
        // - PedComponents
        public int Face { get; set; }
        public int FaceTexture { get; set; }
        public int Head { get; set; }
        public int HeadTexture { get; set; }
        public int Hair { get; set; }
        public int HairTexture { get; set; }
        public int HairColor { get; set; }
        public int HairHighlight { get; set; }
        public int Torso { get; set; }
        public int TorsoTexture { get; set; }
        public int Legs { get; set; }
        public int LegsTexture { get; set; }
        public int Hands { get; set; }
        public int HandsTexture { get; set; }
        public int Shoes { get; set; }
        public int ShoesTexture { get; set; }
        public int Special1 { get; set; }
        public int Special1Texture { get; set; }
        public int Special2 { get; set; }
        public int Special2Texture { get; set; }
        public int Special3 { get; set; }
        public int Special3Texture { get; set; }
        public int Textures { get; set; }
        public int TexturesTexture { get; set; }
        public int Torso2 { get; set; }
        public int Torso2Texture { get; set; }
        // - PedProps
        public int Hats { get; set; }
        public int HatsTexture { get; set; }
        public int Glasses { get; set; }
        public int GlassesTexture { get; set; }
        public int EarPieces { get; set; }
        public int EarPiecesTexture { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown3Texture { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown4Texture { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown5Texture { get; set; }
        public int Watches { get; set; }
        public int WatchesTexture { get; set; }
        public int Wristbands { get; set; }
        public int WristbandsTexture { get; set; }
        public int Unknown8 { get; set; }
        public int Unknown8Texture { get; set; }
        public int Unknown9 { get; set; }
        public int Unknown9Texture { get; set; }
        // - MP Face Features (float, -1.0 - 1.0)
        public float NoseWidth { get; set; }
        public float NosePeakHight { get; set; }
        public float NosePeakLenght { get; set; }
        public float NoseBoneHigh { get; set; }
        public float NosePeakLowering { get; set; }
        public float NoseBoneTwist { get; set; }
        public float EyeBrownHigh { get; set; }
        public float EyeBrownForward { get; set; }
        public float CheeksBoneHigh { get; set; }
        public float CheeksBoneWidth { get; set; }
        public float CheeksWidth { get; set; }
        public float EyesOpenning { get; set; }
        public float LipsThickness { get; set; }
        public float JawBoneWidth { get; set; }
        public float JawBoneBackLenght { get; set; }
        public float ChimpBoneLowering { get; set; }
        public float ChimpBoneLenght { get; set; }
        public float ChimpBoneWidth { get; set; }
        public float ChimpHole { get; set; }
        public float NeckThikness { get; set; }
        // - MP Face Blend
        public int EyeColor { get; set; }
        public int HeadShape1 { get; set; } = 21;
        public int HeadSkin1 { get; set; } = 21;
        public int HeadShape2 { get; set; } = 0;
        public int HeadSkin2 { get; set; } = 0;
        public int HeadShape3 { get; set; }
        public int HeadSkin3 { get; set; }
        public float HeadShapeMix { get; set; } = 0.5f;
        public float HeadSkinMix { get; set; } = 0.5f;
        public float ThirdMix { get; set; }
        // - MP Overlay
        public int Blemishes { get; set; }
        public float BlemishesOpacity { get; set; }
        public int FacialHair { get; set; }
        public float FacialHairOpacity { get; set; }
        public int Eyebrows { get; set; }
        public float EyebrowsOpacity { get; set; }
        public int Ageing { get; set; }
        public float AgeingOpacity { get; set; }
        public int Makeup { get; set; }
        public float MakeupOpacity { get; set; }
        public int Blush { get; set; }
        public float BlushOpacity { get; set; }
        public int Complexion { get; set; }
        public float ComplexionOpacity { get; set; }
        public int SunDamage { get; set; }
        public float SunDamageOpacity { get; set; }
        public int Lipstick { get; set; }
        public float LipstickOpacity { get; set; }
        public int Moles { get; set; }
        public float MolesOpacity { get; set; }
        public int ChestHair { get; set; }
        public float ChestHairOpacity { get; set; }
        public int BodyBlemishes { get; set; }
        public float BodyBlemishesOpacity { get; set; }
        public int AddBodyBlemishes { get; set; }
        public float AddBodyBlemishesOpacity { get; set; }

        // Currency
        public int Cash { get; set; }

        // Inventory


        // Allowed rooms to enter
        public virtual ICollection<Room> Rooms { get; set; }


        // Operators
        //public override int GetHashCode()
        //{
        //    return Id;
        //}

        //public override bool Equals(object obj)
        //{
        //    var character = obj as Character;
        //    return character != null &&
        //           Id == character.Id;
        //}

        //public static bool operator ==(Character a, Character b)
        //{
        //    return a.Id == b.Id;
        //}

        //public static bool operator !=(Character a, Character b)
        //{
        //    return a.Id != b.Id;
        //}
    }
}
