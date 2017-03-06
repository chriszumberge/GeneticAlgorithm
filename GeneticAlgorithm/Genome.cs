using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class Genome
    {
        protected double mFitness { get; set; } = 0;
        public double Fitness { get { return mFitness; } }

        List<Chromosome> mChromosomes { get; set; } = new List<Chromosome>();
        public List<Chromosome> Chromosomes { get { return mChromosomes; } }

        Random mRandom { get; set; }

        public Genome(Random random, List<Type> chromosomeTypes)
        {
            mRandom = random;

            if (chromosomeTypes.Count != chromosomeTypes.Distinct().Count())
            {
                throw new ArgumentException("A genome can only contain one instance of any given type of chromosome.");
            }

            foreach (Type type in chromosomeTypes)
            {
                if (!type.IsSubclassOf(typeof(Chromosome)))
                {
                    throw new ArgumentException("All types must inherit the chromosome class.");
                }

                mChromosomes.Add((Chromosome)Activator.CreateInstance(type, random));
            }
        }

        public List<Chromosome> GetChromosomes() => mChromosomes;

        public abstract void CalculateFitness();

        public void Reproduce(Genome otherParent, out Genome offspring1, out Genome offspring2, double crossoverRate, double mutationRate)
        {
            offspring1 = this;
            offspring2 = otherParent;

            for (int i = 0; i < offspring1.Chromosomes.Count; i++)
            {
                Chromosome chromosome1 = offspring1.Chromosomes[i];
                Chromosome chromosome2 = offspring2.Chromosomes.Where(x => x.GetType().Equals(chromosome1.GetType())).FirstOrDefault();

                if (chromosome2 != null)
                {
                    GAHelpers.Crossover(mRandom, ref chromosome1, ref chromosome2, crossoverRate);

                    GAHelpers.Mutate(mRandom, ref chromosome1, mutationRate);
                    GAHelpers.Mutate(mRandom, ref chromosome2, mutationRate);
                }
            }
        }
    }
}
