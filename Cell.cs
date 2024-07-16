using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Vector2 = System.Numerics.Vector2;

namespace C_EcoSimApp
{   
    internal class Cell : GameElement
    {
        public List<Component> ActiveComponents = new List<Component>();
        public List<Connection> Connections = new List<Connection>();
        
        public List<Component> InputComponents = new List<Component>();
        public List<Component> HiddenComponents = new List<Component>();
        public List<Component> OutputComponents = new List<Component>();

        public List<Connection> ToHidddenConnections = new List<Connection>();
        public List<Connection> ToOutputConnections = new List<Connection>();
        public int zonesPerRow = GameInst.worldSizeX / GameInst.ZoneSize - 1;

        //position information
        public Vector2 oldPosition = new Vector2();
        public float rotation;
        public HashSet<int> NearZones = new HashSet<int>();
        public int previousZone;

        
        //physical characteristics
        public int diameter = 32;
        public int sa=0;

        //velocity information
        public float angularVelocity = 0;
        public float velocity = 5f;

        //ComponentInformation
        public List<SRay> rays = new List<SRay>();
        public string QuickComponentLookup = "00000000000000000";

        public Cell()
        {
            radius = 16;
        }

        public void InitializeComponents(bool Custom = false)
        {
            if (Custom == false)
            {
                currLife = GameInst.Mrand.Next(500,1500);
                foreach (Component component in ActiveComponents)
                {
                    component.cellParent = this;
                    if (component.componentType == "INPUT") InputComponents.Add(component);
                    else if (component.componentType == "HIDDEN") HiddenComponents.Add(component);
                    else OutputComponents.Add(component);
                    int pos = component.component_ID;
                    QuickComponentLookup = QuickComponentLookup.ReplaceAt(pos, 1, "1");
                }
                foreach (Connection connection in Connections)
                {
                    if (connection.HiddenLayerOrOutput == "HiddenLayer") ToHidddenConnections.Add(connection);
                    else ToOutputConnections.Add(connection);
                }
            }
            else
            {
                //SIGHT
                Sight s = new Sight(nameof(GameElement));
                s.cellParent = this;
                s.fov = (float)0.017 * (30 + 4 * 15); ;
                s.lenght = 100;
                s.raysN = 10;
                s.component_ID = 0;
                string searchNValue0 = Convert.ToString(s.component_ID, 2).PadLeft(5, '0');
                s.name = Component.componentsName[searchNValue0];
                s.initializeRays(true);
                ActiveComponents.Clear();
                ActiveComponents.Add(s);
                InputComponents.Add(s);
                int pos = s.component_ID;
                QuickComponentLookup = QuickComponentLookup.ReplaceAt(pos, 1, "1");

                //CILIA
                Cilia c = new Cilia();
                c.activated = true;
                c.cellParent = this;
                c.ActivationValue = 4;
                c.costPT = 0;
                c.speed = 0.05f;
                c.rotationSpeed = 0.0007f;
                c.component_ID = 14;
                c.DebugName = "ccacc";
                string searchNValue = Convert.ToString(c.component_ID, 2).PadLeft(5, '0');
                c.name = Component.componentsName[searchNValue];
                ActiveComponents.Add(c);
                OutputComponents.Add(c);
                int pos2 = c.component_ID;
                QuickComponentLookup = QuickComponentLookup.ReplaceAt(pos2, 1, "1");


                Connections.Clear();
                for (int i = 0; i < s.raysN / 2; i++)
                {
                    Connection connection = new Connection("", "", "", "", "", "");
                    connection.optionStart = i;
                    connection.optionEnd = 0;
                    connection.connectionStart = s.component_ID;
                    connection.connectionEnd = c.component_ID;
                    connection.weight = -1f;
                    connection.bias = 0;
                    connection.HiddenLayerOrOutput = "Output";
                    connection.InputOrHiddenLayer = "Input";
                    Connections.Add(connection);
                    if (connection.HiddenLayerOrOutput == "HiddenLayer") ToHidddenConnections.Add(connection);
                    else ToOutputConnections.Add(connection);
                }

                for (int i = (int)s.raysN / 2; i < s.raysN; i++)
                {
                    Connection connection = new Connection("", "", "", "", "", "");
                    connection.optionStart = i;
                    connection.optionEnd = 0;
                    connection.connectionStart = s.component_ID;
                    connection.connectionEnd = c.component_ID;
                    connection.weight = 1f;
                    connection.bias = 0;
                    connection.HiddenLayerOrOutput = "Output";
                    connection.InputOrHiddenLayer = "Input";
                    Connections.Add(connection);
                    if (connection.HiddenLayerOrOutput == "HiddenLayer") ToHidddenConnections.Add(connection);
                    else ToOutputConnections.Add(connection);
                }

                
            }
            /*
            Console.WriteLine(ActiveComponents.Count);
            string ALL = "";
            string CONALL = "";
            ActiveComponents.ForEach(x => ALL+= x.REPR());
            Connections.ForEach(x => CONALL += x.REPR());
            Console.WriteLine($"Components: {ALL} \n Connections: {CONALL} \n ____END____");*/
        }

        public Vector2 randomPos()
        {
            Random random = new Random();
            Vector2 returnVal = new Vector2();
            returnVal.X = random.Next(0, GameInst.worldSizeX);
            returnVal.Y = random.Next(0, GameInst.worldSizeY);
            return returnVal;
        }

        public float randomRot()
        {
            Random random = new Random();
            float X = (float)(random.NextDouble()*6.28);
            return X;
        }

        public override void drawSelf(Texture2D cellTexture, SpriteBatch _spriteBatch)
        {
            foreach (Component component in InputComponents)
            {
                if (component.componentFamilyName == "SIGHT")
                {
                    Sight s = (Sight)component;
                    s.VisualRepr(_spriteBatch);
                }
            }
            _spriteBatch.Draw(cellTexture, new Rectangle((int)position.X, (int)position.Y, diameter, diameter), null, Color.White, rotation, new Vector2(diameter, diameter), SpriteEffects.None, 0);

        }

        public float CalcDist(GameElement A, GameElement B)
        {
            float distx = Math.Abs(A.position.X- B.position.X);
            float disty = Math.Abs(A.position.Y- B.position.Y);
            float dist = (float)(0.5 * (distx + disty + Math.Max(distx, disty)));
            return dist;
        }

        public List<CollisionInfo> CheckCollision(List<GameElement> cells)
        {
            List<CollisionInfo> CinfoList = new List<CollisionInfo>();
            int collided = 0;

            foreach (GameElement cell in cells)
            {
                if (cell == this || NearZones.Contains(cell.CurrentZone)==false) continue;
                
                float dist = CalcDist(cell, this);
                if (dist < diameter )
                {
                    collided++;
                    CollisionInfo CI = new CollisionInfo();
                    CI.collides = true;
                    CI.collideswith = cell;
                    CI.distance = dist;
                    CinfoList.Add(CI);
                }
            }
            if (collided == 0)
            {
                return null;
            }
            else return CinfoList;
        }

        public void UpdateZones()
        {
            CurrentZone = (int)(Math.Floor(position.X / GameInst.ZoneSize)+ (zonesPerRow - 1)* Math.Floor(position.Y/GameInst.ZoneSize));
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

        public void WorldBorderCheck()
        {
            /*
            if (position.X < 0) position.X = 1;
            else if (position.X > GameInst.worldSizeX) position.X = GameInst.worldSizeX-1;

            if (position.Y < 0) position.Y = 1;
            else if (position.Y > GameInst.worldSizeY) position.Y = GameInst.worldSizeY - 1;
            */
            
             if (position.X < 0) position.X = GameInst.worldSizeX - 1;
            else if (position.X > GameInst.worldSizeX) position.X = 1;

            if (position.Y < 0) position.Y = GameInst.worldSizeY - 1;
            else if (position.Y > GameInst.worldSizeY) position.Y = 1;
            
        }

        public override void Update(List<GameElement> cells, List<GameElement> plants, List<GameElement> objects, SpriteBatch _spriteBatch)
        {
            //Position rotation handler
            oldPosition = position;
            rotation += angularVelocity * GameInst.deltatime;
            if (angularVelocity > 0.0002f) angularVelocity -= 0.0002f;
            else if (angularVelocity < -0.0002f) angularVelocity += 0.0002f;

            if (angularVelocity > 1) angularVelocity = 1;
            if (angularVelocity < -1) angularVelocity = -1;

            float x_ADD = -(float)Math.Cos(1.57+rotation)*velocity;
            float y_ADD = -(float)Math.Sin(1.57+rotation)*velocity;
            position.X += x_ADD * GameInst.deltatime;
            position.Y += y_ADD * GameInst.deltatime;

            
            /*if (velocity > 0.02f) velocity -= 0.02f;
            else if (velocity < -0.02f) velocity += 0.02f;
            */
            if (velocity > 5) velocity = 5;
            if (velocity < -5) velocity = -5;

            UpdateZones();

            //InterCells collision
            List<CollisionInfo> collisionInfo = CheckCollision(objects);
            if(collisionInfo!=null)
            {
                position = oldPosition;

                //Bounce Back
                foreach(CollisionInfo ci in collisionInfo)
                {
                    Vector2 dir = (this.position - ci.collideswith.position);
                    dir.X *= (float)0.1*GameInst.deltatime;
                    dir.Y *= (float)0.1*GameInst.deltatime;
                    ci.collideswith.position -= dir;
                    position += dir;
                }
            }
            WorldBorderCheck();

            float EnergyBalance = 0;
            //RUN input Components
            foreach (Component component in InputComponents)
            {
                List<float> result = component.RUN(objects);
                if(result!=null) component.results = result;
                else component.results = new List<float>();
                EnergyBalance -= component.CostXtime;
            }

            /*foreach (Connection connection in ToHidddenConnections)
            {
                
            }*/

            foreach(Connection connection in ToOutputConnections)
            {
                int startP = connection.connectionStart;
                int endP = connection.connectionEnd;

                //GET Values to input
                float result = 0;
                if (connection.InputOrHiddenLayer == "Input")
                {
                    if (QuickComponentLookup[startP].ToString() != "1") continue;
                    string searchNValue = Convert.ToString(startP, 2).PadLeft(5, '0');
                    string searchN = Component.componentsName[searchNValue];
                    Component foundComp = new Component();
                    foreach (Component component in InputComponents)
                    {
                        if (component.name == searchN) foundComp = component;
                    }
                    //CHECK
                    //if(foundComp.results.Count <= connection.optionStart) connection.optionStart = foundComp.results.Count - 1;
                    int l = foundComp.results.Count;
                    if (foundComp.results.Count == 0) result = 0;
                    else if(connection.optionStart == 0)
                    {
                        //Left Rays
                        for (int i = 0; i<(int)l/2; i++)
                        {
                            if (foundComp.results.Count>0)
                            {
                                result += foundComp.results[i];
                            }
                        }
                    }
                    else
                    {
                        for (int i = (int)l/2; i < l; i++)
                        {
                            result += foundComp.results[i];
                        }
                    }
                    // else result = foundComp.results[(int)connection.optionStart]; OLD METHOD, ONE RAY CHOOSEN
                }


                //Calculate input*weight*bias, select output component, pass value
                if (connection.HiddenLayerOrOutput == "Output")
                {
                    
                    //CHECK
                    //endP += 7;
                    if (QuickComponentLookup.Length <= endP) endP = QuickComponentLookup.Length - 1; 
                    if (QuickComponentLookup[endP].ToString() != "1") continue;
                    string searchNValue = Convert.ToString(endP, 2).PadLeft(5, '0');
                    string searchN = Component.componentsName[searchNValue];
                    Component foundComp = new Component();
                    foreach (Component component in OutputComponents)
                    {
                        if (component.name == searchN)
                        {
                            foundComp = component;
                            break;
                        }
                    }
                    float passedResult = result * connection.weight + connection.bias;
                    
                    foundComp.inputV += passedResult;
                    foundComp.inputN += 1;
                }
            }

            foreach (Component component in OutputComponents)
            {
                component.RUN(objects);
                EnergyBalance -= component.CostXtime;
            }

            currLife += EnergyBalance;
            //if(EnergyBalance<-0.5f) Console.WriteLine($"life: {currLife}, expenses: {EnergyBalance}");

        }

        public (bool result, List<Component> found) CheckComponentPresence(string ComponentFamilyName)
        {
            List<Component> Found = new List<Component> ();
            int ActiveSight = 0;
            foreach (Component c in ActiveComponents)
            {
                if (c.componentFamilyName == ComponentFamilyName)
                {
                    Found.Add (c);
                    ActiveSight++;
                }
            }
            if (ActiveSight != 0)
            {
                return (true, Found);
            }
            return (false, null);
        }

    }
}
