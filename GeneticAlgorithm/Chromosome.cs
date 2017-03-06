using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class Chromosome// where GeneType : Gene
    {
        public Type GeneType { get { return mGeneType; } }
        Type mGeneType { get; set; }

        public List<Gene> Genes { get { return mGenes; } }
        List<Gene> mGenes { get; set; }

        public int NumGenes => mGenes.Count;

        Random mRandom { get; set; }

        public Chromosome(Random random, Type geneType, int numGenes)
        {
            mGeneType = geneType;
            mRandom = random;
            mGenes = new List<Gene>();

            for (int i = 0; i < numGenes; i++)
            {
                Gene gene = GetRandomGene(mRandom);
                if (!gene.GetType().IsSubclassOf(typeof(Gene)) || !gene.GetType().IsEquivalentTo(mGeneType))
                {
                    throw new Exception("Crated random gene isn't of acceptible type.");
                }
                mGenes.Add(gene);
            }
        }

        public Chromosome(Type geneType, List<Gene> genes)
        {
            mGeneType = geneType;

            if (genes.Any(x => !(x.GetType().IsAssignableFrom(typeof(Gene))) ||
                               !(x.GetType().IsAssignableFrom(mGeneType))))
            {
                throw new ArgumentException("Gene list contains incompatible type");
            }

            mGenes = genes;
        }

        public virtual List<object> DecodeChromosome()
        {
            List<object> result = new List<object>();
            foreach(Gene gene in mGenes)
            {
                result.Add(gene.DecodeGene());
            }
            return result;
        }

        public void SetGenes(List<Gene> genes)
        {
            if (genes.Any(x => !(x.GetType().IsAssignableFrom(mGeneType))))
            {
                throw new ArgumentException("Gene list contains incompatible type");
            }

            mGenes = genes;
        }

        public abstract Gene GetRandomGene(Random random);
    }
}
