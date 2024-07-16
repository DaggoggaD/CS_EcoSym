using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    public class GameInst : Game
    {
        public static float deltatime = 0.1f;
        public static int screenWidth = 1200;
        public static int screenHeight = 600;
        public static int worldSizeX = 3000;
        public static int worldSizeY = 3000;
        public static int worldPheroRowN = 0;
        public static int worldPheroColN = 0;
        public static int pheroSize = 20;
        public static int ZoneSize = 100;
        public static int cellAmount = 500;


        public static Random Mrand = new Random();
        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        internal List<GameElement> cells = new List<GameElement>();
        internal List<GameElement> plants = new List<GameElement>();
        internal List<GameElement> objects = new List<GameElement>();
        internal static List<PheromoneZone> PheromoneZones = new List<PheromoneZone>();
        private SpriteFont font;
        float framerate = 0;
        SimpleFps simpleFps = new SimpleFps();
        Camera2D camera2D = new Camera2D();
        List<GameElement> toRemoveCells = new List<GameElement>();

        Texture2D cellTexture;
        Texture2D square;
        Texture2D circle;
        
        public void SpawnInitialFood(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Plant plant = new Plant();
                plants.Add(plant);
                objects.Add(plant);
            }
        }
        
        public void SpawnInitialCell(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Cell cell = new Cell();
                Genome genome;
                genome = GenomeHandler.GenerateGenome(5, 7, 8, 24);
                cell.ActiveComponents = genome.components;
                cell.Connections = genome.connections;
                cell.InitializeComponents(Custom:false);
                cell.position = cell.randomPos();
                cell.rotation = cell.randomRot();

                //cell.position = new System.Numerics.Vector2(500 - 10 * i, 250 - 150 * i);
                //cell.rotation = i * 3.14f;

                cells.Add(cell);
                objects.Add(cell);
            }

        }

        public GameInst()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void SpawnPheroZones(int size)
        {
            int startPosX = 0;
            int startPosY = 0;
            int numberOfZones = (worldSizeY / size + 1)*(worldSizeX / size + 1);
            int RowLenght = worldSizeX;
            worldPheroRowN = worldSizeX/size + 1;
            worldPheroColN = worldSizeY/size + 1;
            Console.WriteLine(worldPheroRowN);
            Console.WriteLine(worldPheroColN);
            Console.WriteLine(numberOfZones);

            int i = 0;
            
            while (i<numberOfZones)
            {
                PheromoneZone zone = new PheromoneZone();
                zone.pos = new Vector2(startPosX, startPosY);
                zone.size = size;
                zone.WorldZone = 0;
                zone.Adjust();
                //Test
                zone.Pheromones[0] = 0; // (float)Mrand.NextDouble()*10;
                zone.Pheromones[1] = 0; //(float)Mrand.NextDouble()*10;
                PheromoneZones.Add(zone);
                startPosX += size;
                if (startPosX > worldSizeX)
                {
                    startPosX = 0;
                    startPosY += size;
                }
                i++;
            }

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SpawnInitialCell(cellAmount);
            SpawnInitialFood(200);
            SpawnPheroZones(size:pheroSize);
            //PheromoneZones.ForEach(zone => Console.WriteLine(zone.REPR()));
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("FPS");
            // TODO: use this.Content to load your game content here
            cellTexture = Content.Load<Texture2D>("cell2");
            square = Content.Load<Texture2D>("Square");
            circle = Content.Load<Texture2D>("circle");
        }

        public Vector3 GetScreenScale()
        {
            var scaleX = (float)GraphicsDevice.Viewport.Width / (float)screenWidth;
            var scaleY = (float)GraphicsDevice.Viewport.Height / (float)screenHeight;
            return new Vector3(scaleX, scaleY, 1.0f);
        }

        protected override void Update(GameTime gameTime)
        {

            //FPS AND CLEAR
            simpleFps.Update(gameTime);
            GraphicsDevice.Clear(Color.CornflowerBlue);


            //CAMERA

            KeyboardState state = Keyboard.GetState();
            Vector2 dir = new Vector2();

            if (state.IsKeyDown(Keys.Right) && camera2D.Position.X > -worldSizeX + screenWidth)
                dir.X = -10f;
            if (state.IsKeyDown(Keys.Left) && camera2D.Position.X < 0)
                dir.X = 10f;
            if (state.IsKeyDown(Keys.Up) && camera2D.Position.Y < 0)
                dir.Y = 10f;
            if (state.IsKeyDown(Keys.Down) && camera2D.Position.Y > -worldSizeY + screenHeight)
                dir.Y = -10f;
            camera2D.Move(dir);

            

            //EXIT GAME
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            Task.Factory.StartNew(() =>
            Parallel.ForEach(cells.ToList(), cell =>
            {
                cell.Update(cells, plants, objects, _spriteBatch);
            }
            ));
            Parallel.ForEach(PheromoneZones, zone =>
            {
                if (zone.Pheromones[0] > 0) zone.Pheromones[0] -= 0.007f;
                if (zone.Pheromones[1] > 0) zone.Pheromones[1] -= 0.007f;
                if ((zone.Pheromones[0] > 10)) zone.Pheromones[0] = 10;
                if ((zone.Pheromones[1] > 10)) zone.Pheromones[1] = 10;
            }
            );
            
            int toSpawn = cellAmount - cells.Count;
            SpawnInitialCell(toSpawn);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //CAMERA
            var screenScale = GetScreenScale();
            var viewMatrix = camera2D.GetTransform();
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied,
                           null, null, null, null, viewMatrix * Matrix.CreateScale(screenScale));
            //UPDATE CELLS AND PHEROMONES
            PheromoneZones.ForEach(zone => zone.visualRepr(_spriteBatch, square));

            int i = 0;
            foreach (Cell cell in cells.ToList())
            {
                if (cell.currLife<0)
                {
                    i++;
                    cells.Remove(cell);
                    objects.Remove(cell);
                    continue;
                }
                cell.drawSelf(cellTexture, _spriteBatch);
            }

            foreach (Plant plant in plants)
            {
                plant.drawSelf(circle, _spriteBatch);
                plant.UpdateZones();
            }

            _spriteBatch.DrawString(font, cells.Count.ToString(), new Vector2(screenWidth-400, 10), Color.Black);
            simpleFps.DrawFps(_spriteBatch, font, new Vector2(10, 10), Color.Black);
            _spriteBatch.End();
            base.Draw(gameTime);
        }


    }
}
