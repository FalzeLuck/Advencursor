﻿using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Models.Enemy;
using Advencursor._SaveData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace Advencursor
{
    public static class Globals
    {
        public static Game Game { get; set; }
        public static float BulletTime { get; private set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static SpriteFont SpriteFont { get; set; }
        public static GraphicsDevice graphicsDevice { get; set; }
        public static Camera Camera { get; set; }

        public static Effect grayScaleEffect { get; set; }
        public static Viewport Viewport { get; set; }
        public static Point Bounds { get; set; }
        public static Rectangle fullScreenRectangle { get; set; }
        public static bool Paused = false;
        public static Random random { get; set; } = new Random();
        public static List<_Enemy> EnemyManager { get; set; } = new List<_Enemy>();
        public static SoundManager soundManager { get; set; } = new SoundManager();

        public static Texture2D cursor;

        public static void Update(GameTime gameTime)
        {
            TimeManager.Update(gameTime);
            Camera.Update(Globals.Bounds.X,Globals.Bounds.Y);
        }

        public static float RandomFloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min)) + min;
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius)
        {
            Texture2D texture = new Texture2D(graphicsDevice, radius * 2, radius * 2);
            Color[] colorData = new Color[texture.Width * texture.Height];

            float radSquared = radius * radius;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    int xDistance = x - radius;
                    int yDistance = y - radius;

                    if (xDistance * xDistance + yDistance * yDistance <= radSquared)
                    {
                        colorData[y * texture.Width + x] = Color.White;
                    }
                    else
                    {
                        colorData[y * texture.Width + x] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
        public static List<Vector2> CreateCircleOutline(Vector2 center, float radius, int pointCount)
        {
            List<Vector2> circlePoints = new List<Vector2>();

            for (int i = 0; i < pointCount; i++)
            {
                float angle = MathHelper.TwoPi * i / pointCount;

                float x = center.X + radius * (float)Math.Cos(angle);
                float y = center.Y + radius * (float)Math.Sin(angle);

                circlePoints.Add(new Vector2(x, y));
            }

            return circlePoints;
        }

        public static Texture2D CreateRectangleTexture(int width, int height,Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] colorData = new Color[width * height];

            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = color;
            }

            texture.SetData(colorData);
            return texture;
        }

        public static Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            Vector2 translatedPoint = point - pivot;

            float rotatedX = translatedPoint.X * cos - translatedPoint.Y * sin;
            float rotatedY = translatedPoint.X * sin + translatedPoint.Y * cos;

            return new Vector2(rotatedX, rotatedY) + pivot;
        }

        public static Rectangle CreateBoundingRectangle(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            
            float minX = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
            float minY = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));

            float maxX = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X));
            float maxY = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y));


            return new Rectangle(
                (int)minX,  
                (int)minY, 
                (int)(maxX - minX),  
                (int)(maxY - minY) 
            );
        }
        public static void DrawCursor()
        {
            SpriteBatch.Draw(cursor, InputManager._mousePosition,Color.White);
        }
        public static void DrawLine(Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Texture2D pixel = Globals.CreateRectangleTexture(1, 1, Color.White);

            float distance = Vector2.Distance(start, end);

            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            SpriteBatch.Draw(pixel,
                             start,
                             null,  
                             color, 
                             angle, 
                             Vector2.Zero, 
                             new Vector2(distance, thickness),
                             SpriteEffects.None,
                             0);
        }

        public static Vector2 MoveVector(Vector2 start, float distance, float angleInRadians)
        {
            float newX = start.X + distance * (float)Math.Cos(angleInRadians);
            float newY = start.Y + distance * (float)Math.Sin(angleInRadians);

            return new Vector2(newX, newY);
        }

        public static void SetGreyScale(float amount)
        {
            grayScaleEffect.Parameters["grayscaleAmount"].SetValue(amount);
        }

        //If Begin Pleas End too if not it will Error
        public static void BeginDrawGrayScale()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(effect: grayScaleEffect, transformMatrix: Camera.transform);
        }

        public static void EndDrawGrayScale()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(transformMatrix: Globals.Camera.transform);
        }

        public static void LoadSounds()
        {
            //Stage1
            SoundEffect Common1Moving = Content.Load<SoundEffect>("Sound/Effect/Common1/Moving");
            SoundEffect Common1Hit = Content.Load<SoundEffect>("Sound/Effect/Common1/Hit");
            SoundEffect Common1Die = Content.Load<SoundEffect>("Sound/Effect/Common1/Die");
            SoundEffect Elite1Moving = Content.Load<SoundEffect>("Sound/Effect/Elite1/Moving");
            SoundEffect Elite1Slam = Content.Load<SoundEffect>("Sound/Effect/Elite1/Slam");
            SoundEffect Boss1Rolling = Content.Load<SoundEffect>("Sound/Effect/Boss1/RollAttack");
            SoundEffect Boss1WallHit = Content.Load<SoundEffect>("Sound/Effect/Boss1/WallHit");
            SoundEffect Boss1Charge = Content.Load<SoundEffect>("Sound/Effect/Boss1/Charge");
            SoundEffect TomatoBomb = Content.Load<SoundEffect>("Sound/Effect/Boss1/TomatoBomb");
            SoundEffect TomatoPoison = Content.Load<SoundEffect>("Sound/Effect/Boss1/Poison");
            soundManager.LoadSound("Common1Moving", Common1Moving,0.1f);
            soundManager.LoadSound("Common1Hit", Common1Hit,0.2f);
            soundManager.LoadSound("Common1Die", Common1Die,0.1f);
            soundManager.LoadSound("Elite1Moving", Elite1Moving,0.5f);
            soundManager.LoadSound("Elite1Slam", Elite1Slam);
            soundManager.LoadSound("Boss1Rolling", Boss1Rolling,0.3f);
            soundManager.LoadSound("Boss1WallHit", Boss1WallHit);
            soundManager.LoadSound("Boss1Charge", Boss1Charge);
            soundManager.LoadSound("TomatoBomb", TomatoBomb, 0.5f);
            soundManager.LoadSound("TomatoPoison", TomatoPoison ,0.15f);

            //Stage2
            SoundEffect Elite2Moving = Content.Load<SoundEffect>("Sound/Effect/Elite2/Moving");
            SoundEffect Elite2Explode = Content.Load<SoundEffect>("Sound/Effect/Elite2/Explode");
            SoundEffect Boss2Opening = Content.Load<SoundEffect>("Sound/Effect/Boss2/WaterPour");
            SoundEffect Boss2Attack = Content.Load<SoundEffect>("Sound/Effect/Boss2/WaterAttack");
            SoundEffect Boss2Walk = Content.Load<SoundEffect>("Sound/Effect/Boss2/Walk");
            SoundEffect Boss2Dash = Content.Load<SoundEffect>("Sound/Effect/Boss2/Dash");
            SoundEffect Boss2Charge = Content.Load<SoundEffect>("Sound/Effect/Boss2/Charging");
            soundManager.LoadSound("Elite2Moving", Elite2Moving, 0.5f);
            soundManager.LoadSound("Elite2Explode", Elite2Explode, 0.5f);
            soundManager.LoadSound("Boss2Opening", Boss2Opening, 1f);
            soundManager.LoadSound("Boss2Attack", Boss2Attack, 0.75f);
            soundManager.LoadSound("Boss2Walk", Boss2Walk, 0.75f);
            soundManager.LoadSound("Boss2Dash", Boss2Dash, 0.75f);
            soundManager.LoadSound("Boss2Charge", Boss2Charge, 0.75f);

            //Stage3
            SoundEffect Boss3Surprise = Content.Load<SoundEffect>("Sound/Effect/Boss3/Surprise");
            SoundEffect Boss3Hit = Content.Load<SoundEffect>("Sound/Effect/Boss3/Hurt");
            SoundEffect Boss3Warp = Content.Load<SoundEffect>("Sound/Effect/Boss3/Warp");
            SoundEffect Boss3WarpEffect = Content.Load<SoundEffect>("Sound/Effect/Boss3/WarpEffect");
            SoundEffect Boss3Knife = Content.Load<SoundEffect>("Sound/Effect/Boss3/Knife");
            SoundEffect Boss3Laser = Content.Load<SoundEffect>("Sound/Effect/Boss3/Laser");
            SoundEffect Boss3Die = Content.Load<SoundEffect>("Sound/Effect/Boss3/Dead");
            SoundEffect Boss3Healing = Content.Load<SoundEffect>("Sound/Effect/Boss3/Healing");
            soundManager.LoadSound("Boss3Surprise", Boss3Surprise);
            soundManager.LoadSound("Boss3Hit", Boss3Hit);
            soundManager.LoadSound("Boss3Warp", Boss3Warp);
            soundManager.LoadSound("Boss3WarpEffect", Boss3WarpEffect);
            soundManager.LoadSound("Boss3Knife", Boss3Knife,0.5f);
            soundManager.LoadSound("Boss3Laser", Boss3Laser, 0.7f);
            soundManager.LoadSound("Boss3Die", Boss3Die);
            soundManager.LoadSound("Boss3Healing", Boss3Healing);

            //General
            SoundEffect slashSound = Content.Load<SoundEffect>("Sound/Effect/Slash");
            SoundEffect buttonHover = Content.Load<SoundEffect>("Sound/Effect/ButtonHover");
            SoundEffect buttonClick = Content.Load<SoundEffect>("Sound/Effect/ButtonClick");
            SoundEffect EquipItem = Content.Load<SoundEffect>("Sound/Effect/EquipItem");
            SoundEffect Warning = Content.Load<SoundEffect>("Sound/Effect/Warning");
            SoundEffect GachaRare = Content.Load<SoundEffect>("Sound/Effect/GachaRare");
            SoundEffect Win = Content.Load<SoundEffect>("Sound/Effect/Win");
            SoundEffect Lose = Content.Load<SoundEffect>("Sound/Effect/Lose");
            soundManager.LoadSound("Slash", slashSound);
            soundManager.LoadSound("Hover", buttonHover,0.6f);
            soundManager.LoadSound("Click", buttonClick,0.8f);
            soundManager.LoadSound("EquipItem", EquipItem);
            soundManager.LoadSound("Warning", Warning,0.5f);
            soundManager.LoadSound("GachaRare", GachaRare);
            soundManager.LoadSound("Win", Win);
            soundManager.LoadSound("Lose", Lose);

            //ThunderSet
            SoundEffect QThunder = Content.Load<SoundEffect>("Sound/Effect/Thunder/Q");
            SoundEffect WThunder = Content.Load<SoundEffect>("Sound/Effect/Thunder/W");
            SoundEffect EThunder = Content.Load<SoundEffect>("Sound/Effect/Thunder/E");
            SoundEffect RThunder = Content.Load<SoundEffect>("Sound/Effect/Thunder/R");
            soundManager.LoadSound("QThunder", QThunder);
            soundManager.LoadSound("WThunder", WThunder);
            soundManager.LoadSound("EThunder", EThunder);
            soundManager.LoadSound("RThunder", RThunder);

            //BuffSet
            SoundEffect QBuff = Content.Load<SoundEffect>("Sound/Effect/Buff/Q");
            SoundEffect WBuff = Content.Load<SoundEffect>("Sound/Effect/Buff/W");
            SoundEffect EBuff = Content.Load<SoundEffect>("Sound/Effect/Buff/E");
            SoundEffect RBuff = Content.Load<SoundEffect>("Sound/Effect/Buff/R");
            soundManager.LoadSound("QBuff", QBuff);
            soundManager.LoadSound("WBuff", WBuff);
            soundManager.LoadSound("EBuff", EBuff);
            soundManager.LoadSound("RBuff", RBuff);

            //FireSet
            SoundEffect QFire = Content.Load<SoundEffect>("Sound/Effect/Fire/Q");
            SoundEffect WFire = Content.Load<SoundEffect>("Sound/Effect/Fire/W");
            SoundEffect EFire = Content.Load<SoundEffect>("Sound/Effect/Fire/E");
            SoundEffect RFire = Content.Load<SoundEffect>("Sound/Effect/Fire/R");
            SoundEffect RFireAttack = Content.Load<SoundEffect>("Sound/Effect/Fire/RAttack");
            soundManager.LoadSound("QFire", QFire,0.2f);
            soundManager.LoadSound("WFire", WFire);
            soundManager.LoadSound("EFire", EFire);
            soundManager.LoadSound("RFire", RFire);
            soundManager.LoadSound("RFireAttack", RFireAttack,0.2f);


            //Visual Novel Scene
            SoundEffect stab = Content.Load<SoundEffect>("Sound/Effect/Stab");
            SoundEffect click = Content.Load<SoundEffect>("Sound/Effect/Story/Click");
            soundManager.LoadSound("Stab", stab);
            soundManager.LoadSound("ClickVisualNovel", click);



            soundManager.StopAllSounds();
        }

        public static void SetGlobalSongVolume(float volume)
        {
            soundManager.SetSongVolume(volume);
            GameData gameData = new GameData();
            gameData.LoadData();
            gameData.volumeMusic = volume;
            gameData.SaveData();
        }
    }
}
