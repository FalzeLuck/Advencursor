using Advencursor._Animation;
using Advencursor._SaveData;
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
        private float gemAmount;
        private float time;
        private string formattedText;
        private char[] displayText;
        private SpriteFont font;
        private Animation summaryScreen;

        private Random random;
        private float shuffleTimer = 0f;
        private float shuffleInterval = 0.00001f;
        private int shuffleMax = 4;
        private int shuffleIndex = 0;
        private int currentCharIndex = 0;
        private string charPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:.";
        private bool shuffleComplete = false;

        public SummaryScene(ContentManager contentManager, SceneManager sceneManager, bool win, float gemAmount, GameData gameData, float time)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();
            this.win = win;
            this.gemAmount = gemAmount;
            this.time = time;

            random = new Random();

            switch (gameData.stage)
            {
                case 1:
                    bg = Globals.Content.Load<Texture2D>("Background/BG_Stage1");
                    break;
                case 2:
                    bg = Globals.Content.Load<Texture2D>("Background/BG_Stage2");
                    break;
            }
        }

        public void Load()
        {
            font = Globals.Content.Load<SpriteFont>("Font/TextFont");
            if (win)
            {
                summaryScreen = new Animation(Globals.Content.Load<Texture2D>("UI/Summary/Win"), 1, 2, 2, true);
            }
            else
            {
                summaryScreen = new Animation(Globals.Content.Load<Texture2D>("UI/Summary/Lose"), 1, 7, 8, false);
            }

            FormatTime();
        }

        public void Update(GameTime gameTime)
        {
            if (win)
            {
                summaryScreen.Update();
            }

            if (!win)
            {
                if (summaryScreen.currentFrame != 6)
                {
                    summaryScreen.Update();
                }
            };

            if (!shuffleComplete)
            {
                shuffleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (shuffleTimer >= shuffleInterval && currentCharIndex < displayText.Length)
                {
                    shuffleTimer = 0f;

                    if (shuffleIndex >= shuffleMax)
                    {
                        shuffleIndex = 0;
                        displayText[currentCharIndex] = formattedText[currentCharIndex];
                        currentCharIndex++;
                    }
                    else if (displayText[currentCharIndex] != formattedText[currentCharIndex])
                    {
                        displayText[currentCharIndex] = GetRandomChar();
                        shuffleIndex++;
                    }
                    else
                    {
                        shuffleIndex = 0;
                        currentCharIndex++;
                    }

                    if (currentCharIndex >= displayText.Length)
                    {
                        shuffleComplete = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 screenCenter = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
            Texture2D black = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Black);
            spriteBatch.Draw(black, Vector2.Zero, Color.White);
            spriteBatch.Draw(bg, Vector2.Zero, Color.White * 0.2f);
            summaryScreen.Draw(screenCenter);
            spriteBatch.DrawString(font, new string(displayText), new Vector2(screenCenter.X - 270, screenCenter.Y - 230), Color.Black, 0, Vector2.Zero, 1.1f, SpriteEffects.None, 0);

            Globals.DrawCursor();
        }

        private void FormatTime()
        {
            formattedText = TimeSpan.FromSeconds(time).ToString(@"mm\:ss\.ff");
            displayText = new char[formattedText.Length];

            for (int i = 0; i < displayText.Length; i++)
            {
                if (char.IsDigit(formattedText[i]) || formattedText[i] == ':' || formattedText[i] == '.')
                {
                    displayText[i] = GetRandomChar();
                }
                else
                {
                    displayText[i] = formattedText[i];
                }
            }
        }
        private char GetRandomChar()
        {
            int index = random.Next(charPool.Length);
            return charPool[index];
        }
    }
}
