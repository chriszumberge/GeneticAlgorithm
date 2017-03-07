//
// GeneticAlgorithm.cs
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
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm.Examples
{
    public class GAExample
    {
        // holds the entire population of genomes
        List<Genome> mPopulation { get; set; }
        int mPopSize { get; set; }
        // amount of weights per chromosome
        int mChromoLength { get; set; }
        double mTotalFitness { get; set; }
        double mBestFitness { get; set; }
        double mAverageFitness { get; set; }
        double mWorstFitness { get; set; }
        // tracks the best genome
        int mFittestGenome { get; set; }

        // probability that a chromosome's bits will mutate
        // 0.05 - 0.3ish
        double mMutationRate { get; set; }
        // probability of chromosomes crossing over bits
        // 0.7 is pretty good
        double mCrossoverRate { get; set; }

        // generation counter
        int mGeneration { get; set; }

        Random mRandom { get; set; }

        public GAExample(Random random, int popSize, double mutRate, double crossRate, int numWeights)
        {
            mPopSize = popSize;
            mMutationRate = mutRate;
            mCrossoverRate = crossRate;
            mChromoLength = numWeights;
            mTotalFitness = 0;
            mGeneration = 0;
            mFittestGenome = 0;
            mBestFitness = 0;
            mWorstFitness = 99999999;
            mAverageFitness = 0;

            mRandom = random;

            mPopulation = new List<Genome>();

            // initialize population with chromosomes consisting of random
            // weights and all fitnesses set to zero
            for (int i = 0; i < mPopSize; ++i)
            {
                mPopulation.Add(new Genome());

                for (int j = 0; j < mChromoLength; ++j)
                {
                    mPopulation[i].weights.Add((random.NextDouble() * 2) - 1);
                }
            }
        }

        // Crossover
        private void Crossover(List<double> mom, List<double> dad, ref List<double> child1, ref List<double> child2)
        {
            // just return parents as offspring dependent on the rate
            // or if parents are the same
            if ((mRandom.NextDouble() > mCrossoverRate) || (mom == dad))
            {
                child1 = mom;
                child2 = dad;

                return;
            }

            // determine a crossover point
            int cp = mRandom.Next(0, mChromoLength - 1);

            // create offspring
            for (int i = 0; i < cp; i++)
            {
                child1.Add(mom[i]);
                child2.Add(dad[i]);
            }

            for (int i = cp; i < mom.Count; i++)
            {
                child1.Add(dad[i]);
                child2.Add(mom[i]);
            }

            return;
        }

        // Mutate
        private void Mutate(List<double> chromo)
        {
            // traverse the chromosome and mutate each weight dependent
            // on the mutation rate
            for (int i = 0; i < chromo.Count; i++)
            {
                // do we change this weight?
                if (mRandom.NextDouble() < mMutationRate)
                {
                    // add or subtract a small value to the weight
                    chromo[i] += (((mRandom.NextDouble() * 2) - 1) * Params.MaxPerturbation);
                }
            }
        }

        // GetChromoRoulette
        private Genome GetChromoRoulette()
        {
            // generate a random number between 0 and total fitness count
            double slice = (double)(mRandom.NextDouble() * mTotalFitness);

            // this will be set to the chosen chromosome
            Genome TheChosenOne = default(Genome);

            // go through the chromosomes adding up the fitness so far
            double FitnessSoFar = 0;

            for (int i = 0; i < mPopSize; i++)
            {
                FitnessSoFar += mPopulation[i].fitness;

                // if the fitness so far > random number return the chromo at
                // this point
                if (FitnessSoFar >= slice)
                {
                    TheChosenOne = mPopulation[i];
                    break;
                }
            }

            return TheChosenOne;
        }

        // GrabNBest


        // CalculateBestWorstAvTot
        private void CalcuateBestWorstAvTot()
        {
            mTotalFitness = 0;

            double highestSoFar = 0;
            double lowestSoFar = 99999999;

            for (int i = 0; i < mPopSize; i++)
            {
                // update fitness if necessary
                if (mPopulation[i].fitness > highestSoFar)
                {
                    highestSoFar = mPopulation[i].fitness;
                    mFittestGenome = i;
                    mBestFitness = highestSoFar;
                }

                // updat worst if necessary
                if (mPopulation[i].fitness < lowestSoFar)
                {
                    lowestSoFar = mPopulation[i].fitness;
                    mWorstFitness = lowestSoFar;
                }

                mTotalFitness += mPopulation[i].fitness;
            } // next chromosome

            mAverageFitness = mTotalFitness / mPopSize;
        }

        // Reset
        private void Reset()
        {
            mTotalFitness = 0;
            mBestFitness = 0;
            mWorstFitness = 99999999;
            mAverageFitness = 0;
        }

        // This runs the GA for one generation
        public List<Genome> Epoch(List<Genome> oldPopulation)
        {
            mPopulation = oldPopulation;

            Reset();

            mPopulation = mPopulation.OrderBy(x => x.fitness).ToList();

            CalcuateBestWorstAvTot();

            List<Genome> listNewPop = new List<Genome>();

            if ((Params.NumCopiesElite * Params.NumElite % 2) == 0)
            {
                GrabNBest(Params.NumElite, Params.NumCopiesElite, ref listNewPop);
            }

            // now we enter the GA loop

            // repeat until a new population is generated
            while (listNewPop.Count < mPopSize)
            {
                // grab two chromosomes
                Genome mom = GetChromoRoulette();
                Genome dad = GetChromoRoulette();

                // create some offspring via crossover
                List<double> child1 = new List<double>(), child2 = new List<double>();
                Crossover(mom.weights, dad.weights, ref child1, ref child2);

                // now we mutate
                Mutate(child1);
                Mutate(child2);

                // now copy into listNewPop back into mPopulation
                listNewPop.Add(new Genome(child1, 0));
                listNewPop.Add(new Genome(child2, 0));
            }

            // finished so assign new pop back into mPopulation
            mPopulation = listNewPop;

            return mPopulation;
        }

        public void GrabNBest(int nBest, int numCopies, ref List<Genome> population)
        {
            // add to the required amount of copies of the n most fittest
            // to the supplied vector
            while (nBest-- > 0)
            {
                for (int i = 0; i < numCopies; i++)
                {
                    population.Add(mPopulation[(mPopSize - 1) - nBest]);
                }
            }
        }

        public List<Genome> GetChromosomes()
        {
            return mPopulation;
        }

        public double GetAverageFitness()
        {
            return mTotalFitness / mPopSize;
        }

        public double GetBestFitness()
        {
            return mBestFitness;
        }

        public class Genome
        {
            public List<double> weights = new List<double>();

            public double fitness = 0;

            public Genome() { }

            public Genome(List<double> wgts, double ftns)
            {
                weights = wgts;
                fitness = ftns;
            }
        }
    }
}
