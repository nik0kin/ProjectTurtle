using System;
using System.Collections.Generic;
using System.Linq;









using ProjectTurtle.src;
using ProjectTurtle.src.util;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.stuff {
    class Script {
        enum type {
            path, sayThis, sayPack
        };
        bool enable = true;
        type mType; 
        double startTime;//seconds

        Path path;
        string sayThis;
        SayPackScript sayPack;

        internal Script(int startTime, Path path) {
            this.startTime = startTime;
            mType = type.path;
            this.path = path;
        }
        internal Script(int startTime, string sayThis) {
            this.startTime = startTime;
            mType = type.sayThis;
            this.sayThis = sayThis;
        }
        internal Script(SayPackScript sayPack) {
            mType = type.sayPack;
            this.sayPack = sayPack;
        }

        internal void update(double gameTime,Timer timer){
            if (!enable) return;
            double currentSeconds = timer.getTime(gameTime);
            switch(mType){
            case type.path:
                if (currentSeconds >= startTime && path.update(gameTime, currentSeconds))
                    enable = false;
                break;
            case type.sayPack:
                if (sayPack.update(currentSeconds)) 
                    enable = false;
                break;
            case type.sayThis:
                if (startTime <= currentSeconds) {
                    InGame.getInstance().addScreenNote(sayThis);
                    enable = false;
                }
                break;
            }
        }

    }
}
