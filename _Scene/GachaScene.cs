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
        private int pityUltimateSkillThreshold = 10;

        private List<Skill> skillPool;

        private Dictionary<int, float> pullRates = new Dictionary<int, float>()
        {
            {4, 0.01f }, //R
            {3, 0.33f}, //E
            {2, 0.33f}, //W
            {1, 0.33f} //Q
        };

        private float gachaInterval = 0.5f;
        private float gachaWaitTime;
        private List<Item> tempGachaItem;

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
            UIButton roll1Button = new(Globals.Content.Load<Texture2D>("Gacha/Roll1"), new Vector2(Globals.Bounds.X - 500, Globals.Bounds.Y / 2 + 300), OnRoll1ButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("Item/BackButton"), new Vector2(Globals.Bounds.X / 2 - 400, (Globals.Bounds.Y / 2) + 300), OnExitButtonClick);
            uiManager.AddElement("playButton", roll1Button);
            uiManager.AddElement("exitButton", exitButton);

            background = Globals.Content.Load<Texture2D>("Background/Menu");
            textFont = Globals.Content.Load<SpriteFont>("Font/TextFont");

            inventory.LoadInventory("inventory.json", new(Globals.graphicsDevice, 1, 1));
            gameData.LoadData();
        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = true;
            uiManager.Update(gameTime);

            if(Keyboard.GetState().IsKeyDown(Keys.LeftControl) 
                && Keyboard.GetState().IsKeyDown(Keys.Q)
                && Keyboard.GetState().IsKeyDown(Keys.W)
                && Keyboard.GetState().IsKeyDown(Keys.E)
                && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                gameData.gems = 999999;
            }

            if(gachaWaitTime > 0)
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
            //Globals.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            uiManager.Draw(spriteBatch);

            Globals.SpriteBatch.DrawString(textFont,$"Gems : {gameData.gems}",Vector2.Zero,Color.Black);

            if (gachaWaitTime > 0)
            {
                if (tempGachaItem.Count == 1)
                {
                    Vector2 itemOrigin = new Vector2(tempGachaItem[0].texture.Width/2, tempGachaItem[0].texture.Height/2);
                    Vector2 itemPos = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
                    Globals.SpriteBatch.Draw(tempGachaItem[0].texture,itemPos,null,Color.White,0,itemOrigin,1f,SpriteEffects.None,0f);

                    string name = $"{(char)34}{tempGachaItem[0].name}{(char)34}";
                    Vector2 namePos = itemPos + new Vector2(0, 200);
                    Vector2 nameSize = textFont.MeasureString(name);
                    Vector2 nameOrigin = new Vector2(nameSize.X/2, nameSize.Y/2);
                    Globals.SpriteBatch.DrawString(textFont,name,namePos,Color.Black,0f,nameOrigin,1f,SpriteEffects.None,0f);

                    string stat = $"{(char)34}{tempGachaItem[0].statDesc}:{tempGachaItem[0].statValue.ToString("F2")}{(char)34}";
                    Vector2 statPos = namePos + new Vector2(0, 100);
                    Vector2 statSize = textFont.MeasureString(stat);
                    Vector2 statOrigin = new Vector2(statSize.X / 2, statSize.Y / 2);
                    Globals.SpriteBatch.DrawString(textFont, stat, statPos, Color.Black, 0f, statOrigin, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        private void OnRoll1ButtonClick()
        {
            if (gameData.gems - 10 <= 0) return;

            gameData.gems -= 10;
            Item tempItem = RandomItemsWithPity(skillPool,pullRates);


            tempGachaItem.Clear();
            tempGachaItem.Add(tempItem);
            gachaWaitTime = gachaInterval;
        }

        private Item RandomItems(List<Skill> skillPool,Dictionary<int, float> rates)
        {
            double roll = Globals.random.NextDouble();
            double cumulative = 0.0;

            foreach (var rate in rates)
            {
                cumulative += rate.Value;
                if (roll < cumulative)
                {
                    var filteredItems = skillPool.Where(skill => skill.rarity == rate.Key).ToList();
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
                        tempKey = Keys.R;
                    }
                    else if (rate.Key == 2)
                    {
                        tempKey = Keys.W;
                    }
                    else
                    {
                        tempKey = Keys.Q;
                    }

                    return new Item(tempName, tempSkill,tempKey);
                }
            }

            return null;
        }

        private Item RandomItemsWithPity(List<Skill> skillPool,Dictionary<int, float> rates)
        {
            gameData.pityCounter++;

            if (gameData.pityCounter >= pityUltimateSkillThreshold)
            {
                gameData.pityCounter = 0;
                var guaranteedItem = skillPool.Where(skill => skill.rarity == 4).ToList();
                Skill tempSkill = guaranteedItem[Globals.random.Next(guaranteedItem.Count)];
                string tempName = AllSkills.itemNameViaSkillName[tempSkill.name];
                Keys tempKey = Keys.R;
                return new Item(tempName, tempSkill, tempKey);
            }

            return RandomItems(skillPool,rates);
        }

        private void OnExitButtonClick()
        {
            sceneManager.AddScene(new StageSelectScene(contentManager, sceneManager));
        }
    }
}
