using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    class TheGame
    {
        // --------------------------------------------------
        //                  PROPERTIES
        // --------------------------------------------------
        public MotionData Data { get => data; private set => data = value; }
        public GameField Field { get => field; private set => field = value; }
        public bool GameEnabled { get; set; }

        // --------------------------------------------------
        //                  EVENTS
        // --------------------------------------------------
        public event EventHandler BallIsInHole;
        public event EventHandler BallIsInFinish;
        public event EventHandler BallPositionHasChanged;
        public event EventHandler GameFieldHasChanged;

        // --------------------------------------------------
        //                  MEMBERS
        // --------------------------------------------------

        // Old values from motion board to be able to calculate difference in movement
        private int Old_Value_X = 0;
        private int Old_Value_Y = 0;

        // Flag to ensure that motion board is calibrated
        private bool IsCalibrated = false;

        private GameField field;
        private MotionData data;

        // --------------------------------------------------
        //                      CTOR
        // --------------------------------------------------
        public TheGame()
        {
            field = new GameField();
            data = new MotionData();

            GameEnabled = false;

            // TODO: remove this
            Worker = new BackgroundWorker();
            Worker.DoWork += OnWorkerDoWork;
            Worker.RunWorkerAsync();
        }

        public void ResetGame()
        {
            // TODO: reset ball position

            // TODO: evtl kalibrieren
        }

        private void OnWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (GameEnabled)
                {
                    while (field.BallPosition_X_W < 0.9*field.Width)
                    {
                        Field.BallPosition_X_W += 0.1;
                        Field.BallPosition_Y_H += 0.1;
                        BallPositionHasChanged?.Invoke(this, EventArgs.Empty);
                        Thread.Sleep(10);
                    }
                    while (field.BallPosition_X_W > 0.1 * field.Width)
                    {
                        Field.BallPosition_X_W -= 0.2;
                        Field.BallPosition_Y_H -= 0.2;
                        BallPositionHasChanged?.Invoke(this, EventArgs.Empty);
                        Thread.Sleep(10);
                    }
                }
            }
        }

        public BackgroundWorker Worker { get; set; }

        // --------------------------------------------------
        //              IMPLEMENTED EVENTS
        // --------------------------------------------------
        private void Data_NewDataArrived(object sender, EventArgs e)
        {
            // DO NOTHING --> if Game is NOT enabled yet
            if (!GameEnabled)
                return;

            // If Board is NOT calibrated yet --> calibrate it
            if (!IsCalibrated)
            {
                Calibrate();
                return;
            }

            // If Game is ENABLED and Board is CALIBRATED --> handle motion data and set BALL POSITION

            //Console.WriteLine("X: {0} Y: {1} Z: {2}", data.Axis_X.ToString().PadRight(8), data.Axis_Y.ToString().PadRight(8), data.Axis_Z.ToString().PadRight(8));

            // Kalibrierung nach StartUp
            // SetBallPosition + Überprüfung + Event an ModelView wenn Kugel in Loch oder Kugel in Ziel

            // TODO before the following steps --> START UP / CALIBRATION phase
            // As long as the values are not "near" to zero --> DO NOTHING
            // That means, that at the beginning of the game, the player does not 
            // grab the board right --> therefore --> wait until board is held is "normal" position

            // TODO: after START UP
            // 1) Calculate difference between old values and new values --> and then calculate movement of ball
            // 2) Check if Ball would hit wall --> prevent this
            // 3) If ball is in hole --> trigger event to ViewModel
            // 4) If ball is in finish --> trigger event to ViewModel
            // 5) and evertime the position of the ball changes --> trigger "PositionHasChanged" for ViewModel

            // Maybe some of these tasks could be integrated into the Model "GameField"
        }

        private void Calibrate()
        {
            if ((Data.Axis_X <= 150) && (Data.Axis_X >= -150) &&
                (Data.Axis_Y <= 150) && (Data.Axis_Y >= -150))
            {
                IsCalibrated = true;
            }
        }

        // --------------------------------------------------
        //                      METHODS
        // --------------------------------------------------
        public void LoadNewField(GameField.GameElements[,] PlayField, int Width, int Height, int StartPositionX = 1, int StartPositionY = 1)
        {
            Field.PlayField = PlayField;
            Field.Width = Width;
            Field.Height = Height;
            Field.BallPosition_X_W = StartPositionX;
            Field.BallPosition_Y_H = StartPositionY;
            GameFieldHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ResetGame()
        {
            Field.BallPosition_X_W = 1;
            Field.BallPosition_Y_H = 1;
        }
    }
}
