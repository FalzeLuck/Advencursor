using Advencursor._Animation;
using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        public bool isDashing;
        private bool canDash => dashCooldown > 5f;
        private bool dash;

        public Rectangle dashRadius;

        public Common1(Texture2D texture, Vector2 position, int health,int attack, int row, int column) : base(texture, position, health, attack)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  4, true) },
                { "Attack", new(texture,row,column,2,12,true) },
                { "Parry", new(texture,row,column,3,12,true) }
                
            };
            indicator = "Idle";

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



            if (Status.isParalysis && !dash)
            {
                velocity = new Vector2(25, 25);
            }
            else if(!Status.isParalysis && !dash)
            {
                velocity = new(50, 50);
            }

            if (dash)
            {
                dashTimer += TimeManager.TimeGlobal;
                dashCooldown = 0f;

                if (dashTimer > 0f)
                {
                    dash = true;
                    movementAI.Stop();
                }
                if(dashTimer > 0.7f)
                {
                    isAttacking = true;
                    indicator = "Parry";
                }
                if (dashTimer > 1.2f)
                {
                    position += dashDirection * TimeManager.TimeGlobal;
                    isAttacking = false;
                    isDashing = true;
                    indicator = "Attack";
                }
                if(dashTimer > 1.7f)
                {
                    isDashing = false;
                    dash = false;
                }
            }
            else if (!dash)
            {
                indicator = "Idle";
                movementAI.Start();
                isAttacking = false;
                isDashing = false;
                dashCooldown += TimeManager.TimeGlobal;
                dashTimer = 0f;
            }

            
        }

        public void Dash(Sprite target)
        {
            if (canDash)
            {
                dash = true;
                var direction = target.position - position;
                direction.Normalize();
                dashDirection = direction * 600;
            }
        }

        public void DashStop()
        {
            dash = false ;
        }

        public override void TakeDamage(int damage,Player player, bool throughImmune = false)
        {
            if(player.isBuff && player.buffIndicator == "Thunder_")
            {
                Status.Paralysis(2f);
            }

            base.TakeDamage(damage, player, throughImmune);
        }
    }
}
