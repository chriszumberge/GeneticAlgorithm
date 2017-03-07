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
using GeneticAlgorithm.Examples;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm.Demo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //RunGeneticAlgorithm();
            //RunGATutorial();

            RunObjOrientedGA();
            Console.WriteLine();
        }

        private static void RunObjOrientedGA()
        {
            Random random = new Random();

            Population<MathematicGenome> population = new Population<MathematicGenome>(random, 100, (r) => { return new MathematicGenome(r); });

            int GenerationsRequiredToFindASolution = 0;

            bool bFound = false;

            while (!bFound)
            {
                population.CalculateFitness();

                foreach (MathematicGenome genome in population.Members)
                {
                    if (genome.Fitness == 999.0)
                    {
                        Console.WriteLine(String.Concat("Solution found in ", GenerationsRequiredToFindASolution, " generations!"));
                        Console.WriteLine(String.Join(String.Empty, genome.Chromosomes.First().DecodeChromosome()));
                        bFound = true;
                        break;
                    }
                }

                List<MathematicGenome> newPopulation = new List<MathematicGenome>();
                int cPop = 0;

                while (cPop < GATutorial.POP_SIZE)
                {
                    Genome parent1 = population.Roulette();
                    Genome parent2 = population.Roulette();

                    Genome offspring1;
                    Genome offspring2;

                    parent1.Reproduce(parent2, out offspring1, out offspring2, 0.7, 0.001);

                    newPopulation.Add(offspring1 as MathematicGenome);
                    newPopulation.Add(offspring2 as MathematicGenome);
                    cPop += 2;
                }

                ++GenerationsRequiredToFindASolution;

                // exit app if no solution found within the maximum allowable number of generations
                if (GenerationsRequiredToFindASolution > GATutorial.MAX_ALLOWABLE_GENERATIONS)
                {
                    Console.WriteLine("No solutions found this run!");
                    bFound = true;
                }
            }

            //Console.ReadLine();
        }

        private static void RunGATutorial()
        {
            while (true)
            {
                GATutorial.Chromosome[] Population = new GATutorial.Chromosome[GATutorial.POP_SIZE];

                // get a target number from the user
                double target;
                Console.WriteLine("Input a target number: ");
                target = Double.Parse(Console.ReadLine());
                Console.WriteLine(); Console.WriteLine();

                GATutorial ga = new GATutorial();
                // first create a random population, all with zero fitness
                for (int i = 0; i < GATutorial.POP_SIZE; i++)
                {
                    Population[i] = new GATutorial.Chromosome();
                    Population[i].bits = ga.GetRandomBits(GATutorial.CHROMOSOME_LENGTH);
                    Population[i].fitness = 0.0f;
                }

                int GenerationsRequiredToFindASolution = 0;

                // we will set this flag if a solution has been found
                bool bFound = false;

                // enter the main GA loop
                while (!bFound)
                {
                    // this is used during roulette wheel sampling
                    double TotalFitness = 0.0;

                    // test and update the fitness of every chromosome in the population
                    for (int i = 0; i < GATutorial.POP_SIZE; i++)
                    {
                        Population[i].fitness = ga.AssignFitness(Population[i].bits, target);
                        TotalFitness += Population[i].fitness;
                    }

                    // check to see if we have found any solutions (fitness == 999)
                    for (int i = 0; i < GATutorial.POP_SIZE; i++)
                    {
                        if (Population[i].fitness == 999.0)
                        {
                            Console.WriteLine(String.Concat("Solution found in ", GenerationsRequiredToFindASolution, " generations!"));
                            GATutorial.PrintChromosome(Population[i].bits);
                            Console.WriteLine();
                            bFound = true;
                            break;
                        }
                    }

                    // create a new population by selecting two parents at a time and creating offspring
                    // by applying corssover and mutation. do this until the desired number of offspring have been created

                    // define some type of temporary storage for the new population we are about to create
                    GATutorial.Chromosome[] temp = new GATutorial.Chromosome[GATutorial.POP_SIZE];

                    int cPop = 0;

                    // loop until we have created new chromosomes for the whole population
                    while (cPop < GATutorial.POP_SIZE)
                    {
                        // we are going to create the new population by grabbing members of the old population
                        // two at a time via roulette wheel selection
                        string offspring1 = GATutorial.Roulette(TotalFitness, Population);
                        string offspring2 = GATutorial.Roulette(TotalFitness, Population);

                        // add crossover dependent on the crossover rate
                        GATutorial.Crossover(ref offspring1, ref offspring2);

                        // now mutate dependent on the mutation rate
                        GATutorial.Mutate(ref offspring1);
                        GATutorial.Mutate(ref offspring2);

                        // add these offpsring to the new population (assigning zero as their default fitness scores)
                        temp[cPop++] = new GATutorial.Chromosome(offspring1, 0.0);
                        temp[cPop++] = new GATutorial.Chromosome(offspring2, 0.0);
                    }

                    // copy temp population into main population array
                    for (int i = 0; i < GATutorial.POP_SIZE; i++)
                    {
                        Population[i] = temp[i];
                    }

                    ++GenerationsRequiredToFindASolution;

                    // exit app if no solution found within the maximum allowable number of generations
                    if (GenerationsRequiredToFindASolution > GATutorial.MAX_ALLOWABLE_GENERATIONS)
                    {
                        Console.WriteLine("No solutions found this run!");
                        bFound = true;
                    }
                }
            }
        }

        private static void RunGeneticAlgorithm()
        {
            GAExample ga = new GAExample(new Random(), 100, Params.MutationRate, Params.CrossoverRate, 10);

            int maxGenerations = 100;

            for (int i = 0; i < maxGenerations; i++)
            {
                foreach (GAExample.Genome genome in ga.GetChromosomes())
                {
                    double sum = genome.weights.Sum();

                    genome.fitness = Math.Abs(1 / (10 - sum));
                }

                Console.WriteLine("B: " + ga.GetBestFitness() + " | A: " + ga.GetAverageFitness());
                ga.Epoch(ga.GetChromosomes());
            }

            Console.ReadLine();
        }
    }
}
