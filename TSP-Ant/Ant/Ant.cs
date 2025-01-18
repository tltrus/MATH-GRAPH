using System.Globalization;
using System.Windows;
using System.Windows.Media;


namespace WpfApp
{
    /*
        C# version of the code from the book:

        M. Tim Jones (Author)
        AI Application Programming (Programming Series) 2nd Edition
        https://www.amazon.com/AI-Application-Programming-Tim-Jones/dp/1584504218
    */
    internal class Ant
    {
        public delegate void StopHandler();
        public event StopHandler? TimerNotify;
        public delegate void bestTimeHandler(string bestTime);
        public event bestTimeHandler? BestTimeNotify;

        Random rnd = new Random();

        const int MAX_CITIES = 85;
        const int MAX_DISTANCE = 600;
        const int MAX_TOUR = MAX_CITIES * MAX_DISTANCE;

        const int MAX_ANTS = 20;

        const double ALPHA = 1.0;
        const double BETA = 5.0;
        const double RHO = 0.5;	/* Intensity / Evaporation */
        const int QVAL = 100;

        const int MAX_TOURS = 100;

        const int MAX_TIME	= (MAX_TOURS * MAX_CITIES);

        const double INIT_PHEROMONE	= (1.0 / MAX_CITIES);

        int curTime = 1;
        bool isFinish;

        internal struct cityType
        {
            public int x, y, id;
        }

        internal class antType
        {
            public int curCity;
            public int nextCity;
            public int[] tabu = new int[MAX_CITIES]; // посещенные города
            public int pathIndex;
            public int [] path = new int[MAX_CITIES];
            public double tourLength; // общая длина пути
            
            public antType()
            {
            }
        }

        public cityType[] cities = new cityType[MAX_CITIES];

        public antType[] ants = new antType[MAX_ANTS];

                                        /*   From          To     */
        //double[][] distance = new double[MAX_CITIES][MAX_CITIES];
        double[][] distance = Tools.CreateJaggedArray<double[][]>(MAX_CITIES, MAX_CITIES);

                                        /*    From         To      */
        //double[][] pheromone = new double[MAX_CITIES][MAX_CITIES];
        double[][] pheromone = Tools.CreateJaggedArray<double[][]>(MAX_CITIES, MAX_CITIES);

        double best = (double)MAX_TOUR;
        int bestIndex;

        public Ant()
        {
            Init();
        }

        public string Calculate()
        {
            string bestTime = "";

            if (curTime < MAX_TIME)
            {
                if (SimulateAnts() == 0)
                {
                    UpdateTrails();

                    if (curTime != MAX_TIME)
                        RestartAnts();

                    bestTime = "Time is " + curTime + " / best tour " + best.ToString("F2");
                    BestTimeNotify?.Invoke(bestTime);
                }
            }
            else
            {
                isFinish = true;
                TimerNotify?.Invoke();
            }
            curTime++;

            return bestTime;
        }

        // Initialize the cities, their distances and the Ant population.
        void Init()
        {
            int from, to, ant;

            //curTime = 0;

            //cities[0].x = 10;
            //cities[0].y = 10;

            //cities[1].x = 10;
            //cities[1].y = 90;

            //cities[2].x = 90;
            //cities[2].y = 10;

            //cities[3].x = 90;
            //cities[3].y = 90;

            //cities[4].x = 40;
            //cities[4].y = 30;

            //cities[5].x = 70;
            //cities[5].y = 40;

            //cities[6].x = 20;
            //cities[6].y = 60;

            //cities[7].x = 80;
            //cities[7].y = 60;

            //cities[8].x = 45;
            //cities[8].y = 70;

            //cities[9].x = 55;
            //cities[9].y = 85;

            //cities[10].x = 85;
            //cities[10].y = 35;

            //cities[11].x = 65;
            //cities[11].y = 42;

            //cities[12].x = 77;
            //cities[12].y = 49;

            //cities[13].x = 52;
            //cities[13].y = 80;

            //cities[14].x = 50;
            //cities[14].y = 50;

            /* Create the cities and their locations */
            for (from = 0; from < MAX_CITIES; from++)
            {
                /* Randomly place cities around the grid */
                cities[from].x = rnd.Next(10, MAX_DISTANCE - 10);
                cities[from].y = rnd.Next(10, MAX_DISTANCE - 10);
                cities[from].id = from;

                for (to = 0; to < MAX_CITIES; to++)
                {
                    distance[from][to] = 0.0;
                    pheromone[from][to] = INIT_PHEROMONE;
                }
            }

            /* Compute the distances for each of the cities on the map */
            for (from = 0; from < MAX_CITIES; from++)
            {
                for (to = 0; to < MAX_CITIES; to++)
                {
                    if ((to != from) && (distance[from][to] == 0.0))
                    {
                        int xd = Math.Abs(cities[from].x - cities[to].x);
                        int yd = Math.Abs(cities[from].y - cities[to].y);

                        distance[from][to] = Math.Sqrt((xd * xd) + (yd * yd));
                        distance[to][from] = distance[from][to];
                    }
                }
            }

            /* Initialize the ants */
            to = 0;
            for (ant = 0; ant < MAX_ANTS; ant++)
            {
                /* Distribute the ants to each of the cities uniformly */
                if (to == MAX_CITIES) to = 0;

                ants[ant] = new antType();
                ants[ant].curCity = to++;

                for (from = 0; from < MAX_CITIES; from++)
                {
                    ants[ant].tabu[from] = 0;   // 0 - город еще не посещен, 1 - уже добвлен в маршрут
                    ants[ant].path[from] = -1;
                }

                ants[ant].pathIndex = 1;
                ants[ant].path[0] = ants[ant].curCity;
                ants[ant].nextCity = -1;
                ants[ant].tourLength = 0.0;

                /* Load the ant's current city into taboo */
                ants[ant].tabu[ants[ant].curCity] = 1;

            }
        }

        // Reinitialize the ant population to start another tour around the graph.
        void RestartAnts()
        {
            int ant, i, to = 0;

            for (ant = 0; ant < MAX_ANTS; ant++)
            {

                if (ants[ant].tourLength < best)
                {
                    best = ants[ant].tourLength;
                    bestIndex = ant;
                }

                ants[ant].nextCity = -1;
                ants[ant].tourLength = 0.0;

                for (i = 0; i < MAX_CITIES; i++)
                {
                    ants[ant].tabu[i] = 0;
                    ants[ant].path[i] = -1;
                }

                if (to == MAX_CITIES) to = 0;
                ants[ant].curCity = to++;

                ants[ant].pathIndex = 1;
                ants[ant].path[0] = ants[ant].curCity;

                ants[ant].tabu[ants[ant].curCity] = 1;

            }
        }

        /*
         *  antProduct()
         *
         *  Compute the denominator for the path probability equation (concentration
         *  of pheromone of the current path over the sum of all concentrations of
         *  available paths).
         *
         */
        double AntProduct(int from, int to)
        {
            return ((Math.Pow(pheromone[from][to], ALPHA) * Math.Pow((1.0 / distance[from][to]), BETA)));
        }

        /*
         *  selectNextCity()
         *
         *  Using the path probability selection algorithm and the current pheromone
         *  levels of the graph, select the next city the ant will travel to.
         *
         */
        int SelectNextCity(int ant)
        {
            int from, to;
            double denom = 0.0;

            /* Choose the next city to visit */
            from = ants[ant].curCity;

            /* Compute denom */
            for (to = 0; to < MAX_CITIES; to++)
            {
                if (ants[ant].tabu[to] == 0)
                {
                    denom += AntProduct(from, to);
                }
            }

            if (denom == 0.0)
                throw new Exception("denom = 0");

            do
            {
                double p;

                to++;
                if (to >= MAX_CITIES) to = 0;

                if (ants[ant].tabu[to] == 0)
                {
                    p = AntProduct(from, to) / denom;

                    if (rnd.NextDouble() < p) break;
                    //if (0.4 < p) break;
                }

            } while (true);

            return to;
        }

        /*
         *  simulateAnts()
         *
         *  Simulate a single step for each ant in the population.  This function
         *  will return zero once all ants have completed their tours.
         *
         */
        int SimulateAnts()
        {
            int k;
            int moving = 0;

            for (k = 0; k < MAX_ANTS; k++)
            {
                /* Ensure this ant still has cities to visit */
                if (ants[k].pathIndex < MAX_CITIES)
                {

                    ants[k].nextCity = SelectNextCity(k);

                    ants[k].tabu[ants[k].nextCity] = 1;

                    ants[k].path[ants[k].pathIndex++] = ants[k].nextCity;

                    ants[k].tourLength += distance[ants[k].curCity][ants[k].nextCity];

                    /* Handle the final case (last city to first) */
                    if (ants[k].pathIndex == MAX_CITIES)
                    {
                        ants[k].tourLength +=
                          distance[ants[k].path[MAX_CITIES - 1]][ants[k].path[0]];
                    }

                    ants[k].curCity = ants[k].nextCity;

                    moving++;
                }
            }
            return moving;
        }

        /*
         *  updateTrails()
         *
         *  Update the pheromone levels on each arc based upon the number of ants
         *  that have travelled over it, including evaporation of existing pheromone.
         *
         */
        void UpdateTrails()
        {
            int from, to, i, ant;

            /* Pheromone Evaporation */
            for (from = 0; from < MAX_CITIES; from++)
            {
                for (to = 0; to < MAX_CITIES; to++)
                {
                    if (from != to)
                    {
                        pheromone[from][to] *= (1.0 - RHO);

                        if (pheromone[from][to] < 0.0) pheromone[from][to] = INIT_PHEROMONE;
                    }
                }
            }

            /* Add new pheromone to the trails */

            /* Look at the tours of each ant */
            for (ant = 0; ant < MAX_ANTS; ant++)
            {
                /* Update each leg of the tour given the tour length */
                for (i = 0; i < MAX_CITIES; i++)
                {
                    if (i < MAX_CITIES - 1)
                    {
                        from = ants[ant].path[i];
                        to = ants[ant].path[i + 1];
                    }
                    else
                    {
                        from = ants[ant].path[i];
                        to = ants[ant].path[0];
                    }

                    pheromone[from][to] += (QVAL / ants[ant].tourLength);
                    pheromone[to][from] = pheromone[from][to];
                }
            }

            for (from = 0; from < MAX_CITIES; from++)
            {
                for (to = 0; to < MAX_CITIES; to++)
                {
                    pheromone[from][to] *= RHO;
                }
            }
        }

        public void Draw(DrawingContext dc)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < cities.Length; i++)
            {
                // Draw cities
                Point p = new Point(cities[i].x, cities[i].y);
                dc.DrawEllipse(Brushes.Black, null, p, 3, 3);

                // Draw labeling
                FormattedText formattedText = new FormattedText(i.ToString(), CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight, new Typeface("Verdana"), 11, Brushes.Black,
                                                                VisualTreeHelper.GetDpi(MainWindow.visual).PixelsPerDip);
                dc.DrawText(formattedText, new Point(p.X, p.Y - 20));

                // Add points for contour

                var indx = ants[bestIndex].path[i];
                if (indx == -1) continue;
                
                var x = cities[ants[bestIndex].path[i]].x;
                var y = cities[ants[bestIndex].path[i]].y;
                points.Add(new Point(x, y));
            }

            // Draw contour
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(points[0], false, true);
                geometryContext.PolyLineTo(points, true, false);
            }

            Pen pen = new Pen();
            if (!isFinish)
            {
                pen.Brush = Brushes.Blue;
                pen.Thickness = 0.5;
            }
            else
            {
                pen.Brush = Brushes.LimeGreen;
                pen.Thickness = 3;
            }
            dc.DrawGeometry(null, pen, streamGeometry);
        }
    }
}
