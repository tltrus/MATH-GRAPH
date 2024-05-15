using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphBFS
{
    public partial class MainWindow : Window
    {
        Map Field;
        Point pMouse;
        DrawingVisual visual;
        DrawingContext dc;
        int oldCell;
        AdjacencyMatrix AdjMatrix;
        BFS BFS;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            Field = new Map();

            AdjMatrix = new AdjacencyMatrix();
            BFS = new BFS();
        }

        private void Init()
        {
            rtbConsole.Document.Blocks.Clear();

            rtbConsole.AppendText("Draw the walls by left mouse button");
            rtbConsole.AppendText("\rDraw Start and Finish points by right mouse button");
            rtbConsole.AppendText("\rPress Calculation button");
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

        // BFS
        private void BtnBFSStart_Click(object sender, RoutedEventArgs e)
        {
            string result = AdjMatrix.CreateAdjacencyMatrix(Map.map);
            rtbConsole.AppendText("\r" + result);

            BFS.CreatePath(AdjMatrix.adjacency, Map.iStart, Map.iFinish);
            var path = BFS.GetPath();
            rtbConsole.AppendText("\r" + "Path [cell num]: " + path);

            Draw();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Field.Clear();
            Init();
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
            Init();
        }
    }
}
