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


namespace Advencursor._Scene.Stage
{
    public class Stage1 : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private AnimationManager animationManager = new AnimationManager();
        private UIManager uiManager = new UIManager();
        private SoundManager soundManager = new SoundManager();

        private SpriteFont font;


        private Player player;

        List<_Enemy> enemies;
        private Boss1 boss_obj;
        List<Item> items;


        private readonly Timer timer;
        private Texture2D background;

        private readonly Random random = new Random();


        //Stage Timer & Controls
        private float clickCD;
        private float immune_duration;
        private float boss_spawn_time;
        private float boss_dash_cooldown;
        private bool boss_spawned;
        private float enemy_spawn_time;
        private int enemy_count;
        private int enemy_max = 20;

        public Stage1(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            Globals.Game.IsMouseVisible = false;
            Mouse.SetPosition(Globals.Bounds.X / 2, Globals.Bounds.Y / 2);
            font = Globals.Content.Load<SpriteFont>("basicFont");

            timer = new(Globals.Content.Load<Texture2D>("TestUI"),
                font,
                new(0, 0)
                );

            timer.StartStop();
            timer.Repeat = true;
            boss_spawn_time = 0f;
            boss_spawned = false;
        }


        public void Load()
        {
            //Load Background
            background = Globals.Content.Load<Texture2D>("background");

            //Player
            player = new(Globals.Content.Load<Texture2D>("playerTexture"), new(1000, 1000), 10,2,1);

            //Load enemies(Temp)
            enemies = new List<_Enemy> ();
            

            boss_obj = new Boss1(Globals.Content.Load<Texture2D>("Enemies/Boss1"), new(960, 0), 10, 2, 1)
            {
                movementAI = new FollowMovementAI
                {
                    target = player,
                }
            };

            //Load Animation
            Animation slashAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/SlashTexture"), row: 1, column: 4, fps: 12, false);
            animationManager.AddAnimation("Slash", slashAnimation);
            Animation sparkAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/Sparkle"), row: 1, column: 4, fps: 12, false);
            animationManager.AddAnimation("Sparkle", sparkAnimation);


            //Temporary Skill
            Skill fire = new Skill("fireball", 30, 5);
            items = new List<Item>()
            {
                new Item("fireball book",fire,Keys.Q),
            };


            //Load UI
            int space = 1920 / 15;
            UISkill skillUI_Q = new(Globals.Content.Load<Texture2D>("TestUI"), new(space * 4, 1000), fire);
            UISkill skillUI_W = new(Globals.Content.Load<Texture2D>("TestUI"), new(space * 6, 1000), fire);
            UISkill skillUI_E = new(Globals.Content.Load<Texture2D>("TestUI"), new(space * 8, 1000), fire);
            UISkill skillUI_R = new(Globals.Content.Load<Texture2D>("TestUI"), new(space * 10, 1000), fire);
            UIPanel uIPanel = new(Globals.Content.Load<Texture2D>("TestUI"),new(200,200),player);
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
            if (boss_obj.dashed)
            {
                boss_dash_cooldown += TimeManager.TotalSeconds;
            }
            if (boss_dash_cooldown > 10f)
            {
                boss_obj.dashed = false;
                boss_dash_cooldown = 0;
            }

            //Spawning
            if (enemy_spawn_time >= 2f && !boss_spawned)
            {
                enemies.Add(new Kiki(
                    Globals.Content.Load<Texture2D>("Enemies/Kiki"),
                    new(0, random.Next(200, 900)),
                    health: 3,
                    row: 2,
                    column: 2
                    )
                {
                    movementAI = new FollowMovementAI
                    {
                        target = player,
                    }
                });
                enemy_spawn_time = 0;
            }
            if(boss_spawn_time > 1f && !boss_spawned)
            {
                boss_spawned = true;
            }

            //Boss Control
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
            if (!boss_obj.Status.IsAlive())
            {
                boss_obj.checkRadius = new Rectangle(9999, 9999, 0, 0);
            }


            //Parry Control
            if (player.TryParryAttack(boss_obj))
            {
                boss_obj.Stun(2f);
                boss_obj.isAttacking = false;
                boss_obj.charge = false ;
                player.Status.immunity = true;
                boss_obj.Status.TakeDamage(5);
                soundManager.PlaySound("Parry");
                animationManager.SetOffset("Sparkle", new Vector2(0, 0));
                animationManager.UpdatePosition("Sparkle", player.position);
                animationManager.Play("Sparkle");
            }
            foreach (var enemy in enemies)
            {
                if (player.TryParryAttack(enemy))
                {
                    animationManager.SetOffset("Sparkle", new Vector2(0, 0));
                    animationManager.UpdatePosition("Sparkle", player.position);
                    animationManager.Play("Sparkle");
                    enemies.Remove(enemy);
                    break;
                }
            }

            //Mash Update
            timer.Update();
            player.Update(gameTime);
            animationManager.Update(gameTime);
            animationManager.UpdatePosition("Slash",player.position);
            ParticleManager.Update();
            uiManager.Update(gameTime);
            UpdatePlayer();
            UpdateEnemies(gameTime);

            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(background, Vector2.Zero, Color.Gray);
            timer.Draw();
            uiManager.Draw(spriteBatch);
            foreach (var enemy in enemies)
            {
                enemy.Draw();
            }
            if (boss_spawned)
            {
                boss_obj.Draw();
            }
            player.Draw();
            ParticleManager.Draw();
            animationManager.Draw();
        }

        private void UpdatePlayer()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                player.UseSkill(Keys.Q);
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


            if (InputManager.MouseLeftClicked)
            {
                animationManager.SetOffset("Slash", new Vector2(0, 0));
                player.indicator = "Attack";
                animationManager.Play("Slash");
            }

            if (animationManager.IsComplete("Slash"))
            {
                foreach (var enemy in enemies)
                {
                    if (enemy.Status.immunity)
                    {
                        enemy.Status.immunity = false;
                    }
                }
                player.indicator = "Idle";
                animationManager.Stop("Slash");
            }

            if (animationManager.IsComplete("Sparkle"))
            {

                animationManager.Stop("Sparkle");
            }

            foreach (var item in items)
            {
                player.EquipItem(item);
                item.skill.Update(TimeManager.TotalSeconds);
            }

        }
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime);
                if (enemy.collision.Intersects(animationManager.GetCollision("Slash", player.position)) && animationManager.IsCollision("Slash"))
                {
                    enemy.Status.TakeDamage(1);
                    enemy.Status.immunity = true;
                    enemy.movementAI.Stop();
                    enemy.recovery_time = 0;
                }

                if (!enemy.Status.IsAlive())
                {
                    enemy.position = new Vector2(2000, 2000);
                    enemies.Remove(enemy);
                    break; //Don't remove. If remove = crash
                }

                if (enemy.collision.Intersects(player.collision))
                {
                    enemy.indicator = "Attack";
                    player.Status.TakeDamage(1);
                    player.Status.immunity = true;
                    enemy.movementAI.Stop();
                    enemy.recovery_time = 0;
                }
                if (enemy.recovery_time > 1f)
                {
                    enemy.indicator = "Idle";
                    enemy.movementAI.Start();
                }

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
        }
    }
}
