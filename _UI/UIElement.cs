using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
        public float opacity = 1f;
        public Rectangle collision;

        public UIElement(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteEffects = SpriteEffects.None;


            int frameWidth = texture.Width;
            int frameHeight = texture.Height;
            int startX = (int)((position.X - frameWidth / 2));
            int startY = (int)((position.Y - frameHeight / 2));
            collision = new(startX, startY, frameWidth, frameHeight);
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
