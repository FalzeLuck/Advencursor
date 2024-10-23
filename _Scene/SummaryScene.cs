using Advencursor._Animation;
using Advencursor._Models;
using Advencursor._SaveData;
using Advencursor._Scene.Stage;
using Advencursor._Scene.Transition;
using Advencursor._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Advencursor._Scene
{
    public class SummaryScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private Texture2D bg;
        private bool win;
        private int gemAmount;
        private float time;
        private GameData gameData;
        private SpriteFont font;
        private Animation summaryScreen;

        private Texture2D black;

        //TimeText
        private string formattedTime;
        private char[] displayTime;
        private float shuffleTimeTimer = 0f;
        private float shuffleTimeInterval = 0.0001f;
        private int shuffleTimeMax = 20;
        private int shuffleTimeIndex = 0;
        private int currentTimeCharIndex = 0;
        private bool shuffleTimeComplete = false;

        //Gem Text
        private string formattedGems;
        private char[] displayGems;
        private float shuffleGemsTimer = 0f;
        private float shuffleGemsInterval = 0.0001f;
        private int shuffleGemsMax = 20;
        private int shuffleGemsIndex = 0;
        private int currentGemsCharIndex = 0;
        private bool shuffleGemsComplete = false;
        private Texture2D gemTexture;

        private string charPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:. ";

        public SummaryScene(ContentManager contentManager, SceneManager sceneManager, bool win, int gemAmount, GameData gameData, float time)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            this.uiManager = new UIManager();
            this.win = win;
            this.gemAmount = gemAmount;
            this.time = time;
            this.gameData = gameData;
            switch (gameData.stage)
            {
                case 1:
                    bg = Globals.Content.Load<Texture2D>("Background/BG_Stage1");
                    break;
                case 2:
                    bg = Globals.Content.Load<Texture2D>("Background/BG_Stage2");
                    break;
                case 3:
                    bg = Globals.Content.Load<Texture2D>("Background/BG_Stage3");
                    break;
            }
        }

        public void Load()
        {
            Globals.soundManager.StopAllSounds();
            font = Globals.Content.Load<SpriteFont>("Font/TextFont");
            gemTexture = Globals.Content.Load<Texture2D>("UI/Gacha/Gem");
            black = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Black);
            if (win)
            {
                summaryScreen = new Animation(Globals.Content.Load<Texture2D>("UI/Summary/Win"), 1, 2, 2, true);
            }
            else
            {
                summaryScreen = new Animation(Globals.Content.Load<Texture2D>("UI/Summary/Lose"), 1, 7, 8, false);
            }

            StartShuffleTime();
            StartShuffleGems();

            gameData.gems += gemAmount;
            gameData.SaveData();

            int buttonY = (Globals.Bounds.Y / 2) + 150;
            int buttonX = (Globals.Bounds.X / 2) - 200;
            UIElement menuButton = new UIButton(Globals.Content.Load<Texture2D>("UI/Summary/MenuButton"), new Vector2(buttonX, buttonY),OnMenuClick);
            UIElement restartButton = new UIButton(Globals.Content.Load<Texture2D>("UI/Summary/RestartButton"), new Vector2(buttonX + 400, buttonY), OnRestartClick);
            uiManager.AddElement("menuButton", menuButton);
            uiManager.AddElement("restartButton", restartButton);
        }

        public void Update(GameTime gameTime)
        {
            if (win)
            {
                summaryScreen.Update();
            }
            if (!win && summaryScreen.currentFrame != 6)
            {
                summaryScreen.Update();
            }

            if (!shuffleTimeComplete)
            {
                shuffleTimeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (shuffleTimeTimer >= shuffleTimeInterval && currentTimeCharIndex < displayTime.Length)
                {
                    shuffleTimeTimer = 0f;

                    if (shuffleTimeIndex >= shuffleTimeMax)
                    {
                        shuffleTimeIndex = 0;
                        displayTime[currentTimeCharIndex] = formattedTime[currentTimeCharIndex];
                        currentTimeCharIndex++;
                    }
                    else if (displayTime[currentTimeCharIndex] != formattedTime[currentTimeCharIndex])
                    {
                        displayTime[currentTimeCharIndex] = GetRandomChar();
                        shuffleTimeIndex++;
                    }
                    else
                    {
                        shuffleTimeIndex = 0;
                        currentTimeCharIndex++;
                    }

                    if (currentTimeCharIndex >= displayTime.Length)
                    {
                        shuffleTimeComplete = true;
                    }
                }
            }

            if (!shuffleGemsComplete)
            {
                shuffleGemsTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (shuffleGemsTimer >= shuffleGemsInterval && currentGemsCharIndex < displayGems.Length)
                {
                    shuffleGemsTimer = 0f;

                    if (shuffleGemsIndex >= shuffleGemsMax)
                    {
                        shuffleGemsIndex = 0;
                        displayGems[currentGemsCharIndex] = formattedGems[currentGemsCharIndex];
                        currentGemsCharIndex++;
                    }
                    else if (displayGems[currentGemsCharIndex] != formattedGems[currentGemsCharIndex])
                    {
                        displayGems[currentGemsCharIndex] = GetRandomChar();
                        shuffleGemsIndex++;
                    }
                    else
                    {
                        shuffleGemsIndex = 0;
                        currentGemsCharIndex++;
                    }

                    if (currentGemsCharIndex >= displayGems.Length)
                    {
                        shuffleGemsComplete = true;
                    }
                }
            }

            uiManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 screenCenter = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
            spriteBatch.Draw(black, Vector2.Zero, Color.White);
            spriteBatch.Draw(bg, Vector2.Zero, Color.White * 0.2f);
            summaryScreen.Draw(screenCenter);

            spriteBatch.DrawString(font, new string(displayTime), new Vector2(screenCenter.X - 270, screenCenter.Y - 230), Color.Black, 0, Vector2.Zero, 0.22f, SpriteEffects.None, 0);
            Vector2 gemSize = font.MeasureString(formattedGems);
            Vector2 gemOrigin = new Vector2(gemSize.X / 2, gemSize.Y / 2);
            spriteBatch.DrawString(font, new string(displayGems), new Vector2(screenCenter.X, screenCenter.Y), Color.Black, 0, gemOrigin, 0.8f, SpriteEffects.None, 0);

            Vector2 stringSize = font.MeasureString(new string(displayGems));
            Vector2 gemPos = new Vector2(screenCenter.X + stringSize.X/2 + 50, screenCenter.Y - 20);
            Vector2 gemTextureOrigin = new Vector2(gemTexture.Width/2,gemTexture.Height/2);
            spriteBatch.Draw(gemTexture, gemPos,null, Color.White,0,gemOrigin,0.5f,SpriteEffects.None,0f);

            uiManager.Draw(spriteBatch);

            Globals.DrawCursor();
        }

        private void StartShuffleTime()
        {
            formattedTime = TimeSpan.FromSeconds(time).ToString(@"mm\:ss\.ff");
            displayTime = new char[formattedTime.Length];

            for (int i = 0; i < displayTime.Length; i++)
            {
                if (char.IsDigit(formattedTime[i]) || formattedTime[i] == ':' || formattedTime[i] == '.')
                {
                    displayTime[i] = GetRandomChar();
                }
                else
                {
                    displayTime[i] = formattedTime[i];
                }
            }

            shuffleTimeComplete = false;
            currentTimeCharIndex = 0;
        }

        private void StartShuffleGems()
        {
            formattedGems = $"{gemAmount}";
            displayGems = new char[formattedGems.Length];

            for (int i = 0; i < displayGems.Length; i++)
            {
                if (char.IsDigit(formattedGems[i]) || formattedGems[i] == ' ')
                {
                    displayGems[i] = GetRandomChar();
                }
                else
                {
                    displayGems[i] = formattedGems[i];
                }
            }

            shuffleGemsComplete = false;
            currentGemsCharIndex = 0;
        }

        private char GetRandomChar()
        {
            int index = Globals.random.Next(charPool.Length);
            return charPool[index];
        }

        private void OnMenuClick()
        {
            sceneManager.AddScene(new MenuScene(contentManager,sceneManager),new CircleTransition(Globals.graphicsDevice));
        }

        private void OnRestartClick()
        {
            switch (gameData.stage)
            {
                case 1:
                    sceneManager.AddScene(new Stage1(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
                    break;
                case 2:
                    sceneManager.AddScene(new Stage2(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
                    break;
                case 3:
                    sceneManager.AddScene(new Stage3(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
                    break;
            }
        }
    }

}
