//
// Params.cs
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
namespace GeneticAlgorithm
{
    public static class Params
    {
        public static double CrossoverRate = 0.7;
        public static double MutationRate = 0.1;
        public static double MaxPerturbation = 0.3;
        public static int NumCopiesElite = 1;
        public static int NumElite = 4;
    }
}
