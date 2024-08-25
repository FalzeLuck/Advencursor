using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy
{
    public class Special1 : _Enemy
    {
        private float walkTimer;

        public Special1(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  4, true) },
                { "Attack", new(texture,row,column,2,12,true) },
                { "Parry", new(texture,row,column,3,12,true) }

            };

            indicator = "Idle";
            walkTimer = 0;
            velocity = new Vector2(50, 0);
        }

        public override void Update(GameTime gameTime)
        {

            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update(gameTime);
                collision = animations[indicator].GetCollision(position);
            }
            UpdateParryZone();

            walkTimer += TimeManager.TotalSeconds;
            Trace.WriteLine(velocity);

            if (walkTimer > 1.5f)
            {
                walkTimer = 0f;
            }
            else if (walkTimer > 1f)
            {
                position = position;
            }
            else if (walkTimer > 0)
            {
                position += velocity * TimeManager.TotalSeconds;
            }


            if (collision.X + collision.Width > Globals.Bounds.X)
            {
                velocity = new Vector2(velocity.X * - 1,0);
                position += new Vector2(-10,0);
            }

            if (collision.X < 0)
            {
                velocity = new Vector2(velocity.X * -1, 0);
                position += new Vector2(10, 0);
            }

        }
        }
}
