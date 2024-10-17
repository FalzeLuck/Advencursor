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
using Advencursor._SaveData;
using System.Text.Json;
using System.IO;
using Advencursor._Models.Enemy.Stage1;
using System.Text;

namespace Advencursor._Models
{
    [Serializable]
    public class Player : Sprite
    {
        public Dictionary<Keys, Skill> Skills {  get; private set; }
        public Inventory Inventory { get; set; }

        private GameData gameData;


        public float parryDuration = 0.1f;
        public float parryCooldown = 1.0f;

        private float parryTimer = 0f;
        public float cooldownTimer {  get; private set; } = 0f;
        public bool isParrying => parryTimer > 0;
        public bool CanParry => cooldownTimer <= 0;

        public float normalAttackCooldown;
        public float normalAttackDelay = 0.4f;


        public bool isStun;
        public float stunDuration;
        public float stunWaitDuration;
        public bool isFlip;
        public Vector2 stunPosition;

        public bool isBuff;
        public string buffIndicator;

        public bool isStop;

        public string finalIndicator {  get; private set; }

        //Immune
        private float immuneDuration;


        public Player(Texture2D texture, Vector2 position, float health,float attack, int row, int column) : base(texture, position)
        {
            animations = new Dictionary<string, Animation>
            {
                { "Normal_Idle", new(texture, row, column,1,  8, true) },
                { "Normal_Attack", new(texture, row, column,4, 2,  16, false) },
                { "Thunder_Idle", new(texture, row, column,1,  8, true) },
                { "Thunder_Attack", new(texture, row, column,4,2,  16, false) }
            };
            
            Skills = new Dictionary<Keys, Skill>();
            Status = new(health,attack);
            Status.SetCritRate(10);
            Status.SetCritDamage(50);
            Inventory = new Inventory();

            isStun = false;
            isBuff = false;
            indicator = "Idle";
            buffIndicator = "Normal_";
            finalIndicator =  buffIndicator + indicator;
            stunWaitDuration = 0f;

            //default skill
            Skill nullSkill = new Skill("null", new SkillData());
            Skills = new()
            {
                {Keys.Q,nullSkill},
                {Keys.W,nullSkill},
                {Keys.E,nullSkill},
                {Keys.R,nullSkill},
            };
        }

        public void LoadGameData(GameData gameData)
        {
            this.gameData = gameData;
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
                Status.SetAttack((int)(Status.BaseAttack + (item.statValue/100)* Status.BaseAttack));
            }
            else if (item.statDesc == "Critical Rate")
            {
                Status.SetCritRate(item.statValue + 10);
            }
            else if (item.statDesc == "Critical Damage")
            {
                Status.SetCritDamage(item.statValue + 50);
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
        public void ChangeAnimation(string name,bool flip)
        {
            isFlip = flip;
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

        public void TakeDamage(float damage,_Enemy fromwho)
        {
            if (!Status.immunity)
            {
                Status.TakeDamage(damage,fromwho);
            }
        }

        public void Immunity(float duration)
        {
            Status.immunity = true;
            immuneDuration = duration;
            foreach(var anim in animations.Values)
            {
                anim.Blink(duration);
            }
        }

        public void CheckEnemyCollision()
        {
            float takeDamageCooldown = 1f;
            foreach (var enemy in Globals.EnemyManager)
            {
                if (enemy.Status.IsAlive())
                {

                    if (enemy.collision.Intersects(collision) && enemy.collisionCooldown <= 0)
                    {
                        if (enemy is Common1)
                        {
                            Common1 common1 = (Common1)enemy;
                            if (common1.isDashing)
                            {
                                switch (gameData.stage)
                                {
                                    case 1:
                                        TakeDamage(250f, enemy);
                                        break;
                                    case 2:
                                        TakeDamage(250f, enemy);
                                        break;
                                    case 3:
                                        TakeDamage(300f, enemy);
                                        break;
                                }

                                
                            }
                            else
                            {
                                switch (gameData.stage)
                                {
                                    case 1:
                                        TakeDamage(100f, enemy);
                                        break;
                                    case 2:
                                        TakeDamage(100f, enemy);
                                        break;
                                    case 3:
                                        TakeDamage(200f, enemy);
                                        break;
                                }
                            }
                            enemy.CollisionCooldownReset(takeDamageCooldown);
                        }
                        if (enemy is Elite1)
                        {
                            Elite1 elite1 = (Elite1)enemy;
                            TakeDamage(enemy.Status.Attack, elite1);
                            enemy.CollisionCooldownReset(takeDamageCooldown);
                        }
                        if (enemy is Boss1)
                        {
                            Boss1 boss1 = (Boss1)enemy;

                            if (boss1.dashing)
                            {
                                if (gameData.stage == 1)
                                {
                                    TakeDamage(5000, enemy);
                                }
                                else
                                {
                                    TakeDamage(4000,enemy);
                                }
                                Immunity(1f);
                            }
                            else
                            {
                                if (gameData.stage == 1)
                                {
                                    TakeDamage(3000, enemy);
                                }
                                else
                                {
                                    TakeDamage(2500, enemy);
                                }
                            }
                            enemy.CollisionCooldownReset(takeDamageCooldown);
                        }
                    }

                }
            }
        }
        public void DoNormalAttack()
        {
            normalAttackCooldown = normalAttackDelay;
        }

        public bool CanNormalAttack()
        {
            if (normalAttackCooldown <= 0) 
            {
                foreach (var anim in animations.Values) anim.Reset();
                return true;
            }
            else { return false; }
        }

        public void SetAttackDelay(float delay)
        {
            normalAttackDelay = delay;
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
                    int decreaseamount = 125;
                    int newWidth = collision.Width - decreaseamount;
                    int newHeight = collision.Height - decreaseamount;
                    int newX = collision.X + decreaseamount / 2;
                    int newY = collision.Y + decreaseamount / 2;
                    collision = new Rectangle(newX,newY,newWidth,newHeight);
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
                    foreach (var anim in animations.Values)
                    {
                        anim.blinkingDuration = 0f;
                        anim.opacityValue = 1f;
                    }
                }
            }

            if (normalAttackCooldown >= 0f)
            {
                normalAttackCooldown -= TimeManager.TimeGlobal;
            }

            CheckEnemyCollision();
        }

        public override void Draw()
        {
            if (animations.ContainsKey(finalIndicator))
            {
                animations[finalIndicator].IsFlip = isFlip;
                animations[finalIndicator].Draw(position);
            }
            foreach(var skill in Skills.Values)
            {
                skill.Draw();
            }

            //DrawCollision();
        }

        public void DrawCollision()
        {
            Globals.DrawLine(new Vector2(collision.X, collision.Y), new Vector2(collision.X + collision.Width, collision.Y), Color.Green, 1);
            Globals.DrawLine(new Vector2(collision.X, collision.Y + collision.Height), new Vector2(collision.X + collision.Width, collision.Y + collision.Height), Color.Green, 1);
        }

        public void SavePlayer()
        {
            PlayerData data = new()
            {
                Health = Status.MaxHP,
                Attack = Status.Attack,
                CritRate = Status.CritRate,
                CritDamage = Status.CritDam,
                SkillNames = new Dictionary<Keys, string>()
            };
            foreach (var skill in Skills)
            {
                data.SkillNames[skill.Key] = skill.Value.name;
            }

            string serializedData = JsonSerializer.Serialize(data);
            string encodedJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedData));
            File.WriteAllText("playerdata.dat", encodedJson);
        }

        public void LoadPlayer(int row, int column)
        {
            if (!File.Exists("playerdata.dat")) return;
            string encodedJson = File.ReadAllText("playerdata.dat");
            string deserializedData  = Encoding.UTF8.GetString(Convert.FromBase64String(encodedJson));
            PlayerData data = JsonSerializer.Deserialize<PlayerData>(deserializedData);

            Texture2D playertexture = Globals.Content.Load<Texture2D>("playerTexture");
            foreach (var anim in animations.Values)
            {
                anim.SetRowColumn(row, column);
            }
            Status.SetHP(data.Health);
            Status.SetAttack(data.Attack);
            Status.SetCritRate(data.CritRate);
            Status.SetCritDamage(data.CritDamage);

            foreach (var skill in data.SkillNames)
            {
                AddSkill(skill.Key, AllSkills.allSkills[skill.Value]);
            }

        }
    }
}
