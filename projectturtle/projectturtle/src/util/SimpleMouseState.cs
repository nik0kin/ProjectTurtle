using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.util {
    class SimpleMouseState {
        public bool left;
        public bool mid;
        public bool right;
        public bool prevLeft;
        public bool prevMid;
        public bool prevRight;
        Vector2i position;

        public int mouseWheelPos = 0;
        public int prevMouseWheelPos = 0;

        internal SimpleMouseState(RenderWindow window) {
            window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(updateMouseWheel);
        }

        internal void update(RenderWindow window) { 
            prevLeft = left;
		    prevMid = mid;
		    prevRight = right;
		    left = Mouse.IsButtonPressed(Mouse.Button.Left);
		    mid = Mouse.IsButtonPressed(Mouse.Button.Middle);
		    right = Mouse.IsButtonPressed(Mouse.Button.Right);
            
		    position = Mouse.GetPosition(window);
            
        }
        internal void wheelUpdate() {
            //how can i redo this so i dont need this
            prevMouseWheelPos = mouseWheelPos;
        }
        internal void updateMouseWheel(object sender, MouseWheelEventArgs e) {           
            mouseWheelPos += e.Delta;
        }
        internal bool DoneClickedIt(mouseButton button) {
            switch (button) {
            case mouseButton.LEFT:
                return left && !prevLeft;
            case mouseButton.MIDDLE:
                return mid && !prevMid;
            case mouseButton.RIGHT:
                return right && !prevRight;
            default:
                throw new Exception("wtf");
            }
        }
		///checking to see if you released the mouse guarentees you can't double click 
		///something through to the next page
		internal bool DoneReleasedIt(mouseButton button) {
            switch (button) {
            case mouseButton.LEFT:
                return !left && prevLeft;
            case mouseButton.MIDDLE:
                return !mid && prevMid;
            case mouseButton.RIGHT:
                return !right && prevRight;
            default:
                throw new Exception("wtf");
            }
        }
        internal Vector2i getPos() {
            return position;
        }
    }
    enum mouseButton{
        LEFT, MIDDLE, RIGHT
    };
}
