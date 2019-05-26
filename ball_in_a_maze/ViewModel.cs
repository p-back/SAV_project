using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ball_in_a_maze 
{
    public class ViewModel : INotifyPropertyChanged
    {
        public MainView mainView { get; set; }

        // viewmodel property
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop_name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop_name));
        }

        public ViewModel(MainView mainWindow)
        {
            mainWindow.DataContext = this;
            // save the mainView for further usage
            mainView = mainWindow;

            activeCOMPorts = new ActiveCOMPorts();
            activeCOMPorts.PortsUpdated += OnPortsUpdated;

            // create the levels
            levelTraining = new LevelTraining();

            // create the game and connect to events
            theGame = new TheGame();
            theGame.BallPositionHasChanged      += OnBallPositionHasChanged;
            theGame.GameFieldHasChanged         += OnGameFieldHasChanged;
            theGame.BallIsInHole                += OnBallIsInHole;
            theGame.BallIsInFinish              += OnBallIsInFinish;
            theGame.Data.StartUpSuccessfull     += OnStartUpSuccessfull;
            theGame.Data.StartUpFailed          += OnStartUpFailed;
            theGame.Data.DataError              += OnDataError;

            // create the pages
            startPage = new StartPage();
            startPage.DataContext = this;
            startPage.TryConnecting += OnTryConnecting;

            gamePage = new GamePage(mainWindow.MainFrame.Height, mainWindow.MainFrame.Height);
            gamePage.DataContext = this;

            //load the training level
            theGame.LoadNewField(levelTraining.Level, levelTraining.Width, levelTraining.Height);

            // load the start page
            mainWindow.MainFrame.Navigate(startPage);
        }

        // --------------------------------------------------
        //                  PROPERTIES
        // --------------------------------------------------
        public string[] COMPorts
        {
            get { return activeCOMPorts.Ports; }
            set
            {
                OnPropertyChanged("COMPorts");
            }
        }

        public double Ball_X
        {
            get { return theGame.Field.BallPosition_X_W; }
            set
            {
                OnPropertyChanged("Ball_X");
            }
        }
        public double Ball_Y
        {
            get { return theGame.Field.BallPosition_Y_H; }
            set
            {
                OnPropertyChanged("Ball_Y");
            }
        }


        // --------------------------------------------------
        //       FUNCTIONS BEHIND EVENTS FROM UI
        // --------------------------------------------------
        private void OnTryConnecting(object sender, EventArgs e)
        {
            startPage.btnConnect.IsEnabled = false;
            startPage.cboxPorts.IsEnabled = false;

            // try connecting to the chosen COM port
            int port = startPage.cboxPorts.SelectedIndex;
            theGame.Data.TryConnect(port);
        }
        
        // --------------------------------------------------
        //      FUNCTIONS BEHIND EVENTS FROM MODEL
        // --------------------------------------------------
        private void OnBallPositionHasChanged(object sender, EventArgs e)
        {
            // set properties
            Ball_X = theGame.Field.BallPosition_X_W;
            Ball_Y = theGame.Field.BallPosition_Y_H;
        }
        private void OnGameFieldHasChanged(object sender, EventArgs e)
        {
            gamePage.LoadGameField(theGame.Field);
        }
        private void OnBallIsInFinish(object sender, EventArgs e)
        {
            // TODO switch to WIN page
        }

        private void OnBallIsInHole(object sender, EventArgs e)
        {
            // TODO switch to LOST page
        }
        private void OnDataError(object sender, EventArgs e)
        {
            MessageBox.Show("Data Error during connection.", "Connection error");
            // switch back to start page
            startPage.btnConnect.IsEnabled = true;
            startPage.cboxPorts.IsEnabled = true;
            mainView.MainFrame.Navigate(startPage);
        }
        private void OnStartUpFailed(object sender, EventArgs e)
        {
            MessageBox.Show("Cant establish connection.", "Connection error");
            // switch back to start page
            startPage.btnConnect.IsEnabled = true;
            startPage.cboxPorts.IsEnabled = true;
            mainView.MainFrame.Navigate(startPage);
        }
        private void OnPortsUpdated(object sender, EventArgs e)
        {
            //set property
            COMPorts = null;
        }
        private void OnStartUpSuccessfull(object sender, EventArgs e)
        {
            // startup was succesfull we can switch to the game page
            mainView.MainFrame.Navigate(gamePage);
            // enable game
            theGame.GameEnabled = true;
        }

        // --------------------------------------------------
        //                  MEMBERS
        // --------------------------------------------------
        // instantiate the game itself
        private TheGame theGame { get; set; }

        // the levels
        private LevelTraining levelTraining { get; set; }

        // the UI pages
        private StartPage startPage { get; set; }
        private GamePage gamePage { get; set; }

        // class to display active COM connections
        private ActiveCOMPorts activeCOMPorts { get; set; }
    }
}
