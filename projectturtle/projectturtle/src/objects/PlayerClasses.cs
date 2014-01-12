using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.objects.playerclasses.heroes;
using ProjectTurtle.src.objects;
using ProjectTurtle.src.objects.playerclasses;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.effects;

namespace ProjectTurtle.src {
    abstract class PlayerClassI : Moveable{
        internal static int COLLISION_RADIUS = 12, PLAYER_CIRCLE_SZ = 30;

        abstract internal bool doAbility();
        /*abstract internal bool doSecondAbility(bool alt);
        abstract internal void doThirdAbility();*/
        abstract internal Cooldown getFirstCooldown();
        abstract internal Cooldown getSecondCooldown();
        abstract internal Cooldown getThirdCooldown();
        abstract internal void setTarget(Moveable m, double gameTime);
        abstract internal Texture getFirstAbilityTexture();
        abstract internal Texture getSecondAbilityTexture();
        abstract internal Texture getThirdAbilityTexture();
        //abstract internal bool getFirstRepeater();
        //abstract internal bool getSecondRepeater();
        abstract internal override int getRange();

        internal static int getPlayerAmt(PlayerClassNum[] raidComp) {
            int i = 0;
            foreach (PlayerClassNum p in raidComp) {
                if (p != PlayerClassNum.none) i++;
            }
            return i;
        }
        internal static List<PlayerClassI> makeRaid(PlayerClassNum[] raidComp, Vector2f start) {
            List<PlayerClassI> l = new List<PlayerClassI>();
            int i = 0, j = 0;
            int vnum = 1, anum = 1, pnum = 1, mnum = 1, arnum = 1, bnum = 1;
            foreach (PlayerClassNum p in raidComp) {
                Vector2f spot = new Vector2f(start.X + 40 * i, start.Y + 40 * j);
                switch (p) {
                case PlayerClassNum.vang:
                    l.Add(new Vanguard(spot, "Vanguard " + vnum));
                    vnum++;
                    break;
                case PlayerClassNum.assa:
                    l.Add(new Assassin(spot, "Assassin " + anum));
                    anum++;
                    break;
                case PlayerClassNum.mage:
                    l.Add(new Mage(spot, "Flame Mage " + mnum));
                    mnum++;
                    break;
                case PlayerClassNum.puri:
                    l.Add(new Puri(spot, "Purifier " + pnum));
                    pnum++;
                    break;
                case PlayerClassNum.archer:
                    l.Add(new Archer(spot, "Archer " + arnum));
                    arnum++;
                    break;
                case PlayerClassNum.bard:
                    l.Add(new Bard(spot, "Bard " + bnum));
                    bnum++;
                    break;
                //heroes
                case PlayerClassNum.goodfraiser:
                    l.Add(new GoodFraiser(spot, "Good Fraiser"));
                    break;
                }
                i++;
                if (i % 7 == 0) { j++; i = 0; }
            }
            return l;
        }
        //TODO this is where we should convert this into sending ChampItemPair and turn it into a PlayerClassI
        //TODO change the return to whatever we are using for the raidframes, also remake List<List<ProjectTurtle.src.stuff.ChampItemPair>> into a class
        internal static List<PlayerClassI> makeRaid(List<List<ChampItemPair>> raidComp, Vector2f start) {
            List<PlayerClassI> l = new List<PlayerClassI>();
            int i = 0, j = 0;
            int vnum = 1, anum = 1, pnum = 1, mnum = 1, arnum = 1, bnum = 1;
            foreach(List<ChampItemPair> group in raidComp){
                foreach (ChampItemPair unit in group) {
                    Vector2f spot = new Vector2f(start.X + 40 * i, start.Y + 40 * j);
                    switch (unit.hero) {
                        case PlayerClassNum.vang:
                            l.Add(new Vanguard(spot, "Vanguard " + vnum));
                            vnum++;
                            break;
                        case PlayerClassNum.assa:
                            l.Add(new Assassin(spot, "Assassin " + anum));
                            anum++;
                            break;
                        case PlayerClassNum.mage:
                            l.Add(new Mage(spot, "Flame Mage " + mnum));
                            mnum++;
                            break;
                        case PlayerClassNum.puri:
                            l.Add(new Puri(spot, "Purifier " + pnum));
                            pnum++;
                            break;
                        case PlayerClassNum.archer:
                            l.Add(new Archer(spot, "Archer " + arnum));
                            arnum++;
                            break;
                        case PlayerClassNum.bard:
                            l.Add(new Bard(spot, "Bard " + bnum));
                            bnum++;
                            break;
                        //heroes
                        case PlayerClassNum.goodfraiser:
                            l.Add(new GoodFraiser(spot, "Good Fraiser"));
                            break;
                    }
                    i++;
                    if (i % 7 == 0) { j++; i = 0; }
                }
            }
            return l;
        }
        internal static PlayerClassNum getClassNum(PlayerClassI p){
            if (p is Mage) return PlayerClassNum.mage;
            if (p is Puri) return PlayerClassNum.puri;
            if (p is Bard) return PlayerClassNum.bard;
            if (p is Assassin) return PlayerClassNum.assa;
            if (p is Vanguard) return PlayerClassNum.vang;
            if (p is Archer) return PlayerClassNum.archer;
            if (p is GoodFraiser) return PlayerClassNum.goodfraiser;

            return PlayerClassNum.none;
        }
        internal static String getClassName(PlayerClassNum p){
            switch (p)
            {
                case PlayerClassNum.vang:
                    return "Vanguard";
                case PlayerClassNum.assa:
                    return "Assassin";
                case PlayerClassNum.mage:
                    return "Mage";
                case PlayerClassNum.puri:
                    return "Purifier";
                case PlayerClassNum.archer:
                    return "Archer";
                case PlayerClassNum.bard:
                    return "Bard";
                //heros
                case PlayerClassNum.goodfraiser:
                    return "Good Fraizer";
            }

            return("noname");
        }
        internal static Color getClassColor(PlayerClassNum p) {
            switch (p) {
            case PlayerClassNum.vang:
                return new Color(255,165,0);
            case PlayerClassNum.assa:
                return new Color(186, 85, 211);
            case PlayerClassNum.mage:
                return Color.Red;
            case PlayerClassNum.puri:
                return Color.Green;
            case PlayerClassNum.archer:
                return Color.Yellow;
            case PlayerClassNum.bard:
                return Color.Blue;
            case PlayerClassNum.none:
                return new Color(190, 190, 190);
            //heros
            case PlayerClassNum.goodfraiser:
                return new Color(255,215,0);
            }

            return Color.Black;
        }
        //pretty much the unlock list atm
        internal static List<PlayerClassNum> getHeroList() {
            List<PlayerClassNum> l = new List<PlayerClassNum>();
            l.Add(PlayerClassNum.goodfraiser);

            return l;
        }

        
    }
    internal enum PlayerClassNum {
        none, cant, 
        //reg types
        mage, puri, vang, assa, archer, bard,
        //heroes
        goodfraiser

    }
    internal abstract class Hero : PlayerClassI {
        //dont require nothin atm!
    }

}
