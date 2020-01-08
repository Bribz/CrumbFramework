using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrumbShared.Objects
{
    public class PlayerAvatar : Entity
    {
        public CharacterData Data;
        private Vector3f Input;

        public PlayerAvatar() : base()
        {
            Data = new CharacterData();
            Input = Vector3f.Zero;
        }

        public PlayerAvatar(CharacterData data) : base()
        {
            Data = data;
            Input = Vector3f.Zero;
        }

        public override void tick(float delta)
        {
            Velocity = Input;
            base.tick(delta);
        }

        public virtual void update_input(InputKeys keys)
        {
            Input = Vector3f.Zero;
            if((keys & InputKeys.Up) != 0)
            {
                Input.Z += 1f;
            }
            if ((keys & InputKeys.Down) != 0)
            {
                Input.Z -= 1f;
            }
            if ((keys & InputKeys.Left) != 0)
            {
                Input.X -= 1f;
            }
            if ((keys & InputKeys.Right) != 0)
            {
                Input.X += 1f;
            }
            Input.Normalize();
        }
    }
}
