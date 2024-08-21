using Advencursor._AI;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy
{
    public class SlimeTheBoss : Boss
    {

        public SlimeTheBoss(Texture2D texture, Vector2 position, int health) : base(texture, position, health)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            collision = new((int)(position.X - texture.Width / 2), (int)(position.Y - texture.Height / 2), texture.Width, texture.Height);
            movementAI.Move(this);
            speed = 300;
        }
    }
}
