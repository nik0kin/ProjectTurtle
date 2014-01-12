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
using ProjectTurtle.src.bossfights;
using ProjectTurtle.src.stuff;

namespace ProjectTurtle.src.bossfights {
	internal struct BossFightInfo{
		internal BossFightRecord records;
		//internal String deathString;//TODO make whole dialogue? class?
		internal List<PlayerClassNum> addedLts;
		internal string /*bgString,*/desc;//
		internal int maxplayers;
		internal string name;
		internal int enragetimer;
        internal Backdrop backdrop;
		
		internal BossFightInfo(string name, int maxplayers, int enragetimer, string desc,
                List<PlayerClassNum> addedLts, BossFightRecord records,Backdrop backdrop){
            //maxplayers = -1;
            //enragetimer = -1;
            this.name = name;
            this.maxplayers = maxplayers;
            this.enragetimer = enragetimer;
            this.desc = desc;
            this.records = records;
            this.addedLts = addedLts;
            this.backdrop = backdrop;
        }
	}
    abstract class BossFight {
        
        protected List<Enemy> enemys;
        protected List<FightObject> fightObjects;

        internal List<Enemy> getEnemies() {
            return enemys;
        }
        internal List<FightObject> getObjects() {
            return fightObjects;
        }
        //abstract internal string getBackground();
        //abstract internal int getMaxPlayers();
        internal bool isBossFightDead() {
            bool b = true;
            foreach(Moveable e in enemys){
                if (!e.isDead()) {
                    b = false;
                    break;
                }
            }
            return b;
        }
        abstract internal BossFightID getFightID();
        /*abstract internal string getBGString();

        abstract internal int getEnrageTimer();
        abstract internal string getDeathString();*/

        internal void setEnraged(bool v) {
            foreach (Moveable enemy in enemys) {
                if (enemy is BossI) {
                    ((BossI)enemy).setEnraged(v);
                }
            }
        }

        abstract internal List<Script> getTimedScripts();
        /// <summary>
        /// starts the next boss fight?
        /// </summary>
        /// <param name="phase">phase number</param>
        /// <returns>false on error</returns>
        abstract internal bool initPhase(int phase);

        //abstract internal PlayerClassNum getAddedPlayers();
        abstract internal BossFightInfo getBossInfo();
    }
    internal class BossFightRecord {
        internal int[] timeForStars;//{gold,silver,bronze}
        internal int[] smallAmtForStars;

        internal BossFightRecord() {}
        internal BossFightRecord(int[] time, int[] small) {
            timeForStars = time;
            smallAmtForStars = small; 
        }
    }
    internal class FightObject {
        Vector2f pos;//mid bottom of the image
        Texture text;
        Sprite sprite;
        IntRect sourceRect;
        protected AnimationPlayer animationPlayer;

        internal FightObject(Texture text, Vector2f pos){
            this.text = text;
            this.pos = pos;
            sourceRect = new IntRect(0, 0, (int)text.Size.X, (int)text.Size.Y);
            sprite = new Sprite(text);
            sprite.TextureRect = sourceRect;
        }
        internal FightObject(Texture text, Vector2f pos, IntRect sourceRect)
            : this(text, pos) {
            this.sourceRect = sourceRect;
            sprite.TextureRect = sourceRect;
        }
        internal FightObject(Texture text, Vector2f pos, IntRect sourceRect, bool animationplayer)
            : this(text, pos,sourceRect) {
            if (animationplayer)
                animationPlayer = new AnimationPlayer();
        }

        internal virtual void update(double gameTime){
            
        }
        internal void draw(RenderWindow window, double gameTime) {
            if (!animationPlayer.Draw(gameTime, window, pos, 1)) {
                FloatRect ir = GeoLib.midBottomPosToTopRightRect(pos, sourceRect);
                sprite.Position = new Vector2f(ir.Left, ir.Top);
                
                window.Draw(sprite);
            }
        }
        internal Vector2f getMid() {
            return pos;
        }
    }
}