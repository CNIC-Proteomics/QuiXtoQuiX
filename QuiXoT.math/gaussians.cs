using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
 

namespace QuiXoT.math
{

    public enum Resolution { LOW, HIGH }


    /// <summary>
    /// structure for the fit parameters
    /// </summary>
    public struct fitDataStrt
    {
        
        private double AVal;
        private double BVal;
        private double fVal;
        private double deltaMzVal;
        private double alphaVal;
        private double sigmaVal;
        private double signoiseVal;
        
        /// <summary>
        /// constructor of the structure for the fit parameters
        /// </summary>
        /// <param name="AValue">(double)concentration of A</param>
        /// <param name="BValue">(double)concentration of B</param>
        /// <param name="deltaMzValue">(double)experimental deviation of m/z</param>
        /// <param name="alphaValue">(double)Leptokurtosis</param>
        /// <param name="sigmaValue">(double)gaussian width</param>
        /// <param name="signoiseValue">(double)Signal to Noise relation</param>
        public fitDataStrt(double AValue, double BValue, double fValue ,double deltaMzValue, double alphaValue,
                           double sigmaValue, double signoiseValue)
        {
            AVal = AValue;
            BVal = BValue;
            fVal = fValue;
            deltaMzVal = deltaMzValue;
            alphaVal = alphaValue;
            sigmaVal = sigmaValue;
            signoiseVal = signoiseValue;
                       
        }

        public double A
        {
            get 
            {
                return AVal;
            }
            set 
            {
                AVal = Math.Abs(value);
            }
        }

        public double B
        {
            get
            {
                return BVal;
            }
            set
            {
                BVal = Math.Abs(value);
            }
        }
        public double f
        {
            get
            {
                return fVal;
            }
            set
            {
                fVal = Math.Abs(value);
            }
        }
        public double deltaMz
        {
            get
            {
                return deltaMzVal;
            }
            set
            {
                deltaMzVal = value;
            }
        }
        public double alpha
        {
            get
            {
                return alphaVal;
            }
            set
            {
                alphaVal = Math.Abs(value);
            }
        }
        public double sigma
        {
            get
            {
                return sigmaVal;
            }
            set
            {
                sigmaVal = value;
            }
        }
        public double signoise
        {
            get
            {
                return signoiseVal;
            }
            set
            {
                signoiseVal = value;
            }
        }

    }
    

    

    /// <summary>
    /// Structure for reading the fit parameters XML file.
    /// </summary>
    public struct instrumentParamsStrt 
    {
        private string instNameVal;
        private Resolution instResolutionVal;
        private int kmaxVal;
        private double alphaVal;
        private double sigmaVal;
        private double deltaRVal;
        private double fVal;
        private double deltaMzVal;
        private double sn_fVal;
        private double varAVal;
        private double varBVal;
        private double varfVal;
        private double varSigmaVal;
        private double varAlphaVal;
        private double varSnVal;

        public instrumentParamsStrt(string instNameValue,Resolution instResolutionValue,int kmaxValue,
                                    double alphaValue, double sigmaValue, double deltaRValue, double fValue,
                                    double deltaMzValue, double sn_fValue, double varAValue, double varBValue,
                                    double varfValue, double varSigmaValue, double varAlphaValue,double varSnValue)
        {
            instNameVal = instNameValue;
            instResolutionVal = instResolutionValue;
            kmaxVal = kmaxValue;
            alphaVal = alphaValue;
            sigmaVal = sigmaValue;
            deltaRVal = deltaRValue;
            fVal = fValue;
            deltaMzVal = deltaMzValue;
            sn_fVal = sn_fValue;
            varAVal = varAValue;
            varBVal = varBValue;
            varfVal = varfValue;
            varSigmaVal = varSigmaValue;
            varAlphaVal = varAlphaValue;
            varSnVal = varSnValue;
        }

        public string instName
        {
            get 
            {
                return instNameVal;
            }
            set 
            {
                instNameVal = value;
            }
        }

        public Resolution instResolution
        {
            get
            {
                return instResolutionVal;
            }
            set
            {
                instResolutionVal = value;
            }
        }
        public int kmax
        {
            get
            {
                return kmaxVal;
            }
            set
            {
                kmaxVal = value;
            }
        }
        public double alpha
        {
            get
            {
                return alphaVal;
            }
            set
            {
                alphaVal = value;
            }
        }
        
        public double sigma
        {
            get
            {
                return sigmaVal;
            }
            set
            {
                sigmaVal = value;
            }
        }
        
        public double deltaR
        {
            get
            {
                return deltaRVal ;
            }
            set
            {
                deltaRVal  = value;
            }
        }
        

        public double f
        {
            get
            {
                return fVal;
            }
            set
            {
                fVal = value;
            }
        }
        public double deltaMz
        {
            get
            {
                return deltaMzVal;
            }
            set
            {
                deltaMzVal = value;
            }
        }
        
        public double sn_f
        {
            get
            {
                return sn_fVal;
            }
            set
            {
                sn_fVal = value;
            }
        }
        public double varA
        {
            get
            {
                return varAVal;
            }
            set
            {
                varAVal = value;
            }
        }
        public double varB
        {
            get
            {
                return varBVal;
            }
            set
            {
                varBVal = value;
            }
        }
        public double varf
        {
            get
            {
                return varfVal;
            }
            set
            {
                varfVal = value;
            }
        }
                    
        public double varSigma
        {
            get
            {
                return varSigmaVal;
            }
            set
            {
                varSigmaVal = value;
            }
        }
        public double varAlpha
        {
            get
            {
                return varAlphaVal;
            }
            set
            {
                varAlphaVal = value;
            }
        }

        public double varSn
        {
            get
            {
                return varSnVal;
            }
            set
            {
                varSnVal = value;
            }
        }


    }

    public class Gaussians
    {

        /// <summary>
        /// Calculates the isotopic envelope.
        /// </summary>
        /// <param name="intensities">(Comb.mzI[])array of isotopic peaks</param>
        /// <param name="expData">(Comb.mzI[])array of the experimental data</param>
        /// <param name="fitData">(fitDataStr)Fit parameters</param>
        /// <param name="charge">(int)Charge</param>
        /// <param name="deltaR">(double)Shift due to the marking (in 18O, deltaR=2.004245778)</param>
        /// <returns>(Comb.mzI[])array of intensities representing the envelope</returns>
        public static Comb.mzI[] calEnvelope(Comb.mzI[] intensities, Comb.mzI[] expData, fitDataStrt fitData, int charge, double deltaR)
        {
            double A = fitData.A;
            double B = fitData.B;
            double f=fitData.f;
            double deltaMz= fitData.deltaMz;
            double alpha=fitData.alpha;
            double sigma=fitData.sigma;
            double signoise=fitData.signoise;

            Comb.mzI[] envelope=new Comb.mzI[expData.Length];
            
            double Coeff1 = A + B * (1 - f) * (1 - f);
            double Coeff2 = 2*B*f*(1-f);
            double Coeff3 = B*f*f;

            //Modify this to obtain beatiful figures for your posters
            //Coeff1 = 0;//A + B * (1 - f) * (1 - f);
            //Coeff2 = 0;// 2 * B * f * (1 - f);
            //Coeff3 = 0; //B * f * f;
            //

            int nGaussians = intensities.Length;
            //int nGaussians = 4; //intensities.GetUpperBound(0);

            //ADDED 30.nov.2006

            
            Comb.mzI[] intensitiesCoeff1 = new Comb.mzI[intensities.Length];
            Comb.mzI[] intensitiesCoeff2 = new Comb.mzI[intensities.Length];
            Comb.mzI[] intensitiesCoeff3 = new Comb.mzI[intensities.Length];

            for (int i = 0; i <= intensitiesCoeff1.GetUpperBound(0); i++)
            {
                intensitiesCoeff1[i].mz = intensities[i].mz;
                intensitiesCoeff2[i].mz = intensities[i].mz + deltaR / charge;
                intensitiesCoeff3[i].mz = intensities[i].mz + 2 * deltaR / charge;
            }
             
            for (int i = 0; i < nGaussians; i++)
            {
                intensitiesCoeff1[i].I = Coeff1 * intensities[i].I;
                intensitiesCoeff2[i].I = Coeff2 * intensities[i].I;
                intensitiesCoeff3[i].I = Coeff3 * intensities[i].I;
            }
        

            Comb.mzI[] intensitiesTotal = new Comb.mzI[intensities.Length + 4];

            //First 4 peaks 
            intensitiesTotal[0].mz = intensitiesCoeff1[0].mz;
            intensitiesTotal[0].I = intensitiesCoeff1[0].I;
            intensitiesTotal[1].mz = intensitiesCoeff1[1].mz;
            intensitiesTotal[1].I = intensitiesCoeff1[1].I;
            intensitiesTotal[2].mz = intensitiesCoeff1[2].mz;
            intensitiesTotal[2].I = intensitiesCoeff1[2].I + intensitiesCoeff2[0].I;
            intensitiesTotal[3].mz = intensitiesCoeff1[3].mz;
            intensitiesTotal[3].I = intensitiesCoeff1[3].I + intensitiesCoeff2[1].I;

            //Peaks in the middle
            for (int i = 4; i <= intensitiesTotal.GetUpperBound(0)-4; i++) 
            {
                
                intensitiesTotal[i].mz = intensitiesCoeff3[i-4].mz;
                intensitiesTotal[i].I = intensitiesCoeff1[i].I+intensitiesCoeff2[i-2].I+intensitiesCoeff3[i-4].I;
                
            }

            //Last 4 peaks
            intensitiesTotal[intensitiesTotal.GetUpperBound(0) - 3].mz = intensitiesCoeff2[intensitiesCoeff2.GetUpperBound(0)-1].mz;
            intensitiesTotal[intensitiesTotal.GetUpperBound(0) - 3].I = intensitiesCoeff2[intensitiesCoeff2.GetUpperBound(0) - 1].I + intensitiesCoeff3[intensitiesCoeff3.GetUpperBound(0)-3].I;
            intensitiesTotal[intensitiesTotal.GetUpperBound(0) - 2].mz = intensitiesCoeff2[intensitiesCoeff2.GetUpperBound(0)].mz;
            intensitiesTotal[intensitiesTotal.GetUpperBound(0) - 2].I = intensitiesCoeff2[intensitiesCoeff2.GetUpperBound(0)].I + intensitiesCoeff3[intensitiesCoeff3.GetUpperBound(0) - 2].I;
            intensitiesTotal[intensitiesTotal.GetUpperBound(0) - 1].mz = intensitiesCoeff3[intensitiesCoeff3.GetUpperBound(0) - 1].mz;
            intensitiesTotal[intensitiesTotal.GetUpperBound(0) - 1].I = intensitiesCoeff3[intensitiesCoeff3.GetUpperBound(0) - 1].I;
            intensitiesTotal[intensitiesTotal.GetUpperBound(0)].mz = intensitiesCoeff3[intensitiesCoeff3.GetUpperBound(0)].mz;
            intensitiesTotal[intensitiesTotal.GetUpperBound(0)].I = intensitiesCoeff3[intensitiesCoeff3.GetUpperBound(0)].I;



            //search the positions (m/z) of the peaks on the experimental data
            //Assuming that expData is sorted by m/z!!!
            double[] expmz = new double[expData.Length];
            int[] intensitiesMzPos = new int[intensitiesTotal.Length];


            for (int i = 0; i <= expData.GetUpperBound(0); i++)
            {
                expmz[i] = expData[i].mz;
            }
            for (int i = 0; i <= intensitiesTotal.GetUpperBound(0); i++)
            {
                intensitiesMzPos[i] = Utilities.find(expmz, intensitiesTotal[i].mz);
            }


            //Add background to theoretical envelope
            for (int i = 0; i <= envelope.GetUpperBound(0); i++)
            {
                envelope[i].mz = expData[i].mz;
                envelope[i].I = signoise;
            }

            double maxContribSigma = 3 * sigma;

            for (int i = 0; i <= intensitiesTotal.GetUpperBound(0); i++)
            {
            
                double diff = 0;
                int idx=0;
                while (diff < maxContribSigma  && intensitiesMzPos[i] + idx  < envelope.Length) //Meter el parámetro a la hoja XML de parámetros 
                {
                    double contrib;
                    contrib = intensitiesTotal[i].I * gaussDblExp(envelope[intensitiesMzPos[i] + idx].mz, intensitiesTotal[i].mz, sigma, alpha).I;
                    envelope[intensitiesMzPos[i] + idx].I += contrib;
                    //diff = contrib / (envelope[intensitiesMzPos[i] + idx].I+0.00001);
                    diff = Math.Abs(envelope[intensitiesMzPos[i] + idx].mz - envelope[intensitiesMzPos[i]].mz);
                    idx++;                  
                }

                diff = 0;
                idx = 1;
                while (diff < maxContribSigma && intensitiesMzPos[i] - idx > 0 ) //Meter el parámetro a la hoja XML de parámetros 
                {                    
                    double contrib;
                    contrib = intensitiesTotal[i].I * gaussDblExp(envelope[intensitiesMzPos[i] - idx].mz, intensitiesTotal[i].mz, sigma, alpha).I;
                    envelope[intensitiesMzPos[i] - idx].I += contrib;
                    //diff = contrib / (envelope[intensitiesMzPos[i] - idx].I+0.0001);
                    diff = Math.Abs(envelope[intensitiesMzPos[i] - idx].mz - envelope[intensitiesMzPos[i]].mz);
                    idx++;
                }

            }



            //Correct deltaMZ 
            //Comb.mzI[] envelopeCorrected = (Comb.mzI[])envelope.Clone();
            Comb.mzI[] envelopeCorrected =new Comb.mzI[envelope.Length];

            for (int i = 1; i <= envelopeCorrected.GetUpperBound(0); i++)
            {
                envelopeCorrected[i].mz = envelope[i].mz;
                envelopeCorrected[i].I = signoise; //signoise
            }

            int deltaMZpos = 0;
            double deltaMZq = 0;
            double initPos = expData[deltaMZpos + 1].mz;
            while ((double)deltaMZq < Math.Abs(deltaMz))
            {
                deltaMZq = expData[deltaMZpos + 1].mz - initPos;
                deltaMZpos++;
            }

            if (deltaMz < 0)
            {
                deltaMZpos = -deltaMZpos;
            }


            if (Math.Sign(deltaMZpos) >= 0)
            {
                for (int i = 1 ; i <= envelopeCorrected.GetUpperBound(0) - deltaMZpos; i++)
                {
                    envelopeCorrected[i + deltaMZpos].I = envelope[i].I;
                }
            }
            else
            {                    
                for (int i = - deltaMZpos; i <= envelopeCorrected.GetUpperBound(0) + deltaMZpos; i++)
                {
                    envelopeCorrected[i + deltaMZpos+1].I = envelope[i].I;                    
                }
            }
            

            //ADDED_END
           
            //DEPRECATED 30.nov.2006
            /*
            for (int i = 0; i <= expData.GetUpperBound(0); i++)
            {
                envelope[i].mz=expData[i].mz;
                envelope[i].I += signoise;
                        
                for (int j = 0; j < nGaussians; j++){
                        
                    envelope[i].I += Coeff1 * gaussDblExp(expData[i].mz, intensities[j].mz + deltaMz, sigma, alpha).I * intensities[j].I;
                    envelope[i].I += Coeff2 * gaussDblExp(expData[i].mz, intensities[j].mz + deltaMz + deltaR / charge, sigma, alpha).I * intensities[j].I;
                    envelope[i].I += Coeff3 * gaussDblExp(expData[i].mz, intensities[j].mz + deltaMz + 2 * deltaR / charge, sigma, alpha).I * intensities[j].I; 
                            
                }
            }
            */
            //DEPRECATED_END

            return envelopeCorrected;
        }

        public static Comb.mzI[] calFunction(Comb.mzI[] expData, fitDataStrt fitData)
        {
            double a = fitData.A;
            double b = fitData.alpha;
            double c = fitData.B;
            
            Comb.mzI[] fdata = new Comb.mzI[expData.Length];

            for (int i = 1; i <= expData.GetUpperBound(0); i++)
            {
                fdata[i].mz = expData[i].mz;
                fdata[i].I = a + b * fdata[i].mz + c * fdata[i].mz * fdata[i].mz;
            }


            return fdata;
        }


        public static Comb.mzI gaussDblExp(double x, double mu, double sigma, double alpha) 
        {
            Comb.mzI gDblExp = new Comb.mzI();

            gDblExp.mz = x;
            gDblExp.I = (1-alpha)* gaussian(x, mu, sigma).I+ alpha * dblExp(x,mu,sigma).I;

            return gDblExp;
        }

        /// <summary>
        ///  gaussian(x,mu,sigma)
        /// </summary>
        /// <param name="x">(double)x</param>
        /// <param name="mu">(double)gaussian center</param>
        /// <param name="sigma">(double)gaussian width</param>
        /// <returns>(Comb.mzI)Gaussian height</returns>
        public static Comb.mzI gaussian(double x, double mu,double sigma) 
        {


            Comb.mzI gauss = new Comb.mzI();
            sigma = Math.Abs(sigma);

                gauss.mz = x;
                gauss.I = (1 / (Math.Sqrt(2 * Math.PI)*sigma))*Math.Exp(-((x-mu)*(x-mu))/(2*sigma*sigma));

            return gauss;
        }

        /// <summary>
        /// Double exponential(x,mu,sigma)
        /// </summary>
        /// <param name="x">(double)x</param>
        /// <param name="mu">(double)Double exponential center</param>
        /// <param name="sigma">(double)double exponential width</param>
        /// <returns>(Comb.mzI)double exponential height</returns>
        public static Comb.mzI dblExp(double x, double mu, double sigma)
        {
            Comb.mzI dEx = new Comb.mzI();
            sigma = Math.Abs(sigma);

            dEx.mz = x;
            dEx.I = (1/(2*sigma)) * Math.Exp(-Math.Abs(x - mu) / sigma);

            return dEx;
        }


        #region initial conditions
        
        
        public static fitDataStrt getInitialConditions( fitDataStrt initialConds, 
                                                        Comb.mzI[] expData,
                                                        Comb.mzI[] intensities,
                                                        double sn_f,
                                                        int charge, 
                                                        double deltaR)
        {
            double h0=getMax(expData,intensities[0].mz+(initialConds.deltaMz-1)/charge,intensities[0].mz+(initialConds.deltaMz+1)/charge);
            double hmark=getMax(expData,intensities[0].mz+(initialConds.deltaMz+2*deltaR-1)/charge,intensities[0].mz+(initialConds.deltaMz+2*deltaR+1)/charge);

            double alpha = initialConds.alpha;
            double sigma = initialConds.sigma;
            double efficiency = initialConds.f;
            double PI = Math.PI;

            initialConds.B = hmark / ((1 - alpha) / (Math.Sqrt(2 * PI) * sigma) + alpha / (2 * sigma)) / intensities[0].I;
            initialConds.A = h0 / ((1 - alpha) / (Math.Sqrt(2 * PI) * sigma) + alpha / (2 * sigma))/intensities[0].I;
            initialConds.signoise = averagef(expData,sn_f);

            return initialConds;
        }

        public static double getMax(Comb.mzI[] expData, double rangeMin, double rangeMax) 
        {
            double max = 0;
            try
            {
                for (int i = 0; i <= expData.GetUpperBound(0); i++)
                {
                    if (expData[i].mz >= rangeMin && expData[i].mz <= rangeMax && expData[i].I > max) max = expData[i].I;
                }
            }
            catch { }

            return max;
        }

        public static double getMin(Comb.mzI[] expData, double rangeMin, double rangeMax) 
        {
            double min = getMax(expData, rangeMin, rangeMax);
            try
            {
                for (int i = 0; i <= expData.GetUpperBound(0); i++)
                {
                    if (expData[i].mz >= rangeMin && expData[i].mz <= rangeMax && expData[i].I < min) min = expData[i].I;
                }
            }
            catch { }

            return min;
        }

        private static double averagef(Comb.mzI[] expData, double sn_f) 
        {
            double avg = 0;
            try
            {
                for (int i = 0; i <= expData.GetUpperBound(0); i++)
                {
                    avg += Math.Pow(expData[i].I, sn_f);
                }
                avg = Math.Pow(avg / (expData.GetUpperBound(0) + 1), 1 / sn_f);
            }
            catch { }

            return avg;
        }

        #endregion

        public static instrumentParamsStrt readFitParametersXML(string fileXml, string instrument)
        {

            instrumentParamsStrt parameters = new instrumentParamsStrt();

            //Initialize necessary objets for XML reading
            XmlTextReader reader = new XmlTextReader(fileXml);
            XmlNodeType nType = reader.NodeType;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(reader);

            //Get the instrument tags
            XmlNodeList xmlnodeInstrument = xmldoc.GetElementsByTagName("instrument");
            
            //search the correct <instrument> entry
            for (int i = 0; i < xmlnodeInstrument.Count; i++)
            {
                if (xmlnodeInstrument[i].Attributes["id"].Value.Trim() == instrument.Trim()) 
                {
                    for (int j = 0; j < xmlnodeInstrument[i].ChildNodes.Count; j++) 
                    {
                        if (xmlnodeInstrument[i].ChildNodes[j].Name == "resolution") 
                        {
                            if (xmlnodeInstrument[i].ChildNodes[j].InnerText.Trim() == "LOW")
                            {
                                parameters.instResolution = Resolution.LOW;
                            }
                            if (xmlnodeInstrument[i].ChildNodes[j].InnerText.Trim() == "HIGH")
                            {
                                parameters.instResolution = Resolution.HIGH;
                            }
                        }

                        if (xmlnodeInstrument[i].ChildNodes[j].Name == "kmax") 
                        {
                            parameters.kmax = int.Parse(xmlnodeInstrument[i].ChildNodes[j].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                        }

                        if (xmlnodeInstrument[i].ChildNodes[j].Name == "initialFitParams") 
                        {
                            for (int k = 0; k < xmlnodeInstrument[i].ChildNodes[j].ChildNodes.Count; k++)
                            {
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "alpha") 
                                {
                                    parameters.alpha = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "sigma")
                                {
                                    parameters.sigma = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "deltaR")
                                {
                                    parameters.deltaR = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "efficiency")
                                {
                                    parameters.f = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "deltaMZ")
                                {
                                    parameters.deltaMz = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "SN_f")
                                {
                                    parameters.sn_f = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                        }
                        if (xmlnodeInstrument[i].ChildNodes[j].Name == "deltaFitParams")
                        {
                            for (int k = 0; k < xmlnodeInstrument[i].ChildNodes[j].ChildNodes.Count; k++)
                            {
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "A")
                                {
                                    parameters.varA = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "B")
                                {
                                    parameters.varB = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "efficiency")
                                {
                                    parameters.varf = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "sigma")
                                {
                                    parameters.varSigma = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "alpha")
                                {
                                    parameters.varAlpha = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                if (xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].Name == "SN")
                                {
                                    parameters.varSn = double.Parse(xmlnodeInstrument[i].ChildNodes[j].ChildNodes[k].InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                        }
                    }
                }
            }
            
     
            return parameters;

        }

    }
}
