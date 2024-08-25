using Advencursor._UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Models;
using System.Xml.Linq;

namespace Advencursor._Managers
{
    public class UIManager
    {
        private List<UIElement> uiElements;
        private Dictionary<string, ProgressBar> progressBarDictionary;

        public UIManager()
        {
            uiElements = new List<UIElement>();
            progressBarDictionary = new Dictionary<string, ProgressBar>();
        }

        public void AddElement(UIElement element)
        {
            uiElements.Add(element);
        }

        public void AddElement(string barName,ProgressBar progressBar)
        {
            progressBarDictionary.Add(barName, progressBar);
        }


        public void Update(GameTime gameTime)
        {
            foreach (var element in uiElements)
            {
                element.Update(gameTime);
            }
            foreach (var value in progressBarDictionary.Values)
            {
                value.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var element in uiElements)
            {
                element.Draw(spriteBatch);
            }
            foreach (var value in progressBarDictionary.Values)
            {
                value.Draw(spriteBatch);
            }
        }

        public void UpdateBarValue(string barName,float value)
        {
            if (progressBarDictionary.ContainsKey(barName))
            {
                progressBarDictionary[barName].UpdateValue(value);
            }
        }

        public void SetAllOpacity(float opacity)
        {
            foreach (var element in uiElements)
            {
                element.opacity = opacity;
            }
            foreach (var value in progressBarDictionary.Values)
            {
                value.opacity = opacity;
            }
        }

        public bool CheckCollide(Sprite sprite)
        {
            foreach (var element in uiElements)
            {
                if (sprite.collision.Intersects(element.collision))
                {
                    return true;
                }
            }
            foreach (var value in progressBarDictionary.Values)
            {
                if (sprite.collision.Intersects(value.collision))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
