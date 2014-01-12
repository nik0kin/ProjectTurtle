using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.stuff;
using ProjectTurtle.src.objects;
using ProjectTurtle.src;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.util;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.bossfights {
    class CheckDPSBossFight : BossFight {
        const int MAXPLAYERS = 20;
        const string NAME = "DPS Check Boss";
        const string BG_PATH = "images/backgrounds/boss_tomhanks";

        internal CheckDPSBossFight(){
            enemys = getNewBossList();
        }
        internal static BossFightRecord getBossFightRecord() {
            int[] times, smalls;

            times = new int[3] { 0, 0, 0 };//{gold,silver,bronze}
            smalls = new int[3] { 0, 0, 0 };
            return new BossFightRecord(times, smalls);
        }
        static List<Enemy> getNewBossList() {
            List<Enemy> l = new List<Enemy>();
            Enemy boss = new CheckDPSBoss(new Vector2f(225, 200));
            l.Add(boss);
            return l;
        }
        internal int getEnrageTimer() {
            return 300;
        }
        internal static string getFightDescr(){
            return "this boss is to check dps";
        }
        internal static string getName() { return NAME; }

        internal int getMaxPlayers() { return MAXPLAYERS; }

        internal override BossFightID getFightID() { return BossFightID.dpsCheck; }
        internal string getBGString() { return BG_PATH; }

        internal override List<Script> getTimedScripts() {
            return null;
        }
        internal string getDeathString() {
            return "dps check win";
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
    class CheckDPSBoss : BossI {
        enum cooldowns {
            volley, melee, spawnAdds
        }
        const int BOSS_HP = 3000;


        static Texture bossCircleTexture;

        internal static new void LoadContent() {

            bossCircleTexture = GameBox.loadTexture("images/avatars/circle_meat");
        }
        internal CheckDPSBoss(Vector2f v)
            : base(v) {
            enemyCooldowns = new Cooldown[0];

            currentHp = maxHp = BOSS_HP;
            currentTexture = bossCircleTexture;
            innerCircle = 150;
            circleSize = 330;

        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            aggro = getFirstThreat();
            if (currentHp <= 0) {
                wipeAllThreat();
                return;
            }
        }
        internal override bool takeDamage(Moveable giver, float amt) {
            return takeDamage(giver, amt, 1);
        }
        internal override string getCooldownString(double gameTime, int decimalPlaces) {
            return "no cooldowns";
        }
    }
}
