﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    class TheGame
    {
        // --------------------------------------------------
        //                  PROPERTIES
        // --------------------------------------------------
        public GameField.GameElements[,] TheGameField
        {
            get { return Field.PlayField; }
            set
            {
                if (!Equals(value, Field.PlayField))
                {
                    Field.PlayField = value;
                    //OnPropertyChanged("TheGameField");
                }
            }
        }

        public int BallPosition_X
        {
            get { return Field.BallPosition_X_W; }
            set
            {
                if (value != Field.BallPosition_X_W)
                {
                    Field.BallPosition_X_W = value;
                    //OnPropertyChanged("BallPosition_X");
                }
            }
        }
        public int BallPosition_Y
        {
            get { return Field.BallPosition_Y_H; }
            set
            {
                if (value != Field.BallPosition_Y_H)
                {
                    Field.BallPosition_Y_H = value;
                    //OnPropertyChanged("BallPosition_Y");
                }
            }
        }

        public MotionData Data { get => data; private set => data = value; }
        public GameField Field { get => field; private set => field = value; }

        // --------------------------------------------------
        //                  EVENTS
        // --------------------------------------------------
        public event EventHandler BallIsInHole;
        public event EventHandler BallIsInFinish;
        public event EventHandler PositionHasChanged;

        // --------------------------------------------------
        //                  MEMBERS
        // --------------------------------------------------

        // Old values from motion board to be able to calculate difference in movement
        private int Old_Value_X = 0;
        private int Old_Value_Y = 0;

        private GameField field;
        private MotionData data;

        // --------------------------------------------------
        //                      CTOR
        // --------------------------------------------------
        public TheGame(int GameField_Width, int GameField_Height, int PortNumber)
        {
            field = new GameField(GameField_Width, GameField_Height);
            data = new MotionData(PortNumber);
           
            data.NewDataAvailable   += Data_NewDataArrived;
            /* This is done in the ViewModel
            data.StartUpFailed      += Data_StartUpFailed;
            data.DataError          += Data_DataError;
            */
        }

        // --------------------------------------------------
        //              IMPLEMENTED EVENTS
        // --------------------------------------------------

        private void Data_NewDataArrived(object sender, EventArgs e)
        {
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

        // --------------------------------------------------
        //                      METHODS
        // --------------------------------------------------
        public void LoadNewField(GameField.GameElements[,] PlayField)
        {
            TheGameField = PlayField;
        }
    }
}
