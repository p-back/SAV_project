using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    class LevelBase
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public GameField.GameElements[,] Level { get; set; }
    }
}
