using Advencursor._Models;
using Advencursor._SaveData;
using Advencursor._Scene.Stage;
using Advencursor._UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene
{
    public class SettingScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private Texture2D background;
        private Texture2D nocheckButton;
        private Texture2D checkButton;
        private GameData gameData;
        public SettingScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();
            gameData = new GameData();
        }

        public void Load()
        {
            checkButton = Globals.Content.Load<Texture2D>("UI/Setting/CheckButton");
            nocheckButton = Globals.Content.Load<Texture2D>("UI/Setting/NoCheckButton");

            Vector2 buttonPosStart = new Vector2(Globals.Bounds.X/2 + 150,(Globals.Bounds.Y/2) - 48);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("UI/Setting/ExitButton"), new Vector2(Globals.Bounds.X/2,Globals.Bounds.Y - 300), OnExitButtonClick);
            UIBackground settingBG = new(Globals.Content.Load<Texture2D>("UI/Setting/SettingUI"), new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2));
            UIButton musicOnButton = new(nocheckButton, buttonPosStart + new Vector2(0,0), OnMusicOnButtonClick);
            UIButton musicOffButton = new(nocheckButton, buttonPosStart + new Vector2(100, 0), OnMusicOffButtonClick);
            UIButton effectOnButton = new(nocheckButton, buttonPosStart + new Vector2(0, 100), OnEffectOnButtonClick);
            UIButton effectOffButton = new(nocheckButton, buttonPosStart + new Vector2(100, 100), OnEffectOffButtonClick);
            uiManager.AddElement("settingBG", settingBG);
            uiManager.AddElement("exitButton", exitButton);
            uiManager.AddElement("musicOnButton", musicOnButton);
            uiManager.AddElement("musicOffButton", musicOffButton);
            uiManager.AddElement("effectOnButton", effectOnButton);
            uiManager.AddElement("effectOffButton", effectOffButton);
            gameData.LoadData();
            ChangeButton();

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
        private void OnMusicOnButtonClick()
        {
            Globals.soundManager.SetSongVolume(0.2f);
            gameData.volumeMusic = 0.2f;
            gameData.SaveData();
            ChangeButton();
        }
        private void OnMusicOffButtonClick()
        {
            Globals.soundManager.SetSongVolume(0f);
            gameData.volumeMusic = 0f;
            gameData.SaveData();
            ChangeButton();
        }
        private void OnEffectOnButtonClick()
        {
            Globals.soundManager.SetGlobalSoundEffectVolume(1f);
            gameData.volumeEffect = 1f;
            gameData.SaveData();
            ChangeButton();
        }
        private void OnEffectOffButtonClick()
        {
            Globals.soundManager.SetGlobalSoundEffectVolume(0f);
            gameData.volumeEffect = 0f;
            gameData.SaveData();
            ChangeButton();
        }

        private void OnExitButtonClick()
        {
            gameData.SaveData();
            sceneManager.AddScene(new MenuScene(contentManager, sceneManager));
        }

        private void ChangeButton()
        {
            if (gameData.volumeMusic <= 0)
            {
                uiManager.ChangeTexture("musicOffButton", checkButton);
                uiManager.ChangeTexture("musicOnButton", nocheckButton);
            }
            else
            {
                uiManager.ChangeTexture("musicOffButton", nocheckButton);
                uiManager.ChangeTexture("musicOnButton", checkButton);
            }

            if(gameData.volumeEffect <= 0)
            {
                uiManager.ChangeTexture("effectOffButton", checkButton);
                uiManager.ChangeTexture("effectOnButton", nocheckButton);
            }
            else
            {
                uiManager.ChangeTexture("effectOffButton", nocheckButton);
                uiManager.ChangeTexture("effectOnButton", checkButton);
            }
        }
    }
}
