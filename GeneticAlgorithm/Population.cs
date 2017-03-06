using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public sealed class Population<GenomeType> where GenomeType : Genome
    {
        int mPopulationSize { get; set; }
        public int PopulationSize => mPopulationSize;

        public List<GenomeType> Members => mMembers;
        List<GenomeType> mMembers { get; set; } = new List<GenomeType>();

        Random mRandom { get; set; }

        double mTotalFitness { get; set; } = 0;

        public Population(List<GenomeType> newPopulation)
        {
            mMembers = newPopulation;
        }

        public Population(Random random, int populationSize, Func<Random, GenomeType> genomeCreator)
        {
            mRandom = random;

            for (int i = 0; i < populationSize; i++)
            {
                mMembers.Add(genomeCreator.Invoke(random));
            }
        }

        public void CalculateFitness()
        {
            foreach (GenomeType genome in mMembers)
            {
                genome.CalculateFitness();
                mTotalFitness += genome.Fitness;
            }
        }

        public GenomeType Roulette()
        {
            // generate random number between 0 and fitness count
            double slice = mRandom.NextDouble() * mTotalFitness;

            // go through the chromosomes adding up the fitness so far
            double fitnessSoFar = 0.0;

            for (int i = 0; i < mPopulationSize; i++)
            {
                fitnessSoFar += mMembers[i].Fitness;

                // if the fitness so far > random number return the chromosome at this point
                if (fitnessSoFar >= slice)
                {
                    return mMembers[i];
                }
            }

            return mMembers.Last();
        }
    }
}
