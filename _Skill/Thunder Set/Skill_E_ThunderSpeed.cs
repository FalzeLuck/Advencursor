using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Models.Enemy;
using Advencursor._Particles;
using Advencursor._Particles.Emitter;
using Advencursor._SaveData;
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
        private ParticleEmitterData ped1;
        private ParticleEmitter pe;
        private ParticleEmitter pe1;

        private float bufftime;
        private float duration;
        private bool isUsing = false;

        private List<Rectangle> collision = new List<Rectangle>();
        private List<float> collisionCooldown = new List<float>();

        private float oldPlayerDelay;

        //For Multiplier
        private Player player;
        private float skillMultiplier = 0.5f;
        private float speedMultiplier = 2f;
        public Skill_E_ThunderSpeed(string name, float cooldown, SkillData skillData) : base(name, cooldown, skillData)
        {
            skillMultiplier = skillData.GetMultiplierNumber(name, "Damage Multiplier");
            duration = skillData.GetMultiplierNumber(name, "Duration");
            speedMultiplier = skillData.GetMultiplierNumber(name, "Speed Multiplier");
            rarity = 3;
            setSkill = "Thunder";
            description = "Release lightning power into the ground. Enemy who surpass will take damage and got paralized. Also, user will gain additional attack speed.";
        }

        public override void Use(Player player)
        {
            base.Use(player);
            spriteEmitter = new SpriteEmitter(() => player.position);
            bufftime = duration;

            ped = new()
            {
                particleData = new LightningParticleData()
                {
                    sizeStart = 150f,
                    sizeEnd = 150f,
                    colorStart = Color.Cyan,
                    colorEnd = Color.White,
                },
                interval = 0.07f,
                emitCount = 1,
                angleVariance = 180f,
                speedMax = 0.5f,
                speedMin = 0.5f,
                lifeSpanMax = bufftime,
                lifeSpanMin = bufftime,
                rotationMax = 180,
            };

            ped1 = new()
            {
                particleData = new LightningParticleData()
                {
                    sizeStart = 150f,
                    sizeEnd = 150f,
                    colorStart = Color.Yellow,
                    colorEnd = Color.White,
                },
                interval = 0.07f,
                emitCount = 1,
                angleVariance = 180f,
                speedMax = 0.5f,
                speedMin = 0.5f,
                lifeSpanMax = bufftime,
                lifeSpanMin = bufftime,
                rotationMax = 180,
            };

            pe = new(spriteEmitter, ped);
            pe1 = new(spriteEmitter, ped1);
            ParticleManager.AddParticleEmitter(pe);
            ParticleManager.AddParticleEmitter(pe1);
            

            collisionCooldown = new List<float>(new float[100]);

            oldPlayerDelay = player.normalAttackDelay;
            player.SetAttackDelay(oldPlayerDelay / speedMultiplier);

            isUsing = true;
        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);

            if (isUsing)
            {
                bufftime -= TimeManager.TimeGlobal;

                
                if(bufftime <= 0)
                {
                    ParticleManager.RemoveParticleEmitter(pe);
                    ParticleManager.RemoveParticleEmitter(pe1);
                    isUsing = false;
                    player.SetAttackDelay(oldPlayerDelay);
                }

                collision.Add(new Rectangle((int)player.position.X,(int)player.position.Y,10,10));

                for (int i = 0; i < Globals.EnemyManager.Count; i++)
                {
                    if (collisionCooldown[i] <= 0)
                    {
                        if (collision.Any(collide => collide.Intersects(Globals.EnemyManager[i].collision)) && Globals.EnemyManager[i].Status.IsAlive())
                        {
                            Globals.EnemyManager[i].TakeDamage(skillMultiplier, player);
                            collisionCooldown[i] = 0.1f;
                        }
                    }
                }

                for (int i = 0; i < collisionCooldown.Count; i++)
                {
                    collisionCooldown[i] -= TimeManager.TimeGlobal;
                }
                
            }

            if (!isUsing)
            {
                
                collision.Clear();
                collisionCooldown.Clear();
            }

        }
    }
}
