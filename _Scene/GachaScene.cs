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

namespace Advencursor._Scene
{
    public class GachaScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

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

        private float gachaInterval = 1f;
        private float gachaWaitTime;
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
        }

        public void Load()
        {
            UIButton roll1Button = new(Globals.Content.Load<Texture2D>("UI/Gacha/Roll1"), new Vector2(Globals.Bounds.X / 2 - 100, Globals.Bounds.Y / 2 + 400), OnRoll1ButtonClick);
            UIButton roll5Button = new(Globals.Content.Load<Texture2D>("UI/Gacha/Roll5"), new Vector2(Globals.Bounds.X - 500, Globals.Bounds.Y / 2 + 400), OnRoll5ButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("UI/Gacha/ButtonExit"), new Vector2(Globals.Bounds.X - 110, 75), OnExitButtonClick);
            UIButton thunderButton = new(Globals.Content.Load<Texture2D>("UI/Gacha/ButtonThunder"), new Vector2(220, 250), OnThunderButtonClick);
            UIButton buffButton = new(Globals.Content.Load<Texture2D>("UI/Gacha/ButtonBuff"), new Vector2(220, 250 + 350), OnBuffButtonClick);
            uiManager.AddElement("roll1Button", roll1Button);
            uiManager.AddElement("roll5Button", roll5Button);
            uiManager.AddElement("exitButton", exitButton);
            uiManager.AddElement("thunderButton", thunderButton);
            uiManager.AddElement("buffButton", buffButton);


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

            if (gachaWaitTime > 0)
            {
                uiManager.HideAll();
                gachaWaitTime -= TimeManager.TotalSeconds;
            }
            else
            {
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
            Globals.SpriteBatch.DrawString(textFont, $"{gameData.gems}", new Vector2(rightAlignedX, 170), Color.Black, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f
            );

            if (gachaWaitTime > 0)
            {
                if (tempGachaItem.Count == 1)
                {
                    Vector2 itemOrigin = new Vector2(tempGachaItem[0].texture.Width / 2, tempGachaItem[0].texture.Height / 2);
                    Vector2 itemPos = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
                    Globals.SpriteBatch.Draw(tempGachaItem[0].texture, itemPos, null, Color.White, 0, itemOrigin, 1f, SpriteEffects.None, 0f);

                    string name = $"{(char)34}{tempGachaItem[0].name}{(char)34}";
                    Vector2 namePos = itemPos + new Vector2(0, 200);
                    Vector2 nameSize = textFont.MeasureString(name);
                    Vector2 nameOrigin = new Vector2(nameSize.X / 2, nameSize.Y / 2);
                    Globals.SpriteBatch.DrawString(textFont, name, namePos, Color.Black, 0f, nameOrigin, 1f, SpriteEffects.None, 0f);

                    string stat = $"{(char)34}{tempGachaItem[0].statDesc}:{tempGachaItem[0].statValue.ToString("F2")}{(char)34}";
                    Vector2 statPos = namePos + new Vector2(0, 100);
                    Vector2 statSize = textFont.MeasureString(stat);
                    Vector2 statOrigin = new Vector2(statSize.X / 2, statSize.Y / 2);
                    Globals.SpriteBatch.DrawString(textFont, stat, statPos, Color.Black, 0f, statOrigin, 1f, SpriteEffects.None, 0f);
                }
                else if (tempGachaItem.Count > 1)
                {
                    float scale = 0.5f;
                    for (int i = 0; i < tempGachaItem.Count; i++)
                    {
                        Vector2 itemOrigin = new Vector2(tempGachaItem[i].texture.Width / 2, tempGachaItem[i].texture.Height / 2);
                        Vector2 itemPos = new Vector2((Globals.Bounds.X / tempGachaItem.Count) + (i * tempGachaItem[i].texture.Width), Globals.Bounds.Y / 2);
                        Globals.SpriteBatch.Draw(tempGachaItem[i].texture, itemPos, null, Color.White, 0, itemOrigin, scale, SpriteEffects.None, 0f);

                        string name = $"{(char)34}{tempGachaItem[i].name}{(char)34}";
                        Vector2 namePos = itemPos + new Vector2(0, 200);
                        Vector2 nameSize = textFont.MeasureString(name);
                        Vector2 nameOrigin = new Vector2(nameSize.X / 2, nameSize.Y / 2);
                        Globals.SpriteBatch.DrawString(textFont, name, namePos, Color.Black, 0f, nameOrigin, scale, SpriteEffects.None, 0f);

                        string stat = $"{(char)34}{tempGachaItem[i].statDesc}:{tempGachaItem[i].statValue.ToString("F2")}{(char)34}";
                        Vector2 statPos = namePos + new Vector2(0, 100);
                        Vector2 statSize = textFont.MeasureString(stat);
                        Vector2 statOrigin = new Vector2(statSize.X / 2, statSize.Y / 2);
                        Globals.SpriteBatch.DrawString(textFont, stat, statPos, Color.Black, 0f, statOrigin, scale, SpriteEffects.None, 0f);

                    }
                }
            }
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
            gachaWaitTime = gachaInterval;
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
            gachaWaitTime = gachaInterval;
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
    }
}
