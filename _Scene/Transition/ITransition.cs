using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene.Transition
{
    public interface ITransition
    {
        bool IsComplete { get; }
        bool IsInTransition { get; }
        void Start(bool isInTransition);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
