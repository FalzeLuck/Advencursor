using Advencursor._Managers;
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
using Advencursor._Models.Enemy.Stage1;


namespace Advencursor._Scene.Stage
{
    public class Stage1 : StageAbstract
    {
        private List<Elite1> eliteEnemy;
        private List<Special1> specialEnemy;
        private List<PoisonPool> poisonPool;
        private Boss1 boss_obj;

        protected float special_spawn_time;
        protected int special_count = 0;
        protected int special_max = 3;
        protected float elite_reset_time = 10f;


        public Stage1(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            Globals.Game.IsMouseVisible = false;
            font = Globals.Content.Load<SpriteFont>("basicFont");

            damageNumberManager = new(Globals.Content.Load<SpriteFont>("Font/DamageNumber"));

            timer = new(Globals.Content.Load<Texture2D>("TestUI"),
                font,
                new(0, 0)
                );
            isPause = false;
            timer.StartStop();
            timer.Repeat = true;
            boss_spawn_time = 0f;
            elite_spawn_time = 0f;
            enemy_spawn_time = 0f;
            boss_spawned = false;
        }


        public override void Load()
        {
            base.Load();
            Texture2D tempTexture = new Texture2D(Globals.graphicsDevice, 1, 1);

            //Load Background
            background = Globals.Content.Load<Texture2D>("Background/BG_Stage1");

            //Load enemies
            commonEnemy = new List<Common1>();
            eliteEnemy = new List<Elite1>();
            specialEnemy = new List<Special1>();
            poisonPool = new List<PoisonPool>();


            //Boss
            boss_obj = new Boss1(Globals.Content.Load<Texture2D>("Enemies/Boss1"), new(100000, 500), health: 100000, attack: 3000, row: 2, column: 8)
            {

            };
        }

        public override void Update(GameTime gameTime)
        {
            CheckPause(gameTime);

            if (!isPause)
            {
                EnemyManage();
                UpdateBoss(gameTime);
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.Update(gameTime);
                }
                UiManage(gameTime);
                CollisionManage(gameTime);
                UpdatePlayer();
                UpdateEnemies(gameTime);
                UpdateElites(gameTime);
                UpdateSpecial(gameTime);
                UpdatePoisonPool(gameTime);
                timer.Update();
                player.Update(gameTime);
                animationManager.Update(gameTime);
                animationManager.UpdatePosition("Slash", player.position);
                ParticleManager.Update();
                SceneManage();
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            if (startWarning)
            {
                DrawWarning();
            }
            timer.Draw();
            foreach (var pool in poisonPool)
            {
                pool.Draw();
            }
            foreach (var special in specialEnemy)
            {
                special.Draw();
            }
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
            player.Draw();
            animationManager.Draw();
            ParticleManager.Draw();
            damageNumberManager.Draw();
            uiManager.Draw(spriteBatch);
            
            if (isPause)
            {
                DrawPause();
            }

        }
        protected override void UpdatePlayer()
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

                if (player.CanNormalAttack() || animationManager.animations["Slash"].IsComplete)
                {
                    foreach (var enemy in commonEnemy)
                    {
                        if (enemy.Status.immunity)
                        {
                            enemy.Status.immunity = false;
                        }
                    }
                    foreach (var elite in eliteEnemy)
                    {
                        if (elite.Status.immunity)
                        {
                            elite.Status.immunity = false;
                        }
                    }
                    foreach (var enemy in specialEnemy)
                    {
                        if (enemy.Status.immunity)
                        {
                            enemy.Status.immunity = false;
                        }
                    }
                    boss_obj.Status.immunity = false;
                    player.ChangeAnimation("Idle");
                    animationManager.Stop("Slash");
                }

                if (InputManager.MouseRightClicked && player.CanNormalAttack())
                {
                    animationManager.SetOffset("Slash", new Vector2(player.collision.Width / 2, 0));
                    player.ChangeAnimation("Attack", true);
                    animationManager.Flip("Slash", true);
                    animationManager.Play("Slash");
                    player.DoNormalAttack();
                }
                if (InputManager.MouseLeftClicked && player.CanNormalAttack())
                {
                    animationManager.SetOffset("Slash", new Vector2(-player.collision.Width / 2, 0));
                    player.ChangeAnimation("Attack", false);
                    animationManager.Flip("Slash", false);
                    animationManager.Play("Slash");
                    player.DoNormalAttack();
                }

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
                if (elite.Status.IsAlive())
                {
                    if (elite.slamRadius.Intersects(player.collision) && !elite.isSlam)
                    {
                        elite.Slam();
                    }

                    if (elite.isSlamming && elite.slamRadius.Intersects(player.collision))
                    {
                        player.Stun(1);
                        player.Status.TakeDamage(3000, elite);
                        player.Immunity(0.5f);
                    }

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
                    Common1 poolTemp = new Common1(new(Globals.graphicsDevice, 1, 1), Vector2.Zero, 1, 1, 1, 1);
                    player.Status.TakeDamage((player.Status.MaxHP*3)/100, poolTemp);
                }

                if (pool.poolDuration > 5f)
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
                    boss_obj.Charge(player);
                }

                if (boss_obj.charge)
                {
                    if (boss_obj.charge_duration >= 3f)
                    {
                        boss_obj.Dash();
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
            elite_reset_time -= TimeManager.TimeGlobal;
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
            if (enemy_spawn_time >= 0.1f && !startWarning) 
            {
                if (enemy_count < enemy_max)
                {

                    int spawnDirection = Globals.random.Next(1, 5);

                    Vector2 spawnpoint = Vector2.Zero;
                    int spawnExpand = 100;
                    if (spawnDirection == 1)
                    {
                        spawnpoint = new(Globals.random.Next(0, 1920), 0 - spawnExpand);
                    }
                    else if (spawnDirection == 2)
                    {
                        spawnpoint = new(Globals.random.Next(0, 1920), 1080 + spawnExpand);
                    }
                    else if (spawnDirection == 3)
                    {
                        spawnpoint = new(0 - spawnExpand, Globals.random.Next(0, 1080));
                    }
                    else if (spawnDirection == 4)
                    {
                        spawnpoint = new(1920 + spawnExpand, Globals.random.Next(0, 1080));
                    }


                    Common1 enemy = (new Common1(
                        Globals.Content.Load<Texture2D>("Enemies/Common1"),
                        spawnpoint,
                        health: 2000,
                        attack: 100,
                        row: 3,
                        column: 8
                        )
                    {
                        movementAI = new FollowMovementAI
                        {
                            target = player,
                        }
                    });
                    enemy.ChangeColor( Color.White );
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
                        damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
                        Globals.EnemyManager.Remove(enemy);
                        commonEnemy.Remove(enemy);
                        break; //Don't remove. If remove = crash
                    }
                }

            }

            //Elite1
            if (elite_spawn_time > 30f && !boss_spawned && elite_reset_time <= 0)
            {
                if (elite_count < elite_max)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float spawnSide = Globals.RandomFloat(0, Globals.Bounds.X);
                        Elite1 enemy = (new Elite1(
                            Globals.Content.Load<Texture2D>("Enemies/Elite1"),
                            new(spawnSide, -100),
                            health: 20000,
                            attack: 1500,
                            row: 3,
                            column: 8
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
                        elite_count++;
                        elite_reset_time = 10f;
                    }
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
                        enemy_killed++;
                        elite_killed++;
                        if (elite_killed < 5)
                        {
                            player.Status.SetCritRate(player.Status.CritRate + 4);
                            player.Status.AddAttack(100);
                        }
                        damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
                        Globals.EnemyManager.Remove(enemy);
                        eliteEnemy.Remove(enemy);
                        break;
                    }
                }
            }
            //Warning
            if (Keyboard.GetState().IsKeyDown(Keys.K) && !boss_spawned)
            {
                boss_spawn_time = 115f;
                timer.TimeSet(115f);
            }
            if (boss_spawn_time > 115f && !boss_spawned)
            {
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.Status.Kill();
                }
                startWarning = true;
                enemy_count = 0;
                enemy_max = 0;
            }

            //Boss1
            if (boss_spawn_time > 120f && !boss_spawned)
            {
                startWarning = false;
                elite_spawn_time = 0f;
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
                       column: 8
                       )
                    {
                        movementAI = new FollowMovementAI
                        {
                            target = player,
                        }
                    };
                    specialEnemy.Add(enemy);
                    Globals.EnemyManager.Add(enemy);
                    special_count++;
                    special_spawn_time = 0f;
                }
            }
            foreach (var special in specialEnemy)
            {
                if (special.isBombed)
                {
                    poisonPool.Add(new PoisonPool(
                        Globals.Content.Load<Texture2D>("GroundEffect/PoisonPool"),
                        special.position,
                        1,
                        6
                        ));
                    
                }
                if (!special.Status.IsAlive())
                {
                    special_count--;
                    Globals.EnemyManager.Remove(special);
                    specialEnemy.Remove(special);
                    break;
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
                if (enemy.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
                {
                    enemy.TakeDamage(1, player);
                    enemy.Status.immunity = true;
                }
            }
            damageNumberManager.Update();
        }

        protected override void SceneManage()
        {
            if (!player.Status.IsAlive())
            {
                GotoSummary(false);
            }


            if (boss_obj.animations["Die"].currentFrame == 15)
            {
                damageNumberManager.UnSubscribeToTakeDamageEvent(boss_obj.Status, boss_obj);
                soundManager.StopAllSounds();
                boss_spawned = false;
                boss_obj.position = new(9999, 9999);
                uiManager.RemoveElement("bossBar");
                enemy_max = 0;
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.Status.Kill();
                }
                boss_killed = true;
                GotoSummary(true);
            }
        }



    }
}
