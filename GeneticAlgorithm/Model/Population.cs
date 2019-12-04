using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm.Model
{
    public class Population
    {
        public Individual[] individuals;
        public double fittest = 0;
        public double secondFittest = 0;
        public double leastFittest = 0;
        public double thirdFittest = 0;
        public double fourthFittest = 0;

        //Initialize population
        public void initializePopulation(int size)
        {
            individuals = new Individual[size];

            for (int i = 0; i < size; i++)
            {
                individuals[i] = new Individual();
            }
        }


        //Get the fittest individual
        public Individual getFittest()
        {
            double maxFit = double.MinValue;
            int maxFitIndex = 0;
            for (int i = 0; i < individuals.Length; i++)
            {
                if (maxFit <= individuals[i].fitness)
                {
                    maxFit = individuals[i].fitness;
                    maxFitIndex = i;
                }
            }

            fittest = individuals[maxFitIndex].fitness;

            return individuals[maxFitIndex];
        }

        //Get the second most fittest individual
        public Individual getSecondFittest()
        {
            int maxFit1 = 0;
            int maxFit2 = 0;
            for (int i = 0; i < individuals.Length; i++)
            {
                if (individuals[i].fitness > individuals[maxFit1].fitness)
                {
                    maxFit2 = maxFit1;
                    maxFit1 = i;
                }
                else if (individuals[i].fitness > individuals[maxFit2].fitness)
                {
                    maxFit2 = i;
                }
            }

            secondFittest = individuals[maxFit2].fitness;

            return individuals[maxFit2];
        }

        //Get the third most fittest individual
        public Individual getThirdFittest()
        {
            int maxFit1 = 0;
            int maxFit2 = 0;

            for (int i = 0; i < individuals.Length; i++)
            {
                if (individuals[i].fitness > secondFittest)
                {

                }
                else if (individuals[i].fitness > individuals[maxFit1].fitness)
                {
                    maxFit2 = maxFit1;
                    maxFit1 = i;
                }
                else if (individuals[i].fitness > individuals[maxFit2].fitness)
                {
                    maxFit2 = i;
                }
            }

            thirdFittest = individuals[maxFit2].fitness;

            return individuals[maxFit2];
        }

        public Individual getFourthFittest()
        {
            int maxFit1 = 0;
            int maxFit2 = 0;

            for (int i = 0; i < individuals.Length; i++)
            {
                if (individuals[i].fitness > thirdFittest)
                {

                }
                else if (individuals[i].fitness > individuals[maxFit1].fitness)
                {
                    maxFit2 = maxFit1;
                    maxFit1 = i;
                }
                else if (individuals[i].fitness > individuals[maxFit2].fitness)
                {
                    maxFit2 = i;
                }
            }

            fourthFittest = individuals[maxFit2].fitness;

            return individuals[maxFit2];
        }

        //Get index of least fittest individual
        public int getLeastFittestIndex()
        {
            double minFitVal = double.MaxValue;
            int minFitIndex = 0;
            for (int i = 0; i < individuals.Length; i++)
            {
                if (minFitVal >= individuals[i].fitness)
                {
                    minFitVal = individuals[i].fitness;
                    minFitIndex = i;
                }
            }

            leastFittest = individuals[minFitIndex].fitness;

            return minFitIndex;
        }

        //Calculate fitness of each individual
        public void calculateFitness()
        {

            for (int i = 0; i < individuals.Length; i++)
            {
                individuals[i].calcFitness();
            }
            getFittest();
            getSecondFittest();
            getLeastFittestIndex();
        }
    }
}
