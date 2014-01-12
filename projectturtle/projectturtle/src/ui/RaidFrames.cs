using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.objects;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.ui {
    class RaidFrames {
        const int FRAME_SPACING = 3;
        const int MAX_COLM = 5;
        PlayerFrame[] playerFrames;
        InGame parent;

		//
        internal RaidFrames(InGame inGame, List<PlayerClassI> raid, Vector2f v) {
            playerFrames = new PlayerFrame[raid.Count];

            int i = 0;
            foreach (PlayerClassI p in raid) {
                playerFrames[i] = new PlayerFrame(this,p, new Vector2f(v.X + (i / MAX_COLM * (FRAME_SPACING + PlayerFrame.FRAMESIZE)), v.Y + (i % MAX_COLM * (FRAME_SPACING + PlayerFrame.FRAMESIZE))));
                i++;
            }
            parent = inGame;
        }
		internal void draw(RenderWindow window) {
            foreach (PlayerFrame pf in playerFrames) {
                pf.draw(window);
            }
        }
        internal void checkClicks(SimpleMouseState mouseState) {
            foreach (PlayerFrame pf in playerFrames) {
                pf.didClick(mouseState);
            }
        }
        internal InGame getInGame() {
            return parent;
        }
    }
    class PlayerFrame : Button{
        internal const int FRAMESIZE = 35;
        const int INSIDE_SPACER = 3;
        const int INSIDESIZE = 35 - INSIDE_SPACER * 2;
        static Texture playerFrame, insideFrame;

        RaidFrames parent;
        PlayerClassI player;

        Sprite playerFrameSprite, insideFrameSprite;
        Text hpPercentText, threatText;

        internal static void LoadContent() {
            playerFrame = GameBox.loadTexture("images/UI/playerFrame");
            insideFrame = GameBox.loadTexture("images/UI/insideFrame");
        }

        internal PlayerFrame(RaidFrames raidFrames, PlayerClassI m, Vector2f spot)
            : base(new IntRect((int)spot.X, (int)spot.Y, FRAMESIZE, FRAMESIZE)) {
            player = m;
            parent = raidFrames;

            playerFrameSprite = new Sprite(playerFrame);
            playerFrameSprite.Scale = new Vector2f(FRAMESIZE / playerFrame.Size.X, FRAMESIZE / playerFrame.Size.Y);
            playerFrameSprite.Position = spot;
            insideFrameSprite = new Sprite(insideFrame);
            insideFrameSprite.Scale = new Vector2f(FRAMESIZE / insideFrame.Size.X, FRAMESIZE / insideFrame.Size.Y);
            insideFrameSprite.Position = new Vector2f(spot.X + INSIDE_SPACER, spot.Y + INSIDE_SPACER);
            insideFrameSprite.Color = PlayerClassI.getClassColor(PlayerClassI.getClassNum(player));

            hpPercentText = new Text("hppercent",GameBox.corbalFont,10U);
            hpPercentText.Position = new Vector2f(spot.X + INSIDE_SPACER + 2, spot.Y + INSIDE_SPACER + 8);
            hpPercentText.Color = Color.Black;
            threatText = new Text("threat", GameBox.corbalFont, 10U);
            threatText.Position = new Vector2f(spot.X + INSIDE_SPACER + INSIDESIZE - 5, spot.Y + INSIDE_SPACER + INSIDESIZE - 10);
            threatText.Color = Color.White;
        }

        internal override bool didClick(SimpleMouseState mouseState) {
            bool b = base.didClick(mouseState);
            if (b) {
                InGame.getInstance().setSelected(player);
            }
            return b;
        }
        internal override void draw(RenderWindow window) {
            window.Draw(playerFrameSprite);
            window.Draw(insideFrameSprite);
            hpPercentText.DisplayedString = Math.Round(player.getHPpercent() * 100, 1) + "%";
            window.Draw(hpPercentText);
            //draw threat 
            int threatRank = parent.getInGame().getThreatRankOnMainEnemy(player);
            if (threatRank != -1) {
                threatText.DisplayedString = threatRank + "";
                window.Draw(threatText);
            }

        }

    }
}
