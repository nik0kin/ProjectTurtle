using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;

namespace ProjectTurtle.src.objects {
    abstract class BossI : Enemy {
        internal BossI(Vector2f v) : base(v, MoveableEnum.boss) { }

        //abstract internal Cooldown getFirstCooldown();
        //abstract internal Cooldown getSecondCooldown();
        //abstract internal Cooldown getThirdCooldown();
        //abstract internal Cooldown getFourthCooldown();

        abstract internal string getCooldownString(double gameTime, int decimalPlaces);
        //abstract internal int getAddCount();

        internal bool enraged;
        internal void setEnraged(bool v){
            enraged = true;
        }
        protected int adds;
        internal void changeAddAmt(int p) {
            adds += p;
        }
		internal override bool takeDamage(Moveable giver, float amt) {
            return takeDamage(giver, amt, 1);
        }
    }
}
