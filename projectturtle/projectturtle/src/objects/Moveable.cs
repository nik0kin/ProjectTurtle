using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.stuff;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.util;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.objects {
    enum MoveableEnum {
        player, boss,
        mage, puri, add
    }
    class Moveable {
        internal float destY {
            get {
                return y;
            }
            set {
                y = value;
            }
        }

        //remove above
        internal const float PLAYER_SPEED = 2.0f;
        internal const int CIRCLE_SZ = 25, CIRCLE_BOSS_SZ = 100;
        const short BOUNCEHEIGHT = 5;
        internal const float DEFAULT_RUNSPEED = .02f;//or update speed?

        static Texture TESTWHITEBOX;

        protected static Texture playerCircleTexture;
        protected static Texture destCircleTexture;
        protected static Texture redDotTexture;
        protected static Texture blueCircleTexture;

        internal float pX = 1, pY = 1, prevPlayerX = 1, prevPlayerY = 1;
        internal float playerX{
            get {
                return pX;
            }
            set {
                if (pY != pX && value == pY)
                    Console.Write(' ');
                pX = value;
            }
        }
        internal float playerY {
            get {
                return pY;
            }
            set {
                if (pY != pX && value == pX)
                    Console.Write(' ');
                pY = value;
            }
        }

        internal float destX = 1, y = 1;
        internal float currentHp = 5;
        internal int maxHp;
        protected int innerCircle;
        bool dead;
        protected double startMoveTime;
        protected int circleSize;
        protected MoveableEnum type;
        protected Texture currentTexture;//TODO will be old
        protected SimpleModel model;
        protected string name;
        protected Moveable target;
        protected Buffs buffs;
        protected Direction direction;
        protected Direction? overrideDirection = null;
        protected WeaponPlayer weaponPlayer;
        protected IntRect boundingRect;//for selection clicks
        Sprite destCircleSprite = new Sprite(destCircleTexture);
        Sprite currentSprite = new Sprite(playerCircleTexture);
        Text currentHPText = new Text("currentHP", GameBox.corbalFont, 10U);

        float time;//for update?
        protected float runspeed = DEFAULT_RUNSPEED ;
        //moving
        protected int rangeNeeded = 0;

        protected bool visible = true;

        protected short bump = 0;
        private bool up = true;

        public Moveable() {
            buffs = new Buffs(this);
        }
        public Moveable(Vector2f v) {
            playerX = destX = (int)v.X;
            playerY = destY = (int)v.Y;
            circleSize = CIRCLE_SZ;
            currentTexture = playerCircleTexture;
            buffs = new Buffs(this);

            destCircleSprite = new Sprite(destCircleTexture);
            currentSprite = new Sprite(playerCircleTexture);

            currentHPText = new Text("currentHP", GameBox.corbalFont,10U);
        }
        public Moveable(Vector2f v, MoveableEnum mobEnum) {
            playerX = destX = (int)v.X;
            playerY = destY = (int)v.Y;
            type = mobEnum;
            if (mobEnum == MoveableEnum.player) {
                circleSize = CIRCLE_SZ;
                currentTexture = playerCircleTexture;
                currentHp = maxHp = 15;
            }
            buffs = new Buffs(this);

            
        }

        internal static void LoadContent() {
            playerCircleTexture = GameBox.loadTexture("images/circle");
            destCircleTexture = GameBox.loadTexture("images/avatars/circle_dest");
            redDotTexture = GameBox.loadTexture("images/red_dot");
            blueCircleTexture = GameBox.loadTexture("images/circle_select");

            TESTWHITEBOX = GameBox.loadTexture("images/UI/insideFrame");
        }
        internal virtual void Update(double gameTime) {
            if (!visible) return;
            if (currentHp <= 0) return;
            updatePathing(gameTime);
            //buffs.Update(gameTime);
            updateFacing();

            //update bump moved into pathing loop?

            
        }
        internal virtual void Draw(double gameTime, RenderWindow window, bool selected) {
            if (!visible) return;
            if ((destX != playerX || destY != playerY) && this is PlayerClassI) {//dest circle
                //TODO put this code in update or somewhere when dest changes:
                Vector2f v = new Vector2f(destX - destCircleTexture.Size.X / 2, destY - destCircleTexture.Size.Y / 2);
                destCircleSprite.Position = v;
                
                destCircleSprite.Color = PlayerClassI.getClassColor(PlayerClassI.getClassNum((PlayerClassI)this));
                window.Draw(destCircleSprite);
            }
            if (model == null) {
                currentSprite.Position = new Vector2f(playerX, playerY);
                window.Draw(currentSprite);
            } else
                model.draw(window, gameTime, new Vector2f(playerX, playerY), new Vector2f(0, bump), Moveable.hpToColor(getHPpercent(), selected));

            Vector2f topright = GeoLib.midBottomPosToTopRight(getMid(), SimpleModel.CLASS_RECT, SimpleModel.CLASS_SCALE);
            Vector2f wepPos = new Vector2f(topright.X, topright.Y + bump);

            if(weaponPlayer != null)
                weaponPlayer.draw(window, gameTime, wepPos, direction);

            int i = currentHp > 10 ? 6 : 3;//spacing
            currentHPText.DisplayedString = currentHp + "";
            currentHPText.Position = new Vector2f(playerX - i, playerY);
            window.Draw(currentHPText);
            //draw buffs
            buffs.draw(window, getMid());

            Sprite sprite = new Sprite(TESTWHITEBOX);
            sprite.Color = new Color(255,78,0,100);
            
            FloatRect fr = GeoLib.midBottomPosToTopRightRect(getMid(), boundingRect);
            sprite.Position = new Vector2f(fr.Left, fr.Top);
            sprite.Scale = new Vector2f(fr.Width/29,fr.Height/29); 
            window.Draw(sprite);
        }
        protected void updateFacing() {
            if (model != null) {
                if (overrideDirection.HasValue)
                    model.changeDirection(overrideDirection.Value);
                else
                    model.changeDirection(direction);
            }
        }
        private void updatePathing(double gameTime) {
            short x = 0,y = 0;

            time += (float)(gameTime - GameBox.getInstance().getLastUpdateTime()) / 1000;
            while (time > runspeed) {//copied from animation player to normalize stuff with time?
                time -= runspeed;

                if (destX != playerX) {
                    double amtMovedX = ((gameTime - startMoveTime) / 1000) * PLAYER_SPEED;
                    if ((destX - playerX) > 0) {//east
                        double a = (playerX + amtMovedX);
                        if (Math.Abs(a) > destX + .1)
                            playerX = destX;
                        else
                            playerX = (float)a;
                        x = 1;
                    } else if ((destX - playerX) < 0) {//west
                        double a = (playerX - amtMovedX);
                        if (Math.Abs(a) < destX + .1)
                            playerX = destX;
                        else
                            playerX = (float)a;
                        x = -1;
                    }
                }
                if (destY != playerY) {
                    double amtMovedY = ((gameTime - startMoveTime) / 1000) * PLAYER_SPEED;
                    if ((destY - playerY) > 0) {//north
                        double a = (playerY + amtMovedY);
                        if (Math.Abs(a) > destY + .1)
                            playerY = destY;
                        else
                            playerY = (float)a;
                        y = -1;
                    } else if ((destY - playerY) < 0) {//south
                        double a = (playerY - amtMovedY);
                        if (Math.Abs(a) < destY + .1)
                            playerY = destY;
                        else {
                            playerY = (float)a;
                        }
                        y = 1;
                    }
                }
                //update bump
                if (isMoving()) {
                    if (up)
                        bump--;
                    else
                        bump++;
                    if (bump < 0 || bump > BOUNCEHEIGHT) up = !up;
                } else {
                    bump = 0;
                    up = true;
                } 
                //figure out direction
                if (x == 1) 
                    direction = Direction.east;
                else if (x == -1) 
                    direction = Direction.west;
                if (y == 1) 
                    direction = Direction.north;
                else if (y == -1) 
                    direction = Direction.south;

                if (x == 0 && y == 0) {
                    if (target != null) {
                        //look at target
                        direction = getDirectionOfUnit(target);
                    } else
                        direction = Direction.south;//default
                }
            }//end run while?

            if (rangeNeeded > 0 && InGame.isInRange(getMid(), getDestMid(),rangeNeeded)) {//if we are close enough to that one spot...
                //then we are there
                destX = playerX;
                destY = playerY;
                rangeNeeded = -1;
            }
        }
        //startTime in TotalMiliseconds, spot is middle?
        internal void sendPlayerTo(Vector2f spot, double startTime) {
            if (currentHp <= 0) return;
            //instantly right now
            destX = (int)(spot.X); //- circleSize / 2);
            destY = (int)(spot.Y); // - circleSize / 2);
            prevPlayerX = playerX;
            prevPlayerY = playerY;
            if (target != null) {
                double d = InGame.getDistanceBetweenCircles(getDestMid(), getInnerRadius(), target.getMid(), target.getInnerRadius());
                if (!checkDestCircleRange(target, getRange())) {
                    target = null;
                }
            }

            startMoveTime = startTime;
        }
        internal void sendPlayerInRangeOf(Vector2f spot, int range, double startTime) {
            //if (type == MoveableEnum.boss) return;TODO make it so the player cant move anything else
            if (currentHp <= 0) return;
            //instantly right now
            //find destXY thats in range
            //A2 + B2 = C2 
            //it will just follow reg pathing, but there will be a flag, that can be tripped if a player is going to a range
            rangeNeeded = range;
            destX = (int)(spot.X); //- circleSize / 2);
            destY = (int)(spot.Y); // - circleSize / 2);
            //Vector2f dMid = getDestMid();
            prevPlayerX = playerX;
            prevPlayerY = playerY;

            startMoveTime = startTime;
        }
        internal void sendPlayerInCircleRangeOf(Vector2f spot, int spotR, int range, double startTime) {
            if (type == MoveableEnum.boss) return;
            if (currentHp <= 0) return;
            //http://www.vb-helper.com/howto_net_line_circle_intersections.html
            //TODO I CANT WAIT TO REMAKE THIS LOL
            double x1 = getMid().X, y1 = getMid().Y;
            double x2 = spot.X, y2 = spot.Y;

            double cx = spot.X, cy = spot.Y;

            double dx = x2 - x1;
            double dy = y2 - y1;
            double t;
            double radius = spotR;

            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (x1 - cx) + dy * (y1 - cy));
            double C = (x1 - cx) * (x1 - cx) + (y1 - cy) * (y1 - cy) - radius * radius;

            double det = B * B - 4 * A * C;
            //if( (A <= 0.0000001) || (det < 0) ){
                //' No real solutions.
                //Return 0
            //}else if( det == 0){
                //' One solution.
                t = -B / (2 * A);
                spot.X = (float)(x1 + t * dx);
                spot.Y = (float)(y1 + t * dy);
                //Return 1
            //}else {
                //' Two solutions.
                t = (-B + Math.Sqrt(det)) / (2 * A);
                double ix1 = x1 + t * dx;
                double iy1 = y1 + t * dy;
                t = (-B - Math.Sqrt(det)) / (2 * A);
                double ix2 = x1 + t * dx;
                double iy2 = y1 + t * dy;
                //Return 2
            //}

            rangeNeeded = range;
            destX = (float)(ix2);// - circleSize / 2);
            destY = (float)(iy2);// - circleSize / 2);
            prevPlayerX = playerX;
            prevPlayerY = playerY;

            startMoveTime = startTime;
        }
        /*
         * internal static bool areCircleswithRange(Vector2f v1, int r1, Vector2f v2, int r2, int range) {
            float dx = v1.X - v2.X;
            float dy = v1.Y - v2.Y;
            if (Math.Sqrt((dy * dy) + (dx * dx)) > (r1 +range + r2))
                return false;
            return true;
        }*/
        //old before playerX/Y was the middle
        /*internal bool isInMe(int x, int y) {
            double center_x = playerX + circleSize / 2;
            double center_y = playerY + circleSize / 2;

            double D = Math.Sqrt(Math.Pow(center_x - x, 2) + Math.Pow(center_y - y, 2));
            return D <= circleSize / 2;
        }*/
        internal bool isInMe(int x, int y) {
            return isInMe(new Vector2f(x, y));
        }

        internal bool isInMyHextangle(Vector2f v) {
            IntRect r = SimpleModel.HEXHP_RECT;
            float scale = model.getHPScale();
            return GeoLib.isInRect(new FloatRect((int)(playerX - r.Width / 2 * scale), (int)(playerY - r.Height / 2 * scale), (int)(r.Width * scale), (int)(r.Height * scale)), v);
        }
        internal bool isInMe(Vector2f v){
            //check if that vector is posisitioned in the bounding rectangle
            return GeoLib.isInRect(GeoLib.midBottomPosToTopRightRect(getMid(), boundingRect), v);
            
        }
        internal Vector2f getPosition() {
            return new Vector2f(playerX,playerY);
        }
        internal void setPosition(Vector2f v) {
            playerX = destX = (int)v.X;
            playerY = destY = (int)v.Y;
        }
        internal Vector2f getMid() {
            //return new Vector2f(playerX + circleSize / 2, playerY + circleSize / 2);
            return new Vector2f(playerX, playerY);
        }
        internal Vector2f getDestMid() {
            //return new Vector2f(destX + circleSize / 2, destY + circleSize / 2);
            return new Vector2f(destX, destY);
        }
        internal bool isMoving(){
            return !(playerX == destX && playerY == destY);
        }
        //returns if it killed them or not
        internal virtual bool takeDamage(Moveable giver, float amount) {
            if (currentHp - amount <= 0)
                amount = currentHp;
            InGame.getInstance().addToDamageMeter(giver, amount);
            currentHp -= amount;
            if (currentHp <= 0) {
                currentHp = 0;
                dead = true;
            }
            return currentHp == 0;
        }
        internal void heal(int amt) {
            if (currentHp >= maxHp || dead) return;
            currentHp += amt;
            if (currentHp > maxHp) currentHp = maxHp;
        }
        internal double getHPpercent() { return currentHp / maxHp; }
        internal bool isDead() { return dead; }
        internal string getName() { return name; }
        internal int getInnerRadius() { return innerCircle; }

        //returns true if you arent in range, then if you arent, runs you to be in range
        internal bool checkRangeAndMove(Vector2f spot, int range, double gameTime) {
            bool aBool;
            Vector2f dMid = getDestMid();

            if (aBool = (!InGame.isInRange(getMid(), spot, range)) && !(dMid.X == spot.X && dMid.Y == spot.Y)) {// so it doesnt spam
                sendPlayerInRangeOf(spot, range, gameTime);
            }
            if (this is ProjectTurtle.src.bossfights.Ursidor && !aBool && ((ProjectTurtle.src.bossfights.Ursidor)this).charging)
                Console.WriteLine("wtf");
            return aBool;
        }
        internal bool checkCircleRangeAndMove(Moveable mob, int range, double gameTime) {
            bool aBool;
            if (aBool = (!InGame.areCircleswithRange(getMid(),getInnerRadius(), mob.getMid(),mob.getInnerRadius(), range))) {
                sendPlayerInCircleRangeOf( mob.getMid(), mob.getInnerRadius(), range, gameTime);
            }
            return aBool;
        }
        internal bool checkCircleRange(Moveable mob, int range){
            return InGame.areCircleswithRange(getMid(),getInnerRadius(), mob.getMid(),mob.getInnerRadius(), range);
        }
        internal bool checkDestCircleRange(Moveable mob, int range) {
            return InGame.areCircleswithRange(getDestMid(), getInnerRadius(), mob.getMid(), mob.getInnerRadius(), range);
        }
        internal void addBuff(Buff b) {
            buffs.addBuff(b);
        }
        internal void removeBuffType(BuffType buffType) {
            buffs.removeBuffType(buffType);
        }
        internal void updateBuffs(double gameTime) {
            buffs.Update(gameTime);
        }
        internal virtual int getRange() {
            return -1;
        }
        internal void setVisable(bool b) {
            visible = b;
        }
        //TODO we dont need to make new colors everytime
        private static Color hpToColor(double p,bool selected) {
            if (p > .7) return new Color(127, 255, 0, (byte)(selected ? 255 : 190));
            if (p > .3) return new Color(255, 255, 0, (byte)(selected ? 255 : 190));
            if (p > 0) return new Color(255, 0, 0, (byte)(selected ? 255 : 190));
            return Color.Cyan;
        }
        //to face Moveable m
        protected Direction getDirectionOfUnit(Moveable m) {
            if (target == null) return Direction.south;//default dir

            float xD = (m.playerX - playerX);
            float yD = m.playerY - playerY;

            if (Math.Abs(xD) > Math.Abs(yD)) {//side to side oriented
                if (xD > 0)//east
                    return Direction.east;
                else
                    return Direction.west;
            } else {//up down
                if (yD > 0)//south
                    return Direction.south;
                else
                    return Direction.north;
            }



            //return Direction.south;
        }


        protected static double getDistance(Moveable ursidor, Moveable fish) {
            return InGame.getDistance(ursidor.getMid(), fish.getMid());
        }
        internal void setRing(Color? c){
            model.setRing(c);
        }

    }
}
