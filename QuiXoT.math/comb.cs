using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace QuiXoT.math
{

    public class Comb
    {

        /// <summary>
        /// Structure for chemical composition.
        /// </summary>
        public struct compStrt 
        {
            private string ElemVal;
            private int NatsVal;

            public compStrt(string ElemValue, int NatsValue) 
            {
                ElemVal = ElemValue;
                NatsVal = NatsValue;
            }

            public string Elem 
            {
                get 
                {
                    return ElemVal;
                }
                set 
                {
                    ElemVal = value;
                }
            }

            public int Nats
            {
                get
                {
                    return NatsVal;
                }
                set
                {
                    NatsVal = value;
                }
            }
        }

        /// <summary>
        /// Structure for (I vs m/z) data. 
        /// </summary>
        [Serializable]
        public struct mzI
        {
            private Double mzVal;
            private Double IVal;

            public mzI(double mzValue, double IValue)
            {
                mzVal = mzValue;
                IVal = IValue;
            }

            public double mz
            {
                get
                {
                    return mzVal;
                }
                set
                {
                    mzVal = value;
                }
            }

            public double I
            {
                get
                {
                    return IVal;
                }
                set
                {
                    IVal = value;
                }
            }

            public override string ToString()
            {
                return (String.Format("{0}, {1}", mzVal, IVal));
            }


        }


        /// <summary>
        /// Calculates the I vs m/z spectra for a determined composition
        /// </summary>
        /// <param name="comp">(compStrt[])Chemical composition to analyze</param>
        /// <param name="isotopes">(isotList[][])isotopes list loaded</param>
        /// <param name="Charge">(int)charge of the peptide</param>
        /// <param name="kmax">(int)maximum number of peaks to be calculated</param>
        /// <returns>(mzI[]) set of intensity peaks on their correspondent m/z</returns>
        public static mzI[] calIntensities(compStrt[] comp, isotList[][] isotopes, int Charge,double calibrationError, int kmax) 
        {
            double protonMass = 1.007276466812;  // source: http://physics.nist.gov/cgi-bin/cuu/Value?mpu

            // Search for the correspondency of the elements in the chemical formula
            int[] elemIndex=new int[comp.GetUpperBound(0)+1];
            for (int i = 0; i <= comp.GetUpperBound(0); i++) 
            {
                for(int j=0;j<=isotopes.GetUpperBound(0);j++)
                {
                    if (comp[i].Elem == isotopes[j][0].Elem) 
                    {
                        elemIndex[i] = j;
                    }
                }                
            }

            double[,] ItPeaks=new double[comp.GetUpperBound(0)+1,kmax];
            
            //calculate the Nnomial for k=[ 1 ... kmax] for each element
            for (int i = 0; i <= comp.GetUpperBound(0); i++) 
            {

                // Assign the isotopes' natural abundancies of the element. 
                double[] f = new double[7];
                for (int j = 0; j <= isotopes[elemIndex[i]].GetUpperBound(0); j++) 
                {
                    if (Math.Round(isotopes[elemIndex[i]][j].DMass, 0) <= 6) 
                    {
                        f[(int)Math.Round(isotopes[elemIndex[i]][j].DMass,0)]=isotopes[elemIndex[i]][j].f;
                    }
                }

                for (int k = 0; k < kmax; k++)
                {
                    ItPeaks[i,k] = Nnomial(k, f, comp[i].Nats);                  
                }
            }
            
      
            //Calculate the theoretical precursor mass
            double thPrecMass=0.0;
            for (int i = 0; i <= comp.GetUpperBound(0); i++) 
            {
                thPrecMass +=  comp[i].Nats * isotopes[elemIndex[i]][0].Mass;
            }
            thPrecMass += Charge * protonMass;



            //Join the isotopic spectra in the mzI ResultSpectrum structure, 
            //correct mz charge relation
            
            mzI[] ResultSpectrum = joinIsot(ItPeaks, thPrecMass);
            
            for (int i = 0; i <= ResultSpectrum.GetUpperBound(0); i++) 
            {
                ResultSpectrum[i].mz = ((ResultSpectrum[i].mz) / Charge)+calibrationError; // +Charge * protonMass;
            }

            return ResultSpectrum;

        }

        /// <summary>
        /// returns -1.0 if the summatory of the natural frequencies is not 1.0
        /// indexes must be in increasing order.
        /// </summary> 
        public static double Nnomial(int k, double[] f, int Nat)
           
        {
            double factor = 0.0;

            // general checks
            int bound = f.GetUpperBound(0);
            
            decimal sumf;
            
            
            sumf = 0.0M;
            

            for (int i = 0; i <= bound; i++) 
            {
                sumf +=(decimal)f[i];
            }

            if (sumf != 1) return -1.0;


            // summatory for each combination of isotopes
            // WARNING: the max number of isotopes is 6. 
            // For any higher number, one must change the code. 
            
            if (bound!=6)
            {
                return -1.0;
            }

            int[] idx = new int[7];
             
            for (idx[1] = 0; idx[1] <= Math.Floor((double)k / 1); idx[1]++) 
            {
                for (idx[2] = 0; idx[2] <= Math.Floor((double)k / 2); idx[2]++)
                {
                    for (idx[3] = 0; idx[3] <= Math.Floor((double)k / 3); idx[3]++)
                    {
                        for (idx[4] = 0; idx[4] <= Math.Floor((double)k / 4); idx[4]++)
                        {
                            for (idx[5] = 0; idx[5] <= Math.Floor((double)k / 5); idx[5]++)
                            {
                                for (idx[6] = 0; idx[6] <= Math.Floor((double)k / 6) ; idx[6]++)
                                {
                                    int sumi = 1 * idx[1] + 2 * idx[2] + 3 * idx[3] + 
                                               4 * idx[4] + 5 * idx[5] + 6 * idx[6];
                                    if (sumi == k)
                                    {
                                        factor+=nnomialFkcond(f,idx,Nat);
                                    }
                                }
                            }
                        }
                    }
                }
            }
                        
            return factor;
        }

        
        /// <summary>
        /// <param name="f">(double) frequencies. Array of 6 elements</param>
        /// <param name="idx">(int) Combination of isotopes that keeps the condition of SUM(isotopes)=k</param>
        /// <returns>(double) combinatorial factor for a set of isotopes that keeps the condition of SUM(isotopes)=k
        /// (n!/(i!j!...(n-i-j-...)!)*(fi)^i*(fj)^j*...*(1-fi-fj)^(n-i-j) 
        /// </returns>
        /// </summary>
        private static double nnomialFkcond(double[] f, int[] idx,int k)
        {
            double kLog = factlog10(k);
            int bound=idx.GetUpperBound(0);
            double[] idxLog = new double[bound+1];
            double fij = 1.0;

            int nminus = k; 
            for (int i = 1; i <= bound; i++) 
            {
                idxLog[i] = factlog10(idx[i]);
                nminus -= idx[i];
                if (f[i] == 0 && idx[i] > 0) return 0.0;                
            }

            double nminusLog = factlog10(nminus);

            double divfactLog = kLog-nminusLog;
            double fminus = 1.0;
            
            for (int i = 1; i <= bound; i++) 
            {
                divfactLog -= idxLog[i];
                fij *= Math.Pow(f[i], (double)idx[i]);
                
            }


            for (int i = 1; i <= bound; i++) 
            {
                fminus -= f[i];
            }

            //fminus = f[0];

            double divfact = Math.Pow(10, divfactLog);



            fij *= Math.Pow(fminus, nminus);

            double factor = divfact * fij;

            return factor;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="n">(int) number to factorize</param>
        /// <returns>log10(n!)</returns>
        public static double factlog10(int n) 
        {

            double ml = Math.Log10(1.0);
            for (int t = 1; t <= n; t++)
            {
                ml += Math.Log10(t);
            }
            //double res = Math.Pow(10, ml);
            //return res;
            return ml;
        }

        /// <summary>
        /// joins the spectra formed by the isotopes of two elements
        /// </summary>
        /// <param name="isot1">(double[]) spectra of element 1</param>
        /// <param name="isot2">(double[]) spectra of element 2</param>
        /// <returns>(double[]) spectra of element 1 + element 2</returns>
        public static double[] joinIsot(double[] isot1, double[] isot2) 
        {
            int bound1 = isot1.GetUpperBound(0);
            int bound2 = isot2.GetUpperBound(0);
            int max = Math.Max(bound1, bound2);

            double[] sum=new double[bound1+bound2+1];

            for (int i = 0; i <= bound1; i++) 
            {
                for (int j = 0; j <= bound2; j++) 
                {
                    sum[i + j] += isot1[i] * isot2[j];
                }
            }

            return sum;
        }

        /// <summary>
        /// joins the spectra formed by the isotopes of n elements
        /// </summary>
        /// <param name="ItPeaks">(double[elements,kmax]) spectra of the elements</param>
        /// <returns>(double[]) sum spectrum of the n elements</returns>
        private static mzI[] joinIsot(double[,] ItPeaks, double thPrecMass)
        {
            int bound = ItPeaks.GetUpperBound(1);
            int bound2 = ItPeaks.GetUpperBound(0);

            mzI[] sum = new mzI[bound + 1];
            mzI[] jIsot = new mzI[bound + 1];

            //Initialize the matrix sum with the first value of the matrix ItPeaks
            for (int i = 0; i <= bound; i++) 
            {
                sum[i].I = ItPeaks[0, i];
                sum[i].mz = thPrecMass + i;
            }

            //Join the spectra in the matrix sum

            if (ItPeaks.GetUpperBound(0) > 1)
            {
                for (int numSpectra = 1; numSpectra <= bound2 ; numSpectra++)
                {
                    for (int i = 0; i <= bound; i++)
                    {
                        for (int j = 0; j <= bound; j++)
                        {
                            if ((i + j) <= bound)
                            {
                                jIsot[i + j].I += sum[i].I * ItPeaks[numSpectra, j];
                                jIsot[i + j].mz = thPrecMass + (i + j);
                            }
                        }
                    }
                    jIsot.CopyTo(sum,0);
                    if (numSpectra < bound2)
                    {
                        jIsot = null;
                        jIsot = new mzI[bound + 1];
                    }
                }
            }
            

            return jIsot;
        }



        public static void writeSpectrum(mzI[][] data,
                                        string filename,
                                        string header)
        {
            
            StreamWriter sw = new StreamWriter(filename, false);

            sw.Write(sw.NewLine);
            sw.Write(sw.NewLine);

            //write the spectrum header
            sw.Write(header);

            sw.Write(sw.NewLine);
            sw.Write(sw.NewLine);
           
            sw.Write("\t");
            sw.Write("experimental spectrum");
            sw.Write("\t");
            sw.Write("fitted spectrum");
            sw.Write(sw.NewLine);
            
            // First we write the headers.
            sw.Write("m/z");
            sw.Write("\t");
            sw.Write("Intensity Exp");
            sw.Write("\t");
            sw.Write("Intensity Fitted");
            sw.Write(sw.NewLine);

            //Write the data

            int numOfPoints = data[0].GetUpperBound(0);
            int numOfData = data.GetUpperBound(0);
            int specs=-1;
            for (int i = 0; i < numOfData; i++)
            {
                if (data[i] != null) specs++;
            }

            try
            {
                for (int i = 0; i <= numOfPoints; i++)
                {
                    sw.Write(data[0][i].mz.ToString());
                    sw.Write("\t");

                    for (int j = 0; j <= specs; j++)
                    {
                        sw.Write(data[j][i].I.ToString());
                        sw.Write("\t");                        
                    }
                    sw.Write(sw.NewLine);
                }
            }
            catch
            {
            }


            sw.Close();



        }



    }
}
