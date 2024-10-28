using Advencursor._Managers;
using Advencursor._SaveData;
using Advencursor._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene
{
    public class IntroScene : IScene
    {

        private ContentManager contentManager;
        private SceneManager sceneManager;
        //Fade
        private float fadeTimeMax = 1f;
        private float fadeTime;
        private float fadeFactor;
        private float fadeOpacity;
        private Texture2D blackScreenTexture;
        private bool fadetoblack;

        private Texture2D logoTexture;

        private SpriteFont spriteFont;
        private string warningText = "WARNING\r\n\r\nThis game features flashing lights, intense colors, and rapid-action sequences. \r\nThese effects may trigger discomfort or seizures for players with photosensitive epilepsy or visual sensitivities. \r\nIf you experience dizziness, nausea, or visual disturbances, please stop playing.\r\n\r\nThis game also includes cartoonish combat scenes and fast-paced visuals that may not be suitable for all players. \r\nPlease enjoy responsibly and remember to play at your own pace.";
        private float _targetWidthPercentage = 0.8f;
        private float _targetHeightPercentage = 0.8f;

        private int phaseIndicator;
        private enum IntroPhase
        {
            Logo,
            Warning
        }
        private float logoPhaseTimer = 3f;
        private float warningPhaseTimer = 4f;
        public IntroScene(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
        }
        public void Load()
        {
            spriteFont = Globals.Content.Load<SpriteFont>("Font/TextFont");
            logoTexture = Globals.Content.Load<Texture2D>("Logo");
            fadetoblack = false;
            phaseIndicator = (int)IntroPhase.Logo;
            blackScreenTexture = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Black);
            fadeOpacity = 1;
            fadeTime = fadeTimeMax;
        }

        public void Update(GameTime gameTime)
        {
            if (phaseIndicator == (int)IntroPhase.Logo)
            {
                logoPhaseTimer -= TimeManager.TotalSeconds;
                if (logoPhaseTimer <= 1)
                {
                    fadetoblack = true;
                }
                if (logoPhaseTimer <= 0)
                {
                    phaseIndicator = (int)IntroPhase.Warning;
                    fadetoblack = false;
                }
            }
            else if (phaseIndicator == (int)IntroPhase.Warning)
            {
                warningPhaseTimer -= TimeManager.TotalSeconds;


                if(warningPhaseTimer <= 0)
                {
                    sceneManager.AddScene(new MenuScene(contentManager, sceneManager));
                }
            }
            FadeUpdate();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(blackScreenTexture, Vector2.Zero, Color.White);
            if(phaseIndicator == (int)IntroPhase.Logo)
            {
                Vector2 origin = new Vector2(logoTexture.Width/2, logoTexture.Height/2);
                spriteBatch.Draw(logoTexture,new Vector2(Globals.Bounds.X/2,Globals.Bounds.Y/2),null,Color.White,0f,origin,1f,SpriteEffects.None,0f);
            }
            if (phaseIndicator == (int)IntroPhase.Warning)
            {
                Vector2 textSize = spriteFont.MeasureString(warningText);

                float targetWidth = Globals.Bounds.X * _targetWidthPercentage;
                float targetHeight = Globals.Bounds.Y * _targetHeightPercentage;

                float scaleX = targetWidth / textSize.X;
                float scaleY = targetHeight / textSize.Y;
                float scale = MathHelper.Min(scaleX, scaleY);

                Vector2 position = new Vector2(
                    (Globals.Bounds.X - textSize.X * scale) / 2,
                    (Globals.Bounds.Y - textSize.Y * scale) / 2
                );

                spriteBatch.DrawString(spriteFont, warningText, position, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            }

            Globals.SpriteBatch.Draw(blackScreenTexture, Vector2.Zero, Color.White * fadeOpacity);
        }

        private void FadeUpdate()
        {
            if (fadetoblack)
            {
                fadeTime += TimeManager.TotalSeconds;
                if (fadeTime >= 1)
                {
                    fadeTime = 1;
                }
                
            }
            else
            {
                fadeTime -= TimeManager.TotalSeconds;
                if (fadeTime <= 0)
                {
                    fadeTime = 0;
                }
            }
            float normalizedValue = fadeTime / fadeTimeMax;
            normalizedValue = Math.Clamp(normalizedValue, 0, 1);
            fadeOpacity = normalizedValue;
        }
    }
}
