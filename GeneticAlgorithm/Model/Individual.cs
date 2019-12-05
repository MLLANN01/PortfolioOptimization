using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeneticAlgorithm.Model
{
    public class Individual
    {
        public double fitness = 0;
        public double[] genes = new double[10];
        public int geneLength = 10;
        double genesSum = 0;

        string appleFile = "../../../Data\\Apple.csv";
        string microsoftFile = "../../../Data\\Microsoft.csv";
        string amazonFile = "../../../Data\\Amazon.csv";
        string netflixFile = "../../../Data\\Netflix.csv";
        string facebookFile = "../../../Data\\Facebook.csv";
        string blueLinxFile = "../../../Data\\BlueLinx.csv";
        string exxonMobilFile = "../../../Data\\ExxonMobil.csv";
        string johnsonFile = "../../../Data\\Johnson.csv";
        string jpMorganFile = "../../../Data\\JPMorgan.csv";
        string mcdonaldFile = "../../../Data\\McDonald.csv";

        double[] appleReturns = new double[40];
        double[] microsoftReturns = new double[40];
        double[] amazonReturns = new double[40];
        double[] netflixReturns = new double[40];
        double[] facebookReturns = new double[40];

        double[] blueLinxReturns = new double[40];
        double[] exxonMobileReturns = new double[40];
        double[] johnsonReturns = new double[40];
        double[] jpMorganReturns = new double[40];
        double[] mcdonaldReturns = new double[40];

        public Individual()
        {
            Random rand = new Random();
            fitness = 0;

            //Set genes randomly for each individual
            for (int i = 0; i < genes.Length; i++)
            {
                genes[i] = rand.NextDouble();
                genesSum += genes[i];
            }

            for (int i = 0; i < genes.Length; i++)
            {
                // make genes add to 1
                genes[i] = (genes[i] / genesSum) * 1;
                //     Console.WriteLine("Gene: " + genes[i]);
            }

            //  Console.WriteLine("Next");

            genesSum = 0;

        }

        //Calculate fitness
        public void calcFitness()
        {

            appleReturns = getReturns(appleFile);
            microsoftReturns = getReturns(microsoftFile);
            amazonReturns = getReturns(amazonFile);
            netflixReturns = getReturns(netflixFile);
            facebookReturns = getReturns(facebookFile);
            blueLinxReturns = getReturns(blueLinxFile);
            exxonMobileReturns = getReturns(exxonMobilFile);
            johnsonReturns = getReturns(johnsonFile);
            jpMorganReturns = getReturns(jpMorganFile);
            mcdonaldReturns = getReturns(mcdonaldFile);

            fitness = 0;
            double[] port_ret = new double[40];
            double sharpe = 0;
            double mean = 0;
            double variance = 0;
            double portSum = 0;
            double temp = 0;

            // Calculate portfolio return for a given entry
            for (int i = 0; i <= 39; i++)
            {
                port_ret[i] = appleReturns[i] * genes[0] + 
                    microsoftReturns[i] * genes[1] + 
                    amazonReturns[i] * genes[2] + 
                    netflixReturns[i] * genes[3] + 
                    facebookReturns[i] * genes[4] + 
                    blueLinxReturns[i] * genes[5] + 
                    exxonMobileReturns[i] * genes[6] + 
                    johnsonReturns[i] * genes[7] + 
                    mcdonaldReturns[i] * genes[8] + 
                    jpMorganReturns[i] * genes[9];
            }

            // Get the sum of all portfolio returns
            for (int i = 0; i <= 39; i++)
            {
                portSum += port_ret[i];
            }

            // Calculate mean of portfolio returns
            mean = portSum / 40;

            // calculate variance
            for (int i = 0; i <= 39; i++)
            {
                temp += (port_ret[i] - mean) * (port_ret[i] - mean);
            }

            // calculate standard deviation
            variance = (float)Math.Sqrt(temp / 40);

            //Calculate Sharpe Ratio
            sharpe = (float)(mean / variance) * 100;

            fitness = sharpe;

            // Console.WriteLine("Fitness: " + fitness);

        }

        public static double[] getReturns(String file)
        {
            var reader = new StreamReader(File.OpenRead(file));

            double[] Returns = new double[40];
            double open = 0;
            double close = 0;
            double dayReturn = 0;

            //read first line
            reader.ReadLine();

            for (int i = 0; i <= 39; i++)
            {
                var line = reader.ReadLine();
                var value = line.Split("\t");

                Double.TryParse(value[3], out open);
                Double.TryParse(value[4], out close);

                dayReturn = close - open;

                Returns[i] = dayReturn;

                open = 0;
                close = 0;
                dayReturn = 0;

            }

            return Returns;
        }
    }
}
