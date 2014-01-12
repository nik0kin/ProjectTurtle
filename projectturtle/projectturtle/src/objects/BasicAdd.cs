using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.util;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.objects {
    class BasicAdd : Enemy {
        const int ADD_HP = 50;
        const float MELEE_DMG = .75f;
        const float MELEE_CD = 2f;
        const int MELEE_RANGE = 25;
        const int SPAWNTHREAT = 5;

        static Texture basicAddTexture;
        static Texture meleeProjectileTexture;
        static IntRect meleeProjectileRect;

        BossI parent;

        float meleeDmg = MELEE_DMG;

        internal new static void LoadContent(){
            basicAddTexture = GameBox.loadTexture("images/avatars/circle_add");
            meleeProjectileTexture = GameBox.loadTexture("images/blue_dot");
            meleeProjectileRect = new IntRect(0, 0, 5, 5);
        }

        internal BasicAdd(Vector2f v, Moveable spawnOn, BossI parent)
            : base(v, MoveableEnum.add) {
            enemyCooldowns = new Cooldown[1];
            enemyCooldowns[0] = new Cooldown(MELEE_CD);
            currentHp = maxHp = ADD_HP;
            circleSize = 25;
            innerCircle = 10;
            updateThreat(spawnOn, SPAWNTHREAT);
            reOrgThreat();
            currentTexture = basicAddTexture;
            this.parent = parent;
        }
        internal BasicAdd(Vector2f v, Moveable spawnOn, int hp, int meleeDmg,BossI parent) : this(v,spawnOn,parent){
            currentHp = maxHp = hp;
            this.meleeDmg = meleeDmg;
        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            aggro = getFirstThreat();
            if (currentHp <= 0) {
                wipeAllThreat();
                return;
            }
            if (aggro == null) return;
            //melee
            if (!isMoving() ) {
                if (checkRangeAndMove(aggro.getMid(), MELEE_RANGE, gameTime)) {

                } else if (enemyCooldowns[0].use(gameTime))
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), aggro, doMelee, meleeProjectileTexture, meleeProjectileRect));
            }

        }
        internal Cooldown getFirstCooldown() {
            return enemyCooldowns[0];
        }

        internal bool doMelee(Moveable mob) {
            bool dead = mob.takeDamage(this, MELEE_DMG);
            if (dead) {
                dropThreat(mob);
            }
            return false;
        }

        internal override bool takeDamage(Moveable giver, float amt, int threatMultiplier) {
            if (aggro == null) {
                aggro = giver;
            }
            if (isDead()) return false;
            bool b = base.takeDamage(giver, amt, threatMultiplier);
            if (b) parent.changeAddAmt(-1);
            return b;
        }
    }
}
