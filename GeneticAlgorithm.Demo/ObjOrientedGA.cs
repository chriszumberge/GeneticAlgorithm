using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Demo
{
    public class MathematicGenome : Genome
    {
        public MathematicGenome(Random random) : base(random, new List<Type> { typeof(MathematicChromosome) })
        {
        }

        public override void CalculateFitness()
        {
            foreach (Chromosome chromosome in Chromosomes)
            {
                if (chromosome is MathematicChromosome)
                {
                    MathematicChromosome mathChromo = chromosome as MathematicChromosome;

                    List<string> decodedChromosome = mathChromo.DecodeChromosome().Select(x => x.ToString()).ToList();

                    double result = 0;

                    for (int i = 0; i < decodedChromosome.Count - 1; i += 2)
                    {
                        switch(decodedChromosome[i])
                        {
                            case "+":
                                result += Int32.Parse(decodedChromosome[i + 1]);
                                break;
                            case "-":
                                result -= Int32.Parse(decodedChromosome[i + 1]);
                                break;
                            case "*":
                                result *= Int32.Parse(decodedChromosome[i + 1]);
                                break;
                            case "/":
                                result /= Int32.Parse(decodedChromosome[i + 1]);
                                break;
                        }
                    }

                    if (result == 10)
                    {
                        mFitness = 999.0;
                    }
                    else
                    {
                        mFitness = 1 / (double)(10 - result);
                    }
                }
            }
        }
    }

    public class MathematicChromosome : Chromosome
    {
        public MathematicChromosome(Random random) : base(random, typeof(MathmaticGene), 75)
        {

        }

        public override Gene GetRandomGene(Random random)
        {
            return new MathmaticGene(random);
        }

        public override List<object> DecodeChromosome()
        {
            List<object> result = new List<object>();

            bool bOperator = true;
            string this_gene = String.Empty;

            foreach (Gene gene in Genes)
            {
                this_gene = gene.DecodeGene().ToString();

                if (String.IsNullOrEmpty(this_gene))
                    continue;

                if (bOperator)
                {
                    if (this_gene.Equals("+") || this_gene.Equals("-") || this_gene.Equals("*") || this_gene.Equals("/"))
                    {
                        bOperator = false;
                        result.Add(this_gene);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (this_gene.Equals("+") || this_gene.Equals("-") || this_gene.Equals("*") || this_gene.Equals("/"))
                    {
                        continue;
                    }
                    else
                    {
                        bOperator = true;
                        result.Add(this_gene);
                        continue;
                    }
                }
            }

            return result;
        }
    }

    public class MathmaticGene : Gene
    {
        public MathmaticGene(Random random) : base(random, 4){}

        public override object DecodeGene()
        {
            int geneValue = BitsToInteger();
            if (geneValue < 10)
            {
                return geneValue.ToString();
            }
            else
            {
                switch (geneValue)
                {
                    case 10:
                        return "+";
                    case 11:
                        return "-";
                    case 12:
                        return "*";
                    case 13:
                        return "/";
                    default:
                        return String.Empty;

                }//end switch
            }
        }

        private int BitsToInteger()
        {
            int val = 0;
            int value_to_add = 1;

            string bits = String.Join(String.Empty, GetNucleotids());

            for (int i = bits.Length; i > 0; i--)
            {
                if (bits.ElementAt(i - 1) == '1')
                {
                    val += value_to_add;
                }
                value_to_add *= 2;
            }
            return val;
        }
    }
}
