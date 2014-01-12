using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.objects;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.screens;

//TODO target system, everyone has one target, and redo the gay selecting no target(should always be able to, even if a cd)
namespace ProjectTurtle.src {
    class Mage : PlayerClassI{
        const int MAGE_HP = 6;
        const float FIREBALL_DMG = 2.5f;
        const int FIREBALL_CD = 2;
        const int FIREBALL_RANGE = 250;
        const float FIREBALL_AOE_PERCENT = .6f;
        const int FIREBALL_AOE_RANGE = 25;
        const float BIGFIREBALL_DMG = 4;
        const int BIGFIREBALL_STACK = 10;
        const float BIGFIREBALL_STACK_CHANCE = .6F;
        const int BIGFIREBALL_RANGE = 300;
        const int BIGFIREBALL_AOE_RANGE = 100;
        //non gameplay constants
        const float FIREBALL_GFX_SCALE = 1.1f, BIGFIREBALL_GFX_SCALE = 1.1f;

        static Texture pyroWepAnimations, pyroBaseTexture, fireballProjAnimTexture;
        static Texture fireballAbilityTexture, bigFireballAbilityTexture;
        static Texture bigFireballAoeProjectileTexture, fireballProjectileTexture, fireballAoeProjectileTexture;
        static IntRect bigFireballAoeProjectileRect, fireballProjectileRect, fireballAoeProjectileRect;
        Cooldown a1;
        StackingBuff sb1;
        bool fireballRepeat, bigFireballRepeat;

        internal static new void LoadContent(){
            //ability button textures
            fireballAbilityTexture = GameBox.loadTexture("images/fire");
            bigFireballAbilityTexture = GameBox.loadTexture("images/abilitys/bigfireball");
           
            //ability projectile textures/rectangles
            fireballProjectileTexture = GameBox.loadTexture("images/red_dot");
            fireballProjectileRect = new IntRect(0, 0, 5, 5);
            fireballAoeProjectileTexture = GameBox.loadTexture("images/abilitys/fireballaoe_proj");
            fireballAoeProjectileRect = new IntRect(0, 0, 2, 2);
            bigFireballAoeProjectileTexture = GameBox.loadTexture("images/abilitys/bigfireball_proj");
            bigFireballAoeProjectileRect = new IntRect(0, 0, 8, 8);

            pyroBaseTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "pyrobase");
            pyroWepAnimations = GameBox.loadTexture(SimpleModel.CLASS_PATH + "pyroanim");
            fireballProjAnimTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "pyroproj");
            
        }
        internal Mage(Vector2f start, string name) {
            playerX = destX = (int)start.X;
            playerY = destY = (int)start.Y;
            type = MoveableEnum.mage;
            circleSize = PLAYER_CIRCLE_SZ;

            currentHp = maxHp = MAGE_HP;
            this.name = name;
            innerCircle = COLLISION_RADIUS;
            buffs = new Buffs(this);

            model = new SimpleModel(pyroBaseTexture, SimpleModel.CLASS_RECT, SimpleModel.CLASS_SCALE);
            boundingRect = SimpleModel.CLASS_BOUNDING_RECT;

            //ability1, fireball, repeating
            a1 = new Cooldown(FIREBALL_CD);
            fireballRepeat = true;
            bigFireballRepeat = true;

            //ability2 big fireball, everytime you use ability one, 
            //you have a chance(60%), gain a stacking buff til 10, then you do a big fireball
            sb1 = new StackingBuff(this, BIGFIREBALL_STACK);

            weaponPlayer = new WeaponPlayer(pyroWepAnimations, .3f, .75f);
        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            if (currentHp <= 0) return;

            if (!isMoving() && (fireballRepeat) && target != null && !target.isDead()) {
                //game stuff then
                if (checkCircleRangeAndMove(target, FIREBALL_RANGE, gameTime)) {
                    //do nothing because you are running
                } else if (a1.use(gameTime)) {
                    weaponPlayer.doAnimation1();
                    Animation fireballAnim = new Animation(fireballProjAnimTexture, new Vector2f(0,0), 3, new Vector2f(24, 20), .2f, Animation.Type.looping, "fireballproj");
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), target, doFireball, fireballAnim, FIREBALL_GFX_SCALE, false));
                }

            }
            if (!isMoving() && (bigFireballRepeat) && target != null && !target.isDead()) {
                //game stuff then
                if (checkCircleRangeAndMove(target, BIGFIREBALL_RANGE, gameTime)) {
                    
                } else if (sb1.use()) {
                    weaponPlayer.doAnimation1();
                    Animation bigFireballAnim = new Animation(fireballProjAnimTexture, new Vector2f(3*24,0), 3, new Vector2f(24, 20), .2f, Animation.Type.looping, "bigfireballproj");
                    Projectile proj = new Projectile(this, getMid(), target, doBigFireball, bigFireballAnim, BIGFIREBALL_GFX_SCALE, false);

                    InGame.getInstance().addProjectile(proj);
                }

            }
            //update abilitys effected by cdReduc
            a1.setCDReduc(buffs.getValue(BuffType.CDreduc));
        }
        internal override bool doAbility() {
            return false;
        }
        internal override Cooldown getFirstCooldown() {
            return a1;
        }
        internal override Cooldown getSecondCooldown() {
            return null;
        }
        internal override Cooldown getThirdCooldown() {
            return null;
        }
        internal override void setTarget(Moveable m, double gameTime) {
            target = m;
        }

        internal override Texture getFirstAbilityTexture() {
            return fireballAbilityTexture;
        }
        internal override Texture getSecondAbilityTexture() {
            return bigFireballAbilityTexture;
        }
        internal override Texture getThirdAbilityTexture() {
            return null;
        }
        //damagee = person who gets damaged
        internal bool doFireball(Moveable damagee) {
            damagee.takeDamage(this, FIREBALL_DMG);
            if (InGame.randy.NextDouble() <= BIGFIREBALL_STACK_CHANCE) 
                sb1.stack(1);
            //aoe
            List<Moveable> enemys = InGame.getInstance().getAliveEnemyMoveables(damagee, FIREBALL_AOE_RANGE);
            foreach (Moveable mob in enemys) {
                if (!mob.isDead() && mob != damagee) {
                    InGame.getInstance().addProjectileFromProj(new Projectile(this, damagee.getMid(), mob,
                            doFireballAoe, fireballAoeProjectileTexture, fireballAoeProjectileRect));
                }
            }
            
            return true;
        }
        internal bool doFireballAoe(Moveable damagee) {
            damagee.takeDamage(this, FIREBALL_DMG * FIREBALL_AOE_PERCENT);
            return true;
        }
        internal bool doBigFireball(Moveable damagee) {
            damagee.takeDamage(this, BIGFIREBALL_DMG);
            //aoe
            List<Moveable> enemys = InGame.getInstance().getAliveEnemyMoveables(damagee, BIGFIREBALL_AOE_RANGE);
            foreach (Moveable mob in enemys) {
                if (!mob.isDead() && mob != damagee) 
                    InGame.getInstance().addProjectileFromProj(new Projectile(this, damagee.getMid(), mob, doBigFireballAoe, 
                            bigFireballAoeProjectileTexture,bigFireballAoeProjectileRect));
            }
            return true;
        }
        internal bool doBigFireballAoe(Moveable damagee) {
            damagee.takeDamage(this, BIGFIREBALL_DMG);
            return true;
        }
        internal override int getRange() {
            return FIREBALL_RANGE;
        }
    }
}
