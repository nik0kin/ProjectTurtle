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

namespace ProjectTurtle.src.effects{
	enum Direction{
		south,north, east, west
	};
	public class SimpleModel{
        internal const string BOSS_PATH =  "images/characters/bosses/";
        internal const string CLASS_PATH = "images/characters/units/";
        internal const float CLASS_SCALE = .75f;
        internal static IntRect CLASS_RECT = new IntRect(0, 0, 64, 84);
        internal static IntRect HEXHP_RECT = new IntRect(0, 0, 40, 16);
        internal static IntRect CLASS_BOUNDING_RECT = new IntRect(0, 0, 50, 84);

        static Texture healthHexTexture, extraHexRingTexture;

		Direction direction;
        IntRect rect, sourceRect;
		Texture texture;
        Color? ringColor;
        float scale, hpScale;
        AnimationPlayer animationPlayer;
        Sprite healthHexSprite, extraHexRingSprite, sprite;
		
        internal static void LoadContent(){
            healthHexTexture = GameBox.loadTexture("images/characters/bottomhealthhexagon");
            extraHexRingTexture = GameBox.loadTexture("images/characters/bottombuffhexagon");
        }

        public SimpleModel(Texture texture, IntRect rect, float scale) : this(texture,rect,scale,scale){

        }
        public SimpleModel(Texture texture, IntRect rect, float scale, float hpScale) {
			this.texture = texture;
			if(texture == null)
				throw new Exception("null texture");
            this.rect = rect;
            sourceRect = new IntRect(0, 0, rect.Width, rect.Height);
            this.scale = scale;
            this.hpScale = hpScale;
            animationPlayer = new AnimationPlayer();

            healthHexSprite = new Sprite(healthHexTexture);
            extraHexRingSprite = new Sprite(extraHexRingTexture);
            sprite = new Sprite(texture);
		}
        //pos is in the middle of the bottom of the sprite, also in the middle of healthHexTexture
		internal void draw(RenderWindow window, double gameTime, Vector2f midPos,Vector2f bump,Color healthColor){
            Vector2f v = new Vector2f(midPos.X - healthHexTexture.Size.X / 2.0f * hpScale, midPos.Y - healthHexTexture.Size.Y / 2.0f * hpScale);
            //inside health hex 
            healthHexSprite.Position = v;
            healthHexSprite.Scale = new Vector2f(hpScale, hpScale);
            healthHexSprite.Color = healthColor;
            window.Draw(healthHexSprite);
            //alert ring
            if (ringColor.HasValue) {
                extraHexRingSprite.Position = v;
                extraHexRingSprite.Scale = new Vector2f(hpScale, hpScale);
                extraHexRingSprite.Color = ringColor.Value;
                window.Draw(extraHexRingSprite);
            }
            //draw model
            Vector2f drawPos = new Vector2f(midPos.X - rect.Width / 2.0f * scale + (int)bump.X, midPos.Y - rect.Height * scale + (int)bump.Y);
            if (!animationPlayer.Draw(gameTime, window, midPos, scale, direction)) {
                sprite.TextureRect = sourceRect;
                sprite.Position = drawPos;
                sprite.Scale = new Vector2f(scale,scale);
                window.Draw(sprite);
            }
            
		}

		internal void changeDirection(Direction d){
            if (d == direction)
                return;
			direction = d;
            int frame;

            if (d == Direction.south)
                frame = 0;
            else if (d == Direction.north)
                frame = 1;
            else if (d == Direction.east)
                frame = 2;
            else //west
                frame = 3;

            sourceRect = new IntRect((int)frame * rect.Width, 0, rect.Width, rect.Height);
		}
		internal Direction getDirection(){
			return direction;
		}

        //quick on/off indicator
        internal void setRing(Color? color) {
            this.ringColor = color;
        }

        internal float getHPScale() {
            return hpScale;
        }
        internal void setAnimation(Animation ani) {
            animationPlayer.PlayAnimation(ani,null);
        }
        internal void setAnimation(Animation ani, Func<bool> function) {
            animationPlayer.PlayAnimation(ani,function);
        }

        internal bool isAnimating() {
            return animationPlayer.isAnimating();
        }
    }
}

