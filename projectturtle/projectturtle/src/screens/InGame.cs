using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.objects;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.util;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.bossfights;

namespace ProjectTurtle.src.screens {
    class InGame {
        const double TIMETILPOSTGAME = 5.0;
        const int COOLDOWN_DECIMAL_PLACES = 1;
        const int NOTE_STAY_TIME = 5;

        //keybinds?
        const Keyboard.Key TARGET_KEY = Keyboard.Key.A;
        const Keyboard.Key RESTART_KEY = Keyboard.Key.Numpad0;
        const Keyboard.Key TARGETALL_KEY = Keyboard.Key.S;
        const Keyboard.Key USE_KEY = Keyboard.Key.D;
        const Keyboard.Key RAID_TAB_KEY = Keyboard.Key.Tab;

        internal static Random randy = new Random();
        internal static InGame inGame; 

        SimpleMouseState mouseState, prevMouseState;
        SimpleKeyboardState keyState, prevKeyState;

        Texture targetTexture;
        Texture currentCursor;

        Sprite currentCursorSprite;
        Text selectedCoordText, firstCooldownText, secondCooldownText, bossCooldownText, enrageText, 
                damagemeterText, firstThreatText, secondThreatText, thirdThreatText, fightStatusText,
                countDownText;

        bool targetNeeded = false;//TODO make this into an enum :D and organize all this
        bool targetAll;

        //ui stuff
        Button abilityButton1, abilityButton2, abilityButton3;
        Backdrop mBackdrop; bool bUpdate = true;
        Sprite hpBarSprite;

        List<Moveable> moveables;
        Moveable selected;

        List<Projectile> projectiles, projectilesToAdd, projectilesToRemove;
        List<GroundEffect> groundEffects, groundEffectsToRemove;

        DamageMeter damageMeter;
        BossFight bossFight;
        Timer timer;
        double mGameTime;
        FightStatus fightStatus = FightStatus.preFight;
        int playerAmt;
        double finalTime = 9999;
        bool bossEnraged;
        Hashtable thisRaid;
        RaidFrames raidFrames;
        Enemy mainEnemy;

        List<Script> activeScripts;
        List<FightObject> fightObjects;

        public void LoadContent(BossFight bossFight){
			inGame = this;
            this.bossFight = bossFight;
            
            //botBlackTexture = contentManager.Load<Texture2D>("images/bottom_black");
            //backgroundTexture = contentManager.Load<Texture2D>(bossFight.getBGString());
            //foregroundTexture = contentManager.Load<Texture2D>("images/fg");

            targetTexture = GameBox.loadTexture("images/target");

            moveables = new List<Moveable>();

            abilityButton1 = new Button(new IntRect(650, 560, 35, 35));
            abilityButton2 = new Button(new IntRect(700, 560, 35, 35));
            abilityButton3 = new Button(new IntRect(750, 560, 35, 35));

            projectiles = new List<Projectile>();
            projectilesToAdd = new List<Projectile>();
            projectilesToRemove = new List<Projectile>();
            groundEffects = new List<GroundEffect>();
            groundEffectsToRemove = new List<GroundEffect>();

            damageMeter = new DamageMeter();
			activeScripts = bossFight.getTimedScripts();

            fightObjects = bossFight.getObjects();

            //gfx stuff
            selectedCoordText = new Text("selected coord", GameBox.corbalFont, 15U);
            selectedCoordText.Position = new Vector2f(10, 560);
            selectedCoordText.Color = Color.Yellow;
            firstCooldownText = new Text("first cd", GameBox.corbalFont, 15U);
            firstCooldownText.Position = new Vector2f(300, 550);
            firstCooldownText.Color = Color.White;
            secondCooldownText = new Text("second cd", GameBox.corbalFont, 15U);
            secondCooldownText.Position = new Vector2f(300, 565);
            secondCooldownText.Color = Color.White;
            bossCooldownText = new Text("boss cd", GameBox.corbalFont, 15U);
            bossCooldownText.Position = new Vector2f(300, 575);
            bossCooldownText.Color = Color.White;
            enrageText = new Text("ENRAGE OHHHHHH SHIT SON", GameBox.corbalFont, 15U);
            enrageText.Position = new Vector2f(375, 5);
            enrageText.Color = Color.Red;
            damagemeterText = new Text("damagemeter", GameBox.corbalFont, 15U);
            firstThreatText = new Text("first threat", GameBox.corbalFont, 15U);
            firstThreatText.Position = new Vector2f(15, 15);
            firstThreatText.Color = Color.White;
            secondThreatText = new Text("second threat", GameBox.corbalFont, 15U);
            secondThreatText.Position = new Vector2f(15, 30);
            secondThreatText.Color = Color.White;
            thirdThreatText = new Text("third threat", GameBox.corbalFont, 15U);
            thirdThreatText.Position = new Vector2f(15, 45);
            thirdThreatText.Color = Color.White;
            fightStatusText = new Text("fightstatus", GameBox.corbalFont, 15U);
            fightStatusText.Position = new Vector2f(50, 520);
            fightStatusText.Color = Color.White;
            countDownText = new Text("countDown", GameBox.corbalFont, 15U);
            countDownText.Position = new Vector2f(400, 520);
            countDownText.Color = Color.White;

            //shouldnt load here
            hpBarSprite = new Sprite(GameBox.loadTexture("images/UI/insideFrame"));
            hpBarSprite.Position = new Vector2f(500, 550);
            hpBarSprite.Color = Color.Red;

            currentCursorSprite = new Sprite();
		}
		//oldish one
        public void LoadContent(BossFight bossFight, PlayerClassNum[] raidComp) {
            this.LoadContent(bossFight);

            List<PlayerClassI> playerRaid = PlayerClassI.makeRaid(raidComp, new Vector2f(191,50));
			//TODO preset spots specific to bossfight
            foreach (PlayerClassI p in playerRaid) {
                //place all in moveables
                moveables.Add(p);
            }
            List<Enemy> enemys = bossFight.getEnemies();
            foreach (Moveable m in enemys)
            {
                moveables.Add(m);
            }
            if (enemys.Count >= 1) mainEnemy = enemys[0];

            selected = playerRaid[0];
			playerAmt = playerRaid.Count;
            
            raidFrames = new RaidFrames(this, playerRaid, new Vector2f(5,200));
        }
		//newish one
		public void LoadContent(BossFight bossFight, List<List<ChampItemPair>> raidComp) {
            this.LoadContent(bossFight);

            List<PlayerClassI> playerRaid = PlayerClassI.makeRaid(raidComp, new Vector2f(191,50));
            foreach (PlayerClassI p in playerRaid) {
                //place all in moveables
                moveables.Add(p);
            }
            List<Enemy> enemys = bossFight.getEnemies();
            foreach (Moveable m in enemys)
            {
                moveables.Add(m);
            }
            if (enemys.Count >= 1) mainEnemy = enemys[0];
            selected = playerRaid[0];
			playerAmt = playerRaid.Count;
   
            raidFrames = new RaidFrames(this, playerRaid, new Vector2f(5,200));

            LoadBackdrop(bossFight.getFightID());
        }
        public void LoadContent(BossFight bossFight, PlayerClassNum[] raidComp, Hashtable raidSetup) {
            LoadContent(bossFight, raidComp);
            PositionFactory.getSetupRaid(getAlivePlayerMoveables(), raidSetup);
        }
		public void LoadContent(BossFight bossFight, List<List<ChampItemPair>> raidComp, Hashtable raidSetup) {
            LoadContent(bossFight, raidComp);
            PositionFactory.getSetupRaid(getAlivePlayerMoveables(), raidSetup);
        }
        internal void LoadBackdrop(BossFightID boss) {
            if (mBackdrop != null) mBackdrop.UnloadContent();
            BossFightInfo info = BossFightLibrary.getBossInfo(boss);
            mBackdrop = info.backdrop;
            mBackdrop.LoadContent();
        }
        public void Update(double gameTime, SimpleMouseState mState, SimpleKeyboardState kState) {
            mouseState = mState;
            keyState = kState;
            mGameTime = gameTime;
            if (mouseState.right && mouseState.getPos().Y < 550) {
                tryImoveHere(selected, new Vector2f(mouseState.getPos().X, mouseState.getPos().Y), gameTime);
            }
            if (keyState.IsKeyDown(RESTART_KEY)) {//restart
                GameBox.getInstance().doLastBossFight();
            } 
            if (keyState.IsKeyDown(Keyboard.Key.F1) && !prevKeyState.IsKeyDown(Keyboard.Key.F1)) {//lose fight
                if (fightStatus == FightStatus.fightEngaged || fightStatus == FightStatus.preFight) {
                    fightStatus = FightStatus.fightLost;
                    timer = new Timer(gameTime);
                } else {
                    GameBox.getInstance().initPostFight(bossFight.getFightID(), finalTime, getPlayerAmt(), damageMeter, fightStatus == FightStatus.fightWon, bossFight.getBossInfo().enragetimer,thisRaid);
                }
            }
            if (fightStatus == FightStatus.fightEngaged || fightStatus == FightStatus.preFight) {//stop updatings stuff after fight for now, later we might have mobs move around after 
                foreach (Moveable m in moveables) {
                    m.Update(gameTime);
                    m.updateBuffs(gameTime);
                    //EFF check if i need all these checks inside this forloop
                    if (mouseState.left) {
                        if (targetNeeded && currentCursor == targetTexture && selected is PlayerClassI && m.isInMe(mouseState.getPos().X, mouseState.getPos().Y)) {//if looking for target
                            PlayerClassI p = (PlayerClassI)selected;
                            if (!targetAll)
                                p.setTarget(m, gameTime);
                            else
                                setAllPlayersTarget(m, gameTime);
                            targetNeeded = false;
                            targetAll = false;
                            currentCursor = null;
                        } else if (mouseState.DoneClickedIt(mouseButton.LEFT) && m.isInMe(mouseState.getPos().X, mouseState.getPos().Y)) {
                            //selection happens right here
                            //selected = m;
                            setSelected(m);
                        }
                    }
                }
            }
            if (kState.IsKeyDown(RAID_TAB_KEY) && !prevKeyState.IsKeyDown(RAID_TAB_KEY)){
                //if we hit *TAB* select next person in raid? (maybe closest ?)
                if(selected != null){
                    int index = moveables.IndexOf(selected);
                    do{
                        index = (index + 1) % moveables.Count;
                    }while(moveables[index] is Enemy);
                    setSelected(moveables[index]);
                }
            }
            if (selected is PlayerClassI && kState.IsKeyDown(TARGET_KEY) && !prevKeyState.IsKeyDown(TARGET_KEY)) {
                if (!targetNeeded) {
                    targetNeeded = true;
                    currentCursor = targetTexture;
                } else {
                    targetNeeded = false;
                    currentCursor = null;
                }
            }
            if (selected is PlayerClassI && kState.IsKeyDown(TARGETALL_KEY) && !prevKeyState.IsKeyDown(TARGETALL_KEY)) {
                if (!targetAll) {
                    targetNeeded = true;
                    targetAll = true;
                    currentCursor = targetTexture;
                } else {
                    targetNeeded = false;
                    targetAll = false;
                    currentCursor = null;
                }
            }
            if (selected is PlayerClassI && kState.IsKeyDown(USE_KEY) && !prevKeyState.IsKeyDown(USE_KEY)) {
                ((PlayerClassI)selected).doAbility();
            }
            
            foreach (Projectile p in projectiles) {
                p.Update(gameTime);
                if (p.doesCollide() ) {
                    foreach(PlayerClassI m in getAlivePlayerMoveables()){
                        if(p.getOwner() != m && m.isInMe(p.getMid()))
                            projectilesToRemove.Add(p);
                    }
                    
                }
            }
            foreach (GroundEffect g in groundEffects) {
                if (g.update(gameTime)) {//if thats true, do groundfire
                    if (g.doAbility == null) {
                        removeGroundEffect(g);
                        continue;
                    }
                    List<PlayerClassI> peopleGotHit = new List<PlayerClassI>();
                    foreach (PlayerClassI p in getPlayerMoveables()) {
                        //add people to list near fire
                        if( Math.Sqrt(Math.Pow(p.getMid().X-g.getMid().X,2) + Math.Pow(p.getMid().Y-g.getMid().Y,2)) < 13){
                            peopleGotHit.Add(p);
                        }
                    }
                    foreach(PlayerClassI p in peopleGotHit){
                        g.doAbility(p);
                    }
                    removeGroundEffect(g);
                }
            }
            //clean up
            foreach (Projectile p in projectilesToRemove) {
                projectiles.Remove(p);
            }
            projectilesToRemove.Clear();
            foreach (GroundEffect g in groundEffectsToRemove) {
                groundEffects.Remove(g);
            }
            groundEffectsToRemove.Clear();
            foreach (Projectile p in projectilesToAdd) {
                projectiles.Add(p);
            }
            projectilesToAdd.Clear();
            //check if boss enrages
            if (fightStatus == FightStatus.fightEngaged && !bossEnraged && timer.getTime(gameTime) > bossFight.getBossInfo().enragetimer) {
                bossFight.setEnraged(true);
                bossEnraged = true;
            }

            //end game stuffs
            if (fightStatus == FightStatus.fightEngaged) {
                //check to see if either side has won
                if (bossFight.isBossFightDead()) {//bossdead
                    finalTime = timer.getFormattedTime(gameTime,3);
                    fightStatus = FightStatus.fightWon;
                    //addScreenNote(bossFight.getBossInfo().deathstring); TODO
                    //start timer to go to post game
                    timer.reset(gameTime);
                } else if (!areAnyPlayersAlive()) {//peopledead
                    finalTime = timer.getFormattedTime(gameTime,3);
                    fightStatus = FightStatus.fightLost;
                    //start timer to go to post game
                    timer.reset(gameTime);
                }
            }
            raidFrames.checkClicks(mouseState);
            if (fightStatus == FightStatus.fightLost || fightStatus == FightStatus.fightWon) {
                if (timer.getTime(gameTime) > TIMETILPOSTGAME) {
                    GameBox.getInstance().initPostFight(bossFight.getFightID(), finalTime, getPlayerAmt(), damageMeter, fightStatus == FightStatus.fightWon, bossFight.getBossInfo().enragetimer, thisRaid);
                }
            }
            //scripts
            if(fightStatus == FightStatus.fightEngaged)
                updateScripts(gameTime);
            //backdrops
            if (bUpdate = mBackdrop.update(gameTime, 600))//TODO make its so it moves ya know
                bUpdate = false;
            //objects? but this should be near other important stuff
            foreach (FightObject ob in fightObjects)
                ob.update(gameTime);

            prevMouseState = mouseState;
            prevKeyState = kState;

        }


        public void Draw(double gameTime, RenderWindow window) {
            //spriteBatch.Draw(backgroundTexture, new Vector2f(0, 0), Color.White);
            mBackdrop.draw(window, new Vector2f(0, 0));
            foreach (GroundEffect g in groundEffects) {
                g.draw(window);
            }
            //objects
            foreach (FightObject ob in fightObjects)
                ob.draw(window, gameTime);
            foreach (Moveable m in moveables) {
                m.Draw(gameTime, window, m == selected);
            }
            foreach (Projectile p in projectiles) {
                p.Draw(gameTime,window);
            }
            
            //UI
            //spriteBatch.Draw(botBlackTexture, new Vector2f(0, 550), Color.White);
            if (selected != null) {
                selectedCoordText.DisplayedString = (currentCursor == targetTexture) + " (" + selected.playerX + "," + selected.playerY + ") D(" + selected.destX + "," + selected.destY + ")";
                window.Draw(selectedCoordText);
            }
            if(selected is PlayerClassI){
                PlayerClassI p = (PlayerClassI)selected;
                //abilityButton1.draw(spriteBatch,p.getFirstAbilityTexture());
                //abilityButton2.draw(spriteBatch, p.getSecondAbilityTexture());
                firstCooldownText.DisplayedString = "" + p.getFirstCooldown().getFormattedTimeLeft(gameTime, COOLDOWN_DECIMAL_PLACES);
                window.Draw(firstCooldownText);
                if (p.getSecondCooldown() != null) {
                    secondCooldownText.DisplayedString = "" + p.getSecondCooldown().getFormattedTimeLeft(gameTime, COOLDOWN_DECIMAL_PLACES);
                    window.Draw(secondCooldownText);
                } 
            } else if (selected is BossI) {
                BossI b = (BossI)selected;
                bossCooldownText.DisplayedString = b.getCooldownString(gameTime,COOLDOWN_DECIMAL_PLACES);
                window.Draw(bossCooldownText);
            }
            if (bossEnraged) {
                window.Draw(enrageText);
            }
            //hpbar lol
            hpBarSprite.Scale = new Vector2f(300/29*(float)mainEnemy.getHPpercent(),50/29.0f);
            window.Draw(hpBarSprite);
            //cursor
            if (currentCursor != null) {
                currentCursorSprite.Texture = currentCursor;
                currentCursorSprite.Position = new Vector2f(mouseState.getPos().X, mouseState.getPos().Y);
                window.Draw(currentCursorSprite);
            }
            //damage meter
            if (fightStatus == FightStatus.fightEngaged) {
                string[] array = damageMeter.getOrderedDamageStrings((float)timer.getTime(gameTime));
                for (int i = 0; i < array.Length; i++) {
                    damagemeterText.DisplayedString = (i + 1) + ". " + array[i];
                    damagemeterText.Position = new Vector2f(600, 10 + i * 15);
                    damagemeterText.Color = DamageMeter.getColor(array[i]);
                    window.Draw(damagemeterText);
                }
            }
            //threat meter
            if (mainEnemy is Enemy) {
                Enemy e = (Enemy)mainEnemy;

                if (e.getFirstThreat() != null) {
                    firstThreatText.DisplayedString = "First: " + e.getFirstThreat().getName() + " " + e.getThreatAmt(e.getFirstThreat());
                    window.Draw(firstThreatText);
                }
                if (e.getSecondThreat() != null) {
                    secondThreatText.DisplayedString = "Second: " + e.getSecondThreat().getName() + " " + e.getThreatAmt(e.getSecondThreat());
                    window.Draw(secondThreatText);
                }
                if (e.getThirdThreat() != null) {
                    thirdThreatText.DisplayedString = "Third: " + e.getThirdThreat().getName() + " " + e.getThreatAmt(e.getThirdThreat());
                    window.Draw(thirdThreatText);
                }
            }
            //fightstatus
            fightStatusText.DisplayedString = fightStatus + " " + (fightStatus == FightStatus.fightEngaged ? timer.getFormattedTime(gameTime, 3) + "" : "");
            window.Draw(fightStatusText);
            if (fightStatus == FightStatus.fightWon || fightStatus == FightStatus.fightLost) {
                countDownText.DisplayedString = (TIMETILPOSTGAME - timer.getFormattedTime(gameTime, 1)) + (fightStatus == FightStatus.fightWon ? " WINWINWIN" : "LOSELOSELOSE") + " " + finalTime;
                window.Draw(countDownText);
            }
            //raidframes
            raidFrames.draw(window);
            //notes
            drawNotes(window, gameTime);
            //warning?
            if (warningFGSprite != null)
                window.Draw(warningFGSprite);
        }
        internal static InGame getInstance() { return inGame; }
        internal void addProjectile(Projectile p) {
            projectiles.Add(p);
        }
        internal void addProjectileFromProj(Projectile p) {
            projectilesToAdd.Add(p);
        }
        internal void removeProjectile(Projectile p) {
            projectilesToRemove.Add(p);
        }
        Sprite warningFGSprite;//should we just use one? EFF
        internal void setWarningFG(Texture texture) {
            warningFGSprite = new Sprite(texture);
            warningFGSprite.Position = new Vector2f(0, 0);
        }
        internal void resetWarningFG() {
            warningFGSprite = null;
        }
        internal List<Moveable> getPlayerMoveables() {
            List<Moveable> l = new List<Moveable>();
            foreach (Moveable m in moveables) {
                if(m is PlayerClassI)
                    l.Add(m);
            }
            return l;
        }
        internal List<Moveable> getAlivePlayerMoveables() {
            List<Moveable> l = new List<Moveable>();
            foreach (Moveable m in moveables) {
                if (m is PlayerClassI && !m.isDead())
                    l.Add(m);
            }
            return l;
        }
        internal List<Enemy> getAliveEnemyMoveables() {
            List<Enemy> l = new List<Enemy>();
            foreach (Moveable m in moveables) {
                if (m is Enemy && !m.isDead())
                    l.Add((Enemy)m);
            }
            return l;
        }
        //-1 for maxRange if it doesnt matter
        internal List<Moveable> getClosestPlayers(Vector2f v,int amt, bool alive, int maxRange, Type ignoreClass){
            double[] lengths = new double[amt];
            for (int i = 0; i < lengths.Length;i++ ) {//populate with "big" number
                lengths[i] = 9999;
            }
            Moveable[] players = new Moveable[amt];
            foreach (Moveable m in moveables) {//go thru every player
                if (m is PlayerClassI) {
                    if (ignoreClass == m.GetType() || (alive && m.isDead())) continue;
                    double length = getDistance(v, m.getMid());
                    if(maxRange != -1 && length > maxRange) continue;//if hes too far, continue
                    for (int i = 0; i < amt;i++ ) {//if not start placing him somewhere
                        if (length < lengths[i]) {//if here
                            //push everything down
                            for (int j = amt - 1; j != i; j-- ) {
                                lengths[j] = lengths[j - 1];
                                players[j] = players[j - 1];
                            }
                            //then place
                            lengths[i] = length;
                            players[i] = m;
                            break;
                        }
                    }
                }
            }
            return players.ToList<Moveable>();
        }
        internal bool areAnyPlayersAlive() {
            bool b = false;
            foreach (Moveable m in moveables) {
                if (m is PlayerClassI && !m.isDead()) {
                    b = true;
                    break;
                }
            }
            return b;
        }
        internal int getPlayerAmt() {
            return playerAmt;
        }
        //gets in circle range?
        internal List<Moveable> getAliveEnemyMoveables(Moveable me, int range) {
            List<Moveable> l = new List<Moveable>();
            foreach (Moveable m in moveables) {
                if (m is Enemy && !m.isDead() && areCircleswithRange(me.getMid(), me.getInnerRadius(),m.getMid(), m.getInnerRadius(), range))
                    l.Add(m);
            }
            return l;
        }

        internal void addGroundEffect(GroundEffect g) {
            groundEffects.Add(g);
        }
        internal void removeGroundEffect(GroundEffect g) {
            groundEffectsToRemove.Add(g);
        }
        internal static bool isInRange(Vector2f v1, Vector2f v2, int range) {
            bool s = (Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2)) <= range);
            return s;
        }
        internal static double getDistance(Vector2f v1, Vector2f v2){
            return (Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2)));
        }
        internal static bool areCirclesIntersecting(Vector2f v1, int r1, Vector2f v2, int r2) {
            float dx = v1.X - v2.X;
            float dy = v1.Y - v2.Y;
            if(Math.Sqrt((dy * dy) + (dx * dx)) > (r1 + r2)) 
                return false;
            return true;
        }
        internal static bool areCircleswithRange(Vector2f v1, int r1, Vector2f v2, int r2, int range) {
            float dx = v1.X - v2.X;
            float dy = v1.Y - v2.Y;
            if (Math.Sqrt((dy * dy) + (dx * dx)) > (r1 +range + r2))
                return false;
            return true;
        }
        internal static bool areCircleswithRange(Moveable m1, Moveable m2, int range) {
            return areCircleswithRange(m1.getMid(), m1.getInnerRadius(), m2.getMid(), m2.getInnerRadius(), range);
        }
        //ya know, it has to do with areCircleswithRange
        internal static double getDistanceBetweenCircles(Vector2f v1, int r1, Vector2f v2, int r2) {
            float dx = v1.X - v2.X;
            float dy = v1.Y - v2.Y;
            double d = Math.Sqrt((dy * dy) + (dx * dx)) - (r1 + r2);
            return d;

        }
        //returns true if you can go there, returns false if you just go close enough
        internal bool tryImoveHere(Moveable guy, Vector2f dest, double gameTime) {
            //check to see if anythings in there, foreach all the moveables
            bool b = true ;
            foreach (Moveable m in moveables) {
                if(m == guy)
                    continue;
                if (areCirclesIntersecting(dest, guy.getInnerRadius(), m.getMid(), m.getInnerRadius()) && !guy.isDead()) {
                    b = false;
                }
            }
            if (b && guy is PlayerClassI) {

                guy.sendPlayerTo(dest, gameTime);
            }
            //
            return b;
        }

        internal void addToDamageMeter(Moveable giver, float amount) {
            if (fightStatus != FightStatus.fightEngaged) return;//has to be during boss fight to add
            if (!(giver is PlayerClassI))
                return;
            damageMeter.addToDamageMeter(giver, amount);
        }
        internal void addMoveable(Moveable m) {
            moveables.Add(m);
        }
        internal void removeGroundEffect(Moveable m) {
            //moveablesToRemove.Add(g);
        }
        internal void startBossFight() {
            if (fightStatus != FightStatus.preFight) return;
            //start timer, and check to see if you killed boss in Update loop
            //if you killed boss, send display a win string on the screen and then start post screen
            timer = new Timer(mGameTime);
            fightStatus = FightStatus.fightEngaged;
            //reset boss cd's
            /*List<Enemy> enemies = getAliveEnemyMoveables();
            foreach (Enemy e in enemies) {
                e.resetCooldowns(mGameTime);
            }*/
            GameBox.getInstance().save();//for fun
            thisRaid = PositionFactory.getRaidSetup(getAlivePlayerMoveables());
            GameBox.getInstance().setLastRaid(thisRaid);
        }
        internal void setAllPlayersTarget(Moveable target, double gameTime) {
            foreach(PlayerClassI p in getAlivePlayerMoveables()){
                p.setTarget(target, gameTime);
            }
        }
        internal FightStatus getFightStatus() {
            return fightStatus;
        }
        internal void setSelected(Moveable player) {
            //unset selection ring 
            if (selected != null)
                selected.setRing(null);

            selected = player;

            //set new selected ring
            if(selected != null)
                selected.setRing(new Color(255,0,255));
            
        }
        internal int getThreatRankOnMainEnemy(PlayerClassI player) {
            return mainEnemy.getThreatRank(player);
        }
        //TODO keep active list of this
        internal List<PlayerClassI> getLowestHPplayers(int amt, Moveable caster, int range) {
            if(amt != 2)
                throw new Exception();
            List<PlayerClassI> l = new List<PlayerClassI>();
            l.Add(null);
            l.Add(null);
            foreach (PlayerClassI mob in getAlivePlayerMoveables()) {
                if(areCircleswithRange(mob.getMid(),mob.getInnerRadius(),caster.getMid(),caster.getInnerRadius(),range)){
                    if ((l[0] == null || l[0].getHPpercent() > mob.getHPpercent())) {
                        l[1] = l[0];
                        l[0] = mob;
                    } else if ((l[1] == null || l[1].getHPpercent() > mob.getHPpercent())) {
                        l[1] = mob;
                    }
                }
            }
            return l;
        }

        string[] notes = new string[2];
        double lastPost;
        internal void addScreenNote(string p) {
            notes[1] = notes[0];//cheap way atm
            notes[0] = p;
            lastPost = GameBox.getInstance().getGameTimeSeconds();
        }
        //TODO EFF terrible right now i think
        private void drawNotes(RenderWindow window, double gameTime) {
            if (notes[0] != null) {
                Text t1 = new Text(notes[0], GameBox.corbalFont, 8U);
                t1.Position = new Vector2f(300, 10);
                window.Draw(t1);
            }
            if (notes[1] != null) {
                Text t2 = new Text(notes[1], GameBox.corbalFont, 8U);
                t2.Position = new Vector2f(300, 30);
                window.Draw(t2);
            }
            if (gameTime/1000 >= lastPost + NOTE_STAY_TIME)
                notes[0] = notes[1] = null;
        }
        private void updateScripts(double gameTime) {
            if (activeScripts == null) return;
            foreach (Script s in activeScripts) {
                s.update(gameTime,timer);
            }
        }
        internal void addScript(Script s) {
            activeScripts.Add(s);
        }
        internal void setMainEnemy(Enemy e) {
            if (!bossFight.getEnemies().Contains(e)) throw new Exception("enemy must be in bossfight");
            mainEnemy = e;
        }

        internal PlayerClassI getRandomAlivePlayer() {
            List<Moveable> mobs = getAlivePlayerMoveables();
            if (mobs.Count == 0) return null;
            int i = InGame.randy.Next(mobs.Count);
            return (PlayerClassI) mobs[i];
        }

    }


    internal enum FightStatus {
        preFight, fightEngaged, fightWon, fightLost
    }
}
