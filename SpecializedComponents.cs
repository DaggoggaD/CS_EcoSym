using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class Sight : Component
    {
        /*
         * SIGHT COMPONENT STATS:
         * LENGHT 0-10
         * FOV    0-10
         * N-RAYS 0-10
         */

        public float lenght;
        public float fov;
        public float raysN;
        public float anglePerRay;
        int updated = 0;
        string SightType;

        public bool RaysInitialized = false;

        public List<SRay> rays = new List<SRay>();


        public Sight(string type) {
            componentFamilyName = "SIGHT";
            componentType = "INPUT";
            SightType = type;
        }

        public void initializeRays(bool Custom = false)
        {
            if (Custom == false)
            {
                lenght = (float)Math.Floor(components_stats[0] * 10);  // 0:0, 10:100
                fov = (float)0.017 * (30 + components_stats[1] * 15);   // 0:0.5rad(30°ca), 10:3.14rad(180°)
                raysN = (float)Math.Floor(components_stats[2]) + 3;   // 0:3, 10:13
            }
            CostXtime = components_stats[3]/100;
            anglePerRay = fov / raysN;
            for (int i = 0; i < raysN; i++)
            {
                SRay ray = new SRay();
                ray.start = cellParent.position;
                ray.lenght = lenght;
                float angle = anglePerRay / 2 - fov / 2 + anglePerRay * i - 1.57f + cellParent.rotation;
                ray.dir.X = lenght * (float)Math.Cos(angle);
                ray.dir.X = lenght * (float)Math.Sin(angle);
                ray.CalcEnd();
                rays.Add(ray);
            }
            RaysInitialized = true;
        }

        public void UpdateRays(bool? draw=false, SpriteBatch spriteBatch = null)
        {
            for (int i = 0; i < rays.Count; i++)
            {
                rays[i].start = cellParent.position;
                float angle = anglePerRay / 2 - fov / 2 + anglePerRay * i - 1.57f + cellParent.rotation;
                rays[i].dir.X = lenght * (float)Math.Cos(angle);
                rays[i].dir.Y = lenght * (float)Math.Sin(angle);
                rays[i].CalcEnd();
                if (draw == false || spriteBatch==null) return;
                CustomMonoGameUtil.DrawLine(spriteBatch, rays[i].start, rays[i].end, rays[i].color, 2);
            }
        }



        public void CheckCollisionSight(List<GameElement> elements)
        {
            HashSet<int> nearZones = cellParent.NearZones;
            foreach (GameElement element in elements)
            {
                if (element == null)
                {
                    elements.Remove(element);
                    continue;
                }
                if (element.GetType().Name.ToString() != SightType && SightType!=nameof(GameElement)) continue;
                //add return collision info
                if (element == cellParent) continue;
                if (nearZones.Contains(element.CurrentZone) == false) continue;
                float dist = cellParent.CalcDist(cellParent, element);
                if (dist - element.radius > lenght) continue;
                foreach (SRay ray in rays)
                {
                    List<CollisionInfo> collisions = new List<CollisionInfo>();
                    PointF res1 = new PointF();
                    PointF res2 = new PointF(); 
                    int amount = FindLineCircleIntersections(element.position.X, element.position.Y, element.radius, CustomMonoGameUtil.Vector2ToPoint(ray.start), CustomMonoGameUtil.Vector2ToPoint(ray.end), out res1, out res2);
                    if (amount != 0)
                    {
                        float d1 = (float)Distance(CustomMonoGameUtil.Vector2ToPoint(cellParent.position), res1);
                        float d2 = (float)Distance(CustomMonoGameUtil.Vector2ToPoint(cellParent.position), res2);
                        CollisionInfo ci = new CollisionInfo();

                        Vector2 dir1 = - ray.start + CustomMonoGameUtil.ToVector2(res1);
                        Vector2 dir2 = - ray.start + CustomMonoGameUtil.ToVector2(res1);


                        //REMOVE BACKWARD RAYS
                        float RelativeDir1 = Vector2.Dot(ray.dir, dir1);
                        float RelativeDir2 = Vector2.Dot(ray.dir, dir2);

                        if (RelativeDir2 < 0 && RelativeDir1 >= 0)
                        {
                            ci.point = CustomMonoGameUtil.ToVector2(res1);
                            ci.distance = d1;
                        }
                        else if (RelativeDir1 < 0 && RelativeDir2 >= 0)
                        {
                            ci.point = CustomMonoGameUtil.ToVector2(res2);
                            ci.distance = d2;
                        }
                        else if (RelativeDir1 < 0 && RelativeDir2 < 0) continue;
                        else
                        {
                            if (d1 <= d2)
                            {
                                ci.point = CustomMonoGameUtil.ToVector2(res1);
                                ci.distance = d1;
                            }
                            else
                            {
                                ci.point = CustomMonoGameUtil.ToVector2(res2);
                                ci.distance = d2;
                            }
                        }
                        ci.collideswith = element;
                        ci.collides = true;
                        collisions.Add(ci);
                        ray.colls = collisions;
                    }
                }
            }
        }

        //cx,cy is center point of the circle 
        public PointF ClosestIntersection(float cx, float cy, float radius,
                                          PointF lineStart, PointF lineEnd)
        {
            PointF intersection1;
            PointF intersection2;
            int intersections = FindLineCircleIntersections(cx, cy, radius, lineStart, lineEnd, out intersection1, out intersection2);

            if (intersections == 1)
                return intersection1; // one intersection

            if (intersections == 2)
            {
                double dist1 = Distance(intersection1, lineStart);
                double dist2 = Distance(intersection2, lineStart);

                if (dist1 < dist2)
                    return intersection1;
                else
                    return intersection2;
            }

            return PointF.Empty; // no intersections at all
        }

        private double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        // Find the points of intersection.
        private int FindLineCircleIntersections(float cx, float cy, float radius,PointF point1, PointF point2, out PointF intersection1, out PointF intersection2)
        {
            float dx, dy, A, B, C, det, t;

            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
            C = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new PointF(float.NaN, float.NaN);
                intersection2 = new PointF(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                intersection2 = new PointF(float.NaN, float.NaN);
                return 1;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                return 2;
            }
        }


        public void VisualRepr(SpriteBatch spriteBatch)
        {
            if (updated == 0)
            {
                UpdateRays(true, spriteBatch);
            }
            else
            {
                for (int i = 0; i < rays.Count; i++)
                {
                   
                    CustomMonoGameUtil.DrawLine(spriteBatch, rays[i].start, rays[i].end, rays[i].color, 2);
                }
            }
        }

        public override dynamic RUN(List<GameElement> elements)
        {
            if (RaysInitialized==false) initializeRays();
            UpdateRays();
            updated++;
            List<float> collisionRays = new List<float>();
            CheckCollisionSight(elements);
            foreach (SRay ray in rays)
            {
                
                if (ray.colls.Count!=0)
                {   
                    ray.color = new Microsoft.Xna.Framework.Color(255, 0, 0, 0.1f);
                }
                else
                {
                    ray.color = new Microsoft.Xna.Framework.Color(0, 255, 0, 0.1f);
                }

                if (ray.colls.Count == 0) collisionRays.Add(0);
                foreach (CollisionInfo coll in ray.colls)
                {
                    if (coll.distance < 0) coll.distance = 0;
                    float returnV = (10 * (lenght - coll.distance)) / lenght;
                    if (returnV < 0 ) returnV = 0;
                    collisionRays.Add(returnV);
                }
                ray.colls = new List<CollisionInfo>();
            }
            updated = 0;

            
            return collisionRays;
        }
    }

    internal class Position : Component
    {
        public float accuracy;
        public bool activated = false;

        public Position()
        {
            componentFamilyName = "POSITION";
            componentType = "INPUT";
            DebugName = "Normal";
        }

        public void Initialize()
        {
            accuracy = components_stats[0];
            CostXtime = components_stats[1]/100;
            activated = true;
        }

        public override dynamic RUN(List<GameElement> elements)
        {
            results.Clear();
            if (!activated) Initialize();
            float positionX = cellParent.position.X + GameInst.Mrand.Next((-10+(int)accuracy)*GameInst.screenWidth/100, (10-(int)accuracy) * GameInst.screenWidth / 100);
            float positionY = cellParent.position.Y + GameInst.Mrand.Next((-10 + (int)accuracy) * GameInst.screenHeight / 100, (10 - (int)accuracy) * GameInst.screenHeight / 100);
            
            if(positionX>GameInst.screenWidth) positionX = GameInst.screenWidth;
            else if(positionX <0) positionX = 0;

            if (positionY>GameInst.screenHeight) positionY = GameInst.screenHeight;
            else if(positionY <0) positionY = 0;

            positionX = (10 * positionX) / GameInst.screenWidth;
            positionY = (10*positionY) / GameInst.screenHeight;

            results.Add(positionX);
            results.Add(positionY);

            return results;
        }
    }

    internal class Pheromone : Component
    {
        public float SmellRange;
        public float SmellActivation;
        public float SmellChannel;
        public bool activated = false;

        public Pheromone()
        {
            componentFamilyName = "PHEROMONE";
            componentType = "INPUT";
            DebugName = "Normal";
        }

        public void Initialize()
        {
            SmellRange = components_stats[0]/2;
            SmellActivation = components_stats[1];
            SmellChannel = components_stats[2];
            CostXtime = components_stats[3] / 100;
            activated = true;
        }

        public override dynamic RUN(List<GameElement> elements)
        {
            results.Clear();
            if (!activated) Initialize();
            int channel;
            if (SmellChannel < 5) channel = 0;
            else channel = 1;
            int cellPSX = (int)cellParent.position.X / GameInst.pheroSize;
            int cellPSY = (int)cellParent.position.Y / GameInst.pheroSize;
            Vector2 parentADJ_TILE = new Vector2(cellPSX, cellPSY);
            List<PheromoneZone> foundZones = findRange(parentADJ_TILE, SmellRange, GameInst.PheromoneZones);
            float res = 0;
            foundZones.ForEach(zone => res += zone.Pheromones[channel]);
            res = res / foundZones.Count;
            results.Add(res);
            results.Add(res);
            return results;
        }

        public List<PheromoneZone> findRange(Vector2 tile, float range, List<PheromoneZone> zones)
        {
            List<PheromoneZone> found = new List<PheromoneZone>();

            int starty = (int)Math.Max(0, (tile.Y - range));
            int endy = (int)Math.Min(GameInst.worldPheroRowN - 1, (tile.Y + range));
            int row = 0;
            for (row = starty; row <= endy; row++)
            {
                int xrange = (int)(range - Math.Abs(row - tile.Y));

                int startx = (int)Math.Max(0, (tile.X - xrange));
                int endx = (int)Math.Min(GameInst.worldPheroColN - 1, (tile.X + xrange));
                int col;
                for (col = startx; col <= endx; col++)
                {
                    int index = row * GameInst.worldPheroColN + col;
                    //Console.WriteLine(index);
                    found.Add(zones[index]);
                }
            }

            return found;
        }

    }

    internal class Cilia : Component
    {
        public float ActivationValue;
        public float costPT;
        public float speed;
        public float rotationSpeed;
        public bool activated = false;

        public float ActivationFunction(float inp)
        {
            if (inp < -ActivationValue) return inp + ActivationValue;
            else if (inp > ActivationValue) return inp - ActivationValue;
            else return 0;

        }
        public Cilia()
        {
            componentFamilyName = "CILIA";
            componentType = "OUTPUT";
            DebugName = "Normal";
        }

        public void Initialize()
        {
            ActivationValue = components_stats[0];
            costPT = 2*components_stats[1]/100;
            speed = components_stats[2]/100;
            rotationSpeed = components_stats[3]/1000;
            activated = true;
        }

        public override dynamic RUN(List<GameElement> elements)
        {
            
            if (!activated) Initialize();
            if (inputN>0)
            {
                float usedInput = ActivationFunction(inputV);
                if (usedInput > 10) usedInput = 10;
                else if (usedInput < -10) usedInput = -10;

                if (usedInput > 0)
                {
                    cellParent.angularVelocity += rotationSpeed*usedInput/7;
                }else if (usedInput < 0){
                    cellParent.angularVelocity += rotationSpeed*usedInput/7;
                }

                inputN = 0;
                inputV = 0;
                return usedInput;
            }
            return null;
        }
    }

    internal class Flagellum : Component
    {
        public float ActivationValue;
        public float costPT;
        public float speed;
        public float rotationSpeed;
        public bool activated = false;

        public float ActivationFunction(float inp)
        {
            if (inp < -ActivationValue) return inp + ActivationValue;
            else if (inp > ActivationValue) return inp - ActivationValue;
            else return 0;

        }
        public Flagellum()
        {
            componentFamilyName = "FLAGELLUM";
            componentType = "OUTPUT";
            DebugName = "Normal";
        }

        public void Initialize()
        {
            ActivationValue = components_stats[0]/2;
            costPT = 1.5f*components_stats[1]/100;
            speed = components_stats[2]/100;
            rotationSpeed = components_stats[3]/1000;
            activated = true;
        }

        public override dynamic RUN(List<GameElement> elements)
        {
            if (!activated) Initialize();
            if (inputN > 0)
            {
                float usedInput = ActivationFunction(inputV);
                if (usedInput > 10) usedInput = 10;
                else if (usedInput < -10) usedInput = -10;

                if (usedInput > 0)
                {
                    cellParent.velocity += speed * usedInput / 7;
                }
                else if (usedInput < 0)
                {
                    cellParent.velocity += speed * usedInput / 7;
                }

                inputN = 0;
                inputV = 0;
                return usedInput;
            }
            return null;
        }
    }

    internal class PheroRelease : Component
    {
        public float ActivationValue;
        public float channel;
        public float AmountXTime;
        public float DeployRange;
        public bool activated = false;

        public float ActivationFunction(float inp)
        {
            if (inp < -ActivationValue) return inp + ActivationValue;
            else if (inp > ActivationValue) return inp - ActivationValue;
            else return 0;

        }
        public PheroRelease()
        {
            componentFamilyName = "PHEROMONE RELEASE";
            componentType = "OUTPUT";
            DebugName = "Normal";
        }

        public void Initialize()
        {
            ActivationValue = components_stats[0] / 2;
            channel = components_stats[1];
            if (channel < 5) channel = 0;
            else  channel = 1;
            AmountXTime = components_stats[2] / 500;
            CostXtime = AmountXTime * 5 * 1.5f;
            DeployRange = components_stats[3]/2;
            activated = true;
        }

        public List<PheromoneZone> findRange(Vector2 tile, float range, List<PheromoneZone> zones)
        {
            List<PheromoneZone> found = new List<PheromoneZone>();

            int starty = (int)Math.Max(0, (tile.Y - range));
            int endy = (int)Math.Min(GameInst.worldPheroRowN - 1, (tile.Y + range));
            int row = 0;
            for (row = starty; row <= endy; row++)
            {
                int xrange = (int)(range - Math.Abs(row - tile.Y));

                int startx = (int)Math.Max(0, (tile.X - xrange));
                int endx = (int)Math.Min(GameInst.worldPheroColN - 1, (tile.X + xrange));
                int col;
                for (col = startx; col <= endx; col++)
                {
                    int index = row * GameInst.worldPheroColN + col;
                    //Console.WriteLine(index);
                    found.Add(zones[index]);
                }
            }

            return found;
        }

        public override dynamic RUN(List<GameElement> elements)
        {
            if (!activated) Initialize();
            if (inputN > 0)
            {
                int cellPSX = (int)cellParent.position.X / GameInst.pheroSize;
                int cellPSY = (int)cellParent.position.Y / GameInst.pheroSize;
                Vector2 parentADJ_TILE = new Vector2(cellPSX, cellPSY);
                List<PheromoneZone> foundZones = findRange(parentADJ_TILE, DeployRange, GameInst.PheromoneZones);
                foundZones.ForEach(zone => zone.Pheromones[(int)channel] += AmountXTime);
            }
            return null;
        }
    }
}