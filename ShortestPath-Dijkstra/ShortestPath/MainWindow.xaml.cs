using System.Dynamic;
using System.Globalization;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;


namespace WpfApp
{
    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        DrawingVisual visual;
        DrawingContext dc;
        int width, height;


        Vertex[] V;     // Массив вершин графа
        double[,] M;    // Матрица смежности (расстояний)
        int n = 5;      // Количество вершин

        int StartVertex;    // Стартовая вершина
        int FinishVertex;   // Финишная вершина

        //int i = 0;          // счетчик цикла
        int notMarked;  // Количество неотмеченных вершин
        int vm, pv;         // Индексы вершин
        int MinDist;        // В матрице смежности обозначает, что ребро отсутствует между вершинами


        public MainWindow()
        {
            InitializeComponent();

            width = (int)g.Width;
            height = (int)g.Height;

            visual = new DrawingVisual();

            Init();
            Drawing();
        }

        void Init()
        {
            StartVertex = 0;    // Стартовая вершина
            FinishVertex = n - 1;   // Финишная вершина
            MinDist = 10000000; // В матрице смежности обозначает, что ребро отсутствует между вершинами
            vm = pv = notMarked = 0;

            V = new Vertex[n];

            for (int i = 0; i < V.Length; i++)
            {
                double x = rnd.Next(10, width - 10);
                double y = rnd.Next(10, height - 10);
                V[i] = new Vertex(x, y);                        // 
                V[i].marked = false;                            // Ни одна вершина не отмечена
            }

            V[StartVertex].marked = true;   // отметить стартовую точку
            V[StartVertex].prevVertex = -1; // У стартовой вершины нет предыдущей
            notMarked = n - 1;              // начальное количество неотмеченных вершин

            // Matrix
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
                }
            }
            // При автоматическом создании матрицы смежности разрываем ребро между начальной и финишной ячейками
            M[StartVertex, FinishVertex] = MinDist;
            M[FinishVertex, StartVertex] = MinDist;

            // Из-за того, что при создании Матрица смежности ссылается на объекты вершин, а вершины ссылались на матрицу
            // этот кусок программы вынесен сюда, чтобы не было конфликта
            for (int i = 0; i < V.Length; i++)
            {
                V[i].distFromStart = (int)M[StartVertex, i];    // Начальные расстояния
            }

            MatrixUpdate(); // Adjacency matrix
        }
        
        private void MatrixUpdate()
        {
            dgMatrix.Columns.Clear();
            dgMatrix.Items.Clear();

            string[] labels = new string[] { " 0", " 1", " 2", " 3", " 4" };

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
            dynamic row4 = new ExpandoObject();

            double[] values0 = { M[0, 0], M[0, 1], M[0, 2], M[0, 3], M[0, 4] };
            double[] values1 = { M[1, 0], M[1, 1], M[1, 2], M[1, 3], M[1, 4] };
            double[] values2 = { M[2, 0], M[2, 1], M[2, 2], M[2, 3], M[2, 4] };
            double[] values3 = { M[3, 0], M[3, 1], M[3, 2], M[3, 3], M[3, 4] };
            double[] values4 = { M[4, 0], M[4, 1], M[4, 2], M[4, 3], M[4, 4] };

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
            for (int i = 0; i < values4.GetLength(0); i++)
                ((IDictionary<String, Object>)row4)[labels[i].Replace(' ', '_')] = values4[i];
            dgMatrix.Items.Add(row4);
            #endregion
        }

        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                DrawCities(dc);
                CreateGraph(dc);

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
                dc.DrawEllipse(Brushes.White, null, new Point(x, y), 4, 4);

                // Connections
                DrawConnections(dc);

                // Labeling
                FormattedText formattedText = new FormattedText(i.ToString(), CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight, new Typeface("Verdana"), 14, Brushes.White,
                                                                VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                Point textPos = new Point(x, y - 20);
                dc.DrawText(formattedText, textPos);
            }
        }

        private void DrawConnections(DrawingContext dc)
        {
            // City connections
            for (int i = 1; i < n; i++)
                for (int j = 0; j < i; j++)
                    if (M[i, j] < 10000000)
                    {
                        Point p0 = new Point(V[i].X, V[i].Y);
                        Point p1 = new Point(V[j].X, V[j].Y);
                        dc.DrawLine(new Pen(Brushes.White, 0.5), p0, p1);
                    }
        }

        // Dijkstra's algorithm
        private void CreateGraph(DrawingContext dc)
        {
            while (notMarked != 0)  // пока есть неотмеченные вершины
            {
                // Поиск минимального ребра у непросмотренных вершин
                for (int i = 0; i < n; i++)
                    if (!V[i].marked && V[i].distFromStart < MinDist) // найти неотмеченную вершину
                    {
                        vm = i;                         // с минимальным значением distFromStart
                        MinDist = V[i].distFromStart;   // 
                    }

                V[vm].marked = true;    // отметить ее
                notMarked--;

                // Продолжение поиска, исключая ранее найденное ребро
                for (int i = 0; i < n; i++)
                    if (!V[i].marked) // цикл по всем неотмеченным вершинам
                    {
                        if (V[i].distFromStart > (V[vm].distFromStart + (int)M[vm, i]))
                        {
                            V[i].distFromStart = V[vm].distFromStart + (int)M[vm, i];
                            V[i].prevVertex = vm;
                        }
                    }
            }

            pv = V[FinishVertex].prevVertex;  // содержит индекс предыдущей вершины, куда нужно рисовать ребро

            float x1 = (float)V[FinishVertex].X;
            float y1 = (float)V[FinishVertex].Y;

            // Отрисовка ребер шаг за шагом пока не достигнем стартовой вершины с prevVertex = -1
            do
            {
                dc.DrawLine(new Pen(Brushes.Yellow, 2), new Point(x1, y1), new Point(V[pv].X, V[pv].Y));

                x1 = (float)V[pv].X;
                y1 = (float)V[pv].Y;

                pv = V[pv].prevVertex;

            } while (pv > -1);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Drawing();
        }

        public class Vertex
        {
            public readonly double X;
            public readonly double Y;

            public bool marked; // Индикатор состояния
            public int distFromStart; // Расстояние от стартовой вершины
            public int prevVertex; // Предыдущая вершина

            public Vertex(double x, double y)
            {
                X = x;
                Y = y;
            }
        }
    }
}