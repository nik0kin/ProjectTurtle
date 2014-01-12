using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections;

using ProjectTurtle.src.Objects;
using ProjectTurtle.src.UI;
using ProjectTurtle.src.Effects;
using ProjectTurtle.src.Stuff;
using ProjectTurtle.src.Util;
using ProjectTurtle.src.BossFights;


namespace ProjectTurtle.src.Screens {
    class RaidSelect {
        const int MAX_RAID_SIZE = 20;
        static Texture2D bg;

        PlayerClassNum[] raid;

        Button backButton, startButton;
        Button vangButton, puriButton, mageButton,
                assButton, archerButton, bardButton;
        Button hero1, hero2, hero3, hero4;

        Button[] raidButtons;

        BossFight bossFight;
        int limit, currentAmt;
        string fightName;

        UnlockedHeroes mUnlockedHeroes;

        internal static void LoadContent(ContentManager contentManager) {
            bg = contentManager.Load<Texture2D>("images/screens/raidSelect");
        }
        internal RaidSelect(BossFight bossFight,UnlockedHeroes unlockedHeroes) {
            this.bossFight = bossFight;
            fightName = BossFightLibrary.getBossFightName(bossFight.getFightID());
            limit = bossFight.getBossInfo().maxplayers;//getMaxPlayers();

            raid = new PlayerClassNum[MAX_RAID_SIZE];
            for (int i = limit; i < MAX_RAID_SIZE; i++) {
                raid[i] = PlayerClassNum.cant;
            }

            backButton = new Button(new Rectangle(49, 502, 153, 80));
            startButton = new Button(new Rectangle(559, 517, 177, 68));

            vangButton = new Button(new Rectangle(10,5,63,54));
            puriButton = new Button(new Rectangle(85, 5, 63, 54));
            mageButton = new Button(new Rectangle(10, 71, 63, 54));
            assButton = new Button(new Rectangle(85, 71, 63, 54));
            archerButton = new Button(new Rectangle(10, 71 + (71 - 5), 63, 54));
            bardButton = new Button(new Rectangle(85, 71 + (71 - 5), 63, 54));

            hero1 = new Button(new Rectangle(10,245,63,54));
            hero2 = new Button(new Rectangle(85, 245, 63, 54));
            hero3 = new Button(new Rectangle(10, 312, 63, 54));
            hero4 = new Button(new Rectangle(85, 312, 63, 54));

            raidButtons = new Button[MAX_RAID_SIZE];
            raidButtons[0] = new Button(new Rectangle(443, 19, 63, 54));
            raidButtons[1] = new Button(new Rectangle(443, 82, 63, 54));
            raidButtons[2] = new Button(new Rectangle(443, 150, 63, 54));
            raidButtons[3] = new Button(new Rectangle(443, 214, 63, 54));
            raidButtons[4] = new Button(new Rectangle(443, 281, 63, 54));
            
            raidButtons[5] = new Button(new Rectangle(519, 19, 63, 54));
            raidButtons[6] = new Button(new Rectangle(519, 82, 63, 54));
            raidButtons[7] = new Button(new Rectangle(519, 150, 63, 54));
            raidButtons[8] = new Button(new Rectangle(519, 214, 63, 54));
            raidButtons[9] = new Button(new Rectangle(519, 281, 63, 54));

            raidButtons[10] = new Button(new Rectangle(595, 19, 63, 54));
            raidButtons[11] = new Button(new Rectangle(595, 82, 63, 54));
            raidButtons[12] = new Button(new Rectangle(595, 150, 63, 54));
            raidButtons[13] = new Button(new Rectangle(595, 214, 63, 54));
            raidButtons[14] = new Button(new Rectangle(595, 281, 63, 54));

            raidButtons[15] = new Button(new Rectangle(671, 19, 63, 54));
            raidButtons[16] = new Button(new Rectangle(671, 82, 63, 54));
            raidButtons[17] = new Button(new Rectangle(671, 150, 63, 54));
            raidButtons[18] = new Button(new Rectangle(671, 214, 63, 54));
            raidButtons[19] = new Button(new Rectangle(671, 281, 63, 54));

            mUnlockedHeroes = unlockedHeroes;
        }
        internal void Update(GameTime gameTime, MouseState mouseState, MouseState prevMouseState) {
            if (vangButton.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released) 
                addClassToRaid(PlayerClassNum.vang);
            else if (puriButton.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released) 
                addClassToRaid(PlayerClassNum.puri);
            else if (mageButton.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released) 
                addClassToRaid(PlayerClassNum.mage);
            else if (assButton.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released) 
                addClassToRaid(PlayerClassNum.assa);
            else if (archerButton.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released)
                addClassToRaid(PlayerClassNum.archer);
            else if (bardButton.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released)
                addClassToRaid(PlayerClassNum.bard);

            else if (mUnlockedHeroes.isUnlockedClass(1) && hero1.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released)
                addClassToRaid(mUnlockedHeroes.getUnlockedClass(1));
            else if (mUnlockedHeroes.isUnlockedClass(2) && hero2.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released)
                addClassToRaid(mUnlockedHeroes.getUnlockedClass(2));
            else if (mUnlockedHeroes.isUnlockedClass(3) && hero3.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released)
                addClassToRaid(mUnlockedHeroes.getUnlockedClass(3));
            else if (mUnlockedHeroes.isUnlockedClass(4) && hero4.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released)
                addClassToRaid(mUnlockedHeroes.getUnlockedClass(4));

            int i = 0;
            foreach (Button rButton in raidButtons) {
                if (rButton.didClick(mouseState) && raid[i] != PlayerClassNum.none) {//remove that class if you click
                    raid[i] = PlayerClassNum.none;
                    currentAmt--;
                }
                i++;
            }

            if (backButton.didClick(mouseState) && prevMouseState.LeftButton == ButtonState.Released) {
                GameBox.getInstance().gotoBossSelect();
            }
            if (startButton.didClick(mouseState)) {
                if (raid[0] != PlayerClassNum.none && raid[0] != PlayerClassNum.cant) {
                    PlayerClassNum added = bossFight.getBossInfo().addedLts[0];//TODO erero pronme
                    if (added != PlayerClassNum.none && added != PlayerClassNum.cant)
                        raid[limit] = added;
                    GameBox.getInstance().startBossFight(bossFight, raid.ToArray());
                }
            }
            prevMouseState = mouseState;
        }

        internal void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(bg, Vector2.Zero, Color.White);

            spriteBatch.DrawString(GameBox.spFont, fightName, new Vector2(322, 520), Color.Black);

            spriteBatch.DrawString(GameBox.spFont, raid[0]+"", new Vector2(450, 40), PlayerClassI.getClassColor(raid[0]));
            spriteBatch.DrawString(GameBox.spFont, raid[1] + "", new Vector2(450, 100), PlayerClassI.getClassColor(raid[1]));
            spriteBatch.DrawString(GameBox.spFont, raid[2] + "", new Vector2(450, 170), PlayerClassI.getClassColor(raid[2]));
            spriteBatch.DrawString(GameBox.spFont, raid[3] + "", new Vector2(450, 230), PlayerClassI.getClassColor(raid[3]));
            spriteBatch.DrawString(GameBox.spFont, raid[4] + "", new Vector2(450, 300), PlayerClassI.getClassColor(raid[4]));

            spriteBatch.DrawString(GameBox.spFont, raid[5] + "", new Vector2(525, 40), PlayerClassI.getClassColor(raid[5]));
            spriteBatch.DrawString(GameBox.spFont, raid[6] + "", new Vector2(525, 100), PlayerClassI.getClassColor(raid[6]));
            spriteBatch.DrawString(GameBox.spFont, raid[7] + "", new Vector2(525, 170), PlayerClassI.getClassColor(raid[7]));
            spriteBatch.DrawString(GameBox.spFont, raid[8] + "", new Vector2(525, 230), PlayerClassI.getClassColor(raid[8]));
            spriteBatch.DrawString(GameBox.spFont, raid[9] + "", new Vector2(525, 300), PlayerClassI.getClassColor(raid[9]));

            spriteBatch.DrawString(GameBox.spFont, raid[10] + "", new Vector2(600, 40), PlayerClassI.getClassColor(raid[10]));
            spriteBatch.DrawString(GameBox.spFont, raid[11] + "", new Vector2(600, 100), PlayerClassI.getClassColor(raid[11]));
            spriteBatch.DrawString(GameBox.spFont, raid[12] + "", new Vector2(600, 170), PlayerClassI.getClassColor(raid[12]));
            spriteBatch.DrawString(GameBox.spFont, raid[13] + "", new Vector2(600, 230), PlayerClassI.getClassColor(raid[13]));
            spriteBatch.DrawString(GameBox.spFont, raid[14] + "", new Vector2(600, 300), PlayerClassI.getClassColor(raid[14]));

            spriteBatch.DrawString(GameBox.spFont, raid[15] + "", new Vector2(675, 40), PlayerClassI.getClassColor(raid[15]));
            spriteBatch.DrawString(GameBox.spFont, raid[16] + "", new Vector2(675, 100), PlayerClassI.getClassColor(raid[16]));
            spriteBatch.DrawString(GameBox.spFont, raid[17] + "", new Vector2(675, 170), PlayerClassI.getClassColor(raid[17]));
            spriteBatch.DrawString(GameBox.spFont, raid[18] + "", new Vector2(675, 230), PlayerClassI.getClassColor(raid[18]));
            spriteBatch.DrawString(GameBox.spFont, raid[19] + "", new Vector2(675, 300), PlayerClassI.getClassColor(raid[19]));

            //draw unlocked heroes
            spriteBatch.DrawString(GameBox.smallArial, mUnlockedHeroes.getUnlockedClass(1) + "", new Vector2(17, 265), Color.Gold);
            spriteBatch.DrawString(GameBox.smallArial, mUnlockedHeroes.getUnlockedClass(2) + "", new Vector2(91, 265), Color.Gold);
            spriteBatch.DrawString(GameBox.smallArial, mUnlockedHeroes.getUnlockedClass(3) + "", new Vector2(17, 331), Color.Gold);
            spriteBatch.DrawString(GameBox.smallArial, mUnlockedHeroes.getUnlockedClass(4) + "", new Vector2(91, 331), Color.Gold);
            

        }
        private void addClassToRaid(PlayerClassNum id) {
            if (currentAmt >= limit) return;

            for (int i = 0; i < raid.Length;i++ ) {
                if (raid[i] == PlayerClassNum.none) {
                    raid[i] = id;
                    currentAmt++;
                    break;
                }
            }

        }
    }
}
