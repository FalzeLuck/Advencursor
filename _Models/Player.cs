using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using Advencursor._Skill;
using Advencursor._Combat;
using System.Diagnostics;
using Advencursor._Animation;
using Advencursor._Models.Enemy;
using System.Xml.Linq;
using Advencursor._Models.Enemy._CommonEnemy;
using System.ComponentModel;

namespace Advencursor._Models
{
    [Serializable]
    public class Player : Sprite
    {
        public Status Status { get; set; }
        public Dictionary<Keys, Skill> Skills {  get; private set; }
        public Inventory Inventory { get; set; }


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

        public bool isStop;

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
            AddSkill(item.keys,item.skill);
            CalculateStat(item);

        }

        public void CalculateStat(Item item)
        {
            if(item == null) return;

            if(item.statDesc == "Health")
            {
                Status.SetHP((int)(Status.BaseHP + item.statValue));
            }
            else if (item.statDesc == "Attack")
            {
                Status.SetAttack((int)(Status.BaseAttack + item.statValue));
            }
            else if (item.statDesc == "Critical Rate")
            {
                Status.SetCritRate(item.statValue);
            }
            else if (item.statDesc == "Critical Damage")
            {
                Status.SetCritDamage(item.statValue);
            }
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
                Skills[key].Use(this);
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

        public void Stop()
        {
            isStop = true;
        }

        public void Start()
        {
            isStop = false;
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
                //Immunity(0.5f);
            }
        }

        public void Immunity(float duration)
        {
            Status.immunity = true;
            immuneDuration = duration;
        }

        public void CheckEnemyCollision()
        {
            float takeDamageCooldown = 1f;
            foreach (var enemy in Globals.EnemyManager)
            {
                if (enemy.collision.Intersects(collision) && enemy.collisionCooldown <= 0)
                {
                    if (enemy is Common1)
                    {
                        Common1 common1 = (Common1)enemy;
                        if (common1.isDashing)
                        {
                            TakeDamage((int)(enemy.Status.Attack * 2.5f));
                        }
                        else
                        {
                            TakeDamage((int)(enemy.Status.Attack));
                        }
                        enemy.CollisionCooldownReset(takeDamageCooldown);
                    }
                    if (enemy is Elite1)
                    {
                        Elite1 elite1 = (Elite1)enemy;

                        TakeDamage(enemy.Status.Attack);
                        enemy.CollisionCooldownReset(takeDamageCooldown);
                    }
                    if (enemy is Boss1)
                    {
                        Boss1 boss1 = (Boss1)enemy;

                        if (boss1.dashing)
                        {
                            TakeDamage(enemy.Status.Attack + 2000);
                        }
                        else
                        {
                            TakeDamage(enemy.Status.Attack);
                        }
                        enemy.CollisionCooldownReset(takeDamageCooldown);
                    }
                }
            }
        }


        public  override void Update(GameTime gameTime)
        {
            if (!isStun && !isStop)
            {
                position = new(InputManager._mousePosition.X, InputManager._mousePosition.Y);
                if (animations.ContainsKey(finalIndicator))
                {
                    animations[finalIndicator].Update();
                    collision = animations[finalIndicator].GetCollision(position);
                }
            }
            else if (isStun)
            {
                stunWaitDuration += TimeManager.TimeGlobal;
                Mouse.SetPosition((int)stunPosition.X,(int)stunPosition.Y);

                if(stunWaitDuration > stunDuration)
                {
                    isStun = false;
                    stunWaitDuration = 0f;
                }
            }
            else if (isStop)
            {
                collision = new();
                Mouse.SetPosition((int)position.X, (int)position.Y);
            }


            
            if (parryTimer > 0)
            {
                parryTimer -= TimeManager.TimeGlobal;
            }
            if (cooldownTimer > 0)
            {
                cooldownTimer -= TimeManager.TimeGlobal;
            }

            foreach (var skill in Skills.Values)
            {
                skill.Update(TimeManager.TimeGlobal,this);
            }

            if (Status.immunity)
            {
                immuneDuration -= TimeManager.TimeGlobal;

                if(immuneDuration <= 0f)
                {
                    Status.immunity = false;
                }
            }

            CheckEnemyCollision();
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
