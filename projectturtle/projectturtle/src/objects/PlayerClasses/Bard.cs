using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.screens;
using ProjectTurtle.src;

namespace ProjectTurtle.src.objects.playerclasses {
    class Bard : PlayerClassI {
        const int BARD_HP = 7;
        const float BUFFDMG_AMT = .6f;//assa archer vang
        const float BUFF_CD_PERCENT = .15f;
        const int BUFF_PLAYER_AMT = 3;
        const int BUFF_RANGE = 125;
        const float BUFF_TIME = .1f;

        static Texture bardBaseTexture, bardWepAnimation;
        static Texture buffDmgAbilityTexture, buffCDAbilityTexture;
        static Texture buffDmgBeamTexture, buffCDBeamTexture;
        static FloatRect buffDmgBeamRect, buffCDBeamRect;

        Sprite buffDmgBeamSprite, buffCDBeamSprite;
        List<Moveable> beamTargets;
        Cooldown a1;

        bool dmgBeam = true;
        internal static new void LoadContent() {
            buffDmgAbilityTexture = GameBox.loadTexture("images/abilitys/buffDmgIcon");
            buffCDAbilityTexture = GameBox.loadTexture("images/abilitys/buffCDIcon");

            //beams
            buffDmgBeamTexture = GameBox.loadTexture("images/abilitys/buffDmg_beam"); ;
            buffDmgBeamRect = new FloatRect(0, 0, 4, 4);
            buffCDBeamTexture = GameBox.loadTexture("images/abilitys/buffCD_beam");
            buffCDBeamRect = new FloatRect(0, 0, 4, 4);

            bardBaseTexture = GameBox.loadTexture(SimpleModel.CLASS_PATH + "bardbase");
            bardWepAnimation = GameBox.loadTexture(SimpleModel.CLASS_PATH + "bardanim"); 
        }
        internal Bard(Vector2f start, string name) {
            playerX = destX = (int)start.X;
            playerY = destY = (int)start.Y;
            circleSize = PLAYER_CIRCLE_SZ;

            currentHp = maxHp = BARD_HP;
            this.name = name;
            innerCircle = COLLISION_RADIUS;
            buffs = new Buffs(this);

            model = new SimpleModel(bardBaseTexture, SimpleModel.CLASS_RECT, SimpleModel.CLASS_SCALE);
            boundingRect = SimpleModel.CLASS_BOUNDING_RECT;

            a1 = new Cooldown(BUFF_TIME);

            weaponPlayer = new WeaponPlayer(bardWepAnimation, .1f, .75f);

            buffDmgBeamSprite = new Sprite(buffDmgBeamTexture);
            buffCDBeamSprite = new Sprite(buffCDBeamTexture);
        }
        internal override void Update(double gameTime) {
            base.Update(gameTime);
            if (currentHp <= 0) return;

            //set beam targets
            if (a1.use(gameTime)) {
                //get closest 3 players   
                beamTargets = InGame.getInstance().getClosestPlayers(getMid(), BUFF_PLAYER_AMT, true, BUFF_RANGE,this.GetType());
                foreach (Moveable m in beamTargets) {
                    if (m == this || m == null) continue;
                    if (dmgBeam) {
                        doBuffDmg(gameTime, m);
                    } else {
                        doBuffCD(gameTime, m);
                    }
                }
            }
            
        }
        internal override void Draw(double gameTime, RenderWindow window, bool selected){
            //draw beams
            if (beamTargets != null && beamTargets.Count > 1)
            foreach (Moveable m in beamTargets) {
                if (m == this || m == null) continue;
                Beam.drawBeam(window, getMid(), m.getMid(), dmgBeam ? buffDmgBeamSprite : buffCDBeamSprite);

            }

            base.Draw(gameTime, window, selected);
        }
        //returns if it has an active ability?
        internal override bool doAbility() {
            dmgBeam = !dmgBeam;
            weaponPlayer.doAnimation1();

            return true;
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
            //bigHealTarget = m;
        }

        internal override Texture getFirstAbilityTexture() {
            return dmgBeam ? buffDmgAbilityTexture : buffCDAbilityTexture;
        }
        internal override Texture getSecondAbilityTexture() {
            return null;
        }
        internal override Texture getThirdAbilityTexture() {
            return null;
        }
        internal bool doBuffDmg(double gameTime,Moveable m) {
            m.addBuff(new Buff(gameTime, BuffType.meleeDmg, BUFF_TIME, BUFFDMG_AMT));
            return true;
        }
        internal bool doBuffCD(double gameTime, Moveable m) {
            m.addBuff(new Buff(gameTime, BuffType.CDreduc, BUFF_TIME, BUFF_CD_PERCENT));
            return true;
        }
        internal override int getRange() {
            return -1;
        }
    }
}
