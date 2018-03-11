using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Camera
{
    public class Camera
    {

        public bool Enabled {
            set {
                World.RenderingCamera = value ? _camera : null;
            }
            get {
                return World.RenderingCamera == _camera;
            }
        }

        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public float FieldOfView { get; set; } = 50f;

        public CitizenFX.Core.Camera _camera;
        private Tween<Waypoint> tween;

        public Camera()
            : this(Vector3.Zero, Vector3.Zero, 50)
        {
        }

        public Camera(Vector3 Position, Vector3 LookAt)
            : this(Position, LookAt, 50)
        {
        }

        public Camera(Vector3 Position, Vector3 LookAt, float FieldOfView)
        {
            this.Position = Position;
            this.Rotation = LookAt;
            this.FieldOfView = FieldOfView;

            _camera = World.CreateCamera(Position, Vector3.Zero, FieldOfView);
            _camera.PointAt(LookAt);
            tween = new Tween<Waypoint>(new LerpFunc<Waypoint>(Waypoint.Lerp));
        }

        public void LookAt(Vector3 point)
        {
            Rotation = point;
        }

        public void MoveTo(Waypoint waypoint)
        {
            tween.Stop(StopBehavior.AsIs);
            var currentWaypoint = new Waypoint(Position, Rotation, FieldOfView, 0);

            tween.Start(currentWaypoint, waypoint, waypoint.Duration, ScaleFuncs.CubicEaseInOut);
        }

        public void Update()
        {
            if (_camera == null) return;

            if (tween.State == TweenState.Running)
            {
                tween.Update(CitizenFX.Core.Game.LastFrameTime);

                Position = tween.CurrentValue.Position;
                Rotation = tween.CurrentValue.LookAt;
                FieldOfView = tween.CurrentValue.FieldOfView;
            }

            _camera.Position = Position;
            _camera.PointAt(Rotation);
            _camera.FieldOfView = FieldOfView;
        }
    }
}
