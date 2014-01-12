using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ProjectTurtle.src.ui;
using ProjectTurtle.src;
using ProjectTurtle.src.screens;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

namespace ProjectTurtle{
	public class Backdrop{
        static Vector2f TEXTOFFSET = new Vector2f(100, 100);

		String backgroundString, waterString;
		Texture backgroundTexture;
        Texture waterTexture;
        Sprite backgroundSprite, waterSprite;

        List<PropSprite> topProps, lowProps;
        int offset;
        /// <summary>
        /// The FloatRects are relative to inside that Texture sent in
        /// </summary>
		public Backdrop(String[] bgText, FloatRect startSquare, FloatRect finalSquare){
			backgroundString = bgText[0];
            waterString = bgText[1];

            topProps = new List<PropSprite>();
            lowProps = new List<PropSprite>();

           
		}
		
		internal void LoadContent(){
            backgroundTexture = GameBox.loadTexture(backgroundString);
            waterTexture = GameBox.loadTexture(waterString);

            backgroundSprite = new Sprite(backgroundTexture);
            waterSprite = new Sprite(waterTexture);
		}
        internal void UnloadContent() {

        }
        //highestY unit
		internal bool update(double gameTime, int highestY){
            if (offset + 5 < highestY && offset < GameBox.GAMERESY) {
                offset = highestY + 5;
                if (offset > 600) {
                    offset = 600;
                    return false;
                } 
            }
            return true;
		}
		//start=drawstart(used for screen shakes?)
		internal void draw(RenderWindow window, Vector2f start){
            IntRect destRect = new IntRect((int)start.X, (int)start.Y, GameBox.GAMERESX, GameBox.GAMERESY);
            IntRect sourRect = new IntRect((int)TEXTOFFSET.X, (int)TEXTOFFSET.Y + offset, GameBox.GAMERESX, GameBox.GAMERESY);
            //background
            backgroundSprite.Position = start;
            backgroundSprite.TextureRect = sourRect;
            window.Draw(backgroundSprite);
            //low props
            foreach (PropSprite ps in lowProps) {
                ps.draw(window, new Vector2f(0, offset));
            }
            //wate r
            waterSprite.Position = start;
            waterSprite.TextureRect = sourRect;
            window.Draw(waterSprite);
            //high pros
            foreach (PropSprite ps in topProps) {
                ps.draw(window, new Vector2f(0, offset));
            }

}

        internal void addTopProp(PropSprite s, bool top) {
            if (top)
                topProps.Add(s);
            else
                lowProps.Add(s);
		}
	}
    internal struct PropSprite {
        //internal Vector2f v;
        //internal Texture text;
        Sprite sprite;
        internal PropSprite(Texture t, Vector2f pos){
            sprite = new Sprite(t);
            sprite.Position = pos;
        }
		
		internal void draw(RenderWindow window,Vector2f offset){
            window.Draw(sprite);
		}
    }
    internal class forest {
        internal const string PATH = "images/ENVIRONMENTS/1_forest/";
        const short amt = 10;
        static Texture[] textures = new Texture[amt];

        internal static void LoadContent() {//hope this works, whata gem
            for (int i = 0; i < textures.Length; i++) {
                textures[i] = GameBox.loadTexture(PATH + ((prop)i).ToString());
            }
        }

        internal enum prop{
            rock1a,rock2a,rock3a,rock3b,rock4a,rock4b,rock5a,rock5b,
            tree1,tree2,tree3,tree4
        }
        internal static Texture getPropTexture(prop p){
            return textures[(int)p];
        }
    }
    
}

