using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Combat;
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
        public float stand_time;

        public Boss1(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,4,1,  2, true) },
                { "Attack", new(texture,row,column,3,8,false) },
                {"Stun", new(texture,row,column,1,0,true) },
                {"Die", new(texture,row,column,2,8,false) },

            };
            indicator = "Idle";


            checkRadius = new Rectangle(9999, 9999, 0, 0);
            charge = false;
            dashing = false;
            dashed = false;
            stunned = false;

        }

        public override void Update(GameTime gameTime)
        {
            collisionCooldown -= TimeManager.TimeGlobal;
            Vector2 playerPosition = new(InputManager._mousePosition.X, InputManager._mousePosition.Y);

            

            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
                collision = animations[indicator].GetCollision(position);
            }



            if (Status.IsAlive())
            {


                UpdateParryZone();

                //Update Radius
                checkRadius = collision;
                checkRadius = ChangeRectangleSize(checkRadius,300,false);
                collision = ChangeRectangleSize(collision,125,true);
                

                //Dashing
                if (dashing)
                {
                    indicator = "Attack";
                    position += velocity * TimeManager.TimeGlobal;

                    if (animations["Attack"].currentFrame == 21  && stand_time >= 0.5f)
                    {
                        animations["Attack"].PauseFrame(5);
                    }

                    if (collision.X + collision.Width >= Globals.Bounds.X || collision.X <= 0 || collision.Y + collision.Height >= Globals.Bounds.Y || collision.Y <= 0)
                    {
                        if (!animations["Attack"].IsComplete)
                        {
                            animations["Attack"].Play();
                            Globals.Camera.Shake(0.2f, 5);
                        }
                        velocity = Vector2.Zero;
                        stand_time -= TimeManager.TimeGlobal;
                    }

                    if(stand_time <= 0f)
                    {
                        dashing = false;
                        stand_time = 0.5f;
                    }
                }

                //Stun
                if (stunned)
                {
                    indicator = "Stun";
                    stuntimer += TimeManager.TimeGlobal;
                    if (stuntimer > stunduration) stunned = false;
                }

                if (!dashing && !stunned && !charge && movementAI != null)
                {
                    movementAI.Move(this);
                    //Change Flip Cuz art do wrong side
                    if (playerPosition.X < position.X)
                    {
                        foreach (var anim in animations.Values)
                        {
                            anim.IsFlip = false;
                        }
                    }
                    else
                    {
                        foreach (var anim in animations.Values)
                        {
                            anim.IsFlip = true;
                        }
                    }


                    int topBound = 0;
                    int bottomBound = 1080;
                    int leftBound = 0;
                    int rightBound = 1920;
                    indicator = "Idle";

                    int speed = 100;
                    if (collision.X > leftBound && collision.X + collision.Width < rightBound && collision.Y > topBound && collision.Y + collision.Height < bottomBound)
                    {
                        velocity = new(speed);
                    }
                    else if (collision.X <= leftBound)
                    {
                        velocity = new(0, speed);
                        position += new Vector2(1, 0);
                    }
                    else if (collision.Y <= topBound)
                    {
                        velocity = new(speed, 0);
                        position += new Vector2(0, 1);
                    }
                    else if (collision.X + collision.Width >= rightBound)
                    {
                        velocity = new(0, speed);
                        position -= new Vector2(1, 0);
                    }
                    else if (collision.Y + collision.Height >= bottomBound)
                    {
                        velocity = new(speed, 0);
                        position -= new Vector2(0, 1);
                    }

                }



            }
            else
            {
                checkRadius = new();
                collision = new();
            }

        }

        public void Dash(Sprite target)
        {
            stand_time = 0.5f;
            animations["Attack"].Reset();
            dashing = true;
            var direction = target.position - position;
            direction.Normalize();
            velocity = direction * 1000;
        }

        public void Stun(float stunduration)
        {
            stunned = true;
            stuntimer = 0f;
            this.stunduration = stunduration;
        }


    }
}
