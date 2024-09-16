using Advencursor._Models;
using Advencursor._Scene.Stage;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene
{
    public class InventoryScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private Texture2D background;

        private Inventory inventory = new Inventory();
        private Texture2D gridTexture;
        private Texture2D gridTextureSelected;
        private int gridColumns = 5;
        private int gridRows = 6;
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

        //Mouse Cot=ntrol
        private Rectangle mouseCollision;

        //Player
        private Player player;


        private int totalItems;
        private int totalPages;
        private float scrollbarTrackHeight;
        private int previousScrollValue = 0;

        public InventoryScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();

            totalVisibleItems = gridColumns * gridRows;
            scrollbarPosition = new Vector2((itemSize*gridColumns) + gridStartPos.X, gridStartPos.Y);
        }

        public void Load()
        {
            player = new(Globals.Content.Load<Texture2D>("playerTexture"), Vector2.Zero, 0, 0, 0, 0);

            UIButton equipButton = new(Globals.Content.Load<Texture2D>("Button/EquipButton"), new Vector2(Globals.Bounds.X/2 - 400, Globals.Bounds.Y / 2), OnEquipButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("Button/exitButton"), new Vector2(Globals.Bounds.X - 400, (Globals.Bounds.Y / 2) + 300), OnExitButtonClick);
            uiManager.AddElement(equipButton);
            uiManager.AddElement(exitButton);

            background = Globals.Content.Load<Texture2D>("Background/Stage1_1");
            gridTexture = Globals.Content.Load<Texture2D>("Item/Grid");
            gridTextureSelected = Globals.Content.Load<Texture2D>("Item/GridSelected");



            Skill_Q_ThunderCore ThunderCore = new Skill_Q_ThunderCore("Thunder Core", 5);
            Skill_W_ThunderShuriken ThunderShuriken = new Skill_W_ThunderShuriken("Thunder Shuriken", 2);
            //Skill_E_ThunderSpeed ThunderSpeed = new Skill_E_ThunderSpeed("Thunder Speed", 2,player);
            //Skill_R_IamStorm ThunderStorm = new Skill_R_IamStorm("I am the Storm", 2, player);
            Texture2D temp = Globals.Content.Load<Texture2D>("UI/SkillUI");
            Texture2D temp2 = Globals.Content.Load<Texture2D>("UI/HealthBarFull");

            inventory.Items.Add(new Item(temp,"ThunderCore book", ThunderCore, Keys.Q));
            for (int i = 0; i < 41; i++)
            {
                inventory.Items.Add(new Item(temp, "Item " + i, null, Keys.None));
            }

            //inventory.Items.Add(new Item("ThunderSpeed book", ThunderSpeed, Keys.E));
            //inventory.Items.Add(new Item("ThunderStorm book", ThunderStorm, Keys.R));


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
                    currentScrollIndex--;
                    scrollOffset = 0;
                }
            }
            else if (scrollOffset <= -itemSize)
            {
                if (currentScrollIndex < inventory.Items.Count - totalVisibleItems)
                {
                    currentScrollIndex++;
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

                    currentScrollIndex = (int)(scrollbarThumbPosition / scrollbarTrackHeight * totalPages);
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
            Globals.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            uiManager.Draw(spriteBatch);

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
                                    spriteBatch.Draw(inventory.Items[selectedItemIndex].texture, position, Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(gridTextureSelected, position, Color.White);
                                    spriteBatch.Draw(inventory.Items[selectedItemIndex].texture, position, Color.White);
                                }
                            }
                            else
                            {
                                if (itemCollide.Intersects(mouseCollision))
                                {
                                    spriteBatch.Draw(gridTexture, position, Color.Yellow);
                                    spriteBatch.Draw(inventory.Items[itemIndex].texture, position, Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(gridTexture, position, Color.White);
                                    spriteBatch.Draw(inventory.Items[itemIndex].texture, position, Color.White);
                                }
                            }
                        }
                    }
                }
            }

            Texture2D scrollbar = Globals.Content.Load<Texture2D>("Item/scrollBarTexture");
            Texture2D thumb = Globals.Content.Load<Texture2D>("Item/scrollBarThumb");
            DrawScrollbar(spriteBatch,scrollbar,thumb);
        }

        private void DrawScrollbar(SpriteBatch spriteBatch, Texture2D scrollbarTexture, Texture2D thumbTexture)
        {
            spriteBatch.Draw(scrollbarTexture,new Rectangle((int)scrollbarPosition.X, (int)scrollbarPosition.Y, scrollbarWidth, scrollbarHeight),Color.Gray);

            spriteBatch.Draw(thumbTexture,new Rectangle((int)scrollbarPosition.X, (int)(scrollbarPosition.Y + scrollbarThumbPosition), scrollbarWidth, (int)scrollbarThumbHeight),Color.White);
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
            inventory.EquipItem(inventory.Items[selectedItemIndex],player);
            Trace.WriteLine(inventory.Items[selectedItemIndex].name);
        }

        private void OnExitButtonClick()
        {
            Globals.Game.Exit();
        }
    }
}
