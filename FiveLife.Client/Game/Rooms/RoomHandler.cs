using CitizenFX.Core;
using CitizenFX.Core.Native;
using FiveLife.Shared.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.Rooms
{
    public class RoomHandler : FiveLifeScript
    {
        bool busy = false;
        public static int RoomId = 0;

        Dictionary<int, bool> visible = new Dictionary<int, bool>();

        public override async void Initialize()
        {
            foreach (var room in Data.Rooms)
            {
                if (!room.IsPrivate)
                {
                    var blip = World.CreateBlip(new Vector3(room.OutsideX, room.OutsideY, room.OutsideZ));
                    blip.Sprite = (BlipSprite)room.BlipSprite;
                    blip.Color = (BlipColor)room.BlipColor;
                    blip.IsShortRange = true;
                    blip.Name = room.Name;
                }
            }

            UpdateVisibility();

            RegisterEvent<int>("fivelife.room.left", OnRoomLeft);
            RegisterEvent<int, Room>("fivelife.room.joined", OnRoomJoined);

            RegisterEvent<bool, Room>("fivelife.room.enter", OnRoomEnter);
            RegisterEvent<Room>("fivelife.room.exit", OnRoomExit);

            #region DebugBlips
            //Debug.WriteLine("Adding Blips?");
            //var x = -2000;
            //var y = 4000;
            //foreach(var sprite in Enum.GetValues(typeof(BlipSprite)))
            //{
            //    var blip = World.CreateBlip(new Vector3((x+=300), (y), 0));
            //    blip.Name = "Blip: " + ((int)sprite).ToString();
            //    blip.Sprite = (BlipSprite)sprite;

            //    if((x / 100) % 25 == 0)
            //    {
            //        y -= 300;
            //        x = -2000;
            //    }

            //}
            #endregion

        }

        private async void UpdateVisibility()
        {
            RoomId = 0;
            Data.Rooms = await Database.Repository<Room>.GetAll();
            foreach (var room in Data.Rooms)
            {
                foreach (var inside in room.CurrentlyInside)
                {
                    // hide everyone inside any room
                    var p = new PlayerList()[inside];
                    if (p == null) continue;
                    visible[p.Character.Handle] = false;
                    //API.SetEntityLocallyInvisible(p.Character.Handle);

                    // find the room we're currently in
                    if (inside == CitizenFX.Core.Game.Player.ServerId)
                    {
                        RoomId = room.Id;
                        break;
                    }
                }
            }
            Debug.WriteLine(RoomId.ToString());
            // If we are in a room, find out who's there too
            if (RoomId != 0)
            {
                Debug.WriteLine(String.Join(",", Data.Rooms.FirstOrDefault(e => e.Id == RoomId).CurrentlyInside));

                foreach (var obj in Data.Rooms.FirstOrDefault(e => e.Id == RoomId).CurrentlyInside)
                {
                    var p = new PlayerList()[obj];
                    visible[p.Character.Handle] = true;

                }
            }
            else // we're outside, show everyone
            {
                foreach (var p in new PlayerList())
                {
                    visible[p.Character.Handle] = true;
                }
            }
        }

        private void OnRoomJoined(int obj, Room room)
        {

            if(RoomId != 0)
                UpdateVisibility();
        }

        private void OnRoomLeft(int obj)
        {
            if (RoomId != 0)
                UpdateVisibility();
        }

        private async void OnRoomExit(Room room)
        {
            RoomId = 0;

            Function.Call(Hash.NETWORK_SET_VOICE_CHANNEL, 0);

            var Outside = new Vector3(room.OutsideSpawnX, room.OutsideSpawnY, room.OutsideSpawnZ);

            await CitizenFX.Core.Game.Player.Teleport(Outside, room.OutsideSpawnHeading);

            Function.Call(Hash.REMOVE_IPL, room.IPL);

            busy = false;
        }

        private async void OnRoomEnter(bool allowed, Room room)
        {
            if (!allowed)
            {
                busy = false;
                return;
            }
            RoomId = room.Id;

            Function.Call(Hash.REQUEST_IPL, room.IPL);
            Function.Call(Hash.NETWORK_SET_VOICE_CHANNEL, room.Id);

            var Inside = new Vector3(room.InsideSpawnX, room.InsideSpawnY, room.InsideSpawnZ);

            await CitizenFX.Core.Game.Player.Teleport(Inside, room.InsideSpawnHeading);


            busy = false;
        }

        public override async Task Loop()
        {
            if (Data.Rooms == null) return;

            foreach (var vis in visible)
            {
                if (!vis.Value)
                {
                    API.SetEntityLocallyInvisible(vis.Key);
                    API.SetEntityCollision(vis.Key, false, true);
                }
                else
                {
                    API.SetEntityLocallyVisible(vis.Key);
                    API.SetEntityCollision(vis.Key, true, true);
                }
            }

            foreach (var room in Data.Rooms)
            {
                var Outside = new Vector3(room.OutsideX, room.OutsideY, room.OutsideZ - 0.95f);
                var Inside = new Vector3(room.InsideX, room.InsideY, room.InsideZ - 0.95f);

                World.DrawMarker(MarkerType.HorizontalCircleSkinny, Outside, Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 0), System.Drawing.Color.FromArgb(70, 50, 50, 50));

                if (CitizenFX.Core.Game.Player.Character.Position.DistanceToSquared(Outside) < 1.35f && !busy)
                {
                    busy = true;
                    FireServerEvent("fivelife.room.enter", CitizenFX.Core.Game.Player.ServerId, room, Data.Character);
                }

                if (room.Id == RoomId)
                {
                    World.DrawMarker(MarkerType.HorizontalCircleSkinny, Inside, Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 0), System.Drawing.Color.FromArgb(70, 50, 50, 50));

                    if (CitizenFX.Core.Game.Player.Character.Position.DistanceToSquared(Inside) < 1.35f && !busy)
                    {
                        busy = true;
                        FireServerEvent("fivelife.room.leave", CitizenFX.Core.Game.Player.ServerId, room, Data.Character);
                    }
                }
            }
        }

    }
}
