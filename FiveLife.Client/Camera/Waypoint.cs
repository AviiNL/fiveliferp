using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Camera
{
    public struct Waypoint
    {
        public Vector3 Position { get; set; }
        public Vector3 LookAt { get; set; }
        public float FieldOfView { get; set; }
        public float Duration { get; set; }

        public Waypoint(Vector3 Position, Vector3 LookAt, float FieldOfView, float Duration)
        {
            this.Position = Position;
            this.LookAt = LookAt;
            this.FieldOfView = FieldOfView;
            this.Duration = Duration;
        }

        public static Waypoint Lerp(Waypoint a, Waypoint b, float t)
        {
            return new Waypoint
            {
                LookAt = Vector3.Lerp(a.LookAt, b.LookAt, t),
                Position = Vector3.Lerp(a.Position, b.Position, t),
                Duration = a.Duration,
                FieldOfView = a.FieldOfView + (b.FieldOfView - a.FieldOfView) * t
            };
        }
    }
}
