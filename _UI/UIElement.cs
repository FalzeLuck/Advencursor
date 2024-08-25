using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._UI
{
    public abstract class UIElement
    {
        public bool isVisible { get; set; } = true;
        public readonly Texture2D texture;
        protected readonly Vector2 origin;
        public Vector2 position { get; set; }
        public float rotation { get; set; }
        public SpriteEffects spriteEffects;
        public bool flip;
        protected Color _color = Color.White;

        public UIElement(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteEffects = SpriteEffects.None;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
