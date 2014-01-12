using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.objects;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.util;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src {
    class Projectile {
        static int num = 0;

        float travelTime = 100;

        bool collide;
        int id = num++;
        Vector2f currentLoc;
        Vector2f mid;//relative to nothing
        Moveable owner, dest;
        Texture currentTexture;
        IntRect? currentTextureRect;
        Sprite currentSprite;
        Func<Moveable, bool> doAbility;
        protected double startMoveTime;
        float scale;

        //for animations?
        AnimationPlayer animationPlayer;

        internal Projectile(Moveable owner, Vector2f start, Moveable d, Func<Moveable, bool> doA, Texture texture, IntRect rect) 
            : this(owner,start,d,doA,texture,rect,1.0f){

        }

        internal Projectile(Moveable owner, Vector2f start, Moveable d, Func<Moveable, bool> doA, Texture texture, IntRect rect, float scale) {
            currentLoc = start;
            dest = d;
            doAbility = doA;
            currentTexture = texture;
            currentTextureRect = rect;
            collide = false;
            this.owner = owner;
            this.scale = scale;

            xNeeded = d.getMid().X - start.X;
            yNeeded = d.getMid().Y - start.Y;

            mid = new Vector2f(rect.Width / 2, rect.Height / 2);
            scale = 1.0f;

            currentSprite = new Sprite(currentTexture);
            currentSprite.Scale = new Vector2f(scale, scale);
            currentSprite.Position = start;
            currentSprite.Origin = mid;

        }
        //with collide
        internal Projectile(Moveable owner, Vector2f start, Moveable d, Func<Moveable, bool> doA, Texture texture, IntRect rect, float scale, bool collide)
                : this(owner,start,d,doA,texture,rect,scale){
            this.collide = collide;
        }
        //collide and animation
        internal Projectile(Moveable owner, Vector2f start, Moveable d, Func<Moveable, bool> doA, Animation animation, float scale, bool collide)
            : this(owner, start, d, doA, null, new IntRect(0, 0, animation.FrameWidth, animation.FrameHeight), scale) {
            this.collide = collide;

            animationPlayer = new AnimationPlayer();
            animationPlayer.PlayAnimation(animation);
            
        }
        double xNeeded;
        double yNeeded;
        float time, runspeed = .01f;
        internal void Update(double gameTime) {
            if (startMoveTime == 0) startMoveTime = gameTime;
            //movement code (for not moving targets)
            
            time += (float)(gameTime - GameBox.getInstance().getLastUpdateTime()) / 1000;
            while (time > runspeed) {//copied from animation player to normalize stuff with time?
                time -= runspeed;
                
                double amtMovedX = ((gameTime - startMoveTime) / (travelTime * 1000)) * xNeeded;
                currentLoc.X += (float)amtMovedX;

                double amtMovedY = ((gameTime - startMoveTime) / (travelTime * 1000)) * yNeeded;
                currentLoc.Y += (float)amtMovedY;
            }

            bool s = false;
            bool s1 = false;
            if (Math.Sign(xNeeded) == 0) { s = true; }
            else if (Math.Sign(xNeeded) == -1 && currentLoc.X < dest.getMid().X) { s = true; } 
            else if (Math.Sign(xNeeded) == 1 && currentLoc.X > dest.getMid().X) { s = true; }
            if (Math.Sign(yNeeded) == 0) { s1 = true; }
            else if (Math.Sign(yNeeded) == -1 && currentLoc.Y < dest.getMid().Y) { s1 = true; } 
            else if (Math.Sign(yNeeded) == 1 && currentLoc.Y > dest.getMid().Y) { s1 = true; }
            //hit stuff code
            if (s && s1){//Math.Abs(currentLoc.X - dest.playerX) < .1 && Math.Abs(currentLoc.Y - dest.playerY) < .1) {
                //hit then
                doAbility(dest);
                InGame.getInstance().removeProjectile(this);
            }
        }
        internal void Draw(double gameTime, RenderWindow window) {
            float rot = (float)Math.Atan((yNeeded / xNeeded));
            if (rot < 0) {
                if (yNeeded < 0) {//TODO can this be more elegant?
                    rot += (float)Math.PI / 2;
                } else
                    rot -= (float)Math.PI / 2;
            } else
                if (yNeeded < 0) {
                    rot -= (float)Math.PI / 2;
                } else
                    rot += (float)Math.PI / 2;
                
            if (currentTexture != null) {
                currentSprite.Position = currentLoc;
                currentSprite.TextureRect = currentTextureRect.Value;
                currentSprite.Rotation = rot;
                window.Draw(currentSprite);
            } else if (animationPlayer != null)
                animationPlayer.Draw(gameTime, window, currentLoc, rot, scale, Direction.east);//dir doesnt matter
            //current loc, is topright coord, and animationplayer takes bottommid, but lets not worry about this right now
            // its why projectiles dont look like they all go to the same place

        }
        internal bool doesCollide() {
            return collide;
        }
        internal Vector2f getMid() {
            return new Vector2f(currentLoc.X+mid.X,currentLoc.Y+mid.Y);
        }
        internal Moveable getOwner() {
            return owner;
        }
        

    }
}
