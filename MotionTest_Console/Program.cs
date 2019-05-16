using MotionTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionTest_Console
{
    class Program
    {
        private static MotionData data;

        static void Main(string[] args)
        {
            var game = new TheGame(10, 10);
            //data = new MotionData(NewDataArrived);
            game.PrintPlayField_Console();
            while (true) ;
        }

        private static void NewDataArrived(object sender, EventArgs e)
        {
            Console.WriteLine("X: {0} Y: {1} Z: {2}", data.Axis_X.ToString().PadRight(8), data.Axis_Y.ToString().PadRight(8), data.Axis_Z.ToString().PadRight(8));
        }
    }
}
