using Advencursor._Models;
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
using Advencursor._Skill;
using Advencursor._SaveData;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Advencursor._Managers;
using Advencursor._Combat;
using static System.Formats.Asn1.AsnWriter;

namespace Advencursor._Scene
{
    public class GachaScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private Texture2D gachaDim;
        private Texture2D itemBackground;

        private Texture2D background;

        private Inventory inventory;
        private SpriteFont textFont;
        private GameData gameData;
        private int pityUltimateSkillThreshold = 70;

        private List<Skill> skillPool;

        private Dictionary<int, float> pullRates = new Dictionary<int, float>()
        {
            {4, 0.04f }, //R
            {3, 0.32f}, //E
            {2, 0.32f}, //W
            {1, 0.32f} //Q
        };
        private string selectedSet;

        //Pull Animation
        private float hue;
        private Vector2 gachacenter;
        private bool gachaStart;
        private float fadeTimeMax = 0.5f;
        private float fadeTime;
        private float fadeFactor;
        private float fadeOpacity;
        private Texture2D blackScreenTexture;
        private float gachaInterval = 2f;
        private float gachaWaitTime;
        private float gachaRevealOpacity;
        private List<Item> tempGachaItem;

        private Texture2D gemBG;

        public GachaScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();
            inventory = new Inventory();
            gameData = new GameData();
            tempGachaItem = new List<Item>();
            skillPool = AllSkills.allSkills.Values.ToList();
            gachaWaitTime = 0;
            gachacenter = new Vector2(Globals.Bounds.X/2, Globals.Bounds.Y/2);
        }

        public void Load()
        {
            blackScreenTexture = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Black);

            UIButton roll1Button = new(Globals.Content.Load<Texture2D>("UI/Gacha/Roll1"), new Vector2(Globals.Bounds.X / 2 - 100, Globals.Bounds.Y / 2 + 400), OnRoll1ButtonClick);
            UIButton roll5Button = new(Globals.Content.Load<Texture2D>("UI/Gacha/Roll5"), new Vector2(Globals.Bounds.X - 500, Globals.Bounds.Y / 2 + 400), OnRoll5ButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("UI/Gacha/ButtonExit"), new Vector2(120, 75), OnExitButtonClick);
            UIButton thunderButton = new(Globals.Content.Load<Texture2D>("UI/Gacha/ButtonThunder"), new Vector2(220, 250), OnThunderButtonClick);
            UIButton buffButton = new(Globals.Content.Load<Texture2D>("UI/Gacha/ButtonBuff"), new Vector2(220, 250 + 300), OnBuffButtonClick);
            UIButton fireButton = new(Globals.Content.Load<Texture2D>("UI/Gacha/ButtonFire"), new Vector2(220, 250 + 600), OnFireButtonClick);
            uiManager.AddElement("roll1Button", roll1Button);
            uiManager.AddElement("roll5Button", roll5Button);
            uiManager.AddElement("exitButton", exitButton);
            uiManager.AddElement("thunderButton", thunderButton);
            uiManager.AddElement("buffButton", buffButton);
            uiManager.AddElement("fireButton", fireButton);

            gachaDim = Globals.Content.Load<Texture2D>("UI/Gacha/GachaDim");
            itemBackground = Globals.Content.Load<Texture2D>("UI/Gacha/GachaItemBG");

            background = Globals.Content.Load<Texture2D>("UI/Gacha/BannerThunder");
            textFont = Globals.Content.Load<SpriteFont>("Font/TextFont");
            gemBG = Globals.Content.Load<Texture2D>("UI/Gacha/GemAmount");

            inventory.LoadInventory(new(Globals.graphicsDevice, 1, 1));
            gameData.LoadData();
            selectedSet = gameData.gachaSelectedSet;
            selectedSet = "Thunder";
        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = false;
            uiManager.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)
                && Keyboard.GetState().IsKeyDown(Keys.Q)
                && Keyboard.GetState().IsKeyDown(Keys.W)
                && Keyboard.GetState().IsKeyDown(Keys.E)
                && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                gameData.gems = 999999;
            }
            if (gachaStart)
            {
                gachaWaitTime -= TimeManager.TotalSeconds;
                hue = (float)(gameTime.TotalGameTime.TotalSeconds % 1.0);
                uiManager.HideAll();
                if (gachaWaitTime <= 1)
                {
                    float normalizedValue = gachaWaitTime / 1f;
                    normalizedValue = Math.Clamp(normalizedValue, 0, 1);
                    gachaRevealOpacity = normalizedValue;
                }
                if (InputManager.MouseLeftClicked && gachaWaitTime <= 0)
                {
                    gachaWaitTime = 0.1f;
                    gachaStart = false;
                }
            }
            else
            {
                gachaWaitTime -= TimeManager.TotalSeconds;
                if(gachaWaitTime <= 0)
                    uiManager.ShowAll();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            uiManager.Draw(spriteBatch);
            Globals.SpriteBatch.Draw(gemBG, new Vector2(Globals.Bounds.X - 400, 150), Color.White);
            Vector2 textSize = textFont.MeasureString($"{gameData.gems}");
            float rightAlignedX = (Globals.Bounds.X - 200) - (textSize.X * 0.35f);
            Globals.SpriteBatch.DrawString(textFont, $"{gameData.gems}", new Vector2(rightAlignedX, 170), Color.Black, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);

            if (gachaStart)
            {
                if (fadeTime > 0)
                {
                    fadeTime -= TimeManager.TotalSeconds;
                    float normalizedValue = fadeTime / fadeTimeMax;
                    normalizedValue = Math.Clamp(normalizedValue, 0, 1);
                    fadeOpacity = 1.0f - normalizedValue;
                }
                else
                {
                    fadeOpacity -= 1 * TimeManager.TotalSeconds;
                    Vector2 objectorigin = new Vector2(gachaDim.Width/2, gachaDim.Height/2);
                    Globals.SpriteBatch.Draw(gachaDim,gachacenter,null,Color.White,0f,objectorigin,1f,SpriteEffects.None,0f);
                    objectorigin = new Vector2(itemBackground.Width / 2, itemBackground.Height / 2);
                    

                    
                    Color tempcol = Color.Black;
                    if (tempGachaItem.Count == 1)
                    {
                        Vector2 itemOrigin = new Vector2(tempGachaItem[0].texture.Width / 2, tempGachaItem[0].texture.Height / 2);
                        Vector2 itemPos = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
                        Globals.SpriteBatch.Draw(itemBackground, itemPos, null, Color.White, 0f, objectorigin, 1.2f, SpriteEffects.None, 0f);
                        if (tempGachaItem[0].skill.rarity == 4)
                        {

                            Color rainbowColor = HSVtoRGB(hue, 1f, 1f);
                            Globals.SpriteBatch.Draw(tempGachaItem[0].texture, itemPos, null, Color.White, 0, itemOrigin, 1f, SpriteEffects.None, 0f);
                            Globals.SpriteBatch.Draw(tempGachaItem[0].texture, itemPos, null, rainbowColor * 0.2f, 0, itemOrigin, 1f, SpriteEffects.None, 0f);
                            Globals.SpriteBatch.Draw(tempGachaItem[0].texture, itemPos, null, tempcol * gachaRevealOpacity, 0, itemOrigin, 1f, SpriteEffects.None, 0f);
                        }
                        else
                        {
                            tempcol = Color.Black;
                            Globals.SpriteBatch.Draw(tempGachaItem[0].texture, itemPos, null, Color.White, 0, itemOrigin, 1f, SpriteEffects.None, 0f);
                            Globals.SpriteBatch.Draw(tempGachaItem[0].texture, itemPos, null, tempcol * gachaRevealOpacity, 0, itemOrigin, 1f, SpriteEffects.None, 0f);
                        }

                        string name = $"{(char)34}{tempGachaItem[0].name}{(char)34}";
                        Vector2 namePos = itemPos - new Vector2(0, 200);
                        Vector2 nameSize = textFont.MeasureString(name);
                        Vector2 nameOrigin = new Vector2(nameSize.X / 2, nameSize.Y / 2);
                        Globals.SpriteBatch.DrawString(textFont, name, namePos, Color.Black, 0f, nameOrigin, 0.2f, SpriteEffects.None, 0f);

                        string stat = $"{(char)34}{tempGachaItem[0].statDesc}:{tempGachaItem[0].statValue.ToString("F2")}{(char)34}";
                        Vector2 statPos = itemPos + new Vector2(0, 200);
                        Vector2 statSize = textFont.MeasureString(stat);
                        Vector2 statOrigin = new Vector2(statSize.X / 2, statSize.Y / 2);
                        Globals.SpriteBatch.DrawString(textFont, stat, statPos, Color.Black, 0f, statOrigin, 0.2f, SpriteEffects.None, 0f);
                    }
                    else if (tempGachaItem.Count > 1)
                    {
                        float scale = 0.7f;
                        for (int i = 0; i < tempGachaItem.Count; i++)
                        {
                            Vector2 itemOrigin = new Vector2(tempGachaItem[i].texture.Width / 2, tempGachaItem[i].texture.Height / 2);
                            Vector2 itemPos = new Vector2((Globals.Bounds.X/2) - 300, 0);
                            int horizontalDistance = 300;
                            int verticalDistance = 200;
                            if(i < tempGachaItem.Count / 2)
                            {
                                itemPos = new Vector2(itemPos.X + (horizontalDistance * i), (Globals.Bounds.Y/2) - verticalDistance);
                            }
                            else
                            {
                                itemPos = new Vector2(itemPos.X + (horizontalDistance * (i-(tempGachaItem.Count / 2))), (Globals.Bounds.Y / 2) + verticalDistance);
                            }
                            Globals.SpriteBatch.Draw(itemBackground, itemPos, null, Color.White, 0f, objectorigin, scale + 0.2f, SpriteEffects.None, 0f);
                            if (tempGachaItem[i].skill.rarity == 4)
                            {
                                
                                Color rainbowColor = HSVtoRGB(hue, 1f, 1f);
                                Globals.SpriteBatch.Draw(tempGachaItem[i].texture, itemPos, null, Color.White, 0, itemOrigin, scale, SpriteEffects.None, 0f);
                                Globals.SpriteBatch.Draw(tempGachaItem[i].texture, itemPos, null, rainbowColor * 0.2f, 0, itemOrigin, scale, SpriteEffects.None, 0f);
                                Globals.SpriteBatch.Draw(tempGachaItem[i].texture, itemPos, null, tempcol * gachaRevealOpacity, 0, itemOrigin, scale, SpriteEffects.None, 0f);
                            }
                            else
                            {
                                tempcol = Color.Black;
                                Globals.SpriteBatch.Draw(tempGachaItem[i].texture, itemPos, null, Color.White, 0, itemOrigin, scale, SpriteEffects.None, 0f);
                                Globals.SpriteBatch.Draw(tempGachaItem[i].texture, itemPos, null, tempcol * gachaRevealOpacity, 0, itemOrigin, scale, SpriteEffects.None, 0f);
                            }
                            

                            string name = $"{(char)34}{tempGachaItem[i].name}{(char)34}";
                            Vector2 namePos = itemPos - new Vector2(0, 110);
                            Vector2 nameSize = textFont.MeasureString(name);
                            Vector2 nameOrigin = new Vector2(nameSize.X / 2, nameSize.Y / 2);
                            Globals.SpriteBatch.DrawString(textFont, name, namePos, Color.Black, 0f, nameOrigin, scale - 0.5f, SpriteEffects.None, 0f);

                            string stat = $"{(char)34}{tempGachaItem[i].statDesc}:{tempGachaItem[i].statValue.ToString("F2")}{(char)34}";
                            Vector2 statPos = itemPos + new Vector2(0, 110);
                            Vector2 statSize = textFont.MeasureString(stat);
                            Vector2 statOrigin = new Vector2(statSize.X / 2, statSize.Y / 2);
                            Globals.SpriteBatch.DrawString(textFont, stat, statPos, Color.Black, 0f, statOrigin, scale - 0.5f, SpriteEffects.None, 0f);
                        }
                    }
                }
                Globals.SpriteBatch.Draw(blackScreenTexture,Vector2.Zero,Color.White * fadeOpacity);
                if(gachaWaitTime <= 0) Globals.SpriteBatch.DrawString(textFont, "Click Anywhere to Close", new Vector2(Globals.Bounds.X/2 - 210,900), Color.Black, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            }
            //Globals.SpriteBatch.DrawString(textFont, gameData.pityCounter.ToString(), Vector2.Zero, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Globals.DrawCursor();
        }

        private void OnRoll1ButtonClick()
        {
            if (gameData.gems - 10 < 0) return;

            gameData.gems -= 10;
            Item tempItem = RandomItemsWithPity(skillPool, pullRates, selectedSet);


            tempGachaItem.Clear();
            tempGachaItem.Add(tempItem);
            inventory.Items.Add(tempItem);
            gameData.SaveData();
            inventory.SaveInventory();
            StartPull();
        }

        private void OnRoll5ButtonClick()
        {
            int rollNumber = 6;
            if (gameData.gems - 10 * (rollNumber - 1) < 0) return;

            gameData.gems -= 10 * (rollNumber - 1);
            List<Item> results = new List<Item>();
            for (int i = 0; i < rollNumber; i++)
            {
                results.Add(RandomItemsWithPity(skillPool, pullRates, selectedSet));
            }

            tempGachaItem.Clear();
            tempGachaItem.AddRange(results);
            inventory.Items.AddRange(tempGachaItem);
            gameData.SaveData();
            inventory.SaveInventory();
            StartPull();
        }
        private void StartPull()
        {
            fadeOpacity = 0;
            fadeTime = fadeTimeMax;
            gachaStart = true;
            gachaWaitTime = 3;
            gachaRevealOpacity = 1f;
        }

        private void OnThunderButtonClick()
        {
            selectedSet = "Thunder";
            background = Globals.Content.Load<Texture2D>("UI/Gacha/BannerThunder");
        }

        private void OnBuffButtonClick()
        {
            selectedSet = "Food";
            background = Globals.Content.Load<Texture2D>("UI/Gacha/BannerBuff");
        }
        private void OnFireButtonClick()
        {
            selectedSet = "Fire";
            background = Globals.Content.Load<Texture2D>("UI/Gacha/BannerFire");
        }

        private Item RandomItems(List<Skill> skillPool, Dictionary<int, float> rates, string set)
        {
            double roll = Globals.random.NextDouble();
            double cumulative = 0.0;

            foreach (var rate in rates)
            {
                cumulative += rate.Value;
                if (roll < cumulative)
                {
                    var filteredItems = skillPool.Where(skill => skill.rarity == rate.Key && skill.setSkill == set).ToList();
                    Skill tempSkill = filteredItems[Globals.random.Next(filteredItems.Count)];
                    string tempName = AllSkills.itemNameViaSkillName[tempSkill.name];
                    Keys tempKey = Keys.None;
                    if (rate.Key == 4)
                    {
                        tempKey = Keys.R;
                        gameData.pityCounter = 0;
                    }
                    else if (rate.Key == 3)
                    {
                        tempKey = Keys.E;
                    }
                    else if (rate.Key == 2)
                    {
                        tempKey = Keys.W;
                    }
                    else if (rate.Key == 1)
                    {
                        tempKey = Keys.Q;
                    }

                    return new Item(tempName, tempSkill, tempKey);
                }
            }

            return null;
        }

        private Item RandomItemsWithPity(List<Skill> skillPool, Dictionary<int, float> rates, string set)
        {
            gameData.pityCounter++;

            if (gameData.pityCounter >= pityUltimateSkillThreshold)
            {
                gameData.pityCounter = 0;
                var guaranteedItem = skillPool.Where(skill => skill.rarity == 4 && skill.setSkill == set).ToList();
                Skill tempSkill = guaranteedItem[Globals.random.Next(guaranteedItem.Count)];
                string tempName = AllSkills.itemNameViaSkillName[tempSkill.name];
                Keys tempKey = Keys.R;
                return new Item(tempName, tempSkill, tempKey);
            }

            return RandomItems(skillPool, rates, set);
        }

        private void OnExitButtonClick()
        {
            sceneManager.AddScene(new StageSelectScene(contentManager, sceneManager));
        }

        public Color HSVtoRGB(float h, float s, float v)
        {
            float r = 0, g = 0, b = 0;
            int i = (int)Math.Floor(h * 6);
            float f = h * 6 - i;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            switch (i % 6)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
            }

            return new Color(r, g, b);
        }
    }
}
