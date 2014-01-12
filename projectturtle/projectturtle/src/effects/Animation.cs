using System;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.effects {
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    class Animation {
        bool directional{
           get {
                return b;
            }
            set {
                b = value;
            } 
        }// looking; south, north, east, west
        bool b;
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture Texture {
            get { return texture; }
        }
        Texture texture;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount {
            get { return totalFrames; }
        }
        int totalFrames;

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth {
            // Assume square frames.
            get { return (int)size.X; }
        }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight {
            get { return (int)size.Y; }
        }

        public int[] CustomFrames {
            get { return frames; }
        }
        int[] frames;

        /// <summary>
        /// Constructors a new animation.
        /// </summary>        
        public Animation(Texture texture, Vector2f startPostition, int totalFrames, Vector2f size, float frameTime, Type animationType, string name) {
            this.texture = texture;
            this.frameTime = frameTime;
            this.animationType = animationType;
            this.startPostition = startPostition;
            this.totalFrames = totalFrames;
            this.size = size;
            this.name = name;
        }
        public Animation(Texture texture, Vector2f startPostition, int totalFrames, 
                Vector2f size, float frameTime, Type animationType, string name, bool directional) 
                : this(texture,startPostition,totalFrames,size,frameTime,animationType,name){
            this.directional = directional;//hate doing this
        }
        //for custom frames
        public Animation(Texture texture, Vector2f startPostition, int[] frames,
                Vector2f size, float frameTime, string name)
            : this(texture, startPostition, frames.Length, size, frameTime, Animation.Type.customorder, name) {
            this.frames = frames;
        }
        public Vector2f startPostition;
        Vector2f size;
        public Type animationType;
        public string name;
        public enum Type {
            normal,
            looping,
            fronttobacktofront,
            keepLast,
            fronttobacktofrontend,
            customorder
        }
        public static Animation createAnimation(string image_file, float secondsPerFrame,
                int numOfFrame, Vector2f frameSize, Vector2f coord, Animation.Type aniType, string name) {

            return new Animation(GameBox.loadTexture(image_file), new Vector2f(frameSize.X * coord.X,
                    frameSize.Y * coord.Y), numOfFrame, frameSize, secondsPerFrame, aniType, name);
        }
        public bool isDirectional() {
            return directional;
        }


        internal void setDirectional(bool p) {
            directional = p;
        }
    }
}
