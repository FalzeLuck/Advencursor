﻿using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Particles;
using Advencursor._Particles.Emitter;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Skill.Thunder_Set
{
    public class Skill_E_ThunderSpeed : Skill
    {
        private SpriteEmitter spriteEmitter;

        private ParticleEmitterData ped;
        private ParticleEmitter pe;

        private float bufftime;
        private bool isUsing = false;

        //For Multiplier
        private Player player;
        private float skillMultiplier = 1.2f;
        public Skill_E_ThunderSpeed(string name, float cooldown, Player player) : base(name, cooldown)
        {
            this.player = player;
            spriteEmitter = new SpriteEmitter(() => player.position);

            ped = new()
            {
                particleData = new()
                {
                    colorStart = Color.LightGoldenrodYellow,
                    colorEnd = Color.White,
                },
                interval = 0.01f,
                emitCount = 1,
                angleVariance = 0f,
                speedMax = 0f,
                speedMin = 0f,
            };

            pe = new(spriteEmitter, ped);
        }

        public override void Use()
        {
            base.Use();
            ParticleManager.AddParticleEmitter(pe);
            bufftime = 2f;

            isUsing = true;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);

            if (isUsing)
            {
                bufftime -= TimeManager.TotalSeconds;

                
                if(bufftime <= 0)
                {
                    ParticleManager.RemoveParticleEmitter(pe);
                    isUsing = false;
                }
            }

        }
    }
}