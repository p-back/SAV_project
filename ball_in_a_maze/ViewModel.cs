using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ball_in_a_maze
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel(MainView mainWindow)
        {
            mainWindow.DataContext = this;

            ActiveCOMPorts = new ActiveCOMPorts();

            // create the pages
            StartPage = new StartPage();
            StartPage.DataContext = this;
            StartPage.ConnectionEstablished += ActiveCOMPorts.OnConnectionestablished;

            // create the levels
            LevelTraining = new LevelTraining();

            // create the game
            TheGame = new TheGame(LevelTraining.Width, LevelTraining.Height, 1);

            //load the training level
            TheGame.LoadNewField(LevelTraining.Level);

            // TODO: MotionData can now establish connection to board via "TryConnect"
            // TheGame.Data.TryConnect(PortNum);

            // TODO: implement this event --> it indicates that the postion
            // of the ball has changed --> call PropertyChanged event
            // TheGame.PositionHasChanged += ;

            // TODO: add events for MotionData (Error Events)
            //TheGame.Data.StartUpFailed  += Data_StartUpFailed;
            //TheGame.Data.DataError      += Data_DataError;

            // TODO: implement these two events from TheGame
            // TheGame.BallIsInHole += ;
            // TheGame.BallIsInFinish += ;

            // load the start page
            mainWindow.MainFrame.Navigate(StartPage);
        }

        // TODO: ViewModel MUST contain the PROPERTIES that the View
        // will BIND on to --> BallPosition, ...
        // See "TheGame.cs" for already finished code and copy here

        // TODO: these public properties can be private members --> ViewModel does not need to access them
        // instantiate the game itself
        // TODO: change name of these variabled --> must not be equal to class names
        public TheGame TheGame { get; set; }

        // the levels
        public LevelTraining LevelTraining { get; set; }

        // the UI pages
        public StartPage StartPage { get; set; }

        // class to display active COM connections
        public ActiveCOMPorts ActiveCOMPorts { get; set; }

        // TODO: needs to be implemented
        public event PropertyChangedEventHandler PropertyChanged;

        // TODO: use this method to call "PropertyChanged" Event
        private void OnPropertyChanged(string prop_name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop_name));
        }
    }
}
