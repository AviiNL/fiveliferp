using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.UI
{
    public enum ProximityRange
    {
        Whispering = 1,
        Speaking = 5,
        Shouting = 12,
    }

    public class Voip : FiveLifeScript
    {

        private ProximityRange range = ProximityRange.Speaking;

        private Container rect = new Container(new PointF(5, 692.5f), new SizeF(0, 20), Color.Empty);

        private Text rangeText = new Text("Range: ", new PointF(10, 1), 0.3f, Color.Empty);

        private Color colorTextSpeaking;
        private Color colorTextIdle;

        private Color colorRectSpeaking;
        private Color colorRectIdle;


        public override void Initialize()
        {
            colorTextIdle = Color.FromArgb(25, 225, 245, 254);
            colorRectIdle = Color.FromArgb(25, 0, 0, 0);

            colorTextSpeaking = Color.FromArgb(180, 3, 169, 244);
            colorRectSpeaking = Color.FromArgb(180, 0, 0, 0);

            rangeText.Outline = true;
            rangeText.Shadow = true;

            rect.Items.Add(rangeText);
        }

        public override async Task Loop()
        {
            if (API.IsPauseMenuActive()) return;

            if (CitizenFX.Core.Game.IsControlJustPressed(0, CitizenFX.Core.Control.HUDSpecial))
            {
                switch (range)
                {
                    case ProximityRange.Shouting:
                        SetProximity(ProximityRange.Speaking);
                        break;
                    case ProximityRange.Whispering:
                        SetProximity(ProximityRange.Shouting);
                        break;
                    case ProximityRange.Speaking:
                        SetProximity(ProximityRange.Whispering);
                        break;
                }
            }

            Function.Call(Hash.NETWORK_SET_TALKER_PROXIMITY, (float)range);
            Function.Call(Hash.NETWORK_CLEAR_VOICE_CHANNEL);
            Function.Call(Hash.NETWORK_SET_VOICE_CHANNEL, 0); // Based off of current room
            Function.Call(Hash.NETWORK_SET_VOICE_ACTIVE, true);

            UpdateColor();
            UpdatePosition();

            rangeText.Caption = range.ToString();
            rect.Size = new SizeF(rangeText.Width + 20, 20);

            rect.ScaledDraw();
        }


        private void UpdatePosition()
        {
            //if (Game.Player.Character.CurrentVehicle == null)
            //{
            //    rect.Position = new PointF(5, 692.5f);
            //}
            //else
            {
                rect.Position = new PointF(5 + 198 + 5, 692.5f);
            }
        }

        private void UpdateColor()
        {
            if (Function.Call<bool>(Hash.NETWORK_IS_PLAYER_TALKING, CitizenFX.Core.Game.Player))
            {
                rect.Color = colorRectSpeaking;
                rangeText.Color = colorTextSpeaking;
            }
            else
            {
                rect.Color = colorRectIdle;
                rangeText.Color = colorTextIdle;
            }
        }

        public void SetProximity(ProximityRange range)
        {
            this.range = range;
        }
    }
}
