using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class GameElement
    {
        public Vector2 position = new Vector2();
        public float radius;
        public int CurrentZone;
        public float currLife = 1000;
        public virtual void drawSelf(Texture2D cellTexture, SpriteBatch _spriteBatch)
        {
            
        }

        public virtual void Update(List<GameElement> cells, List<GameElement> plants, List<GameElement> objects, SpriteBatch _spriteBatch)
        {
            
        }

    }


}
