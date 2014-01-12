using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.stuff;
using ProjectTurtle.src.objects;//TODO make a premade class with all the 'using' 

namespace ProjectTurtle.src.bossfights {
    class BuseyBossFight : BossFight{
        const int MAXPLAYERS = 7;
        const string NAME = "Busey Boss";
        const string BG_PATH = "images/bg";

        internal BuseyBossFight(){
            enemys = getNewBossList();

        }
        internal static BossFightRecord getBossFightRecord() {
            int[] times, smalls;

            times = new int[3] { 30, 40, 50 };//{gold,silver,bronze}
            smalls = new int[3] { 5, 5, 6 };
            return new BossFightRecord(times, smalls);
        }
        static List<Enemy> getNewBossList() {
            List<Enemy> l = new List<Enemy>();
            Enemy boss = new BuseyBoss(new Vector2f(300, 300));
            l.Add(boss);
            return l;
        }
        internal int getEnrageTimer() {
            return 60;
        }
        internal static string getFightDescr() {
            return "busey boss is basically a tank and spank, \nwith adds spawned on main threat every 20s, \nand dont stand in fire!";
        }
        internal static string getName() { return NAME; }

        internal int getMaxPlayers() { return MAXPLAYERS; }

        internal override BossFightID getFightID() { return BossFightID.buseyBoss; }
        internal string getBGString() { return BG_PATH; }

        internal override List<Script> getTimedScripts() {
            List<Script> l = new List<Script>();
            List<saypack> ls = new List<saypack>();
            ls.Add(new saypack("Busey: YEAH IM TALKING TO MYSELF", 6));
            ls.Add(new saypack("Busey: wait who?", 3));
            ls.Add(new saypack("Busey: ME AND YOU?", 3));
            ls.Add(new saypack("Busey: oh well raoroarooaraoror?", 3));

            l.Add(new Script(new SayPackScript(ls)));
            return l;
        }
        internal string getDeathString() {
            return "Busey: Buuuuuuuuuuuuuuuuuuuussssssssssey";
        }
        internal override bool initPhase(int phase) {
            return false;
        }
        internal PlayerClassNum getAddedPlayers() {
            return PlayerClassNum.none;
        }
        internal override BossFightInfo getBossInfo() {
            throw new NotImplementedException();
        }
    }
}
