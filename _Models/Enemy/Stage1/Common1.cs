using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models.Enemy._CommonEnemy
{
    public class Common1 : _Enemy
    {
        private float dashTimer;
        private float dashCooldown;
        private Vector2 dashDirection;
        private float jumpTime = 0f;

        public bool isDashing;
        private bool canDash => dashCooldown > 5f;
        private bool dash;

        public Rectangle dashRadius;

        //Jump section
        private int jumpHeightCount = 0;
        private int jumpHeightMax = 20;
        float jumpXDirection;
        float jumpYDirection;

        public Common1(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Walk", new(texture, row, column,2,  8, false) },
                { "Attack", new(texture,row,column,2,8,true) },
                { "Die", new(texture,row,column,3,12,false) },
            };
            indicator = "Walk";

            dash = false;
            dashCooldown = 5f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
                collision = animations[indicator].GetCollision(position);
            }
            UpdateParryZone();
            if (Status.IsAlive())
            {
                //Update Radius
                dashRadius = collision;
                int increaseamount = 300;
                int newX = dashRadius.X - increaseamount / 2;
                int newY = dashRadius.Y - increaseamount / 2;
                int newWidth = dashRadius.Width + increaseamount;
                int newHeight = dashRadius.Height + increaseamount;
                dashRadius = new Rectangle(newX, newY, newWidth, newHeight);

                movementAI.Move(this);
                Status.Update();
                int defaultSpeed = 200;
                if (Status.isParalysis)
                {
                    defaultSpeed = 100;
                }
                
                if (dash)
                {
                    dash = true;
                    dashTimer += TimeManager.TimeGlobal;
                    dashCooldown = 0f;
                    velocity = Vector2.Zero;

                    if (dashTimer > 0f)
                    {
                        indicator = "Attack";
                    }
                    if (dashTimer > 0.7f)
                    {
                        isAttacking = true;
                    }
                    if (dashTimer > 1.2f)
                    {
                        position += dashDirection * 600 * TimeManager.TimeGlobal;
                        isAttacking = false;
                        isDashing = true;
                        indicator = "Attack";
                    }
                    if (dashTimer > 1.7f)
                    {
                        isDashing = false;
                        dash = false;
                    }



                }
                else if (!dash)
                {
                    indicator = "Walk";
                    isAttacking = false;
                    isDashing = false;
                    dashCooldown += TimeManager.TimeGlobal;
                    dashTimer = 0f;
                    

                    Vector2 dir = movementAI.target.position - position;
                    dir.Normalize();

                    if (animations["Walk"].currentFrame == 10)
                    {
                        animations["Walk"].PauseFrame(4);
                    }
                    if (animations["Walk"].currentFrame == 11)
                    {
                        if (jumpHeightCount < jumpHeightMax)
                        {
                            jumpXDirection = dir.X * defaultSpeed * TimeManager.TimeGlobal;
                            jumpYDirection = -600 * TimeManager.TimeGlobal;
                            jumpHeightCount++;
                        }
                        else
                        {
                            if (position.Y < movementAI.target.position.Y)
                            {
                                jumpXDirection = dir.X * defaultSpeed * TimeManager.TimeGlobal;
                                jumpYDirection = 600 * TimeManager.TimeGlobal;
                            }
                            else
                            {
                                jumpXDirection = 0;
                                jumpYDirection = 0;
                                animations["Walk"].Play();
                            }
                        }
                        position += new Vector2(jumpXDirection, jumpYDirection);
                    }
                    if (animations["Walk"].IsComplete)
                    {
                        animations["Walk"].Reset();
                        jumpHeightCount = 0;
                    }

                }

            }


        }

        public void Dash()
        {
            if (canDash)
            {
                animations["Attack"].Reset();
                jumpHeightCount = 0;
                dash = true;
                var direction = movementAI.target.position - position;
                direction.Normalize();
                dashDirection = direction;
            }
        }

        public void DashStop()
        {
            dash = false;
        }

        public override void TakeDamage(float multiplier, Player player, bool throughImmune = false, bool NoCrit = false)
        {
            if (player.isBuff && player.buffIndicator == "Thunder_")
            {
                Status.Paralysis(2f);
            }

            base.TakeDamage(multiplier, player, throughImmune);
        }
    }
}
