using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.objects;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.util;
using ProjectTurtle.src.bossfights;


namespace ProjectTurtle.src.screens {
    class RaidSelect2 {
        internal const string IMAGEPATH = "images/menus/raidselect/";
        const int BUTTONS_Y = 525;
        const int BACK_BUTTON_X = 475;
        const int CLEAR_BUTTON_X = 525;
        const int START_BUTTON_X = 575;

        static Texture startButtonTexture;
        static Texture backButtonTexture;
        static Texture clearButtonTexture;

        static Texture barracksListTexture;
        static Texture bossArt;
        static Texture locationmarkerTexture;
        static Texture raidpartybgTexture;



        //static Texture heroBarTestTexture;

		BossFightID boss;
        Button clearButton, startButton, backButton;
        BarracksScrollList barracksScrollList;
		PillarGroup pillarGroup;
        ChampItemPair? draggedChamp;
        SimpleMouseState mouseState;

        Text bossString, worldString, barracksString, currentPlayersString, draggedString;
        Sprite bossArtSprite, locationSprite, raidpartybgSprite, barracksListSprite;

        short currentPlayerAmt = 0,maxPlayers;
		
        internal static void LoadContent() {
            startButtonTexture = GameBox.loadTexture(IMAGEPATH + "playbutton");
            backButtonTexture = GameBox.loadTexture(IMAGEPATH + "backbutton");
            clearButtonTexture = GameBox.loadTexture(IMAGEPATH + "backbutton");

            barracksListTexture = GameBox.loadTexture(IMAGEPATH + "barrackspane");
            bossArt = GameBox.loadTexture(IMAGEPATH + "bosspane");
            locationmarkerTexture = GameBox.loadTexture(IMAGEPATH + "mappane");
            raidpartybgTexture = GameBox.loadTexture(IMAGEPATH + "partypane");

            //heroBarTestTexture = GameBox.loadTexture(IMAGEPATH + "barracksunit");

        }

        internal RaidSelect2(BossFightID boss, Barracks barracks) {
            backButton = new Button(new IntRect(BACK_BUTTON_X, BUTTONS_Y, 50, 50),backButtonTexture);
            clearButton = new Button(new IntRect(CLEAR_BUTTON_X, BUTTONS_Y, 50, 50), clearButtonTexture);
            startButton = new Button(new IntRect(START_BUTTON_X, BUTTONS_Y, 200, 50), startButtonTexture);

			this.boss = boss;
            maxPlayers = (short)BossFightLibrary.getBossInfo(boss).maxplayers;
            barracksScrollList = new BarracksScrollList(barracks,new Vector2f(0, 210));
            pillarGroup = new PillarGroup(new FloatRect(200, 180,600,420));

            bossString = new Text(BossFightLibrary.getBossInfo(boss).name.ToUpper(), GameBox.pixelzimFont, 30U);
            bossString.Color = Color.White;
            bossString.Position = new Vector2f(265, 130);

            worldString = new Text("FOREST", GameBox.pixelzimFont, 30U);
            worldString.Color = Color.White;
            worldString.Position = new Vector2f(685, 130);

            barracksString = new Text("BARRACKS", GameBox.pixelzimFont, 30U);
            barracksString.Color = Color.White;
            barracksString.Position = new Vector2f(11, 175);

            currentPlayersString = new Text(currentPlayerAmt + "/" + maxPlayers, GameBox.corbalFont, 30U);
            currentPlayersString.Color = new Color(140,240,70);
            currentPlayersString.Position = new Vector2f(550, 400);

            draggedString = new Text("", GameBox.corbalFont, 30U);
            draggedString.Color = Color.White;
            

            //bossArtSprite, locationSprite, raidpartybgSprite, barracksListSprite;
            bossArtSprite = new Sprite(bossArt);
            bossArtSprite.Position = new Vector2f(0, 0);
            locationSprite = new Sprite(locationmarkerTexture);
            locationSprite.Position = new Vector2f(400, 0);
            raidpartybgSprite = new Sprite(raidpartybgTexture);
            raidpartybgSprite.Position = new Vector2f(200, 180);
            barracksListSprite = new Sprite(barracksListTexture);
            barracksListSprite.Position = new Vector2f(0, 180);
        }
        internal void Update(double gameTime, SimpleMouseState mouseState) {
            this.mouseState = mouseState;
            if (backButton.didClick(mouseState)) {
                GameBox.getInstance().gotoBossSelect();
            }
            if (clearButton.didClick(mouseState)) {
                //clear raid button
				resetRaid();
            }
            if (startButton.didClick(mouseState)) {
                //would start fight
				startFight();
            }
            barracksScrollList.update(gameTime, mouseState);
			
			ChampItemPair? newDraggedChamp = barracksScrollList.checkHeroDrag();
            if (draggedChamp != null && currentPlayerAmt < maxPlayers)
            {
                //we want released over anything, because if they dont release it specifically over it..
                if(newDraggedChamp == null){
                //if (mouseState.getPos().left == ButtonState.Released /*mouseState.DoneReleasedIt(mouseButton.left)*/){
                    if(pillarGroup.hoverAndDrop(new Vector2f(mouseState.getPos().X, mouseState.getPos().Y), mouseState, (ChampItemPair)draggedChamp)) {
                        barracksScrollList.setDisabledChamp((ChampItemPair)draggedChamp,true);
                        //run added champ code here?
                        draggedChamp = null;
                        currentPlayerAmt++;
                    }else{
                        draggedChamp = null;
                    }
				}
			}
            draggedChamp = newDraggedChamp;
            
        }
        internal void Draw(RenderWindow window) {
            window.Clear(Color.Magenta);
            //bossart
            window.Draw(bossArtSprite);
            window.Draw(bossString);
            //location marker
            window.Draw(locationSprite);
            window.Draw(worldString);
            //raid party
            window.Draw(raidpartybgSprite);
            pillarGroup.draw(window);
            //barracks
            window.Draw(barracksListSprite);
            window.Draw(barracksString);
            barracksScrollList.draw(window);
            //buttons
            backButton.draw(window);
            clearButton.draw(window);
            startButton.draw(window);
            currentPlayersString.DisplayedString = currentPlayerAmt + "/" + maxPlayers;
            window.Draw(currentPlayersString);


            if (draggedChamp != null) {
                draggedString.Position = new Vector2f(mouseState.getPos().X, mouseState.getPos().Y);
                draggedString.DisplayedString = draggedChamp.Value.hero + " " + draggedChamp.Value.item;
                window.Draw(draggedString);
            }
        }
		internal void resetRaid(){
            currentPlayerAmt = 0;
			pillarGroup.reset();
            barracksScrollList.clear();
		}
		internal void startFight(){
            List<List<ChampItemPair>> raid = pillarGroup.getRaidGroups();
            int num = 0;
            foreach (List<ChampItemPair> g in raid)
                foreach (ChampItemPair c in g)
                    num++;
            if (num == 0) return;
            GameBox.getInstance().startBossFight(BossFightLibrary.getBossFight(boss), raid);
		}
    }
}
