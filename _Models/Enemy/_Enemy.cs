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

        public float collisionCooldown { get; set; }

        public _Enemy(Texture2D texture, Vector2 position, int health, int attack) : base(texture, position)
        {
            Status = new(health, attack);
            animations = new Dictionary<string, Animation>();
        }

        public override void Update(GameTime gameTime)
        {
            collisionCooldown -= TimeManager.TimeGlobal;
            Vector2 playerPosition = new(InputManager._mousePosition.X, InputManager._mousePosition.Y);

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
        public virtual void TakeDamage(float fixedDamage, Sprite fromwho)
        {
            Status.TakeDamageNoCrit(fixedDamage, fromwho);
        }

        public virtual void TakeDamage(float multiplier, Player player, bool throughImmune = false, bool NoCrit = false)
        {
            if (throughImmune)
            {
                Status.TakeDamageNoImmune(multiplier * player.Status.Attack, player);
            }
            else
            {
                Status.TakeDamage(multiplier * player.Status.Attack, player);
            }
        }

        public virtual void TakeDamage(float multiplier, Player player, float fixedDamage, bool throughImmune = false, bool NoCrit = false)
        {
            if (throughImmune)
            {
                Status.TakeDamageNoImmune(fixedDamage, player);
            }
            else
            {
                if (NoCrit)
                {
                    Status.TakeDamageNoCrit(fixedDamage, player);
                }
                else
                    Status.TakeDamage(fixedDamage, player);
            }
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
