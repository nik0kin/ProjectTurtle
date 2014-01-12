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
    //this class will show the icon either way, and if you hover over it, it will show info! yay!
    class IconToolTip{
        static Texture blackbg;//a white image we'll draw anycolor
        internal static void loadContent(){
            blackbg = GameBox.loadTexture("images/UI/insideFrame");
        }

        Sprite icon;
        Sprite bgSprite;

        Text titleText, descText;

        string title;
        string desc;
        Vector2f titleV, descV;
        bool hover, disable;
        //FloatRect outerRect;

        ItemID mItem = ItemID.na;

        internal IconToolTip(Sprite icon, string tooltipTitle, string tooltipDesc){
            this.icon = icon;
            title = tooltipTitle;
            desc = tooltipDesc;
            disable = false;
            hover = false;
		    mItem = ItemID.na;

		    bgSprite = new Sprite(blackbg);
            bgSprite.Color = (Color.Black);

            titleText = new Text("", GameBox.corbalFont, 20U);
            titleText.Color = (Color.White);
		    descText = new Text("",GameBox.corbalFont,20U);
            descText.Color = (Color.White);

        }
        //ineffici? EFF
        internal void update(SimpleMouseState mouseState, Vector2f pos, ItemID item){
            icon.Texture = (Item.getItemTexture(item));
            if (item != mItem) {//if the current item is different...
                if (item != ItemID.none) {
                    Item ite = new Item(item);
                    title = ite.name;
                    desc = ite.description;
                    disable = false;
				    //delete ite;
			    } else { title = desc = "noneZ"; disable = true; }//or disable tooltip
                mItem = item;
            }
		    icon.Position = pos;//icon.rect.X = (int)pos.X; icon.rect.Y = (int)pos.Y;
            

            //very basic, doesnt care ifyou are near end of screen or anything
		    FloatRect rect = icon.GetGlobalBounds();	
    		
		    titleV = new Vector2f(rect.Left + rect.Width + 5, rect.Top);//TODOX we are making alot of news without any deletes here
		    descV = new Vector2f(rect.Left, rect.Top + rect.Height + 5);

            float startX = rect.Left - 5, startY = rect.Top - 5;
            float width = 5 + rect.Width + 50 /*title.Length * 10*/ + 5;
            float height = 5 + rect.Height + 30;
            //outerRect = new Rect<float>(startX, startY, width, height);
		    bgSprite.Position = new Vector2f((float)startX,(float)startY);
            bgSprite.Scale = new Vector2f((float)width, (float)height);

		    titleText.DisplayedString = (title);
		    titleText.Position = (titleV);
		    descText.DisplayedString = (desc);
		    descText.Position = (descV);

            hover = GeoLib.isInRect(icon.TextureRect, mouseState.getPos() );
        }
        internal void draw(RenderWindow window){
            if (hover) return;//not sure when this is ever called
            window.Draw(icon);
        }
        internal void drawToolTip(RenderWindow window) {
            if (!hover) return;
            if (disable) {
                window.Draw(icon);
                //spriteBatch.Draw(icon.text, icon.rect, Color.White); 
                return;
            }

            window.Draw(bgSprite);
            window.Draw(icon);

            window.Draw(titleText);
            window.Draw(descText);
        }
    }
}
