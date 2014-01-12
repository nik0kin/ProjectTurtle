using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using System.Collections;

using ProjectTurtle.src.objects;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.util;
using ProjectTurtle.src.bossfights;

namespace ProjectTurtle.src.screens {
    class BossSelect {
        static Texture bgTexture;
        static List<BossFightID> bossFights;
        static Hashtable stars;    //key = fightnum, value = BossStarInfo

        static int totalStars;
        BossFightID selectedFight = BossFightID.none;
        BossStarInfo selectedStars;

        Button makeGroupButton;
        BossListButton[] bossListButtons;
        
        internal static void LoadContent(){
            bgTexture = GameBox.loadTexture("images/screens/bossSelect");
            stars = new Hashtable();
            bossFights = new List<BossFightID>();
            bossFights.Add(BossFightID.buseyBoss);
            bossFights.Add(BossFightID.tomHanks);
            bossFights.Add(BossFightID.dpsCheck);
            bossFights.Add(BossFightID.fraiser);
            //TODO would load star stuff from file
            stars.Add(BossFightID.buseyBoss, new BossStarInfo());
            stars.Add(BossFightID.tomHanks, new BossStarInfo());
            stars.Add(BossFightID.dpsCheck, new BossStarInfo());
            stars.Add(BossFightID.fraiser, new BossStarInfo());
            stars.Add(BossFightID.urisdor, new BossStarInfo());
        }
        internal BossSelect() {
            makeGroupButton = new Button(new IntRect(580,428,132, 59));
            bossListButtons = new BossListButton[4];
            bossListButtons[0] = new BossListButton(new IntRect(15, 67, 277, 40), BossFightID.buseyBoss);
            bossListButtons[1] = new BossListButton(new IntRect(13, 109, 276, 40), BossFightID.tomHanks);
            bossListButtons[2] = new BossListButton(new IntRect(13, 149, 276, 40), BossFightID.dpsCheck);
            bossListButtons[3] = new BossListButton(new IntRect(13, 189, 276, 40), BossFightID.fraiser);
        }
        internal void Update(double gameTime, SimpleMouseState mouseState){
            if(selectedFight != BossFightID.none)
                if (mouseState.left && makeGroupButton.didHit(mouseState.getPos()) ) {
                    GameBox.getInstance().initRaidSelect(selectedFight);
                }
            if (mouseState.left) {
                foreach (BossListButton b in bossListButtons) {
                    if (b.didHit(mouseState.getPos())) {
                        selectedFight = b.getBossFight();
                        selectedStars = (BossStarInfo)stars[selectedFight];
                    }
                }
            }
        }
        internal void Draw(RenderWindow window) {
/*            spriteBatch.Draw(bgTexture, new Vector2f(0,0), Color.White);

            spriteBatch.DrawString(GameBox.corbalFont, totalStars + "", new Vector2f(157, 507), Color.Black);
            spriteBatch.DrawString(GameBox.corbalFont, BossFightLibrary.getBossFightName(bossListButtons[0].getBossFight()), new Vector2f(57, 77), Color.Black);
            spriteBatch.DrawString(GameBox.corbalFont, BossFightLibrary.getBossFightName(bossListButtons[1].getBossFight()), new Vector2f(66, 119), Color.Black);
            spriteBatch.DrawString(GameBox.corbalFont, BossFightLibrary.getBossFightName(bossListButtons[2].getBossFight()), new Vector2f(66, 159), Color.Black);
            spriteBatch.DrawString(GameBox.corbalFont, BossFightLibrary.getBossFightName(bossListButtons[3].getBossFight()), new Vector2f(66, 199), Color.Black);
            //selected
            if (selectedFight != BossFightID.none) {
                spriteBatch.DrawString(GameBox.corbalFont, selectedFight + " "+selectedStars.winStar, new Vector2f(440, 46), Color.Black);
                spriteBatch.DrawString(GameBox.corbalFont, getTimeString(selectedFight), new Vector2f(440, 72), Color.Black);
                spriteBatch.DrawString(GameBox.corbalFont, getSmallestString(selectedFight), new Vector2f(463, 98), Color.Black);

                spriteBatch.DrawString(GameBox.corbalFont, BossFightLibrary.getBossFightDesc(selectedFight), new Vector2f(350, 128), Color.Black);
            }
 */
        }

        internal static void updateTotalStars() {
            int t = 0;
            foreach (BossFightID b in bossFights) {
                BossStarInfo bsi = (BossStarInfo)stars[b];
                t += bsi.getStarCount();
            }


            totalStars = t;
        }
        internal static BossStarInfo getBossStarInfo(BossFightID id){
            return (BossStarInfo)stars[id];
        }
        private string getTimeString(BossFightID n) {
            BossFightRecord r = BossFightLibrary.getBossFightRecord(n);
            BossStarInfo i = (BossStarInfo)stars[n];
            if (r.timeForStars == null) return "nullers";
            string s = i.seconds + " G:" + r.timeForStars[0] + " S:" + r.timeForStars[1] + " B:" + r.timeForStars[2];

            return s;
        }
        private string getSmallestString(BossFightID n) {
            BossFightRecord r = BossFightLibrary.getBossFightRecord(n);
            BossStarInfo i = (BossStarInfo)stars[n];
            if (r.smallAmtForStars == null) return "nullers";

            string s = i.smallest + " G:" + r.smallAmtForStars[0] + " S:" + r.smallAmtForStars[1] + " B:" + r.smallAmtForStars[2];

            return s;
        }
    }
}
