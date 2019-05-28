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

        private void DoTheBinding(Ellipse ell, DependencyProperty prop, string str, GridIndexToPixelConverter conv)
        {
            Binding bind = new Binding();
            bind.Converter = conv;
            bind.Path = new PropertyPath(str);
            bind.Mode = BindingMode.OneWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ell.SetBinding(prop, bind);
        }

        /// <summary>
        /// the class GamePage has one grid as top level object 
        /// there are a grid and a canvas in it
        /// these two are overlapping
        /// this function fills this grid dynamically
        /// the canvas has a ellipse which represents the game ball
        /// </summary>
        /// <param name="Field"></param>
        public void LoadGameField(GameField Field, TheGame.Dimension[] Dimensions)
        {
            // clear the grid
            gridGame.Children.Clear();
            gridGame.RowDefinitions.Clear();
            gridGame.ColumnDefinitions.Clear();

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
                        int RecMargin = 1;
                        rec.Margin = new Thickness(RecMargin);
                        Grid.SetColumn(rec, i);
                        Grid.SetRow(rec, j);
                        gridGame.Children.Add(rec);
                        // set the dimension
                        Dimensions[(int)GameField.GameElements.Border].Height = (gridGame.Height / Field.Height - RecMargin) / (gridGame.Height / Field.Height);
                        Dimensions[(int)GameField.GameElements.Border].Width  = (gridGame.Width / Field.Width   - RecMargin) / (gridGame.Width / Field.Width);
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
                        Dimensions[(int)GameField.GameElements.Hole].Height = (gridGame.Height / Field.Height) / (gridGame.Height / Field.Height);
                        Dimensions[(int)GameField.GameElements.Hole].Width  = (gridGame.Width / Field.Width) / (gridGame.Width / Field.Width);
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
                        Dimensions[(int)GameField.GameElements.Finish].Height = (gridGame.Height / Field.Height) / (gridGame.Height / Field.Height);
                        Dimensions[(int)GameField.GameElements.Finish].Width = (gridGame.Width / Field.Width) / (gridGame.Width / Field.Width);
                    }
                    else if (Field.PlayField[i, j] == GameField.GameElements.Empty)
                    {
                        // nothing to do here...
                        Dimensions[(int)GameField.GameElements.Empty].Height = (gridGame.Height / Field.Height) / (gridGame.Height / Field.Height);
                        Dimensions[(int)GameField.GameElements.Empty].Width = (gridGame.Width / Field.Width) / (gridGame.Width / Field.Width);
                    }
                }
            }
            
            // create the binding (must be done everytime a new field is loaded because the converter calculation changes)
            DoTheBinding(ellBall, Canvas.LeftProperty, "Ball_X", new GridIndexToPixelConverter(gridGame.Width, Field.Width));
            DoTheBinding(ellBall, Canvas.TopProperty, "Ball_Y", new GridIndexToPixelConverter(gridGame.Height, Field.Height));

            // set the new game ball size
            ellBall.Height = gridGame.Height / Field.Height * 3 / 4;
            ellBall.Width = gridGame.Width / Field.Width * 3 / 4;

            // set ball dimension for collision detecting and trigger event
            Dimensions[Dimensions.Length].Height = ellBall.Height / (gridGame.Height / Field.Height);
            Dimensions[Dimensions.Length].Width =  ellBall.Width  / (gridGame.Width / Field.Width);
            NewDimensionsAvailable?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ResetGame;
        public event EventHandler CloseGame;
        public event EventHandler ChooseAnotherLevel;
        public event EventHandler NewDimensionsAvailable;

        private void btnResetGame_Click(object sender, RoutedEventArgs e)
        {
            ResetGame?.Invoke(this, EventArgs.Empty);
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
