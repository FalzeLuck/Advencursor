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
        public Status Status { get; set; }
        public MovementAI movementAI { get; set; }
        public Rectangle parryZone;
        public bool isAttacking;

        public float collisionCooldown {  get; private set; }

        public _Enemy(Texture2D texture, Vector2 position, int health,int attack) : base(texture, position)
        {
            Status = new(health, attack);
            animations = new Dictionary<string, Animation>();
        }

        public override void Update(GameTime gameTime)
        {
            collisionCooldown -= TimeManager.TotalSeconds;
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

        public virtual void TakeDamage(int damage, Player player)
        {
            Status.TakeDamage(damage);
        }

        public virtual void CollisionCooldownReset(float timer)
        {
            collisionCooldown = timer;
        }

    }
}
