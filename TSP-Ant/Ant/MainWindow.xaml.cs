using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;


namespace WpfApp
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;

        public delegate void FinalDrawHandler();
        public event FinalDrawHandler? FinalDrawNotify;

        Random rnd = new Random();
        public static DrawingVisual visual;
        DrawingContext dc;
        int width, height;

        Ant Ant;

        public MainWindow()
        {
            InitializeComponent();

            width = (int)g.Width;
            height = (int)g.Height;

            visual = new DrawingVisual();

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }

        void Init()
        {
            g.Children.Clear();
            if (rtbConsole.Document == null)
                rtbConsole.Document = new System.Windows.Documents.FlowDocument();

            rtbConsole.Document.Blocks.Clear();
            Ant = new Ant();
            Ant.BestTimeNotify += (str) => {

                // Добавляется первая строка
                if (rtbConsole.Document.Blocks.Count == 0)
                {
                    rtbConsole.Document.Blocks.Add(new Paragraph(new Run(str)));
                    return;
                }

                // Вторая и следующие строки добавляются перед первой
                rtbConsole.Document.Blocks.InsertBefore(rtbConsole.Document.Blocks.FirstBlock,
                    new Paragraph(new Run(str)));

            };
            Ant.TimerNotify += () =>
            {
                timer.Stop();
            };
        }
        
        private void Drawing()
        {
            g.RemoveVisual(visual);

            using (dc = visual.RenderOpen())
            {
                Ant.Draw(dc);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void timerTick(object? sender, EventArgs e)
        {
            Ant.Calculate();
            Drawing();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Init();
            timer.Start();
        }
    }
}