using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;









using ProjectTurtle.src.util;
using ProjectTurtle.src.objects;

namespace ProjectTurtle.src.util {
    class StackingBuff {
        int count = 0;
        int max;
        internal StackingBuff(int max) {
            this.max = max;
        }
        internal StackingBuff(Moveable onWho, int max){
            this.max = max;
        }
        internal void stack(int amt) {
            count += amt;
            if (count > max)
                count = max;
        }
        //returns if it can be used and does resets the stacks if it can
        internal bool use() {
            bool r = count == max;
            if (r) {
                count = 0;
            }
            return r;
        }
        //isAtMaxStack 
        internal bool ready() {
            return count == max;
        }
        internal void reset() {
            count = 0;
        }
        internal int getCount() {
            return count;
        }
    }
}
