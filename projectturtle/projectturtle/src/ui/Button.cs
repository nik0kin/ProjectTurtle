using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ProjectTurtle.src.stuff;
using ProjectTurtle.src.bossfights;
using ProjectTurtle.src.util;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

namespace ProjectTurtle.src.ui {
    class Button /*: Component*/{
        protected IntRect mSize;
		protected Texture texture;
        protected Sprite sprite;
        internal Button() { }
        internal Button(IntRect size) {
            mSize = size;
            sprite = null;
        }
        internal Button(IntRect size, Texture text)
            : this(size) {
			texture = text;
            if (text == null)
               throw new Exception("null texture?");

            sprite = new Sprite(text);
            sprite.Position = new Vector2f((float)size.Left, (float)size.Top);
        }
        /// <summary>
        /// is the cursor inside the button rectangle?
        /// </summary>
        /// <param name="loc">cursor location</param>
        /// <returns></returns>
        internal bool didHit(Vector2i loc) {
            return loc.X >= mSize.Left && loc.X <= mSize.Left + mSize.Width
                && loc.Y >= mSize.Top && loc.Y <= mSize.Top + mSize.Height;
        }
        internal virtual bool didClick(SimpleMouseState mouseState) {
            bool a = mouseState.DoneClickedIt(mouseButton.LEFT);
            bool b = didHit(mouseState.getPos());

            return a && b;
        }
		internal virtual bool didRelease(SimpleMouseState mouseState) {
            return mouseState.DoneReleasedIt(mouseButton.LEFT) && didHit(mouseState.getPos());
        }
        internal virtual void draw(RenderWindow window) {
            if (sprite != null)
                window.Draw(sprite);
        }/*
        internal void draw(RenderRenderWindow window, Texture texture) {
            if (texture != null)
                spriteBatch.Draw(texture, mSize, Color.White);
            else
                throw new Exception("null texture");
        }*/
    }
   class BossListButton : Button{
        BossFightID boss;
        internal BossListButton(IntRect size, BossFightID b)
            : base(size) {
            boss = b;
        }

        internal BossFightID getBossFight() {
            return boss;
        }
    }
    class RollOverTextureButton : Button {
        //Texture texture;
        Texture rolloverTexture;
        Sprite rolloverSprite;
        bool rollover;

        internal RollOverTextureButton(Texture texture, Texture rollOver, Vector2f loc) {
            if (texture == null || rollOver == null) throw new Exception("Null textures");
            rolloverTexture = rollOver;
            this.texture = texture;
            mSize = new IntRect((int)loc.X, (int)loc.Y, (int)texture.Size.X, (int)texture.Size.Y);

            rolloverSprite = new Sprite(rolloverTexture);
            rolloverSprite.Position = loc;
            sprite = new Sprite(texture);
            sprite.Position = loc;
        }
        internal override bool didClick(SimpleMouseState mouseState) {
            rollover = didHit(new Vector2i(mouseState.getPos().X, mouseState.getPos().Y));
            return mouseState.left && rollover;
        }
        internal override void draw(RenderWindow window) {
            if (rollover)
                window.Draw(rolloverSprite);
            else
                window.Draw(sprite);
        }

    }
    /// <summary>
    /// A button that when you hover over it, it changes colors
    /// </summary>
    class RollOverStringButton : Button {
        Font font;
        Color normalColor, rolloverColor;
        Vector2f loc;
        bool rollover;
        Text textString;

        internal RollOverStringButton(Vector2f loc, string s, Font font, Color normalColor, Color rolloverColor, bool bold, uint fontSize) {
            this.font = font;
            this.normalColor = normalColor;
            this.rolloverColor = rolloverColor;
            this.loc = loc;
            
            textString = new Text(s, font, fontSize);
		
		    if(bold)
			    textString.Style = Text.Styles.Bold;
    		
		    textString.Position = (loc);
		    rollover = false;

		    //sf::Vector2f fontSz = font.MeasureString(s);
            mSize = new IntRect((int)loc.X, (int)loc.Y, (int)GetWidth(textString), (int)fontSize);
        }
        internal override bool didClick(SimpleMouseState mouseState) {
            rollover = didHit(mouseState.getPos());
            return mouseState.DoneClickedIt(mouseButton.LEFT) && rollover;
        }
        internal override void draw(RenderWindow window) {
            if (!rollover)
                textString.Color = (normalColor);
            else
                textString.Color = (rolloverColor);

            window.Draw(textString);
        }
        //from the internets:
        internal static int GetWidth(Text Text){
		    //Make sure its not empty, because this would crash it later otherwise.
		    if( Text.ToString().Length == 0 )
			    return 0;

		    //Temp variables that use the same font and size as the original text.
		    //This is to get the width of one '#'
		    Text Temp1 = new Text( "#", Text.Font, Text.CharacterSize );
		    Text Temp2 = new Text( "# #", Text.Font, Text.CharacterSize );
    		
		    //Now we can get the width of the whitespace by subtracting the width of 2 '#' from "# #".
		    int Width = (int)(Temp2.GetGlobalBounds().Width - Temp1.GetGlobalBounds().Width*2);

		    //The width of the string without taking into consideration for spaces left at the beggining or end.
		    int TotalWidth = (int)Text.GetGlobalBounds().Width;

		    //Get the text from the sf::Text as an std::string.
		    string str = Text.ToString();

		    //Bool to see if we encounter a string consisting of only spaces.
		    bool OnlySpaces = false;

		    //Go through all characters in the string from the start.
		    for( int i = 0; i < str.Length; i++ )
		    {
			    if( str[i] == ' ' )
				    //Add each space's width to the sum of it all.
				    TotalWidth += Width;
			    else
				    break;

			    //Check if we have gone through the whole string.
			    if( i == str.Length -1 )
				    OnlySpaces = true;
		    }

		    //Go through all characters in the string from the end, as long as it wasn't only spaces.
		    if( !OnlySpaces )
		    {
                for (int i = str.Length - 1; i > 0; i--)
			    {
                    if (str[i] == ' ')
					    //Add each space's width to the sum of it all.
					    TotalWidth += Width;
				    else
					    break;
			    }
		    }

		    return TotalWidth;
	    }
    }

}
