using Advencursor._Models;
using Advencursor._SaveData;
using Advencursor._Scene.Stage;
using Advencursor._Scene.Transition;
using Advencursor._Skill;
using Advencursor._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

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
        private Dictionary<Keys, Item> equippedItems = new Dictionary<Keys, Item>();

        private Texture2D backgroundInventory;
        private Texture2D midgroundInventory;
        private Texture2D gridTexture;
        private Texture2D gridTextureSelected;
        private int gridColumns = 4;
        private int gridRows = 3;
        private int totalVisibleItems;
        private float scrollOffset = 0f;
        private int currentScrollIndex = 0;
        private int selectedItemIndex;
        private float scrollStep = 150f;

        SpriteFont textFont;
        SpriteFont textFontThai;

        private bool drawOnHover;

        private Vector2 gridStartPos = new((Globals.Bounds.X / 2) + 54, 255);
        private int itemSize = 170;

        //Scrollbar
        private int scrollbarHeight = 538;
        private int scrollbarWidth = 51;
        private Vector2 scrollbarPosition;
        private float scrollbarThumbHeight = 148;
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



            scrollbarPosition = new Vector2(gridStartPos.X + 770, gridStartPos.Y + 51);
        }

        public void Load()
        {
            Texture2D tempTexture = Globals.Content.Load<Texture2D>("UI/SkillUI");
            gameData.LoadData();
            inventory.LoadInventory(tempTexture);

            if (gameData.isFirstTime)
            {
                inventory.Items.Add(new(AllSkills.itemNameViaSkillName["Thunder Core"], AllSkills.allSkills["Thunder Core"], Keys.Q));
                gameData.isFirstTime = false;
                gameData.SaveData();
            }

            textFont = Globals.Content.Load<SpriteFont>("Font/TextFont");
            textFontThai = Globals.Content.Load<SpriteFont>("Font/TextFontThai");

            player = new(Globals.Content.Load<Texture2D>("playerTexture"), Vector2.Zero, 15000, 800, 0, 0);
            player.LoadPlayer(4, 1);



            UIButton equipButton = new(Globals.Content.Load<Texture2D>("Item/EquipButton"), new Vector2(800, Globals.Bounds.Y - 100), OnEquipButtonClick);
            UIButton statButton = new(Globals.Content.Load<Texture2D>("Item/StatusButton"), new Vector2(170, Globals.Bounds.Y - 105), OnStatusButtonHover, true);
            UIButton playButton = new(Globals.Content.Load<Texture2D>("Item/StartButton"), new Vector2(Globals.Bounds.X - 300, Globals.Bounds.Y - 115), OnPlayButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("Item/BackButton"), new Vector2(Globals.Bounds.X - 100, 65), OnExitButtonClick);
            uiManager.AddElement("equipButton", equipButton);
            uiManager.AddElement("statButton", statButton);
            uiManager.AddElement("exitButton", exitButton);
            uiManager.AddElement("playButton", playButton);

            uiManager.SetScale("statButton", 0.75f);

            backgroundInventory = Globals.Content.Load<Texture2D>("Item/Background2");
            midgroundInventory = Globals.Content.Load<Texture2D>("Item/Background1");

            gridTexture = Globals.Content.Load<Texture2D>("Item/Grid");
            gridTextureSelected = Globals.Content.Load<Texture2D>("Item/GridSelected");

            



            totalVisibleItems = gridColumns * gridRows;
            totalItems = inventory.Items.Count;
            totalPages = (int)Math.Ceiling((float)inventory.Items.Count / totalVisibleItems);
            scrollbarTrackHeight = scrollbarHeight - scrollbarThumbHeight;

            Texture2D nullTexture = new(Globals.graphicsDevice, 1, 1);
            Item nullItem = new Item("null book", AllSkills.allSkills["null"], Keys.None);
            for (int i = 0; i < totalVisibleItems * 2; i++)
            {
                inventory.Items.Add(nullItem);
            }


            inventory.SortItem();
        }

        public void Update(GameTime gameTime)
        {
            //Cheat Inventory
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)
                && Keyboard.GetState().IsKeyDown(Keys.Q)
                && Keyboard.GetState().IsKeyDown(Keys.W)
                && Keyboard.GetState().IsKeyDown(Keys.E)
                && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                CheatInventory();
            }

            Globals.Game.IsMouseVisible = false;
            uiManager.Update(gameTime);

            int currentScrollValue = Mouse.GetState().ScrollWheelValue;
            var scrollDelta = currentScrollValue - previousScrollValue;
            previousScrollValue = currentScrollValue;

            if (scrollDelta > 0 && currentScrollIndex > 0)
            {
                currentScrollIndex -= gridColumns;
                currentScrollIndex = Math.Max(0, currentScrollIndex);
            }
            else if (scrollDelta < 0 && currentScrollIndex < inventory.Items.Count - totalVisibleItems)
            {
                currentScrollIndex += gridColumns;
                currentScrollIndex = Math.Min(inventory.Items.Count - totalVisibleItems, currentScrollIndex);
            }

            if (scrollOffset >= itemSize)
            {
                scrollOffset = 0;
            }
            else if (scrollOffset <= -itemSize)
            {
                scrollOffset = 0;
            }
            scrollbarThumbPosition = (float)currentScrollIndex / (inventory.Items.Count - totalVisibleItems) * scrollbarTrackHeight;
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            //Mouse Drag is bug. Waiting to fix.
            /*if (Mouse.GetState().LeftButton == ButtonState.Pressed)
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
            */
            mouseCollision = new((int)mousePosition.X, (int)mousePosition.Y, 1, 1);

            SelectedItem();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(backgroundInventory, Vector2.Zero, Color.White);
            Globals.SpriteBatch.Draw(midgroundInventory, Vector2.Zero, Color.White);
            Color fontColor = new Color(85, 17, 95);
            Globals.SpriteBatch.DrawString(textFont, $"Stage {gameData.stage}", new(73, 43), fontColor, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(textFont, $"Inventory", new(1300, 155), fontColor, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            uiManager.Draw(spriteBatch);

            float itemScale = 0.57f;

            for (int row = 0; row < gridRows; row++)
            {
                for (int col = 0; col < gridColumns; col++)
                {
                    int itemIndex = currentScrollIndex + row * gridColumns + col;

                    if (itemIndex < inventory.Items.Count)
                    {
                        Vector2 position = new Vector2(gridStartPos.X + (col * itemSize) + (col * 10), gridStartPos.Y + (row * itemSize) + (row * 12) + scrollOffset);
                        Rectangle itemCollide = new((int)position.X, (int)position.Y, itemSize, itemSize);

                        if (position.Y >= gridStartPos.Y - itemSize && position.Y < (gridStartPos.Y + gridRows * itemSize) + itemSize)
                        {
                            Color mouseHoverColor = new Color(252, 248, 148);
                            if (itemIndex == selectedItemIndex)
                            {
                                if (itemCollide.Intersects(mouseCollision))
                                {
                                    spriteBatch.Draw(gridTextureSelected, position, mouseHoverColor);
                                    spriteBatch.Draw(inventory.Items[selectedItemIndex].texture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
                                }
                                else
                                {
                                    spriteBatch.Draw(gridTextureSelected, position, mouseHoverColor);
                                    spriteBatch.Draw(inventory.Items[selectedItemIndex].texture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
                                }
                            }
                            else
                            {
                                if (itemCollide.Intersects(mouseCollision))
                                {
                                    spriteBatch.Draw(gridTexture, position, mouseHoverColor);
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
            DrawScrollbar(spriteBatch, scrollbar, thumb);

            DrawEquipItem(itemScale);

            if (inventory.Items[selectedItemIndex].skill.name != "null")
                DrawCurrentItem();


            DrawOnHover();

            drawOnHover = false;

            Globals.DrawCursor();
        }

        private void DrawScrollbar(SpriteBatch spriteBatch, Texture2D scrollbarTexture, Texture2D thumbTexture)
        {
            spriteBatch.Draw(scrollbarTexture, new Rectangle((int)scrollbarPosition.X, (int)scrollbarPosition.Y, scrollbarWidth, scrollbarHeight), Color.LightGray);

            spriteBatch.Draw(thumbTexture, new Rectangle((int)scrollbarPosition.X, (int)(scrollbarPosition.Y + scrollbarThumbPosition), scrollbarWidth, (int)scrollbarThumbHeight), Color.White);
        }

        private void DrawEquipItem(float itemScale)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 position = new Vector2(85, gridStartPos.Y + (i * itemSize) + (i * 20) - 100);
                Keys keyIndex = new Keys();
                if (i == 0) { keyIndex = Keys.Q; }
                else if (i == 1) { keyIndex = Keys.W; }
                else if (i == 2) { keyIndex = Keys.E; }
                else if (i == 3) { keyIndex = Keys.R; }
                Texture2D equipItemTexture = AllSkills.allSkillTextures[player.Skills[keyIndex].name];

                Globals.SpriteBatch.Draw(gridTexture, position, Color.White);
                Globals.SpriteBatch.Draw(equipItemTexture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
            }
        }

        private void DrawCurrentItem()
        {
            //Draw Big Item
            Vector2 bigItemOrigin = new(inventory.Items[selectedItemIndex].texture.Width / 2, inventory.Items[selectedItemIndex].texture.Height / 2);
            Vector2 bigItemPosition = new((Globals.Bounds.X / 4) + 150, (Globals.Bounds.Y / 2) - 125);
            Globals.SpriteBatch.Draw(inventory.Items[selectedItemIndex].texture, bigItemPosition, null, Color.White, 0f, bigItemOrigin, 1f, SpriteEffects.None, 0f);


            Color fontColor = new Color(85, 17, 95);

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
            Vector2 mainStatSize = textFont.MeasureString(mainStatString);
            Vector2 mainStatOrigin = new(mainStatSize.X / 2, mainStatSize.Y / 2);
            Vector2 nameSize = textFont.MeasureString(inventory.Items[selectedItemIndex].name);
            Vector2 nameOrigin = new(nameSize.X / 2, nameSize.Y / 2);
            Globals.SpriteBatch.DrawString(textFont, $"\"{inventory.Items[selectedItemIndex].name}\"", new(bigItemPosition.X, bigItemPosition.Y - 220), fontColor, 0, nameOrigin, 0.24f, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(textFont, mainStatString, new(bigItemPosition.X, bigItemPosition.Y + 155), fontColor, 0, mainStatOrigin, 0.24f, SpriteEffects.None, 0f);


            //Skill Description
            
            string skillName = $"Skill : {(char)34}{inventory.Items[selectedItemIndex].skill.name}{(char)34}";
            string skillDesc = WrapText(textFont, inventory.Items[selectedItemIndex].skill.description, 2000);
            Vector2 skillNameSize = textFont.MeasureString(skillName);
            Vector2 skillNameOrigin = new(skillNameSize.X / 2, skillNameSize.Y / 2);
            Vector2 skillDescSize = textFont.MeasureString(skillDesc);
            Vector2 skillDescOrigin = new(skillDescSize.X / 2, 0);
            Globals.SpriteBatch.DrawString(textFont, skillName, new(bigItemPosition.X, bigItemPosition.Y + 210), fontColor, 0, skillNameOrigin, 0.24f, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(textFont, skillDesc, new(bigItemPosition.X, bigItemPosition.Y + 250), fontColor, 0, skillDescOrigin, 0.2f, SpriteEffects.None, 0f);



        }


        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {

                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {

                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
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
        }

        private void OnStatusButtonHover()
        {
            drawOnHover = true;
        }

        private void DrawOnHover()
        {
            if (drawOnHover)
            {
                Vector2 pos = new Vector2(200, gridStartPos.Y);

                Texture2D playerStatBackground = Globals.Content.Load<Texture2D>("Item/PlayerBackground");
                //Player Stat
                string Header = "Player Status";
                Vector2 HeaderSize = textFont.MeasureString(Header);
                Vector2 HeaderOrigin = new(HeaderSize.X / 2, HeaderSize.Y / 2);
                string HP = $"HP : {player.Status.MaxHP.ToString()}";
                string Attack = $"Attack = {player.Status.Attack.ToString()}";
                string CritRate = $"Critical Rate = {player.Status.CritRate.ToString("F2")}%";
                string CritDam = $"Critical Damage = {player.Status.CritDam.ToString("F2")}%";
                Globals.SpriteBatch.Draw(playerStatBackground, pos, Color.White);
                Globals.SpriteBatch.DrawString(textFont, Header, new Vector2(pos.X + playerStatBackground.Width / 2, pos.Y + HeaderSize.Y), Color.Black, 0, HeaderOrigin, 0.3f, SpriteEffects.None, 0f);
                Globals.SpriteBatch.DrawString(textFont, HP, new Vector2(pos.X + 10, pos.Y + 100), Color.Black, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
                Globals.SpriteBatch.DrawString(textFont, Attack, new Vector2(pos.X + 10, pos.Y + 150), Color.Black, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
                Globals.SpriteBatch.DrawString(textFont, CritRate, new Vector2(pos.X + 10, pos.Y + 200), Color.Black, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
                Globals.SpriteBatch.DrawString(textFont, CritDam, new Vector2(pos.X + 10, pos.Y + 250), Color.Black, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            }
        }

        private void OnPlayButtonClick()
        {
            player.SavePlayer();
            inventory.SaveInventory();
            if (gameData.stage == 1)
            {
                sceneManager.AddScene(new Stage1(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
            }else if (gameData.stage == 2)
            {
                sceneManager.AddScene(new Stage2(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
            }else if(gameData.stage == 3)
            {
                sceneManager.AddScene(new Stage3(contentManager, sceneManager), new CircleTransition(Globals.graphicsDevice));
            }
        }

        private void OnExitButtonClick()
        {
            player.SavePlayer();
            inventory.SaveInventory();
            sceneManager.AddScene(new StageSelectScene(contentManager, sceneManager));
        }



        private void CheatInventory()
        {
            inventory.Items.Clear();

            foreach (var skill in AllSkills.allSkills)
            {
                Item temp = new(AllSkills.itemNameViaSkillName[skill.Key], skill.Value, AllSkills.itemKeyViaSkillName[skill.Key]);
                inventory.Items.Add(temp);
            }
            inventory.ClearNull();
            inventory.SortItem();
        }
    }
}
