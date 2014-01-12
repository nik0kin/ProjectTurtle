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
using ProjectTurtle.src.screens;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.util;

namespace ProjectTurtle.src.bossfights {
    class EvilFraiserFight : BossFight {
        const int MAXPLAYERS = 9;//10 with good fraizer
        const int ENRAGE = 80;
        const string NAME = "Fraizer";
        const string BG_PATH = "images/backgrounds/boss_fraiser";

        EdwardDog dog;
        EvilFraiser boss;

        internal static void LoadContent(){
            EvilFraiser.LoadContent();
            EdwardDog.LoadContent();
        }
        internal EvilFraiserFight() {
            enemys = new List<Enemy>();
            dog = new EdwardDog(new Vector2f(400, 200),this);
            boss = new EvilFraiser(new Vector2f(671, 450), dog);
            dog.setLeasher(boss);
            boss.setVisable(false);
            enemys.Add(dog);
            enemys.Add(boss);
            
        }
        internal static BossFightRecord getBossFightRecord() {
            int[] times, smalls;

            times = new int[3] { 50, 60, 70 };//{gold,silver,bronze}
            smalls = new int[3] { 7, 8, 9 };
            return new BossFightRecord(times, smalls);
        }
        static List<Enemy> getNewBossList() {
            
            return null;
        }
        internal int getEnrageTimer() {
            return ENRAGE;
        }
        internal static string getFightDescr(){
            return "fraizer is a two phase fight\n"+
                    "first phase starts with fraizer's dog, tank and \n" +
                    "spank him til x%, then the fraizers come out.\n" +
                    "fraizer's dog gets leashedand does less dmg, \n" +
                    "but he will bite now, causing a debuff that \n" + 
                    "must be dispelled by standing near good fraizer";
        }
        internal static string getName() { return NAME; }

        internal int getMaxPlayers() { return MAXPLAYERS; }

        internal override BossFightID getFightID() { return BossFightID.fraiser; }
        internal string getBGString() { return BG_PATH; }

        internal override List<Script> getTimedScripts() {
            List<Script> l = new List<Script>();
            return l;
        }
        internal string getDeathString() {
            return "Frazier: deathstring";
        }
        internal override bool initPhase(int phase) {
            if (phase < 1 || phase > 3) return false;
            if (phase == 2) {
                //enable main boss
                Path p = new Path(boss);
                boss.setVisable(true);
                p.addLoc(new Vector2f(450, 475), "Frazier: noooooooo pupster!");
                p.addLoc(new Vector2f(450, 350));
                p.addLoc(new Vector2f(600,200));
                InGame.getInstance().addScript(new Script(1, p));
                boss.updateThreat(dog.getFirstThreat(), 5);
                boss.updateThreat(InGame.getInstance().getAlivePlayerMoveables(), 1);
                boss.reOrgThreat();
                InGame.getInstance().setMainEnemy(boss);
                //and debuff dog
                dog.setLeashed(true);
                return true;
            } else if (!dogEnrage && !boss.isDead() && phase == 3) {
                //enrage boss
                boss.setDogEnrage();
                InGame.getInstance().addScreenNote("FRAIZER: NOOOOOOOOO EDDDI");
                dogEnrage = true;
            }
            return true;
        }
        bool dogEnrage = false;
        internal PlayerClassNum getAddedPlayers() {
            return PlayerClassNum.goodfraiser;
        }
        internal override BossFightInfo getBossInfo() {
            throw new NotImplementedException();
        }
    }
    class EvilFraiser : BossI {
        enum cooldowns {
            volley, melee, bite
        }
        const int BOSS_HP = 600;
        const int VOLLEY_DMG = 2;
        const int VOLLEY_CD = 2;
        const int VOLLEY_ENRAGE_MULTI = 2;
        const int MELEE_DMG = 3;
        const float MELEE_CD = 1.0f;
        const int BITE_CD = 12;

        const float ENRAGE_DOG = 2.0F;

        static Texture meleeProjectileTexture, volleyProjectileTexture;
        static IntRect meleeProjectileRect, volleyProjectileRect;
        static Texture bossCircleTexture;

        EdwardDog dog;

        internal static new void LoadContent() {
            meleeProjectileTexture = GameBox.loadTexture( "images/abilitys/tomhanksmelee_proj");
            meleeProjectileRect = new IntRect(0, 0, 20, 10);
            volleyProjectileTexture = GameBox.loadTexture("images/abilitys/tomhanksvolley_proj");
            volleyProjectileRect = new IntRect(0, 0, 10, 5);
            bossCircleTexture = GameBox.loadTexture("images/avatars/circle_fraiser_evil");
        }
        internal EvilFraiser(Vector2f v, EdwardDog ed)
            : base(v) {
            enemyCooldowns = new Cooldown[3];
            enemyCooldowns[(int)cooldowns.volley] = new Cooldown(VOLLEY_CD);
            enemyCooldowns[(int)cooldowns.melee] = new Cooldown(MELEE_CD);
            enemyCooldowns[(int)cooldowns.bite] = new Cooldown(BITE_CD);
            currentHp = maxHp = BOSS_HP;
            currentTexture = bossCircleTexture;
            innerCircle = 40;
            circleSize = 100;

            dog = ed;
        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            aggro = getFirstThreat();
            if (currentHp <= 0) {
                wipeAllThreat();
                return;
            }
            if (aggro == null) {//if no one has aggro put cooldowns on CD (start of fight)
                //enemyCooldowns[(int)cooldowns.bite].use(gameTime);
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
            //do bite
            if (!isMoving() && enemyCooldowns[(int)cooldowns.bite].use(gameTime)) {
                dog.doBite(InGame.getInstance().getRandomAlivePlayer());
            }
        }

        internal bool doVolley(Moveable mob) {
            float volleyDMG = VOLLEY_DMG * (enraged || dogEnrage ? VOLLEY_ENRAGE_MULTI : 1);
            bool dead = mob.takeDamage(this, volleyDMG);
            if (dead) {
                dropThreat(mob);
            }
            return false;
        }
        internal bool doMelee(Moveable mob) {
            bool dead = mob.takeDamage(this, MELEE_DMG * (dogEnrage ? ENRAGE_DOG : 1));
            if (dead) {
                dropThreat(mob);
            }
            return false;
        }
        
        internal override string getCooldownString(double gameTime, int decimalPlaces) {
            return "1st_threat = " + (getFirstThreat() == null ? "none" : getFirstThreat().getName()) + " volley" + enemyCooldowns[0].getFormattedTimeLeft(gameTime, decimalPlaces) + " melee "
                        + enemyCooldowns[1].getFormattedTimeLeft(gameTime, decimalPlaces) + " bite " + enemyCooldowns[2].getFormattedTimeLeft(gameTime, decimalPlaces);
        }
        bool dogEnrage = false;
        internal void setDogEnrage() {
            dogEnrage = true;
        }
    }
    //if the dog is commanded to bite, it switches aggro to the random target
    //and next melee puts a debuff on the target (no time limit, )
    class EdwardDog : BossI {
        const int DOG_HP = 400;

        const int MELEE_RANGE = 40;
        const int MELEE_DMG = 3;
        const float MELEE_CD = 1.5f;
        const float UNLEASHED_DMG_BUFF = .5f; //percent
        const float PHASE_2_TRANSITION_PERCENT = .75f;
        //bite
        const int BITE_RANGE = 25;
        const float BITE_DMG = 3;
        const int BITE_TOTAL_TIME = 15;
        const int BITE_TICK_TIME = 3;

        static Texture meleeProjectileTexture, volleyProjectileTexture;
        static IntRect meleeProjectileRect, volleyProjectileRect;
        static Texture bossCircleTexture;
        static Texture leashTexture, biteTexture;
        
        BossFight myBossFight;
        Moveable boss;
        bool leashed = false;
        Moveable biteTarget;
        Sprite leashSprite;

        enum dogState {
            p1, p2, biting, moving
        };
        dogState dState = dogState.p1;
        internal static new void LoadContent() {
            meleeProjectileTexture = GameBox.loadTexture("images/abilitys/tomhanksmelee_proj");
            meleeProjectileRect = new IntRect(0, 0, 20, 10);
            volleyProjectileTexture = GameBox.loadTexture("images/abilitys/tomhanksvolley_proj");
            volleyProjectileRect = new IntRect(0, 0, 10, 5);
            bossCircleTexture = GameBox.loadTexture("images/avatars/circle_edward");
            leashTexture = GameBox.loadTexture("images/abilitys/fraizer/leash");
            biteTexture = GameBox.loadTexture("images/abilitys/fraizer/bite_big");
        }
        internal EdwardDog(Vector2f start, BossFight bf)
            : base(start) {
            myBossFight = bf;
            enemyCooldowns = new Cooldown[1];
            currentHp = maxHp = DOG_HP;
            currentTexture = bossCircleTexture;
            innerCircle = 30;
            circleSize = 75;

            enemyCooldowns[0] = new Cooldown(MELEE_CD);

            leashSprite = new Sprite(leashTexture);
        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            aggro = getFirstThreat();
            if (currentHp <= 0) {
                myBossFight.initPhase(3);//EFF make it so it doesnt spam this?buts ok i guess
                wipeAllThreat();
                return;
            } else if (dState == dogState.p1 && getHPpercent() <= PHASE_2_TRANSITION_PERCENT) {
                dState = dogState.p2;
                myBossFight.initPhase(2);
            }
            if (aggro == null) {//if no one has aggro put cooldowns on CD (start of fight)
                //enemyCooldowns[(int)cooldowns.spawnAdds].use(gameTime);
                return;
            }
            //try biting
            if (dState == dogState.biting) {
                if (checkRangeAndMove(biteTarget.getMid(), BITE_RANGE, gameTime)) {//get close
                    
                } else {// then bite
                    //InGame.getInstance().addProjectile(new Projectile(this, getMid(), biteTarget, doMelee, meleeProjectileTexture, meleeProjectileRect));
                    if (InGame.isInRange(getMid(), biteTarget.getMid(), BITE_RANGE)) {//idk how it gets to this if it is still runnning
                        Buff b = new Buff(gameTime, BuffType.physicalDOT, BITE_TOTAL_TIME, doBiteDmg, BITE_TICK_TIME);
                        b.addAvatarTexture(biteTexture);

                        biteTarget.addBuff(b);
                        dState = dogState.p2;
                    }
                }
            }
            //melee
            else if (!isMoving() && enemyCooldowns[0].use(gameTime)) {
                if (checkRangeAndMove(aggro.getMid(), MELEE_RANGE, gameTime)) {//get close

                } else
                InGame.getInstance().addProjectile(new Projectile(this, getMid(), aggro, doMelee, meleeProjectileTexture, meleeProjectileRect));
            }
             
        }
        internal bool doMelee(Moveable mob) {
            bool dead = mob.takeDamage(this, MELEE_DMG * (leashed ? 1: (1 + UNLEASHED_DMG_BUFF)));
            if (dead) {
                dropThreat(mob);
            }
            return false;
        }
        internal override bool takeDamage(Moveable giver, float amt) {
            return takeDamage(giver, amt, 1);
        }
        internal void doBite(Moveable m) {
            if (isDead()) return;
            dState = dogState.biting;
            biteTarget = m;
            InGame.getInstance().addScreenNote("BITE ON "+m.getName());
        }
        internal bool doBiteDmg(Moveable m) {
            m.takeDamage(this, BITE_DMG);
            return false;
        }

        internal void setLeashed(bool p) {
            leashed = p;
        }
        internal void setLeasher(Moveable m) {
            boss = m;
        }
        internal override void Draw(double gameTime, RenderWindow window, bool selected) {
            base.Draw(gameTime, window, selected);
            if (!isDead() && leashed && InGame.getDistance(boss.getMid(), getMid()) < 300) Beam.drawBeam(window, getMid(), boss.getMid(), leashSprite);
        }
        internal override string getCooldownString(double gameTime, int decimalPlaces) {
            return "1st_threat = " + (getFirstThreat() == null ? "none" : getFirstThreat().getName()) + " melee" + enemyCooldowns[0].getFormattedTimeLeft(gameTime, decimalPlaces) 
                    + "leashed = " + leashed;
        }
    }
}
