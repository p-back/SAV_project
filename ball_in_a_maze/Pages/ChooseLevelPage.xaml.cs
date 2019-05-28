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
    /// Interaktionslogik für ChooseLevel.xaml
    /// </summary>
    public partial class ChooseLevelPage : Page
    {
        public ChooseLevelPage()
        {
            InitializeComponent();
        }

        public event EventHandler LevelTrainingSelected;
        public event EventHandler LevelBeginnerSelected;
        public event EventHandler LevelAdvancedSelected;

        private void btnLevelTraining_Click(object sender, RoutedEventArgs e)
        {
            LevelTrainingSelected?.Invoke(this, EventArgs.Empty);
        }

        private void btnLevelBeginner_Click(object sender, RoutedEventArgs e)
        {
            LevelBeginnerSelected?.Invoke(this, EventArgs.Empty);
        }

        private void btnLevelAdvanced_Click(object sender, RoutedEventArgs e)
        {
            LevelAdvancedSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}
