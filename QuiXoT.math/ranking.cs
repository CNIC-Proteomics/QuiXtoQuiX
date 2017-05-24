using System;
using System.Collections.Generic;
using System.Text;

namespace QuiXoT.math
{
    public class Utilities
    {
        public static int find(double[] matrix, double evaluated) 
        {

            int ranking=0;

            int low=1;
            int high=matrix.GetUpperBound(0);
            int diff=high - low;
            
            int actEval=(int)Math.Truncate((double)((high-low)/2));
           

            if (evaluated < matrix[1]) 
            {
                ranking = 0;
                return ranking;
            }
            if (evaluated == matrix[1])
            {
                ranking = 1;
                return ranking;
            }
            if (evaluated == matrix[matrix.GetUpperBound(0)])
            {
                ranking = matrix.GetUpperBound(0)-1;
                return ranking;
            }
            if (evaluated > matrix[matrix.GetUpperBound(0)])
            {
                ranking = matrix.GetUpperBound(0) - 1;
                return ranking;
            }

            while(diff>3)
            {

                if (evaluated == matrix[actEval])
                {
                    ranking = actEval;
                    return ranking;
                }

                if (evaluated < matrix[actEval])
                {
                    high = actEval;
                    diff = high-low;
                    actEval = low + (int)Math.Truncate((double)((high - low) / 2));
                }
                else 
                {
                    low = actEval;
                    diff = high - low;
                    actEval = low + (int)Math.Truncate((double)((high - low) / 2));
                }

            }

            if (evaluated >= matrix[actEval])
            {
                ranking = actEval - 1;
            }
            else
            {
                ranking = actEval;
            }

            return ranking;

        }

        #region Weight calculation methods


        /// <summary>
        /// Weight version 6
        /// </summary>
        /// <param name="quanMethod">method used to quantify</param>
        /// <param name="A">quantity of not labeled sample</param>
        /// <param name="B">quantity of labeled sample</param>
        /// <param name="MSQ1">Mean of the sum of squares 1</param>
        /// <param name="MSQ2">Mean of the sum of squares 2</param>
        /// <param name="MSQ3">Mean of the sum of squares 3</param>
        /// <param name="efficiency">labeling efficiency detected</param>
        /// <returns></returns>
        public static double calWeight(LNquantitate.quantitationStrategy qStrategy,
                                        double A,
                                        double B,
                                        double MSQ1,
                                        double MSQ2,
                                        double MSQ3,
                                        double efficiency)
        {


            double weight=0.0;
            double zeroCorr = 0.00000000001;

            A += zeroCorr;
            B += zeroCorr;

    
            switch (qStrategy)
            {
                case LNquantitate.quantitationStrategy.iTRAQ:

                    weight = A > B ? Math.Pow(A, 2) : Math.Pow(B, 2);
                    break;

                case LNquantitate.quantitationStrategy.SILAC:
                    double Imax = A >= B ? A : B;
                    weight = 1 / (MSQ1 / (Imax * Imax) + MSQ2 / (Imax * Imax));
                    break;

                case LNquantitate.quantitationStrategy.O18_ZS:
                    
                    double B0 = B * (1 - efficiency) * (1 - efficiency);
                    double B1 = 2 * B * efficiency * (1 - efficiency);
                    double B2 = B * efficiency * efficiency;

                    double AB0 = A + B0;
                    double B1B2 = B1 + B2;


                    if (AB0 >= B1B2)
                    {
                        weight = 1 / ((MSQ1 + MSQ2) / (A * A));
                    }
                    else
                    {
                        weight = 1 / ((MSQ3 + MSQ2) / (B * B));
                    }
                    break;

                default:
                    weight = 0.0;
                    break;
            }
            
            return weight;
 
        }

        /// <summary>
        /// Weight version 5
        /// </summary>
        /// <param name="MSQLeft">Mean of the sum of squares of the left window</param>
        /// <param name="MSQPept">Mean of the sum of squares of the peptide</param>
        /// <param name="MSQRight">Mean of the sum of squares of the right window</param>
        /// <param name="A">quantity of not labeled sample</param>
        /// <param name="B">quantity of labeled sample</param>
        /// <param name="efficiency">labeling efficiency detected</param>
        /// <returns>weight</returns>
        public static double calWeight(double MSQLeft,
                                        double MSQPept,
                                        double MSQRight,
                                        double A,
                                        double B,
                                        double efficiency)
        {
            double zeroCorr = 0.00000000001;

            A += zeroCorr;
            B += zeroCorr;

            double weight = 0;

            double B0 = B * (1 - efficiency) * (1 - efficiency);
            double B1 = 2 * B * efficiency * (1 - efficiency);
            double B2 = B * efficiency * efficiency;

            double AB0 = A + B0;
            double B1B2 = B1 + B2;


            if (AB0 >= B1B2)
            {
                weight = 1 / ((MSQLeft + MSQPept) / (AB0 * AB0));
            }
            else
            {
                weight = 1 / ((MSQRight + MSQPept) / (B1B2 * B1B2));
            }


            return weight;
        }

        /// <summary>
        /// Weight version 4
        /// </summary>
        /// <param name="MSQLeft">Mean of the sum of squares of the left window</param>
        /// <param name="MSQPept">Mean of the sum of squares of the peptide</param>
        /// <param name="MSQRight">Mean of the sum of squares of the right window</param>
        /// <param name="A">quantity of not labeled sample</param>
        /// <param name="B">quantity of labeled sample</param>
        /// <returns>weight</returns>
        public static double calWeight(double MSQLeft,
                                        double MSQPept,
                                        double MSQRight,
                                        double A,
                                        double B)
        {
            double zeroCorr = 0.00000000001;

            A += zeroCorr;
            B += zeroCorr;

            double weight = 0;
            if (A >= B)
            {
                weight = 1 / ((MSQLeft + MSQPept) / (A * A));
            }
            else
            {
                weight = 1 / ((MSQRight + MSQPept) / (B * B));
            }


            return weight;
        }

        /// <summary>
        /// Weight, version 2
        /// </summary>
        /// <param name="A"></param>
        /// <param name="SD_A"></param>
        /// <param name="B"></param>
        /// <param name="SD_B"></param>
        /// <returns></returns>
        public static double calWeight(double A, double SD_A, double B, double SD_B)
        {
            double zeroCorr = 0.0001;

            A += zeroCorr;
            B += zeroCorr;

            double maxAB = Math.Max(A, B);
            double maxDADB = A > B ? SD_A : SD_B;

            //double weight = 1 / (( Math.Abs(SD_A /A)*Math.Abs(SD_A/A) + Math.Abs(SD_B / B) * Math.Abs(SD_B / B))*Math.Log(2)*Math.Log(2));
            double weight = 1 / ((Math.Abs(maxDADB / maxAB) * Math.Abs(maxDADB / maxAB)));

            return weight;
        }

        public static double calWeight(double A, double SD_A,
                                        double B, double SD_B,
                                        double sigma, double SD_sigma,
                                        double f, double SD_f,
                                        double alpha, double SD_alpha,
                                        double deltaMZ, double SD_deltaMZ)
        {

            double zeroCorr = 0.0001;

            A += zeroCorr;
            B += zeroCorr;
            sigma += zeroCorr;
            f += zeroCorr;
            alpha += zeroCorr;
            deltaMZ += zeroCorr;

            double factors = Math.Abs(SD_A / A) + Math.Abs(SD_B / B) + Math.Abs(SD_sigma / sigma) + Math.Abs(SD_f / f) + Math.Abs(SD_alpha / alpha) + Math.Abs(SD_deltaMZ / deltaMZ);

            double weight = 1 / (factors * factors);

            return weight;

        }


        public static double calWeight(double A, double SD_A,
                                        double B, double SD_B,
                                        double sigma, double SD_sigma,
                                        double f, double SD_f,
                                        double deltaMZ, double SD_deltaMZ)
        {

            double zeroCorr = 0.0001;

            A += zeroCorr;
            B += zeroCorr;
            sigma += zeroCorr;
            f += zeroCorr;
            deltaMZ += zeroCorr;

            double factors = Math.Abs(SD_A / A) + Math.Abs(SD_B / B) + Math.Abs(SD_sigma / sigma) + Math.Abs(SD_f / f) + Math.Abs(SD_deltaMZ / deltaMZ);

            double weight = 1 / (factors * factors);

            return weight;

        }

        #endregion

    
    }
}
