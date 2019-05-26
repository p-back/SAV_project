using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    public class GameField
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
        public int Width { get; set; }
        public int Height { get; set; }
        public double BallPosition_X_W { get; set; }
        public double BallPosition_Y_H { get; set; }

        public void PrintPlayField_Console()
        {
            if (PlayField == null)
                return;

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
