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
using Advencursor._SaveData;

namespace Advencursor._Scene
{
    public class MenuScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private Texture2D background;
        private GameData gameData;
        public MenuScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();
            gameData = new GameData();
        }

        public void Load()
        {
            int posX = 400;
            int posY = 250;
            int yOffset = 150;
            UIButton playButton = new(Globals.Content.Load<Texture2D>("Button/playButton"), new Vector2(posX, posY * 0 + yOffset), OnPlayButtonClick);
            UIButton gachaButton = new(Globals.Content.Load<Texture2D>("Button/gachaMenuButton"), new Vector2(posX, posY * 1 + yOffset), OnGachaButtonClick);
            UIButton settingButton = new(Globals.Content.Load<Texture2D>("Button/settingButton"), new Vector2(posX, posY * 2 + yOffset), OnSettingButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("Button/exitButton"), new Vector2(posX, posY * 3 + yOffset), OnExitButtonClick);
            uiManager.AddElement("playButton", playButton);
            uiManager.AddElement("gachaButton", gachaButton);
            uiManager.AddElement("settingButton", settingButton);
            uiManager.AddElement("exitButton", exitButton);
            gameData.LoadData();
            if (gameData.isFirstTime)
            {
                uiManager.SetDark("gachaButton", true);
            }
            Globals.soundManager.StopAllSounds();
            Globals.soundManager.SetGlobalSoundEffectVolume(gameData.volumeEffect);
            background = Globals.Content.Load<Texture2D>("Background/Menu");
        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = false;
            uiManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            uiManager.Draw(spriteBatch);
            Globals.DrawCursor();
        }

        private void OnPlayButtonClick()
        {
            if (gameData.isFirstTime)
                sceneManager.AddScene(new DialogueIntro(contentManager, sceneManager));
            else
                sceneManager.AddScene(new StageSelectScene(contentManager, sceneManager));
        }
        private void OnGachaButtonClick()
        {
            if (gameData.isFirstTime) return;
            gameData.SaveData();
            sceneManager.AddScene(new GachaScene(contentManager, sceneManager));
        }
        private void OnSettingButtonClick()
        {
            gameData.SaveData();
            sceneManager.AddScene(new SettingScene(contentManager, sceneManager));
        }
        private void OnExitButtonClick()
        {
            Globals.Game.Exit();
        }
    }
}
