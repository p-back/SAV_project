using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    public class TheGame
    {
        // --------------------------------------------------
        //                  PROPERTIES
        // --------------------------------------------------
        public MotionData Data { get; private set; }
        public GameField Field { get; private set; }
        public bool GameEnabled { get; set; }
        public double Border_H { get; set; }
        public double Border_W { get; set; }
        public double Hole_Radius { get; set; }
        public double Finish_Radius { get; set; }
        public double Ball_Radius { get; set; }

        public struct Dimension
        {
            public double Height;
            public double Width;
        };
        public Dimension[] Dimensions { get; set; }

        // --------------------------------------------------
        //                  EVENTS
        // --------------------------------------------------
        public event EventHandler BallIsInHole;
        public event EventHandler BallIsInFinish;
        public event EventHandler BallPositionHasChanged;
        public event EventHandler GameFieldHasChanged;
        public event EventHandler BoardIsCalibrated;

        // --------------------------------------------------
        //                PRIVATE MEMBERS
        // --------------------------------------------------

        // Old values from motion board to be able to calculate difference in movement
        private int Old_Value_X = 0;
        private int Old_Value_Y = 0;

        // Flag to ensure that motion board is calibrated
        private bool IsCalibrated = false;


        // --------------------------------------------------
        //               PUBLIC METHODS
        // --------------------------------------------------

        /// <summary>
        /// Method that loads new PlayField with given parameter and triggers event
        /// </summary>
        /// <param name="PlayField">The PlayField to be loaded</param>
        /// <param name="Width">Width of PlayField</param>
        /// <param name="Height">Height of PlayField</param>
        /// <param name="StartPositionX">X start position of Ball</param>
        /// <param name="StartPositionY">Y start position of Ball</param>
        public void LoadNewField(GameField.GameElements[,] PlayField, int Width, int Height, int StartPositionX = 1, int StartPositionY = 1)
        {
            Field.PlayField             = PlayField;
            Field.Width                 = Width;
            Field.Height                = Height;
            Field.BallPosition_X_W      = StartPositionX;
            Field.BallPosition_Y_H      = StartPositionY;

            // Infom ViewModel that GameField has changed
            GameFieldHasChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method that RESETS the whole Game Logic
        /// </summary>
        public void ResetGame()
        {
            // Reset FLAGs
            IsCalibrated = false;
            GameEnabled = false;

            // Reset Ball Position
            Field.BallPosition_X_W = 1;
            Field.BallPosition_Y_H = 1;
            Old_Value_X = 0;
            Old_Value_Y = 0;

            idx_x = 0;
            idx_y = 0;
            move_x = 0.0;
            move_y = 0.0;

            // Trigger Event so ViewModel can actually reset ball Position
            BallPositionHasChanged?.Invoke(this, EventArgs.Empty);
        }

        // --------------------------------------------------
        //                      CTOR
        // --------------------------------------------------
        public TheGame()
        {
            Field = new GameField();
            Data = new MotionData();

            // create a dimension for every type of object (+1 for the ball which is not part of thy enum)
            Dimensions = new Dimension[Enum.GetValues(typeof(GameField.GameElements)).Length +1];

            // Disable Game --> Reset State
            GameEnabled = false;

            // Add Event that is called when new data has arrived from motion board
            Data.NewDataAvailable += Data_NewDataArrived;
        }

        // --------------------------------------------------
        //              IMPLEMENTED EVENTS
        // --------------------------------------------------

        /// <summary>
        /// Event Handler that is called by MotionData when new motion values are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            // Set NEW position of BALL
            SetBallPosition();
        }

        // --------------------------------------------------
        //              PRIVATE METHODS
        // --------------------------------------------------

        private int idx_x = 0;
        private int idx_y = 0;
        private double move_x = 0;
        private double move_y = 0;

        private bool CalcIdx_out_of_Motion()
        {
            // Calculate movement of ball
            int Diff_X = Old_Value_X + Data.Axis_X;
            int Diff_Y = Old_Value_Y + Data.Axis_Y;

            // Save new values
            Old_Value_X = Data.Axis_X;
            Old_Value_Y = Data.Axis_Y;

            move_x = Diff_X / 5000.0;
            move_x = Math.Round(move_x, 1);
            move_x = -move_x;
            move_y = Diff_Y / 5000.0;
            move_y = Math.Round(move_y, 1);
            move_y = -move_y;

            // Prevent ball from hitting border
            var t1 = Field.BallPosition_X_W + move_x;
            var t2 = Field.BallPosition_Y_H + move_y;
            idx_x = (int)Math.Round(t1, 0);
            idx_y = (int)Math.Round(t2, 0);

            if ((idx_x < 0) || (idx_y < 0) || (idx_x >= Field.Width) || (idx_y >= Field.Height))
                return false;

            return true;
        }

        /// <summary>
        /// Method that calculates and sets the new ball position depending on input motion values
        /// </summary>
        private void SetBallPosition()
        {
            if(!CalcIdx_out_of_Motion())
                return;

            if (Field.PlayField[idx_x, idx_x] == GameField.GameElements.Border)
                return;

            // Move Ball
            Field.BallPosition_X_W += move_x;
            Field.BallPosition_Y_H += move_y;

            // If Ball is EITHER in HOLE or in FINISH --> no need to move it anymore --> Game is OVER
            if (IsBallInHole() || IsBallInFinish())
                return;

            // Trigger Event --> inform ViewModel that Ball Position has changed
            BallPositionHasChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method that checks if Ball is in finish, and triggers EVENT
        /// </summary>
        /// <returns></returns>
        private bool IsBallInFinish()
        {
            if (Field.PlayField[idx_x, idx_x] == GameField.GameElements.Finish)
            {
                BallIsInFinish?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Method that checks if BALL is in HOLE and triggers EVENT
        /// </summary>
        /// <returns></returns>
        private bool IsBallInHole()
        {
            if(Field.PlayField[idx_x, idx_y] == GameField.GameElements.Hole)
            {
                BallIsInHole?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        private void Calibrate()
        {
            if ((Data.Axis_X <= 150) && (Data.Axis_X >= -150) &&
                (Data.Axis_Y <= 150) && (Data.Axis_Y >= -150))
            {
                IsCalibrated = true;
                BoardIsCalibrated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
