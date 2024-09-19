using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Scene;
using Advencursor._Scene.Stage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Advencursor
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private readonly SceneManager _sceneManager;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _sceneManager = new();
        }


        protected override void Initialize()
        {
            Globals.Bounds = new(1920, 1080);
            _graphics.PreferredBackBufferWidth = Globals.Bounds.X;
            _graphics.PreferredBackBufferHeight = Globals.Bounds.Y;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Globals.Content = Content;
            Globals.Game = this;
            Globals.graphicsDevice = GraphicsDevice; 
            Globals.Viewport = GraphicsDevice.Viewport;

            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = _spriteBatch;
            Globals.SpriteFont = _font;


            _sceneManager.AddScene(new MenuScene(Content, _sceneManager));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Globals.Game.Exit();

            Globals.Update(gameTime);
            InputManager.Update();
            TimeManager.Update(gameTime);
            _sceneManager.GetCurrScene().Update(gameTime);
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();
            _sceneManager.GetCurrScene().Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
