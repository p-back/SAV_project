using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    /// <summary>
    /// this class represents the beginner level of the ball in a maze game
    /// there are no holes, just borders at the edges
    /// </summary>
    class LevelTraining
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public GameField.GameElements[,] Level { get; set; }

        public LevelTraining()
        {
            Height = 50;
            Width = 50;

            Level = new GameField.GameElements[Height, Width];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    // edges are filled with borders
                    if (i == 0 || j == 0 || i == Height-1 || j == Width - 1)
                    {
                        Level[i, j] = GameField.GameElements.Border;
                    }
                    else
                    {
                        Level[i, j] = GameField.GameElements.Empty;
                    }
                }
            }
            // set finish in the right upper edge
            Level[Height - 2, Width - 2] = GameField.GameElements.Finish;
        }
    }
}
