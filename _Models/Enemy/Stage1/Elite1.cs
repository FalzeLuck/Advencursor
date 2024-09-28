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

namespace Advencursor._Models.Enemy.Stage1
{
    public class Elite1 : _Enemy
    {
        public Rectangle slamRadius;
        public float slamCooldown;
        public float slamChargeTime;
        public string slamIndicator;
        public bool isSlamming;
        public bool isSlam;

        public float stunduration;
        public float stuntimer;
        public bool stunned;

        Texture2D warningTexture;
        private bool warningTrigger;
        private float warningOpacity;
        public Elite1(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  8, true) },
                { "Attack", new(texture,row,column,6,3,8,true) },
                { "Charge",new(texture,row,column,1,8,true) },
                { "Stun",new(texture,row,column,1,8,true) },
                { "Die",new(texture,row,column,2,8,false) }

            };
            indicator = "Idle";
            warningTexture = Globals.CreateRectangleTexture(300,300,Color.Red);
            slamIndicator = "slamCharge";

            isSlam = false;
            isSlamming = false;
            slamCooldown = 10f;
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
                //Update Slam Radius
                slamRadius = new Rectangle(collision.X, collision.Y, 300, 300);
                collision = ChangeRectangleSize(collision, 100, true);

                if (!isSlam && !stunned)
                {
                    indicator = "Idle";
                    velocity = new(150);
                    if (Status.isParalysis)
                    {
                        velocity = new(75);
                    }
                    movementAI.Move(this);
                    slamCooldown += TimeManager.TimeGlobal;
                }
                if (isSlam)
                {
                    slamChargeTime += TimeManager.TimeGlobal;
                    velocity = new(0, 0);
                    if (slamChargeTime > 0)
                    {
                        slamIndicator = "slamCharge";
                        indicator = "Charge";
                        isAttacking = false;
                    }

                    if (slamChargeTime > 2.5f)
                    {
                        animations["Attack"].scale = 1.5f;
                        indicator = "Attack";
                        isAttacking = true;
                    }

                    if (slamChargeTime > 3f)
                    {
                        slamIndicator = "slamFinish";
                        isAttacking = false;
                        isSlamming = true;
                    }

                    if (slamChargeTime > 3.5f)
                    {
                        isSlam = false;
                        isSlamming = false;
                        slamCooldown = 0f;
                    }
                }

                //Stun
                if (stunned)
                {
                    indicator = "Stun";
                    stuntimer += TimeManager.TimeGlobal;
                    if (stuntimer > stunduration) stunned = false;
                }
            }
        }

        public override void Draw()
        {
            if (Status.IsAlive() && isSlam && !(slamChargeTime > 2.5f))
            {
                Vector2 origin = new Vector2(warningTexture.Width/2, warningTexture.Height/2);
                if (warningOpacity <= 0.3f)
                {
                    warningTrigger = true;
                }
                else if (warningOpacity >= 0.8f)
                {
                    warningTrigger = false;
                }
                if (!warningTrigger)
                {
                    warningOpacity -= 1 * TimeManager.TimeGlobal;
                }
                else
                {
                    warningOpacity += 1 * TimeManager.TimeGlobal;
                }
                Globals.SpriteBatch.Draw(warningTexture, position, null, Color.White * warningOpacity, 0, origin, 1f, SpriteEffects.None, 0f);
            }

            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Draw(position);
            }
        }

        public override void TakeDamage(float multiplier, Player player, bool throughImmune = false, bool NoCrit = false)
        {
            if (player.isBuff && player.buffIndicator == "Thunder_")
            {
                Status.Paralysis(2f);
            }

            base.TakeDamage(multiplier, player, throughImmune);
        }


        public void Slam()
        {
            if (slamCooldown > 10f)
            {
                isSlam = true;
                slamChargeTime = 0;
            }
        }

        public void Stun(float duration)
        {
            slamChargeTime = 9999f;
            stunned = true;
            stuntimer = 0f;
            stunduration = duration;
        }
    }
}
