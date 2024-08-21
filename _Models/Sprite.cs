using Advencursor._Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class Sprite
    {
        protected Texture2D texture;
        protected Vector2 origin;
        public Vector2 position {  get; set; }
        public int speed {  get; set; }
        public float rotation {  get; set; }

        public SpriteEffects spriteEffects;
        public bool flip;

        protected Dictionary<string ,Animation> animations;
        protected int row;
        protected int column;

        public Sprite(Texture2D texture,Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            speed = 300;
            origin = new Vector2(texture.Width/2, texture.Height/2);
            spriteEffects = SpriteEffects.None;
        }


        public virtual void Draw()
        {
            Globals.SpriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1, spriteEffects, 1);
        }

        public void FlipHorizontal()
        {
            if (flip == true)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                flip = false;
            }
            else if (flip == false)
            {
                spriteEffects = SpriteEffects.None;
                flip = true;
            }
        }
    }
}
