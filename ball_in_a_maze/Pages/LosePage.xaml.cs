using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ball_in_a_maze
{
    /// <summary>
    /// Interaktionslogik für LosePage.xaml
    /// </summary>
    public partial class LosePage : Page
    {
        public LosePage()
        {
            InitializeComponent();
        }

        public event EventHandler RetryLevel;
        public event EventHandler ChooseAnotherLevel;
        public event EventHandler CloseGame;

        private void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            RetryLevel?.Invoke(this, EventArgs.Empty);
        }

        private void btnChooseAnotherLevel_Click(object sender, RoutedEventArgs e)
        {
            ChooseAnotherLevel?.Invoke(this, EventArgs.Empty);
        }

        private void btnCloseGame_Click(object sender, RoutedEventArgs e)
        {
            CloseGame?.Invoke(this, EventArgs.Empty);
        }
    }
}
