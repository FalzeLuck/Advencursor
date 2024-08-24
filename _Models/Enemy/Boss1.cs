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

        public Boss1(Texture2D texture, Vector2 position, int health,int attack, int row, int column) : base(texture, position, health,attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  1, true) },
                { "Attack", new(texture,row,column,2,1,true) },
                {"Stun", new(texture,row,column,3,1,true) }

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
                int topBound = 0;
                int bottomBound = 1080;
                int leftBound = 0;
                int rightBound = 1920;
                indicator = "Idle";

                if (collision.X > leftBound && collision.X + collision.Width < rightBound && collision.Y > topBound && collision.Y + collision.Height < bottomBound)
                {
                    velocity = new(50, 50);
                }
                else if (collision.X <= leftBound)
                {
                    velocity = new(0,50);
                    position += new Vector2(1,0);
                }
                else if (collision.Y <= topBound)
                {
                    velocity = new(50, 0);
                    position += new Vector2(0, 1);
                }
                else if (collision.X + collision.Width >= rightBound)
                {
                    velocity = new(0, 50);
                    position -= new Vector2(1, 0);
                }
                else if (collision.Y + collision.Height >= bottomBound)
                {
                    velocity = new(50, 0);
                    position -= new Vector2(0, 1);
                }

            }

            //Dashing
            if (dashing)
            {
                indicator = "Attack";
                position += velocity * TimeManager.TotalSeconds;

                if(collision.X+collision.Width >= Globals.Bounds.X || collision.X <= 0 || collision.Y+collision.Height >= Globals.Bounds.Y || collision.Y <= 0)
                {
                    dashing = false;
                }
            }

            //Stun
            if (stunned)
            {
                indicator = "Stun";
                stuntimer += TimeManager.TotalSeconds;
                if(stuntimer > stunduration) stunned = false;
            }
        }

        public void Dash(Sprite target)
        {
            dashing = true;
            var direction = target.position - position;
            direction.Normalize();
            velocity = direction * 2000;
        }

        public void Stun(float stunduration)
        {
            stunned = true;
            stuntimer = 0f;
            this.stunduration = stunduration;
        }
    }
}
