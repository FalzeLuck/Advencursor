﻿using Advencursor._Models;
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

namespace Advencursor._Scene.Stage
{
    public class StageSelectScene : IScene
    {

        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private GameData gameData;

        private Texture2D background;

        private Vector2 screenCenter;
        private int moveButtonSpeed = 1000;
        private Vector2 posStage1;
        private Vector2 posStage1des;
        private Vector2 posStage2;
        private Vector2 posStage2des;

        private enum Stage
        {
            Stage1 = 1,
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
        }

        public void Load()
        {
            Texture2D nullTexture = new Texture2D(Globals.graphicsDevice, 1, 1);
            gameData.LoadData();
            screenCenter = new(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);



            UIButton stage1Button = new(Globals.Content.Load<Texture2D>("Button/Stage1Button"), new Vector2(Globals.Bounds.X / 2 , Globals.Bounds.Y / 2), OnStage1ButtonClick);
            UIButton stage2Button = new(Globals.Content.Load<Texture2D>("Button/Stage2Button"), new Vector2(Globals.Bounds.X / 2 + 500, Globals.Bounds.Y / 2), OnStage2ButtonClick);
            uiManager.AddElement("stage1Button", stage1Button);
            uiManager.AddElement("stage2Button", stage2Button);


            posStage1 = uiManager.GetElementPosition("stage1Button");
            posStage1des = uiManager.GetElementPosition("stage1Button");
            posStage2 = uiManager.GetElementPosition("stage2Button");
            posStage2des = uiManager.GetElementPosition("stage2Button");
            background = Globals.Content.Load<Texture2D>("Background/Stage1_2");

        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = true;
            uiManager.Update(gameTime);
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Rectangle mouseCollision = new((int)mousePosition.X, (int)mousePosition.Y, 1, 1);


            if (currentStage == 1)
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



            /*if (moveButtonSpeed == 0)
            {
                posStage1 = posStage1des;
                uiManager.SetElementPosition(posStage1, "stage1Button");
            }*/
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            uiManager.Draw(spriteBatch);
        }


        private void MoveAllButton(Vector2 direction)
        {
            posStage1 += direction * moveButtonSpeed * TimeManager.TotalSeconds;
            uiManager.SetElementPosition(posStage1, "stage1Button");
            posStage2 += direction * moveButtonSpeed * TimeManager.TotalSeconds;
            uiManager.SetElementPosition(posStage2, "stage2Button");
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
                posStage2des = screenCenter;
                currentStage = (int)Stage.Stage2;
                uiManager.SetElementPosition(posStage2, "stage2Button");
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
    }
}
