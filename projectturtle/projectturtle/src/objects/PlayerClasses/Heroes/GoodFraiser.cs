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
using ProjectTurtle.src.screens;

//good fraiser is based off the purifier

namespace ProjectTurtle.src.objects.playerclasses.heroes {
    class GoodFraiser : Hero {
        const int MAX_HP = 9;
        const int AOEHEAL_AMT = 1;
        const float AOEHEAL_CD = .9f;//this is a little faster!
        const int AOEHEAL_RANGE = 200;
        const int BIGHEAL_AMT = 5;//big heals 2 people
        const int BIGHEAL_CD = 2;
        const int BIGHEAL_RANGE = 300;
        const float DISPELL_CD = .5f;
        const int DISPELL_RANGE = 25;

        static Texture healAbilityTexture, bigHealAbilityTexture, puriCircleTexture;
        static Texture healProjectileTexture, bigHealProjectileTexture;
        static Texture dispellEffectTexture;
        static IntRect healProjectileRect, bigHealProjectileRect;
        

        Cooldown a1, a2, a3;
        //Moveable bigHealTarget, bigHealTarget2;

        internal static new void LoadContent() {
            healAbilityTexture = GameBox.loadTexture("images/heal_dot");
            bigHealAbilityTexture = GameBox.loadTexture("images/abilitys/bigheal");
            puriCircleTexture = GameBox.loadTexture("images/avatars/circle_fraiser_small");
            //projectiles
            healProjectileTexture = GameBox.loadTexture("images/heal_dot"); ;
            healProjectileRect = new IntRect(0, 0, 5, 5);
            bigHealProjectileTexture = GameBox.loadTexture("images/abilitys/bigheal_proj");
            bigHealProjectileRect = new IntRect(0, 0, 8, 8);
            //dispell
            dispellEffectTexture = GameBox.loadTexture("images/abilitys/fraiser_dispell");

        }
        internal GoodFraiser(Vector2f start, string name) {
            playerX = destX = (int)start.X;
            playerY = destY = (int)start.Y;
            type = MoveableEnum.player;
            circleSize = 50;
            currentTexture = puriCircleTexture;
            currentHp = maxHp = MAX_HP;
            this.name = name;
            innerCircle = 20;

            //ability1, aoeheal, repeating
            a1 = new Cooldown(AOEHEAL_CD);
            //ability2 bigheal lowest 2 percent target that wont overheal 
            a2 = new Cooldown(BIGHEAL_CD);
            //ability3 small aoe dispell aura, that also silences them TODO
            a3 = new Cooldown(DISPELL_CD);
        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            if (currentHp <= 0) return;

            //aoe heal
            List<Moveable> players = InGame.getInstance().getPlayerMoveables();
            if (!isMoving() && players.Count > 0 && a1.use(gameTime)) {
                foreach (Moveable mob in players) {
                    if (mob == this) {
                        doHeal(this);
                        continue;
                    }
                    if (!mob.isDead() && (Math.Sqrt(Math.Pow(mob.playerX - playerX, 2) + Math.Pow(mob.playerY - playerY, 2)) < AOEHEAL_RANGE))
                        InGame.getInstance().addProjectile(new Projectile(this, getMid(), mob, doHeal, healProjectileTexture, healProjectileRect));
                }
            }
            //big heal
            if (!isMoving() && players.Count > 0 && a2.use(gameTime)) {
                List<PlayerClassI> lowTwo = InGame.getInstance().getLowestHPplayers(2, this, BIGHEAL_RANGE);

                if (lowTwo[0] != null)
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), lowTwo[0], doBigHeal, bigHealProjectileTexture, bigHealProjectileRect));
                if (lowTwo[1] != null)
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), lowTwo[1], doBigHeal, bigHealProjectileTexture, bigHealProjectileRect));
            }
            //dispell bite 
            if (!isMoving() && players.Count > 0 && a3.use(gameTime)) {
                foreach (Moveable mob in players) {
                    if (mob == this) {
                        doDispell(this, gameTime);
                        continue;
                    }
                    if (!mob.isDead() && InGame.areCircleswithRange(mob,this,DISPELL_RANGE)/*(Math.Sqrt(Math.Pow(mob.playerX - playerX, 2) + Math.Pow(mob.playerY - playerY, 2)) < DISPELL_RANGE)*/) {
                        doDispell(mob, gameTime);
                    }
                }
                //TODO make only if something gets dispelled
                InGame.getInstance().addGroundEffect(new ProjectTurtle.src.effects.GroundEffect(
                        new Vector2f(getMid().X - dispellEffectTexture.Size.X / 2, getMid().Y - dispellEffectTexture.Size.Y / 2), gameTime, .1f, dispellEffectTexture));

            }

            //update abilitys effected by cdReduc
            a1.setCDReduc(buffs.getValue(BuffType.CDreduc));
            a2.setCDReduc(buffs.getValue(BuffType.CDreduc));
            a3.setCDReduc(buffs.getValue(BuffType.CDreduc));
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
            //bigHealTarget = m;
        }

        internal override Texture getFirstAbilityTexture() {
            return healAbilityTexture;
        }
        internal override Texture getSecondAbilityTexture() {
            return bigHealAbilityTexture;
        }
        internal override Texture getThirdAbilityTexture() {
            return null;
        }
        internal bool doHeal(Moveable m) {
            m.heal(AOEHEAL_AMT);
            return true;
        }
        internal bool doBigHeal(Moveable m) {
            m.heal(BIGHEAL_AMT);
            return true;
        }
        internal bool doDispell(Moveable m, double gameTime) {
            m.removeBuffType(BuffType.physicalDOT);
            return true;
        }
        internal override int getRange() {
            return -1;
        }
    }
}
