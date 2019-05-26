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
    /// Interaktionslogik für GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        public GamePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// the class GamePage only constists of one grid
        /// this function fills this grid dynamically
        /// </summary>
        /// <param name="Field"></param>
        public void LoadGameField(GameField Field)
        {
            // clear the grid
            gridGame.Children.Clear();
            gridGame.RowDefinitions.Clear();
            gridGame.ColumnDefinitions.Clear();

            gridGame.Loaded += OnLoaded;

            // create rows and coloums
            for (int i = 0; i < Field.Width; i++)
                gridGame.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < Field.Height; i++)
                gridGame.RowDefinitions.Add(new RowDefinition());

            // now fill the grid according to the field
            for (int i = 0; i < Field.Width; i++)
            {
                for (int j = 0; j < Field.Height; j++)
                {
                    // check what needs to be placed here
                    if (Field.PlayField[i,j] == GameField.GameElements.Border)
                    {
                        // border is a black rectangle
                        Rectangle rec = new Rectangle();
                        rec.Fill = new SolidColorBrush(Colors.Black);
                        rec.Margin = new Thickness(1);
                        Grid.SetColumn(rec, i);
                        Grid.SetRow(rec, j);
                        gridGame.Children.Add(rec);
                    }
                    else if (Field.PlayField[i, j] == GameField.GameElements.Hole)
                    {
                        Ellipse ell = new Ellipse();
                        ell.Fill = new SolidColorBrush(Colors.OrangeRed);
                        ell.Stroke = new SolidColorBrush(Colors.Black);
                        ell.StrokeThickness = 1;
                        Grid.SetColumn(ell, i);
                        Grid.SetRow(ell, j);
                        gridGame.Children.Add(ell);
                    }
                    else if (Field.PlayField[i, j] == GameField.GameElements.Finish)
                    {
                        Ellipse ell = new Ellipse();
                        ell.Fill = new SolidColorBrush(Colors.DarkGreen);
                        ell.Stroke = new SolidColorBrush(Colors.Black);
                        ell.StrokeThickness = 1;
                        Grid.SetColumn(ell, i);
                        Grid.SetRow(ell, j);
                        gridGame.Children.Add(ell);
                    }
                    else if (Field.PlayField[i, j] == GameField.GameElements.Empty)
                    {
                        // nothing to do here...
                    }
                }
            }

            // set the ball size and postion
            ellBall.Height = 10;
            ellBall.Width = 10;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            double test = gridGame.ActualHeight;
        }
    }
}
