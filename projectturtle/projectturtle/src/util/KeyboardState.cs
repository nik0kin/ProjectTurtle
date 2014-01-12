using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

//temp soultion? if this isnt fast. its too redundant i think
namespace ProjectTurtle.src.util {
    struct SimpleKeyboardState {
        bool F1;
        bool A,S,D;
        bool Tab;
        bool Numpad0;

        public SimpleKeyboardState(bool init) {
            F1 = Keyboard.IsKeyPressed(Keyboard.Key.F1);

            A = Keyboard.IsKeyPressed(Keyboard.Key.A);
            S = Keyboard.IsKeyPressed(Keyboard.Key.S);
            D = Keyboard.IsKeyPressed(Keyboard.Key.D);

            Tab = Keyboard.IsKeyPressed(Keyboard.Key.Tab);

            Numpad0 = Keyboard.IsKeyPressed(Keyboard.Key.Numpad0);
        }

        public bool IsKeyDown(Keyboard.Key key){
            switch(key){
            case Keyboard.Key.F1:
                return F1;
            case Keyboard.Key.A:
                return A;
            case Keyboard.Key.S:
                return S;
            case Keyboard.Key.D:
                return D;

            case Keyboard.Key.Tab:
                return Tab;


            case Keyboard.Key.Numpad0:
                return Numpad0;

            }

            throw new Exception("need to add this key: " + key);
        }
    }
}
