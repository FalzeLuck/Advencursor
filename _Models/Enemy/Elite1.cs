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

        private Dictionary<string,Animation> slamTexture;

        public Elite1(Texture2D texture, Vector2 position, int health, int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  1, true) },
                { "Attack", new(texture,row,column,2,1,true) },
                { "Charge",new(texture,row,column,3,1,true) },
                { "Stun",new(texture,row,column,4,1,true) },
                { "Die",new(texture,row,column,4,1,false) }

            };
            indicator = "Idle";

            Texture2D slamFile = Globals.Content.Load<Texture2D>("Animation/slamTexture");
            slamTexture = new Dictionary<string, Animation>
            {
                {"slamCharge", new(slamFile,2,3,startrow: 1,fps:15,true) },
                {"slamFinish", new(slamFile,2,3,startrow: 2,fps:15,true) }
            };
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

            if (slamTexture.ContainsKey(slamIndicator))
            {
                slamTexture[slamIndicator].Update();
            }
            UpdateParryZone();

            //Update Slam Radius
            slamRadius = collision;
            int increaseamount = 150;
            int newX = slamRadius.X - increaseamount / 2;
            int newY = slamRadius.Y - increaseamount / 2;
            int newWidth = slamRadius.Width + increaseamount;
            int newHeight = slamRadius.Height + increaseamount;
            slamRadius = new Rectangle(newX, newY, newWidth, newHeight);

            if (!isSlam && !stunned)
            {
                indicator = "Idle";
                velocity = new(80, 80);
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

                if(slamChargeTime > 2.5f)
                {
                    indicator = "Attack";
                    isAttacking = true;
                }

                if (slamChargeTime > 3f)
                {
                    slamIndicator = "slamFinish";
                    isAttacking = false ;
                    isSlamming = true;
                }

                if(slamChargeTime > 3.5f)
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

        public override void Draw()
        {
            if (isSlam && slamTexture.ContainsKey(slamIndicator))
            {
                slamTexture[slamIndicator].Draw(position);
            }
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Draw(position);
            }
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
            this.stunduration = duration;
        }
    }
}
