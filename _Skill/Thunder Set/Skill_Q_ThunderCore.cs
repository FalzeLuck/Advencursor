using Advencursor._Animation;
using Advencursor._Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill.Thunder_Set
{
    public class Skill_Q_ThunderCore : Skill
    {
        private float buffTime;
        private Vector2 position;

        Animation aura;
        public  Skill_Q_ThunderCore(string name, float cooldown) : base(name, cooldown)
        {
            aura = new(Globals.Content.Load<Texture2D>("Item/SetThunder/Q_Thunder"),1,4,8,true);
            buffTime = 8f;
        }

        public override void Use(Player player)
        {
            base.Use(player);
            buffTime = 0f;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            buffTime += deltaTime;

            if (buffTime > 0f)
            {
                aura.Update();
                aura.SetOpacity(1f);
                position = player.position;
                player.isBuff = true;
                player.buffIndicator = "Thunder_";
            }
            if (buffTime > 8f)
            {
                player.isBuff = false;
                player.buffIndicator = "Normal_";
            }
        }

        public override void Draw()
        {
            if (buffTime <= 8f)
            aura.Draw(position);
        }
    }
}
