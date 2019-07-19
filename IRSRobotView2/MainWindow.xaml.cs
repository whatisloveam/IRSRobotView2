using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace IRSRobotView2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort myPort;
        string[] ports;

        Location[,] Maze;
        Floodfill floodfill;
        int StartX, StartY, FinishX, FinishY;

        public MainWindow()
        {
            InitializeComponent();
            Maze = new Location[5, 9];
            GenerateMaze();

            floodfill = new Floodfill();

            myPort = new SerialPort();
            myPort.DataReceived += MyPort_DataReceived;
            myPort.ReadTimeout = 2000;
            myPort.WriteTimeout = 2000;

            SendButton.IsEnabled = false;
            DisconnectButton.IsEnabled = false;
            ports = SerialPort.GetPortNames();
            comboBox.ItemsSource = ports;
            comboBox.SelectedValue = "COM7";
        }

        private void GenerateMaze()
        {
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 9; x++)
                {
                    Maze[y, x] = new Location(x, y);
                    Maze[y, x].Content.PreviewMouseDown +=
                        new MouseButtonEventHandler(this.Content_PreviewMouseDown);
                    myGrid.Children.Add(Maze[y, x].LeftLine);
                    myGrid.Children.Add(Maze[y, x].UpLine);
                    myGrid.Children.Add(Maze[y, x].Content);
                }
        }

        private void Content_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int hoverX = ((int)((TextBlock)sender).Margin.Left - Constants.XOffset) / 40;
            int hoverY = ((int)((TextBlock)sender).Margin.Top - Constants.XOffset) / 40;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    Maze[Constants.GreenY, Constants.GreenX].Content.Background = Brushes.White;
                    Maze[hoverY, hoverX].Content.Background = Brushes.Green;
                    Constants.GreenX = hoverX;
                    Constants.GreenY = hoverY;
                }
                else
                {
                    Maze[Constants.RedY, Constants.RedX].Content.Background = Brushes.White;
                    Maze[hoverY, hoverX].Content.Background = Brushes.Red;
                    Constants.RedX = hoverX;
                    Constants.RedY = hoverY;
                }
            }
            else
            {
                var status = (2 * Convert.ToInt32(Maze[hoverY, hoverX].LeftWall)
                    + Convert.ToInt32(Maze[hoverY, hoverX].UpWall) + 1) % 4;

                Maze[hoverY, hoverX].LeftWall = Convert.ToBoolean(status / 2);
                Maze[hoverY, hoverX].UpWall = Convert.ToBoolean(status % 2);

                Maze[hoverY, hoverX].LeftLine.Stroke =
                    Maze[hoverY, hoverX].LeftWall ? Brushes.CornflowerBlue : Brushes.White;
                Maze[hoverY, hoverX].UpLine.Stroke =
                    Maze[hoverY, hoverX].UpWall ? Brushes.CornflowerBlue : Brushes.White;
            }
        }

        private void MyPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string readstring;
            try
            {
                readstring = myPort.ReadLine();
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    textBox.AppendText(readstring + "\n");
                    textBox.ScrollToEnd();
                }));
            }
            catch (TimeoutException)
            {

            }
        }

        private void ResetMarks()
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    Maze[y, x].Content.Text = "0";
                    if (Maze[y, x].Content.Background == Brushes.Pink)
                        Maze[y, x].Content.Background = Brushes.White;
                }
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            ResetMarks();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartX = Constants.GreenX;
            StartY = Constants.GreenY;
            FinishX = Constants.RedX;
            FinishY = Constants.RedY;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMarks();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            myPort.PortName = (string)comboBox.SelectedItem;
            myPort.BaudRate = 57600;
            try
            {
                textBox.Clear();
                myPort.Open();
                ConnectButton.IsEnabled = false;
                DisconnectButton.IsEnabled = true;
                SendButton.IsEnabled = true;
            }
            catch (InvalidOperationException)
            {

            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myPort.Close();
                ConnectButton.IsEnabled = true;
                DisconnectButton.IsEnabled = false;
                SendButton.IsEnabled = false;
            }
            catch (InvalidOperationException)
            {

            }
        }






        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string writestring;
            writestring = textBox1.Text;
            try
            {
                myPort.WriteLine(String.Format("{0}", writestring));
                textBox1.Clear();
            }
            catch (TimeoutException)
            {

            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //textBox.CaretIndex = textBox.Text.Length;
            textBox.ScrollToEnd();
        }

    }
}
