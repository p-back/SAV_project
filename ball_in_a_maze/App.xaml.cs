using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ball_in_a_maze
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MainView view = new MainView();
            ViewModel viewModel = new ViewModel(view);
            view.Show();
        }
    }
}
