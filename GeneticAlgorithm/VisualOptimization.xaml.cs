using GeneticAlgorithm.Model;
using GeneticAlgorithm.Services;
using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GeneticAlgorithm
{
    /// <summary>
    /// Interaction logic for VisualOptimization.xaml
    /// </summary>
    public partial class VisualOptimization : UserControl, INotifyPropertyChanged
    {
        Population population = new Population();
        public Individual fittest;
        public Individual secondFittest;
        public Individual thirdFittest;
        public Individual fourthFittest;
        public Individual child = new Individual();
        public Individual child2 = new Individual();

        // public RouletteEngine rouletteEngine = new RouletteEngine();

        int generationCount = 0;
        double mutationRate = 5;
        Stopwatch stopWatch = new Stopwatch();

        private double _appl_weight;
        private double _msft_weight;
        private double _amzn_weight;
        private double _nflx_weight;
        private double _fb_weight;
        private double _bxc_weight;
        private double _xom_weight;
        private double _jnj_weight;
        private double _mcd_weight;
        private double _jpm_weight;
        private string _elapsed_time;

        public double Appl_Weight { private set { _appl_weight = value; OnPropertyChanged("Appl_Weight"); } get { return _appl_weight; } }
        public double Msft_Weight { private set { _msft_weight = value; OnPropertyChanged("Msft_Weight"); } get { return _msft_weight; } }
        public double Amzn_Weight { private set { _amzn_weight = value; OnPropertyChanged("Amzn_Weight"); } get { return _amzn_weight; } }
        public double Nflx_Weight { private set { _nflx_weight = value; OnPropertyChanged("Nflx_Weight"); } get { return _nflx_weight; } }
        public double Fb_Weight { private set { _fb_weight = value; OnPropertyChanged("Fb_Weight"); } get { return _fb_weight; } }
        public double Bxc_Weight { private set { _bxc_weight = value; OnPropertyChanged("Bxc_Weight"); } get { return _bxc_weight; } }
        public double Xom_Weight { private set { _xom_weight = value; OnPropertyChanged("Xom_Weight"); } get { return _xom_weight; } }
        public double Jnj_Weight { private set { _jnj_weight = value; OnPropertyChanged("Jnj_Weight"); } get { return _jnj_weight; } }
        public double Mcd_Weight { private set { _mcd_weight = value; OnPropertyChanged("Mcd_Weight"); } get { return _mcd_weight; } }
        public double Jpm_Weight { private set { _jpm_weight = value; OnPropertyChanged("Jpm_Weight"); } get { return _jpm_weight; } }
        public string Elapsed_Time { private set { _elapsed_time = value; OnPropertyChanged("Elapsed_Time"); } get { return _elapsed_time; } }

        public ChartValues<ObservableValue> MyValues { get; set; }

        public VisualOptimization()
        {
            InitializeComponent();

            MyValues = new ChartValues<ObservableValue>();

            Elapsed_Time = "Time: 00:00:00.00";

            this.DataContext = this;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Read(double fittest, int count)
        {
            MyValues.Add(new ObservableValue(fittest));
        }

        private void Mutation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mutationRate = sliderMutation.Value;
        }

        private void StartOptimization(object sender, RoutedEventArgs e)
        {
           // stopWatch.Reset();
            MyValues.Clear();
            Int32.TryParse(PopulationText.Text, out int i);
            Int32.TryParse(OptimalValue.Text, out int j);
            int populationSize = i;
            int optimalValue = j;

            Task.Factory.StartNew(() => optimizePortfolio(populationSize, optimalValue))
                .ContinueWith(t =>
                {
                    Dispatcher.BeginInvoke((Action)delegate () { });

                });
        }

        void optimizePortfolio(int populationSize, int optimalValue)
        {
            Random rn = new Random(10);
            double genesSum = 0;
            double secGenesSum = 0;
            double thirdGenesSum = 0;
            double fourthGenesSum = 0;
            double childGenesSum = 0;
            double child2GenesSum = 0;


            stopWatch.Reset();
            Elapsed_Time = "Time: 00:00:00.00";

            stopWatch.Start();

            population.initializePopulation(populationSize);

            population.calculateFitness();

            Console.WriteLine("Generation: " + generationCount + " Fittest: " + population.fittest + " Second Fittest: " + population.secondFittest + " least fit: " + population.leastFittest);

            /*
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Genes: " + population.getFittest().genes[i]);
            }
            */

            Read(population.fittest, generationCount);

            //Continue genetics until optimal fitness value is found
            while (population.fittest < optimalValue)
            {
                Thread.Sleep(50);
                ++generationCount;

                selection();

                crossover();

                if (rn.Next() % 10 < mutationRate)
                {
                    mutation();
                }

                replacement(populationSize);

                for (int i = 0; i < fittest.genes.Length; i++)
                {
                    genesSum += fittest.genes[i];
                    secGenesSum += secondFittest.genes[i];
                    thirdGenesSum += thirdFittest.genes[i];
                    fourthGenesSum += fourthFittest.genes[i];
                    childGenesSum += child.genes[i];
                    child2GenesSum += child2.genes[i];
                }

                for (int i = 0; i < fittest.genes.Length; i++)
                {
                    // make genes add to 1
                    //fittest.genes[i] = (fittest.genes[i] / genesSum) * 1;
                    //secondFittest.genes[i] = (secondFittest.genes[i] / secGenesSum) * 1;
                    //thirdFittest.genes[i] = (thirdFittest.genes[i] / thirdGenesSum) * 1;
                    //fourthFittest.genes[i] = (fourthFittest.genes[i] / fourthGenesSum) * 1;
                    child.genes[i] = (child.genes[i] / childGenesSum) * 1;
                    child2.genes[i] = (child2.genes[i] / child2GenesSum) * 1;
                }

                genesSum = 0;
                secGenesSum = 0;
                thirdGenesSum = 0;
                fourthGenesSum = 0;
                childGenesSum = 0;
                child2GenesSum = 0;

                for (int i = 0; i < fittest.genes.Length; i++)
                {
                    genesSum += fittest.genes[i];
                    secGenesSum += secondFittest.genes[i];
                    thirdGenesSum += thirdFittest.genes[i];
                    fourthGenesSum += fourthFittest.genes[i];
                    childGenesSum += child.genes[i];
                    child2GenesSum += child2.genes[i];
                }

                Console.WriteLine("Genes Sum: " + genesSum);
                Console.WriteLine("SecGenes Sum: " + secGenesSum);
                Console.WriteLine("ThirdGenes Sum: " + thirdGenesSum);
                Console.WriteLine("FourthGenes Sum: " + fourthGenesSum);
                Console.WriteLine("Child Genes Sum: " + childGenesSum);
                Console.WriteLine("Child2 Genes Sum: " + child2GenesSum);


                genesSum = 0;
                secGenesSum = 0;
                thirdGenesSum = 0;
                fourthGenesSum = 0;
                childGenesSum = 0;
                child2GenesSum = 0;

                updateFitnessValues();

                population.calculateFitness();

                Console.WriteLine("Generation: " + generationCount + " Fittest: " + population.fittest + " Second Fittest: " + population.secondFittest);

                Appl_Weight = population.getFittest().genes[0] * 100;
                Msft_Weight = population.getFittest().genes[1] * 100;
                Amzn_Weight = population.getFittest().genes[2] * 100;
                Nflx_Weight = population.getFittest().genes[3] * 100;
                Fb_Weight = population.getFittest().genes[4] * 100;
                Bxc_Weight = population.getFittest().genes[5] * 100;
                Xom_Weight = population.getFittest().genes[6] * 100;
                Jnj_Weight = population.getFittest().genes[7] * 100;
                Mcd_Weight = population.getFittest().genes[8] * 100;
                Jpm_Weight = population.getFittest().genes[9] * 100;

                /*
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Genes: " + population.getFittest().genes[i]);
                }
                */

                //Update chart
                Read(population.fittest, generationCount);

                Console.WriteLine("\n\n");

            }

            Console.WriteLine("\nSolution found in generation " + generationCount);
            Console.WriteLine("Fitness: " + population.getFittest().fitness);
            Console.WriteLine("Genes: ");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(population.getFittest().genes[i]);
            }

            Console.WriteLine("");

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;

            Elapsed_Time = "Time: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
               ts.Hours, ts.Minutes, ts.Seconds,
               ts.Milliseconds / 10);
        }

        //Selection
        void selection()
        {
            //Select the most fittest individual
            fittest = population.getFittest();

            //Select the second most fittest individual
            secondFittest = population.getSecondFittest();

            thirdFittest = population.getThirdFittest();

            fourthFittest = population.getFourthFittest();
        }

        //Crossover
        void crossover()
        {
            Random rn = new Random();
            List<double> tempList = new List<double>();
            List<double> tempList2 = new List<double>();


            /*
            Console.WriteLine("Pre Crossover Reults:");

            Console.WriteLine("Fittest Genes:");

            for (int i = 0; i <= 9; i++)
            {
               	Console.WriteLine(fittest.genes[i]);
            }

            Console.WriteLine("Second Fittest Genes:");

            for (int i = 0; i <= 9; i++)
            {
               	Console.WriteLine(secondFittest.genes[i]);
            }
            */

            //Select a random crossover point
            int crossOverPoint = rn.Next(9);

            // Console.WriteLine("Crossover Point: " + crossOverPoint);

            //Swap values among parents
            /*
            for (int i = 0; i < crossOverPoint; i++)
            {
                double temp = fittest.genes[i];
                fittest.genes[i] = secondFittest.genes[i];
                secondFittest.genes[i] = temp;

            }
            */

            //Select a random crossover point
            // int crossOverPoint2 = rn.Next(9);

            // Console.WriteLine("Crossover Point: " + crossOverPoint);

            //Swap values among parents
            /*
            for (int i = 0; i < crossOverPoint2; i++)
            {
                double temp = thirdFittest.genes[i];
                thirdFittest.genes[i] = fourthFittest.genes[i];
                fourthFittest.genes[i] = temp;
            }
            */

            /*
            Console.WriteLine("Post Crossover Reults:");

            Console.WriteLine("Fittest Genes:");

            for (int i = 0; i <= 9; i++)
            {
             	Console.WriteLine(fittest.genes[i]);
            }

            Console.WriteLine("Second Fittest Genes:");

            for (int i = 0; i <= 9; i++)
            {
              	Console.WriteLine(secondFittest.genes[i]);
            }
            */

            for (int i = 0; i < crossOverPoint; i++)
            {
                tempList.Add(fittest.genes[i]);
            }

            for (int i = crossOverPoint; i < 10; i++)
            {
                tempList.Add(secondFittest.genes[i]);
            }

            int crossOverPoint2 = rn.Next(9);

            for (int i = 0; i < crossOverPoint2; i++)
            {
                tempList2.Add(thirdFittest.genes[i]);
            }

            for (int i = crossOverPoint2; i < 10; i++)
            {
                tempList2.Add(fourthFittest.genes[i]);
            }

            child.genes = tempList.ToArray();
            child2.genes = tempList2.ToArray();

            /*
            Console.WriteLine("List Genes:");

            for (int i = 0; i <= 9; i++)
            {
                Console.WriteLine(tempList[i]);
            }

            child.genes = tempList.ToArray();

            Console.WriteLine("Child Genes:");

            for (int i = 0; i <= 9; i++)
            {
                Console.WriteLine(child.genes[i]);
            }
            */
        }

        //Mutation
        void mutation()
        {
            Random rn = new Random();

            //Select a random mutation point
            int mutationPoint = rn.Next(population.individuals[0].geneLength - 1);

            // Console.WriteLine("Fittest Mutation Point: " + mutationPoint);

            //Mutate values at the mutation point
            child.genes[mutationPoint] = rn.NextDouble();

            //Select a random mutation point
            int mutationPoint2 = rn.Next(population.individuals[0].geneLength - 1);

            // Console.WriteLine("Fittest Mutation Point: " + mutationPoint);

            //Mutate values at the mutation point
            child2.genes[mutationPoint2] = rn.NextDouble();

            /*
            Console.WriteLine("Fittest Post Mutation: ");

            for (int i = 0; i <= 9; i++)
            {
              	Console.WriteLine(fittest.genes[i]);
            }
            */

            //Mutate values at mutation point
            // mutationPoint = rn.Next(population.individuals[0].geneLength - 1);

            //  Console.WriteLine("Second Fittest Mutation Point: " + mutationPoint);

            //Mutate values at the mutation point
            // secondFittest.genes[mutationPoint] = rn.NextDouble();

            /*
            Console.WriteLine("Second Fittest Post Mutation: ");

            for (int i = 0; i <= 9; i++)
            {
               	Console.WriteLine(secondFittest.genes[i]);
            }
            */

            /*
            mutationPoint = rn.Next(population.individuals[0].geneLength - 1);
            thirdFittest.genes[mutationPoint] = rn.NextDouble();

            mutationPoint = rn.Next(population.individuals[0].geneLength - 1);
            fourthFittest.genes[mutationPoint] = rn.NextDouble();
            */


        }

        void updateFitnessValues()
        {
            //Update fitness values of offspring
            fittest.calcFitness();
            secondFittest.calcFitness();
            thirdFittest.calcFitness();
            fourthFittest.calcFitness();
            child.calcFitness();
           // child2.calcFitness();
        }

        //Replace least fittest individual from most fittest offspring
        void replacement(int populationSize)
        {
            int replaceIndex = 0;

            replaceIndex = population.getLeastFittestIndex();

            population.individuals[replaceIndex] = child;
            /*
            Random rn = new Random();
            int replaceIndex2 = rn.Next(populationSize - 1);
            population.individuals[replaceIndex2] = child2;
            */



        }

    }
}

