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
        public GamePage(double PageHeight, double PageWidth)
        {
            InitializeComponent();

            // initialize ball
            ellBall.Fill = new SolidColorBrush(Colors.Gold);
            ellBall.Stroke = new SolidColorBrush(Colors.Black);

            // save the page width and height for later usage
            pageHeight = PageHeight;
            pageWidth = PageWidth;
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

        private double pageHeight { get; set; }
        private double pageWidth { get; set; }

        /// <summary>
        /// the class GamePage has one grid as top level object 
        /// there are a grid and a canvas in it
        /// these two are overlapping
        /// this function fills this grid dynamically
        /// the canvas has a ellipse which represents the game ball
        /// </summary>
        /// <param name="Field"></param>
        public void LoadGameField(GameField Field)
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
            
            // create the binding (must be done everytime a new field is loaded because the converter calculation changes)
            DoTheBinding(ellBall, Canvas.LeftProperty, "Ball_X", new GridIndexToPixelConverter(pageWidth, Field.Width));
            DoTheBinding(ellBall, Canvas.TopProperty, "Ball_Y", new GridIndexToPixelConverter(pageHeight, Field.Height));

            // set the new game ball size
            ellBall.Height = pageHeight / Field.Height * 3 / 4;
            ellBall.Width = pageWidth / Field.Width * 3 / 4;
        }
    }
}
