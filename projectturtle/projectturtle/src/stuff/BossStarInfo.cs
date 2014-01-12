using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTurtle.src.stuff {
    class BossStarInfo {
        internal bool winStar;
        internal bool[] timeStars;
        internal double seconds;//TODO make all these have gets/sets
        internal bool[] smallStars;
        internal int smallest;

        internal BossStarInfo() {
            winStar = false;
            timeStars = new bool[3];
            smallStars = new bool[3];
            smallest = 9999;
            seconds = 9999;
        }

        internal int getStarCount() {
            int t = 0;
            t += winStar ? 1 : 0;
            t += timeStars[0] ? 1 : 0;
            t += timeStars[1] ? 1 : 0;
            t += timeStars[2] ? 1 : 0;
            t += smallStars[0] ? 1 : 0;
            t += smallStars[1] ? 1 : 0;
            t += smallStars[2] ? 1 : 0;

            return t;
        }
    }
}
