using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class PheromoneZone
    {
        public List<float> Pheromones = new List<float> { 0f, 0f, 0f };
        public int WorldZone;
        public Vector2 pos;
        public Vector2 CenterPoint;
        public int size;

        public void Adjust()
        {
            CenterPoint.X = pos.X + size/2;
            CenterPoint.Y = pos.Y + size/2;
        }

        public void visualRepr(SpriteBatch _spriteBatch, Texture2D square)
        {
            _spriteBatch.Draw(square, new Rectangle((int)pos.X, (int)pos.Y, size, size), null, new Color(Pheromones[0]/10, Pheromones[1]/10, 0.5f,.6f), 0, new Vector2(0, 0), SpriteEffects.None, 0);
            /*if (Pheromones[0]>=9.5) _spriteBatch.Draw(square, new Rectangle((int)pos.X, (int)pos.Y, size, size), null, new Color(1, Pheromones[1]/10, 0.5f, 1f), 0, new Vector2(0, 0), SpriteEffects.None, 0);
            if (Pheromones[1]>=9.5) _spriteBatch.Draw(square, new Rectangle((int)pos.X, (int)pos.Y, size, size), null, new Color(Pheromones[0]/10, 1, 0.5f, 1f), 0, new Vector2(0, 0), SpriteEffects.None, 0);
            else _spriteBatch.Draw(square, new Rectangle((int)pos.X, (int)pos.Y, size, size), null, new Color(Pheromones[0] / 10, Pheromones[1] / 10, 0.5f, .6f), 0, new Vector2(0, 0), SpriteEffects.None, 0);
            */
        }

        public string REPR()
        {
            string ret = $"World Zone: {WorldZone}; PosX: {pos.X}; PosY: {pos.Y}; Size: {size}; Pheromones: [\n";
            Pheromones.ForEach(x => ret += x + "\n");
            ret+= "]";
            return ret;
        }
    }
}
