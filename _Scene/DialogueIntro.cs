using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Advencursor._Scene
{
    public class DialogueIntro : DialogueScene
    {
        private bool isIntro;
        private Vector2 textPosition;

        //Fade
        private float fadeTimeMax = 1f;
        private float fadeTime;
        private float fadeFactor;
        private float fadeOpacity;
        private Texture2D blackScreenTexture;

        //Texture
        private Texture2D bg;
        private Texture2D textUI;
        private Texture2D textureDead;

        //Life
        private Texture2D textureLife;
        private Vector2 lifePos;
        private Vector2 lifeOrigin;
        private float lifeOpacity = 1;
        public DialogueIntro(ContentManager contentManager, SceneManager sceneManager) : base(contentManager, sceneManager)
        {
            isIntro = true;
        }

        public override void Load()
        {
            base.Load();
            currentSceneDialogue = dialogueManager.GetSceneDialogue("Intro");
            blackScreenTexture = Globals.CreateRectangleTexture(Globals.Bounds.X, Globals.Bounds.Y, Color.Black);
            fadeOpacity = 1;

            textureLife = Globals.Content.Load<Texture2D>("Story/L");
            lifePos = new Vector2(Globals.Bounds.X/2, Globals.Bounds.Y/2);
            lifeOrigin = new Vector2(textureLife.Width/2, textureLife.Height/2);

            textUI = Globals.Content.Load<Texture2D>("UI/SkillBackground");
            bg = Globals.Content.Load<Texture2D>("Background/BG_Stage1");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isIntro)
            {
                if (dialogueIndex >= currentSceneDialogue.Count)
                {
                    dialogueIndex = 0;
                    isIntro = false;
                    fadeTime = fadeTimeMax;
                    currentSceneDialogue = dialogueManager.GetSceneDialogue("Start");
                }
            }
            else
            {
                fadeTime -= TimeManager.TotalSeconds;
                float normalizedValue = fadeTime / fadeTimeMax;
                normalizedValue = Math.Clamp(normalizedValue, 0, 1);
                fadeOpacity = normalizedValue;


                if (currentSceneDialogue != null && dialogueIndex < currentSceneDialogue.Count)
                {
                    Dialogue currentLine = currentSceneDialogue[dialogueIndex];
                    if (currentLine.speaker == "L.I.F.E." || currentLine.speaker == "???")
                    {
                        lifeOpacity = 0f;
                    }
                    else
                    {
                        lifeOpacity = 0.5f;
                    }
                }
                
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Globals.SpriteBatch.Draw(bg, Vector2.Zero, Color.White);
            if(!isIntro)
            {
                Globals.SpriteBatch.Draw(textureLife, lifePos, null, Color.White, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                Globals.SpriteBatch.Draw(textureLife, lifePos, null, Color.Black * lifeOpacity, 0f, lifeOrigin, 1.5f, SpriteEffects.None, 0f);
                Vector2 uiOrigin = new Vector2(textUI.Width/2, textUI.Height/2);
                Vector2 uiPos = new Vector2(Globals.Bounds.X / 2, 930);
                Globals.SpriteBatch.Draw(textUI, uiPos, null, Color.White, 0f, uiOrigin, 1f, SpriteEffects.None, 0f);
                DrawCurrentDialogue(new Vector2((Globals.Bounds.X / 2) - 500, 930), 0.2f, Color.Black);
                DrawCurrentSpeaker(new Vector2(Globals.Bounds.X / 2, 860), 0.4f, Color.Black);
            }

            Globals.SpriteBatch.Draw(blackScreenTexture, Vector2.Zero, Color.White * fadeOpacity);
            if (isIntro)
            {
                DrawCurrentDialogue(new Vector2((Globals.Bounds.X/2) - 600,Globals.Bounds.Y/2),0.3f,Color.White);
            }
            

            Globals.DrawCursor();
        }

        private void DrawCurrentDialogue(Vector2 position,float scale,Color color)
        {
            if (currentSceneDialogue != null && dialogueIndex < currentSceneDialogue.Count)
            {
                Dialogue currentLine = currentSceneDialogue[dialogueIndex];
                string displayedText = currentLine.text.Substring(0, currentCharIndex);
                Globals.SpriteBatch.DrawString(font, $"{displayedText}", position, color,0,Vector2.Zero,scale,SpriteEffects.None,0f);
            }
        }

        private void DrawCurrentSpeaker(Vector2 position, float scale, Color color)
        {
            if (currentSceneDialogue != null && dialogueIndex < currentSceneDialogue.Count)
            {
                Dialogue currentLine = currentSceneDialogue[dialogueIndex];
                Vector2 origin = font.MeasureString(currentLine.speaker);
                origin = new Vector2(origin.X/2, origin.Y/2);
                Globals.SpriteBatch.DrawString(font, $"{currentLine.speaker}", position, color, 0, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
