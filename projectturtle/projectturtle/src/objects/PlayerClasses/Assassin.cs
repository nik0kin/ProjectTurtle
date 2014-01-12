using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.objects {
    class Assassin : PlayerClassI {
        const int ASSASSIN_HP = 9;
        const float MELEE_AMT = .70f;
        const float MELEE_CD = .5f;
        const int MELEE_RANGE = 25;
        const float STUN_AMT = 5;
        const float STUN_PUSHBACK = .05f;//now percent
        const float STUN_CD = 10.0f;

        static Texture assaBaseTexture, assaWepAnimation;
        static Texture assHitAbilityTexture, assAoeAbilityTexture;
        static Texture assHitProjectileTexture, assStunProjectileTexture;
        static IntRect assHitProjectileRect, assStunProjectileRect;

        Cooldown a1, a2;

        internal static new void LoadContent() {
            assHitAbilityTexture = GameBox.loadTexture("images/abilitys/asshit");
            assAoeAbilityTexture = GameBox.loadTexture("images/abilitys/assstun");

            assaBaseTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "assabase");
            assaWepAnimation = GameBox.loadTexture(SimpleModel.CLASS_PATH + "assaanim");
            //projectile texture/rects
            assHitProjectileTexture = GameBox.loadTexture("images/abilitys/asshit_proj");
            assHitProjectileRect = new IntRect(0, 0, 5, 5);
            assStunProjectileTexture = GameBox.loadTexture("images/abilitys/assstun_proj");
            assStunProjectileRect = new IntRect(0, 0, 10, 10);
        }
        internal Assassin(Vector2f start, string name) {
            playerX = destX = (int)start.X;
            playerY = destY = (int)start.Y;
            circleSize = PLAYER_CIRCLE_SZ;
            
            currentHp = maxHp = ASSASSIN_HP;
            this.name = name;
            innerCircle = COLLISION_RADIUS;

            model = new SimpleModel(assaBaseTexture, SimpleModel.CLASS_RECT, SimpleModel.CLASS_SCALE);
            boundingRect = SimpleModel.CLASS_BOUNDING_RECT;

            //ability1, 
            a1 = new Cooldown(MELEE_CD);
            //ability 2
            a2 = new Cooldown(STUN_CD);

            weaponPlayer = new WeaponPlayer(assaWepAnimation, new float[] {.15f,.3f}, .75f);
        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            if (currentHp <= 0) return;
            if (target == null) return;

            if (!isMoving() && !target.isDead()) {
                if (checkCircleRangeAndMove(target, MELEE_RANGE, gameTime)) {
                    
                }else if(a1.use(gameTime)){
                    //game stuff then
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), target, doMeleeHit, assHitProjectileTexture, assHitProjectileRect));
                    weaponPlayer.doAnimation1();
                } else if (!target.isDead() && a2.use(gameTime)) {
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), target, doAssStun, assStunProjectileTexture, assStunProjectileRect));
                    weaponPlayer.doAnimation2();
                }
                
            }
            
            //update abilitys effected by cdReduc
            a1.setCDReduc(buffs.getValue(BuffType.CDreduc));
            a2.setCDReduc(buffs.getValue(BuffType.CDreduc));
        }
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
            return assHitAbilityTexture;
        }
        internal override Texture getSecondAbilityTexture() {
            return assAoeAbilityTexture;
        }
        internal override Texture getThirdAbilityTexture() {
            return null;
        }
        internal bool doMeleeHit(Moveable m) {
            if (m is Enemy)
                ((Enemy)m).takeDamage(this, MELEE_AMT + (float)buffs.getValue(BuffType.meleeDmg), 1);
            else
                m.takeDamage(this, MELEE_AMT + (float)buffs.getValue(BuffType.meleeDmg));
            return true;
        }
        internal bool doAssStun(Moveable m) {
            if (m is Enemy) {
                ((Enemy)m).takeDamage(this, STUN_AMT + (float)buffs.getValue(BuffType.meleeDmg), 1);
                ((Enemy)m).pushbackCooldownPercent(GameBox.getInstance().getGameTime(), STUN_PUSHBACK);
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
