using Advencursor._Managers;
using Advencursor._Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._AI
{
    public class KeepDistanceAI : MovementAI
    {
        public Player target {  get; set; }
        public float distance { get; set; }

        public override void Move(Sprite bot)
        {
            if (target == null) return;
            if (stop) return;

            var direction = target.position - bot.position;
            var length = direction.Length();

            if (length > distance + 2)
            {
                direction.Normalize();
                bot.position += direction * bot.velocity * TimeManager.TimeGlobal;
            }

            else if (length < distance - 2)
            {
                direction.Normalize();
                bot.position -= direction * bot.velocity * TimeManager.TimeGlobal;
            }
        }

        public override void Stop()
        {
            stop = true;
        }
        public override void Start()
        {
            stop = false;
        }
    }
}
