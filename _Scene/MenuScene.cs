using Advencursor._Models;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Advencursor._Scene.Stage;
using Advencursor._UI;

namespace Advencursor._Scene
{
    public class MenuScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private Texture2D background;
        public MenuScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();
        }

        public void Load()
        {
            UIButton playButton = new(Globals.Content.Load<Texture2D>("Button/playButton"), new Vector2(Globals.Bounds.X - 400 ,Globals.Bounds.Y/2),OnPlayButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("Button/exitButton"), new Vector2(Globals.Bounds.X - 400, (Globals.Bounds.Y / 2) + 300), OnExitButtonClick);
            uiManager.AddElement(playButton);
            uiManager.AddElement(exitButton);

            background = Globals.Content.Load<Texture2D>("Background/Menu");
        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = true;
            uiManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(background,Vector2.Zero,Color.White);
            uiManager.Draw(spriteBatch);
        }

        private void OnPlayButtonClick()
        {
            sceneManager.AddScene(new InventoryScene(contentManager, sceneManager));
        }

        private void OnExitButtonClick()
        {
            Globals.Game.Exit();
        }
    }
}
