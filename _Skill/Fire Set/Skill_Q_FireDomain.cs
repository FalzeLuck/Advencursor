using Advencursor._AI;
using Advencursor._Animation;
using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Models.Enemy;
using Advencursor._Particles;
using Advencursor._Particles.Emitter;
using Advencursor._SaveData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace Advencursor._Skill.Fire_Set
{
    public class Skill_Q_FireDomain : Skill
    {
        private float radius;
        private float duration;
        private float multiplier;
        private float healInterval;
        private float healPercentage;
        private float healTick = 0f;
        private float skillDuration;

        private Texture2D floorTexture;
        private Animation floorAnim;
        private Vector2 litPosition;
        private float oldAmp;
        private float newAmp;

        private float radiusX;
        private float radiusY;
        private float collisionRadiusX;
        private float collisionRadiusY;

        Texture2D pixel;

        public Skill_Q_FireDomain(string name, SkillData skillData) : base(name, skillData)
        {
            radius = skillData.GetMultiplierNumber(name, "Radius");
            duration = skillData.GetMultiplierNumber(name, "Duration");
            multiplier = skillData.GetMultiplierNumber(name, "Damage Amplifier");
            healInterval = skillData.GetMultiplierNumber(name, "Heal Interval");
            healPercentage = skillData.GetMultiplierNumber(name, "Heal Percentage");
            floorTexture = Globals.Content.Load<Texture2D>("Item/SetFire/Q_Effect");
            radiusX = radius * 1.75f;
            radiusY = radius;
            collisionRadiusX = radiusX * 0.9f;
            collisionRadiusY = radiusY * 0.9f;
            float scale = (radius * 2) / Math.Min(floorTexture.Width, floorTexture.Height);
            floorAnim = new Animation(floorTexture, 1, 12, 8, true, new Vector2(scale * 1.75f, scale));
            floorAnim.SetOpacity(0.5f);
            rarity = 1;
            setSkill = "Fire";
            description = "Create a circle of fiery fire around you. Turns an area into a firestorm. Enemies that enter are bound by flames. Causes more damage to be received. When the duration ends, the bound fire is released.";

            pixel = new Texture2D(Globals.graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public override void Use(Player player)
        {
            base.Use(player);
            litPosition = player.position;
            skillDuration = duration;


        }

        public override void Update(float deltaTime, Player player)
        {
            base.Update(deltaTime, player);
            skillDuration -= deltaTime;
            if (skillDuration <= 0)
            {
                foreach (var enemy in Globals.EnemyManager)
                {
                    enemy.ampMultiplier = 0;
                }
            }

            if (skillDuration > 0)
            {
                healTick -= TimeManager.TimeGlobal;
                floorAnim.Update();
                if (IsOvalCollidingWithRectangle(litPosition,collisionRadiusX,collisionRadiusY,player.collision) && healTick <= 0)
                {
                    player.Status.Heal((healPercentage / 100) * player.Status.MaxHP);
                    healTick = healInterval;
                }
                foreach (var enemy in Globals.EnemyManager)
                {
                    if (IsOvalCollidingWithRectangle(litPosition, collisionRadiusX, collisionRadiusY, enemy.collision))
                    {
                        enemy.ampMultiplier = multiplier;
                    }
                }
            }

        }

        public override void Draw()
        {
            if (skillDuration > 0)
            {
                floorAnim.Draw(litPosition);

                //Draw Collision Oval Check
                /*int segments = 50;
                DrawOval(Globals.SpriteBatch, pixel, litPosition, collisionRadiusX, collisionRadiusY, segments, Color.White);*/
            }
        }
        
        public bool IsPointInOval(Vector2 point, Vector2 ovalCenter, float radiusX, float radiusY)
        {
            float dx = (point.X - ovalCenter.X) / radiusX;
            float dy = (point.Y - ovalCenter.Y) / radiusY;

            
            return (dx * dx + dy * dy) <= 1;
        }

        
        public bool IsOvalCollidingWithRectangle(Vector2 ovalCenter, float radiusX, float radiusY, Microsoft.Xna.Framework.Rectangle rectangle)
        {
            Vector2 scaledRectCenter = new Vector2(
                (rectangle.Center.X - ovalCenter.X) / radiusX,
                (rectangle.Center.Y - ovalCenter.Y) / radiusY
            );

            float scaledRectWidth = rectangle.Width / (2 * radiusX);
            float scaledRectHeight = rectangle.Height / (2 * radiusY);

            RectangleF scaledRectangle = new RectangleF(
                scaledRectCenter.X - scaledRectWidth,
                scaledRectCenter.Y - scaledRectHeight,
                2 * scaledRectWidth,
                2 * scaledRectHeight
            );

            Vector2[] corners = new Vector2[]
            {
        new Vector2(rectangle.Left, rectangle.Top),
        new Vector2(rectangle.Right, rectangle.Top),
        new Vector2(rectangle.Left, rectangle.Bottom),
        new Vector2(rectangle.Right, rectangle.Bottom)
            };

            foreach (var corner in corners)
            {
                if (IsPointInOval(corner, ovalCenter, radiusX, radiusY))
                {
                    return true;
                }
            }

            Vector2 closestPoint = new Vector2(
                MathHelper.Clamp(ovalCenter.X, rectangle.Left, rectangle.Right),
                MathHelper.Clamp(ovalCenter.Y, rectangle.Top, rectangle.Bottom)
            );

            return IsPointInOval(closestPoint, ovalCenter, radiusX, radiusY);
        }


        public void DrawOval(SpriteBatch spriteBatch, Texture2D pixel, Vector2 ovalCenter, float radiusX, float radiusY, int segments, Color color)
        {
            // Calculate the angle step based on the number of segments
            float angleStep = MathHelper.TwoPi / segments;

            Vector2[] points = new Vector2[segments];

            // Calculate points on the ellipse's circumference
            for (int i = 0; i < segments; i++)
            {
                float angle = i * angleStep;
                float x = ovalCenter.X + radiusX * (float)Math.Cos(angle);
                float y = ovalCenter.Y + radiusY * (float)Math.Sin(angle);
                points[i] = new Vector2(x, y);
            }

            // Draw the ellipse by connecting consecutive points
            for (int i = 0; i < segments - 1; i++)
            {
                DrawLine(spriteBatch,pixel, points[i], points[i + 1], color);
            }

            // Connect the last point to the first to complete the loop
            DrawLine(spriteBatch,pixel, points[segments - 1], points[0], color);
        }

        public void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 point1, Vector2 point2, Color color)
        {
            float distance = Vector2.Distance(point1, point2);
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            spriteBatch.Draw(pixel, point1, null, color, angle, Vector2.Zero, new Vector2(distance, 1), SpriteEffects.None, 0);
        }
    }

}
