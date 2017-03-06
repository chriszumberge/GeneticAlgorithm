using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class GATutorial
    {
        const double CROSSOVER_RATE = 0.7;
        const double MUTATION_RATE = 0.001;
        public const int POP_SIZE = 100;
        public const int CHROMOSOME_LENGTH = 300;
        const int GENE_LENGTH = 4;
        public const int MAX_ALLOWABLE_GENERATIONS = 400;

        static Random mRandom = new Random();
        static double GetRandomNumber() => mRandom.NextDouble();

        public class Chromosome
        {
            public string bits { get; set; }
            public double fitness { get; set; }

            public Chromosome()
            {
                bits = String.Empty;
                fitness = 0.0;
            }
            public Chromosome(string bts, double ftns)
            {
                bits = bts;
                fitness = ftns;
            }
        }

        public string GetRandomBits(int length)
        {
            string bits = String.Empty;
            for (int i = 0; i < length; i++)
            {
                if (GetRandomNumber() > 0.5)
                {
                    bits = String.Concat(bits, "1");
                }
                else
                {
                    bits = String.Concat(bits, "0");
                }
            }
            return bits;
        }

        static int BinaryStringToInteger(string bits)
        {
            int val = 0;
            int value_to_add = 1;

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

        static int ParseBits(string bits, ref int[] buffer)
        {
            // counter for buffer position
            int cBuff = 0;

            // step through bits a gene at a time until end and store decimal values
            // of valid operators and numbers. Don't forget we are looking for operator - 
            // number - operator - number and so on... We ignore the unused genes 1111
            // and 1110

            // flag to determine if we are looking for an operator or a number
            bool bOperator = true;

            // storage for decimal value of currently tested genen
            int this_gene = 0;

            for (int i = 0; i < CHROMOSOME_LENGTH; i+= GENE_LENGTH)
            {
                // covnert the current gene to decimal
                this_gene = BinaryStringToInteger(bits.Substring(i, GENE_LENGTH));

                // find a gene which represents an operator
                if (bOperator)
                {
                    if ((this_gene < 10) || (this_gene > 13))
                    {
                        continue;
                    }
                    else
                    {
                        bOperator = false;
                        buffer[cBuff++] = this_gene;
                        continue;
                    }
                }
                // find a gene which represents a number
                else
                {
                    if (this_gene > 9)
                    {
                        continue;
                    }
                    else
                    {
                        bOperator = true;
                        buffer[cBuff++] = this_gene;
                        continue;
                    }
                }
            } // next gene

            //	now we have to run through buffer to see if a possible divide by zero
            //	is included and delete it. (ie a '/' followed by a '0'). We take an easy
            //	way out here and just change the '/' to a '+'. This will not effect the 
            //	evolution of the solution
            for (int i = 0; i < cBuff; i++)
            {
                if ((buffer[i] == 13) && (buffer[i + 1] == 0))
                {
                    buffer[i] = 10;
                }
            }

            return cBuff;
        }

        public double AssignFitness(string bits, double target_value)
        {
            //holds decimal values of gene sequence
            int[] buffer = new int[(int)(CHROMOSOME_LENGTH / GENE_LENGTH)];

            int num_elements = ParseBits(bits, ref buffer);

            // ok, we have a buffer filled with valid values of: operator - number - operator - number..
            // now we calculate what this represents.
            float result = 0.0f;

            for (int i = 0; i < num_elements - 1; i += 2)
            {
                switch (buffer[i])
                {
                    case 10:

                        result += buffer[i + 1];
                        break;

                    case 11:

                        result -= buffer[i + 1];
                        break;

                    case 12:

                        result *= buffer[i + 1];
                        break;

                    case 13:

                        result /= buffer[i + 1];
                        break;

                }//end switch

            }

            // Now we calculate the fitness. First check to see if a solution has been found
            // and assign an arbitarily high fitness score if this is so.

            if (result == (float)target_value)

                return 999.0;

            else

                return 1 / (double)(target_value - result);
        }

        public static void PrintChromosome(string bits)
        {
            // holds decimal values of gene sequence
            int[] buffer = new int[(int)CHROMOSOME_LENGTH / GENE_LENGTH];

            // parse the bit string
            int num_elements = ParseBits(bits, ref buffer);

            for (int i = 0; i < num_elements; i++)
            {
                PrintGeneSymbol(buffer[i]);
            }

            return;
        }

        public static void PrintGeneSymbol(int val)
        {
            if (val < 10)
            {
                Console.Write(val + " ");
            }
            else
            {
                switch (val)
                {

                    case 10:

                        Console.Write("+");
                        break;

                    case 11:

                        Console.Write("-");
                        break;

                    case 12:

                        Console.Write("*");
                        break;

                    case 13:

                        Console.Write("/");
                        break;

                }//end switch
                Console.Write(" ");
            }
            return;
        }

        public static void Mutate(ref string bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                if (GetRandomNumber() < MUTATION_RATE)
                {
                    string mutation = String.Empty;
                    if (bits.ElementAt(i) == '1')
                    {
                        mutation = "0";
                    }
                    else
                    {
                        mutation = "1";
                    }

                    bits = String.Concat(bits.Substring(0, i), mutation, bits.Substring(i + 1, bits.Length - 1 - i));
                }
            }
        }

        public static void Crossover(ref string offspring1, ref string offspring2)
        {
            if (GetRandomNumber() < CROSSOVER_RATE)
            {
                // create a random crossover point
                int crossover = (int)GetRandomNumber() * CHROMOSOME_LENGTH;

                string t1 = offspring1.Substring(0, crossover) + offspring2.Substring(crossover, CHROMOSOME_LENGTH);
                string t2 = offspring2.Substring(0, crossover) + offspring1.Substring(crossover, CHROMOSOME_LENGTH);

                offspring1 = t1;
                offspring2 = t2;
            }
        }

        public static string Roulette(double total_fitness, Chromosome[] Population)
        {
            // generate random number between 0 and fitness count
            double slice = GetRandomNumber() * total_fitness;

            // go through the chromosomes adding up the fitness so far
            double fitnessSoFar = 0.0;

            for (int i = 0; i < POP_SIZE; i++)
            {
                fitnessSoFar += Population[i].fitness;

                // if the fitness so far > random number return the chromosome at this point
                if (fitnessSoFar >= slice)
                {
                    return Population[i].bits;
                }
            }

            return String.Empty;
        }
    }
}
