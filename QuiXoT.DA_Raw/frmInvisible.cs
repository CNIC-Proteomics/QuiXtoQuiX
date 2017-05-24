using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuiXoT.math;
using QuiXoT.DA_Raw;

namespace QuiXoT.Forms
{
    public partial class frmInvisible : Form
    {
        public frmInvisible()
        {
            InitializeComponent();
        }

        public void openRaw(string rawFilePath)
        {
            myRaw.Open(rawFilePath);
            myRaw.SetCurrentController(0, 1);
        }

        public int numSpectra()
        {
            int numberOfSpectra = 0;

            myRaw.GetNumSpectra(ref numberOfSpectra);

            return numberOfSpectra;
        }

        public object getSpectrum(int _scanNumber)
        {
            object myPeakList = null;
            object myPeakListFlags = null;
            int myPeakCount = 0;

            myRaw.GetMassListFromScanNum(ref _scanNumber,
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

    //    public enum adquisitionMode
    //    {
    //        RetentionTime,
    //        position
    //    }
    //    public enum massUnits
    //    {
    //        amu,
    //        mmu,
    //        ppm,
    //    }
    //    public enum spectrumTypes
    //    {
    //        Full,
    //        ZoomScan,
    //        MSMS,
    //    }
    //    public enum spectrumPositions
    //    {
    //        previous,
    //        same,
    //        next
    //    }

    //    public class BinStackOptions
    //    {

    //        public BinStackOptions()
    //        {
    //            useParentalMass = false;
    //            parentalMassUnits = massUnits.amu;
    //            individualRow = false;
    //        }

    //        private spectrumTypes spectrumTypeVal;
    //        private adquisitionMode modeVal;
    //        private spectrumPositions spectrumPosVal;
    //        private float retentionTimeVal;
    //        private bool useParentalMassVal;
    //        private float parentalMassVal;
    //        private massUnits parentalMassUnitsVal;
    //        private bool individualRowVal;

    //        public spectrumTypes spectrumType
    //        {
    //            get { return spectrumTypeVal; }
    //            set { spectrumTypeVal = value; }
    //        }
    //        public adquisitionMode mode
    //        {
    //            get { return modeVal; }
    //            set { modeVal = value; }
    //        }
    //        public spectrumPositions spectrumPos
    //        {
    //            get { return spectrumPosVal; }
    //            set { spectrumPosVal = value; }
    //        }
    //        public float retentionTime
    //        {
    //            get { return retentionTimeVal; }
    //            set { retentionTimeVal = value; }
    //        }
    //        public bool useParentalMass
    //        {
    //            get { return useParentalMassVal; }
    //            set { useParentalMassVal = value; }
    //        }
    //        public float parentalMass
    //        {
    //            get { return parentalMassVal; }
    //            set { parentalMassVal = value; }
    //        }
    //        public massUnits parentalMassUnits
    //        {
    //            get { return parentalMassUnitsVal; }
    //            set { parentalMassUnitsVal = value; }
    //        }
    //        public bool individualRow
    //        {
    //            get { return individualRowVal; }
    //            set { individualRowVal = value; }
    //        }


    //    }

    //    public class DA_raw
    //    {



    //        public Comb.mzI[] extData;
    //        public string instrumentName;

    //        private XCALIBURFILESLib.XRaw _Raw = new XCALIBURFILESLib.XRaw();
    //        private XCALIBURFILESLib.XDetectorRead _Detector;
    //        private XCALIBURFILESLib.XSpectra _Spectra;


    //        /// <summary>
    //        /// Reads a given set of selected scans of a given raw 
    //        /// </summary>
    //        /// <param name="filePath">(string) directory path of the raws</param>
    //        /// <param name="rawfile">(string) name of the raw to open</param>
    //        /// <param name="scannumber">(int[]) set of MSMS id scans to read</param>
    //        /// <param name="options">options selected to create the binStack</param>
    //        /// <returns>(Comb.mzI[][]) spectrum of each selected scan</returns>
    //        public Comb.mzI[][] ReadScanRaw(string filePath, string rawfile, int[] scannumber, double[] parentMassList, BinStackOptions options) // string specType, string spectrumPosition)
    //        {
    //            int stepSearch;

    //            switch (options.spectrumPos)
    //            {
    //                case spectrumPositions.previous:
    //                    stepSearch = -1;
    //                    break;
    //                case spectrumPositions.next:
    //                    stepSearch = 1;
    //                    break;
    //                default:
    //                    stepSearch = 0;
    //                    break;
    //            }


    //            if (filePath == null || filePath.Length == 0)
    //                return null;
    //            if (rawfile == null || rawfile.Length == 0)
    //                return null;

    //            Comb.mzI[][] scansRaw = new Comb.mzI[scannumber.GetUpperBound(0) + 1][];

    //            //open the raw
    //            try
    //            {
    //                // start Xcalibur objects
    //                //GC.AddMemoryPressure(300000000);
    //                //_Raw = new XCALIBURFILESLib.XRaw();

    //                int iGeneration = GC.GetGeneration(_Raw);
    //                int iMaxGeneration = GC.MaxGeneration;



    //                long iTotalMem = GC.GetTotalMemory(true);
    //                string rawFilePath = filePath.ToString().Trim() + "\\" + rawfile.ToString().Trim();

    //                _Raw.Open(rawFilePath);
    //                _Detector = (XCALIBURFILESLib.XDetectorRead)_Raw.get_Detector(XCALIBURFILESLib.XDetectorTypes.XMS_Device, 1);

    //                _Spectra = _Detector.get_Spectra(0) as XCALIBURFILESLib.XSpectra;

    //            }
    //            catch
    //            {
    //                _Raw.Close();
    //                //MessageBox.Show("Could not open selected raw file: " + e.Message);
    //                return null;
    //            }

    //            XCALIBURFILESLib.XFilters _filter = _Detector.Filters as XCALIBURFILESLib.XFilters;

    //            #region mode: retention time
    //            if (options.mode == adquisitionMode.RetentionTime)
    //            {
    //                double startTime = new double();
    //                double endTime = new double();
    //                double centralTime = new double();
    //                short actualIndex = new short();
    //                double actualRetentionTime = new double();
    //                short firstScan = new short();
    //                short lastScan = new short();

    //                _Spectra.IndexToRetentionTime(1, ref actualIndex, ref startTime);
    //                _Spectra.IndexToRetentionTime(_Spectra.Count, ref actualIndex, ref endTime);

    //                for (int i = 0; i <= scannumber.GetUpperBound(0); i++)
    //                {
    //                    double parentMass = 0;
    //                    if (options.useParentalMass)
    //                    {
    //                        parentMass = parentMassList[i];
    //                    }

    //                    _Spectra.IndexToRetentionTime((short)scannumber[i], ref actualIndex, ref centralTime);

    //                    float timeMargin = options.retentionTime / (2 * 60);
    //                    double startCheckingTime = centralTime - timeMargin;
    //                    double stopCheckingTime = centralTime + timeMargin;

    //                    if (startCheckingTime < startTime) { startCheckingTime = startTime; }
    //                    if (stopCheckingTime > endTime) { stopCheckingTime = endTime; }

    //                    _Spectra.RetentionTimeToIndex(startCheckingTime, ref actualRetentionTime, ref firstScan);
    //                    _Spectra.RetentionTimeToIndex(stopCheckingTime, ref actualRetentionTime, ref lastScan);

    //                    try
    //                    {
    //                        for (int j = firstScan; j <= lastScan; j++)
    //                        {
    //                            short tentativeSpectrum = (short)j;
    //                            XCALIBURFILESLib.XFilter filt = (XCALIBURFILESLib.XFilter)_filter.ScanNumber(tentativeSpectrum);
    //                            string ff = filt.Text;

    //                            XCALIBURFILESLib.XSpectrumRead Xspec = new XCALIBURFILESLib.XSpectrumRead();

    //                            switch (options.spectrumType)
    //                            {
    //                                case spectrumTypes.Full:
    //                                    if (ff.Contains("Full") && !ff.Contains("ms2"))
    //                                        Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
    //                                    scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
    //                                    break;
    //                                case spectrumTypes.MSMS:
    //                                    if (ff.Contains("ms2"))
    //                                        Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
    //                                    scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
    //                                    break;
    //                                case spectrumTypes.ZoomScan:
    //                                    if (ff.Contains("Z") && !ff.Contains("Full"))
    //                                        Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
    //                                    scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
    //                                    break;
    //                            }

    //                        }
    //                    }
    //                    catch
    //                    {
    //                        //_Raw.Close();

    //                        //_Detector = null;
    //                        //_Spectra = null;
    //                        //GC.Collect();
    //                        //GC.WaitForPendingFinalizers();

    //                        ////MessageBox.Show("Could not open selected raw file: " + e.Message);
    //                        ////Application.DoEvents();
    //                        //return null;
    //                    }
    //                }
    //            }
    //            #endregion

    //            #region mode: position

    //            if (options.mode == adquisitionMode.position)
    //            {

    //                //for each identified spectrum (~each row of the QuiXML)
    //                for (int i = 0; i <= scannumber.GetUpperBound(0); i++)
    //                {

    //                    bool spectrumFound = false;

    //                    double parentMass = 0;
    //                    if (options.useParentalMass)
    //                    {
    //                        parentMass = parentMassList[i];
    //                    }

    //                    try
    //                    {
    //                        short tentativeSpectrum = (short)(scannumber[i] + stepSearch);
    //                        short spectrumToQuantitate = 0;

    //                        if (options.spectrumPos == spectrumPositions.same)
    //                        {
    //                            spectrumFound = true;
    //                            spectrumToQuantitate = tentativeSpectrum;
    //                        }

    //                        while (!spectrumFound)
    //                        {
    //                            try
    //                            {
    //                                XCALIBURFILESLib.XFilter filt = (XCALIBURFILESLib.XFilter)_filter.ScanNumber(tentativeSpectrum);  //(short)scannumber[i]
    //                                string ff = filt.Text;

    //                                switch (options.spectrumType)
    //                                {
    //                                    case spectrumTypes.Full:
    //                                        if (ff.Contains("Full") && !ff.Contains("ms2"))
    //                                        {
    //                                            spectrumFound = true;
    //                                            spectrumToQuantitate = tentativeSpectrum;
    //                                        }
    //                                        break;
    //                                    case spectrumTypes.MSMS:
    //                                        if (ff.Contains("ms2"))
    //                                        {
    //                                            spectrumFound = true;
    //                                            spectrumToQuantitate = tentativeSpectrum;
    //                                        }
    //                                        break;
    //                                    case spectrumTypes.ZoomScan:
    //                                        if (ff.Contains("Z") && !ff.Contains("Full"))
    //                                        {
    //                                            spectrumFound = true;
    //                                            spectrumToQuantitate = tentativeSpectrum;
    //                                        }
    //                                        break;
    //                                }

    //                            }
    //                            catch
    //                            {
    //                                _Raw.Close();

    //                                _Detector = null;
    //                                _Spectra = null;
    //                                GC.Collect();
    //                                GC.WaitForPendingFinalizers();

    //                                //MessageBox.Show("Could not open selected raw file: " + e.Message);
    //                                //Application.DoEvents();
    //                                return null;
    //                            }

    //                            tentativeSpectrum = (short)(tentativeSpectrum + stepSearch);
    //                        }


    //                        XCALIBURFILESLib.XSpectrumRead Xspec = _Spectra.Item(spectrumToQuantitate) as XCALIBURFILESLib.XSpectrumRead;

    //                        #region not-useful
    //                        //try
    //                        //{

    //                        //    XCALIBURFILESLib.XParentScans XparentScans = Xspec.ParentScans as XCALIBURFILESLib.XParentScans;
    //                        //    short prScansCount = XparentScans.Count;
    //                        //    short numOfSpectra = _Spectra.Count;
    //                        //    string[] fll = new string[_filter.Count];
    //                        //    for (int k=0; k<_filter.Count;k++)
    //                        //    {
    //                        //        XCALIBURFILESLib.XFilter fil= (XCALIBURFILESLib.XFilter)_filter.Item(1);
    //                        //        fll[k] = fil.Text;
    //                        //        //fil.Validate
    //                        //    }

    //                        //}
    //                        //catch { }


    //                        //Get the instrument name
    //                        //XCALIBURFILESLib.XInstrument instrument = _Detector.Instrument as XCALIBURFILESLib.XInstrument;
    //                        //instrumentName = instrument.Name;
    //                        //instrument = null;
    //                        // spectrum data
    //                        #endregion

    //                        scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec);

    //                    }
    //                    catch
    //                    {
    //                        _Raw.Close();

    //                        _Detector = null;
    //                        _Spectra = null;
    //                        GC.Collect();
    //                        GC.WaitForPendingFinalizers();

    //                        //MessageBox.Show("Could not open selected raw file: " + e.Message);
    //                        //Application.DoEvents();
    //                        return null;

    //                    }



    //                }
    //            }
    //            #endregion

    //            _Raw.Close();

    //            GC.Collect();
    //            return scansRaw;

    //        }

    //        private Comb.mzI[][] newScansRaw(BinStackOptions _options, Comb.mzI[][] _scansRaw, int _spectrumToQuantitate, double _parentMass, XCALIBURFILESLib.XSpectrumRead _Xspec)
    //        {
    //            double[,] data = _Xspec.Data as double[,];

    //            double minMass = data[0, 0];
    //            double maxMass = data[0, data.GetUpperBound(1)];
    //            int minMassPos = 0;
    //            int maxMassPos = data.GetUpperBound(1);

    //            Comb.mzI[][] scansRaw = (Comb.mzI[][])_scansRaw.Clone();

    //            if (_options.useParentalMass)
    //            {
    //                //Mass conversions (mz data in the raw file is expressed in amu)
    //                switch (_options.parentalMassUnits)
    //                {
    //                    case massUnits.amu:
    //                        minMass = _parentMass - _options.parentalMass / 2;
    //                        maxMass = _parentMass + _options.parentalMass / 2;
    //                        break;
    //                    case massUnits.mmu:
    //                        // amu = mmu * 1e3
    //                        minMass = _parentMass - _options.parentalMass * 1000 / 2;
    //                        maxMass = _parentMass + _options.parentalMass * 1000 / 2;
    //                        break;
    //                    case massUnits.ppm:
    //                        // amu = ppm * parentalMass * 1e-6
    //                        minMass = _parentMass - _options.parentalMass * _parentMass * 1e-6;
    //                        maxMass = _parentMass - _options.parentalMass * _parentMass * 1e-6;
    //                        break;
    //                }

    //                if (minMass < data[0, 0]) minMass = data[0, 0];
    //                if (maxMass > data[0, data.GetUpperBound(1)]) maxMass = data[0, data.GetUpperBound(1)];

    //            }

    //            //determine index of minimum mass
    //            for (int pos = 0; pos <= data.GetUpperBound(1); pos++)
    //            {
    //                if (data[0, pos] < minMass) minMassPos = pos + 1;
    //                else break;
    //            }

    //            //determine index of maximum mass
    //            for (int pos = data.GetUpperBound(1); pos >= 0; pos--)
    //            {
    //                if (data[0, pos] > maxMass) maxMassPos = pos - 1;
    //                else break;
    //            }

    //            int diffPos = maxMassPos - minMassPos;

    //            extData = new Comb.mzI[diffPos + 1];

    //            int counter = 0;
    //            for (int k = minMassPos; k <= maxMassPos; k++)
    //            {
    //                extData[counter].mz = data[0, k];
    //                extData[counter].I = data[1, k];
    //                counter++;
    //            }

    //            try
    //            {
    //                if (scansRaw[_spectrumToQuantitate] == null)
    //                    scansRaw[_spectrumToQuantitate] = new Comb.mzI[extData.GetUpperBound(0)];

    //                switch (_options.mode)
    //                {
    //                    case adquisitionMode.position:
    //                        scansRaw[_spectrumToQuantitate] = (Comb.mzI[])extData.Clone();
    //                        break;

    //                    case adquisitionMode.RetentionTime:
    //                        for (int i = extData.GetLowerBound(0); i < extData.GetUpperBound(0); i++)
    //                        {
    //                            scansRaw[_spectrumToQuantitate][i].I += extData[i].I;
    //                        }
    //                        break;
    //                }
    //            }
    //            catch
    //            {
    //                //blank spectrum
    //                extData = new Comb.mzI[1];
    //                scansRaw[_spectrumToQuantitate] = (Comb.mzI[])extData.Clone();
    //                //return null;

    //            }

    //            return scansRaw;
    //        }

    //    }


    }
}
