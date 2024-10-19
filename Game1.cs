using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._SaveData;
using Advencursor._Scene;
using Advencursor._Scene.Stage;
using Advencursor._Scene.Transition;
using Advencursor._Skill;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Advencursor
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private readonly SceneManager _sceneManager;
        private Camera camera;
        private GameData gameData;
        private SkillData skillData;
        float blendFactor = 0.0f; // Start in full color
        float globalOpacity = 1.0f; // Full opacity


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _sceneManager = new();
            camera = new Camera();
            gameData = new GameData();
            skillData = new SkillData();
        }


        protected override void Initialize()
        {
            Globals.Bounds = new(1920, 1080);
            _graphics.PreferredBackBufferWidth = Globals.Bounds.X;
            _graphics.PreferredBackBufferHeight = Globals.Bounds.Y;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            Window.IsBorderless = false;
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.ApplyChanges();

            Globals.Content = Content;
            Globals.Game = this;
            Globals.graphicsDevice = GraphicsDevice;
            Globals.Viewport = GraphicsDevice.Viewport;
            Globals.Camera = camera;
            Globals.grayScaleEffect = Content.Load<Effect>("greyScale");
            Globals.SetGreyScale(1.0f);
            Globals.fullScreenRectangle = new Rectangle(0, 0, Globals.Bounds.X, Globals.Bounds.Y);

            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = _spriteBatch;
            Globals.SpriteFont = _font;

            gameData.LoadData();
            skillData.LoadData();
            AllSkills.skillData = skillData;
            AllSkills.Reset();
            _sceneManager.AddScene(new DialogueIntro(Content, _sceneManager));
        }

        protected override void Update(GameTime gameTime)
        {
            Globals.Update(gameTime);
            InputManager.Update();
            TimeManager.Update(gameTime);
            _sceneManager.Update(gameTime);
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(transformMatrix: Globals.Camera.transform);
            _sceneManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
