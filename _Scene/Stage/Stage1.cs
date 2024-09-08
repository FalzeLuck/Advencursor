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


namespace Advencursor._Scene.Stage
{
    public class Stage1 : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private AnimationManager animationManager = new AnimationManager();
        private SoundManager soundManager = new SoundManager();

        private SpriteFont font;


        private Player player;

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
        private bool canClick = true;
        private float immune_duration;
        private float boss_spawn_time;
        private float boss_dash_cooldown;
        private bool boss_spawned;
        private float enemy_spawn_time;
        private float elite_spawn_time;
        private float special_spawn_time;
        private int enemy_count = 0;
        private int elite_count = 0;
        private int special_count = 0;
        private int enemy_max = 20;
        private int elite_max = 2;
        private int special_max = 2;
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
            //Load Background
            background = Globals.Content.Load<Texture2D>("Background/Stage1_5");

            //Player
            player = new(Globals.Content.Load<Texture2D>("playerTexture"), new(1000, 1000), health: 1000, attack: 50, row: 4, column: 1);

            //Load enemies(Temp)
            commonEnemy = new List<Common1> ();
            eliteEnemy = new List<Elite1> ();
            specialEnemy = new List<Special1> ();
            poisonPool = new List<PoisonPool> ();



            boss_obj = new Boss1(Globals.Content.Load<Texture2D>("Enemies/Boss1"), new(100000, 500), health: 4000,attack:2,row: 3,column: 1)
            {
                movementAI = new FollowMovementAI
                {
                    target = player,
                }
            };

            //Load Animation
            Animation slashAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/SlashTexture"), row: 1, column: 8, fps: 30, false);
            animationManager.AddAnimation("Slash", slashAnimation);
            Animation sparkAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/Sparkle"), row: 1, column: 4, fps: 12, false);
            animationManager.AddAnimation("Sparkle", sparkAnimation);


            //Temporary Skill
            Skill_Q_ThunderCore ThunderCore = new Skill_Q_ThunderCore("Thunder Core", 5);
            Skill_W_ThunderShuriken ThunderShuriken = new Skill_W_ThunderShuriken("Thunder Shuriken", 10);
            items = new List<Item>()
            {
                new Item("ThunderCore book",ThunderCore,Keys.Q),
                new Item("ThunderShuriken book",ThunderShuriken,Keys.W),
            };


            //Load UI
            UIBackground uIBackground = new(Globals.Content.Load<Texture2D>("UI/SkillBackground"), new(Globals.Bounds.X / 2, 930));
            int startX = (int)uIBackground.position.X - (uIBackground.texture.Width/2);
            int space = uIBackground.texture.Width / 10;
            int skillY = 950;
            UISkill skillUI_Q = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 2), skillY), ThunderCore);
            UISkill skillUI_W = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 4), skillY), ThunderShuriken);
            UISkill skillUI_E = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 6), skillY), ThunderCore);
            UISkill skillUI_R = new(Globals.Content.Load<Texture2D>("UI/SkillUI"), new(startX + (space * 8), skillY), ThunderCore);
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
            //Timer
            boss_spawn_time += TimeManager.TotalSeconds;
            enemy_spawn_time += TimeManager.TotalSeconds;
            if (!boss_obj.stunned)
            {
                elite_spawn_time += TimeManager.TotalSeconds;
            }
            special_spawn_time += TimeManager.TotalSeconds;
            if (boss_obj.dashed)
            {
                boss_dash_cooldown += TimeManager.TotalSeconds;
            }
            if (boss_dash_cooldown > 15f)
            {
                boss_obj.dashed = false;
                boss_dash_cooldown = 0;
            }

            //Spawning
            if (enemy_spawn_time >= 0.1f && !boss_spawned)
            {
                if (enemy_count < enemy_max)
                {

                    int spawnDirection = random.Next(1, 3);
                    int spawnSide = 0;
                    if (spawnDirection == 1)
                    {
                        spawnSide = 100;
                    }
                    else if (spawnDirection == 2)
                    {
                        spawnSide = 1820;
                    }
                    commonEnemy.Add(new Common1(
                        Globals.Content.Load<Texture2D>("Enemies/Common1"),
                        new(spawnSide, random.Next(200, 900)),
                        health: 200,
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
                    enemy_spawn_time = 0;
                    enemy_count++;
                }
            }
            if (elite_spawn_time > 15f && !boss_spawned)
            {
                eliteEnemy.Add(new Elite1(
                    Globals.Content.Load<Texture2D>("Enemies/Elite1"),
                    new(1920/2, 0),
                    health: 1000,
                    attack: 1,
                    row: 4,
                    column: 1
                    )
                {
                    movementAI = new FollowMovementAI
                    {
                        target = player,
                    }
                });

                elite_spawn_time = 0f;
            }
            if(boss_spawn_time > 135f && !boss_spawned || Keyboard.GetState().IsKeyDown(Keys.K) && !boss_spawned)
            {
                special_spawn_time = 10f;
                if(player.position.X > Globals.Bounds.X / 2)
                {
                    boss_obj.position = new Vector2(150,boss_obj.position.Y);
                }
                if (player.position.X <= Globals.Bounds.X / 2)
                {
                    boss_obj.position = new Vector2(1920-150, boss_obj.position.Y);
                }
                boss_spawned = true;
                commonEnemy.Clear();
                eliteEnemy.Clear();


                Texture2D bg = Globals.Content.Load<Texture2D>("UI/BossBarNone");
                Texture2D fg = Globals.Content.Load<Texture2D>("UI/BossBarFull");
                ProgressBarAnimated bossBar = new ProgressBarAnimated(bg,fg,boss_obj.Status.MaxHP,new(Globals.Bounds.X/2,100));
                uiManager.AddElement("bossBar",bossBar);
            }
            if (boss_spawned && special_spawn_time > 10f)
            {
                if(special_count < special_max)
                {
                    specialEnemy.Add(new Special1(
                        Globals.Content.Load<Texture2D>("Enemies/Special1"),
                        new(boss_obj.position.X, boss_obj.position.Y + random.Next(100, 300)),
                        health: 600,
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
                    special_count++;
                    special_spawn_time = 0f;
                }
            }

            //Boss Control
            UpdateBoss(gameTime);

            


            //Parry Control
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
                    elite.Status.TakeDamage(player.Status.Attack);
                }
            }

            //UI Control
            if (uiManager.CheckCollide(player))
            {
                uiManager.SetAllOpacity(0.5f);
            }
            else if (uiManager.CheckCollide(boss_obj))
            {
                uiManager.SetAllOpacity(0.5f);
            }
            else if(commonEnemy.Any(collidable => uiManager.CheckCollide(collidable)))
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
            
            

            //Mash Update
            timer.Update();
            player.Update(gameTime);
            animationManager.Update(gameTime);
            animationManager.UpdatePosition("Slash",player.position);
            ParticleManager.Update();
            uiManager.Update(gameTime);
            uiManager.UpdateBarValue("bossBar", boss_obj.Status.CurrentHP);
            uiManager.UpdateBarValue("playerBar", player.Status.CurrentHP);
            UpdatePlayer();
            UpdateEnemies(gameTime);
            UpdateElites(gameTime);
            UpdateSpecial(gameTime);
            UpdatePoisonPool(gameTime);
            
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
            ParticleManager.Draw();
            animationManager.Draw();
        }

        private void UpdatePlayer()
        {
            if (!player.isStun)
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


                if (InputManager.MouseRightClicked && canClick)
                {
                    animationManager.SetOffset("Slash", new Vector2(player.collision.Width / 2, 0));
                    player.ChangeAnimation("Attack");
                    animationManager.Play("Slash");
                    canClick = false;
                }
                if (InputManager.MouseLeftClicked && canClick)
                {
                    animationManager.SetOffset("Slash", new Vector2(-player.collision.Width / 2, 0));
                    player.ChangeAnimation("Attack");
                    animationManager.Play("Slash");
                    canClick = false;
                }
            }

            if (animationManager.IsComplete("Slash"))
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
                canClick = true;
            }

            if (animationManager.IsComplete("Sparkle"))
            {
                animationManager.Stop("Sparkle");
            }

            foreach (var item in items)
            {
                player.EquipItem(item);
            }

            if (player.Status.immunity)
            {
                immune_duration += TimeManager.TotalSeconds;

                if (immune_duration > 1f)
                {
                    player.Status.immunity = false;
                    immune_duration = 0f;
                }
            }

            if (!player.Status.IsAlive())
            {
                sceneManager.RemoveScene(this);
            }
        }
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (var enemy in commonEnemy)
            {
                enemy.Update(gameTime);
                if (enemy.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
                {
                    enemy.TakeDamage(player.Status.Attack, player);
                    enemy.Status.immunity = true;
                }

                if (enemy.dashRadius.Intersects(player.collision))
                {
                    enemy.Dash(player);
                }

                if(enemy.collision.Intersects(player.collision) && enemy.isDashing)
                {
                    player.Status.TakeDamage(50);
                    player.Status.immunity = true;
                }

                if (!enemy.Status.IsAlive())
                {
                    enemy_killed++;
                    enemy_count--;
                    if (enemy_killed % 40 == 0)
                    {
                        player.Status.AddShield(200);
                    }
                    enemy.position = new Vector2(2000, 2000);
                    commonEnemy.Remove(enemy);
                    break; //Don't remove. If remove = crash
                }

            }
            
        }
        private void UpdateElites(GameTime gameTime)
        {
            foreach (var elite in eliteEnemy)
            {
                elite.Update(gameTime);

                if (elite.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
                {
                    elite.Status.TakeDamage(player.Status.Attack);
                    elite.Status.immunity = true;
                }
                if (elite.slamRadius.Intersects(player.collision) && !elite.isSlam)
                {
                    elite.Slam();
                }

                if(elite.isSlamming && elite.slamRadius.Intersects(player.collision))
                {
                    player.Stun(2);
                    player.Status.TakeDamage(300);
                    player.Status.immunity = true;
                }

                if (animationManager.IsComplete("Slash"))
                {
                    elite.Status.immunity = false;
                }

                if (!elite.Status.IsAlive())
                {
                    player.Status.AddAttack(20);
                    eliteEnemy.Remove(elite);
                    break;
                }
            }
        }
        private void UpdateSpecial(GameTime gameTime)
        {
            foreach (var special in specialEnemy)
            {
                special.Update(gameTime);

                if (special.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
                {
                    special.Status.TakeDamage(player.Status.Attack);
                    special.Status.immunity = true;
                }

                if (animationManager.IsComplete("Slash"))
                {
                    special.Status.immunity = false;
                }

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

                        specialEnemy.Remove(special);
                        break;
                    }
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
                    player.Status.TakeDamage(10);
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
            if (boss_spawned && boss_obj.Status.IsAlive())
            {
                boss_obj.Update(gameTime);
            }

            if (player.collision.Intersects(boss_obj.checkRadius) && !boss_obj.dashed)
            {
                boss_obj.dashed = true;
                boss_obj.charge = true;
                boss_obj.charge_duration = 0;
                soundManager.PlaySound("Charge");
            }

            if (boss_obj.charge)
            {
                boss_obj.movementAI.Stop();
                boss_obj.charge_duration += TimeManager.TotalSeconds;
                if (boss_obj.charge_duration >= 2f)
                {
                    soundManager.StopSound("Charge");
                    boss_obj.isAttacking = true;
                    soundManager.PlaySound("Beep");

                }
                if (boss_obj.charge_duration >= 3f)
                {
                    boss_obj.charge = false;
                    boss_obj.isAttacking = false;
                    boss_obj.Dash(player);
                }
            }
            if (!boss_obj.dashing && !boss_obj.charge && !boss_obj.stunned)
            {
                boss_obj.movementAI.Start();
                boss_obj.indicator = "Idle";
            }
            if (boss_obj.collision.Intersects(player.collision) && boss_obj.dashing)
            {
                player.Status.TakeDamage(500);
                player.Status.immunity = true;
            }
            if (boss_obj.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
            {
                boss_obj.Status.TakeDamage(player.Status.Attack);
                boss_obj.Status.immunity = true;
            }
            if (!boss_obj.Status.IsAlive())
            {
                boss_obj.checkRadius = new Rectangle(9999, 9999, 0, 0);
                sceneManager.RemoveScene(this);
            }
        }
    }
}
