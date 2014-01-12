using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.objects;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.screens;
using ProjectTurtle.src.bossfights;
using ProjectTurtle.src.objects.playerclasses;
using ProjectTurtle.src.objects.playerclasses.heroes;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.util;

#pragma warning disable 0162

namespace ProjectTurtle.src.screens {
    public enum GameState {
        menuScreen, bossSelect, raidSelect, inGame, postFight, settingsMenu
    }
    public class GameBox {
        internal const int GAMERESX = 800;
        internal const int GAMERESY = 600;
        const bool ISFULLSCREEN = false;

        const bool ALLUNLOCKS = true;
        
        static readonly PlayerClassNum[] ten_bard_compare = new PlayerClassNum[10] {
                PlayerClassNum.vang,PlayerClassNum.bard,PlayerClassNum.mage,PlayerClassNum.archer,PlayerClassNum.assa,
                PlayerClassNum.mage, PlayerClassNum.assa,PlayerClassNum.archer, PlayerClassNum.bard, PlayerClassNum.puri};
        static readonly PlayerClassNum[] twenty = new PlayerClassNum[20] {
                PlayerClassNum.vang,PlayerClassNum.vang,PlayerClassNum.puri,PlayerClassNum.puri,PlayerClassNum.puri,
                PlayerClassNum.archer, PlayerClassNum.archer,PlayerClassNum.archer, PlayerClassNum.bard, PlayerClassNum.bard,
                PlayerClassNum.assa, PlayerClassNum.assa,PlayerClassNum.assa, PlayerClassNum.bard, PlayerClassNum.bard,
                PlayerClassNum.mage, PlayerClassNum.mage,PlayerClassNum.mage, PlayerClassNum.bard, PlayerClassNum.bard };
        static readonly PlayerClassNum[] DPSCHECK_RAIDCOMP = new PlayerClassNum[23] {
                PlayerClassNum.vang,PlayerClassNum.vang,PlayerClassNum.vang,PlayerClassNum.vang,PlayerClassNum.bard,
                PlayerClassNum.archer, PlayerClassNum.archer,PlayerClassNum.archer, PlayerClassNum.archer, PlayerClassNum.bard,
                PlayerClassNum.assa, PlayerClassNum.assa,PlayerClassNum.assa, PlayerClassNum.assa, PlayerClassNum.bard,
                PlayerClassNum.mage, PlayerClassNum.mage,PlayerClassNum.mage, PlayerClassNum.mage, PlayerClassNum.bard,
                PlayerClassNum.bard, PlayerClassNum.bard, PlayerClassNum.bard};
        static readonly PlayerClassNum[] FRAIZER_TEST = new PlayerClassNum[10] {
                PlayerClassNum.vang,PlayerClassNum.vang,PlayerClassNum.mage,PlayerClassNum.archer,PlayerClassNum.assa,
                PlayerClassNum.mage, PlayerClassNum.assa,PlayerClassNum.bard, PlayerClassNum.goodfraiser, PlayerClassNum.puri};
        //INSTANTS
        static readonly PlayerClassNum[] INSTANT_RAIDCOMP = FRAIZER_TEST;//= new PlayerClassNum[4] { PlayerClassNum.archer, PlayerClassNum.bard, PlayerClassNum.puri, PlayerClassNum.vang };
        const BossFightID INSTANT_BOSSID = BossFightID.fraiser;
        
        internal static GameBox game;
        internal static Font pixelzimFont;
        internal static Font corbalFont;

        bool quit;

        double gameTime;
        double prevGameTime;
        
        //screens
        BossSelect bossSelect;
        MenuScreen menuScreen;
        RaidSelect2 raidSelect;
        InGame inGame;
        PostFight postFight;
        SettingsMenu settingsMenu;

        BossFightID lastBossID = BossFightID.none;
        PlayerClassNum[] lastRaidComp;
        List<List<ChampItemPair>> lastRaidCompN;
        Hashtable lastRaid;
        UnlockedHeroes unlockedHeroes;

        Barracks currentBarracks;
        SimpleMouseState mouseState;
        
        GameState gameState;
        RenderWindow window;


        static void OnClose(object sender, EventArgs e) {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
        public void run(){
            GameState prevGameState = gameState;
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
		    while (window.IsOpen() && !quit) {
                double totaltime = stopwatch.ElapsedMilliseconds;
                //Console.WriteLine(totaltime);
			    window.DispatchEvents();
                
                Update(totaltime);
                if (gameState != prevGameState) {
                    prevGameState = gameState;
                    continue;//to skip weird updates with prev moustateates?
                }
			    window.Clear(new Color(255, 255, 255));//white
                Draw(totaltime);

			    window.Display();
                prevGameState = gameState;
                prevGameTime = totaltime;
		    }
		    save();
    		
        }


        public GameBox() {
            window = new RenderWindow(new VideoMode(GAMERESX, GAMERESY, 32), "Turtle");
            window.Closed += new EventHandler(OnClose);
            //graphics = null;

//            if (ISFULLSCREEN) graphics.IsFullScreen = true;
            
            game = this;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void loadContent() {
            pixelzimFont = new Font("res/fonts/Pixelzim 3x5.ttf");
            corbalFont = new Font("res/fonts/Corbel.ttf");

            Moveable.LoadContent();
            GroundEffect.LoadContent();
            //enemys
            BasicAdd.LoadContent();
            BuseyBoss.LoadContent();
            TomHanks.LoadContent();
            CheckDPSBoss.LoadContent();
            EvilFraiserFight.LoadContent();
            UrsidorBossFight.LoadContent();
            //playerclasses
            Mage.LoadContent();
            Puri.LoadContent();
            Vanguard.LoadContent();
            Assassin.LoadContent();
            Archer.LoadContent();
            Bard.LoadContent();
            //heros
            GoodFraiser.LoadContent();
            //screens
            BossSelect.LoadContent();
            PostFight.LoadContent();
            //RaidSelect.LoadContent();
            RaidSelect2.LoadContent();
            MenuScreen.LoadContent();
            SettingsMenu.LoadContent();
            //other ui
            PlayerFrame.LoadContent();
            BarracksScrollList.LoadContent();
            IconToolTip.loadContent();
            Pillar.LoadContent();

            Item.LoadContent();
            SimpleModel.LoadContent();
            //props
            forest.LoadContent();
            
            menuScreen = new MenuScreen();
            settingsMenu = new SettingsMenu();
            bossSelect = new BossSelect();
            gameState = GameState.menuScreen;

            if (ALLUNLOCKS)
                unlockedHeroes = new UnlockedHeroes(true);
            else
                unlockedHeroes = new UnlockedHeroes(false);
            //mainmenu
            //play = new RollOverStringButton(

            currentBarracks = Barracks.load();

            mouseState = new SimpleMouseState(window);
        }
        internal void save() {
            Barracks.save(currentBarracks);
        }
        internal void Exit() {
            Barracks.save(currentBarracks);
            quit = true;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void Update(double gameTime) {
            mouseState.update(window);
//            KeyboardState keyState = Keyboard.GetState();
            SimpleKeyboardState keyState = new SimpleKeyboardState(true);
            this.gameTime = gameTime;
            switch (gameState) {
            case GameState.menuScreen:
                menuScreen.Update(gameTime, mouseState);
                break;
            case GameState.bossSelect:
//                bossSelect.Update(gameTime, mouseState.getPos());
                break;
            case GameState.raidSelect:
                raidSelect.Update(gameTime, mouseState);
                break;
            case GameState.inGame:
                inGame.Update(gameTime, mouseState, keyState);
                break;
            case GameState.settingsMenu:
                settingsMenu.Update(gameTime, mouseState);
                break;
            /*case GameState.XXXXX:
                break;*/
            case GameState.postFight:
                postFight.Update(gameTime, mouseState);
                break;
            }
/*            if (keyState.IsKeyDown(Keyboard.Key.NumPad1)) {
                startBossFight(BossFightLibrary.getBossFight(INSTANT_BOSSID), INSTANT_RAIDCOMP);
                BossFightLibrary.getBossFight(ProjectTurtle.src.bossfights.BossFightID.buseyBoss);
            } else if (keyState.IsKeyDown(Keyboard.Key.NumPad2)) {
                startBossFight(BossFightLibrary.getBossFight(BossFightID.dpsCheck), DPSCHECK_RAIDCOMP);
            }
*/
            mouseState.wheelUpdate();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void Draw(double gameTime) {
            switch (gameState) {
            case GameState.menuScreen:
                menuScreen.Draw(window);
                break;
            case GameState.bossSelect:
//                bossSelect.Draw(spriteBatch);
                break;
            case GameState.raidSelect:
                raidSelect.Draw(window);
                break;
            case GameState.inGame:
                inGame.Draw(gameTime, window);
                break;
            case GameState.postFight:
                postFight.Draw(window);
                break;
            case GameState.settingsMenu:
                settingsMenu.Draw(window);
                break;

            }

        }
        internal static GameBox getInstance() { return game; }

        internal void initRaidSelect(BossFightID id) {
            gameState = GameState.raidSelect;
            raidSelect = new RaidSelect2(id, currentBarracks);
        }
        //old version!!
        internal void startBossFight(BossFight bossFight, PlayerClassNum[] raidComp) {
            inGame = new InGame();
            BossFightID id = bossFight.getFightID();
            if (lastRaid != null && id == lastBossID) {
                inGame.LoadContent(bossFight, raidComp, lastRaid);
            } else {
                inGame.LoadContent(bossFight, raidComp);
            }
            gameState = GameState.inGame;
            lastBossID = id;
            lastRaidComp = raidComp;
        }
        //new version!
        internal void startBossFight(BossFight bossFight, List<List<ChampItemPair>> raidComp) {
            inGame = new InGame();
            BossFightID id = bossFight.getFightID();
            if (lastRaid != null && id == lastBossID) {
                inGame.LoadContent(bossFight, raidComp, lastRaid);
            } else {
                inGame.LoadContent(bossFight, raidComp);
            }
            gameState = GameState.inGame;
            lastBossID = id;
            lastRaidCompN = raidComp;
        }

        internal void restartBossFight(BossFight bossFight, Hashtable raid) {
            inGame = new InGame();
            inGame.LoadContent(bossFight, lastRaidCompN, raid);
            gameState = GameState.inGame;

        }
        internal void doLastBossFight() {
            startBossFight(BossFightLibrary.getBossFight(lastBossID), lastRaidCompN);
        }
        internal void initPostFight(BossFightID id, double timeSecond, int playerAmt, DamageMeter damageMeter, bool win, int enrageTimer, Hashtable lastRaid) {
            inGame = null;
            //this.lastRaid = lastRaid;
            gameState = GameState.postFight;
            postFight = new PostFight(id, timeSecond, playerAmt, damageMeter, win, enrageTimer, lastRaid);
            BossSelect.updateTotalStars();
        }
        internal void gotoBossSelect() {
            postFight = null;
            //gameState = GameState.bossSelect;
            gameState = GameState.menuScreen;
        }
        internal double getGameTime() {
            return gameTime;
        }
        internal double getGameTimeSeconds() {
            return gameTime / 1000;
        }
        internal void setLastRaid(Hashtable lastRaid) {
            this.lastRaid = lastRaid;
        }
        internal void setScreen(GameState gameState) {
            this.gameState = gameState;
        }
         
        public static Texture loadTexture(String str){
		    Texture text = new Texture("res/" + str + ".png");

            Console.WriteLine("loaded res/" + str + ".png");
		    return text;
	    }

        internal double getLastUpdateTime() {
            return prevGameTime;
        }
    }

}