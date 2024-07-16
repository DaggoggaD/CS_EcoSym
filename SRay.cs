using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class SRay
    {
        public Vector2 start;
        public Vector2 end;
        public Vector2 dir;
        public float lenght;
        public Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(0, 255, 0, 0.1f);
        public List<CollisionInfo> colls = new List<CollisionInfo>();
        
        public void CalcDir()
        {
            dir = end-start;
        }

        public void CalcEnd()
        {
            end.X = start.X + dir.X;
            end.Y = start.Y + dir.Y;
        }

    }
}