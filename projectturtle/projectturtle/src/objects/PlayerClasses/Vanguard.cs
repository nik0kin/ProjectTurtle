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
    class Vanguard : PlayerClassI {
        const int VANGUARD_HP = 20;
        const int THREAT_MULTIPLIER = 4;
        const float TANKHIT_AMT = 2.5f;
        const int TANKHIT_CD = 5;
        const int TANKHIT_RANGE = 25;
        const float TANKAOE_AMT = .5f;
        const float TANKAOE_CD = 1.0f;
        const int TANKAOE_RANGE = 40;

        static Texture tankHitAbilityTexture, tankAoeAbilityTexture;//, vangCircleTexture;
        static Texture slamWaveTexture;
        static Texture tankHitProjectileTexture, tankAoeProjectileTexture;
        static IntRect tankHitProjectileRect, tankAoeProjectileRect;
        static Texture vangBaseTexture, vangWepAnimations;

        Cooldown a1, a2;
        
        internal static new void LoadContent() {
            tankHitAbilityTexture = GameBox.loadTexture("images/abilitys/tankhit");
            tankAoeAbilityTexture = GameBox.loadTexture("images/abilitys/tankaoe");
            //vangCircleTexture = GameBox.loadTexture("images/avatars/baseclasses/vanguard");
            //projectile texture/rects
            tankHitProjectileTexture = GameBox.loadTexture("images/abilitys/tankhit_proj");
            tankHitProjectileRect = new IntRect(0, 0, 5, 5);
            tankAoeProjectileTexture = GameBox.loadTexture("images/abilitys/tankaoe_proj");
            tankAoeProjectileRect = new IntRect(0, 0, 5, 5);

            vangBaseTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "vangbase");
            vangWepAnimations = GameBox.loadTexture(SimpleModel.CLASS_PATH + "vanganim");

            slamWaveTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "vangaoewave");
        }
        internal Vanguard(Vector2f start, string name) {
            playerX = destX = (int)start.X;
            playerY = destY = (int)start.Y;
            type = MoveableEnum.puri;
            circleSize = PLAYER_CIRCLE_SZ;
            //currentTexture = vangCircleTexture;
            currentHp = maxHp = VANGUARD_HP;
            this.name = name;
            innerCircle = COLLISION_RADIUS;
            buffs = new Buffs(this);

            model = new SimpleModel(vangBaseTexture, SimpleModel.CLASS_RECT,.75f);
            boundingRect = SimpleModel.CLASS_BOUNDING_RECT;

            //ability1, 
            a1 = new Cooldown(TANKHIT_CD,true);
            //ability 2
            a2 = new Cooldown(TANKAOE_CD);


            weaponPlayer = new WeaponPlayer(vangWepAnimations,.2f, .75f);
        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            if (currentHp <= 0) return;

            if (!isMoving()  && target != null && !target.isDead()) {
                if (checkCircleRangeAndMove(target, TANKHIT_RANGE, gameTime) ) {

                }else if(a1.use(gameTime)){
                    //game stuff then
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), target, doTankHit, tankHitProjectileTexture, tankHitProjectileRect));
                    weaponPlayer.doAnimation1();
                }
                
            }
            List<Enemy> enemys = InGame.getInstance().getAliveEnemyMoveables();
            if (InGame.getInstance().getFightStatus() == FightStatus.fightEngaged  && enemys.Count > 0 && a2.use(gameTime)) {
                foreach (Moveable mob in enemys) {
                    if (!mob.isDead() && InGame.areCirclesIntersecting(getMid(), TANKAOE_RANGE, mob.getMid(),mob.getInnerRadius()))
                        InGame.getInstance().addProjectile(new Projectile(this, getMid(), mob, doTankAoe, tankAoeProjectileTexture, tankAoeProjectileRect));
                }
                InGame.getInstance().addGroundEffect(new CircleWave(gameTime, getMid(), slamWaveTexture, 20, new Color(255, 140, 0)));
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
            return tankHitAbilityTexture;
        }
        internal override Texture getSecondAbilityTexture() {
            return tankAoeAbilityTexture;
        }
        internal override Texture getThirdAbilityTexture() {
            return null;
        }
        internal bool doTankHit(Moveable m) {
            if (m is Enemy)
                ((Enemy)m).takeDamage(this, TANKHIT_AMT + (float)buffs.getValue(BuffType.meleeDmg), THREAT_MULTIPLIER);
            else
                m.takeDamage(this, TANKHIT_AMT + (float)buffs.getValue(BuffType.meleeDmg));
            return true;
        }
        internal bool doTankAoe(Moveable m) {
            if (m is Enemy)
                ((Enemy)m).takeDamage(this, TANKAOE_AMT, THREAT_MULTIPLIER);
            else//lol what?
                m.takeDamage(this, TANKAOE_AMT);
            return true;
        }
        internal override int getRange() {
            return TANKHIT_RANGE;
        }
        
    }
}
