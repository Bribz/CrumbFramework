using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrumbShared
{
    public class CharacterData
    {
        public string Name { get; set; }
        public ulong CharacterID { get; set; }
        public Vector3f position;
        public float Rotation;
    }
}
