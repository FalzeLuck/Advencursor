using Advencursor._UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Managers
{
    public class UIManager
    {
        private List<UIElement> uiElements;

        public UIManager()
        {
            uiElements = new List<UIElement>();
        }

        public void AddElement(UIElement element)
        {
            uiElements.Add(element);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var element in uiElements)
            {
                element.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var element in uiElements)
            {
                element.Draw(spriteBatch);
            }
        }


    }
}
