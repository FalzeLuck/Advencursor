using Advencursor._Models;
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

namespace Advencursor._Scene.Stage
{
    public class StageSelectScene : IScene
    {

        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uiManager;

        private Texture2D background;

        private Inventory inventory = new Inventory();
        private string pathinventory = "inventory.json";
        

        public StageSelectScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            uiManager = new UIManager();

        }

        public void Load()
        {
            Texture2D nullTexture = new Texture2D(Globals.graphicsDevice, 1, 1);

            


            UIButton stage1Button = new(Globals.Content.Load<Texture2D>("Button/Stage1Button"), new Vector2(Globals.Bounds.X / 2 - 400, 500 + Globals.Bounds.Y / 2), OnEquipButtonClick);
            uiManager.AddElement(stage1Button);

            background = Globals.Content.Load<Texture2D>("Background/Stage1_2");
            

            inventory.LoadInventory(pathinventory, nullTexture);




        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = true;
            uiManager.Update(gameTime);
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Rectangle mouseCollision = new((int)mousePosition.X, (int)mousePosition.Y, 1, 1);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            uiManager.Draw(spriteBatch);

           
        }

        

        


        private void OnEquipButtonClick()
        {
            

        }


        private void Cleanup()
        {
            background = null;
            

            if (inventory != null && inventory.Items != null)
            {
                inventory.Items.Clear();
            }

        }
    }
}
