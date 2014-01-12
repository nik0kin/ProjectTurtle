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
    //TODO make bossfights interfaces, easier to grab info? like bossfightrecord and enrage timer?
    class EvilTomHanksFight : BossFight {
        const int MAXPLAYERS = 20;
        const string NAME = "Tom Hank and Spank";
        const string BG_PATH = "images/backgrounds/boss_tomhanks";

        internal EvilTomHanksFight() {
            enemys = getNewBossList();

        }
        internal static BossFightRecord getBossFightRecord() {
            int[] times, smalls;

            times = new int[3] { 35, 50, 65 };//{gold,silver,bronze}
            smalls = new int[3] { 14, 16, 18 };
            return new BossFightRecord(times, smalls);
        }
        static List<Enemy> getNewBossList() {
            List<Enemy> l = new List<Enemy>();
            Enemy boss = new TomHanks(new Vector2f(225, 200));
            l.Add(boss);
            return l;
        }
        internal int getEnrageTimer() {
            return 75;
        }
        internal static string getFightDescr(){
            return "tom hanks is straight dps race, \nadds spawn on the player with 2nd most threat";
        }
        internal static string getName() { return NAME; }

        internal override List<Script> getTimedScripts() {
            List<Script> l = new List<Script>();
            l.Add(new Script(1, "Tom Hanks: Fu"));
            l.Add(new Script(10, "Tom Hanks: You think you can take me?"));
            return l;
        }
        internal string getDeathString() {
            return "Tom Hanks: Wilson will have my revenge!";
        }

        internal int getMaxPlayers() { return MAXPLAYERS; }

        internal override BossFightID getFightID() { return BossFightID.tomHanks; }
        internal string getBGString() { return BG_PATH; }
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
    //tom hanks spawns adds on the person with second most threat
    class TomHanks : BossI {
        enum cooldowns {
            volley, melee, spawnAdds
        }
        const int BOSS_HP = 2000;
        const int VOLLEY_DMG = 3;
        const int VOLLEY_CD = 5;
        const int VOLLEY_ENRAGE_MULTI = 3;
        const int MELEE_DMG = 7;
        const float MELEE_CD = 1.25f;
        const int SPAWNADDS_CD = 10;

        static Texture meleeProjectileTexture, volleyProjectileTexture;
        static IntRect meleeProjectileRect, volleyProjectileRect;
        static Texture bossCircleTexture;

        internal static new void LoadContent() {
            meleeProjectileTexture = GameBox.loadTexture( "images/abilitys/tomhanksmelee_proj");
            meleeProjectileRect = new IntRect(0, 0, 20, 10);
            volleyProjectileTexture = GameBox.loadTexture("images/abilitys/tomhanksvolley_proj");
            volleyProjectileRect = new IntRect(0, 0, 10, 5);
            bossCircleTexture = GameBox.loadTexture("images/avatars/bosses/hanknspank");
        }
        internal TomHanks(Vector2f v)
            : base(v) {
            enemyCooldowns = new Cooldown[3];
            enemyCooldowns[(int)cooldowns.volley] = new Cooldown(VOLLEY_CD);
            enemyCooldowns[(int)cooldowns.melee] = new Cooldown(MELEE_CD);
            enemyCooldowns[(int)cooldowns.spawnAdds] = new Cooldown(SPAWNADDS_CD);
            currentHp = maxHp = BOSS_HP;
            currentTexture = bossCircleTexture;
            innerCircle = 145;
            circleSize = 300;

        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            aggro = getFirstThreat();
            if (currentHp <= 0) {
                wipeAllThreat();
                return;
            }
            if (aggro == null) {//if no one has aggro put cooldowns on CD (start of fight)
                //enemyCooldowns[(int)cooldowns.spawnAdds].use(gameTime);
                return;
            }
            //melee
            if (!isMoving() && enemyCooldowns[(int)cooldowns.melee].use(gameTime)) {
                InGame.getInstance().addProjectile(new Projectile(this, getMid(), aggro, doMelee, meleeProjectileTexture, meleeProjectileRect));
            }
            //try to aoe-volley
            List<Moveable> players = InGame.getInstance().getAlivePlayerMoveables();
            if (!isMoving() && players.Count > 0 && enemyCooldowns[(int)cooldowns.volley].use(gameTime)) {
                foreach (Moveable mob in players) {
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), mob, doVolley, volleyProjectileTexture, volleyProjectileRect));
                }
            }
            //spawn 3 adds
            if (!isMoving() && enemyCooldowns[(int)cooldowns.spawnAdds].use(gameTime)) {
                InGame.getInstance().addProjectile(new Projectile(this, getMid(), getSecondThreat(), doSpawnAdds, meleeProjectileTexture, meleeProjectileRect));//TODO make a new visual for the projectile that spawns them
            }
        }
        internal bool doVolley(Moveable mob) {
            bool dead = mob.takeDamage(this, VOLLEY_DMG * (enraged ? VOLLEY_ENRAGE_MULTI : 1));
            if (dead) {
                dropThreat(mob);
            }
            return false;
        }
        internal bool doMelee(Moveable mob) {
            bool dead = mob.takeDamage(this, MELEE_DMG);
            if (dead) {
                dropThreat(mob);
            }
            return false;
        }

        internal bool doSpawnAdds(Moveable mob) {
            InGame.getInstance().addMoveable(new BasicAdd(new Vector2f(mob.getMid().X - 10, mob.getMid().Y), mob,this));
            InGame.getInstance().addMoveable(new BasicAdd(new Vector2f(mob.getMid().X + 10, mob.getMid().Y), mob, this));
            InGame.getInstance().addMoveable(new BasicAdd(new Vector2f(mob.getMid().X, mob.getMid().Y - 10), mob, this));
            InGame.getInstance().addMoveable(new BasicAdd(new Vector2f(mob.getMid().X, mob.getMid().Y + 10), mob, this));
            changeAddAmt(4);
            return false;
        }
        internal override bool takeDamage(Moveable giver, float amt) {
            return takeDamage(giver, amt, 1);
        }
        internal override string getCooldownString(double gameTime, int decimalPlaces) {
            return "volley" + enemyCooldowns[0].getFormattedTimeLeft(gameTime, decimalPlaces) + " melee "
                        + enemyCooldowns[1].getFormattedTimeLeft(gameTime, decimalPlaces) + " adds(" + adds + ") " + enemyCooldowns[2].getFormattedTimeLeft(gameTime, decimalPlaces);
        }
    }
}
