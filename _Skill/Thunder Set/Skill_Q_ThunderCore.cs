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
            description = "Awake lightning power in your soul. Cause the mouse cursor to shine with lightning. When attacking enemies during this time, enemy will be shocked by lightning and inflict Paralysis for a short period.";
            buffTime = 8f;
        }

        public override void Use(Player player)
        {
            base.Use(player);
            aura = new(Globals.Content.Load<Texture2D>("Item/SetThunder/Q_Thunder"), 1, 4, 8, true);
            buffTime = 0f;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            buffTime += deltaTime;

            if (buffTime > 0f && aura != null)
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
            if (buffTime <= 8f && aura != null)
            aura.Draw(position);
        }
    }
}
