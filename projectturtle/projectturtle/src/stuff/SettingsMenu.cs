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
    class SettingsMenu {
        internal const string IMAGEPATH = "images/menus/raidselect/";
        const int BUTTONS_Y = 525;
        const int BACK_BUTTON_X = 475;

        static Texture backButtonTexture;
        
        Button backButton;
        SimpleMouseState mouseState;

        internal static void LoadContent() {
            backButtonTexture = GameBox.loadTexture(IMAGEPATH + "backbutton");
            //throw new NotImplementedException();
        }

        internal SettingsMenu() {
            backButton = new Button(new IntRect(BACK_BUTTON_X, BUTTONS_Y, 50, 50), backButtonTexture); 
        }

        internal void Update(double gameTime, SimpleMouseState mouseState) {
            this.mouseState = mouseState;
            if (backButton.didClick(mouseState)) {
                GameBox.getInstance().setScreen(GameState.menuScreen);
            }
            //throw new NotImplementedException();
        }

        internal void Draw(RenderWindow window) {
            //throw new NotImplementedException();
            window.Clear(new Color(255, 245, 238));
            backButton.draw(window);
        } 
    }
}
