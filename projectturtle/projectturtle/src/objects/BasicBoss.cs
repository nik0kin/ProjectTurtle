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
    class BuseyBoss : BossI{
        enum cooldowns {
            volley, melee, groundFire, spawnAdds
        }
        const int BOSS_HP = 500;
        const int VOLLEY_DMG = 2;
        const int VOLLEY_CD = 4;
        const int VOLLEY_ENRAGE_MULTI = 2;
        const int MELEE_DMG = 3;
        const float MELEE_CD = 1.4f;
        const int GROUNDFIRE_DMG = 5;
        const int GROUNDFIRE_CD = 10;
        const int SPAWNADDS_CD = 20;
		//visual constants
		const int INNERCIRCLE = 38;
        new const int CIRCLE_SZ = 100;

        static Texture meleeProjectileTexture, volleyProjectileTexture;
        static IntRect meleeProjectileRect, volleyProjectileRect;
        static Texture bossCircleTexture;

        internal static new void LoadContent() {
            meleeProjectileTexture = GameBox.loadTexture("images/blue_dot");
            meleeProjectileRect = new IntRect(0, 0, 5, 5);
            volleyProjectileTexture = GameBox.loadTexture("images/atk_boss");
            volleyProjectileRect = new IntRect(0, 0, 20, 10);
            bossCircleTexture = GameBox.loadTexture("images/avatars/bosses/busey");
        }
		//define your cooldowns and base stats
        internal BuseyBoss(Vector2f v) : base(v){
            enemyCooldowns = new Cooldown[4];
            enemyCooldowns[(int)cooldowns.volley] = new Cooldown(VOLLEY_CD);
            enemyCooldowns[(int)cooldowns.melee] = new Cooldown(MELEE_CD);
            enemyCooldowns[(int)cooldowns.groundFire] = new Cooldown(GROUNDFIRE_CD);
            enemyCooldowns[(int)cooldowns.spawnAdds] = new Cooldown(SPAWNADDS_CD);
            currentHp = maxHp = BOSS_HP;
            currentTexture = bossCircleTexture;
            innerCircle = INNERCIRCLE;
            circleSize = CIRCLE_SZ;
        }
		internal override void Update(double gameTime){
            base.Update(gameTime);           
            
            //melee
            if (!isMoving() && enemyCooldowns[(int)cooldowns.melee].use(gameTime)) {
                InGame.getInstance().addProjectile(new Projectile(this, getMid(), aggro, doMelee, meleeProjectileTexture, meleeProjectileRect));
            }
            //try to aoe-volley
            List<Moveable> players = InGame.getInstance().getAlivePlayerMoveables();
            if (!isMoving() && players.Count > 0 && enemyCooldowns[(int)cooldowns.volley].use(gameTime)) {
                foreach(Moveable mob in players){
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), mob, doVolley, volleyProjectileTexture, volleyProjectileRect));
                }
            }
            //put ground aoe on someone
            if (!isMoving() && players.Count > 0 && enemyCooldowns[(int)cooldowns.groundFire].use(gameTime)) {
                int randyPlayer = InGame.randy.Next(players.Count);
                InGame.getInstance().addGroundEffect(new GroundEffect(new Vector2f(players[randyPlayer].playerX, players[randyPlayer].playerY), 1, doFireAoe, gameTime));
            }
            //spawn 2 adds
            if (!isMoving() && enemyCooldowns[(int)cooldowns.spawnAdds].use(gameTime)) {
                InGame.getInstance().addProjectile(new Projectile(this, getMid(), aggro, doSpawnAdds, meleeProjectileTexture, meleeProjectileRect));//TODO make a new visual for the projectile that spawns them
            }
        }
        internal bool doVolley(Moveable mob){
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
        internal bool doFireAoe(Moveable mob) {
            bool dead = mob.takeDamage(this, GROUNDFIRE_DMG);
            if (dead) {
                dropThreat(mob);
            }
            return false;
        }
        internal bool doSpawnAdds(Moveable mob) {
            InGame.getInstance().addMoveable(new BasicAdd(new Vector2f(mob.getMid().X - 10,mob.getMid().Y),mob,this));
            InGame.getInstance().addMoveable(new BasicAdd(new Vector2f(mob.getMid().X + 10,mob.getMid().Y),mob,this));
            changeAddAmt(2);
            return false;
        }
        internal override bool takeDamage(Moveable giver, float amt) {
            return takeDamage(giver, amt, 1);
        }
        internal override string getCooldownString(double gameTime,int decimalPlaces) {
            return "volley" + enemyCooldowns[0].getFormattedTimeLeft(gameTime, decimalPlaces) + " melee "
                        + enemyCooldowns[1].getFormattedTimeLeft(gameTime, decimalPlaces) + " groundfire " + enemyCooldowns[2].getFormattedTimeLeft(gameTime, decimalPlaces)
                        + " adds(" + adds + ") " + enemyCooldowns[3].getFormattedTimeLeft(gameTime, decimalPlaces);
        }
    }
}
