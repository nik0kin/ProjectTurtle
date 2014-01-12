using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.effects {
    class WeaponPlayer {
        float time, scale;
        float[] frameTime;
        short attacks, mFrame, frame;//mFrame is overall frame, frame is current animation frame
        Texture texture;
        Sprite sprite;
        IntRect sourceRect;
        Direction lastDirection;
      
        enum AniState {
            anim1, anim2,
            idle
        }
        AniState state;


        internal WeaponPlayer(Texture texture, float[] frameTime,float scale) {
            this.attacks = (short)frameTime.Length;
            this.texture = texture;
            this.frameTime = frameTime;
            this.scale = scale;
            sprite = new Sprite(texture);
        }
        internal WeaponPlayer(Texture texture, float frameTime, float scale)
            : this(texture, new float[1] { frameTime },scale) {

        }

        internal void draw(RenderWindow window, double gameTime, Vector2f midBotPos,Direction direction) {
            Vector2f drawPos = new Vector2f(midBotPos.X - 50*scale,midBotPos.Y - 50*scale);
            

            sourceRect = GeoLib.getRectOfFrame(mFrame, 4, new Vector2f(164,184));
            sprite.TextureRect = sourceRect;
            sprite.Position = drawPos;
            sprite.Scale = new Vector2f(scale, scale);
            window.Draw(sprite);

            if (state == AniState.idle) {
                if (lastDirection != direction || mFrame > 3) {
                    switch (direction) {
                    case Direction.south:
                        mFrame = 0;
                        break;
                    case Direction.north:
                        mFrame = 1;
                        break;
                    case Direction.east:
                        mFrame = 2;
                        break;
                    case Direction.west:
                        mFrame = 3;
                        break;
                    }
                    lastDirection = direction;
                }
                return;
            }

            float cFrameTime = frameTime[(int)state];//current frameTime
            time += (float)(gameTime - GameBox.getInstance().getLastUpdateTime()) / 1000;//getting seconds
            if (time > cFrameTime && time < cFrameTime + cFrameTime) {
                if (frame == 0) {
                    time -= cFrameTime;
                    mFrame++;
                    frame++;
                }else
                    state = AniState.idle;
                
            } 
        }

        internal void doAnimation1() {
            if (state == AniState.anim1) return;
            time = 0.0f;

            switch (lastDirection) {
            case Direction.south:
                mFrame = 4;
                break;
            case Direction.north:
                mFrame = 6;
                break;
            case Direction.east:
                mFrame = 8;
                break;
            case Direction.west:
                mFrame = 10;
                break;
            }
            state = AniState.anim1;
            frame = 0;
        }
        internal void doAnimation2() {
            if (attacks < 2) throw new Exception("one attack");

            time = 0.0f;

            switch (lastDirection) {
            case Direction.south:
                mFrame = 12;
                break;
            case Direction.north:
                mFrame = 14;
                break;
            case Direction.east:
                mFrame = 16;
                break;
            case Direction.west:
                mFrame = 18;
                break;
            }
            state = AniState.anim2;
            frame = 0;
        }
    }
}
