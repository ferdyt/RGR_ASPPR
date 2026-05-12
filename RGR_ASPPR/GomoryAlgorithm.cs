using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Lab_4_ASPPR
{
    internal static class GomoryAlgorithm
    {
        static public int GetXWithMaxFractialPart(double[] lastColumn)
        {
            int maxIndex = -1;
            double maxFract = -1.0;
            double epsilon = 1e-9;

            for (int i = 0; i < lastColumn.Length; i++)
            {
                double val = lastColumn[i];
                double fract = val - Math.Floor(val);

                if (fract < epsilon || fract > (1.0 - epsilon))
                    fract = 0;

                if (fract > maxFract)
                {
                    maxFract = fract;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        static public void CalculateCoefficients()
        {

        }
    }
}
