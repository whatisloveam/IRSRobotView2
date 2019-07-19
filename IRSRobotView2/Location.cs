using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IRSRobotView2
{
    class Location
    {
        public Line UpLine;
        public Line LeftLine;

        public TextBlock Content;

        public bool UpWall { get; set; }
        public bool LeftWall { get; set; }

        public Location(int x, int y)
        {
            Content = new TextBlock();

            UpLine = new Line();
            LeftLine = new Line();

            UpLine.Stroke = Brushes.CornflowerBlue;
            LeftLine.Stroke = Brushes.CornflowerBlue;

            UpLine.StrokeThickness = 4;
            LeftLine.StrokeThickness = 4;

            UpLine.X1 = Constants.XOffset + x * Constants.LocationSize;
            UpLine.X2 = Constants.XOffset + (x + 1) * Constants.LocationSize;
            UpLine.Y1 = Constants.YOffset + y * Constants.LocationSize;
            UpLine.Y2 = Constants.YOffset + y * Constants.LocationSize;

            LeftLine.X1 = Constants.XOffset + x * Constants.LocationSize;
            LeftLine.X2 = Constants.XOffset + x * Constants.LocationSize;
            LeftLine.Y1 = Constants.YOffset + y * Constants.LocationSize;
            LeftLine.Y2 = Constants.YOffset + (y + 1) * Constants.LocationSize;

            Content.Width = Constants.LocationSize;
            Content.Height = Constants.LocationSize;

            Content.VerticalAlignment = VerticalAlignment.Top;
            Content.HorizontalAlignment = HorizontalAlignment.Left;
            Content.TextAlignment = TextAlignment.Center;

            Content.Margin = new Thickness(Constants.XOffset + x * Constants.LocationSize, Constants.YOffset + y * Constants.LocationSize, 0, 0);
            Content.Background = Brushes.White;

            LeftWall = true;
            UpWall = true;
        }
    }
}