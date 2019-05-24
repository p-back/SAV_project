using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ball_in_a_maze
{
    class ViewModel
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

            // load the start page
            mainWindow.MainFrame.Navigate(StartPage);
        }

        // instantiate the game itself
        public TheGame TheGame { get; set; }

        // the levels
        public LevelTraining LevelTraining { get; set; }

        // the UI pages
        public StartPage StartPage { get; set; }

        // class to display active COM connections
        public ActiveCOMPorts ActiveCOMPorts { get; set; }

    }
}
