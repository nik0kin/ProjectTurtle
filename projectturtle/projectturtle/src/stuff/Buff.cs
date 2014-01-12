using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.objects;

namespace ProjectTurtle.src.stuff {
    //each player has "Buffs" an object that handles them
    class Buffs {
        List<Buff> buffs;
        Moveable parent;

        internal Buffs(Moveable parent) {
            buffs = new List<Buff>();
            this.parent = parent;
        }
        internal void Update(double gameTime) {
            for (int i = buffs.Count - 1; i >= 0; i--) {
                if (buffs[i].Update(gameTime)) {
                    buffs.Remove(buffs[i]);
                }
            }
        }
        internal void draw(RenderWindow window,Vector2f mid) {
            foreach (Buff b in buffs) {
                b.draw(window, mid);
            }
        }
        internal void addBuff(Buff b) {
            b.parent = this;
            buffs.Add(b);
        }
        internal double getValue(BuffType bType) {
            double d = 0;
            foreach (Buff b in buffs) {
                if (b.getBuffType() == bType) {
                    d += b.getValue();
                }
            }

            return d;
        }
        /// <summary>
        /// removes all buffs of a certain type
        /// </summary>
        /// <param name="buffType">type of buff to remove</param>
        /// <returns>buffs removed</returns>
        internal int removeBuffType(BuffType buffType) {
            int i = 0;

            for (int b = buffs.Count - 1; b >= 0; b--) {
                if (buffs[b].getBuffType() == buffType) {
                    i++;
                    buffs.Remove(buffs[b]);
                }
            }
            return i;
        }
        internal Moveable getParent() { return parent; }

        internal void remove(Buff buff) {
            buffs.Remove(buff);
        }
    }
    class Buff {
        double startTime;
        double totalLength;//seconds
        double value;
        double tickTime;
        int ticks,currentTick;

        Func<Moveable, bool> doAbility;

        BuffType bType;
        Texture biteTexture;//why is this named bite?
        Sprite biteSprite;
        internal Buffs parent;

        internal Buff(BuffType buffType, double value) {
            totalLength = double.MaxValue;//infitite?
            bType = buffType;
            this.value = value;
        }
        internal Buff(double gameTime, BuffType buffType, double totalLength, double value) {
            if (buffType == BuffType.physicalDOT)
                throw new Exception("use correct instanstor");
            startTime = gameTime/1000;
            this.totalLength = totalLength;
            this.value = value;
            bType = buffType;
        }
        internal Buff(double gameTime, BuffType buffType, double totalLength, Func<Moveable, bool> doAbility, double tickTime) {
            startTime = gameTime / 1000;
            this.totalLength = totalLength;
            this.doAbility = doAbility;
            bType = buffType;
            ticks = (int)(totalLength / tickTime);
            currentTick = 0;
            this.tickTime = tickTime;
        }
        //returns true if its expired, then it will be thrown away
        internal bool Update(double gameTime) {
            if(bType != BuffType.physicalDOT)
                return startTime + totalLength < gameTime / 1000;
            //dot code
            if (currentTick < ticks) {
                if (gameTime > startTime + currentTick * tickTime) {
                    currentTick++;
                    doAbility(parent.getParent());
                }
                
            }
            return startTime + totalLength < gameTime / 1000;
        }
        internal BuffType getBuffType() {
            return bType;
        }
        internal double getValue(){
            return value;
        }

        internal void addAvatarTexture(Texture biteTexture) {
            this.biteTexture = biteTexture;
            biteSprite = new Sprite(biteTexture);
        }
        internal void draw(RenderWindow window, Vector2f mid) {
            if (biteTexture != null) {
                biteSprite.Position = mid;
                window.Draw(biteSprite);
            }
        }
    }
    //make sure to update the unit-classes, if it would affect them
    enum BuffType {
        meleeDmg, CDreduc, physicalHaste,

        physicalDOT
    }
}
