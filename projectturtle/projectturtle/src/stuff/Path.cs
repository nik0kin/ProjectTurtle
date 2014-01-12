using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src;
using ProjectTurtle.src.util;
using ProjectTurtle.src.objects;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.stuff {
    class Path {
        Moveable mob;
        List<pathPoint> path;

        int i;
        bool done, sent;
        double waittime;
        internal Path(Moveable m){
            mob = m;
            path = new List<pathPoint>();
        }
        internal void addLoc(Vector2f v) {
            path.Add(new pathPoint(v, 0,null));
        }
        internal void addLoc(Vector2f v, string s) {
            path.Add(new pathPoint(v, 0,s));
        }
        internal void addLoc(Vector2f v, float waitTimeSeconds) {
            path.Add(new pathPoint(v, waitTimeSeconds,null));
        }
        internal void addLoc(Vector2f v, float waitTimeSeconds, string s) {
            path.Add(new pathPoint(v, waitTimeSeconds,s));
        }
        //set waittime, and wait if needed?
        //run to first point,
        //while not there, do nothing
        //once there, increment point count
        //and do wait amount
        internal bool update(double gameTime, double seconds) {
            if (done || i >= path.Count) { 
                done = true; 
                return done; 
            }

            if (waittime == 0 && path[i].waitTime != 0) {//not waiting, but need to
                waittime = seconds;
            } else if (waittime + path[i].waitTime >= seconds) {
                //waiting
            } else if (!sent) {
                mob.sendPlayerTo(path[i].point,gameTime);
                sent = true;
            } else if (mob.getMid().X == path[i].point.X && mob.getMid().Y == path[i].point.Y) {
                //made it, re route
                if (path[i].say != null)
                    InGame.getInstance().addScreenNote(path[i].say);
                i++;
                waittime = 0;
                sent = false;
            } else {
                //in transit
                Console.WriteLine("intransit");
            }

            return done;
        }

        struct pathPoint {
            internal Vector2f point;
            internal float waitTime;
            internal string say;

            internal pathPoint(Vector2f p, float waitT, string s) {
                point = p;
                waitTime = waitT;
                say = s;
            }
        }
    }
    class SayPackScript {
        List<saypack> sayPack;
        bool done;
        int current, i;
        internal SayPackScript(List<saypack> sayP) {
            sayPack = sayP;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns>returns if its done</returns>
        internal bool update(double seconds) {
            if(i >= sayPack.Count) done = true;
            if (done) return done;

            if (current + sayPack[i].times <= seconds) {
                current = current + sayPack[i].times;

                InGame.getInstance().addScreenNote(sayPack[i].sayString);
                i++;
            }


            return done;
        }
    }
    //you put these in a list, and each time is the time since the last say string in the list
    internal struct saypack {
        internal string sayString;
        internal int times;

        internal saypack(string s, int timeAfter) {
            sayString = s;
            times = timeAfter;
        }
    }
}
