using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ProjectTurtle.src.UI
{
    class Sprite  /*: Component*/ {
        internal Rectangle rect;
        internal Texture2D text;
        internal Sprite(Texture2D t){
            text = t;
            rect = new Rectangle(0, 0, t.Width, t.Height);
            if (text == null) throw new Exception("null texture" );
        }
		internal Sprite(Rectangle r, Texture2D t) : this(t)
        {
            rect = r;
        }
		internal Sprite(Texture2D t, Vector2 pos) : this(t){
            rect = new Rectangle((int)pos.X, (int)pos.Y, t.Width, t.Height);
		}
		
		internal void draw(SpriteBatch spriteBatch){
			spriteBatch.Draw(text,rect,Color.White);
          
		}
    }
}
//encapsulation, polymorphism, inheritance