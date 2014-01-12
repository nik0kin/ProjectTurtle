using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.stuff;
using ProjectTurtle.src.util;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.ui{
	internal class PillarGroup{
		const int GROUPS = 5;
		const int PILLAR_WIDTH = 160;
		const int PILLAR_HEIGHT = 40;
		
        FloatRect rect;
		
		Pillar[] pillars;

		internal PillarGroup(FloatRect rect){
            this.rect = rect;
			pillars = new Pillar[GROUPS];
			pillars[0] = new Pillar(new FloatRect(260,260,PILLAR_WIDTH,PILLAR_HEIGHT));
			pillars[1] = new Pillar(new FloatRect(580,260,PILLAR_WIDTH,PILLAR_HEIGHT));
			pillars[2] = new Pillar(new FloatRect(420,340,PILLAR_WIDTH,PILLAR_HEIGHT));
			pillars[3] = new Pillar(new FloatRect(260,420,PILLAR_WIDTH,PILLAR_HEIGHT));
			pillars[4] = new Pillar(new FloatRect(580,420,PILLAR_WIDTH,PILLAR_HEIGHT));
		}
		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mouseState"></param>
        /// <param name="addedChamp"></param>
        /// <returns> if you could drop it there</returns>
		internal bool hoverAndDrop (Vector2f release, SimpleMouseState mouseState, ChampItemPair addedChamp){
			//throw new System.NotImplementedException ();
            if (!GeoLib.isInRect(rect,release)) 
                return false;
            for(int i=0;i<GROUPS;i++){
				if(pillars[i].update(release)){
					pillars[i].addUnit(addedChamp);
                    Console.WriteLine(i + " pillar");
					return true;//only one of these can be true;
				}
			}
			
			return false;
		}
        internal void draw(RenderWindow window) {
            foreach (Pillar p in pillars)
                p.draw(window);
        }
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="List<List<ChampItemPair>>"/>, 5 raid groups of a variable amount each
		/// </returns>
		internal List<List<ChampItemPair>> getRaidGroups(){
			List<List<ChampItemPair>> groups = new List<List<ChampItemPair>>();
			foreach(Pillar p in pillars)
				groups.Add(p.getGroup());
			return groups;
		}
		internal void reset(){
			foreach(Pillar p in pillars)
				p.reset();
		}
	}
	internal class Pillar{
        static Texture pillarUnused,pillarUsed;
        static Texture assa, arch, puri, vang, pyro, bard;
        FloatRect rect;
		List<ChampItemPair> units;//ingroup
        Sprite pillarSprite, unitSprite;

        internal static void LoadContent(){
            pillarUnused = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "partyempty");
            pillarUsed = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "partyfilled");

            assa = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackassa");
            arch = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackarch");//TODO remove
            puri = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackpuri");
            vang = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackvang");
            pyro = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackpyro");
            bard = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackbard");
        }
		internal Pillar (FloatRect postion){
			rect = postion;
			units = new List<ChampItemPair>();

            pillarSprite = new Sprite();
            unitSprite = new Sprite();
		}
		internal bool update(Vector2f click){
			return GeoLib.isInRect(rect,click);	
		}
        internal void draw(RenderWindow window) {
            pillarSprite.Position = new Vector2f(rect.Left,rect.Top);
            pillarSprite.Texture = units.Count > 0 ? pillarUsed : pillarUnused;
            window.Draw(pillarSprite);

            int i=0;
            foreach (ChampItemPair unit in units) {
                //TODO EFF this must not efficient
                unitSprite.Position = new Vector2f(rect.Left + (i++ * 50), rect.Top);
                unitSprite.Texture = getUnitTexture(unit.hero);
                window.Draw(unitSprite);
            }
        }
		internal void reset(){
			units = new List<ChampItemPair>();	
		}
		internal List<ChampItemPair> getGroup(){
			return units;
		}

        internal void addUnit(ChampItemPair addedChamp) {
            units.Add(addedChamp);
            
        }
        private static Texture getUnitTexture(PlayerClassNum id){
            switch(id){
                case PlayerClassNum.archer:
                    return arch;
                case PlayerClassNum.assa:
                    return assa;
                case PlayerClassNum.bard:
                    return bard;
                case PlayerClassNum.mage:
                    return pyro;
                case PlayerClassNum.puri:
                    return puri;
                case PlayerClassNum.vang:
                    return vang;
                default:
                    throw new Exception("id: " + id + " not allowed here");
            }
        }
    }
}

