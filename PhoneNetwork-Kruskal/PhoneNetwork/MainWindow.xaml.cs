using System.Dynamic;
using System.Globalization;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;


namespace PhoneNet
{
    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        DrawingVisual visual;
        DrawingContext dc;
        int width, height;

        int n = 4;      // num of vertexes
        double[,] M;    // adjacency matrix
        MyVector[] V;  // array of graph vertexes

        double S = 0;
        int i = 0;

        List<CityLink> links;

        public MainWindow()
        {
            InitializeComponent();

            width = (int)g.Width;
            height = (int)g.Height;

            visual = new DrawingVisual();

            Init();
        }

        void Init()
        {
            links = new List<CityLink>(); // for oplimal connection saving

            V = new MyVector[n];

            // city creating
            for (int i = 0; i < V.Length; i++)
            {
                double x = rnd.Next(50, width - 50);
                double y = rnd.Next(50, height - 50);
                V[i] = new MyVector(x, y, i); // i - vertex id
            }

            // Adjacency matrix
            M = new double[n, n];
            double s = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    s = Math.Sqrt((V[j].X - V[i].X) * (V[j].X - V[i].X) + (V[j].Y - V[i].Y) * (V[j].Y - V[i].Y));
                    s = Math.Round(s);
                    M[i, j] = s;
                    M[j, i] = s;

                    S += s;
                }
            }

            MatrixUpdate(); // Adjacency matrix

            Drawing();
        }
        
        private void MatrixUpdate()
        {
            dgMatrix.Columns.Clear();
            dgMatrix.Items.Clear();

            string[] labels = new string[] { " 0", " 1", " 2", " 3" };

            foreach (string label in labels)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = label;
                column.Width = 50;
                column.Binding = new Binding(label.Replace(' ', '_')); // for trick

                dgMatrix.Columns.Add(column);
            }

            #region Trick is using for dynamicaly adding the rows
            dynamic row0 = new ExpandoObject();
            dynamic row1 = new ExpandoObject();
            dynamic row2 = new ExpandoObject();
            dynamic row3 = new ExpandoObject();

            double[] values0 = { M[0, 0], M[0, 1], M[0, 2], M[0, 3] };
            double[] values1 = { M[1, 0], M[1, 1], M[1, 2], M[1, 3]};
            double[] values2 = { M[2, 0], M[2, 1], M[2, 2], M[2, 3] };
            double[] values3 = { M[3, 0], M[3, 1], M[3, 2], M[3, 3] };

            for (int i = 0; i < values0.GetLength(0); i++)
                ((IDictionary<String, Object>)row0)[labels[i].Replace(' ', '_')] = values0[i];
            dgMatrix.Items.Add(row0);
            for (int i = 0; i < values1.GetLength(0); i++)
                ((IDictionary<String, Object>)row1)[labels[i].Replace(' ', '_')] = values1[i];
            dgMatrix.Items.Add(row1);
            for (int i = 0; i < values2.GetLength(0); i++)
                ((IDictionary<String, Object>)row2)[labels[i].Replace(' ', '_')] = values2[i];
            dgMatrix.Items.Add(row2);
            for (int i = 0; i < values3.GetLength(0); i++)
                ((IDictionary<String, Object>)row3)[labels[i].Replace(' ', '_')] = values3[i];
            dgMatrix.Items.Add(row3);
            #endregion
        }

        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                CreateGraph(dc);
                DrawCities(dc);
                
                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void DrawCities(DrawingContext dc)
        {
            for (int i = 0; i < V.Length; i++)
            {
                double x = V[i].X;
                double y = V[i].Y;

                // Buildings
                Rect rect = new Rect()
                {
                    X = x,
                    Y = y,
                    Width = 5,
                    Height = 5,
                };
                dc.DrawRectangle(Brushes.White, null, rect);

                // City connections
                foreach (var link in links)
                    dc.DrawLine(new Pen(Brushes.White, 1), link.city1, link.city2);

                // Labeling
                FormattedText formattedText = new FormattedText(i.ToString(), CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight, new Typeface("Verdana"), 14, Brushes.White,
                                                                VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                Point textPos = new Point(x, y - 20);
                dc.DrawText(formattedText, textPos);
            }
        }

        // Kruskal's algorithm
        private void CreateGraph(DrawingContext dc)
        {
            int v1 = 0;
            int v2 = 0;
            int id = 0;

            for (int k = 1; k < n; k++)
            {
                double MinLength = 10000000;

                for (int i = 1; i < n; i++)
                    for (int j = 0; j < i; j++)
                    {
                        if (V[i].id != V[j].id && M[i, j] < MinLength)
                        {
                            v1 = i;
                            v2 = j;
                            MinLength = M[i, j];
                        }
                    }

                id = V[v2].id;
                for (i = 0; i < n; i++)
                    if (V[i].id == id)
                    {
                        V[i].id = V[v1].id;
                    }

                links.Add(new CityLink() { city1 = new Point(V[v1].X, V[v1].Y), city2 = new Point(V[v2].X, V[v2].Y) });
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Drawing();
        }

        class CityLink
        {
            public Point city1;
            public Point city2;
        }
    }
}