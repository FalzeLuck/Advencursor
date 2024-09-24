using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Scene.Transition;

namespace Advencursor._Scene
{
    public class SceneManager
    {
        private readonly Stack<IScene> sceneStack;
        private IScene currentScene;
        private IScene nextScene;
        private ITransition transition;
        private bool isTransitioning;
        private bool transitionStart;

        public SceneManager()
        {
            sceneStack = new();
            isTransitioning = false;
            transitionStart = false;
        }

        public void AddScene(IScene scene, ITransition transition = null)
        {
            nextScene = scene;
            this.transition = transition ?? new FadeTransition();
            transitionStart = true;
            isTransitioning = true;  
        }

        public void RemoveScene()
        {
            sceneStack.Pop();
            currentScene = sceneStack.Count > 0 ? sceneStack.Peek() : null;
        }

        public IScene GetCurrScene()
        {
            return currentScene;
        }

        public void Update(GameTime gameTime)
        {
            if (transitionStart)
            {
                transition.Start(false);
                transitionStart = false;
            }

            if (isTransitioning)
            {
                transition.Update(gameTime);

                if (transition.IsComplete && !transition.IsInTransition)
                {
                    sceneStack.Push(nextScene);
                    nextScene.Load();
                    currentScene = nextScene;

                    transition.Start(true);
                }
                else if (transition.IsComplete && transition.IsInTransition)
                {
                    isTransitioning = false;
                }
            }
            else
            {
                currentScene?.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScene?.Draw(spriteBatch);

            if (isTransitioning)
            {
                transition.Draw(spriteBatch);
            }
        }

    }
}
