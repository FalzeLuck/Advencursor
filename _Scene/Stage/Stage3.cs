using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Models.Enemy._CommonEnemy;
using Advencursor._Models;
using Advencursor._Particles;
using Advencursor._Skill;
using Advencursor._UI;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Models.Enemy.Stage1;
using Advencursor._Models.Enemy.Stage2;
using System.Diagnostics;
using System.ComponentModel;

namespace Advencursor._Scene.Stage
{
    public class Stage3 : StageAbstract
    {
        List<Elite1> eliteEnemy;
        List<Elite2> eliteEnemy2;
        private int elite1_killed = 0;
        private float elite1_reset_time = 0;
        private float elite2_reset_time = 0;
        Boss3 boss_obj;
        Boss2 mini_boss1;
        Boss1 mini_boss2;

        public Stage3(ContentManager contentManager, SceneManager sceneManager)
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

            timer.StartStop();
            timer.Repeat = true;
            boss_spawn_time = 0f;
            elite_spawn_time = 0f;
            enemy_spawn_time = 0f;
            boss_spawned = false;
            elite_count = 0;
            enemy_max = 30;
        }


        public override void Load()
        {
            base.Load();
            Texture2D tempTexture = new Texture2D(Globals.graphicsDevice, 1, 1);

            //Load Background
            background = Globals.Content.Load<Texture2D>("Background/BG_Stage2");

            //Load enemies
            commonEnemy = new List<Common1>();
            eliteEnemy = new List<Elite1>();
            eliteEnemy2 = new List<Elite2>();
            boss_obj = new Boss3(Globals.Content.Load<Texture2D>("Enemies/Boss3"), new Vector2(Globals.Bounds.X / 2, -200), 250000, 5000, 6, 12)
            {
                movementAI = new FollowMovementAI()
            };
        }

        public override void Update(GameTime gameTime)
        {
            CheckPause(gameTime);
            if (!isPause)
            {
                EnemyManage();
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.Update(gameTime);
                }
                UiManage(gameTime);
                CollisionManage(gameTime);

                UpdatePlayer();
                UpdateEnemies(gameTime);
                UpdateElites(gameTime);

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
            foreach (var enemy in commonEnemy)
            {
                enemy.Draw();
            }
            if (boss_spawned)
            {
                boss_obj.Draw();
            }
            foreach (var elite in eliteEnemy)
            {
                elite.Draw();
            }
            uiManager.Draw(spriteBatch);
            player.Draw();

            animationManager.Draw();
            ParticleManager.Draw();
            damageNumberManager.Draw();

            if (isPause)
            {
                DrawPause();
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
                        player.Status.TakeDamage(2500, elite);
                        player.Immunity(0.5f);
                    }

                }
            }
        }

        private void EnemyManage()
        {
            //Timer
            boss_spawn_time += TimeManager.TimeGlobal;
            enemy_spawn_time += TimeManager.TimeGlobal;
            elite_spawn_time += TimeManager.TimeGlobal;
            elite1_reset_time -= TimeManager.TimeGlobal;
            elite2_reset_time -= TimeManager.TimeGlobal;

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
                        health: 6000,
                        attack: 200,
                        row: 3,
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
                        damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
                        Globals.EnemyManager.Remove(enemy);
                        commonEnemy.Remove(enemy);
                        break; //Don't remove. If remove = crash
                    }
                }

            }
            //Elite1
            if (elite_spawn_time > 10f && !boss_spawned && elite1_reset_time <= 0)
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
                        elite1_reset_time = 20f;
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
                        elite1_killed++;
                        if (elite1_killed < 5)
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
            //Elite2
            if (elite_spawn_time > 15f && !startWarning && elite2_reset_time <= 0 && !boss_spawned)
            {
                if (elite_count < elite_max)
                {
                    for (int i = 0; i < 2 && i < elite_max; i++)
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

                        Elite2 enemy = (new Elite2(
                            Globals.Content.Load<Texture2D>("Enemies/Elite2"),
                            spawnpoint,
                            health: 10000,
                            attack: 1,
                            row: 3,
                            column: 4
                            )
                        {
                            movementAI = new FollowMovementAI
                            {
                                target = player,
                            }
                        });
                        eliteEnemy2.Add(enemy);
                        Globals.EnemyManager.Add(enemy);
                        elite_count++;
                        elite2_reset_time = 7.5f;
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
                        elite_killed++;
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

            //Boss3
            if (boss_spawn_time > 120f && !boss_spawned)
            {
                startWarning = false;
                elite_spawn_time = 0f;
                boss_obj = new Boss3(Globals.Content.Load<Texture2D>("Enemies/Boss3"), new Vector2(Globals.Bounds.X / 2, -200), 600000, 5000, 6, 12)
                {
                    movementAI = new FollowMovementAI()
                    {
                        target = player,
                    }
                };

                boss_spawned = true;

                commonEnemy.Clear();
                Globals.EnemyManager.RemoveAll(common => common is Common1);
                enemy_count = 0;
                enemy_max = 0;
                Globals.EnemyManager.Add(boss_obj);
                damageNumberManager.SubscribeToTakeDamageEvent(boss_obj.Status, boss_obj);


                Texture2D bg = Globals.Content.Load<Texture2D>("UI/BossBarNone");
                Texture2D fg = Globals.Content.Load<Texture2D>("UI/BossBarFull");
                ProgressBarAnimated bossBar = new ProgressBarAnimated(bg, fg, boss_obj.Status.MaxHP, new(Globals.Bounds.X / 2, 100));
                uiManager.AddElement("bossBar", bossBar);

                boss_obj.Start();
            }

            if (!boss_obj.Status.IsAlive() && boss_spawned)
            {
                boss_obj.Die();
                boss_obj.position = new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
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
            base.SceneManage();
            if (boss_obj.animations["Die"].IsComplete)
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
                GotoSummary(true);
            }
        }
    }
}
