using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionTest_Console
{
    class TheGame
    {
        public enum GameElements { Border, Empty, Hole };
        //public char[] ConsoleSymbols = new char[] { '#', ' ', 'x' } { get; private set; }
        public GameElements[,] PlayField { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BallPosition_W_X { get; private set; }
        public int BallPosition_H_Y { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="PlayField_Width"></param>
        /// <param name="PlayField_Height"></param>
        public TheGame(int PlayField_Width, int PlayField_Height)
        {
            // Save dimensions and create PLAYFIELD
            Width = PlayField_Width;
            Height = PlayField_Height;
            PlayField = new GameElements[PlayField_Width, PlayField_Height];

            // Reset all fields to Empty blocks
            for (int w = 0; w < Width; w++)
            {
                for (int h = 0; h < Height; h++)
                {
                    if ( (w == 0) || (h == 0) || (w == Width-1) || (h ==  Height-1) )
                        PlayField[w, h] = GameElements.Border;
                    else
                        PlayField[w,h] = GameElements.Empty;
                }
            }

            // Reset BALL position
            BallPosition_W_X = 1;
            BallPosition_H_Y = 1;

            //
        }

        public void PrintPlayField_Console()
        {
            // Reset all fields to Empty blocks
            for (int w = 0; w < Width; w++)
            {
                for (int h = 0; h < Height; h++)
                {
                    if ( (BallPosition_H_Y == h) && (BallPosition_W_X == w))
                            Console.Write('*');
                    else
                        Console.Write(PlayField[w, h] == GameElements.Empty ? ' ' : '#');
                }
                Console.WriteLine();
            }
        }
    }
}
