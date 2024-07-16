using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class CollisionInfo
    {
        public bool collides;
        public GameElement collideswith;
        public float distance;
        public Vector2 point;
    }
}
