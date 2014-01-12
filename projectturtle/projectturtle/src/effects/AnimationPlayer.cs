using System;

using ProjectTurtle.src.screens;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

namespace ProjectTurtle.src.effects {

    //same name as xna one lool
    public enum SpriteEffects {
        None, FlipHorizontally, FlipVertically
    }

    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    class AnimationPlayer {
        int counter;//TODO ???
        public SpriteEffects flip = SpriteEffects.None;
        Func<bool> endFunction;

        enum State {
            Idle,
            Animation,
        }
        State state = State.Idle;

        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        Animation animation;

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        public int FrameIndex {
            get { return frameIndex; }
        }
        int frameIndex;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private float time;

        /// <summary>
        /// Gets a texture origin at the bottom center of each frame.
        /// </summary>
        public Vector2f Origin {
            get {
                if(animation != null)
                    return new Vector2f(animation.FrameWidth/2, animation.FrameHeight/2); 
                else
                    return new Vector2f(0,0);
            }
        }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation, Func<bool> endFunction) {
            if (animation == null) {
                state = State.Idle;
                return;
            }

            // Start the new animation.
            this.animation = animation;
            this.frameIndex = 0;
            this.time = 0.0f;
            state = State.Animation;

            if (animation.animationType == Animation.Type.fronttobacktofrontend)
                counter = 1;
            else
                counter = 0;

            if (animation.animationType == Animation.Type.customorder)
                frameIndex = animation.CustomFrames[counter];

            this.endFunction = endFunction;
        }
        public void PlayAnimation(Animation animation){
            PlayAnimation(animation, null);
        }
        public void StopAnimation() {
            //animation = null;
            state = State.Idle;
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// position is bottom middle of the image
        /// Returns if it drew?
        /// </summary>
        public bool Draw(double gameTime, RenderWindow window, Vector2f position, float rot, float scale, Direction direction) {
            if (animation == null || state == State.Idle)//idk how this happens
                return false;
            position = new Vector2f(position.X - animation.FrameWidth / 2.0f * scale, position.Y - animation.FrameHeight * scale);

            // Calculate the source rectangle of the current frame.
            FloatRect source = new FloatRect(FrameIndex * animation.Texture.Size.X, 0, animation.Texture.Size.Y, animation.Texture.Size.Y);
            int x = FrameIndex * animation.FrameWidth + (int)animation.startPostition.X, y;
            if (!animation.isDirectional())
                y = (int)animation.startPostition.Y;
            else
                y = (int)direction * animation.FrameHeight + (int)animation.startPostition.Y;
            IntRect source2 = new IntRect(x, y, animation.FrameWidth, animation.FrameHeight);

            // Draw the current frame.
            /*if (rot != 0) {
                spriteBatch.Draw(animation.Texture, position, source2, Color.White, rot, Origin, scale, flip, 0.0f);
            } else
                spriteBatch.Draw(animation.Texture, position, source2, Color.White);
            */
            {//TODO drawing is bad atm
                Sprite s = new Sprite(animation.Texture,source2);
                s.Position = position;
                s.Rotation = rot;
                s.Color = Color.White;
                //s.Origin = Origin;
                s.Scale = new Vector2f(flip == SpriteEffects.FlipHorizontally ? -scale : scale, 
                                        flip == SpriteEffects.FlipHorizontally ? -scale : scale);
                
                window.Draw(s);
            }

            // Process passing time. (elapsed time since last update
            time += (float)(gameTime - GameBox.getInstance().getLastUpdateTime()) / 1000;
            while (time > animation.FrameTime) {
                time -= animation.FrameTime;
                // Advance the frame index; looping or clamping as appropriate.
                switch (animation.animationType) {
                case Animation.Type.fronttobacktofront:// 1 2 3 4 5 4 3 2 1 repeat
                    if (frameIndex <= 0) {
                        counter = 1;
                    } else if (frameIndex >= animation.FrameCount - 1) {
                        counter = -1;

                    }//else do nothing and still increase
                    frameIndex += counter;
                    break;
                case Animation.Type.fronttobacktofrontend:// 1 2 3 4 5 4 3 2 1 END
                    frameIndex += counter;

                    if (frameIndex < 0) {
                        animation = null;
                        state = State.Idle;
                        return false;
                    } else if (frameIndex >= animation.FrameCount - 1) {
                        counter = -1;

                    }//else do nothing and still increase

                    break;
                case Animation.Type.looping: //1 2 3 4 5 1 2 3 4 5
                    frameIndex = (frameIndex + 1) % animation.FrameCount;
                    break;
                case Animation.Type.normal: // 1 2 3 4 5 done to idle
                    frameIndex++;
                    break;
                case Animation.Type.keepLast: // 1 2 3 4 5 5 5 5 5...
                    if (frameIndex + 1 >= animation.FrameCount - 1) {
                        frameIndex = animation.FrameCount - 1;
                        //animation = null;
                    } else {
                        frameIndex++;
                    }
                    if (frameIndex == 2)
                        return true;
                    break;
                case Animation.Type.customorder://an array will be inputed when the animation is created, [1,3,4,2]
                    counter++;
                    frameIndex = animation.CustomFrames[counter];
                    break;
                }

            }
            //for normal, reset to normal
            if (animation.animationType == Animation.Type.normal && frameIndex >= animation.FrameCount) {
                animation = null;
                state = State.Idle;
                callEndFunction();
            }
            return true;
        }
        public bool Draw(double gameTime, RenderWindow window, Vector2f position, float scale) {
            return Draw(gameTime, window, position, scale,Direction.south);
        }
        public bool Draw(double gameTime, RenderWindow window, Vector2f position, float scale, Direction direction) {
            return Draw(gameTime, window, position, 0.0f, scale, direction);
        }
        internal void callEndFunction() {
            if (endFunction != null) 
                endFunction();
        }
        internal bool isAnimating() {
            return state == State.Animation;
        }
    }
}
