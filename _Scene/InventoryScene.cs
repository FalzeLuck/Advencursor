using Advencursor._Models;
using Advencursor._SaveData;
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
using System.IO;
using System.Linq;
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

        private Texture2D background;

        private Inventory inventory = new Inventory();
        private string pathinventory = "inventory.json";
        private Dictionary<Keys,Item> equippedItems = new Dictionary<Keys,Item>();


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

        //Player Save and Load
        private Player player;
        private string pathplayer = "playerdata.json"; 


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
            Texture2D tempTexture = Globals.Content.Load<Texture2D>("UI/SkillUI");

            player = new(Globals.Content.Load<Texture2D>("playerTexture"), Vector2.Zero, 15000, 800, 0, 0);
            //player = LoadPlayer(pathplayer);
            //Trace.WriteLine(player.Status.MaxHP);
            

            UIButton equipButton = new(Globals.Content.Load<Texture2D>("Button/EquipButton"), new Vector2(Globals.Bounds.X/2 - 400, Globals.Bounds.Y / 2), OnEquipButtonClick);
            UIButton playButton = new(Globals.Content.Load<Texture2D>("Button/playButton"), new Vector2(Globals.Bounds.X / 2 - 400, 200 + Globals.Bounds.Y / 2), OnPlayButtonClick);
            UIButton exitButton = new(Globals.Content.Load<Texture2D>("Button/exitButton"), new Vector2(0, 0), OnExitButtonClick);
            uiManager.AddElement(equipButton);
            uiManager.AddElement(exitButton);
            uiManager.AddElement(playButton);

            background = Globals.Content.Load<Texture2D>("Background/Stage1_1");
            gridTexture = Globals.Content.Load<Texture2D>("Item/Grid");
            gridTextureSelected = Globals.Content.Load<Texture2D>("Item/GridSelected");



            Skill ThunderCore = AllSkills.allSkills["Thunder Core"];
            Skill ThunderShuriken = AllSkills.allSkills["Thunder Shuriken"];
            Skill ThunderSpeed = AllSkills.allSkills["Thunder Speed"];
            Skill ThunderStorm = AllSkills.allSkills["I am the Storm"];


            inventory.Items.Add(new Item(tempTexture, "ThunderCore book", ThunderCore, Keys.Q));
            inventory.Items.Add(new Item(tempTexture, "ThunderShuriken book", ThunderShuriken, Keys.W));
            inventory.Items.Add(new Item(tempTexture,"ThunderSpeed book", ThunderSpeed, Keys.E));
            inventory.Items.Add(new Item(tempTexture, "ThunderStorm book", ThunderStorm, Keys.R));

            Texture2D nullTexture = new(Globals.graphicsDevice, 1, 1);
            Item nullItem = new Item(nullTexture, "null book", ThunderCore, Keys.None);
            for (int i = 0; i < totalVisibleItems*2 ; i++)
            {
                inventory.Items.Add(nullItem);
            }

            Trace.WriteLine(inventory.Items.Count);

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
            player.EquipItem(inventory.Items[selectedItemIndex]);
            Trace.WriteLine($"HP : {player.Status.MaxHP} Attack = {player.Status.Attack} Rate = {player.Status.CritRate} Damage = {player.Status.CritDam}");

        }

        private void OnPlayButtonClick()
        {
            SavePlayer(player);
            sceneManager.AddScene(new Stage1(contentManager,sceneManager));
        }

        private void OnExitButtonClick()
        {
            Globals.Game.Exit();
        }

        private void SavePlayer(Player player)
        {
            PlayerData data = new()
            {
                Health = player.Status.MaxHP,
                Attack = player.Status.Attack,
                SkillNames = new Dictionary<Keys, string>()
            };
            foreach (var skill in player.Skills)
            {
                data.SkillNames[skill.Key] = skill.Value.name;
            }

            string serializedData = JsonSerializer.Serialize(data);
            File.WriteAllText("playerdata.json",serializedData);
        }

        private Player LoadPlayer(string filepath)
        {
            string deserializedData = File.ReadAllText(filepath);
            PlayerData data = JsonSerializer.Deserialize<PlayerData>(deserializedData);

            Texture2D playertexture = Globals.Content.Load<Texture2D>("playerTexture");
            Player player = new Player(playertexture,Vector2.Zero,data.Health,data.Attack,row :4,column:1);
            foreach (var skill in data.SkillNames)
            {
                player.AddSkill(skill.Key, AllSkills.allSkills[skill.Value]);
            }

            return player;
        }
    }
}
