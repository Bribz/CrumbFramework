using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrumbShared.Objects
{
    public class Entity
    {
        public string Name;

        public Vector3f Position;
        public Vector3f Velocity { get; protected set; }

        public float Rotation;
        public float MoveSpeed = 3f;

        public int MapID;

        public Entity()
        {
            Name = "";
            Velocity = Vector3f.Zero;
            Position = Vector3f.Zero;
            Rotation = 0f;
        }

        public void set_velocity(Vector3f input)
        {
            Velocity = input;
        }

        public virtual void tick(float delta)
        {
            Position += Velocity * MoveSpeed * delta;
        }
    }
}
