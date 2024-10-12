using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Models;
using System.Xml.Linq;

namespace Advencursor._UI
{
    public class UIManager
    {
        private Dictionary<string,UIElement> uiElements;
        private Dictionary<string, ProgressBar> progressBarDictionary;

        public UIManager()
        {
            uiElements = new Dictionary<string, UIElement>();
            progressBarDictionary = new Dictionary<string, ProgressBar>();
        }

        public void AddElement(string name,UIElement element)
        {
            if (uiElements.ContainsKey(name)) return;
            uiElements.Add(name,element);
        }

        public void AddElement(string name, ProgressBarAnimated bossbar)
        {
            progressBarDictionary.Add(name,bossbar);
        }

        public void RemoveElement(string name)
        {
            uiElements.Remove(name);
            progressBarDictionary?.Remove(name);
        }

        public void RemoveAll()
        {
            uiElements?.Clear();
            progressBarDictionary?.Clear();
        }

        public Vector2 GetElementPosition(string name)
        {
            return uiElements[name].position;
        }

        public void SetElementPosition(Vector2 position, string name)
        {
            uiElements[name].position = position;
        }


        public void Update(GameTime gameTime)
        {
            foreach (var element in uiElements.Values)
            {
                element.Update(gameTime);
            }
            foreach (var element in progressBarDictionary.Values)
            {
                element.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var element in uiElements.Values)
            {
                element.Draw(spriteBatch);
            }
            foreach(var element in progressBarDictionary.Values)
            {
                element.Draw(spriteBatch);
            }
        }

        public void UpdateBarValue(string barName, float value)
        {
            if (progressBarDictionary.ContainsKey(barName))
            {
                progressBarDictionary[barName].UpdateValue(value);
            }
        }

        public void SetAllOpacity(float opacity)
        {
            foreach (var element in uiElements.Values)
            {
                element.opacity = opacity;
            }
        }

        public void SetScale(string name,float scale)
        {
            uiElements[name].scale = scale;
        }

        public void HideAll()
        {
            foreach (var element in uiElements.Values)
            {
                element.isVisible = false;
            }
        }

        public void ShowAll()
        {
            foreach (var element in uiElements.Values)
            {
                element.isVisible = true;
            }
        }

        public bool CheckCollide(Sprite sprite)
        {
            foreach (var element in uiElements.Values)
            {
                if (sprite.collision.Intersects(element.collision))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
