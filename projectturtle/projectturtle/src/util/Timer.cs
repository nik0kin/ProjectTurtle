using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.objects;

namespace ProjectTurtle.src.util {
    class Timer {
        double startSeconds;

        internal Timer(double gameTime) {
            startSeconds = gameTime / 1000;

        }
        internal void reset(double gameTime) {
            startSeconds = gameTime / 1000;
        }
        //TODO make better.
        /*internal void start() {

        }
        internal double stop() {

        }*/
        /// <summary>
        /// returns in seconds
        /// </summary>
        /// <param name="gameTime">gameTime mannn</param>
        /// <returns>total time since timer started</returns>
        internal double getTime(double gameTime) {
            return gameTime/1000 - startSeconds;
        }
        internal double getFormattedTime(double gameTime, int keep) {
            return Math.Round(gameTime / 1000 - startSeconds, keep);
        }

    }
}
