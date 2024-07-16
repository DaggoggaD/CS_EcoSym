using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace C_EcoSimApp
{
    internal class Plant: GameElement
    {

        public HashSet<int> NearZones = new HashSet<int>();
        public int previousZone;
        public int zonesPerRow = GameInst.worldSizeX / GameInst.ZoneSize - 1;
        

        public Plant()
        {
            Random random = new Random();
            currLife = random.Next(10, 100);
            radius = 10 + currLife/10;
            position = randomPos();
            UpdateZones();
        }

        public override void drawSelf(Texture2D plantTexture, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(plantTexture, new Rectangle((int)position.X, (int)position.Y, (int)radius*2, (int)radius*2), null, Color.White, 0, new Microsoft.Xna.Framework.Vector2(radius*2, radius*2), SpriteEffects.None, 0);

        }

        public System.Numerics.Vector2 randomPos()
        {
            Random random = new Random();
            System.Numerics.Vector2 returnVal = new System.Numerics.Vector2();
            returnVal.X = random.Next(0, GameInst.worldSizeX);
            returnVal.Y = random.Next(0, GameInst.worldSizeY);
            return returnVal;
        }

        public void UpdateZones()
        {
            CurrentZone = (int)(Math.Floor(position.X / GameInst.ZoneSize) + (zonesPerRow - 1) * Math.Floor(position.Y / GameInst.ZoneSize));
            if (CurrentZone != previousZone)
            {
                previousZone = CurrentZone;
                NearZones = new HashSet<int>
                {
                    CurrentZone,
                    CurrentZone + 1,
                    CurrentZone - 1,
                    CurrentZone - zonesPerRow,
                    CurrentZone - zonesPerRow - 1,
                    CurrentZone - zonesPerRow + 1,
                    CurrentZone + zonesPerRow,
                    CurrentZone + zonesPerRow - 1,
                    CurrentZone + zonesPerRow + 1
                };
            }
        }

    }
}
