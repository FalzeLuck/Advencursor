using Advencursor._Combat;
using Advencursor._Models;
using Advencursor._SaveData;
using Advencursor._Scene.Stage;
using Advencursor._Scene.Transition;
using Advencursor._Skill;
using Advencursor._Skill.Thunder_Set;
using Advencursor._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Advencursor._Scene
{
    public class InventoryScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private GameData gameData;

        private Inventory inventory = new Inventory();
        private string pathinventory = "inventory.json";
        private Dictionary<Keys,Item> equippedItems = new Dictionary<Keys,Item>();

        private Texture2D backgroundInventory;
        private Texture2D gridTexture;
        private Texture2D gridTextureSelected;
        private int gridColumns = 5;
        private int gridRows = 4;
        private int totalVisibleItems;
        private float scrollOffset = 0f;
        private int currentScrollIndex = 0;
        private int selectedItemIndex;
        private float scrollStep = 150f;

        private Vector2 gridStartPos = new(Globals.Bounds.X/2,50);
        private int itemSize = 150;

        //Scrollbar
        private int scrollbarHeight = 300;
        private int scrollbarWidth = 20;
        private Vector2 scrollbarPosition;
        private float scrollbarThumbHeight = 50;
        private float scrollbarThumbPosition = 0f;
        private bool isDraggingThumb = false;
        private int totalItems;
        private int totalPages;
        private float scrollbarTrackHeight;
        private int previousScrollValue = 0;

        //Mouse Cot=ntrol
        private Rectangle mouseCollision;

        //Player Save and Load
        private Player player;
        private string pathplayer = "playerdata.json"; 

        public InventoryScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();
            gameData = new GameData();

            totalVisibleItems = gridColumns * gridRows;
            scrollbarPosition = new Vector2((itemSize*gridColumns) + gridStartPos.X, gridStartPos.Y);
        }

        public void Load()
        {
            Texture2D tempTexture = Globals.Content.Load<Texture2D>("UI/SkillUI");
            gameData.LoadData();

            player = new(Globals.Content.Load<Texture2D>("playerTexture"), Vector2.Zero, 15000, 800, 0, 0);
            player.LoadPlayer(4,1);
            
            

            UIButton equipButton = new(Globals.Content.Load<Texture2D>("Button/EquipButton"), new Vector2(Globals.Bounds.X/2 - 400, 400 + Globals.Bounds.Y / 2), OnEquipButtonClick);
            UIButton playButton = new(Globals.Content.Load<Texture2D>("Button/playButton"), new Vector2(Globals.Bounds.X / 2 + 400, 400 + Globals.Bounds.Y / 2), OnPlayButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("Button/exitButton"), new Vector2(0, 0), OnExitButtonClick);
            uiManager.AddElement("equipButton",equipButton);
            uiManager.AddElement("exitButton",exitButton);
            uiManager.AddElement("playButton",playButton);

            backgroundInventory = Globals.Content.Load<Texture2D>("Background/Stage1_1");
            gridTexture = Globals.Content.Load<Texture2D>("Item/Grid");
            gridTextureSelected = Globals.Content.Load<Texture2D>("Item/GridSelected");



            Skill ThunderCore = AllSkills.allSkills["Thunder Core"];
            Skill ThunderShuriken = AllSkills.allSkills["Thunder Shuriken"];
            Skill ThunderSpeed = AllSkills.allSkills["Thunder Speed"];
            Skill ThunderStorm = AllSkills.allSkills["I am the Storm"];
            Skill FoodTrap = AllSkills.allSkills["Food Trap"];
            Skill PoisonTrap = AllSkills.allSkills["Poison Trap"];
            Skill EmergencyFood = AllSkills.allSkills["Emergency Food"];
            Skill Invincibility = AllSkills.allSkills["Invincibility"];
            Skill nullSkill = AllSkills.allSkills["null"];


            inventory.Items.Add(new Item("ThunderCore book", ThunderCore, Keys.Q));
            inventory.Items.Add(new Item("ThunderShuriken book", ThunderShuriken, Keys.W));
            inventory.Items.Add(new Item("ThunderSpeed book", ThunderSpeed, Keys.E));
            inventory.Items.Add(new Item("ThunderStorm book", ThunderStorm, Keys.R));
            inventory.Items.Add(new Item("Food Trap Book",FoodTrap,Keys.Q));
            inventory.Items.Add(new Item("Poison Trap Book", PoisonTrap, Keys.W));
            inventory.Items.Add(new Item("Emergency Food Book", EmergencyFood, Keys.E));
            inventory.Items.Add(new Item("Invinc Book", Invincibility, Keys.R));

            Texture2D nullTexture = new(Globals.graphicsDevice, 1, 1);
            Item nullItem = new Item("null book", nullSkill, Keys.None);
            for (int i = 0; i < totalVisibleItems*2 ; i++)
            {
                inventory.Items.Add(nullItem);
            }

            CheatInventory();

            inventory.SaveInventory(pathinventory);



            totalItems = inventory.Items.Count;
            totalPages = (totalItems + gridColumns - 1) / gridColumns;
            scrollbarTrackHeight = scrollbarHeight - scrollbarThumbHeight;

        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = true;
            uiManager.Update(gameTime);

            int currentScrollValue = Mouse.GetState().ScrollWheelValue;
            var scrollDelta = currentScrollValue - previousScrollValue;
            previousScrollValue = currentScrollValue;

            if (scrollDelta > 0 && currentScrollIndex > 0)
            {
                scrollOffset += scrollStep; //Up
            }
            else if (scrollDelta < 0 && currentScrollIndex < inventory.Items.Count / gridColumns)
            {
                scrollOffset -= scrollStep; //Down
            }

            if (scrollOffset >= itemSize)
            {
                if (currentScrollIndex > 0)
                {
                    currentScrollIndex-=gridColumns;
                    scrollOffset = 0;
                }
            }
            else if (scrollOffset <= -itemSize)
            {
                if (currentScrollIndex < inventory.Items.Count - totalVisibleItems)
                {
                    currentScrollIndex+=gridColumns;
                    scrollOffset = 0;
                }
            }

            scrollbarThumbPosition = (float)currentScrollIndex / totalPages * scrollbarTrackHeight;
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (mousePosition.X >= scrollbarPosition.X && mousePosition.X <= scrollbarPosition.X + scrollbarWidth &&
                    mousePosition.Y >= scrollbarPosition.Y + scrollbarThumbPosition &&
                    mousePosition.Y <= scrollbarPosition.Y + scrollbarThumbPosition + scrollbarThumbHeight)
                {
                    isDraggingThumb = true;
                }

                if (isDraggingThumb && currentScrollIndex >= 0 && currentScrollIndex < inventory.Items.Count - totalVisibleItems)
                {
                    float newThumbPosition = mousePosition.Y - scrollbarPosition.Y;
                    scrollbarThumbPosition = MathHelper.Clamp(newThumbPosition, 0, scrollbarTrackHeight);

                    int tempIndex;
                    tempIndex = (int)(scrollbarThumbPosition / scrollbarTrackHeight * totalPages);

                    if (tempIndex % 5 == 0 && scrollbarThumbPosition + scrollbarThumbHeight >= scrollbarTrackHeight)
                    {
                        currentScrollIndex = tempIndex + gridColumns;
                    }
                    else if (tempIndex % 5 == 0)
                    {
                        currentScrollIndex = tempIndex;
                    }
                   

                }
            }
            else
            {
                isDraggingThumb = false;
            }

            mouseCollision = new((int)mousePosition.X, (int)mousePosition.Y, 1, 1);

            SelectedItem();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(backgroundInventory, Vector2.Zero, Color.White);
            uiManager.Draw(spriteBatch);

            float itemScale = 0.5f;

            for (int row = 0; row < gridRows; row++)
            {
                for (int col = 0; col < gridColumns; col++)
                {
                    int itemIndex = currentScrollIndex + row * gridColumns + col;

                    if (itemIndex < inventory.Items.Count)
                    {
                        Vector2 position = new Vector2(gridStartPos.X + col * itemSize, gridStartPos.Y + row * itemSize + scrollOffset);
                        Rectangle itemCollide = new((int)position.X,(int)position.Y, itemSize,itemSize);

                        if (position.Y >= gridStartPos.Y - itemSize && position.Y < (gridStartPos.Y + gridRows * itemSize) + itemSize)
                        {
                            if(itemIndex == selectedItemIndex)
                            {
                                if (itemCollide.Intersects(mouseCollision))
                                {
                                    spriteBatch.Draw(gridTextureSelected, position, Color.Yellow);
                                    spriteBatch.Draw(inventory.Items[selectedItemIndex].texture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
                                }
                                else
                                {
                                    spriteBatch.Draw(gridTextureSelected, position, Color.White);
                                    spriteBatch.Draw(inventory.Items[selectedItemIndex].texture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
                                }
                            }
                            else
                            {
                                if (itemCollide.Intersects(mouseCollision))
                                {
                                    spriteBatch.Draw(gridTexture, position, Color.Yellow);
                                    spriteBatch.Draw(inventory.Items[itemIndex].texture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
                                }
                                else
                                {
                                    spriteBatch.Draw(gridTexture, position, Color.White);
                                    spriteBatch.Draw(inventory.Items[itemIndex].texture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
                                }
                            }
                        }
                    }
                }
            }

            Texture2D scrollbar = Globals.Content.Load<Texture2D>("Item/scrollBarTexture");
            Texture2D thumb = Globals.Content.Load<Texture2D>("Item/scrollBarThumb");
            DrawScrollbar(spriteBatch,scrollbar,thumb);

            for (int i = 0; i < 4; i++)
            {
                Vector2 position = new Vector2(50, gridStartPos.Y + (i * itemSize) + (i * 50));
                Keys keyIndex = new Keys();
                if(i == 0) { keyIndex = Keys.Q; }
                else if(i == 1) { keyIndex = Keys.W; }
                else if (i == 2) { keyIndex = Keys.E; }
                else if (i == 3) { keyIndex = Keys.R; }
                Texture2D equipItemTexture = AllSkills.allSkillTextures[player.Skills[keyIndex].name];

                spriteBatch.Draw(gridTexture, position, Color.White);
                spriteBatch.Draw(equipItemTexture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
            }

            DrawCurrentItem();
        }

        private void DrawScrollbar(SpriteBatch spriteBatch, Texture2D scrollbarTexture, Texture2D thumbTexture)
        {
            spriteBatch.Draw(scrollbarTexture,new Rectangle((int)scrollbarPosition.X, (int)scrollbarPosition.Y, scrollbarWidth, scrollbarHeight),Color.Gray);

            spriteBatch.Draw(thumbTexture,new Rectangle((int)scrollbarPosition.X, (int)(scrollbarPosition.Y + scrollbarThumbPosition), scrollbarWidth, (int)scrollbarThumbHeight),Color.White);
        }

        private void DrawCurrentItem()
        {
            //Draw Big Item
            Vector2 bigItemOrigin = new(inventory.Items[selectedItemIndex].texture.Width / 2, inventory.Items[selectedItemIndex].texture.Height / 2);
            Vector2 bigItemPosition = new(Globals.Bounds.X / 4, Globals.Bounds.Y / 4);
            Globals.SpriteBatch.Draw(inventory.Items[selectedItemIndex].texture, bigItemPosition, null, Color.White, 0f, bigItemOrigin, 1f, SpriteEffects.None, 0f);

            SpriteFont spriteFont = Globals.Content.Load<SpriteFont>("basicFont");
            string mainStat = inventory.Items[selectedItemIndex].statValue.ToString("F2");
            string mainStatDesc = inventory.Items[selectedItemIndex].statDesc;
            string mainStatString;
            if (mainStatDesc != null)
            {
                if (mainStatDesc == "Health")
                {
                    mainStatString = $"{mainStatDesc} : {mainStat}";
                }
                else
                {
                    mainStatString = $"{mainStatDesc} : {mainStat}%";
                }
            }
            else
            {
                mainStatString = $"";
            }
            Vector2 mainStatSize = spriteFont.MeasureString(mainStatString);
            Vector2 mainStatOrigin = new(mainStatSize.X/2, mainStatSize.Y/2);
            Vector2 nameSize = spriteFont.MeasureString(inventory.Items[selectedItemIndex].name);
            Vector2 nameOrigin = new(nameSize.X / 2, nameSize.Y / 2);

            Globals.SpriteBatch.DrawString(spriteFont, inventory.Items[selectedItemIndex].name, new(bigItemPosition.X, bigItemPosition.Y + 200), Color.Black, 0, nameOrigin, 1, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(spriteFont,mainStatString,new(bigItemPosition.X,bigItemPosition.Y + 250),Color.Black,0,mainStatOrigin,1,SpriteEffects.None,0f);


            //Player Stat
            string Header = "Player Status";
            Vector2 HeaderSize = spriteFont.MeasureString(Header);
            Vector2 HeaderOrigin = new(HeaderSize.X / 2, HeaderSize.Y / 2);
            string HP = $"HP : {player.Status.MaxHP.ToString()}";
            string Attack = $"Attack = {player.Status.Attack.ToString()}";
            string CritRate = $"Critical Rate = {player.Status.CritRate.ToString("F2")}%";
            string CritDam = $"Critical Damage = {player.Status.CritDam.ToString("F2")}%";
            Globals.SpriteBatch.DrawString(spriteFont, Header, new(bigItemPosition.X, bigItemPosition.Y + 300), Color.Black, 0, HeaderOrigin, 1, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(spriteFont, HP, new(bigItemPosition.X - 150, bigItemPosition.Y + 350), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(spriteFont, Attack, new(bigItemPosition.X - 150, bigItemPosition.Y + 400), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(spriteFont, CritRate, new(bigItemPosition.X - 150, bigItemPosition.Y + 450), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(spriteFont, CritDam, new(bigItemPosition.X - 150, bigItemPosition.Y + 500), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

        }

        private void SelectedItem()
        {
            for (int row = 0; row < gridRows; row++)
            {
                for (int col = 0; col < gridColumns; col++)
                {
                    int itemIndex = currentScrollIndex + row * gridColumns + col;

                    if (itemIndex < inventory.Items.Count)
                    {
                        Vector2 position = new Vector2(gridStartPos.X + col * itemSize, gridStartPos.Y + row * itemSize + scrollOffset);
                        Rectangle itemCollide = new((int)position.X, (int)position.Y, itemSize, itemSize);

                        if (position.Y >= gridStartPos.Y - itemSize && position.Y < (gridStartPos.Y + gridRows * itemSize) + itemSize)
                        {
                            if (itemCollide.Intersects(mouseCollision) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                            {
                                selectedItemIndex = itemIndex;
                            }
                        }
                    }
                }
            }
        }


        private void OnEquipButtonClick()
        {
            player.EquipItem(inventory.Items[selectedItemIndex]);
            Trace.WriteLine($"HP : {player.Status.MaxHP} Attack = {player.Status.Attack} Rate = {player.Status.CritRate} Damage = {player.Status.CritDam}");

        }

        private void OnPlayButtonClick()
        {
            player.SavePlayer();

            if (gameData.stage == 1)
            {
                sceneManager.AddScene(new Stage1(contentManager, sceneManager),new CircleTransition(Globals.graphicsDevice));
            }
        }

        private void OnExitButtonClick()
        {
            sceneManager.AddScene(new StageSelectScene(contentManager, sceneManager));
        }

        private void CheatInventory()
        {
            Skill ThunderCore = AllSkills.allSkills["Thunder Core"];
            Skill ThunderShuriken = AllSkills.allSkills["Thunder Shuriken"];
            Skill ThunderSpeed = AllSkills.allSkills["Thunder Speed"];
            Skill ThunderStorm = AllSkills.allSkills["I am the Storm"];
            Skill nullSkill = AllSkills.allSkills["null"];
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                if(inventory.Items[i].name == "null book")
                {
                    inventory.Items[i] = new("Q_Cheat", ThunderCore, Keys.Q);
                    inventory.Items[i+1] = new("W_Cheat", ThunderShuriken, Keys.W);
                    inventory.Items[i+2] = new("E_Cheat", ThunderSpeed, Keys.E);
                    inventory.Items[i+3] = new("R_Cheat", ThunderStorm, Keys.R);
                    inventory.Items[i].GenerateStatCheat();
                    inventory.Items[i + 1].GenerateStatCheat();
                    inventory.Items[i + 2].GenerateStatCheat();
                    inventory.Items[i + 3].GenerateStatCheat();
                    inventory.Items.Add(new Item("null book", AllSkills.allSkills["null"],Keys.None));
                    inventory.Items.Add(new Item("null book", AllSkills.allSkills["null"], Keys.None));
                    inventory.Items.Add(new Item("null book", AllSkills.allSkills["null"], Keys.None));
                    inventory.Items.Add(new Item("null book", AllSkills.allSkills["null"], Keys.None));
                    break;
                }
            }
        }

    }
}
