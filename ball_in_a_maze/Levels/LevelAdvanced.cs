using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    class LevelAdvanced : LevelBase
    {
        public LevelAdvanced()
        {
            const int height = 20;
            const int width = 20;

            Height = height;
            Width = width;
            Level = new GameField.GameElements[Height, Width];

            // for better visibility
            char[][] arr = new char[height][] {
                ("********************").ToCharArray(),
                ("* O   O   O   O   O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O O O*").ToCharArray(),
                ("* O O O O O O O OFF*").ToCharArray(),
                ("*   O   O   O   OFF*").ToCharArray(),
                ("********************").ToCharArray()
            };

            // convert this array to the right type
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    GameField.GameElements elem = new GameField.GameElements();
                    if (arr[i][j] == ' ')
                        elem = GameField.GameElements.Empty;
                    else if (arr[i][j] == '*')
                        elem = GameField.GameElements.Border;
                    else if (arr[i][j] == 'O')
                        elem = GameField.GameElements.Hole;
                    else if (arr[i][j] == 'F')
                        elem = GameField.GameElements.Finish;

                    Level[j, i] = elem;
                }
            }
        }
    }
}
