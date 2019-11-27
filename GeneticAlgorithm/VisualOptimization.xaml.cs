using GeneticAlgorithm.Services;
using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.ComponentModel;
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
        Individual fittest;
        Individual secondFittest;
        int generationCount = 0;
        double mutationRate = 5;

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

        public ChartValues<ObservableValue> MyValues { get; set; }

        public VisualOptimization()
        {
            InitializeComponent();

            MyValues = new ChartValues<ObservableValue>();

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
                Thread.Sleep(250);
                ++generationCount;

                selection();

                crossover();

                if (rn.Next() % 5 < mutationRate)
                {
                    mutation();
                }

                for (int i = 0; i < fittest.genes.Length; i++)
                {
                    genesSum += fittest.genes[i];
                    secGenesSum += secondFittest.genes[i];
                }

                for (int i = 0; i < fittest.genes.Length; i++)
                {
                    // make genes add to 1
                    fittest.genes[i] = (fittest.genes[i] / genesSum) * 1;
                    secondFittest.genes[i] = (secondFittest.genes[i] / secGenesSum) * 1;
                }

                genesSum = 0;
                secGenesSum = 0;

                for (int i = 0; i < fittest.genes.Length; i++)
                {
                    genesSum += fittest.genes[i];
                    secGenesSum += secondFittest.genes[i];
                }

                Console.WriteLine("Genes Sum: " + genesSum);
                Console.WriteLine("SecGenes Sum: " + secGenesSum);

                genesSum = 0;
                secGenesSum = 0;

                addFittestOffspring();

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

        }

        //Selection
        void selection()
        {

            //Select the most fittest individual
            fittest = population.getFittest();

            //Select the second most fittest individual
            secondFittest = population.getSecondFittest();
        }

        //Crossover
        void crossover()
        {
            Random rn = new Random();

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
            int crossOverPoint = rn.Next(10);

            Console.WriteLine("Crossover Point: " + crossOverPoint);

            //Swap values among parents
            for (int i = 0; i < crossOverPoint; i++)
            {
                double temp = fittest.genes[i];
                fittest.genes[i] = secondFittest.genes[i];
                secondFittest.genes[i] = temp;

            }

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

        }

        //Mutation
        void mutation()
        {
            Random rn = new Random();

            //Select a random mutation point
            int mutationPoint = rn.Next(population.individuals[0].geneLength - 1);

            // Console.WriteLine("Fittest Mutation Point: " + mutationPoint);

            //Mutate values at the mutation point
            fittest.genes[mutationPoint] = rn.NextDouble();

            /*
            Console.WriteLine("Fittest Post Mutation: ");

            for (int i = 0; i <= 9; i++)
            {
              	Console.WriteLine(fittest.genes[i]);
            }
            */

            //Mutate values at mutation point
            mutationPoint = rn.Next(population.individuals[0].geneLength - 1);

            //  Console.WriteLine("Second Fittest Mutation Point: " + mutationPoint);

            //Mutate values at the mutation point
            secondFittest.genes[mutationPoint] = rn.NextDouble();

            /*
            Console.WriteLine("Second Fittest Post Mutation: ");

            for (int i = 0; i <= 9; i++)
            {
               	Console.WriteLine(secondFittest.genes[i]);
            }
            */

        }

        //Get fittest offspring
        Individual getFittestOffspring()
        {
            if (fittest.fitness > secondFittest.fitness)
            {
                return fittest;
            }
            return secondFittest;
        }


        //Replace least fittest individual from most fittest offspring
        void addFittestOffspring()
        {

            //Update fitness values of offspring
            fittest.calcFitness();
            secondFittest.calcFitness();

        }


    }
}

