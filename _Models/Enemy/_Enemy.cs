using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Combat;
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
    public abstract class _Enemy : Sprite
    {

        public MovementAI movementAI { get; set; }
        public Rectangle parryZone;
        public bool isAttacking;
        protected Texture2D shadowTexture;

        public bool isAmp = false;
        public float ampMultiplier = 1f;
        public float burnDuration = 0f;
        public bool isBurn => burnDuration > 0;

        public float collisionCooldown { get; set; }

        public _Enemy(Texture2D texture, Vector2 position, int health, int attack) : base(texture, position)
        {
            Status = new(health, attack);
            animations = new Dictionary<string, Animation>();
        }

        public override void Update(GameTime gameTime)
        {
            collisionCooldown -= TimeManager.TimeGlobal;
            burnDuration -= TimeManager.TimeGlobal;
            Vector2 playerPosition = new(InputManager._mousePosition.X, InputManager._mousePosition.Y);
            FlipAuto(playerPosition);
        }

        public void FlipAuto(Vector2 playerPosition,bool reverse = false)
        {
            if (!reverse)
            {
                if (playerPosition.X > position.X)
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
            }
            else
            {
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
            }
        }

        public void UpdateParryZone()
        {
            parryZone = collision;
            int increaseamount = 300;
            int newX = parryZone.X - increaseamount / 2;
            int newY = parryZone.Y - increaseamount / 2;
            int newWidth = parryZone.Width + increaseamount;
            int newHeight = parryZone.Height + increaseamount;
            parryZone = new Rectangle(newX, newY, newWidth, newHeight);
        }
        public void BurnDamage(Sprite player)
        {
            if (isBurn)
            {
                if (player is Player)
                {
                    Status.TakeDamageNoCrit(player.Status.Attack, player, Color.OrangeRed);
                    burnDuration = 0;
                }
            }
        }
        public virtual void TakeDamage(float fixedDamage, Sprite fromwho)
        {
            Status.TakeDamageNoCrit(fixedDamage * ampMultiplier, fromwho,Color.White);
            BurnDamage(fromwho);
        }

        public virtual void TakeDamage(float multiplier, Player player, bool throughImmune = false, bool NoCrit = false)
        {
            if (throughImmune)
            {
                Status.TakeDamageNoImmune(multiplier * player.Status.Attack * ampMultiplier, player,NoCrit);
            }
            else
            {
                Status.TakeDamage(multiplier * player.Status.Attack * ampMultiplier, player);
            }
            BurnDamage(player);
        }

        public virtual void TakeDamage(float multiplier, Player player, float fixedDamage, bool throughImmune = false, bool NoCrit = false)
        {
            if (throughImmune)
            {
                Status.TakeDamageNoImmune(fixedDamage * ampMultiplier, player , NoCrit);
            }
            else
            {
                if (NoCrit)
                {
                    Status.TakeDamageNoCrit(fixedDamage * ampMultiplier, player,Color.White);
                }
                else
                    Status.TakeDamage(fixedDamage * ampMultiplier, player);
            }
            BurnDamage(player);
        }

        

        public virtual void CollisionCooldownReset(float timer)
        {
            collisionCooldown = timer;
        }

        public virtual void Die()
        {
            indicator = "Die";
        }

    }
}
