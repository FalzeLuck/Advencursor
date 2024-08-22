using Advencursor._AI;
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
    public class Boss1 : _Enemy
    {
        public bool charge;
        public bool dashing;
        public bool dashed;

        public float stunduration;
        public float stuntimer;
        public bool stunned;
        
        public Rectangle checkRadius;
        public float charge_duration;

        public Boss1(Texture2D texture, Vector2 position, int health, int row, int column) : base(texture, position, health)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  1, true) },
                { "Attack", new(texture,row,column,2,1,false) },
                {"Stun", new(texture,row,column,2,1,true) }

            };
            indicator = "Idle";

            charge = false;
            dashing = false;
            dashed = false;
            stunned = false;
            
        }

        public override void Update(GameTime gameTime)
        {
            movementAI.Move(this);
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update(gameTime);
                collision = animations[indicator].GetCollision(position);
            }

            UpdateParryZone();

            //Update Radius
            checkRadius = collision;
            int increaseamount = 300;
            int newX = checkRadius.X - increaseamount / 2;
            int newY = checkRadius.Y - increaseamount / 2;
            int newWidth = checkRadius.Width + increaseamount;
            int newHeight = checkRadius.Height + increaseamount;
            checkRadius = new Rectangle(newX, newY, newWidth, newHeight);

            //Limit Movement
            if (!dashing && !stunned)
            {
                if (position.Y > 470 && position.Y < 1080 - 200)
                {
                    velocity = new(50, 50);
                }
                else if (position.Y <= 470)
                {
                    velocity = new(50, 0);
                    position = new(position.X, position.Y + 1);
                }
                else if (position.Y >= 1080 - 200)
                {
                    velocity = new(50, 0);
                    position = new(position.X, position.Y - 1);
                }
            }

            //Dashing
            if (dashing)
            {
                position += velocity * TimeManager.TotalSeconds;

                if(collision.X+collision.Width >= Globals.Bounds.X || collision.X <= 0)
                {
                    dashing = false;
                }
            }

            //Stun
            if (stunned)
            {
                stuntimer += TimeManager.TotalSeconds;
                if(stuntimer > stunduration) stunned = false;
            }
        }

        public void Dash(Sprite target)
        {
            dashing = true;
            var direction = target.position - position;
            direction.Normalize();
            if (target.position.X > position.X)
            {
                velocity = new(1000,direction.Y);
            }
            else
            {
                velocity = new(-1000,direction.Y);
            }
        }

        public void Stun(float stunduration)
        {
            stunned = true;
            stuntimer = 0f;
            this.stunduration = stunduration;
        }
    }
}
