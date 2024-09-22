using Advencursor._Managers;
using Advencursor._Models.Enemy;
using Advencursor._Models;
using Advencursor._Skill;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Combat;
using Microsoft.Xna.Framework;
using Advencursor._Animation;
using Advencursor._UI;
using Microsoft.Xna.Framework.Input;
using Advencursor._AI;
using Advencursor._Models.Enemy._CommonEnemy;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Advencursor._Skill.Thunder_Set;
using System.ComponentModel;
using Advencursor._Particles;
using System.Xml.Serialization;
using Advencursor._Particles.Emitter;
using Advencursor._SaveData;
using System.Text.Json;
using System.IO;


namespace Advencursor._Scene.Stage
{
    public class Stage1 : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private AnimationManager animationManager = new AnimationManager();
        private SoundManager soundManager = new SoundManager();
        private DamageNumberManager damageNumberManager;

        private SpriteFont font;


        private Player player;
        private Inventory inventory = new Inventory();

        List<Common1> commonEnemy;
        List<Elite1> eliteEnemy;
        List<Special1> specialEnemy;
        List<PoisonPool> poisonPool;
        private Boss1 boss_obj;
        List<Item> items;


        private readonly Timer timer;
        private Texture2D background;

        private readonly Random random = new Random();


        //Stage Timer & Controls
        private bool canAttack = true;
        private float boss_spawn_time;
        private float boss_dash_cooldown;
        private bool boss_spawned;
        private float enemy_spawn_time;
        private float elite_spawn_time;
        private float special_spawn_time;
        private int enemy_count = 0;
        private int elite_count = 0;
        private int special_count = 0;
        private int enemy_max = 40;
        private int elite_max = 0;
        private int special_max = 3;
        private int enemy_killed = 0;

        //UI
        private UIManager uiManager = new UIManager();


        public Stage1(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            Globals.Game.IsMouseVisible = false;
            Mouse.SetPosition(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
            font = Globals.Content.Load<SpriteFont>("basicFont");

            damageNumberManager = new(Globals.Content.Load<SpriteFont>("Font/DamageNumber"));

            timer = new(Globals.Content.Load<Texture2D>("TestUI"),
                font,
                new(1920/2, 0)
                );

            timer.StartStop();
            timer.Repeat = true;
            boss_spawn_time = 0f;
            elite_spawn_time = 0f;
            enemy_spawn_time = 0f;
            boss_spawned = false;
        }


        public void Load()
        {
            Texture2D tempTexture = new Texture2D(Globals.graphicsDevice, 1, 1);

            //Load Background
            background = Globals.Content.Load<Texture2D>("Background/BG_Stage1");

            //Player
            Texture2D playertexture = Globals.Content.Load<Texture2D>("playerTexture");
            player = new(playertexture, Vector2.Zero, 1, 1, 1, 1);
            player.LoadPlayer(4,1);
            inventory.LoadInventory("inventory.json",tempTexture);
            damageNumberManager.SubscribeToTakeDamageEvent(player.Status, player);

            //Load enemies
            commonEnemy = new List<Common1> ();
            eliteEnemy = new List<Elite1> ();
            specialEnemy = new List<Special1> ();
            poisonPool = new List<PoisonPool> ();


            //Boss
            boss_obj = new Boss1(Globals.Content.Load<Texture2D>("Enemies/Boss1"), new(100000, 500), health: 100000, attack: 3000, row: 2, column: 8)
            {
                
            };
            

            //Load Animation
            Animation slashAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/SlashTexture"), row: 1, column: 1, fps: 5, false);
            animationManager.AddAnimation("Slash", slashAnimation);
            Animation sparkAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/Sparkle"), row: 1, column: 4, fps: 12, false);
            animationManager.AddAnimation("Sparkle", sparkAnimation);

            


            //Load UI
            UIBackground uIBackground = new(Globals.Content.Load<Texture2D>("UI/SkillBackground"), new(Globals.Bounds.X / 2, 930));
            int startX = (int)uIBackground.position.X - (uIBackground.texture.Width/2);
            int space = uIBackground.texture.Width / 10;
            int skillY = 950;


            UISkill skillUI_Q = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 2), skillY), player.Skills[Keys.Q]);
            UISkill skillUI_W = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 4), skillY), player.Skills[Keys.W]);
            UISkill skillUI_E = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 6), skillY), player.Skills[Keys.E]);
            UISkill skillUI_R = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 8), skillY), player.Skills[Keys.R]);
            UIPlayerCheckPanel uIPanel = new(Globals.Content.Load<Texture2D>("TestUI"), new(150, 100), player);

            Texture2D bg = Globals.Content.Load<Texture2D>("UI/HealthBarNone");
            Texture2D fg = Globals.Content.Load<Texture2D>("UI/HealthBarFull");
            ProgressBarAnimated playerHpBar = new ProgressBarAnimated(bg, fg, player.Status.MaxHP, new(Globals.Bounds.X / 2, uIBackground.position.Y - 75));
            

            uiManager.AddElement(uIBackground);
            uiManager.AddElement("playerBar", playerHpBar);
            uiManager.AddElement(skillUI_Q);
            uiManager.AddElement(skillUI_W);
            uiManager.AddElement(skillUI_E);
            uiManager.AddElement(skillUI_R);
            uiManager.AddElement(uIPanel);

            //Load Sound
            SoundEffect beep = Globals.Content.Load<SoundEffect>("Sound/Beep");
            soundManager.LoadSound("Beep",beep);
            SoundEffect charge = Globals.Content.Load<SoundEffect>("Sound/Boss1Charge");
            soundManager.LoadSound("Charge", charge);
            soundManager.SetVolume("Charge", 0.2f);
            SoundEffect parry = Globals.Content.Load<SoundEffect>("Sound/Parry");
            soundManager.LoadSound("Parry", parry);
        }

        public void Update(GameTime gameTime)
        {
            EnemyManage();

            //Boss Control
            UpdateBoss(gameTime);


            //Parry Control
            /*
            if (player.TryParryAttack(boss_obj))
            {
                boss_obj.Stun(5f);
                boss_obj.isAttacking = false;
                boss_obj.charge = false ;
                player.Status.immunity = true;
                soundManager.PlaySound("Parry");
                animationManager.SetOffset("Sparkle", new Vector2(0, 0));
                animationManager.UpdatePosition("Sparkle", player.position);
                animationManager.Play("Sparkle");
            }
            foreach (var enemy in commonEnemy)
            {
                if (player.TryParryAttack(enemy))
                {
                    soundManager.PlaySound("Parry");
                    animationManager.SetOffset("Sparkle", new Vector2(0, 0));
                    animationManager.UpdatePosition("Sparkle", player.position);
                    animationManager.Play("Sparkle");
                    player.Status.immunity = true;
                    enemy.DashStop();
                }
            }
            foreach (var elite in eliteEnemy)
            {
                if (player.TryParryAttack(elite))
                {
                    soundManager.PlaySound("Parry");
                    animationManager.SetOffset("Sparkle", new Vector2(0, 0));
                    animationManager.UpdatePosition("Sparkle", player.position);
                    animationManager.Play("Sparkle");
                    player.Status.immunity = true;
                    elite.Stun(2.5f);
                    elite.Status.TakeDamage(player.Status.Attack,player);
                }
            }*/

            foreach (var enemy in Globals.EnemyManager)
            {
                enemy.Update(gameTime);
            }
            UiManage(gameTime);
            CollisionManage(gameTime);

            //Mash Update
            UpdatePlayer();
            UpdateEnemies(gameTime);
            UpdateElites(gameTime);
            UpdateSpecial(gameTime);
            UpdatePoisonPool(gameTime);

            timer.Update();
            player.Update(gameTime);
            animationManager.Update(gameTime);
            animationManager.UpdatePosition("Slash",player.position);
            ParticleManager.Update();
            
            
            

            SceneManage();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            timer.Draw();
            
            foreach (var enemy in commonEnemy)
            {
                enemy.Draw();
            }
            foreach (var elite in eliteEnemy)
            {
                elite.Draw();
            }
            if (boss_spawned)
            {
                boss_obj.Draw();
            }
            foreach (var pool in poisonPool)
            {
                pool.Draw();
            }
            foreach (var special in specialEnemy)
            {
                special.Draw();
            }
            uiManager.Draw(spriteBatch);
            player.Draw();
            
            animationManager.Draw();
            ParticleManager.Draw();
            damageNumberManager.Draw();
        }

        private void UpdatePlayer()
        {
            if (!player.isStun && !player.isStop)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    player.UseSkill(Keys.Q);
                    player.ChangeAnimation("Idle");
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    player.UseSkill(Keys.W);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    player.UseSkill(Keys.E);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    player.UseSkill(Keys.R);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    player.StartParry();
                }



                if (InputManager.MouseRightClicked && player.CanNormalAttack())
                {
                    animationManager.SetOffset("Slash", new Vector2(player.collision.Width / 2, 0));
                    player.ChangeAnimation("Attack",true);
                    animationManager.Flip("Slash", true);
                    animationManager.Play("Slash");
                    canAttack = false;
                    player.DoNormalAttack(player.normalAttackDelay);
                }
                if (InputManager.MouseLeftClicked && player.CanNormalAttack())
                {
                    animationManager.SetOffset("Slash", new Vector2(-player.collision.Width / 2, 0));
                    player.ChangeAnimation("Attack",false);
                    animationManager.Flip("Slash",false);
                    animationManager.Play("Slash");
                    canAttack=false;
                    player.DoNormalAttack(player.normalAttackDelay);
                }
            }

            if (player.CanNormalAttack())
            {
                foreach (var enemy in commonEnemy)
                {
                    if (enemy.Status.immunity)
                    {
                        enemy.Status.immunity = false;
                    }
                }
                boss_obj.Status.immunity = false ;
                player.ChangeAnimation("Idle");
                animationManager.Stop("Slash");
            }

            if (animationManager.IsComplete("Sparkle"))
            {
                animationManager.Stop("Sparkle");
            }
        }
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (var enemy in commonEnemy)
            {
                if (enemy.dashRadius.Intersects(enemy.movementAI.target.collision))
                {
                    enemy.Dash();
                }
            }
            
        }
        private void UpdateElites(GameTime gameTime)
        {
            foreach (var elite in eliteEnemy)
            {
               
                if (elite.slamRadius.Intersects(player.collision) && !elite.isSlam)
                {
                    elite.Slam();
                }

                if(elite.isSlamming && elite.slamRadius.Intersects(player.collision))
                {
                    player.Stun(2);
                    player.Status.TakeDamage(3000,elite);
                    player.Immunity(0.5f);
                }

                if (animationManager.IsComplete("Slash"))
                {
                    elite.Status.immunity = false;
                }

                
            }
        }
        private void UpdateSpecial(GameTime gameTime)
        {
            foreach (var special in specialEnemy)
            {
                special.Update(gameTime);

                if (animationManager.IsComplete("Slash"))
                {
                    special.Status.immunity = false;
                }
            }
        }
        private void UpdatePoisonPool(GameTime gameTime)
        {
            foreach (var pool in poisonPool)
            {
                pool.Update(gameTime);
                if (pool.collision.Intersects(player.collision) && pool.canBurn)
                {
                    pool.Burn();
                    Common1 poolTemp = new Common1(new(Globals.graphicsDevice,1,1),Vector2.Zero,1,1,1,1);
                    player.Status.TakeDamage(10,poolTemp);
                }

                if(pool.poolDuration > 5f)
                {
                    poisonPool.Remove(pool);
                    break;
                }
            }
        }
        private void UpdateBoss(GameTime gameTime)
        {
            boss_obj.Update(gameTime);
            if (boss_spawned)
            {

                if (player.collision.Intersects(boss_obj.checkRadius) && !boss_obj.dashed)
                {
                    boss_obj.dashed = true;
                    boss_obj.charge = true;
                    boss_obj.charge_duration = 0;
                    soundManager.PlaySound("Charge");
                }

                

                if (boss_obj.charge)
                {
                    boss_obj.charge_duration += TimeManager.TimeGlobal;
                    if (boss_obj.charge_duration >= 2f)
                    {
                        soundManager.StopSound("Charge");
                        boss_obj.isAttacking = true;
                        soundManager.PlaySound("Beep");
                    }
                    if (boss_obj.charge_duration >= 3f)
                    {
                        boss_obj.Dash(player);
                        boss_obj.charge = false;
                        boss_obj.isAttacking = false;
                        
                    }
                }
                if (!boss_obj.dashing && !boss_obj.charge && !boss_obj.stunned && boss_obj.Status.IsAlive())
                {
                    boss_obj.movementAI.Start();
                    boss_obj.indicator = "Idle";
                }
            }

            
            
        }

        private void EnemyManage()
        {
            //Timer
            boss_spawn_time += TimeManager.TimeGlobal;
            enemy_spawn_time += TimeManager.TimeGlobal;
            elite_spawn_time += TimeManager.TimeGlobal;
            special_spawn_time += TimeManager.TimeGlobal;
            if (boss_spawned)
            {
                if (boss_obj.dashed)
                {
                    boss_dash_cooldown += TimeManager.TimeGlobal;
                }
                if (boss_dash_cooldown > 15f)
                {
                    boss_obj.dashed = false;
                    boss_dash_cooldown = 0;
                }
            }

            //Common1
            if (enemy_spawn_time >= 0.1f)
            {
                if (enemy_count < enemy_max)
                {

                    int spawnDirection = Globals.random.Next(1, 5);

                    Vector2 spawnpoint = Vector2.Zero;
                    int spawnExpand = 100;
                    if (spawnDirection == 1)
                    {
                        spawnpoint = new(Globals.random.Next(0, 1920), 0-spawnExpand);
                    }
                    else if (spawnDirection == 2)
                    {
                        spawnpoint = new(Globals.random.Next(0, 1920), 1080+spawnExpand);
                    }
                    else if (spawnDirection == 3)
                    {
                        spawnpoint = new(0-spawnExpand, Globals.random.Next(0, 1080));
                    }
                    else if (spawnDirection == 4)
                    {
                        spawnpoint = new(1920+spawnExpand, Globals.random.Next(0, 1080));
                    }


                    Common1 enemy = (new Common1(
                        Globals.Content.Load<Texture2D>("Enemies/Common1"),
                        spawnpoint,
                        health: 2000,
                        attack: 100,
                        row: 2,
                        column: 8
                        )
                    {
                        movementAI = new FollowMovementAI
                        {
                            target = player,
                        }
                    });
                    commonEnemy.Add(enemy);
                    Globals.EnemyManager.Add(enemy);
                    damageNumberManager.SubscribeToTakeDamageEvent(enemy.Status, enemy);
                    enemy_spawn_time = 0;
                    enemy_count++;
                }
            }
            foreach (var enemy in commonEnemy)
            {
                if (!enemy.Status.IsAlive())
                {
                    
                    enemy.Die();
                    if (enemy.animations["Die"].IsComplete)
                    {
                        enemy_killed++;
                        enemy_count--;
                        if (enemy_killed % 40 == 0)
                        {
                            player.Status.AddShield(200);
                        }
                        damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
                        Globals.EnemyManager.Remove(enemy);
                        commonEnemy.Remove(enemy);
                        break; //Don't remove. If remove = crash
                    }
                }
                
            }

            //Elite1
            if (elite_spawn_time > 15f && !boss_spawned)
            {
                if (elite_count < elite_max)
                {
                    Elite1 enemy = (new Elite1(
                        Globals.Content.Load<Texture2D>("Enemies/Elite1"),
                        new(1920 / 2, 0),
                        health: 20000,
                        attack: 1500,
                        row: 4,
                        column: 1
                        )
                    {
                        movementAI = new FollowMovementAI
                        {
                            target = player,
                        }
                    });
                    eliteEnemy.Add(enemy);
                    Globals.EnemyManager.Add(enemy);
                    damageNumberManager.SubscribeToTakeDamageEvent(enemy.Status, enemy);

                    elite_spawn_time = 0f;
                }
            }
            foreach (var enemy in eliteEnemy)
            {
                if (!enemy.Status.IsAlive())
                {
                    
                    enemy.Die();
                    if (enemy.animations["Die"].IsComplete)
                    {
                        elite_count--;
                        damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
                        Globals.EnemyManager.Remove(enemy);
                        eliteEnemy.Remove(enemy);
                        break;
                    }
                }
            }

            //Boss1
            if (boss_spawn_time > 135f && !boss_spawned || Keyboard.GetState().IsKeyDown(Keys.K) && !boss_spawned)
            {
                special_spawn_time = 10f;
                boss_obj = new Boss1(Globals.Content.Load<Texture2D>("Enemies/Boss1"), new(100000, 500), health: 100000, attack: 3000, row: 3, column: 8)
                {
                    movementAI = new FollowMovementAI
                    {
                        target = player,
                    },
                };
                if (player.position.X > Globals.Bounds.X / 2)
                {
                    boss_obj.position = new Vector2(150, boss_obj.position.Y);
                }
                if (player.position.X <= Globals.Bounds.X / 2)
                {
                    boss_obj.position = new Vector2(1920 - 150, boss_obj.position.Y);
                }
                
                boss_spawned = true;
                
                commonEnemy.Clear();
                eliteEnemy.Clear();
                Globals.EnemyManager.Clear();
                enemy_count = 0;
                enemy_max = 5;
                Globals.EnemyManager.Add(boss_obj);
                damageNumberManager.SubscribeToTakeDamageEvent(boss_obj.Status, boss_obj);


                Texture2D bg = Globals.Content.Load<Texture2D>("UI/BossBarNone");
                Texture2D fg = Globals.Content.Load<Texture2D>("UI/BossBarFull");
                ProgressBarAnimated bossBar = new ProgressBarAnimated(bg, fg, boss_obj.Status.MaxHP, new(Globals.Bounds.X / 2, 100));
                uiManager.AddElement("bossBar", bossBar);
            }

            if (!boss_obj.Status.IsAlive() && boss_spawned)
            {
                boss_obj.Die();
            }

            if (boss_obj.animations["Die"].currentFrame == 15)
            {
                soundManager.StopAllSounds();
                boss_spawned = false;
                boss_obj.position = new(9999, 9999);
                uiManager.RemoveElement("bossBar");
                enemy_max = 0;
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.Status.Kill();
                }
            }


            if (boss_spawned && special_spawn_time > 5f)
            {
                if (special_count < special_max)
                {
                     Special1 enemy = new Special1(
                        Globals.Content.Load<Texture2D>("Enemies/Special1"),
                        new(boss_obj.position.X, boss_obj.position.Y + random.Next(-300, 300)),
                        health: 600,
                        attack: 1,
                        row: 1,
                        column: 4
                        )
                    {
                        movementAI = new FollowMovementAI
                        {
                            target = player,
                        }
                    };
                    specialEnemy.Add(enemy);
                    Globals.EnemyManager.Add(enemy);
                    damageNumberManager.SubscribeToTakeDamageEvent(enemy.Status, enemy);
                    special_count++;
                    special_spawn_time = 0f;
                }
            }
            foreach (var special in specialEnemy)
            {
                if (!special.Status.IsAlive())
                {
                    special.Bomb();
                    if (special.isBombed)
                    {
                        poisonPool.Add(new PoisonPool(
                            Globals.Content.Load<Texture2D>("GroundEffect/PoisonPool"),
                            special.position,
                            1,
                            1
                            ));
                        damageNumberManager.UnSubscribeToTakeDamageEvent(special.Status, special);
                        Globals.EnemyManager.Remove(special);
                        specialEnemy.Remove(special);
                        break;
                    }
                }
            }


        }
        private void UiManage(GameTime gameTime)
        {
            //UI Control
            if (uiManager.CheckCollide(player))
            {
                uiManager.SetAllOpacity(0.5f);
            }
            else if (uiManager.CheckCollide(boss_obj))
            {
                uiManager.SetAllOpacity(0.5f);
            }
            else if (commonEnemy.Any(collidable => uiManager.CheckCollide(collidable)))
            {
                uiManager.SetAllOpacity(0.7f);
            }
            else if (eliteEnemy.Any(collidable => uiManager.CheckCollide(collidable)))
            {
                uiManager.SetAllOpacity(0.7f);
            }
            else if (specialEnemy.Any(collidable => uiManager.CheckCollide(collidable)))
            {
                uiManager.SetAllOpacity(0.7f);
            }
            else { uiManager.SetAllOpacity(1f); }

            uiManager.Update(gameTime);
            uiManager.UpdateBarValue("bossBar", boss_obj.Status.CurrentHP);
            uiManager.UpdateBarValue("playerBar", player.Status.CurrentHP);
        }
        private void CollisionManage(GameTime gameTime)
        {
            foreach (var enemy in Globals.EnemyManager)
            {
                if(enemy.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
                {
                    enemy.TakeDamage(1, player);
                    enemy.Status.immunity = true;
                }
            }
            damageNumberManager.Update();
        }

        private void SceneManage()
        {
            if (!player.Status.IsAlive())
            {
                UnloadScene();
            }
                    
        }

        private void UnloadScene()
        {
            boss_obj.checkRadius = new Rectangle(9999, 9999, 0, 0);
            damageNumberManager.UnSubscribeToTakeDamageEvent(boss_obj.Status, boss_obj);
            damageNumberManager.UnSubscribeToTakeDamageEvent(player.Status, player);
            foreach(var enemy in Globals.EnemyManager)
            {
                damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
            }
            Globals.EnemyManager.Clear();
            player = null;
            inventory.Items.Clear();
            commonEnemy.Clear();
            eliteEnemy.Clear();
            specialEnemy.Clear();
            poisonPool.Clear();
            TimeManager.ChangeGameSpeed(1);
            AllSkills.Reset();
            ParticleManager.RemoveAll();
            sceneManager.RemoveScene(this);
        }

    }
}
