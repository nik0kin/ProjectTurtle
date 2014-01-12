using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.objects;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.effects {
    class GroundEffect {
        static Texture groundFireTexture;
        protected Vector2f mPos;//middle
        int circleSize;
        protected Texture texture;
        protected Sprite sprite;
        Cooldown disappear;
        internal Func<Moveable, bool> doAbility;

        internal static void LoadContent() {
            groundFireTexture = GameBox.loadTexture("images/abilitys/groundfire");
        }

        internal GroundEffect(Vector2f mid, int effectID, Func<Moveable, bool>  doA, double gameTime) {
            mPos = mid;
            doAbility = doA;

            switch (effectID) {//TODO redo with enums?
                case 1:
                    texture = groundFireTexture;
                    circleSize = 25;
                    disappear = new Cooldown(3);
                    disappear.use(gameTime);
                    break;
            }
        }
        internal GroundEffect(Vector2f mid, double gameTime,float lengthSec, Texture texture) {
            mPos = mid;

            this.texture = texture;
            sprite = new Sprite(texture);
            circleSize = (int)texture.Size.X/2;
            disappear = new Cooldown(lengthSec);
            disappear.use(gameTime);
              
        }
        internal virtual bool update(double gameTime) {
            return disappear.use(gameTime);//after a certain amount of time the bomb goes off...
        }
        internal virtual void draw(RenderWindow window) {
            //mPos = new Vector2f(50, 50);
            sprite.Position = mPos;
            window.Draw(sprite);
        }
        internal Vector2f getMid() {
            return new Vector2f(mPos.X + circleSize / 2, mPos.Y + circleSize / 2);
        }

    }
}
