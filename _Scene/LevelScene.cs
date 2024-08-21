using Advencursor._Managers;
using Advencursor._Models;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Advencursor._Scene
{
    public class LevelScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private UIManager uIManager;
        public LevelScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            this.uIManager = new UIManager();
        }

        public void Load()
        {
            
        }

        public void Update(GameTime gameTime)
        {
            Globals.Game.IsMouseVisible = true;


            
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}
