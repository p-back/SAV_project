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
            levelBeginner = new LevelBeginner();

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
            startPage =         new StartPage();
            gamePage =          new GamePage();
            chooseLevelPage =   new ChooseLevelPage();
            winPage =           new WinPage();
            losePage =          new LosePage();
            // set data context of the pages which will do some binding
            startPage.DataContext = this;
            gamePage.DataContext = this;
            // and bind functions to their events
            startPage.TryConnecting                 += OnTryConnecting;
            gamePage.ResetGame                      += OnResetGame;
            gamePage.ChooseAnotherLevel             += OnChooseAnotherLevel;
            gamePage.CloseGame                      += CloseGame;
            chooseLevelPage.LevelTrainingSelected   += OnLevelTrainingSelected;
            chooseLevelPage.LevelBeginnerSelected   += OnLevelBeginnerSelected;
            chooseLevelPage.LevelAdvancedSelected   += OnLevelAdvancedSelected;
            winPage.RetryLevel                      += OnRetryLevel;
            winPage.ChooseAnotherLevel              += OnChooseAnotherLevel;
            winPage.CloseGame                       += CloseGame;
            losePage.RetryLevel                     += OnRetryLevel;
            losePage.ChooseAnotherLevel             += OnChooseAnotherLevel;
            losePage.CloseGame                      += CloseGame;

            // load the start page
            ChangeToPage(startPage);
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
        private void OnLevelAdvancedSelected(object sender, EventArgs e)
        {
            //load the training level
            theGame.LoadNewField(levelTraining.Level, levelTraining.Width, levelTraining.Height);
        }

        private void OnLevelBeginnerSelected(object sender, EventArgs e)
        {
            //load the training level
            theGame.LoadNewField(levelBeginner.Level, levelBeginner.Width, levelBeginner.Height);
        }

        private void OnLevelTrainingSelected(object sender, EventArgs e)
        {
            //load the training level
            theGame.LoadNewField(levelTraining.Level, levelTraining.Width, levelTraining.Height);
        }
        private void CloseGame(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnChooseAnotherLevel(object sender, EventArgs e)
        {
            ChangeToPage(chooseLevelPage);
        }

        private void OnResetGame(object sender, EventArgs e)
        {
            theGame.ResetGame();
            theGame.GameEnabled = true;
        }
        private void OnRetryLevel(object sender, EventArgs e)
        {
            ChangeToPage(gamePage);
            theGame.GameEnabled = true;
        }

        // --------------------------------------------------
        //      FUNCTIONS BEHIND EVENTS FROM MODEL
        // --------------------------------------------------
        private void OnBallPositionHasChanged(object sender, EventArgs e)
        {
            // set properties (value doesnt matter, we jsut need to reach the set function)
            Ball_X = 0;
            Ball_Y = 0;
        }
        private void OnGameFieldHasChanged(object sender, EventArgs e)
        {
            // load the game into the page before loading the page
            gamePage.LoadGameField(theGame.Field);
            ChangeToPage(gamePage);
            // enable game
            theGame.GameEnabled = true;
        }
        private void OnBallIsInFinish(object sender, EventArgs e)
        {
            ChangeToPage(winPage);
            theGame.GameEnabled = false;
        }

        private void OnBallIsInHole(object sender, EventArgs e)
        {
            ChangeToPage(losePage);
            theGame.GameEnabled = false;
        }
        private void OnDataError(object sender, EventArgs e)
        {
            MessageBox.Show("Data Error during connection.", "Connection error");
            // switch back to start page
            startPage.btnConnect.IsEnabled = true;
            startPage.cboxPorts.IsEnabled = true;
            ChangeToPage(startPage);
        }
        private void OnStartUpFailed(object sender, EventArgs e)
        {
            MessageBox.Show("Cant establish connection.", "Connection error");
            // switch back to start page
            startPage.btnConnect.IsEnabled = true;
            startPage.cboxPorts.IsEnabled = true;
            ChangeToPage(startPage);
        }
        private void OnPortsUpdated(object sender, EventArgs e)
        {
            //set property
            COMPorts = null;
        }
        private void OnStartUpSuccessfull(object sender, EventArgs e)
        {
            // startup was succesfull we can switch to the choose level page
            ChangeToPage(chooseLevelPage);
        }

        // --------------------------------------------------
        //                  MEMBERS
        // --------------------------------------------------
        // instantiate the game itself
        private TheGame theGame { get; set; }

        // the levels
        private LevelTraining levelTraining { get; set; }
        private LevelBeginner levelBeginner{ get; set; }

        // the UI pages
        private StartPage startPage { get; set; }
        private GamePage gamePage { get; set; }
        private ChooseLevelPage chooseLevelPage { get; set; }
        private WinPage winPage{ get; set; }
        private LosePage losePage{ get; set; }

        // class to display active COM connections
        private ActiveCOMPorts activeCOMPorts { get; set; }

        // --------------------------------------------------
        //              OTHER METHODS
        // --------------------------------------------------
        private void ChangeToPage(Page p)
        {
            mainView.Dispatcher.Invoke(() =>
            {
                mainView.MainFrame.Navigate(p);
            });
        }
    }
}
