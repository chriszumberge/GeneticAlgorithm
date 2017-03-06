using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public static class GAHelpers
    {
        public static void Crossover(Random random, ref Chromosome chromosome1, ref Chromosome chromosome2, double crossoverRate)
        {
            int chromosomeLength = chromosome1.NumGenes;

            if (random.NextDouble() < crossoverRate)
            {
                // create a random crossover point
                int crossover = (int)(random.NextDouble() * chromosomeLength);

                List<Gene> gene1 = chromosome1.Genes.GetRange(0, crossover).Concat(chromosome2.Genes.GetRange(crossover, chromosomeLength - crossover)).ToList();
                List<Gene> gene2 = chromosome2.Genes.GetRange(0, crossover).Concat(chromosome1.Genes.GetRange(crossover, chromosomeLength - crossover)).ToList();

                chromosome1.SetGenes(gene1);
                chromosome2.SetGenes(gene2);
            }
        }

        public static void Mutate(Random random, ref Chromosome chromosome, double mutationRate)
        {
            for (int i = 0; i < chromosome.NumGenes; i++)
            {
                Gene gene = chromosome.Genes[i];

                for (int j = 0; j < gene.GeneLength; j++)
                {
                    if (random.NextDouble() < mutationRate)
                    {
                        gene.Mutate(j);
                    }
                }
            }
        }
    }
}
