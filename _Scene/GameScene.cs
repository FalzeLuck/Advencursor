using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Models.Enemy;
using Advencursor._Models.Enemy._CommonEnemy;
using Advencursor._Skill;
using Advencursor._UI;
using Advencursor._Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene
{
    public class GameScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private AnimationManager animationManager = new AnimationManager();
        private UIManager uiManager = new UIManager();

        private SpriteFont font;


        private Player _player;

        List<_Enemy> enemies;
        List<Item> items;

        private readonly Timer timer;
        private Texture2D background;
        public GameScene(ContentManager contentManager, SceneManager sceneManager) 
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
            Globals.Game.IsMouseVisible = true;

            font = Globals.Content.Load<SpriteFont>("basicFont");

            timer = new(Globals.Content.Load<Texture2D>("playButton"),
                font,
                new(0, 0),
                15f
                );

            timer.StartStop();
            timer.Repeat = true;
        }

        public void Load() 
        {
            _player = new(Globals.Content.Load<Texture2D>("playerTexture"), new(1000, 1000), 10);
            enemies = new List<_Enemy>
            {
            new Kiki(Globals.Content.Load<Texture2D>("Enemies/Kiki"), new(500, 500), 1, 2, 2)
            {
                movementAI = new FollowMovementAI
                {
                    target = _player,
                }
            }
        };


            Animation slashAnimation = new Animation(Globals.Content.Load<Texture2D>("Animation/SlashTexture"), row: 2, column: 4, fps: 30, false);
            animationManager.AddAnimation("Slash",slashAnimation);

            Skill fire = new Skill("fireball",30,5);

            items = new List<Item>()
            {
                new Item("fireball book",fire,Keys.Q)
            };


            int space = 1920 / 15;
            UISkill skillUI_Q = new(Globals.Content.Load<Texture2D>("playButton"), new(space * 4, 1000),fire);
            UISkill skillUI_W = new(Globals.Content.Load<Texture2D>("playButton"), new(space * 6, 1000), fire);
            UISkill skillUI_E = new(Globals.Content.Load<Texture2D>("playButton"), new(space * 8, 1000), fire);
            UISkill skillUI_R = new(Globals.Content.Load<Texture2D>("playButton"), new(space * 10, 1000), fire);
            uiManager.AddElement(skillUI_Q);
            uiManager.AddElement(skillUI_W);
            uiManager.AddElement(skillUI_E);
            uiManager.AddElement(skillUI_R);

            background = Globals.Content.Load<Texture2D>("background");

        }

        public void Update(GameTime gameTime)
        {
            timer.Update();
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime);
                if (enemy.collision.Intersects(animationManager.GetCollision("Slash", _player.position)) && animationManager.IsCollision("Slash"))
                {
                    enemy.Status.TakeDamage(1);
                    enemy.Status.immunity = true;
                    
                }

                if (!enemy.Status.IsAlive())
                {
                    enemies.Remove(enemy);
                    break; //Don't remove. If remove = crash
                }
            }


            
            _player.Update();
            animationManager.Update(gameTime);
            ParticleManager.Update();
            uiManager.Update(gameTime);
            UpdatePlayer();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.LightGray);
            timer.Draw();
            _player.Draw();
            ParticleManager.Draw();
            animationManager.Draw(_player.position);
            foreach (var enemy in enemies)
            {
                enemy.Draw();
            }
            uiManager.Draw(spriteBatch);
            
        }

        private void UpdatePlayer()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {

                _player.UseSkill(Keys.Q);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {

                _player.UseSkill(Keys.W);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {

                _player.UseSkill(Keys.E);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {

                _player.UseSkill(Keys.R);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {

                
            }


            if (InputManager.MouseLeftClicked)
            {
                animationManager.SetOffset("Slash", new Vector2(0, 0));
                animationManager.Play("Slash");
                _player.FlipHorizontal();
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
                _player.EquipItem(item);
                item.skill.Update(TimeManager.TotalSeconds);
            }
        }


    }
}
