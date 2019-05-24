using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallInAMaze
{
    class GameField
    {
        //   ------------- W / X --------------
        //   -                                -
        //   -                                -
        // H / Y                            H / Y
        //   -                                -
        //   -                                -
        //   -                                -
        //   ------------- W / X --------------
        public enum GameElements { Border, Empty, Hole, Finish };
        public GameElements[,] PlayField { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BallPosition_X_W { get; set; }
        public int BallPosition_Y_H { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="PlayField_Width"></param>
        /// <param name="PlayField_Height"></param>
        public GameField(int PlayField_Width, int PlayField_Height, GameElements[,] playField = null, int x_ballposition = 1, int y_ballposition = 1)
        {
            // Save dimensions and create PLAYFIELD
            Width = PlayField_Width;
            Height = PlayField_Height;

            // Reset BALL position
            BallPosition_X_W = x_ballposition;
            BallPosition_Y_H = y_ballposition;

            // Create NEW game or Load with given
            if (playField == null)
            {
                PlayField = new GameElements[PlayField_Width, PlayField_Height];
                return;
            }
            else
            {
                PlayField = playField;
            }

            // Reset all fields to Empty blocks
            for (int w = 0; w < Width; w++)
            {
                for (int h = 0; h < Height; h++)
                {
                    // if it is a BORDER
                    if ( (w == 0) || (h == 0) || (w == Width-1) || (h ==  Height-1) )
                        PlayField[w, h] = GameElements.Border;
                    else
                        PlayField[w,h] = GameElements.Empty;
                }
            }           
        }

        public void PrintPlayField_Console()
        {
            // Reset all fields to Empty blocks
            for (int w = 0; w < Width; w++)
            {
                for (int h = 0; h < Height; h++)
                {
                    if ( (BallPosition_Y_H == h) && (BallPosition_X_W == w))
                            Console.Write('*');
                    else
                        Console.Write(PlayField[w, h] == GameElements.Empty ? ' ' : '#');
                }
                Console.WriteLine();
            }
        }
    }
}
