using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public abstract class Gene
    {
        public byte[] Nucleotids => mNucleotids;
        byte[] mNucleotids { get; set; }
        public int GeneLength => mNucleotids.Length;

        public Gene(byte[] nucleotids)
        {
            mNucleotids = nucleotids;
        }

        public Gene(Random random, int geneLength)
        {
            mNucleotids = new byte[geneLength];

            for(int i = 0; i < geneLength; i++)
            {
                mNucleotids[i] = GetRandomByte(random);
            }
        }

        public byte[] GetNucleotids() => mNucleotids;

        public abstract object DecodeGene();

        //public abstract object EncodeGene();


        byte GetRandomByte(Random random)
        {
            if (random.NextDouble() > 0.5)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void Mutate(int nucleotidIndex)
        {
            if (mNucleotids[nucleotidIndex] == 1)
            {
                mNucleotids[nucleotidIndex] = 0;
            }
            else
            {
                mNucleotids[nucleotidIndex] = 1;
            }
        }
    }
}
