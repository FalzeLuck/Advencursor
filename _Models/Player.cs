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
using System.Xml.Linq;

namespace Advencursor._Models
{
    public class Player : Sprite
    {
        public Status Status { get; set; }
        public Dictionary<Keys, Skill> Skills {  get; private set; }
        public Inventory Inventory { get; private set; }


        public float parryDuration = 0.1f;
        public float parryCooldown = 1.0f;

        private float parryTimer = 0f;
        public float cooldownTimer {  get; private set; } = 0f;
        public bool isParrying => parryTimer > 0;
        public bool CanParry => cooldownTimer <= 0;


        public bool isStun;
        public float stunDuration;
        public float stunWaitDuration;
        public Vector2 stunPosition;

        public bool isBuff;
        public string buffIndicator;
        public string finalIndicator {  get; private set; }

        private float immuneDuration;


        public Player(Texture2D texture, Vector2 position, int health,int attack, int row, int column) : base(texture, position)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Normal_Idle", new(texture, row, column,1,  TimeManager.framerate, true) },
                { "Normal_Attack", new(texture, row, column,2,  TimeManager.framerate, true) },
                { "Thunder_Idle", new(texture, row, column,3,  TimeManager.framerate, true) },
                { "Thunder_Attack", new(texture, row, column,4,  TimeManager.framerate, true) }
            };
            
            Skills = new Dictionary<Keys, Skill>();
            Status = new(health,attack);
            Inventory = new Inventory();

            isStun = false;
            isBuff = false;
            indicator = "Idle";
            buffIndicator = "Normal_";
            finalIndicator =  buffIndicator + indicator;
            stunWaitDuration = 0f;
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

        public void Stun(float duration)
        {
            isStun = true;
            stunDuration = duration;
            stunPosition = position;
        }

        public void ChangeAnimation(string name)
        {
            if (!isBuff)
            {
                finalIndicator = buffIndicator + name;
            }
            if (isBuff)
            {
                indicator = name;
                finalIndicator = buffIndicator + indicator;
            }
        }

        public void TakeDamage(int damage)
        {
            if (!Status.immunity)
            {
                Status.TakeDamage(damage);
                Immunity(0.5f);
            }
        }

        public void Immunity(float duration)
        {
            Status.immunity = true;
            immuneDuration = duration;
        }


        public  override void Update(GameTime gameTime)
        {
            if (!isStun)
            {
                position = new(InputManager._mousePosition.X, InputManager._mousePosition.Y);
            }
            if (isStun)
            {
                stunWaitDuration += TimeManager.TotalSeconds;
                Mouse.SetPosition((int)stunPosition.X,(int)stunPosition.Y);

                if(stunWaitDuration > stunDuration)
                {
                    isStun = false;
                    stunWaitDuration = 0f;
                }
            }

            
            if (animations.ContainsKey(finalIndicator))
            {
                animations[finalIndicator].Update();
                collision = animations[finalIndicator].GetCollision(position);
            }
            if (parryTimer > 0)
            {
                parryTimer -= TimeManager.TotalSeconds;
            }
            if (cooldownTimer > 0)
            {
                cooldownTimer -= TimeManager.TotalSeconds;
            }

            foreach (var skill in Skills.Values)
            {
                skill.Update(TimeManager.TotalSeconds,this);
            }

            if (Status.immunity)
            {
                immuneDuration -= TimeManager.TotalSeconds;

                if(immuneDuration <= 0f)
                {
                    Status.immunity = false;
                }
            }
        }

        public override void Draw()
        {
            if (animations.ContainsKey(finalIndicator))
            {
                animations[finalIndicator].Draw(position);
            }
            foreach(var skill in Skills.Values)
            {
                skill.Draw();
            }
        }
    }
}
