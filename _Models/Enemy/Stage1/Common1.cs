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

        private bool isShadowStop;
        private Vector2 shadowLockStartPos;
        private Vector2 shadowLockEndPos;
        Vector2 shadowPosition;

        public Common1(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Walk", new(texture, row, column,2,  8, false) },
                { "Attack", new(texture,row,column,7,1,8,false) },
                { "Die", new(texture,row,column,3,12,false) },
                { "LoopAttack", new(texture,row,column,7,1,8,true) },
            };
            indicator = "Walk";

            dash = false;
            dashCooldown = 5f;
            shadowTexture = Globals.Content.Load<Texture2D>("Enemies/Shadow1");
            isShadowStop = false;
            shadowPosition = new Vector2(position.X, position.Y + 75 / 2);

        }

        public override void Update(GameTime gameTime)
        {
            collisionCooldown -= TimeManager.TimeGlobal;
            burnDuration -= TimeManager.TimeGlobal;
            Vector2 playerPosition = new(InputManager._mousePosition.X, InputManager._mousePosition.Y);
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
                collision = animations[indicator].GetCollision(position);
            }
            UpdateParryZone();
            if (Status.IsAlive() && movementAI.target != null)
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


                    if (dashTimer > 2.5f)
                    {
                        isDashing = false;
                        dash = false;
                    }
                    else if (dashTimer > 1.7f)
                    {
                        animations[indicator].IsPause = false;
                        if (animations[indicator].IsComplete)
                        {
                            animations[indicator].PauseFrame(7);
                        }
                    }
                    else if (dashTimer > 1.2f)
                    {
                        animations[indicator].PauseFrame(3);
                        position += new Vector2(dashDirection.X * 1000 * TimeManager.TimeGlobal, 0);
                        isAttacking = false;
                        isDashing = true;
                        indicator = "Attack";
                    }
                    else if (dashTimer > 0.7f)
                    {
                        isAttacking = true;
                    }
                    else if (dashTimer > 0f)
                    {
                        indicator = "Attack";
                        if (animations[indicator].currentFrame == 1)
                        {
                            animations[indicator].PauseFrame(2);
                        }
                    }







                }
                else if (!dash)
                {
                    FlipAuto(playerPosition);

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
                        isShadowStop = true;
                        shadowLockStartPos = new Vector2(position.X, position.Y + 75 / 2);
                        shadowLockEndPos = new Vector2(movementAI.target.position.X, movementAI.target.position.Y);
                    }
                    if (animations["Walk"].currentFrame == 11)
                    {
                        dir = shadowLockEndPos - shadowLockStartPos;
                        dir.Normalize();
                        if (jumpHeightCount < jumpHeightMax)
                        {
                            jumpXDirection = dir.X * defaultSpeed * TimeManager.TimeGlobal;
                            jumpYDirection = -600 * TimeManager.TimeGlobal;
                            jumpHeightCount++;
                        }
                        else
                        {
                            if (position.Y < shadowLockEndPos.Y)
                            {
                                jumpXDirection = dir.X * defaultSpeed * TimeManager.TimeGlobal;
                                jumpYDirection = 600 * TimeManager.TimeGlobal;
                            }
                            else
                            {
                                isShadowStop = false;
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

            if (movementAI.target == null)
            {
                indicator = "LoopAttack";
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

        public override void TakeDamage(float multiplier, Player player, bool throughImmune = false, bool NoCrit = false, Color color = default)
        {
            if (player.isBuff && player.buffIndicator == "Thunder_")
            {
                Status.Paralysis(2f);
            }

            base.TakeDamage(multiplier, player, throughImmune,NoCrit,color);
        }

        public override void Draw()
        {
            DrawShadow();
            base.Draw();
        }

        public void ChangeColor(Color color)
        {
            foreach (var animation in animations.Values)
            {
                animation.color = color;
            }
        }
        private void DrawShadow()
        {
            float distance = Math.Abs(position.Y - shadowLockEndPos.Y);
            float shadowScale = 1f;
            float maxDistance = 1000f;

            if (!isShadowStop)
            {
                shadowPosition = new Vector2(position.X, position.Y + 75 / 2);
            }
            else
            {
                Vector2 dir = shadowLockEndPos - shadowLockStartPos;
                dir.Normalize();

                float minSpeed = 1000f;
                float maxSpeed = 1200f;
                float speedFactor = MathHelper.Clamp(distance / maxDistance, 0.2f, 1.0f);

                float speed = MathHelper.Lerp(minSpeed, maxSpeed, speedFactor);
                if (!dash)
                {
                    if (dir.Y < 0)
                    {
                        if (shadowPosition.Y <= shadowLockEndPos.Y)
                        {
                            shadowPosition = new Vector2(position.X, shadowLockEndPos.Y );
                        }
                        else
                        {
                            shadowPosition = new Vector2(position.X, position.Y + 75);
                        }
                    }
                    else if (dir.Y > 0)
                    {
                        if (shadowPosition.Y >= shadowLockEndPos.Y + 75 / 2)
                        {
                            shadowPosition = new Vector2(position.X, shadowLockEndPos.Y + 75 / 2);
                        }
                        else
                        {
                            shadowPosition = new Vector2(position.X, shadowPosition.Y + dir.Y * speed * TimeManager.TimeGlobal);
                        }
                    }
                }
                else
                {
                    shadowPosition = new Vector2(position.X, shadowPosition.Y);
                }

                shadowScale = MathHelper.Clamp(1 - (distance / maxDistance), 0.4f, 1.0f);

            }


            Vector2 shadowOrigin = new Vector2(shadowTexture.Width / 2, shadowTexture.Height / 2);
            Globals.SpriteBatch.Draw(shadowTexture, shadowPosition, null, Color.White, rotation, shadowOrigin, shadowScale, spriteEffects, 0f);
        }
    }
}
