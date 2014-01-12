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
using ProjectTurtle.src.objects;
using ProjectTurtle.src.screens;

namespace ProjectTurtle {
    class Puri : PlayerClassI {
        const int PURI_HP = 8;
        const int AOEHEAL_AMT = 1;
        const int AOEHEAL_CD = 1;
        const int AOEHEAL_RANGE = 175;
        const int BIGHEAL_AMT = 5;
        const int BIGHEAL_CD = 2;
        const int BIGHEAL_RANGE = 300;

        static Texture puriBaseTexture, puriWepAnimation;
        static Texture healWaveTexture;
        static Texture healAbilityTexture, bigHealAbilityTexture;
        static Texture healProjectileTexture, bigHealProjectileTexture;
        static IntRect healProjectileRect, bigHealProjectileRect;

        Cooldown a1, a2;
        Moveable bigHealTarget;

        internal static new void LoadContent() {
            healAbilityTexture = GameBox.loadTexture("images/heal_dot");
            bigHealAbilityTexture = GameBox.loadTexture("images/abilitys/bigheal");

            puriBaseTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "puribase");
            puriWepAnimation = GameBox.loadTexture(SimpleModel.CLASS_PATH + "purianim");
            //projectiles
            healProjectileTexture = GameBox.loadTexture("images/heal_dot"); ;
            healProjectileRect = new IntRect(0, 0, 5, 5);
            bigHealProjectileTexture = GameBox.loadTexture("images/abilitys/bigheal_proj");
            bigHealProjectileRect = new IntRect(0, 0, 8, 8);

            healWaveTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "purifierhealingwave");
        }
        internal Puri(Vector2f start, string name) {
            playerX = destX = (int)start.X;
            playerY = destY = (int)start.Y;
            type = MoveableEnum.puri;
            circleSize = PLAYER_CIRCLE_SZ;
            
            currentHp = maxHp = PURI_HP;
            this.name = name;
            innerCircle = COLLISION_RADIUS;
            boundingRect = SimpleModel.CLASS_BOUNDING_RECT;

            model = new SimpleModel(puriBaseTexture, SimpleModel.CLASS_RECT, SimpleModel.CLASS_SCALE);

            //ability1, aoeheal, repeating
            a1 = new Cooldown(AOEHEAL_CD);
            //ability2 bigheal lowest % target that wont overheal (cant heal mages?)
            a2 = new Cooldown(BIGHEAL_CD);

            weaponPlayer = new WeaponPlayer(puriWepAnimation, new float[] { .3f, .3f }, .75f);

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
                InGame.getInstance().addGroundEffect(new CircleWave(gameTime, getMid(), healWaveTexture, 60, new Color(173 , 255,47)));
                weaponPlayer.doAnimation1();
            }
            //big heal
            if ( !isMoving() && players.Count > 0 && a2.use(gameTime)) {
                
                bigHealTarget = this;//self
                foreach (Moveable mob in players) {
                    if (!mob.isDead() && bigHealTarget.getHPpercent() > mob.getHPpercent() && (Math.Sqrt(Math.Pow(mob.playerX - playerX, 2) + Math.Pow(mob.playerY - playerY, 2)) < BIGHEAL_RANGE))
                        bigHealTarget = mob;
                }
                if(bigHealTarget != null)
                    InGame.getInstance().addProjectile(new Projectile(this, getMid(), bigHealTarget, doBigHeal,bigHealProjectileTexture,bigHealProjectileRect));
                
                weaponPlayer.doAnimation2();
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
        internal override int getRange() {
            return -1;
        }
    }
    class CircleWave : GroundEffect{
        double start, length,ylength,ratio;
        int var;
        Color color;

        internal CircleWave(double gameTime, Vector2f midPos, Texture text, int x)
            : base(midPos, gameTime, 5.0f, text) {
            start = gameTime;
            ratio = 1.0 * text.Size.Y / text.Size.X;
            var = x;
        }
        internal CircleWave(double gameTime, Vector2f midPos, Texture text, int x, Color c)
                : this(gameTime, midPos, text, x) {
            color = c;
        }
        internal override bool update(double gameTime) {
            double elapsed = gameTime - start;
            length = elapsed / 1000 * var;//width
            ylength = length * ratio;

            return base.update(gameTime);
        }
        internal override void draw(RenderWindow window) {
            //remember mPos is the middle this time
            FloatRect r = new FloatRect((int)(mPos.X - length / 2), (int)(mPos.Y - ylength / 2), (int)length, (int)ylength);

            sprite.Position = new Vector2f((mPos.X - (float)length / 2.0f),(mPos.Y - (float)ylength / 2.0f) );
            sprite.Color = color;
            sprite.Scale = new Vector2f((float)length / sprite.TextureRect.Width, (float)ylength / sprite.TextureRect.Height);
            window.Draw(sprite);
        }
        

    }
}
