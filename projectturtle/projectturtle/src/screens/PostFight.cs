using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using System.Collections;

using ProjectTurtle.src;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.util;
using ProjectTurtle.src.bossfights;

//PostFight processing happnes in the constructor.
namespace ProjectTurtle.src.screens {
    class PostFight {
        static Texture postScreen, starTexture;

        Button nextButton, restartButton,reformPartyButton;
        Sprite postScreenSprite, winSprite, smallStarSprite1, smallStarSprite2,
                smallStarSprite3, timeStarSprite1, timeStarSprite2, timeStarSprite3;
        Text winStatusText, playerAmtText, smallestAmtText1, smallestAmtText2, enrageText,
                timeSecondsText, timeForStarsText1, timeForStarsText2, timeForStarsText3, dmgMeterText;

        BossFightID id;
        double timeSecond;
        int playerAmt;
        int enrageTimer;
        DamageMeter damageMeter;
        bool win;
        Hashtable lastRaid;

        BossStarInfo fightStars;
        BossFightRecord fightRecord;

        static internal void LoadContent() {
            postScreen = GameBox.loadTexture("images/screens/postFight");
            starTexture = GameBox.loadTexture("images/star");
        }
        internal PostFight(BossFightID id, double timeSecond, int playerAmt, DamageMeter damageMeter, bool win, int enrageTimer, Hashtable lastRaid) {
            nextButton = new Button(new IntRect(571, 526, 195, 61));
            restartButton = new Button(new IntRect(234, 526, 191, 64));
            reformPartyButton = new Button(new IntRect(16, 501, 158, 75));

            this.id = id;
            this.timeSecond = timeSecond;
            this.playerAmt = playerAmt;
            this.damageMeter = damageMeter;
            this.win = win;
            this.enrageTimer = enrageTimer;
            this.lastRaid = lastRaid;

            fightStars = BossSelect.getBossStarInfo(id);
            fightRecord = BossFightLibrary.getBossFightRecord(id);
            //update starInfo from last fight
            if (win) {
                fightStars.winStar = win;
                if (playerAmt < fightStars.smallest) {
                    fightStars.smallest = playerAmt;
                    if(playerAmt < fightRecord.smallAmtForStars[0]){
                        fightStars.smallStars[0] = true;
                        fightStars.smallStars[1] = true;
                        fightStars.smallStars[2] = true;
                    } else if (playerAmt < fightRecord.smallAmtForStars[1]) {
                        fightStars.smallStars[1] = true;
                        fightStars.smallStars[2] = true;
                    } else if (fightRecord.smallAmtForStars.Length > 2 && playerAmt < fightRecord.smallAmtForStars[2]) {
                        fightStars.smallStars[2] = true;
                    }
                }
                if (timeSecond < fightStars.seconds) {
                    fightStars.seconds = timeSecond;
                    if (this.timeSecond < fightRecord.timeForStars[0]) {
                        fightStars.timeStars[0] = true;
                        fightStars.timeStars[1] = true;
                        fightStars.timeStars[2] = true;
                    } else if (timeSecond < fightRecord.timeForStars[1]) {
                        fightStars.timeStars[1] = true;
                        fightStars.timeStars[2] = true;
                    } else if (timeSecond < fightRecord.timeForStars[2]) {
                        fightStars.timeStars[2] = true;
                    }
                }

            }
            //make gfx stuff
            postScreenSprite = new Sprite(postScreen);
            postScreenSprite.Position = new Vector2f(0, 0);
            winSprite = new Sprite();
            winSprite.Position = new Vector2f(303, 43);
            smallStarSprite1 = new Sprite();
            smallStarSprite1.Position = new Vector2f(103, 178);
            smallStarSprite2 = new Sprite();
            smallStarSprite2.Position = new Vector2f(103, 200);
            smallStarSprite3 = new Sprite();
            smallStarSprite3.Position = new Vector2f(103, 222);
            timeStarSprite1 = new Sprite();
            timeStarSprite1.Position = new Vector2f(623, 178);
            timeStarSprite2 = new Sprite();
            timeStarSprite2.Position = new Vector2f(623, 200);
            timeStarSprite3 = new Sprite();
            timeStarSprite3.Position = new Vector2f(623, 222);

            winStatusText = new Text(id + " " + (win ? "Win" : "Lose"),GameBox.corbalFont,15U);
            winStatusText.Position = new Vector2f(338, 43);
            winStatusText.Color = Color.Black;
            playerAmtText = new Text(playerAmt + "", GameBox.corbalFont, 15U);
            playerAmtText.Position = new Vector2f(92, 120);
            playerAmtText.Color = Color.Black;
            smallestAmtText1 = new Text(fightRecord.smallAmtForStars[0] + "", GameBox.corbalFont, 15U);
            smallestAmtText1.Position = new Vector2f(122, 178);
            smallestAmtText1.Color = Color.Black;
            smallestAmtText2 = new Text(fightRecord.smallAmtForStars[1] + "", GameBox.corbalFont, 15U);
            smallestAmtText2.Position = new Vector2f(122, 200);
            smallestAmtText2.Color = Color.Black;
            enrageText = new Text(enrageTimer + "", GameBox.corbalFont, 15U);
            enrageText.Position = new Vector2f(621, 271);
            enrageText.Color = Color.Red;
            timeSecondsText = new Text(timeSecond + "", GameBox.corbalFont, 15U);
            timeSecondsText.Position = new Vector2f(621, 114);
            timeSecondsText.Color = Color.Black;
            timeForStarsText1 = new Text(fightRecord.timeForStars[0] + "", GameBox.corbalFont, 15U);
            timeForStarsText1.Position = new Vector2f(642, 178);
            timeForStarsText1.Color = Color.Black;
            timeForStarsText2 = new Text(fightRecord.timeForStars[1] + "", GameBox.corbalFont, 15U);
            timeForStarsText2.Position = new Vector2f(642, 200);
            timeForStarsText2.Color = Color.Black;
            timeForStarsText3 = new Text(fightRecord.timeForStars[2] + "", GameBox.corbalFont, 15U);
            timeForStarsText3.Position = new Vector2f(642, 222);
            timeForStarsText3.Color = Color.Black;
            dmgMeterText = new Text("dmgmeter", GameBox.corbalFont, 15U);
            //winStatusText, playerAmtText, smallestAmtText1, smallestAmtText2, enrageText,
            //    timeSecondsText, timeForStarsText1, timeForStarsText2, timeForStarsText3, dmgMeterText;

        }
        internal void Update(double gameTime,SimpleMouseState mouseState) {
            if (nextButton.didClick(mouseState)) {
                GameBox.getInstance().gotoBossSelect();
            }
            if (restartButton.didClick(mouseState)) {
                GameBox.getInstance().restartBossFight(BossFightLibrary.getBossFight(id),lastRaid);
            }
            if (reformPartyButton.didClick(mouseState)) {
                GameBox.getInstance().initRaidSelect(id);
            }
        }
        internal void Draw(RenderWindow window) {
            window.Draw(postScreenSprite);
            window.Draw(winStatusText);
            if (fightStars.winStar) {
                window.Draw(winSprite);
            }
            //players
            window.Draw(playerAmtText);
            window.Draw(smallestAmtText1);
            window.Draw(smallestAmtText2);
            //spriteBatch.DrawString(Game.spFont, fightRecord.smallAmtForStars[2] + "", new Vector2f(122, 222), Color.Black);
            if (fightStars.smallStars[0]) window.Draw(smallStarSprite1);
            if (fightStars.smallStars[1]) window.Draw(smallStarSprite2);
            if (fightStars.smallStars[2]) window.Draw(smallStarSprite3);
            window.Draw(enrageText);
            //times
            window.Draw(timeSecondsText);
            window.Draw(timeForStarsText1);
            window.Draw(timeForStarsText2);
            window.Draw(timeForStarsText3);
            if (fightStars.timeStars[0]) window.Draw(timeStarSprite1);
            if (fightStars.timeStars[1]) window.Draw(timeStarSprite2);
            if (fightStars.timeStars[2]) window.Draw(timeStarSprite3);
            
            //damage meter
            string[] array = damageMeter.getOrderedDamageStrings((float)timeSecond);
            for (int i = 0; i < array.Length; i++) {
                //bad im sure?
                dmgMeterText.DisplayedString = (i + 1) + ". " + array[i];
                dmgMeterText.Position = new Vector2f(245, 165 + i * 15);
                dmgMeterText.Color = DamageMeter.getColor(array[i]);
                window.Draw(dmgMeterText);
            }
        }
        //TODO add postions you set up, in game


    }
}
