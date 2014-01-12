using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.screens;
using ProjectTurtle.src.util;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src;
using ProjectTurtle.src.effects;
//Archer
//you must have direct line of sight of the boss for first ability
//second ability: must hit with reg arrow 5 times and cd 


namespace ProjectTurtle.src.objects {
    class Archer : PlayerClassI {
        const int ARCHER_HP = 7;
        const float MELEE_AMT = 1.90f;
        const float MELEE_CD = .75f;
        const int MELEE_RANGE = 300;
        const float BIGHIT_AMT = 10;
        const int BIGHIT_STACK_NEEDED = 10;
        const float BIGHIT_CD = 25.0f;
        //animation const
        const float ARROW_GFX_SCALE = .7f;

        static Texture archBaseTexture, archWepAnimation;
        static Texture archerHitAbilityTexture, archerBigAbilityTexture;
        static Texture archProjectileTexture, archerBigProjectileTexture;
        static IntRect archerHitProjectileRect, archerStunProjectileRect;
        Cooldown a1, a2;
        StackingBuff stack;
        //bool hitRepeat, bigHitRepeat;
        //bool doOnce = false;

        internal static new void LoadContent() {
            archerHitAbilityTexture = GameBox.loadTexture("images/abilitys/archerHit");
            archerBigAbilityTexture = GameBox.loadTexture("images/abilitys/archerBigHit");

            archBaseTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "archbase");
            archWepAnimation = GameBox.loadTexture(SimpleModel.CLASS_PATH + "archanim");
            //projectile texture/rects
            archProjectileTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "archproj");
            archerHitProjectileRect = new IntRect(0, 0, 10, 60);
            archerBigProjectileTexture = GameBox.loadTexture("images/abilitys/archerbighit_proj");
            archerStunProjectileRect = new IntRect(0, 0, 10, 10);
        }
        internal Archer(Vector2f start, string name) {
            playerX = destX = (int)start.X;
            playerY = destY = (int)start.Y;
            circleSize = PLAYER_CIRCLE_SZ;
            
            currentHp = maxHp = ARCHER_HP;
            this.name = name;
            innerCircle = COLLISION_RADIUS;
            buffs = new Buffs(this);

            model = new SimpleModel(archBaseTexture, SimpleModel.CLASS_RECT, SimpleModel.CLASS_SCALE);
            boundingRect = SimpleModel.CLASS_BOUNDING_RECT;

            //ability1, 
            a1 = new Cooldown(MELEE_CD);
            //ability 2
            a2 = new Cooldown(BIGHIT_CD);
            stack = new StackingBuff(BIGHIT_STACK_NEEDED);

            weaponPlayer = new WeaponPlayer(archWepAnimation, .3f, .75f);

        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            if (currentHp <= 0) return;
            if (target == null) return;

            if (!isMoving() && !target.isDead()) {
                if (checkCircleRangeAndMove(target, MELEE_RANGE, gameTime)) {
                    
                }else if(a1.use(gameTime)){
                    //game stuff then
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), target, doMeleeHit, archProjectileTexture, archerHitProjectileRect, ARROW_GFX_SCALE, true));
                    weaponPlayer.doAnimation1();
                } else if (a2.use(gameTime) && stack.use()) {
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), target, doBigHit, archerBigProjectileTexture, archerStunProjectileRect));
                    weaponPlayer.doAnimation1();
                }
                
            }
            //update abilitys effected by cdReduc
            a1.setCDReduc(buffs.getValue(BuffType.CDreduc));
            a2.setCDReduc(buffs.getValue(BuffType.CDreduc));
            
        }
        //returns if it needs a target
        internal override bool doAbility() {
            return false;
        }
        internal override Cooldown getFirstCooldown() {
            return a1;
        }
        internal override Cooldown getSecondCooldown() {
            return a2;
        }
        internal override Cooldown getThirdCooldown() {
            return null;
        }
        internal override void setTarget(Moveable m, double gameTime) {
            target = m;
        }
        internal override Texture getFirstAbilityTexture() {
            return archerHitAbilityTexture;
        }
        internal override Texture getSecondAbilityTexture() {
            return archerBigAbilityTexture;
        }
        internal override Texture getThirdAbilityTexture() {
            return null;
        }
        internal bool doMeleeHit(Moveable m) {
            if (m is Enemy)
                ((Enemy)m).takeDamage(this, MELEE_AMT + (float)buffs.getValue(BuffType.meleeDmg), 1);
            else
                m.takeDamage(this, MELEE_AMT);
            stack.stack(1);
            return true;
        }
        internal bool doBigHit(Moveable m) {
            if (m is Enemy) {
                ((Enemy)m).takeDamage(this, BIGHIT_AMT + (float)buffs.getValue(BuffType.meleeDmg)*5, 1);

            } else {//lol what?
                //idc.
            }
            return true;
        }
        internal override int getRange() {
            return MELEE_RANGE;
        }
        
    }
}
