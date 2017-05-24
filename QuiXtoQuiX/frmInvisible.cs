using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QuiXoT.DA_Raw;
using MSFileReaderLib;

namespace msnPerFull
{
    class rawStats
    {
        private MSFileReaderLib.IXRawfile4 rawFile;
        private int[] freqMSn = new int[40];
        public spectrumTypes[] spectrumTypeCache = new spectrumTypes[0];
        public string workingRAWFilePath = "";
        public spectrumCache sCache = new spectrumCache();

        public double[,] getSpectrumFromCache(int spectrumNumber)
        {
            double[,] spectrum = sCache.getSpectrum(spectrumNumber);

            if (spectrum.Length == 0) return null;
            else return spectrum;
        }


        public int initialiseSpectrumTypes(int numOfScans)
        {
            int cacheSize = sCache.cacheSize;
            try
            {
                spectrumTypeCache = new spectrumTypes[numOfScans + 1];
            }
            catch (Exception ex)
            {
                if (ex.Message == "Exception of type 'System.OutOfMemoryException' was thrown.")
                {
                    cacheSize = sCache.lowerCacheSize(0.9);
                    //MessageBox.Show("Memory is full, the spectrum cache will be lowered to " +
                    //    cacheSize.ToString() + ".");
                    //Application.DoEvents();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    spectrumTypeCache = new spectrumTypes[numOfScans + 1];
                }
                else
                {
                    MessageBox.Show("There was an error at " + System.Reflection.MethodInfo.GetCurrentMethod()
                        + ".\n" + "Message was: " + ex.Message);
                    Application.DoEvents();
                }
            }

            return cacheSize;
        }

        public rawStats()
        {
            rawFile = (IXRawfile4)new MSFileReaderLib.MSFileReader_XRawfile();

            //rawFile.Open(fileName);
            //rawFile.SetCurrentController(0, 1);
            //rawFile.GetFirstSpectrumNumber(ref firstSpectrumNumber);
            //rawFile.GetNumSpectra(ref numSpectscrum);
        }

        public void openRawFast(string rawFilePath)
        {
            openRawFast(rawFilePath, 0);
        }

        public void openRawFast(string rawFilePath, int cacheSize)
        {
            if (rawFilePath != workingRAWFilePath)
            {
                closeRaw();
                workingRAWFilePath = rawFilePath;
                openRaw(workingRAWFilePath, cacheSize);
            }
        }

        private void openRaw(string rawFilePath)
        {
            openRaw(rawFilePath, 0);
        }

        private void openRaw(string rawFilePath, int cacheSize)
        {
            workingRAWFilePath = rawFilePath;

            if (cacheSize == 0) sCache = new spectrumCache();
            else sCache = new spectrumCache(cacheSize);

            rawFile.Open(rawFilePath);
            rawFile.SetCurrentController(0, 1);
        }

        public void closeRaw()
        {
            workingRAWFilePath = "";

            //long t1 = DateTime.Now.Ticks;

            rawFile.Close();

            //long t2 = DateTime.Now.Ticks;

            //StreamWriter w = new StreamWriter(@"D:\DATUMARO\trabajo\tareas en curso\tareas1 ug\pruebas QuiXtoQuiX\tiempos.txt", true);
            //w.WriteLine(string.Concat(t1, "\t", t2));
            //w.Close();
        }

        public int lastSpectrumNumber()
        {
            int lastScanNumber = 0;

            rawFile.GetLastSpectrumNumber(ref lastScanNumber);

            return lastScanNumber;
        }

        public int numSpectra()
        {
            int numberOfSpectra = 0;

            rawFile.GetNumSpectra(ref numberOfSpectra);

            return numberOfSpectra;
        }

        public int retentionTimeToIndex(double _retentionTime)
        {
            int index = 0;

            rawFile.ScanNumFromRT(_retentionTime, ref index);

            return index;
        }

        public int getScanNumberFromRT(double _RT)
        {
            int scanNumber = 0;
            rawFile.ScanNumFromRT(_RT, ref scanNumber);

            return scanNumber;
        }

        public double getRTfromScanNumber(int _scanNumber)
        {
            double RT = 0;
            rawFile.RTFromScanNum(_scanNumber, ref RT);

            return RT;
        }

        public int getScanNumberOfPrevOrNextSpectrumByType(int _scan, spectrumPosition _position, spectrumTypes _typeOfSpec)
        {
            double RT = 0;
            rawFile.RTFromScanNum(_scan, ref RT);

            return getScanNumberOfPrevOrNextSpectrumByType(RT, _position, _typeOfSpec);
        }

        public int getScanNumberOfPrevOrNextSpectrumByType(double _RT,
            spectrumPosition _position,
            spectrumTypes _typeOfSpec)
        {
            int scanNumber = 0;
            rawFile.ScanNumFromRT(_RT, ref scanNumber);
            int direction = 0;
            int lastScanNumber = 0;
            rawFile.GetLastSpectrumNumber(ref lastScanNumber);

            switch (_position)
            {
                case spectrumPosition.next:
                    {
                        scanNumber++;
                        direction = 1;
                        break;
                    }
                case spectrumPosition.sameOrNext:
                    {
                        direction = 1;
                        break;
                    }
                case spectrumPosition.previous:
                    {
                        scanNumber--;
                        direction = -1;
                        break;
                    }
                case spectrumPosition.sameOrPrevious:
                    {
                        direction = -1;
                        break;
                    }
                case spectrumPosition.nearestInScanNumber:
                    {
                        int thisScanNumber = 0;
                        rawFile.ScanNumFromRT(_RT, ref thisScanNumber);
                        int previousSN = getScanNumberOfPrevOrNextSpectrumByType(_RT, spectrumPosition.sameOrPrevious, _typeOfSpec);
                        int nextSN = getScanNumberOfPrevOrNextSpectrumByType(_RT, spectrumPosition.sameOrNext, _typeOfSpec);

                        // usually this happens when the en of the war is reached
                        if (nextSN == 0) return previousSN;
                        if (previousSN == 0) return nextSN;

                        if (thisScanNumber - previousSN < nextSN - thisScanNumber)
                            return previousSN;
                        else
                            return nextSN;
                    }
                case spectrumPosition.nearestInRT:
                    {
                        int previousSN = getScanNumberOfPrevOrNextSpectrumByType(_RT, spectrumPosition.sameOrPrevious, _typeOfSpec);
                        int nextSN = getScanNumberOfPrevOrNextSpectrumByType(_RT, spectrumPosition.sameOrNext, _typeOfSpec);

                        // usually this happens when the en of the war is reached
                        if (nextSN == 0) return previousSN;
                        if (previousSN == 0) return nextSN;

                        double previousRT = 0;
                        double nextRT = 0;
                        rawFile.RTFromScanNum(previousSN, ref previousRT);
                        rawFile.RTFromScanNum(nextSN, ref nextRT);

                        if (_RT - previousRT < nextRT - _RT)
                            return previousSN;
                        else
                            return nextSN;
                    }
                case spectrumPosition.same:
                    { // attention! this will not always be a fullScan!
                        return scanNumber;
                    }
            }

            while (scanNumber > 0 && scanNumber < lastScanNumber)
            {
                if (spectrumTypeFromScanNumber(scanNumber) == _typeOfSpec)
                    return scanNumber;
                else
                    scanNumber += direction;
            }

            return 0;
        }

        public double[,] getRTandMaxFromScanNumberOfPrevOrNextFull(int _scanNumber,
                                                            double _referenceMZ,
                                                            double _tolerance,
                                                            spectrumPosition _position)
        {
            int direction = 0;
            int lastSpectrum = 0;
            int newScanNumber = _scanNumber;

            switch (_position)
            {
                case spectrumPosition.previous:
                    {
                        direction = -1;
                        break;
                    }
                case spectrumPosition.next:
                    {
                        direction = 1;
                        break;
                    }
                default:
                    return getRTandMaxFromScanNumber(_scanNumber, _referenceMZ, _tolerance);
            }

            rawFile.GetLastSpectrumNumber(ref lastSpectrum);

            newScanNumber += direction;
            while (newScanNumber > 0 && newScanNumber <= lastSpectrum)
            {
                if (spectrumTypeFromScanNumber(newScanNumber) == spectrumTypes.Full)
                    return getRTandMaxFromScanNumber(newScanNumber, _referenceMZ, _tolerance);
                newScanNumber += direction;
            }

            return null;
        }

        public double[,] getRTandMaxFromScanNumber(int _scanNumber,
                                                        double _referenceMZ,
                                                        double _tolerance) // in ppms
        {
            double[,] RTintensity = new double[2, 1];
            double MZstart = _referenceMZ * (1 - _tolerance / 1e6);
            double MZend = _referenceMZ * (1 + _tolerance / 1e6);
            double maxIntensity = 0;
            double MZforMaxIntensity = 0;
            double RTatMaxIntensity = 0;

            if (_scanNumber > 0)
            {
                rawFile.RTFromScanNum(_scanNumber, ref RTatMaxIntensity);

                object provisionalSpectrum = getSpectrum(_scanNumber, ref sCache.cacheSize);
                double[,] spectrum = (double[,])provisionalSpectrum;

                int spectrumLength = spectrum.GetUpperBound(1) + 1;
                for (int i = 0; i < spectrumLength; i++)
                {
                    // we assume spectrum is ordered ascending in mz
                    if (spectrum[0, i] > MZstart)
                    {
                        if (spectrum[0, i] > MZend) break;

                        if (spectrum[1, i] > maxIntensity)
                        {
                            MZforMaxIntensity = spectrum[0, i];
                            maxIntensity = spectrum[1, i];
                        }
                    }
                }

                RTintensity[0, 0] = RTatMaxIntensity;
                RTintensity[1, 0] = maxIntensity;
            }
            else
            {
                RTintensity[0, 0] = 0;
                RTintensity[1, 0] = 0;
            }

            

            return RTintensity;
        }

        public double[,] getChromatogramSectionData(double _RTreference,
                                        double _RTsectionWidth,
                                        double _referenceMZ,
                                        double _tolerance)
        {
            double RTstart = _RTreference - _RTsectionWidth / 2;
            double RTend = _RTreference + _RTsectionWidth / 2;

            int[] scanNumber = getScanNumbersBetweenRTs(RTstart, RTend, spectrumTypes.Full);
            double[,] chromatogramSection = new double[2, scanNumber.Length];

            for (int i = 0; i < scanNumber.Length; i++)
            {
                double[,] provScan = getRTandMaxFromScanNumber(scanNumber[i], _referenceMZ, _tolerance);
                chromatogramSection[0, i] = provScan[0, 0];
                chromatogramSection[1, i] = provScan[1, 0];
            }

            return chromatogramSection;
        }

        public void writeFileWithChromatogramSection(string _fileName,
                                                    double[,] _chromatogramSection)
        {
            StreamWriter writer = new StreamWriter(_fileName);

            int chromatogramLength = _chromatogramSection.GetUpperBound(1) + 1;
            for (int i = 0; i < chromatogramLength; i++)
            {
                string line = _chromatogramSection[0, i] + "\t" + _chromatogramSection[1, i];
                writer.WriteLine(line);
            }

            writer.Close();
        }

        public int[] getScanNumbersBetweenRTs(double _RTstart, double _RTend, spectrumTypes _scanType)
        {
            int SNstart = 0;
            int SNend = 0;

            double maxRT = getMaxRT();

            if (_RTstart < 0) _RTstart = 0;
            if (_RTend > maxRT) _RTend = maxRT;

            rawFile.ScanNumFromRT(_RTstart, ref SNstart);
            rawFile.ScanNumFromRT(_RTend, ref SNend);

            int[] provScanNumberList = new int[SNend - SNstart + 1];

            int totScans = 0;
            for (int scan = SNstart; scan <= SNend; scan++)
            {
                spectrumTypes provScanType = spectrumTypeFromScanNumber(scan);
                if (provScanType == _scanType)
                {
                    provScanNumberList[totScans] = scan;
                    totScans++;
                }
            }

            // this is to eliminate the empty elements
            int[] scanNumberList = new int[totScans];
            for (int i = 0; i < totScans; i++)
                scanNumberList[i] = provScanNumberList[i];

            return scanNumberList;
        }

        public double getMaxRT()
        {
            double maxRT = 0;
            int lastSpectrum = 0;
            rawFile.GetLastSpectrumNumber(ref lastSpectrum);
            rawFile.RTFromScanNum(lastSpectrum, ref maxRT);
            return maxRT;
        }

        public spectrumTypes spectrumTypeFromScanNumber(int _myScanNumber)
        {
            spectrumTypes type = new spectrumTypes();
            
            // Definitions:
            // ScanType not defined --> -1
            // ScanTypeFull 0
            // ScanTypeSIM 1
            // ScanTypeZoom 2
            // ScanTypeSRM 3
            int pnScanType = -1;

            if (spectrumTypeCache[_myScanNumber] != spectrumTypes.Unknown)
                return spectrumTypeCache[_myScanNumber];

            rawFile.GetScanTypeForScanNum(_myScanNumber, ref pnScanType);

            switch (pnScanType)
            {
                case 0: // full scan, MS or MSn
                    int pnMSOrder = 0;
                    rawFile.GetMSOrderForScanNum(_myScanNumber, ref pnMSOrder);
                    switch (pnMSOrder)
                    {
                        case 1:
                            type = spectrumTypes.Full;
                            break;
                        case 2:
                            type = spectrumTypes.MSMS;
                            break;
                        default:
                            type = spectrumTypes.Other;
                            break;
                    }
                    break;
                case 1:
                    type = spectrumTypes.SIM;
                    break;
                case 2:
                    type = spectrumTypes.ZoomScan;
                    break;
                case 3:
                    type = spectrumTypes.SRM;
                    break;
                default: // including the default -1
                    type = spectrumTypes.ERROR;
                    break;
            }

            spectrumTypeCache[_myScanNumber] = type;

            return type;
        }

        public object getSpectrum(int _scanNumber, ref int _cacheSize)
        {
            double[,] specFromCache = getSpectrumFromCache(_scanNumber);
            
            if (specFromCache != null)
                return specFromCache;

            object myPeakFlags = null;
            int myArraySize = 0;
            object myMassList = null;
            double myCentroidPeakWidth = 0;
            #region GetMassListRangeFromScanNum usage
            //        string bstrFiler = "";
    //        int nIntensityCutoffType = 0;
    //        int intensityCutoffValue = 0;
    //        int nMaxNumberOfPeaks = 0;
    //        int bCentroidResult = 0;
    //        double pdCentroidPeakWidth = 0;
    //        object pvarMassList = 0;
    //        object pvarPeakFlags = 0;
    //        string szMassRange1 = "";
    //        int pnArraySize = 0;
    //        rawFile.GetMassListRangeFromScanNum(ref _scanNumber, bstrFiler, nIntensityCutoffType, intensityCutoffValue,
    //nMaxNumberOfPeaks, bCentroidResult, ref pdCentroidPeakWidth, ref pvarMassList, ref pvarPeakFlags,
            //szMassRange1, ref pnArraySize);

            #endregion
            //long t1 = DateTime.Now.Ticks;

            try
            {
                rawFile.GetMassListFromScanNum(ref _scanNumber,
                                                "", 0, 0, 0, 0,
                                                ref myCentroidPeakWidth,
                                                ref myMassList,
                                                ref myPeakFlags,
                                                ref myArraySize);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Exception of type 'System.OutOfMemoryException' was thrown.")
                {
                    _cacheSize = sCache.lowerCacheSize(0.9);
                    //MessageBox.Show("Memory is full, the spectrum cache will be lowered to " +
                    //    _cacheSize.ToString() + ".");
                    //Application.DoEvents();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    rawFile.GetMassListFromScanNum(ref _scanNumber,
                                                "", 0, 0, 0, 0,
                                                ref myCentroidPeakWidth,
                                                ref myMassList,
                                                ref myPeakFlags,
                                                ref myArraySize);
                }
                else
                {
                    MessageBox.Show("There was an error at " + System.Reflection.MethodInfo.GetCurrentMethod()
                        + ".\n" + "Message was: " + ex.Message);
                    Application.DoEvents();
                }
            }

            //long t2 = DateTime.Now.Ticks;
            ////double milliseconds = (t2 - t1) / 10000.0;

            //StreamWriter w = new StreamWriter(@"D:\DATUMARO\trabajo\tareas en curso\tareas1 ug\pruebas QuiXtoQuiX\tiempos.txt", true);
            //w.WriteLine(string.Concat(t1, "\t", t2));
            //w.Close();

            spectrumlet speclet = new spectrumlet();
            speclet.scanNumber = _scanNumber;
            speclet.spectrum = (double[,])myMassList;
            sCache.add(speclet);

            return myMassList;
        }

        public int[] fullScansAround(int _scanMSMS, int _numOfScans)
        {
            int[] scansAround = new int[_numOfScans];

            int i = 1;
            int n = _numOfScans / 2 - 1;
            while (n >= 0)
            {
                if (_scanMSMS - i >= 0)
                {
                    if (spectrumTypeFromScanNumber(_scanMSMS - i) == spectrumTypes.Full)
                    {
                        scansAround[n] = _scanMSMS - i;
                        n--;
                        i++;
                    }
                    else
                        i++;
                }
                else
                    break;
            }

            i = 1;
            n = _numOfScans / 2;
            while (n < _numOfScans)
            {
                if (_scanMSMS + i < spectrumTypeCache.Length)
                {
                    if (spectrumTypeFromScanNumber(_scanMSMS + i) == spectrumTypes.Full)
                    {
                        scansAround[n] = _scanMSMS + i;
                        n++;
                        i++;
                    }
                    else
                        i++;
                }
                else
                    break;
            }

            return scansAround;
        }

        public int[] fullScansBetween(int _scanStart, int _scanEnd)
        {
            int numOfScans = _scanEnd - _scanStart + 1;
            int[] scansBetween = new int[numOfScans];

            int n = 0;

            for (int i = _scanStart; i <= _scanEnd; i++)
            {
                if (spectrumTypeFromScanNumber(i) == spectrumTypes.Full)
                {
                    scansBetween[n] = i;
                    n++;
                }
            }

            return scansBetween;
        }

        public class spectrumlet
        {
            public int scanNumber = 0;
            public double[,] spectrum = new double[0, 0];
        }

        public class spectrumCache
        {
            private int defaultCacheSize = 1000;
            private int minimumCacheSize = 10;
            
            public spectrumCache(int size)
            {
                cacheSize = size;
                initialiseCache();
            }

            public spectrumCache()
            {
                cacheSize = defaultCacheSize;
                initialiseCache();
            }

            public int lowerCacheSize(double ratio)
            {
                cacheSize = (int)Math.Round(ratio * (double)cacheSize);
                if (cacheSize < minimumCacheSize) cacheSize = minimumCacheSize;
                initialiseCache();

                return cacheSize;
            }

            private void initialiseCache()
            {
                cache = new spectrumlet[cacheSize];
                for (int i = 0; i < cache.Length; i++)
                    cache[i] = new spectrumlet();
            }
            
            public int cacheSize;
            public spectrumlet[] cache;

            public void add(spectrumlet spec)
            {
                for (int i = cacheSize - 1; i > 0; i--)
                    cache[i] = cache[i - 1];

                cache[0] = spec;
            }

            public double[,] getSpectrum(int specNumber)
            {
                double[,] spectrum = new double[0, 0];
                int position = 0;

                for (int i = 0; i < cacheSize; i++)
                {
                    if(cache[i]!=null)
                        if (cache[i].scanNumber == specNumber)
                        {
                            spectrum = cache[i].spectrum;
                            position = i;
                            break;
                        }
                }

                if (spectrum.Length > 0) reorderSpectra(specNumber, spectrum, position);

                return spectrum;
            }

            private void reorderSpectra(int specNumber, double[,] spectrum, int position)
            {
                if (position > 0)
                {
                    for (int i = position; i > 0; i--)
                    {
                        cache[i].spectrum = cache[i - 1].spectrum;
                        cache[i].scanNumber = cache[i - 1].scanNumber;
                    }
                }

                cache[0].spectrum = spectrum;
                cache[0].scanNumber = specNumber;
            }
        }

        public enum spectrumPosition
        {
            same,
            sameOrNext,
            next,
            sameOrPrevious,
            previous,
            nearestInScanNumber,
            nearestInRT
        }
    }
}


/* old namespace QuiXtoQuiX
namespace QuiXtoQuiX
{
    public partial class frmInvisible : Form
    {
        public spectrumTypes[] spectrumTypeCache = new spectrumTypes[0];
        public string workingRAWFilePath = "";

        public void initialiseSpectrumTypes(int numOfScans)
        {
            spectrumTypeCache = new spectrumTypes[numOfScans + 1];
        }

        public frmInvisible()
        {
            InitializeComponent();
        }

        public void openRaw(string rawFilePath)
        {
            myRaw.Open(rawFilePath);
            myRaw.SetCurrentController(0, 1);
        }

        public void closeRaw()
        {
            workingRAWFilePath = "";
            myRaw.Close();
        }

        public int numSpectra()
        {
            int numberOfSpectra = 0;

            myRaw.GetNumSpectra(ref numberOfSpectra);

            return numberOfSpectra;
        }

        public int lastSpectrumNumber()
        {
            int lastScanNumber = 0;

            myRaw.GetLastSpectrumNumber(ref lastScanNumber);

            return lastScanNumber;
        }

        public int[] fullScansBetween(int _scanStart, int _scanEnd)
        {
            int numOfScans = _scanEnd - _scanStart + 1;
            int[] scansBetween = new int[numOfScans];

            int n = 0;

            for (int i = _scanStart; i <= _scanEnd; i++)
            {
                if (spectrumTypeFromScanNumber(i) == spectrumTypes.Full)
                {
                    scansBetween[n] = i;
                    n++;
                }
            }

            return scansBetween;
        }

        public int[] fullScansAround(int _scanMSMS, int _numOfScans)
        {
            int[] scansAround = new int[_numOfScans];

            int i = 1;
            int n = _numOfScans / 2 - 1;
            while (n >= 0)
            {
                if (_scanMSMS - i >= 0)
                {
                    if (spectrumTypeFromScanNumber(_scanMSMS - i) == spectrumTypes.Full)
                    {
                        scansAround[n] = _scanMSMS - i;
                        n--;
                        i++;
                    }
                    else
                        i++;
                }
                else
                    break;
            }

            i = 1;
            n = _numOfScans/2;
            while (n < _numOfScans)
            {
                if (_scanMSMS + i < spectrumTypeCache.Length)
                {
                    if (spectrumTypeFromScanNumber(_scanMSMS + i) == spectrumTypes.Full)
                    {
                        scansAround[n] = _scanMSMS + i;
                        n++;
                        i++;
                    }
                    else
                        i++;
                }
                else
                    break;
            }

            return scansAround;
        }

        public object getSpectrum(int _scanNumber)
        {
            object myPeakList = null;
            object myPeakListFlags = null;
            int myPeakCount = 0;

            int m = myRaw.GetMassListFromScanNum(ref _scanNumber,
                                            "", 0, 0, 0, 0,
                                            ref myPeakList,
                                            ref myPeakListFlags,
                                            ref myPeakCount);
            return myPeakList;
        }

        public double indexToRetentionTime(int _index)
        {
            double retentionTime = 0;

            myRaw.RTFromScanNum(_index, ref retentionTime);

            return retentionTime;
        }

        public int retentionTimeToIndex(double _retentionTime)
        {
            int index = 0;

            myRaw.ScanNumFromRT(_retentionTime, ref index);

            return index;
        }

        public int spectrumCount()
        {
            int count = 0;

            myRaw.GetNumSpectra(ref count);

            return count;
        }

        public int getNumPackets(int _scanNumber)
        {
            int pnNumPackets=0;
            double pdStartTime=0;
            double pdHighMass=0;
            double pdLowMass = 0;
            double pdTIC = 0;
            double pdBasePeakMass = 0;
            double pdBasePeakIntensity = 0;
            int pnNumChannels = 0;
            bool pbUniformTime = false;
            double pfFrequency = 0;

            myRaw.GetScanHeaderInfoForScanNum(_scanNumber,
                                                ref pnNumPackets,
                                                ref pdStartTime,
                                                ref pdLowMass,
                                                ref pdHighMass,
                                                ref pdTIC,
                                                ref pdBasePeakMass,
                                                ref pdBasePeakIntensity,
                                                ref pnNumChannels,
                                                ref pbUniformTime,
                                                ref pfFrequency );

            return pnNumPackets;
        }

        public spectrumTypes spectrumTypeFromScanNumber(int _myScanNumber)
        {
            string myFilterZ = "Z";
            string myFilterMS2 = "Full ms2";
            string mrFilterFull = "Full ms";
            bool errorDuringProcessZ = false;
            bool errorDuringProcessMS2 = false;
            bool errorDuringProcessFull = false;
            spectrumTypes type = new spectrumTypes();
            bool is_Z = false;
            bool is_MS2 = false;
            bool is_Full = false;
            bool errorDuringProcess = false;

            if (spectrumTypeCache[_myScanNumber] != spectrumTypes.Unknown)
                return spectrumTypeCache[_myScanNumber];
            
            is_Z = isWorking(_myScanNumber, myFilterZ, ref errorDuringProcessZ);
            if (!is_Z)
            {
                is_MS2 = isWorking(_myScanNumber, myFilterMS2, ref errorDuringProcessMS2);
                if (!is_MS2)
                {
                    is_Full = isWorking(_myScanNumber, mrFilterFull, ref errorDuringProcessFull);
                }
            }

            errorDuringProcess = errorDuringProcessMS2 || errorDuringProcessZ || errorDuringProcessFull;

            if (errorDuringProcess)
                type = spectrumTypes.ERROR;
            if (is_Z)
                type = spectrumTypes.ZoomScan;
            if (is_MS2)
                type = spectrumTypes.MSMS;
            if (is_Full)
                type = spectrumTypes.Full;
            if ((!is_Full) && (!is_MS2) && (!is_Z) && (!errorDuringProcessFull))
                type = spectrumTypes.Other;

            spectrumTypeCache[_myScanNumber] = type;

            return type;
        }

        public bool isWorking(int _myScanNumber, string _myFilter, ref bool _errorDuringProcess)
        {
            object miPeakList = null;
            object miPeakListFlags = null;
            int myPeakCount = 0;
            int myScanNumberControl = _myScanNumber;
            bool itWorks = false;

            int lRet3 = myRaw.GetMassListFromScanNum(ref _myScanNumber,
                                            _myFilter, 0, 0, 0, 0,
                                            ref miPeakList,
                                            ref miPeakListFlags,
                                            ref myPeakCount);

            // lRet3==0 means there were no such scans found in all the file.
            _errorDuringProcess = !(lRet3 == 1 || lRet3 == 0);

            itWorks = (myScanNumberControl == _myScanNumber) && !_errorDuringProcess && !(lRet3 == 0);

            return itWorks;
        }

        public int getNumberOfScansInRaw()
        {
            int scansInRaw = new int();
            //double ddd = new double();
            //object ooo = null;
            //object ooo2 = null;
            //int iii = 0;
            //int scanNumber = 5943;
            //string sss = "";
            //string filter = "Full";

            int lRet3 = myRaw.GetNumSpectra(ref scansInRaw);

            return scansInRaw;
        }

        //public double getPrecursorMass(int _myScanNumber)
        //{
        //    double ddd = 0;
        //    string sss = "";
        //    object ooo = null;
        //    object ooo2 = null;
        //    int iii = 0;
        //    int iii2 = 0;
        //    int iii3 = 0;
        //    int iii4 = 0;
        //    int iii5 = 0;
        //    int iii6 = 0;
        //    int iii7 = 0;
        //    DateTime dt=new DateTime();

        //    myRaw.GetMassListFromScanNum(ref _myScanNumber, "ms2", 0, 0, 0, 0, ref ooo, ref ooo2, ref iii);

        //    //myRaw.GetNoiseData(ref ooo, ref iii);

        //    //int uno = myRaw.GetAverageMassList(ref iii, ref iii2, ref iii3, ref iii4, ref iii5, ref iii6, "ms2", 100, 100, 100, ref ooo, ref ooo2, ref iii7);

        //    //int uno = myRaw.GetTuneDataLabels(1, ref ooo, ref iii);

        //    return ddd;

        //    //int uno = myRaw.GetStatusLogValueForScanNum(_myScanNumber, "ms2", ref ddd, ref ooo);

        //    //int uno = myRaw.GetStatusLogForScanNum(_myScanNumber, ref ddd, ref ooo, ref ooo2, ref iii);

        //    //int uno = myRaw.GetFilterForScanNum(_myScanNumber, ref aggg);

        //    //int lret3 = myRaw.GetFilters(ref ooo, ref iii);

        //    //int lret5 = myRaw.GetLabelData(ref ooo, ref ooo2, ref _myScanNumber);

        //    //int lret6 = myRaw.GetLastSpectrumNumber(ref iii);

        //    //int lret7 = myRaw.GetMassListFromScanNum(ref _myScanNumber, sss, iii, iii2, iii3, iii4, ref ooo, ref ooo2, ref iii5);

        //    //uno = myRaw.GetMassListRangeFromScanNum(ref _myScanNumber, "ms2", 2, 40, 3, 3, ref ooo, ref ooo2, "", ref iii);

        //    //int lret4 = myRaw.GetHighMass(ref ddd);

        //    //int lret2 = myRaw.GetFirstSpectrumNumber(ref iii);

        //    //int lRet3 = myRaw.GetPrecursorInfoFromScanNum(_myScanNumber,
        //    //                                                ref precursorInfo,
        //    //                                                ref pnArraySize);
        //}
    }
}
 * 
 * */