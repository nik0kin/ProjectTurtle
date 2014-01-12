using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.ui;
using ProjectTurtle.src.util;
using ProjectTurtle.src.bossfights;

namespace ProjectTurtle.src.screens {
    class MenuScreen {
        RollOverStringButton play, leaderboard, stats, settings, help, exit;

        static Texture background, stripe, logo;

        Sprite bgSprite, stripeSprite, logoSprite;

        internal static void LoadContent(){
            background = GameBox.loadTexture("images/screens/mainmenubackground");
            stripe = GameBox.loadTexture("images/screens/mainmenustripe");
            logo = GameBox.loadTexture("images/screens/mainmenulogo");
        }
        internal MenuScreen(){
            int left = 85;
            play = new RollOverStringButton(new Vector2f(left, 215), "PLAY", GameBox.pixelzimFont, Color.Black, Color.Green,false,35);
            leaderboard = new RollOverStringButton(new Vector2f(left, 215 + 48), "LEADERBOARD", GameBox.pixelzimFont, Color.Black, Color.Red, false, 35);
            stats = new RollOverStringButton(new Vector2f(left, 215 + 96), "STATISTICS", GameBox.pixelzimFont, Color.Black, Color.Red, false, 35);
            settings = new RollOverStringButton(new Vector2f(left, 215 + 96 + 48), "SETTINGS", GameBox.pixelzimFont, Color.Black, Color.Green, false, 35);
            help = new RollOverStringButton(new Vector2f(left, 215 + 96 + 96), "ABOUT", GameBox.pixelzimFont, Color.Black, Color.Red, false, 35);
            exit = new RollOverStringButton(new Vector2f(left, 215 + 96 + 96 + 48), "EXIT", GameBox.pixelzimFont, Color.Black, Color.Green, false, 35);

            bgSprite = new Sprite(background);
            bgSprite.Position = new Vector2f(0.0f, 0.0f);

            stripeSprite = new Sprite(stripe);
            stripeSprite.Position = new Vector2f(70.0f, 0.0f);

            logoSprite = new Sprite(logo);
            logoSprite.Position = new Vector2f(80.0f, 100.0f);
        }
        internal void Draw(RenderWindow window) {
            window.Draw(bgSprite);
            window.Draw(stripeSprite);
            window.Draw(logoSprite);

            play.draw(window);
            leaderboard.draw(window);
            stats.draw(window);
            settings.draw(window);
            help.draw(window);
            exit.draw(window);
        }
        internal void Update(double gameTime, SimpleMouseState mouseState) {
            if (play.didClick(mouseState)) //Game.getInstance().setScreen(GameState.bossSelect);
            	GameBox.getInstance().initRaidSelect(BossFightID.urisdor);

            leaderboard.didClick(mouseState);
            stats.didClick(mouseState);

            settings.didClick(mouseState);
//            if (settings.didClick(mouseState)) GameBox.getInstance().setScreen(GameState.settingsMenu);
            
            help.didClick(mouseState);
            
            if (exit.didClick(mouseState)) GameBox.getInstance().Exit();
        }


    }
}
