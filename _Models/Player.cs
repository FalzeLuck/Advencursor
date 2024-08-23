using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Skill;
using Advencursor._Combat;
using System.Diagnostics;
using Advencursor._Animation;
using Advencursor._Models.Enemy;

namespace Advencursor._Models
{
    public class Player : Sprite
    {
        public Status Status { get; set; }
        public Dictionary<Keys, Skill> Skills {  get; private set; }
        public Inventory Inventory { get; private set; }

        public Rectangle collision {  get; private set; }

        public float parryDuration = 0.1f;
        public float parryCooldown = 1.0f;

        private float parryTimer = 0f;
        private float cooldownTimer = 0f;
        public bool isParrying => parryTimer > 0;
        public bool CanParry => cooldownTimer <= 0;



        public Player(Texture2D texture, Vector2 position, int health,int attack, int row, int column) : base(texture, position)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Idle", new(texture, row, column,1,  TimeManager.framerate, true) },
                { "Attack", new(texture, row, column,2,  TimeManager.framerate, true) }
            };
            indicator = "Idle";
            Skills = new Dictionary<Keys, Skill>();
            Status = new(health,attack);
            Inventory = new Inventory();
        }


        public void EquipItem(Item item)
        {
            Inventory.EquipItem(item, this);
        }
        public void AddSkill(Keys key,Skill skill)
        {
            Skills[key] = skill;
        }
        public void RemoveSkill(Keys key) { Skills[key] = null; }

        public void UseSkill(Keys key)
        {
            if (Skills.ContainsKey(key) && Skills[key].CanUse())
            {
                Skills[key].Use();
                ParticleManager.AddParticle(new(position, new()));
                Debug.WriteLine("Player use Q");
            }

            if (Skills.ContainsKey(key) && !Skills[key].CanUse())
            {
                Debug.WriteLine("Player cant use Q");
            }

        }

        public void StartParry()
        {
            if (CanParry)
            {
                parryTimer = parryDuration;
                cooldownTimer = parryCooldown;
            }
        }
        public bool TryParryAttack(_Enemy enemy)
        {
            if(isParrying && enemy.isAttacking && collision.Intersects(enemy.parryZone))
            {
                return true;
            }
            return false;
        }


        public  override void Update(GameTime gameTime)
        {
            position = new(InputManager._mousePosition.X,InputManager._mousePosition.Y);
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update(gameTime);
                collision = animations[indicator].GetCollision(position);
            }
            if (parryTimer > 0)
            {
                parryTimer -= TimeManager.TotalSeconds;
            }
            if (cooldownTimer > 0)
            {
                cooldownTimer -= TimeManager.TotalSeconds;
            }
        }

        public override void Draw()
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Draw(position);
            }
        }
    }
}
