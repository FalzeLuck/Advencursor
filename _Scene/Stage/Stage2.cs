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

namespace Advencursor._Scene.Stage
{
    public class Stage2 : IScene
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
        List<Elite2> eliteEnemy;
        private Boss2 boss_obj;
        List<Item> items;



        private readonly Timer timer;
        private Texture2D background;


        //Stage Timer & Controls
        private float boss_spawn_time;
        private float boss_dash_cooldown;
        private bool boss_spawned;
        private float enemy_spawn_time;
        private float elite_spawn_time;
        private float special_spawn_time;
        private int enemy_count = 0;
        private int elite_count = 0;
        private int enemy_max = 30;
        private int elite_max = 2;
        private int enemy_killed = 0;

        //UI
        private UIManager uiManager = new UIManager();


        public Stage2(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            Globals.Game.IsMouseVisible = false;
            font = Globals.Content.Load<SpriteFont>("basicFont");

            damageNumberManager = new(Globals.Content.Load<SpriteFont>("Font/DamageNumber"));

            timer = new(Globals.Content.Load<Texture2D>("TestUI"),
                font,
                new(1920 / 2, 0)
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
            background = Globals.Content.Load<Texture2D>("Background/BG_Stage2");

            //Player
            Texture2D playertexture = Globals.Content.Load<Texture2D>("playerTexture");
            player = new(playertexture, new Vector2(Globals.Bounds.X / 2, Globals.Bounds.Y / 2), 1, 1, 1, 1);
            player.LoadPlayer(2, 1);
            inventory.LoadInventory(tempTexture);
            damageNumberManager.SubscribeToTakeDamageEvent(player.Status, player);

            //Load enemies
            commonEnemy = new List<Common1>();
            eliteEnemy = new List<Elite2>();

            boss_obj = new Boss2(Globals.Content.Load<Texture2D>("Enemies/Boss1"), new Vector2(Globals.Bounds.X / 2, -200), 250000, 5000, 3, 8)
            {
                movementAI = new FollowMovementAI()
            };


            //Load Animation
            Animation slashAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/SlashTexture"), row: 1, column: 1, fps: 8, false, 1.5f);
            animationManager.AddAnimation("Slash", slashAnimation);




            //Load UI
            UIBackground uIBackground = new(Globals.Content.Load<Texture2D>("UI/SkillBackground"), new(Globals.Bounds.X / 2, 930));
            int startX = (int)uIBackground.position.X - (uIBackground.texture.Width / 2);
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


            uiManager.AddElement("uiBackground", uIBackground);
            uiManager.AddElement("playerBar", playerHpBar);
            uiManager.AddElement("skillUI_Q", skillUI_Q);
            uiManager.AddElement("skillUI_W", skillUI_W);
            uiManager.AddElement("skillUI_E", skillUI_E);
            uiManager.AddElement("skillUI_R", skillUI_R);

            //Load Sound
            SoundEffect beep = Globals.Content.Load<SoundEffect>("Sound/Beep");
            soundManager.LoadSound("Beep", beep);
            SoundEffect charge = Globals.Content.Load<SoundEffect>("Sound/Boss1Charge");
            soundManager.LoadSound("Charge", charge);
            soundManager.SetVolume("Charge", 0.2f);
            SoundEffect parry = Globals.Content.Load<SoundEffect>("Sound/Parry");
            soundManager.LoadSound("Parry", parry);
            Mouse.SetPosition(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
        }

        public void Update(GameTime gameTime)
        {
            EnemyManage();

            //Boss Control
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

            timer.Update();
            player.Update(gameTime);
            animationManager.Update(gameTime);
            animationManager.UpdatePosition("Slash", player.position);
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
                    if (animationManager.IsComplete("Slash"))
                    {
                        elite.Status.immunity = false;
                    }
                }


            }
        }
        private void UpdateBoss(GameTime gameTime)
        {
            

        }

        private void EnemyManage()
        {
            //Timer
            boss_spawn_time += TimeManager.TimeGlobal;
            enemy_spawn_time += TimeManager.TimeGlobal;
            elite_spawn_time += TimeManager.TimeGlobal;
            special_spawn_time += TimeManager.TimeGlobal;

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
                        health: 3000,
                        attack: 300,
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

            //Elite
            if (elite_spawn_time > 10f)
            {
                if (elite_count < elite_max)
                {
                    for (int i = 0; i < 2 && i < elite_max; i++)
                    {
                        int spawnDirection = Globals.random.Next(1, 5);
                        Vector2 spawnpoint = Vector2.Zero;
                        int spawnExpand = 100;
                        if (spawnDirection == 1 && player.position.Y > Globals.Bounds.Y/2)
                        {
                            spawnpoint = new(Globals.random.Next(0, 1920), 0 - spawnExpand);
                        }
                        else if (spawnDirection == 2 && player.position.Y < Globals.Bounds.Y/2)
                        {
                            spawnpoint = new(Globals.random.Next(0, 1920), 1080 + spawnExpand);
                        }
                        else if (spawnDirection == 3 && player.position.X > Globals.Bounds.X/2)
                        {
                            spawnpoint = new(0 - spawnExpand, Globals.random.Next(0, 1080));
                        }
                        else if (spawnDirection == 4 && player.position.X < Globals.Bounds.X / 2)
                        {
                            spawnpoint = new(1920 + spawnExpand, Globals.random.Next(0, 1080));
                        }
                        
                        Elite2 enemy = (new Elite2(
                            Globals.Content.Load<Texture2D>("Enemies/Elite1"),
                            spawnpoint,
                            health: 10000,
                            attack: 1,
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
                        elite_count++;
                        elite_spawn_time = 0f;
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
                        player.Status.SetCritRate(player.Status.CritRate + 4);
                        damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
                        Globals.EnemyManager.Remove(enemy);
                        eliteEnemy.Remove(enemy);
                        break;
                    }
                }
            }

            //Boss1
            if (boss_spawn_time > 120f && !boss_spawned || Keyboard.GetState().IsKeyDown(Keys.K) && !boss_spawned)
            {
                boss_obj = new Boss2(Globals.Content.Load<Texture2D>("Enemies/Boss1"), new Vector2(Globals.Bounds.X / 2, -200), 250000, 5000, 3, 8)
                {
                    movementAI = new FollowMovementAI()
                    {
                        target = player
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
                UnloadScene();
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

        private void SceneManage()
        {
            if (!player.Status.IsAlive())
            {
                UnloadScene();
            }

        }

        private void UnloadScene()
        {
            damageNumberManager.UnSubscribeToTakeDamageEvent(boss_obj.Status, boss_obj);
            damageNumberManager.UnSubscribeToTakeDamageEvent(player.Status, player);
            foreach (var enemy in Globals.EnemyManager)
            {
                damageNumberManager.UnSubscribeToTakeDamageEvent(enemy.Status, enemy);
            }
            Globals.EnemyManager.Clear();
            inventory.Items.Clear();
            commonEnemy.Clear();
            eliteEnemy.Clear();
            TimeManager.ChangeGameSpeed(1);
            AllSkills.Reset();
            ParticleManager.RemoveAll();
            sceneManager.AddScene(new StageSelectScene(contentManager, sceneManager));
        }
    }
}
