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
using Microsoft.Xna.Framework.Media;
using Advencursor._Managers;

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
            int yOffset = 200;
            UIButton playButton = new(Globals.Content.Load<Texture2D>("Button/playButton"), new Vector2(posX + 1250, posY * 2 + yOffset), OnPlayButtonClick);
            UIButton gachaButton = new(Globals.Content.Load<Texture2D>("Button/gachaMenuButton"), new Vector2(posX + 1230, posY * 3 + yOffset), OnGachaButtonClick);
            UIButton settingButton = new(Globals.Content.Load<Texture2D>("Button/settingButton"), new Vector2(posX-100, posY * 2 + yOffset + 100), OnSettingButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("Button/exitButton"), new Vector2(posX-100, posY * 3 + yOffset), OnExitButtonClick);
            uiManager.AddElement("playButton", playButton);
            uiManager.AddElement("gachaButton", gachaButton);
            uiManager.AddElement("settingButton", settingButton);
            uiManager.AddElement("exitButton", exitButton);
            gameData.LoadData();
            if (gameData.isFirstTime)
            {
                uiManager.SetDark("gachaButton", true);
            }

            if (!gameData.stage3Clear)
            {
                Song bgsong = Globals.Content.Load<Song>("Sound/Song/Stage Select Song");
                Globals.soundManager.SetSongVolume(gameData.volumeMusic);
                Globals.soundManager.PlaySong("Stage Select Song", bgsong, true);
                Globals.SetGreyScale(0f);
                
            }
            else
            {
                Song bgsong = Globals.Content.Load<Song>("Sound/Song/Visual Novel Song");
                Globals.soundManager.PlaySong("Visual Novel Song", bgsong, true);
                Globals.SetGreyScale(1.0f);
            }
            Globals.soundManager.StopAllSounds();
            Globals.soundManager.SetGlobalSoundEffectVolume(gameData.volumeEffect);

            Globals.soundManager.SoundReset();
            background = Globals.Content.Load<Texture2D>("Background/Menu");
        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = false;
            uiManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.BeginDrawGrayScale();
            Globals.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            Globals.EndDrawGrayScale();
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
