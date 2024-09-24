using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene
{
    public class SceneManager
    {
        private readonly Stack<IScene> sceneStack;
        private IScene currentScene;
        private bool isTransitioning;

        public SceneManager()
        {
            sceneStack = new();
            isTransitioning = false;
        }

        public void AddScene(IScene scene)
        {
            isTransitioning = true;

            if (currentScene != null)
            {
                RemoveScene();
            }

            scene.Load();
            sceneStack.Push(scene);
            currentScene = scene;
            isTransitioning = false;
        }

        public void RemoveScene()
        {
            if (sceneStack.Count > 0)
            {
                sceneStack.Pop();
            }

            currentScene = sceneStack.Count > 0 ? sceneStack.Peek() : null;
        }

        public IScene GetCurrScene()
        {
            return currentScene;
        }

        public void Update(GameTime gameTime)
        {
            if (!isTransitioning && currentScene != null)
            {
                currentScene.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isTransitioning && currentScene != null)
            {
                currentScene.Draw(spriteBatch);
            }
        }
    }
}
