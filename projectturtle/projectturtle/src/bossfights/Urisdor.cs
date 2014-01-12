using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src;
using ProjectTurtle.src.objects;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.util;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.bossfights{
	//deciding to write the fish within the ursidor bossclass
	internal class UrsidorBossFight  : BossFight{
        const int MAXPLAYERS = 6;
        const string FIGHTNAME = "Urisdor";
        static string[] BG_PATH = {"images/ENVIRONMENTS/1_forest/3_ursidor_base","images/ENVIRONMENTS/1_forest/3_ursidor_water"};
		const int ENRAGETIMER = 160;
        
		static BossFightInfo myInfo;
        //static Texture fishTexture;
        static FishObject fish;//should this be static, but it need to be

        internal static void LoadContent(){
            Ursidor.LoadContent();
            FishObject.LoadContent();
        }
        internal UrsidorBossFight(){
            enemys = getNewBossList();
            fightObjects = new List<FightObject>();
            fightObjects.Add(fish);
        }
		
        static List<Enemy> getNewBossList() {
            fish = new FishObject(new Vector2f(510,510));
            List<Enemy> l = new List<Enemy>();
            Enemy boss = new Ursidor(new Vector2f(250, 305),fish);
            l.Add(boss);
            return l;
        }
        internal override List<Script> getTimedScripts() {
            List<Script> l = new List<Script>();
            /*List<saypack> ls = new List<saypack>();
            ls.Add(new saypack("Busey: YEAH IM TALKING TO MYSELF", 6));
            ls.Add(new saypack("Busey: wait who?", 3));
            ls.Add(new saypack("Busey: ME AND YOU?", 3));
            ls.Add(new saypack("Busey: oh well raoroarooaraoror?", 3));

            l.Add(new Script(new SayPackScript(ls)));*/
            return l;
        }

        internal override bool initPhase(int phase) {
            switch (phase) {
            case 1:
                enemys[0].resetCooldowns(GameBox.getInstance().getGameTime());
                break;
            }
            return false;
        }
		
		internal static BossFightInfo getBossInfoS(){
			if(myInfo.name != null) return myInfo;
			myInfo = new BossFightInfo();
			
			int[] times, smalls;
            times = new int[3] { 60, 70, 80 };//{gold,silver,bronze}
            smalls = new int[2] { 4, 5 };
            /*myInfo.records = new BossFightRecord(times, smalls);
			//no added lts
			myInfo.addedLts = null;//new List<PlayerClassNum>().Add(PlayerClassNum.none);
			myInfo.backdrop = new Backdrop(BG_PATH,new FloatRect(),new FloatRect());
			myInfo.desc = "urisdor fight desc";
			myInfo.name = FIGHTNAME;
			myInfo.maxplayers = MAXPLAYERS;				
			myInfo.enragetimer = ENRAGETIMER;*/

            myInfo = new BossFightInfo(FIGHTNAME, MAXPLAYERS, ENRAGETIMER, "ursidor fight desc", null,
                new BossFightRecord(times, smalls), new Backdrop(BG_PATH, new FloatRect(), new FloatRect()));
			return myInfo;
		}
        internal override BossFightInfo getBossInfo() {
            return getBossInfoS();
        }
        internal override BossFightID getFightID() {
            return BossFightID.urisdor;
        }
       // internal override 
		
	}
	internal class Ursidor : BossI{
        enum cooldowns {
            melee, swipe,
            hungry //we wont treat hungry as a regular cd
        }
		//Hungry Buff-> Charge
		// -happens at 66% and 33%
		//  when he needs to be kited to the river/fish
		//  this buffs the boss X% haste
        const int BOSS_HP = 400;
        const int STUN_DMG = 6;
		const int STUN_AOE_DMG = 3;
        const short HUNGRY_CD = 20;//time til first one?
        const float HUNGERED_RAGE_HASTE_PERCENT = .45f;//45% more dmg
        const int MELEE_DMG = 3;
        const int MELEE_RANGE = 60;
        const float MELEE_CD = 1.4f;//tank is taking 21 from melee and 
        const int SWIPE_DMG = 3;
        const float SWIPE_CD = 3.0f;
        const int FISHING_DIST = 100;
		//visual constants
		const int INNERCIRCLE = 50;//inner radius
        new const int CIRCLE_SZ = 60;//radius
        const float MODEL_SCALE = 1.0f;
        static Vector2f ANIMATION_FRAME_SIZE = new Vector2f(250, 220);

        static Texture ursiBaseTexture, hungeredwarningTexture, chargedwarningTexture;
        static Animation fishEatAnimation, attackAnimation;
        

        Buff hungered; //just a reference to it
        FightObject fish;
        //TODO make load contenting thru bossfight library?, so we can unload content quickly too
        internal new static void LoadContent() {
            ursiBaseTexture = GameBox.loadTexture(SimpleModel.BOSS_PATH + "ursidor/ursidorbase");//images\characters\bosses\ursidor
            Texture t = GameBox.loadTexture(SimpleModel.BOSS_PATH + "ursidor/ursidoranim");
            fishEatAnimation = new Animation(t, new Vector2f(0, 4 * ANIMATION_FRAME_SIZE.Y), 3, ANIMATION_FRAME_SIZE, .25f, Animation.Type.normal, "fisheat");
            attackAnimation = new Animation(t, new Vector2f(0,0), 3, ANIMATION_FRAME_SIZE, .175f, Animation.Type.normal, "attack");
            attackAnimation.setDirectional(true);

            hungeredwarningTexture = GameBox.loadTexture("images/UI/warning");
            chargedwarningTexture = GameBox.loadTexture("images/UI/chargewarning");
        }
		//define your cooldowns and base stats
        internal Ursidor(Vector2f v,FightObject fish) : base(v){
            enemyCooldowns = new Cooldown[3];
            enemyCooldowns[(int)cooldowns.melee] = new Cooldown(MELEE_CD);
            enemyCooldowns[(int)cooldowns.swipe] = new Cooldown(SWIPE_CD);
            enemyCooldowns[(int)cooldowns.hungry] = new Cooldown(HUNGRY_CD);

            currentHp = maxHp = BOSS_HP;

            model = new SimpleModel(ursiBaseTexture, new IntRect(0,0,120,210), MODEL_SCALE,2.3f);

            boundingRect = new IntRect(0, 0, 80, 160);//bad rect im sure

            innerCircle = INNERCIRCLE;
            circleSize = CIRCLE_SZ;

            this.fish = fish;
        }
		internal override void Update(double gameTime){
            base.Update(gameTime);           
            
            //if(isAnimating()) return;

            //melee
            if (chargeTarget != null && !chargeTarget.isDead() ){
                if (!isMoving() && !checkRangeAndMove(chargeTarget.getMid(), MELEE_RANGE, gameTime) && enemyCooldowns[(int)cooldowns.melee].use(gameTime)) {
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), chargeTarget, doMelee, null, new IntRect()));
                    model.setAnimation(attackAnimation);
                    if (charging) stopCharge();
                    overrideDirection = getDirectionOfUnit(chargeTarget);
                }
            }else if (aggro != null && !isMoving() && enemyCooldowns[(int)cooldowns.melee].use(gameTime) && !checkRangeAndMove(aggro.getMid(), MELEE_RANGE, gameTime)) {
                InGame.getInstance().addProjectile(new Projectile(this, getMid(), aggro, doMelee, null, new IntRect()));
                model.setAnimation(attackAnimation);
            }
            //hungry
            if (hungered == null && enemyCooldowns[(int)cooldowns.hungry].ready(gameTime)) {
                hungered = new Buff(BuffType.physicalHaste, HUNGERED_RAGE_HASTE_PERCENT);
                buffs.addBuff(hungered);
                InGame.getInstance().setWarningFG(hungeredwarningTexture);
                model.setRing(Color.Red);
                InGame.getInstance().addScreenNote("HUNGERED RAGEE");
                Console.WriteLine("hungered rage");
            }
			//fish
            if (hungered != null && !model.isAnimating() && InGame.getDistance(this.getMid(), fish.getMid()) < FISHING_DIST) {   
                Console.WriteLine("fish");
                doFishEating(gameTime);
            }
			//charge (check if tank is back in range, to pick up aggro?
            if (!charging && chargeTarget != null) {
                if (InGame.getDistance(this.getMid(), aggro.getMid()) < MELEE_RANGE) {
                    chargeTarget = null;
                    Console.WriteLine("taking off charge target");
                    overrideDirection = null;
                }
            }
            
            //update cds?
            enemyCooldowns[(int)cooldowns.melee].setCDReduc(buffs.getValue(BuffType.physicalHaste));
        }
		internal void doFishEating(double gametime){
			//we just got inrange of the fish object
            buffs.remove(hungered);
            InGame.getInstance().resetWarningFG();
            //hungered = null;
            enemyCooldowns[(int)cooldowns.hungry].use(gametime);
            model.setRing(null);
            InGame.getInstance().addScreenNote("HUNGERED RAGE SWOOTHED");

			//start animation, and dont attack during this (Animation.isAnimating bool? in update)
            model.setAnimation(fishEatAnimation, startCharge);
			
		}
        public bool charging = false;
        PlayerClassI chargeTarget;//charge target gets changed when the original target is in range
        internal bool startCharge(){
			//choose random raid unit
            do{
			    chargeTarget = InGame.getInstance().getRandomAlivePlayer();
            }while(chargeTarget == aggro);
            hungered = null;
            charging = true;
			//gain charge buff
            model.setRing(Color.Green);
            InGame.getInstance().addScreenNote("CHARGE AT "+chargeTarget.getName());
            InGame.getInstance().setWarningFG(chargedwarningTexture);
            runspeed = DEFAULT_RUNSPEED / 4;
            Console.WriteLine("startCharge");
            return true;
		}
        internal void stopCharge() {
            charging = false;
            //take away charge buff
            model.setRing(null);
            InGame.getInstance().resetWarningFG();
            runspeed = DEFAULT_RUNSPEED;
            Console.WriteLine("stopCharge");


        }
        internal bool doMelee(Moveable mob) {
            bool dead = mob.takeDamage(this, MELEE_DMG);
            if (dead) {
                dropThreat(mob);
            }
            return false;
        }
		internal bool doChargeDMG(Moveable attackee){
			bool dead = attackee.takeDamage(this, MELEE_DMG);
            if (dead) {
                dropThreat(attackee);
            }
			//than do the knockback effect
			//Movement.knockback(attackee,new Vector2f());
            return false;
		}
		internal bool doChargeAOE(Moveable attackee){
			bool dead = attackee.takeDamage(this, MELEE_DMG);
            if (dead) {
                dropThreat(attackee);
            }
			//Movement.knockback(attackee,new Vector2f());
            return false;
		}
        internal override string getCooldownString(double gameTime,int decimalPlaces) {
            return "depreciated";
		}
    }
    internal class FishObject : FightObject {
        static Texture fishText;
        Cooldown flutter;
        static Animation flutterAni;

        internal static void LoadContent(){
            if (fishText != null) return;
            fishText = GameBox.loadTexture(SimpleModel.BOSS_PATH + "ursidor/fishbase");
        }

        internal FishObject(Vector2f position)
            : base(fishText, position, new IntRect(0, 0, 24, 20), true) {
            flutter = new Cooldown(5.0f);
            flutterAni = new Animation(fishText, new Vector2f(0,0), 2, new Vector2f(24, 20), .3f, Animation.Type.fronttobacktofrontend, "flutter");
        }
        internal override void update(double gameTime) {
            if (flutter.use(gameTime))
                animationPlayer.PlayAnimation(flutterAni);
        }
    }
			
}


