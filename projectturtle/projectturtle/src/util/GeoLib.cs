using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

namespace ProjectTurtle.src.util{
	public class GeoLib{
		internal static bool isInRect(FloatRect rect, Vector2f pos){
			return !(pos.X < rect.Left || pos.Y < rect.Top
            || pos.X > rect.Left + rect.Width || pos.Y > rect.Top + rect.Height);
		}
        internal static bool isInRect(IntRect rect, Vector2i pos) {
            return !(pos.X < rect.Left || pos.Y < rect.Top
            || pos.X > rect.Left + rect.Width || pos.Y > rect.Top + rect.Height);
        }

        internal static bool isInRect(Vector2f start, FloatRect rect, Vector2f pos){
            return !(pos.X < start.X + rect.Left || pos.Y < start.Y + rect.Top
            || pos.X > start.X + rect.Left + rect.Width || pos.Y > start.Y + rect.Top + rect.Height);
        }
        //these all go to top left tho lol.
        internal static Vector2f midBottomPosToTopRight(Vector2f midBottom, FloatRect rect) {
            return new Vector2f(midBottom.X - rect.Width / 2, midBottom.Y - rect.Height);
        }
        internal static Vector2f midBottomPosToTopRight(Vector2f midBottom, FloatRect rect, float scale) {
            return new Vector2f(midBottom.X - rect.Width / 2 * scale, midBottom.Y - rect.Height * scale);
        }
        internal static Vector2f midBottomPosToTopRight(Vector2f midBottom, IntRect rect, float scale) {
            return new Vector2f(midBottom.X - rect.Width / 2 * scale, midBottom.Y - rect.Height * scale);
        }
        internal static FloatRect midBottomPosToTopRightRect(Vector2f midBottom, FloatRect rect) {
            return new FloatRect((int)midBottom.X - rect.Width / 2, (int)midBottom.Y - rect.Height, rect.Width, rect.Height);
        }
        internal static FloatRect midBottomPosToTopRightRect(Vector2f midBottom, IntRect rect) {
            return new FloatRect((int)midBottom.X - rect.Width / 2, (int)midBottom.Y - rect.Height, rect.Width, rect.Height);
        }

        internal static IntRect getRectOfFrame(short mFrame, int framesPerRow, Vector2f frameSize) {
            int rowNum = mFrame / framesPerRow;
            int colNum = mFrame % framesPerRow;

            return new IntRect((int)(colNum * frameSize.X), (int)(rowNum * frameSize.Y), (int)frameSize.X, (int)frameSize.Y);
        }

        internal static float radsToDegree(float rad) {

            return (float)(rad * 180.0f / Math.PI);
        }

        
    }
}

