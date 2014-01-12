using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using System.Collections;

using ProjectTurtle.src.objects;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.util;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.objects {
    class Enemy : Moveable{
        Moveable[] threat;
        protected Moveable aggro;
        Hashtable threatTable;//key=player, value=threat
        protected Cooldown[] enemyCooldowns;
        public Enemy(Vector2f v, MoveableEnum mobEnum) : base(v,mobEnum){
            threat = new Moveable[3];
            threatTable = new Hashtable();
        }
		internal override void Update (double gameTime){
			base.Update (gameTime);
			
			if (currentHp <= 0) {
                wipeAllThreat();
                return;
            }
			aggro = target = getFirstThreat();
			
			if (aggro == null) {//if no one has aggro put cooldowns on CD (start of fight)
				foreach(Cooldown cd in enemyCooldowns)
                	cd.use(gameTime);
                //return; do we want to return here?
            }
		}
        internal void dropThreat(Moveable m) {
            threatTable.Remove(m);
            reOrgThreat();
        }
        internal void updateThreat(Moveable attacker, float threatAmt){
            if (threatTable.ContainsKey(attacker)) {
                threatAmt = threatAmt + (float)threatTable[attacker];
            }
            threatTable.Remove(attacker);
            threatTable.Add(attacker, threatAmt);
        }
        internal void updateThreat(List<Moveable> mobs, float threatAmt) {
            float maynard = 0;
            foreach (Moveable m in mobs) {
                if (threatTable.ContainsKey(m)) {
                    maynard = threatAmt + (float)threatTable[m];
                } else
                    maynard = threatAmt;
                threatTable.Remove(m);
                threatTable.Add(m, maynard);
            }
            
        }
        //TODO EFF
        internal void reOrgThreat() {
            ICollection keys = threatTable.Keys;
            Moveable[] array = new Moveable[threatTable.Count];
            
            if (threat[0] != null && threat[0].isDead()) { threat[0] = null; }
            if (threat[1] != null && threat[1].isDead()) { threat[1] = null; }
            if (threat[2] != null && threat[2].isDead()) { threat[2] = null; }
            threat = new Moveable[3];

            for (int i = 0; i < threatTable.Count; i++) {
                if (array[i] != null && array[i].isDead()) {
                    continue;
                }
                keys.CopyTo(array, 0);
                float amount = (float)threatTable[array[i]];

                if (threat[0] == null || amount > (float)threatTable[threat[0]]) {//if higher than the higher
                    //then put at top and shift others down
                    threat[2] = threat[1];
                    threat[1] = threat[0];
                    threat[0] = array[i];
                } else if (threat[1] == null || amount > (float)threatTable[threat[1]]) {
                    threat[2] = threat[1];
                    threat[1] = array[i];
                } else if (threat[2] == null || amount > (float)threatTable[threat[2]]) {
                    threat[2] = array[i];
                }
            }

        }
        internal Moveable getFirstThreat() {
            return threat[0];
        }
        internal Moveable getSecondThreat() {
            if (threat[1] == null) 
                return getFirstThreat();
            return threat[1];
        }
        internal Moveable getThirdThreat() {
            if (threat[2] == null) return getSecondThreat();
            return threat[2];
        }
        internal float getThreatAmt(Moveable m) {
            if (threatTable.ContainsKey(m))
                return (float)threatTable[m];
            return 0;
        }
        internal void wipeAllThreat() {
            threatTable.Clear();
            reOrgThreat();
        }
        //true if killing blow?
        internal virtual bool takeDamage(Moveable giver, float amt, int threatMulti) {
            bool killed = base.takeDamage(giver, amt);
            if (aggro == null) {
                aggro = giver;
                updateThreat(InGame.getInstance().getAlivePlayerMoveables(), 1);//put raid in combat
                InGame.getInstance().startBossFight();
            }
            updateThreat(giver, amt * threatMulti);
            reOrgThreat();
            return killed;
        }
        internal void pushbackCooldowns(double gameTime, float amt){
            foreach (Cooldown c in enemyCooldowns) {
                c.pushback(gameTime, amt);
            }
        }
        internal void pushbackCooldownPercent(double gameTime, float amtPercent) {
            foreach (Cooldown c in enemyCooldowns) {
                c.pushback(gameTime, (float)(c.getCooldown() * amtPercent));
            }
        }
        //only returns if you are top 3 right now
        internal int getThreatRank(Moveable player) {
            if (getFirstThreat() == player) return 1;
            if (getSecondThreat() == player) return 2;
            if (getThirdThreat() == player) return 3;
                
            return -1;
        }

        internal void resetCooldowns(double gameTime) {
            foreach (Cooldown c in enemyCooldowns) {
                if(c != null) c.reset(gameTime);
            }
        }

    }
}
