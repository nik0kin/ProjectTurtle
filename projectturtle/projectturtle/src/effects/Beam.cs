using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

namespace ProjectTurtle.src.effects {
    class Beam {
        internal static void drawBeam(RenderWindow window, Vector2f v1, Vector2f v2, Sprite sprite){
            Vector2f currentLoc = v1;
            int spacingX = 2, spacingY = 2;

            //double xNeeded = d.getMid().X - start.X;
            //double yNeeded = d.getMid().Y - start.Y;
            if (Math.Sign(v1.X - v2.X) > 0) {
                spacingX *= -1;
            }
            if (Math.Sign(v1.Y - v2.Y) > 0) {
                spacingY *= -1;
            }
            bool xGood, yGood;
            do{
                double d1 = Math.Abs(currentLoc.X - v2.X);
                if (!(xGood = (d1 <= Math.Abs(spacingX))))
            //double amtMovedX = ((gameTime - startMoveTime) / (travelTime * 1000)) * xNeeded;
                    currentLoc.X += spacingX;// (float)amtMovedX;
                double d2 = Math.Abs(currentLoc.Y - v2.Y);
                if (!(yGood = (d2 <= Math.Abs(spacingY))))
            //double amtMovedY = ((gameTime - startMoveTime) / (travelTime * 1000)) * yNeeded;
                    currentLoc.Y += spacingY;//(float)amtMovedY;
                sprite.Position = currentLoc;
                window.Draw(sprite);
            } while (!xGood || !yGood);
        }

    }
}
