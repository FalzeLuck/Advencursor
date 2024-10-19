using Advencursor._Models;
using Advencursor._Skill;
using Advencursor._UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._SaveData;
using Advencursor._Managers;
using Microsoft.Xna.Framework.Media;

namespace Advencursor._Scene.Stage
{
    public class StageSelectScene : IScene
    {

        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;


        private SoundManager soundManager;
        private Song bgsong;

        private GameData gameData;

        private Texture2D background;

        private Vector2 screenCenter;
        private int moveButtonSpeed = 1000;
        private Vector2 gachaPos;
        private Vector2 gachaPosdes;
        private Vector2 posStage1;
        private Vector2 posStage1des;
        private Vector2 posStage2;
        private Vector2 posStage2des;
        private Vector2 posStage3;
        private Vector2 posStage3des;

        private enum Stage
        {
            Gacha,
            Stage1,
            Stage2,
            Stage3
        }
        private int currentStage;


        public StageSelectScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();
            gameData = new GameData();
            soundManager = new SoundManager();
        }

        public void Load()
        {
            Texture2D nullTexture = new Texture2D(Globals.graphicsDevice, 1, 1);
            gameData.LoadData();
            screenCenter = new(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);

            bgsong = Globals.Content.Load<Song>("Sound/Song/Stage Select Song");
            //soundManager.PlaySong("Stage Select Song",bgsong,true);
            soundManager.SetSongVolume(gameData.volumeMusic);

            UIButton exitButton = new(Globals.Content.Load<Texture2D>("UI/Gacha/ButtonExit2"), new Vector2(120, 75), OnExitButtonClick);
            UIButton gachaButton = new(Globals.Content.Load<Texture2D>("Button/GachaButton"), new Vector2(Globals.Bounds.X / 2 - 500, Globals.Bounds.Y / 2), OnGachaButtonClick);
            UIButton stage1Button = new(Globals.Content.Load<Texture2D>("Button/Stage1Button"), new Vector2(Globals.Bounds.X / 2 , Globals.Bounds.Y / 2), OnStage1ButtonClick);
            UIButton stage2Button = new(Globals.Content.Load<Texture2D>("Button/Stage2Button"), new Vector2(Globals.Bounds.X / 2 + 500, Globals.Bounds.Y / 2), OnStage2ButtonClick);
            UIButton stage3Button = new(Globals.Content.Load<Texture2D>("Button/Stage3Button"), new Vector2(Globals.Bounds.X / 2 + 1000, Globals.Bounds.Y / 2), OnStage3ButtonClick); ;
            uiManager.AddElement("exitButton", exitButton);
            uiManager.AddElement("gachaButton", gachaButton);
            uiManager.AddElement("stage1Button", stage1Button);
            uiManager.AddElement("stage2Button", stage2Button);
            uiManager.AddElement("stage3Button", stage3Button);


            gachaPos = uiManager.GetElementPosition("gachaButton");
            gachaPosdes = uiManager.GetElementPosition("gachaButton");
            posStage1 = uiManager.GetElementPosition("stage1Button");
            posStage1des = uiManager.GetElementPosition("stage1Button");
            posStage2 = uiManager.GetElementPosition("stage2Button");
            posStage2des = uiManager.GetElementPosition("stage2Button");
            posStage3 = uiManager.GetElementPosition("stage3Button");
            posStage3des = uiManager.GetElementPosition("stage3Button");
            background = Globals.Content.Load<Texture2D>("Background/StageSelect");

        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = false;
            uiManager.Update(gameTime);
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Rectangle mouseCollision = new((int)mousePosition.X, (int)mousePosition.Y, 1, 1);


            if (currentStage == 0)
            {
                Vector2 direction = GetVectorDirection(gachaPos, gachaPosdes);
                if (direction.X < 0)
                {
                    MoveAllButton(direction);
                    if (gachaPos.X <= gachaPosdes.X)
                    {
                        gachaPos = screenCenter;
                        uiManager.SetElementPosition(gachaPos, "gachaButton");
                    }
                }
                else if (direction.X > 0)
                {
                    MoveAllButton(direction);
                    if (gachaPos.X >= gachaPosdes.X)
                    {
                        gachaPos = screenCenter;
                        uiManager.SetElementPosition(gachaPos, "gachaButton");
                    }
                }
            }
            else if (currentStage == 1)
            {
                Vector2 direction = GetVectorDirection(posStage1, posStage1des);
                if (direction.X < 0)
                {
                    MoveAllButton(direction);
                    if (posStage1.X <= posStage1des.X)
                    {
                        posStage1 = screenCenter;
                        uiManager.SetElementPosition(posStage1, "stage1Button");
                    }
                }
                else if (direction.X > 0)
                {
                    MoveAllButton(direction);
                    if (posStage1.X >= posStage1des.X)
                    {
                        posStage1 = screenCenter;
                        uiManager.SetElementPosition(posStage1, "stage1Button");
                    }
                }
            } else if (currentStage == 2)
            {
                Vector2 direction = GetVectorDirection(posStage2, posStage2des);
                if (direction.X < 0)
                {
                    MoveAllButton(direction);
                    if (posStage2.X <= posStage2des.X)
                    {
                        posStage2 = screenCenter;
                        uiManager.SetElementPosition(posStage2, "stage2Button");
                    }
                }
                else if (direction.X > 0)
                {
                    MoveAllButton(direction);
                    if (posStage2.X >= posStage2des.X)
                    {
                        posStage2 = screenCenter;
                        uiManager.SetElementPosition(posStage2, "stage2Button");
                    }
                }
            }
            else if (currentStage == 3)
            {
                Vector2 direction = GetVectorDirection(posStage3, posStage3des);
                if (direction.X < 0)
                {
                    MoveAllButton(direction);
                    if (posStage3.X <= posStage3des.X)
                    {
                        posStage3 = screenCenter;
                        uiManager.SetElementPosition(posStage3, "stage3Button");
                    }
                }
                else if (direction.X > 0)
                {
                    MoveAllButton(direction);
                    if (posStage3.X >= posStage3des.X)
                    {
                        posStage3 = screenCenter;
                        uiManager.SetElementPosition(posStage3, "stage3Button");
                    }
                }
            }


            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            uiManager.Draw(spriteBatch);
            Globals.DrawCursor();
        }


        private void MoveAllButton(Vector2 direction)
        {
            gachaPos += direction * moveButtonSpeed * TimeManager.TotalSeconds;
            uiManager.SetElementPosition(gachaPos, "gachaButton");
            posStage1 += direction * moveButtonSpeed * TimeManager.TotalSeconds;
            uiManager.SetElementPosition(posStage1, "stage1Button");
            posStage2 += direction * moveButtonSpeed * TimeManager.TotalSeconds;
            uiManager.SetElementPosition(posStage2, "stage2Button");
            posStage3 += direction * moveButtonSpeed * TimeManager.TotalSeconds;
            uiManager.SetElementPosition(posStage3, "stage3Button");
        }





        private void OnGachaButtonClick()
        {
            if (uiManager.GetElementPosition("gachaButton") == screenCenter)
            {
                gameData.stage = currentStage = (int)Stage.Gacha;
                gameData.SaveData();
                sceneManager.AddScene(new GachaScene(contentManager, sceneManager));
            }
            else if (uiManager.GetElementPosition("gachaButton").X > screenCenter.X || uiManager.GetElementPosition("gachaButton").X < screenCenter.X)
            {
                gameData.SaveData();
                gachaPosdes = screenCenter;
                currentStage = (int)Stage.Gacha;
                uiManager.SetElementPosition(gachaPos, "gachaButton");
            }
        }

        private void OnStage1ButtonClick()
        {
            if (uiManager.GetElementPosition("stage1Button") == screenCenter)
            {
                gameData.stage = currentStage = (int)Stage.Stage1;
                gameData.SaveData();
                sceneManager.AddScene(new InventoryScene(contentManager, sceneManager));
            }
            else if (uiManager.GetElementPosition("stage1Button").X > screenCenter.X || uiManager.GetElementPosition("stage1Button").X < screenCenter.X)
            {
                gameData.SaveData();
                posStage1des = screenCenter;
                currentStage = (int)Stage.Stage1;
                uiManager.SetElementPosition(posStage1, "stage1Button");
            }
        }
        private void OnStage2ButtonClick()
        {
            if (uiManager.GetElementPosition("stage2Button") == screenCenter)
            {
                gameData.stage = currentStage = (int)Stage.Stage2;
                gameData.SaveData();
                sceneManager.AddScene(new InventoryScene(contentManager, sceneManager));
            }
            else if (uiManager.GetElementPosition("stage2Button").X > screenCenter.X || uiManager.GetElementPosition("stage2Button").X < screenCenter.X)
            {
                gameData.SaveData();
                posStage2des = screenCenter;
                currentStage = (int)Stage.Stage2;
                uiManager.SetElementPosition(posStage2, "stage2Button");
            }
        }
        private void OnStage3ButtonClick()
        {
            if (uiManager.GetElementPosition("stage3Button") == screenCenter)
            {
                gameData.stage = currentStage = (int)Stage.Stage3;
                gameData.SaveData();
                sceneManager.AddScene(new InventoryScene(contentManager, sceneManager));
            }
            else if (uiManager.GetElementPosition("stage3Button").X > screenCenter.X || uiManager.GetElementPosition("stage3Button").X < screenCenter.X)
            {
                gameData.SaveData();
                posStage3des = screenCenter;
                currentStage = (int)Stage.Stage3;
                uiManager.SetElementPosition(posStage3, "stage3Button");
            }
        }
        private Vector2 GetVectorDirection(Vector2 position, Vector2 destination)
        {
            Vector2 direction = destination - position;
            direction.Normalize();

            return direction;
        }

        private void Cleanup()
        {
            background = null;

        }

        private void OnExitButtonClick()
        {
            sceneManager.AddScene(new MenuScene(contentManager, sceneManager));
        }
    }
}
