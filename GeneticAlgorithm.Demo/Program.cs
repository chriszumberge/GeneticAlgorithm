//
// Program.cs
//
//
// Author:
//       Chris Zumberge <chriszumberge@gmail.com>
//
// Copyright (c) 2017 Christopher Zumberge
//
// All rights reserved
//
using System;
using System.Linq;

namespace GeneticAlgorithm.Demo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            GeneticAlgorithm ga = new GeneticAlgorithm(new Random(), 100, Params.MutationRate, Params.CrossoverRate, 10);

            int maxGenerations = 100;

            for (int i = 0; i < maxGenerations; i++)
            {
                foreach (Genome genome in ga.GetChromosomes())
                {
                    double sum = genome.weights.Sum();

                    genome.fitness = Math.Abs(1 / (10 - sum));

                    //try
                    //{
                    //    genome.fitness = 1 / (10 - sum);
                    //}
                    //catch (DivideByZeroException dze)
                    //{
                    //    genome.fitness = 10000000;
                    //    i = maxGenerations;
                    //    break;
                    //}
                }

                Console.WriteLine("B: " + ga.GetBestFitness() + " | A: " + ga.GetAverageFitness());
                ga.Epoch(ga.GetChromosomes());
            }

            Console.ReadLine();
        }
    }
}
