using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphLee
{
    public partial class MainWindow : Window
    {
        Map Field;
        Point pMouse;
        DrawingVisual visual;
        DrawingContext dc;
        int oldCell;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            Field = new Map();
        }

        private void Draw()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                Field.Draw(visual, dc);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Alg_Lee.PathCalculation(Field.map, Field.pStart, Field.pFinish);
            Draw();

        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Field.Clear();
            Draw();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pMouse = e.GetPosition(g);
            oldCell = Field.GetCellindex(pMouse);

            Field.SetWall(pMouse);
            Draw();
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            pMouse = e.GetPosition(g);
            Field.SetStartFinish(pMouse);
            Draw();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                pMouse = e.GetPosition(g);

                int index = Field.GetCellindex(pMouse);

                if (index != oldCell)
                {
                    Field.SetWall(pMouse);
                    Draw();
                    oldCell = index;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var wayList = Alg_Lee.PathFind(Field.map, Field.pStart, Field.pFinish);
            foreach (var w in wayList)
            {
                int y = (int)w.Y;
                int x = (int)w.X;

                Field.map[y, x] = -5;
            }

            Draw();
        }
    }
}
