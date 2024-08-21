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

namespace Advencursor._Scene.Stage
{
    public class Stage1 : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private AnimationManager animationManager = new AnimationManager();
        private UIManager uiManager = new UIManager();

        private SpriteFont font;


        private Player player;

        List<_Enemy> enemies;
        private Boss1 boss_obj;
        List<Item> items;

        private readonly Timer timer;
        private Texture2D background;

        //Stage Timer & Controls
        private float clickCD;
        private float immune_duration;
        private float boss_spawn_time;
        private float boss_dash_cooldown;
        private bool boss_spawned;

        public Stage1(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            Globals.Game.IsMouseVisible = false;
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
            player = new(Globals.Content.Load<Texture2D>("playerTexture"), new(1000, 1000), 10,1,1);

            //Load enemies(Temp)
            enemies = new List<_Enemy>
            {
            new Kiki(Globals.Content.Load<Texture2D>("Enemies/Kiki"), new(500, 500), 1, 2, 2)
            {
                movementAI = new FollowMovementAI
                {
                    target = player,
                }
            }
        };

            boss_obj = new Boss1(Globals.Content.Load<Texture2D>("Enemies/Boss1"), new(0, 500), 10, 2, 1)
            {
                movementAI = new FollowMovementAI
                {
                    target = player,
                }
            };

            //Load Animation
            Animation slashAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/SlashTexture"), row: 2, column: 4, fps: 30, false);
            animationManager.AddAnimation("Slash", slashAnimation);


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
        }

        public void Update(GameTime gameTime)
        {
            //Timer
            boss_spawn_time += TimeManager.TotalSeconds;
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
            if(boss_spawn_time > 5f && !boss_spawned)
            {
                enemies.Add(boss_obj);
                boss_spawned = true;
            }

            //Boss Control
            if (player.collision.Intersects(boss_obj.checkRadius) && boss_obj.dashed)
            {
                boss_obj.movementAI.Stop();
                boss_obj.dashed = true;
            }
            else
            {
                boss_obj.movementAI.Start();
            }

            //Mash Update
            timer.Update();
            player.Update(gameTime);
            animationManager.Update(gameTime);
            ParticleManager.Update();
            uiManager.Update(gameTime);
            UpdatePlayer();
            UpdateEnemies(gameTime);

            foreach (var enemy in enemies)
            {
                Trace.WriteLine(boss_obj.checkRadius);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.Gray);
            timer.Draw();
            uiManager.Draw(spriteBatch);
            player.Draw();
            ParticleManager.Draw();
            animationManager.Draw(player.position);
            foreach (var enemy in enemies)
            {
                enemy.Draw();
            }
            
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
                foreach (var enemy in enemies)
                {
                    enemy.movementAI.Start();
                }

            }


            if (InputManager.MouseLeftClicked)
            {
                animationManager.SetOffset("Slash", new Vector2(0, 0));
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
                animationManager.Stop("Slash");
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
                }

                if (!enemy.Status.IsAlive())
                {
                    enemies.Remove(enemy);
                    break; //Don't remove. If remove = crash
                }

                if (enemy.collision.Intersects(player.collision))
                {
                    enemy.indicator = "Attack";
                    player.Status.TakeDamage(1);
                    player.Status.immunity = true;
                    enemy.movementAI.Stop();
                }
                if (immune_duration > 0.25f)
                {
                    enemy.indicator = "Idle";
                    enemy.movementAI.Start();
                }

            }

            if (player.Status.immunity)
            {
                immune_duration += TimeManager.TotalSeconds;

                if (immune_duration > 0.5f)
                {
                    player.Status.immunity = false;
                    immune_duration = 0f;
                }
            }
        }

    }
}
