using Advencursor._Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Advencursor._Managers
{
    public class AnimationManager
    {
        Dictionary<string, Animation> animations;
        private List<Animation> activeAnimations;

        public AnimationManager() 
        {
            animations = new Dictionary<string, Animation>();
            activeAnimations = new List<Animation>();
        }

        public void AddAnimation(string name,Animation animation)
        {
            animations[name] = animation;
        }

        public void Play(string name)
        {
            if (animations.ContainsKey(name))
            {
                Animation animation = animations[name];
                animation.currentFrame = 0;
                animation.IsComplete = false;
                animation.IsCollide = true;
                if (!activeAnimations.Contains(animation))
                {
                    activeAnimations.Add(animation);
                }
            }
        }

        public void Pause(string name)
        {
            Animation animation = GetAnimation(name);
            if (animation != null)
            {
                activeAnimations.Remove(animation);
            }
        }

        public void PauseFrame(string name,int frame)
        {
            Animation animation = GetAnimation(name);
            animation.currentFrame = frame;
            animation.IsComplete = false;
            animation.IsCollide = true;

        }

        public void Stop(string name)
        {
            Animation animation = GetAnimation(name);
            animation.IsCollide = false;
            if (animation != null)
            {
                animation.IsComplete = true;
                activeAnimations.Remove(animation);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = activeAnimations.Count - 1; i >= 0; i--)
            {
                Animation animation = activeAnimations[i];
                if (!animation.IsComplete)
                {
                    animation.Update(gameTime);
                }

                if (animation.IsLooping && animation.IsComplete)
                {
                    activeAnimations.RemoveAt(i);
                }
            }
        }

        public void Draw(Vector2 position)
        {
            foreach (var animation in activeAnimations)
            {
                animation.Draw(position);
            }
        }

        private Animation GetAnimation(string name)
        {
            return animations.ContainsKey(name) ? animations[name] : null;
        }

        public bool IsComplete(string name)
        {
            if (animations.ContainsKey(name))
            {
                return animations[name].IsComplete;
            }
            else
            {
                return false;
            }
        }

        public void SetOffset(string name, Vector2 offset)
        {
            animations[name].offset = offset;
        }

        public Rectangle GetCollision(string name,Vector2 position)
        {
            if (animations.ContainsKey(name))
            {
                Animation animation = animations[name];
                /*int frameWidth = animation.Texture.Width / animation.Column;
                int frameHeight = animation.Texture.Height / animation.Row;

                int startX = (int)((position.X - frameWidth/2) + animation.offset.X);
                int startY = (int)((position.Y-frameHeight/2) + animation.offset.Y);
                */

                return animation.GetCollision(position);
            }
            return new Rectangle(0,0, 0, 0);
            
        }

        public bool IsCollision(string name)
        {
            Animation animation = GetAnimation(name);
            return animation.IsCollide;
        }


    }
}
