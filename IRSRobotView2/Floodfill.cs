using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IRSRobotView2
{
    class Floodfill
    {
        int[,] Mark;
        //     i     0  1   2  3
        int[] dx = { 1, 0, -1, 0 };
        int[] dy = { 0, -1, 0, 1 };

        public Floodfill()
        {
            Mark = new int[5, 9];
            ResetMarks();
        }

        public void ResetMarks()
        {
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 9; x++)
                    Mark[y, x] = 0;
        }
        public void UpdateMarks(Location[,] Maze)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    Maze[y, x].Content.Text = Convert.ToString(Mark[y, x]);
                }
            }
        }
        //                   startX  startY finishX finishY
        public bool Compute(int xs, int ys, int xf, int yf, Location[,] Maze)
        {
            ResetMarks();

            Mark[ys, xs] = 1;// start with mark 1

            int x, y;

            if (Solve(Maze, xf, yf)) // if we have solution, then go in reverse direction by marks
            {
                UpdateMarks(Maze);
                x = xf;
                y = yf;
                for (int N = Mark[yf, xf]; N >= 1; N--) // go by mark decresing
                {
                    if (!(Maze[y, x].Content.Background == Brushes.Green // not brush start and 
                        || Maze[y, x].Content.Background == Brushes.Red))// finish
                        Maze[y, x].Content.Background = Brushes.Pink; // route will have pink color

                    for (int i = 0; i < 4; i++) // find the cell marked lower by 1
                    {
                        if (CanGo(x, y, dx[i], dy[i], Maze) && Mark[y + dy[i], x + dx[i]] == N - 1)
                        {
                            x += dx[i];
                            y += dy[i];
                            break;
                        }
                    }
                }
                return true;
            }
            else // fail, we not find solution
            {
                UpdateMarks(Maze);
                return false;
            }
        }

        private bool Solve(Location[,] Maze, int xf, int yf) // that function search solution
        {
            int N = 1;// start flooding from 1
            bool NoSolution;

            do
            {
                NoSolution = true; // pessimistically believe that there is no solution

                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 4; y++)
                        if (Mark[y, x] == N) // find the cell of the N step
                        {
                            for (int i = 0; i < 4; i++) // flood in all directions
                                if (CanGo(x, y, dx[i], dy[i], Maze) && Mark[y + dy[i], x + dx[i]]
                                    == 0)
                                {
                                    NoSolution = false; //if we could spill, then maybe there is a solution
                                    Mark[y + dy[i], x + dx[i]] = N + 1; // we put on the cell the next step spill
                                    if (x + dx[i] == xf && y + dy[i] == yf) // if you find a finish then win 
                                        return true;//and exit from function!!!
                                }
                        }
                N++; // while there is plenty to pour
            }
            while (NoSolution == false); // while there is empty cells or not find finish coord
            return false; // fail
        }
        private bool CanGo(int x, int y, int dx, int dy, Location[,] Maze)
        {
            if (dx == -1) return !Maze[y, x].LeftWall;
            else if (dx == 1) return !Maze[y, x + 1].LeftWall;
            else if (dy == -1) return !Maze[y, x].UpWall;
            else return !Maze[y + 1, x].UpWall;
        }
    }
}
