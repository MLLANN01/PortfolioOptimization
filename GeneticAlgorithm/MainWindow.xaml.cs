using GeneticAlgorithm.Services;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Windows;
using GeneticAlgorithm.Model;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Population population = new Population();
        Individual fittest;
        Individual secondFittest;
        int generationCount = 0;
        int populationSize = 75;

        public SeriesCollection SeriesCollection { get; set; }

        readonly ChartValues<double> FitnessValues = new ChartValues<double>();
        List<int> generations = new List<int>();

        public MainWindow()
        {
            InitializeComponent();


            // CartesianChart ch = new CartesianChart();
            //AxisY.Clear();
            //AxisY.Add(new Axis { MaxValue = 30, MinValue = 0 });
            /*
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Fitness",
                    Values = FitnessValues
                }
            };
            */

            //  TestGrid.Children.Add(ch);

            DataContext = this;

            

        }

        private void StartOptimization(object sender, RoutedEventArgs e)
        {
            optimizePortfolio(populationSize);
        }

        void optimizePortfolio(int populationSize)
        {
            Random rn = new Random(10);
            double genesSum = 0;
            double secGenesSum = 0;

            //Initalize population
            population.initializePopulation(populationSize);

            population.calculateFitness();

            Console.WriteLine("Generation: " + generationCount + " Fittest: " + population.fittest + " Second Fittest: " + population.secondFittest + " least fit: " + population.leastFittest);

            /*
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Genes: " + population.getFittest().genes[i]);
            }
            */

            FitnessValues.Add(population.fittest);
            generations.Add(generationCount);

            //While population gets an individual with maximum fitness
            while (population.fittest < 18)
            {
                ++generationCount;

                //Do selection
                selection();

                //Do crossover
                crossover();

                //Do mutation under a random probability
                if (rn.Next() % 5 < 5)
                {
                    mutation();
                }

                //Set genes randomly for each individual
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
                    // make genes add to 1
                    genesSum += fittest.genes[i];
                    secGenesSum += secondFittest.genes[i];
                }

                Console.WriteLine("Genes Sum: " + genesSum);
                Console.WriteLine("SecGenes Sum: " + secGenesSum);

                genesSum = 0;
                secGenesSum = 0;

                //Add fittest offspring to population
                addFittestOffspring();

                //Calculate new fitness value
                population.calculateFitness();

                Console.WriteLine("Generation: " + generationCount + " Fittest: " + population.fittest + " Second Fittest: " + population.secondFittest);

                /*
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Genes: " + population.getFittest().genes[i]);
                }
                */


                //modifying the series collection will animate and update the chart
                FitnessValues.Add(population.fittest);
                generations.Add(generationCount);

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

            mutationPoint = rn.Next(population.individuals[0].geneLength - 1);

          //  Console.WriteLine("Second Fittest Mutation Point: " + mutationPoint);

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

            //Get index of least fit individual
            //   int leastFittestIndex = population.getLeastFittestIndex();

            //  Console.WriteLine("LeastFit Index: " + leastFittestIndex);

            //Replace least fittest individual from most fittest offspring
            // population.individuals[leastFittestIndex] = getFittestOffspring();
        }

    }
}
