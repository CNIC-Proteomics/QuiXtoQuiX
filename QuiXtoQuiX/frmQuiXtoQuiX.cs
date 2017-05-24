using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QuiXoT.math;
using QuiXoT.lookUp;
using QuiXoT.DA_Raw;
using Mathlet;
using System.Xml;
using MSFileReaderLib;
using ProteomicUtilities;

namespace QuiXtoQuiX
{
    public partial class frmMain : Form
    {
        private DataSet dataSetRecords;
        private DataSet newDataSet;
        private methodUsed method = methodUsed.quadraticSmooth;
        private massUsed mass = massUsed.experimental;
        private bool firstAndLastIncluded = false; // user defined
        private double tolerance = 5; // user defined
        private int maxScansAcceptedWithoutPeak = 2; // user defined
        private double deltaRT = 0.5; // user defined
        private double RTstep = 0.05; // user defined
        private double chromWidthInRT = 2; // user defined
        private double noiseRatio = 0; // user defined
        public double protonMass = 1.007276466812;  // source: http://physics.nist.gov/cgi-bin/cuu/Value?mpu
        private int cacheSize;
        msnPerFull.rawStats rawReader = new msnPerFull.rawStats();

        // noiseRatio = 1 means "take exactly the result of avgNoise"
        // noiseRatio = 2 means "take twice as much noise than avgNoise", and so on
        //ArrayList peakTable; // only to be used when we calculate the chromatographic peak
        ArrayList peakTableChrom;

        /* IMPORTANT:
         * for historical reasons peakTable became important
         * and its indexes had to be fixed to:
         * 
        peakTable[0] is MSMS_scan (int)
        peakTable[1] is precursorMZ (double)
        peakTable[2] is charge (int)
        peakTable[3] is maxPeakScan (int)
        peakTable[4] is peakIntensity (double)
        peakTable[5] is peakStart (int)
        peakTable[6] is peakEnd (int)
        peakTable[7] is peakMaxRT (double)
        peakTable[8] is peptideSequence (string)
        peakTable[9] is rawFile (double)
        peakTable[10] is lastScan (int)
        peakTable[11] is "rescued" when the scan is a rescued scan, not identified, otherwise it is empty
        peakTable[12] when it is a rescued scan it is the difference in RT between two peaks; zero otherwise
         * 
         * */

        public frmMain()
        {
            InitializeComponent();
            this.Text = "QuiXtoQuiX v." + QuiXtoQuiX.Properties.Settings.Default.version;
            checkedChanged(null, null);
        }

        public bool openFile(string _XMLFileIn,
                            string _XSDFileIn,
                            string _rawFilePath,
                            int _numScansToCheck,
                            int _numScansToTake,
                            bool saveFirstScanGraph,
                            massUsed mass)
        {
            return openFile(_XMLFileIn,
                            _XSDFileIn,
                            _rawFilePath,
                            _numScansToCheck,
                            _numScansToTake,
                            saveFirstScanGraph,
                            mass,
                            null);
        }

        public bool openFile(string _XMLFileIn,
                            string _XSDFileIn,
                            string _rawFilePath,
                            int _numScansToCheck,
                            int _numScansToTake,
                            bool saveFirstScanGraph,
                            massUsed mass,
                            ArrayList _modifList)
        {
            // when _numScansToTake == 0, means the file already contains the first and last scan

            string XMLFileOut = "";

            btnGo.Enabled = false;
            //txbNumScans.Enabled = false;

            switch (method)
            {
                case methodUsed.aroundMSMSscan:
                    XMLFileOut = string.Concat(_XMLFileIn.Substring(0, _XMLFileIn.Length - 4), "_ext-around.xml");
                    break;

                case methodUsed.peakSweep:
                    XMLFileOut = string.Concat(_XMLFileIn.Substring(0, _XMLFileIn.Length - 4), "_ext-sweep.xml");
                    break;

                case methodUsed.XMLdefined:
                    XMLFileOut = string.Concat(_XMLFileIn.Substring(0, _XMLFileIn.Length - 4), "_ext-XMLdef.xml");
                    break;

                case methodUsed.quadraticSmooth:
                    XMLFileOut = string.Concat(_XMLFileIn.Substring(0, _XMLFileIn.Length - 4), "_ext-smooth.xml");
                    break;
            }

            //warning: the default id schema changes when using retention times (new field: "RT")

            int[,] extraScans = new int[0, 0];
            int[] scanStart = new int[0];
            int[] scanEnd = new int[0];
            int[] firstScanFound = new int[0];
            int[] lastScanFound = new int[0];
            double[] peakIntensity = new double[0];
            int[] MSMSscan = new int[0];
            int[] idScan = new int[0];
            double[] maxRTs = new double[0];
            double[] RTScanFromFile = new double[0];
            double[] retTimes = new double[0];
            double[] precMZ = new double[0];
            string[] peptides = new string[0];
            int[] charges = new int[0];

            lblStatus.Text = "Reading XML...";
            Application.DoEvents();

            dataSetRecords = new DataSet("DataSetRecords");
            dataSetRecords.ReadXmlSchema(_XSDFileIn);
            dataSetRecords.ReadXml(_XMLFileIn);

            DataView data = dataSetRecords.Tables["peptide_match"].DefaultView;
            data.Sort = "RAWFileName DESC, FirstScan ASC";

            string[] scanRaws = rawFileForScan(data);

            //extDataSet.Tables[0].Rows[0] = dataSetRecords.Tables[0].Rows[0];

            // does not check over bound problems, as usually MSMS scans are in the middle

            lblStatus.Text = "Getting full scans... 0%";
            Application.DoEvents();



            switch (method)
            {
                case methodUsed.aroundMSMSscan:
                    {
                        idScan = getFirstScan(data, scanRaws, "FirstScan", "ScanRT", _rawFilePath);
                        charges = null; // *** for the moment charge and sequence will be used only in the findPeak method
                        peptides = null;
                        extraScans = takeScansAround(idScan,
                                                    scanRaws,
                                                    _rawFilePath,
                                                    _numScansToTake);
                        break;
                    }

                case methodUsed.XMLdefined:
                    {
                        // this means the XML file knows already which is the first and the last scan
                        scanStart = getScanStart(data);
                        scanEnd = getScanEnd(data);
                        charges = null; // *** for the moment charge and sequence qill be used only in the findPeak method
                        peptides = null;
                        idScan = new int[data.Count];
                        for (int i = 0; i < data.Count; i++)
                            idScan[i] = i;

                        extraScans = takeScansBetween(scanStart,
                                                        scanEnd,
                                                        scanRaws,
                                                        _rawFilePath,
                                                        maxDiffBetweenScans(scanStart, scanEnd));
                        break;

                    }

                case methodUsed.peakSweep:
                    {
                        idScan = getFirstScan(data, scanRaws, "FirstScan", "ScanRT", _rawFilePath);

                        peptides = getFieldValueString(data, "Sequence");
                        // it is important to get the precursor masses after the sequences,
                        // as the masses will be calculated from the sequences if they are not in the xml database
                        // *** to-do: take precursor mass from sequence when it is not in file
                        precMZ = precursorMZ(data, mass);
                        if (precMZ == null) return true;
                        charges = getFieldValueInt(data, "Charge");

                        if (firstAndLastIncluded)
                        {
                            scanEnd = getScanEnd(data);
                            firstScanFound = null;
                            lastScanFound = null;
                        }
                        else
                        {
                            scanEnd = null;
                            firstScanFound = new int[idScan.GetUpperBound(0) + 1];
                            lastScanFound = new int[idScan.GetUpperBound(0) + 1];
                        }

                        extraScans = takeScansSweepAlgorithm(idScan,
                                                            retTimes,
                                                            precMZ,
                                                            scanRaws,
                                                            peptides,
                                                            charges,
                                                            _rawFilePath,
                                                            _numScansToCheck,
                                                            _numScansToTake,
                                                            ref firstScanFound,
                                                            ref lastScanFound,
                                                            scanEnd);
                        if (extraScans == null) return false;

                        writePeakData(_XMLFileIn);
                        break;
                    }

                case methodUsed.quadraticSmooth:
                    {
                        //peakTable = new ArrayList();
                        peakTableChrom = new ArrayList();

                        RTScanFromFile = getFieldValueDouble(data, "PeakMaxRT");

                        peptides = getFieldValueString(data, "Sequence");
                        // it is important to get the precursor masses after the sequences,
                        // as the masses will be calculated from the sequences if they are not in the xml database
                        // *** to-do: take precursor mass from sequence when it is not in file
                        precMZ = precursorMZ(data, mass);
                        if (precMZ == null) return true;
                        charges = getFieldValueInt(data, "Charge");
                        retTimes = retentionTimesFromRaw(data, _rawFilePath);
                        idScan = getFirstScan(data, scanRaws, "FirstScan", "ScanRT", _rawFilePath);
                        firstScanFound = getFieldValueInt(data, "PeakStart");
                        lastScanFound = getFieldValueInt(data, "PeakEnd");
                        peakIntensity = getFieldValueDouble(data, "PeakIntensity");
                        MSMSscan = getFieldValueInt(data, "MSMS_Scan");
                        //
                        // algorithm goes here
                        //

                        extraScans = takeScansSmoothAlgorithm(_rawFilePath,
                                                                firstScanFound,
                                                                lastScanFound,
                                                                peakIntensity,
                                                                MSMSscan,
                                                                idScan,
                                                                retTimes,
                                                                RTScanFromFile,
                                                                precMZ,
                                                                peptides,
                                                                charges,
                                                                scanRaws,
                                                                saveFirstScanGraph,
                                                                _modifList);

                        if (extraScans == null) return false;

                        writePeakData(_XMLFileIn);

                        break;
                    }
            }

            // then writes the XML calculating previously which scans should be taken
            if (!(cbxWritePeakOnly.Enabled && cbxWritePeakOnly.Checked))
            {
                bool errorClean = assignMSMSscanRow(idScan);
                if (!errorClean) return false;

                newDataSet = dataSetRecords.Clone();

                //***
                DataView newData = newDataSet.Tables["peptide_match"].DefaultView;

                lblStatus.Text = "Writing full scans... 0%";
                Application.DoEvents();

                
                makeNewDataSet(idScan, peptides, charges, extraScans, scanRaws);

                int rescuedScans = peakTableChrom.Count - idScan.Length;
                int[] resIdScan = new int[rescuedScans];
                string[] resPeptides = new string[rescuedScans];
                int[] resCharges = new int[rescuedScans];
                int[,] resExtraScans = new int[rescuedScans, 1];
                string[] resScanRaws = new string[rescuedScans];

                if (rescuedScans > 0)
                {
                    // this means extra scans have been rescued
                    for (int i = 0; i < rescuedScans; i++)
                    {
                        ChromPeak chrommy = (ChromPeak)peakTableChrom[i + idScan.Length];

                        if (chrommy.isRescued)
                        {
                            if (Math.Abs(chrommy.RTdifference) < deltaRT)
                            {
                                resIdScan[i] = chrommy.MSMS_scan;
                                resPeptides[i] = chrommy.sequence;
                                resCharges[i] = chrommy.charge;
                                resExtraScans[i, 0] = chrommy.maxPeakScan;
                                resScanRaws[i] = chrommy.rawFile;
                            }
                        }
                    }
                }

                addRescuedScans(resIdScan, resPeptides, resCharges, resExtraScans, resScanRaws, _modifList);

                lblStatus.Text = "Writing extended XML file...";
                Application.DoEvents();

                newData = newDataSet.Tables["peptide_match"].DefaultView;

                newDataSet.WriteXml(XMLFileOut);
            }

            lblStatus.Text = "Done!";
            Application.DoEvents();

            MessageBox.Show("File successfully created");
            checkedChanged(null, null, false);

            return true;
        }

        private int[] getFirstScan(DataView data,
            string[] scanRaws,
            string fieldWithMSMSScan,
            string fieldWithScanRT,
            string rawFilePath)
        {
            int[] idScan = getFieldValueInt(data, fieldWithMSMSScan);
            for (int i = 0; i < idScan.Length; i++)
            {
                double[] ScanRT = new double[0];

                try
                {
                    ScanRT = getFieldValueDouble(data, fieldWithScanRT);
                }
                catch
                {
                }

                if (idScan[i] == 0 && ScanRT.Length > 0) // then check for ScanRT
                {
                    string rawFile = Path.Combine(rawFilePath, scanRaws[i]);

                    if (rawFile != rawReader.workingRAWFilePath)
                    {
                        rawReader.openRawFast(rawFile, cacheSize);
                        int lastScanOfAll = rawReader.lastSpectrumNumber();
                        cacheSize = rawReader.initialiseSpectrumTypes(lastScanOfAll);
                        if (cacheSize != int.Parse(txbCacheSize.Text))
                        {
                            txbCacheSize.Text = cacheSize.ToString();
                            Application.DoEvents();
                        }
                    }

                    idScan[i] = rawReader.getScanNumberOfPrevOrNextSpectrumByType(ScanRT[i],
                                            msnPerFull.rawStats.spectrumPosition.nearestInRT,
                                            spectrumTypes.MSMS);
                }
            }
            return idScan;
        }

        private int[,] takeScansSmoothAlgorithm(string _rawFilePath,
                                                int[] _firstScanFound,
                                                int[] _lastScanFound,
                                                double[] _peakIntensity,
                                                int[] _MSMSscanFromFile,
                                                int[] _idScan,
                                                double[] _retTimes,
                                                double[] _RTfromFile,
                                                double[] _precMZ,
                                                string[] _peptides,
                                                int[] _charges,
                                                string[] _scanRaws,
                                                bool _saveGraphAtFirstScan,
                                                ArrayList _modifs)
        {
            int[,] extraScans;

            string[] scanRawsWithPath = getScanRawsWithPath(_rawFilePath, _scanRaws);
            double percent = 0;
            string percentMessage = "Measuring smoothed peaks (1/2)... ";
            if (_modifs != null) percentMessage = "Measuring smoothed peaks (1/3)... ";

            DateTime startTime = DateTime.Now;

            bool atFirstScan = true;
            for (int MSMSscan = 0; MSMSscan < _idScan.Length; MSMSscan++)
            {
                object[] resultsForScan = new object[5];
                int MSMSscanToSave = 0;

                bool fasten = false; // useful for debugging
                if (!fasten)
                {
                    if (_idScan[MSMSscan] > 0)
                    {
                        if (_MSMSscanFromFile[MSMSscan] == 0)
                        {
                            // case when the maximum has to be calculated
                            resultsForScan = getPeakInformation(scanRawsWithPath[MSMSscan],
                                                        _precMZ[MSMSscan],
                                                        tolerance,
                                                        _retTimes[MSMSscan],
                                                        deltaRT,
                                                        RTstep,
                                                        atFirstScan && _saveGraphAtFirstScan);

                            MSMSscanToSave = _idScan[MSMSscan];
                        }
                        else
                        {
                            // case when the maximum has already been calculated
                            int maxPeakFromFile = _idScan[MSMSscan];

                            object[] newObject = { _idScan[MSMSscan],
                                                 _peakIntensity[MSMSscan],
                                                 _firstScanFound[MSMSscan],
                                                 _lastScanFound[MSMSscan],
                                                 _RTfromFile[MSMSscan] };

                            resultsForScan = newObject;
                            MSMSscanToSave = _MSMSscanFromFile[MSMSscan];
                        }
                    }
                    else
                    {
                        // case when a chromatographic peak position has been calculated from another raw

                        resultsForScan = getPeakInformation(scanRawsWithPath[MSMSscan],
                                                        _precMZ[MSMSscan],
                                                        tolerance,
                                                        _retTimes[MSMSscan],
                                                        deltaRT,
                                                        RTstep,
                                                        atFirstScan && _saveGraphAtFirstScan);

                        MSMSscanToSave = 0;
                    }

                    if (resultsForScan == null)
                        return null;

                    int maxPeakScan = (int)(resultsForScan[0]);
                    double maxPeakValue = (double)(resultsForScan[1]);
                    _firstScanFound[MSMSscan] = (int)(resultsForScan[2]);
                    _lastScanFound[MSMSscan] = (int)(resultsForScan[3]);
                    double maxPeakRT = (double)(resultsForScan[4]);

                    //ArrayList peakRow = new ArrayList();
                    ChromPeak chrom = new ChromPeak();

                    // peakRow[0]

                    //peakRow.Add(MSMSscanToSave); // identification scan, or first scan if first and last are included

                    //peakRow.Add(_precMZ[MSMSscan]); // peakRow[1]
                    //peakRow.Add(_charges[MSMSscan]); // peakRow[2]
                    //peakRow.Add(maxPeakScan); // peakRow[3]
                    //peakRow.Add(maxPeakValue); // peakRow[4]

                    //peakRow.Add(_firstScanFound[MSMSscan]); // peakRow[5]
                    //peakRow.Add(_lastScanFound[MSMSscan]); // peakRow[6]
                    //peakRow.Add(maxPeakRT); // peakRow[7]

                    //peakRow.Add(_peptides[MSMSscan]); // peakRow[8]
                    //peakRow.Add(_scanRaws[MSMSscan]); // peakRow[9]

                    //peakRow.Add(0); // peakRow[10]
                    //peakRow.Add(""); // peakRow[11]
                    //peakRow.Add(0); // peakRow[12]

                    chrom.MSMS_scan = MSMSscanToSave;
                    chrom.precursorMZ = _precMZ[MSMSscan];
                    chrom.charge = _charges[MSMSscan];
                    chrom.maxPeakScan = maxPeakScan;
                    chrom.peakIntensity = maxPeakValue;
                    chrom.peakStart = _firstScanFound[MSMSscan];
                    chrom.peakEnd = _lastScanFound[MSMSscan];
                    chrom.peakMaxRT = maxPeakRT;
                    chrom.sequence = _peptides[MSMSscan];
                    chrom.rawFile = _scanRaws[MSMSscan];
                    chrom.lastScan = 0;
                    chrom.isRescued = false;
                    chrom.RTdifference = 0;

                    //peakTable.Add(peakRow);
                    peakTableChrom.Add(chrom);
                }

                percent = writePercentage(percent, startTime, _idScan.Length, MSMSscan, percentMessage);

                // useful for debugging
                //if (percent > 78.21)
                //{
                //    int debuggy = 0;
                //}

                atFirstScan = false;
            }

            extraScans = new int[peakTableChrom.Count, 1];

            // peakTable[3] is maxPeakScan
            for (int i = 0; i < peakTableChrom.Count; i++)
                extraScans[i, 0] = ((ChromPeak)peakTableChrom[i]).maxPeakScan;
                //extraScans[i, 0] = int.Parse(((ArrayList)peakTable[i])[3].ToString());

            //for (int i = 0; i < peakTable.Count; i++)
            //{
            //    string originalSequence = ((ArrayList)peakTable[i])[8].ToString();
            //}

            percent = writePercentage(percent, startTime, 100, 100, percentMessage);
            percent = 0;

            if (_modifs != null)
            {
                percentMessage = "Measuring smoothed peaks (2/3)... ";
                startTime = DateTime.Now;

                ModificationUtils mod = new ModificationUtils();
                int endOfOriginalPart = peakTableChrom.Count;
                for (int i = 0; i < endOfOriginalPart; i++)
                {
                    //double RTofOriginalMax = (double)((ArrayList)peakTable[i])[7];
                    double RTofOriginalMax = ((ChromPeak)peakTableChrom[i]).peakMaxRT;
                    double counterWeight = double.Parse(_precMZ[i].ToString());
                    // previous line simply copies the weight
                    // next line adjusts the weight to the correct one, giving
                    // back the old-sequest sequence as well.
                    string counterSeq = mod.getCounterSequence(_peptides[i],
                        ref counterWeight, _charges[i], _modifs);
                    int counterCharge = _charges[i];

                    bool alreadyPresent = checkIfAlreadyPresent(_peptides, _charges, counterSeq, counterCharge);
                    if (!alreadyPresent)
                    {
                        object[] extraResults = getPeakInformation(scanRawsWithPath[i],
                                                        counterWeight,
                                                        tolerance,
                                                        _retTimes[i],
                                                        deltaRT,
                                                        RTstep,
                                                        false);

                        if (extraResults == null)
                        {
                            object[] problematicResult = { 0, 0.0, 0, 0, 0.0 };
                            extraResults = problematicResult;
                        }
                        
                        int maxPeakScan = (int)(extraResults[0]);
                        double maxPeakValue = (double)(extraResults[1]);
                        int extraFirstScanFound = (int)(extraResults[2]);
                        int extraLastScanFound = (int)(extraResults[3]);
                        double maxPeakRT = (double)(extraResults[4]);

                        //ArrayList peakRow = new ArrayList();
                        ChromPeak chrom = new ChromPeak();

                        double peakRTdistance = maxPeakRT - RTofOriginalMax;

                        //peakRow.Add(_idScan[i]); // identification scan, or first scan if first and last are included

                        //peakRow.Add(counterWeight); // peakRow[1]
                        //peakRow.Add(_charges[i]); // peakRow[2]
                        //peakRow.Add(maxPeakScan); // peakRow[3]
                        //peakRow.Add(maxPeakValue); // peakRow[4]

                        //peakRow.Add(extraFirstScanFound); // peakRow[5]
                        //peakRow.Add(extraLastScanFound); // peakRow[6]
                        //peakRow.Add(maxPeakRT); // peakRow[7]

                        //peakRow.Add(counterSeq); // peakRow[8]
                        //peakRow.Add(_scanRaws[i]); // peakRow[9]

                        //peakRow.Add(0); // peakRow[10]
                        //peakRow.Add("rescued"); // peakRow[11]
                        //peakRow.Add(peakRTdistance); // peakRow[12]

                        chrom.MSMS_scan = _idScan[i];
                        chrom.precursorMZ = counterWeight;
                        chrom.charge = _charges[i];
                        chrom.maxPeakScan = maxPeakScan;
                        chrom.peakIntensity = maxPeakValue;
                        chrom.peakStart = extraFirstScanFound;
                        chrom.peakEnd = extraLastScanFound;
                        chrom.peakMaxRT = maxPeakRT;
                        chrom.sequence = counterSeq;
                        chrom.rawFile = _scanRaws[i];
                        chrom.lastScan = 0;
                        chrom.isRescued = true;
                        chrom.RTdifference = peakRTdistance;

                        peakTableChrom.Add(chrom);

                        //peakTable.Add(peakRow);
                    }

                    percent = writePercentage(percent, startTime, _idScan.Length, i, percentMessage);
                }
            }

            // *** will add to the peakData, but not yet to the xml
            return extraScans;
        }

        private bool checkIfAlreadyPresent(string[] _peptides, int[] _charges,
            string _counterSeq, int _counterCharge)
        {
            bool alreadyPresent = false;
            for (int j = 0; j < _peptides.Length; j++)
            {
                if (_peptides[j] == _counterSeq && _charges[j] == _counterCharge)
                {
                    alreadyPresent = true;
                    break;
                }
            }
            return alreadyPresent;
        }

        private static string[] getScanRawsWithPath(string _rawFilePath, string[] scanRaws)
        {
            string[] scanRawsWithPath = (string[])scanRaws.Clone();
            for (int i = 0; i < scanRawsWithPath.Length; i++)
                scanRawsWithPath[i] = Path.Combine(_rawFilePath, scanRawsWithPath[i]);
            return scanRawsWithPath;
        }

        private void writePeakData(string _XMLFileIn)
        {
            string peakFileOut = string.Concat(_XMLFileIn.Substring(0, _XMLFileIn.Length - 4), "_peakData.xls");
            StreamWriter peakFile = new StreamWriter(peakFileOut);
            string header = "";

            if (firstAndLastIncluded)
                header = "First scan\tLast scan\tm/z of precursor ion\tcharge\tMost intense full\tRT of most intense full\tPeak intensity\tPeptide sequence\tRaw file\trescued?\tRTdiff";
            else
                header = "Full ms2\tm/z of precursor ion\tcharge\tMost intense full\tRT of most intense full\tPeak intensity\tFirst found\tLast found\tPeptide sequence\tRaw file\trescued?\tRTdiff";

            peakFile.WriteLine(header);

            foreach (ChromPeak o in peakTableChrom)
            {
                string line = "";

                //foreach (object o2 in o)
                //{
                //    line += o2.ToString() + "\t";
                //}

                line += o.MSMS_scan.ToString(); // MSMSscan or FirstScan
                line += "\t";

                if (firstAndLastIncluded)
                {
                    line += o.lastScan.ToString();
                    line += "\t";
                }

                line += o.precursorMZ.ToString();
                line += "\t";
                line += o.charge.ToString();
                line += "\t";
                line += o.maxPeakScan.ToString();
                line += "\t";
                line += o.peakMaxRT.ToString();
                line += "\t";
                line += o.peakIntensity.ToString();
                line += "\t";

                if (!firstAndLastIncluded)
                {
                    line += o.peakStart.ToString();
                    line += "\t";
                    line += o.peakEnd.ToString();
                    line += "\t";
                }

                line += o.sequence;
                line += "\t";
                line += o.rawFile;
                line += "\t";
                // if this is "rescued", it means it is an unidentified peptide of the pair heavy/light
                if (o.isRescued)
                {
                    line += "rescued";
                    line += "\t";
                    line += o.RTdifference.ToString();
                }

                peakFile.WriteLine(line);
            }

            peakFile.Close();
        }

        private bool assignMSMSscanRow(int[] idScan)
        {
            DataView origData = dataSetRecords.Tables["peptide_match"].DefaultView;

            for (int i = 0; i < origData.Table.Rows.Count; i++)
            {
                try
                {
                    origData[i]["MSMS_Scan"] = idScan[i];
                }
                catch(Exception ex)
                {
                    lblStatus.Text = "Problem while assigning MSMS scan numbers";
                    Application.DoEvents();

                    MessageBox.Show("Error, please check whether your schema contains an \"MSMS_Scan\" column."
                        + "\n\nError was: " + ex.ToString());
                    btnGo.Enabled = true;
                    return false;
                }
            }

            return true; // true = no errors
        }

        private void addRescuedScans(int[] _idScan,
                                        string[] _sequence,
                                        int[] _charge,
                                        int[,] _extraScans,
                                        string[] _scanRaws,
                                        ArrayList _modifList)
        {
            makeNewDataSet(_idScan,
                            _sequence,
                            _charge,
                            _extraScans,
                            _scanRaws,
                            true,
                            _modifList);
        }

        private void makeNewDataSet(int[] _idScan,
                                    string[] _sequence,
                                    int[] _charge,
                                    int[,] _extraScans,
                                    string[] _scanRaws)
        {
            makeNewDataSet(_idScan,
                            _sequence,
                            _charge,
                            _extraScans,
                            _scanRaws,
                            false,
                            null);
        }

        private void makeNewDataSet(int[] _idScan,
                                    string[] _sequence,
                                    int[] _charge,
                                    int[,] _extraScans,
                                    string[] _scanRaws,
                                    bool areRescuedScans,
                                    ArrayList _modifList)
        {
            int totalIdScans = _extraScans.GetUpperBound(0);
            double percent = 0;
            DateTime writeStart = DateTime.Now;
            string writingMessage = "Writing full scans... ";

            for (int i = 0; i <= totalIdScans; i++)
            {
                percent = writePercentage(percent, writeStart, totalIdScans, i, writingMessage);

                for (int j = 0; j <= _extraScans.GetUpperBound(1); j++)
                {
                    // newDataSet will not be treated as a pointer, as inside the method a .Copy() is used
                    if (_extraScans[i, j] > 0)
                    {
                        // provSeq and provCharge are defined only to avoid technical problems
                        // when _sequence[]=null and _charge[]=null

                        string provSeq;
                        int provCharge;

                        if (_sequence != null)
                            provSeq = _sequence[i];
                        else
                            provSeq = "";

                        if (_charge != null)
                            provCharge = _charge[i];
                        else
                            provCharge = 0;

                        addRowWithFirstScan(dataSetRecords,
                                            _extraScans[i, j],
                                            _idScan[i],
                                            provSeq,
                                            provCharge,
                                            _scanRaws[i],
                                            areRescuedScans,
                                            _modifList);
                    }
                    else
                        break;
                }
            }
            // at this point the newDataSet contains all the data we need
        }

        private void addRowWithFirstScan(DataSet _originOfData,
                                            int _firstScan,
                                            int _MSMSscan,
                                            string _sequence,
                                            int _charge,
                                            string _scanRaw)
        {
            addRowWithFirstScan(_originOfData,
                                _firstScan,
                                _MSMSscan,
                                _sequence,
                                _charge,
                                _scanRaw,
                                false,
                                null);
        }

        private void addRowWithFirstScan(DataSet _originOfData,
                                            int _firstScan,
                                            int _MSMSscan,
                                            string _sequence,
                                            int _charge,
                                            string _scanRaw,
                                            bool isRescuedScan,
                                            ArrayList _modifList)
        {
            string tableToCopy = "peptide_match";
            string redundancesTable = "Redundances";
            string redTable = "Red";
            
            //DataSet _destinationDataSet = _destinationOfData.Copy();

            DataView originalData = _originOfData.Tables[tableToCopy].DefaultView;
            DataView originalRedundancesData = _originOfData.Tables[redundancesTable].DefaultView;

            DataView resultingData = newDataSet.Tables[tableToCopy].DefaultView;
            DataView redundancesData = newDataSet.Tables[redundancesTable].DefaultView;
            DataView redData = newDataSet.Tables[redTable].DefaultView;

            bool destinationIsEmpty = false;

            int onRows = originalData.Table.Rows.Count;
            int rnRows = resultingData.Table.Rows.Count;
            int rednRows = redData.Table.Rows.Count;

            if (rnRows == 0)
            {
                destinationIsEmpty = true;
                newDataSet = _originOfData.Copy();
                resultingData = newDataSet.Tables[tableToCopy].DefaultView;

                while (resultingData.Count > 1)
                    resultingData[0].Delete();
            }

            for (int a = 0; a < onRows; a++)
            {
                bool isSearchedScan = int.Parse(originalData[a]["MSMS_Scan"].ToString()) == _MSMSscan
                    && originalData[a]["RAWFileName"].ToString() == _scanRaw;

                // just to check
                string retrievedSequence = originalData[a]["Sequence"].ToString();
                if (isRescuedScan && _modifList != null)
                {
                    ModificationUtils utils = new ModificationUtils();
                    retrievedSequence = utils.getCounterSequence(retrievedSequence, _modifList);
                }

                if ((_charge != 0) && (_sequence != ""))
                    isSearchedScan = isSearchedScan
                        && _charge == int.Parse(originalData[a]["Charge"].ToString())
                        && _sequence == retrievedSequence;

                if (isSearchedScan)
                {
                    object[] rowToCopy;

                    DataRow originalRow = (DataRow)originalData[a].Row;

                    int primKeydir = resultingData.Table.Columns.IndexOf(resultingData.Table.TableName + "_Id");
                    int firstScanDir = resultingData.Table.Columns.IndexOf("FirstScan");
                    int MSMSscanDir = resultingData.Table.Columns.IndexOf("MSMS_scan");
                    int PeakStartDir = resultingData.Table.Columns.IndexOf("PeakStart");
                    int PeakEndDir = resultingData.Table.Columns.IndexOf("PeakEnd");
                    int PeakIntensityDir = resultingData.Table.Columns.IndexOf("PeakIntensity");
                    int PeakMaxRTDir = resultingData.Table.Columns.IndexOf("PeakMaxRT");
                    int IndexDir = resultingData.Table.Columns.IndexOf("Index");
                    int spectrumIndexDir = resultingData.Table.Columns.IndexOf("spectrumIndex");
                    int PrecursorMassDir = resultingData.Table.Columns.IndexOf("PrecursorMass");

                    rowToCopy = originalRow.ItemArray;

                    if (IndexDir != -1) rowToCopy[IndexDir] = null;
                    if (spectrumIndexDir != -1) rowToCopy[spectrumIndexDir] = null;

                    rowToCopy[firstScanDir] = _firstScan; // here goes the fullScan
                    rowToCopy[MSMSscanDir] = _MSMSscan; // this is the original MSMSscan, which will be the same as the LastScan

                    rowToCopy[primKeydir] = int.Parse(resultingData[resultingData.Count - 1].Row[primKeydir].ToString()) + 1;
                    //resultingData[resultingData.Count - 1].Row[resultingData.Table.TableName + "_Id"]
                    //    = int.Parse(resultingData[resultingData.Count - 2].Row[resultingData.Table.TableName + "_Id"].ToString()) + 1;

                    for (int provPeak = 0; provPeak < peakTableChrom.Count; provPeak++)
                    {
                        ChromPeak chrom = (ChromPeak)peakTableChrom[provPeak];
                        int provMSMS = chrom.MSMS_scan;
                        string provPeptide = chrom.sequence;
                        int provCharge = chrom.charge;
                        bool provRescued = chrom.isRescued;

                        if (provMSMS == _MSMSscan &&
                            provPeptide == _sequence &&
                            provCharge == _charge &&
                            provRescued == isRescuedScan)
                        {
                            rowToCopy[PeakStartDir] = chrom.peakStart;
                            rowToCopy[PeakEndDir] = chrom.peakEnd;
                            rowToCopy[PeakIntensityDir] = chrom.peakIntensity;
                            rowToCopy[PeakMaxRTDir] = chrom.peakMaxRT;
                            if (PrecursorMassDir > 0)
                                rowToCopy[PrecursorMassDir] = (chrom.precursorMZ *
                                    chrom.charge - (chrom.charge - 1) * protonMass);

                            break;
                        }
                    }
                    //rowToCopy[PeakStartDir] = peakTable

                    if (isRescuedScan)
                    {
                        int FDRDir = resultingData.Table.Columns.IndexOf("FDR");
                        int XC1DDir = resultingData.Table.Columns.IndexOf("XC1D");
                        int XC2DDir = resultingData.Table.Columns.IndexOf("XC2D");
                        int SpDir = resultingData.Table.Columns.IndexOf("Sp");
                        int SpRankDir = resultingData.Table.Columns.IndexOf("SpRank");
                        int SequenceDir = resultingData.Table.Columns.IndexOf("Sequence");

                        if (SequenceDir > 0) rowToCopy[SequenceDir] = _sequence;
                        if (FDRDir > 0) rowToCopy[FDRDir] = 100;
                        if (XC1DDir > 0) rowToCopy[XC1DDir] = 0;
                        if (XC2DDir > 0) rowToCopy[XC2DDir] = 0;
                        if (SpDir > 0) rowToCopy[SpDir] = 0;
                        if (SpRankDir > 0) rowToCopy[SpRankDir] = 1000;
                    }

                    resultingData.Table.Rows.Add(rowToCopy);

                    copyRedundances(originalData, originalRedundancesData, resultingData,
                        redundancesData, redData, a, primKeydir);
                }
            }

            if (destinationIsEmpty)
                resultingData[0].Delete();

            //return newDataSet;
        }

        private static void copyRedundances(DataView originalData, DataView originalRedundancesData, DataView resultingData, DataView redundancesData, DataView redData, int a, int primKeydir)
        {
            int redundancesPrimKeyDir = redundancesData.Table.Columns.IndexOf(redundancesData.Table.TableName + "_Id");
            int redundancesPrimKeyPeptideMatchDir = redundancesData.Table.Columns.IndexOf(resultingData.Table.TableName + "_Id");

            redundancesData.Table.Rows.Add().SetParentRow(resultingData.Table.Rows[resultingData.Count - 1]);
            redundancesData[redundancesData.Count - 1][redundancesPrimKeyDir] =
                resultingData.Table.Rows[resultingData.Count - 1][primKeydir];

            DataRow[] originalRed = new DataRow[0];
            DataRow[] originalRedundances =
                originalData[a].Row.GetChildRows("peptide_match_Redundances");
            if (originalRedundances.Length > 0)
                originalRed =
                    originalRedundancesData[(int)originalRedundances[0][0]].Row.GetChildRows("Redundances_Red");

            int totOriginalRed = originalRed.Length;
            if (totOriginalRed > 0)
            {
                for (int r = 0; r < totOriginalRed; r++)
                {
                    object[] resutingRed = new object[3];
                    resutingRed[0] = originalRed[r][0];
                    resutingRed[1] = originalRed[r][1];
                    resutingRed[2] = redundancesData[redundancesData.Count - 1][redundancesPrimKeyDir];

                    redData.Table.Rows.Add(resutingRed);
                }
            }
        }

        private int[] getScanStart(DataView data)
        {
            int[] scanStart = new int[data.Count];

            int a = 0;
            foreach (object ob in data)
            {
                scanStart[a] = int.Parse(data[a].Row["FirstScan"].ToString());
                a++;
            }
            return scanStart;
        }

        private int[] getScanEnd(DataView data)
        {
            int[] scanStart = new int[data.Count];

            int a = 0;
            foreach (object ob in data)
            {
                scanStart[a] = int.Parse(data[a].Row["LastScan"].ToString());
                a++;
            }
            return scanStart;
        }

        private int maxDiffBetweenScans(int[] _scanStart, int[] _scanEnd)
        {
            int difference = 0;

            for (int i = 0; i < _scanStart.Length; i++)
                if (_scanEnd[i] - _scanStart[i] > difference) difference = _scanEnd[i] - _scanStart[i];

            return difference;
        }

        private static double[] getFieldValueDouble(DataView data, string fieldName)
        {
            double[] maxRTs = new double[data.Count];

            if (data.Table.Columns.Contains(fieldName))
                for (int a = 0; a < data.Count; a++)
                {
                    try
                    {
                        string value = data[a].Row[fieldName].ToString();
                        if (value.Length > 0)
                            maxRTs[a] = double.Parse(data[a].Row[fieldName].ToString());
                    }
                    catch { }
                }

            return maxRTs;
        }

        private static int[] getFieldValueInt(DataView data, string fieldName)
        {
            int[] maxRTs = new int[data.Count];

            if (data.Table.Columns.Contains(fieldName))
                for (int a = 0; a < data.Count; a++)
                {
                    try {
                        string value = data[a].Row[fieldName].ToString();
                        if (value.Length > 0)
                            maxRTs[a] = int.Parse(data[a].Row[fieldName].ToString());
                    }
                    catch { }
                }

            return maxRTs;
        }

        private static string[] getFieldValueString(DataView data, string fieldName)
        {
            string[] maxRTs = new string[data.Count];

            if (data.Table.Columns.Contains(fieldName))
                for (int a = 0; a < data.Count; a++)
                {
                    try { maxRTs[a] = data[a].Row[fieldName].ToString(); }
                    catch { }
                }

            return maxRTs;
        }

        private static double[] scanRTs(DataView data)
        {
            double[] RTScan = new double[data.Count];

            int a = 0;
            foreach (object ob in data)
            {
                try { RTScan[a] = double.Parse(data[a].Row["PeakMaxRT"].ToString()); }
                catch { RTScan[a] = 0; }
                a++;
            }

            return RTScan;
        }

        private double[] retentionTimesFromXML(DataView data)
        {
            double[] retTime = new double[data.Count];

            int a = 0;

            foreach (object ob in data)
            {
                retTime[a] = double.Parse(data[a].Row["RT"].ToString());
                a++;
            }

            return retTime;
        }

        private double[] precursorMZ(DataView data, massUsed mass)
        {
            double[] precMZ = new double[data.Count];
            string massTag = "";

            switch (mass)
            {
                case massUsed.experimental:
                    {
                        massTag = "PrecursorMass";
                        break;
                    }
                case massUsed.theoretical:
                    {
                        massTag = "q_peptide_Mass";
                        break;
                    }
            }

            try
            {
                double massTagChecking = double.Parse(data[0].Row[massTag].ToString());
            }
            catch
            {
                MessageBox.Show("You requested an analysis using the "
                    + mass.ToString() + " mass,\n" +
                    "but the QuiXML you provided does not seem to have <"
                    + massTag + "> tags.");
                return null;
            }

            for (int a = 0; a < data.Count; a++)
            {
                double charge = double.Parse(data[a].Row["Charge"].ToString());
                double precMass = double.Parse(data[a].Row[massTag].ToString());

                precMZ[a] = (precMass + (charge - 1) * protonMass) / charge;
            }

            return precMZ;
        }

        private double[] retentionTimesFromRaw(DataView _data, string _rawPath)
        {
            double[] retTimes = new double[_data.Count];
            string[] rawFile = new string[_data.Count];
            int[] scanNumber = new int[_data.Count];
            string currentRaw = rawReader.workingRAWFilePath;
            double retTimeSum = 0;

            // msnPerFull.rawStats rawReader = new msnPerFull.rawStats();

            for (int a = 0; a < _data.Count; a++)
            {
                rawFile[a] = Path.Combine(_rawPath, _data[a].Row["RAWFileName"].ToString());
                scanNumber[a] = int.Parse(_data[a].Row["FirstScan"].ToString());

                // this is done to open each raw just once
                // (data should be already sorted by rawFileName)
                //if (currentRaw != rawFile[a])
                //{
                //if (currentRaw != "") rawReader.closeRaw();

                if (scanNumber[a] > 0)
                {
                    currentRaw = rawFile[a];
                    rawReader.openRawFast(currentRaw, cacheSize);
                    //}

                    retTimes[a] = rawReader.getRTfromScanNumber(scanNumber[a]);
                    retTimeSum += retTimes[a];
                }
                else
                {
                    try
                    {
                        retTimes[a] = double.Parse(_data[a].Row["ScanRT"].ToString());
                        retTimeSum += retTimes[a];
                    }
                    catch
                    {
                        try
                        {
                            retTimes[a] = double.Parse(_data[a].Row["PeakMaxRT"].ToString());
                            retTimeSum += retTimes[a];
                        }
                        catch { retTimes[a] = 0; }
                    }
                }
            }

            // rawReader.closeRaw();
            if (retTimeSum == 0)
            {
                MessageBox.Show("No retention times could be garnered from the raw path,\n" +
                    "are you sure the raw files in the QuiXML are in the folder?");
            }


            return retTimes;
        }

        private static string[] rawFileForScan(DataView data)
        {
            string[] rawFileForScan = new string[data.Count];

            int a = 0;
            foreach (object ob in data)
            {
                rawFileForScan[a] = data[a].Row["RAWFileName"].ToString();
                a++;
            }

            return rawFileForScan;
        }

        //***
        private int[,] takeScansBetween(int[] _scanStart, int[] _scanEnd, string[] _rawFromScan, string _rawFilePath, int _maxNumOfFulls)
        {
            double percent = 0;
            DateTime readStart = DateTime.Now;

            //frmInvisible reader = new frmInvisible();
            // msnPerFull.rawStats reader = new msnPerFull.rawStats();

            int[,] extraScans = new int[_scanStart.Length, _maxNumOfFulls];

            int amountExtraScans = _scanStart.Length;

            for (int a = 0; a < amountExtraScans; a++)
            {
                string percentMessage = "Getting full scans... ";
                percent = writePercentage(percent, readStart, amountExtraScans, a, percentMessage);

                // spectrumType caché must be checked here ****
                string rawFile = _rawFilePath + "\\" + _rawFromScan[a];

                if (rawFile != rawReader.workingRAWFilePath)
                {
                    cacheSize = rawReader.initialiseSpectrumTypes(0);
                    if (cacheSize != int.Parse(txbCacheSize.Text))
                    {
                        txbCacheSize.Text = cacheSize.ToString();
                        Application.DoEvents();
                    }
                    rawReader.openRawFast(rawFile, cacheSize);
                    rawReader.workingRAWFilePath = rawFile;
                    int lastScanOfAll = rawReader.lastSpectrumNumber();
                    cacheSize = rawReader.initialiseSpectrumTypes(lastScanOfAll);
                    if (cacheSize != int.Parse(txbCacheSize.Text))
                    {
                        txbCacheSize.Text = cacheSize.ToString();
                        Application.DoEvents();
                    }
                }

                int totScansInRaw = rawReader.numSpectra();
                bool thermoLibrariesWorking = (totScansInRaw > 0);

                // this part is to prevent the program from working while it has no access to thermo libraries
                if (!thermoLibrariesWorking)
                {
                    int counterWithNoAccess = 0;

                    while (!thermoLibrariesWorking && counterWithNoAccess < 5)
                    {
                        rawReader.openRawFast(rawFile, cacheSize);
                        totScansInRaw = rawReader.numSpectra();
                        thermoLibrariesWorking = (totScansInRaw > 0);
                        counterWithNoAccess++;
                    }

                    if (!thermoLibrariesWorking)
                    {
                        MessageBox.Show("Problem found while reading raw file " + rawFile +
                            "\nThis might happen for the following reasons:" +
                            "\n1) the raw file is empty or corrupt (in this case, remove it from the QuiXML file)." +
                            "\n2) Thermo libraries are not accessible (restart your computer; if this does not help, check your Xcalibur installation).");

                        Application.DoEvents();
                        lblStatus.Text = "raw file or Thermo libraries not accessible.";
                        return null;
                    }
                }

                //frmReader.openRaw(rawFile);
                int[] fsb = rawReader.fullScansBetween(_scanStart[a], _scanEnd[a]);
                //frmReader.closeRaw();

                for (int i = 0; i < _maxNumOfFulls; i++)
                {
                    if (i < fsb.Length)
                        extraScans[a, i] = fsb[i];
                    else
                        extraScans[a, i] = 0;
                }
            }

            return extraScans;
        }

        private int[,] takeScansAround(int[] _idScan, string[] _rawFromScan, string _rawFilePath, int _numScansToTake)
        {
            // no need to check if the number is an even number bigger than 1 (this has been done already)
            //if (Math.Round((double)_numScansToTake / 2, MidpointRounding.AwayFromZero) != (double)(_numScansToTake / 2))
            //    _numScansToTake = _numScansToTake - 1;

            double percent = 0;
            DateTime readStart = DateTime.Now;
            string percentMessage = "Getting full scans... ";

            //frmInvisible reader = new frmInvisible();
            // msnPerFull.rawStats reader = new msnPerFull.rawStats();

            int[,] extraScans = new int[_idScan.Length, _numScansToTake];

            int amountExtraScans = extraScans.GetLength(0);

            for (int a = 0; a < amountExtraScans; a++)
            {
                percent = writePercentage(percent, readStart, amountExtraScans, a, percentMessage);

                // spectrumType caché must be checked here ****
                string rawFile = _rawFilePath + "\\" + _rawFromScan[a];

                if (rawFile != rawReader.workingRAWFilePath)
                {
                    rawReader.openRawFast(rawFile, cacheSize);
                    int lastScanOfAll = rawReader.lastSpectrumNumber();
                    cacheSize = rawReader.initialiseSpectrumTypes(lastScanOfAll);
                    if (cacheSize != int.Parse(txbCacheSize.Text))
                    {
                        txbCacheSize.Text = cacheSize.ToString();
                        Application.DoEvents();
                    }
                }

                int totScansInRaw = rawReader.numSpectra();
                bool thermoLibrariesWorking = (totScansInRaw > 0);

                // this part is to prevent the program from working while it has no access to thermo libraries
                if (!thermoLibrariesWorking)
                {
                    int counterWithNoAccess = 0;

                    while (!thermoLibrariesWorking && counterWithNoAccess < 5)
                    {
                        rawReader.openRawFast(rawFile, cacheSize);
                        totScansInRaw = rawReader.numSpectra();
                        thermoLibrariesWorking = (totScansInRaw > 0);
                        counterWithNoAccess++;
                    }

                    if (!thermoLibrariesWorking)
                    {
                        MessageBox.Show("Problem found while reading raw file " + rawFile +
                            "\nThis might happen for the following reasons:" +
                            "\n1) the raw file is empty or corrupt (in this case, remove it from the QuiXML file)." +
                            "\n2) Thermo libraries are not accessible (restart your computer; if this does not help, check your Xcalibur installation).");

                        Application.DoEvents();
                        lblStatus.Text = "raw file or Thermo libraries not accessible.";
                        return null;
                    }
                }

                //frmReader.openRaw(rawFile);
                int[] fsa = rawReader.fullScansAround(_idScan[a], _numScansToTake);
                // rawReader.closeRaw();

                for (int i = 0; i < _numScansToTake; i++)
                {
                    extraScans[a, i] = fsa[i];
                }
            }

            return extraScans;
        }

        private object propertiesSmoothingPeak(int _idScan,
                                                double _RTsectionWidth,
                                                double _precursorMZ,
                                                double _tolerance,
                                                string _rawFileName)
        {
            object[] result = new object[4];



            // result is
            // result[0] = FullScan where the maximum of the chromatographic peak is
            // result[1] = PeakStart (left inflection point)
            // result[2] = PeakEnd (right inflection point)
            // result[3] = intensity at maximum of chromatographic peak
            return result;
        }

        private int[,] takeScansSweepAlgorithm(int[] _idScan,
                                        double[] _retTimes,
                                        double[] _precursorMZ,
                                        string[] _rawFromScan,
                                        string[] _peptideSequence,
                                        int[] _charges,
                                        string _rawFilePath,
                                        int _numScansToCheck,
                                        int _numScansToTake,
                                        ref int[] firstScanFound,
                                        ref int[] lastScanFound,
                                        int[] _LastScan)
        {
            int[,] extraScans = new int[_idScan.Length, _numScansToTake + 1];
            int totScans = _idScan.Length;
            int[] chromPeak = new int[totScans];
            double[] maxPeakValue = new double[totScans];
            int[] maxPeakScan = new int[totScans];
            double[] maxPeakRT = new double[totScans];
            int totScansInRaw = 0;
            //peakTable = new ArrayList();
            peakTableChrom = new ArrayList();

            int scanStart = 0;
            int scanEnd = 0;

            //frmInvisible reader_old = new frmInvisible();
//             msnPerFull.rawStats reader = new msnPerFull.rawStats();
            
            double percent = 0;
            DateTime writeStart = DateTime.Now;
            string writingMessage = "Calculating chromatographic peaks (part 1/2)... ";

            cacheSize = rawReader.initialiseSpectrumTypes(0);
            if (cacheSize != int.Parse(txbCacheSize.Text))
            {
                txbCacheSize.Text = cacheSize.ToString();
                Application.DoEvents();
            }

            for (int i = 0; i < totScans; i++)
            {
                percent = writePercentage(percent, writeStart, totScans, i, writingMessage);

                string rawFile = _rawFilePath + "\\" + _rawFromScan[i];

                if (rawFile != rawReader.workingRAWFilePath)
                {
                    rawReader.openRawFast(rawFile, cacheSize);
                    int lastScanOfAll = rawReader.lastSpectrumNumber();
                    cacheSize = rawReader.initialiseSpectrumTypes(lastScanOfAll);
                    if (cacheSize != int.Parse(txbCacheSize.Text))
                    {
                        txbCacheSize.Text = cacheSize.ToString();
                        Application.DoEvents();
                    }
                }

                totScansInRaw = rawReader.numSpectra();
                bool thermoLibrariesWorking = (totScansInRaw > 0);

                // this part is to prevent the program from working while it has no access to thermo libraries
                if (!thermoLibrariesWorking)
                {
                    int counterWithNoAccess = 0;

                    while (!thermoLibrariesWorking && counterWithNoAccess < 5)
                    {
                        rawReader.openRawFast(rawFile, cacheSize);
                        totScansInRaw = rawReader.numSpectra();
                        thermoLibrariesWorking = (totScansInRaw > 0);
                        counterWithNoAccess++;
                    }

                    if (!thermoLibrariesWorking)
                    {
                        MessageBox.Show("Problem found while reading raw file " + rawFile +
                            "\nThis might happen for the following reasons:" +
                            "\n1) the raw file is empty or corrupt (in this case, remove it from the QuiXML file)." +
                            "\n2) Thermo libraries are not accessible (restart your computer; if this does not help, check your Xcalibur installation).");

                        Application.DoEvents();
                        lblStatus.Text = "raw file or Thermo libraries not accessible.";
                        return null;
                    }
                }

                maxPeakValue[i] = 0;
                maxPeakScan[i] = 0;
                maxPeakRT[i] = 0;

                if (firstAndLastIncluded)
                {
                    scanStart = _idScan[i];
                    scanEnd = _LastScan[i];
                }
                else
                {
                    //When using retention times, these ones must be converted to scannumbers.
                    if (_retTimes != null)
                    {
                        if (_retTimes.Length > 0)
                            _idScan[i] = rawReader.retentionTimeToIndex(_retTimes[i]);
                    }

                    // in this case, _LastScan[] should be set to null
                    scanStart = Math.Max(1, (int)((double)_idScan[i] - ((double)_numScansToCheck / 2)));
                    scanEnd = Math.Min(totScansInRaw, (int)((double)_idScan[i] + ((double)_numScansToCheck / 2)));
                }

                for (int adding = -1; adding <= 1; adding += 2)
                {
                    int scansWithoutPeak = 0;
                    int j = _idScan[i];

                    while (scansWithoutPeak < maxScansAcceptedWithoutPeak && j >= scanStart && j <= scanEnd)
                    {
                        if (rawReader.spectrumTypeFromScanNumber(j) == spectrumTypes.Full)
                        {
                            object provisionalSpectrum = rawReader.getSpectrum(j, ref cacheSize);
                            if (cacheSize != int.Parse(txbCacheSize.Text))
                            {
                                txbCacheSize.Text = cacheSize.ToString();
                                Application.DoEvents();
                            }

                            double[,] spectrum = null;

                            spectrum = (double[,])provisionalSpectrum;

                            int totPeaks = spectrum.GetUpperBound(1);

                            double sn_f = 0.01;
                            double noise = 0;

                            // if noiseRatio == 0, then noise is not calculated, making it slightly faster
                            if (noiseRatio > 0)
                                noise = avgNoise(spectrum, sn_f);

                            int peakStart = searchPeakStart(_precursorMZ, i, spectrum, totPeaks);

                            bool peakFound = false;

                            for (int peak = peakStart; peak < totPeaks; peak++)
                            {
                                // warning! this is tru only if spectrum[0, peak] are ordered and ascending
                                if (spectrum[0, peak] > _precursorMZ[i] * (1 + tolerance / 1000000))
                                    break;

                                if (Math.Abs(spectrum[0, peak] - _precursorMZ[i]) < _precursorMZ[i] * tolerance / 1000000)
                                {
                                    if (spectrum[1, peak] >= noise * noiseRatio)
                                    {
                                        peakFound = true;

                                        if (!firstAndLastIncluded)
                                        {
                                            if (adding == -1) // so it is covering the left hand side
                                                firstScanFound[i] = j;
                                            else // then adding is 1, so it is covering the right hand side
                                                lastScanFound[i] = j;
                                        }

                                        if (spectrum[1, peak] > maxPeakValue[i])
                                        {
                                            maxPeakValue[i] = spectrum[1, peak];
                                            maxPeakScan[i] = j;
                                            maxPeakRT[i] = rawReader.getRTfromScanNumber(j);

                                            // this means:
                                            // the maximum intensity (maxPeakValue) for the precursor mass (_precursorMass)
                                            // for the peptide identified in the scan i
                                            // is found at the scan maxPeakScan with intensity maxPeakValue
                                            // or
                                            // maxPeakScan[i] is the scan where the peptide identified in the scan i
                                            // is found with maximum intensity, which is maxPeakValue[i]
                                        }
                                    }
                                }
                            }

                            if (!peakFound && !firstAndLastIncluded) scansWithoutPeak++;
                        }

                        j += adding;
                    }
                }

                //reader.closeRaw();
            }

            percent = 0;
            writeStart = DateTime.Now;
            writingMessage = "Calculating scans around peak (part 2/2)... ";

            for (int i = 0; i < totScans; i++)
            {
                string rawFile = _rawFilePath + "\\" + _rawFromScan[i];

                if (rawFile != rawReader.workingRAWFilePath)
                {
                    rawReader.openRawFast(rawFile, cacheSize);
                    int lastScanOfAll = rawReader.lastSpectrumNumber();
                    cacheSize = rawReader.initialiseSpectrumTypes(lastScanOfAll);
                    if (cacheSize != int.Parse(txbCacheSize.Text))
                    {
                        txbCacheSize.Text = cacheSize.ToString();
                        Application.DoEvents();
                    }
                }

                totScansInRaw = rawReader.numSpectra();
                bool thermoLibrariesWorking = (totScansInRaw > 0);

                // this part is to prevent the program from working while it has no access to thermo libraries
                if (!thermoLibrariesWorking)
                {
                    int counterWithNoAccess = 0;

                    while (!thermoLibrariesWorking && counterWithNoAccess < 5)
                    {
                        rawReader.openRawFast(rawFile, cacheSize);
                        totScansInRaw = rawReader.numSpectra();
                        thermoLibrariesWorking = (totScansInRaw > 0);
                        counterWithNoAccess++;
                    }

                    if (!thermoLibrariesWorking)
                    {
                        MessageBox.Show("Problem found while reading raw file " + rawFile +
                            "\nThis might happen for the following reasons:" +
                            "\n1) the raw file is empty or corrupt (in this case, remove it from the QuiXML file)." +
                            "\n2) Thermo libraries are not accessible (restart your computer; if this does not help, check your Xcalibur installation).");

                        Application.DoEvents();
                        lblStatus.Text = "raw file or Thermo libraries not accessible.";
                        //return null;
                    }
                }

                percent = writePercentage(percent, writeStart, totScans, i, writingMessage);

                if (maxPeakScan[i] != 0)
                {
                    int[] fsa = rawReader.fullScansAround(maxPeakScan[i], _numScansToTake);

                    // copies the scans previous to the most intense peak
                    for (int l = 0; l < _numScansToTake / 2; l++)
                    {
                        extraScans[i, l] = fsa[l];
                    }

                    // copies the most intense peak
                    extraScans[i, _numScansToTake / 2] = maxPeakScan[i];

                    // copies the scans after the most intense peak
                    for (int l = 0; l < _numScansToTake / 2; l++)
                    {
                        extraScans[i, l + _numScansToTake / 2 + 1] = fsa[l + _numScansToTake / 2];
                    }

                    //ArrayList peakRow = new ArrayList();
                    ChromPeak chrom = new ChromPeak();

                    if (!firstAndLastIncluded)
                    {
                        chrom.peakStart = firstScanFound[i];
                        chrom.peakEnd = lastScanFound[i];
                    }
                    else
                    {
                        chrom.peakStart = 0;
                        chrom.peakEnd = 0;
                    }

                    if (firstAndLastIncluded)
                        chrom.lastScan = _LastScan[i];
                    else
                        chrom.lastScan = 0;

                    // *** add number of fulls the peak has
                    // *** add elution time

                    chrom.MSMS_scan = _idScan[i];
                    chrom.precursorMZ = _precursorMZ[i];
                    chrom.charge = _charges[i];
                    chrom.maxPeakScan = maxPeakScan[i];
                    chrom.peakIntensity = maxPeakValue[i];
                    
                    chrom.peakMaxRT = maxPeakRT[i];
                    chrom.sequence = _peptideSequence[i];
                    chrom.rawFile = _rawFromScan[i];

                    chrom.isRescued = false;
                    chrom.RTdifference = 0;

                    //peakTable.Add(peakRow);
                    peakTableChrom.Add(chrom);
                }
                else
                {
                    //MessageBox.Show("Error: chromatographic peak was found in scan 0");
                    //return null;

                    //ArrayList peakRow = new ArrayList();
                    ChromPeak chrom = new ChromPeak();

                    if (!firstAndLastIncluded)
                    {
                        chrom.peakStart = firstScanFound[i];
                        chrom.peakEnd = lastScanFound[i];
                    }
                    else
                    {
                        chrom.peakStart = 0;
                        chrom.peakEnd = 0;
                    }

                    if (firstAndLastIncluded)
                        chrom.lastScan = _LastScan[i];
                    else
                        chrom.lastScan = 0;


                    // *** add number of fulls the peak has
                    // *** add elution time

                    chrom.MSMS_scan = _idScan[i];
                    chrom.precursorMZ = _precursorMZ[i];
                    chrom.charge = _charges[i];
                    chrom.maxPeakScan = 0;
                    chrom.peakIntensity = 0;

                    chrom.peakMaxRT = maxPeakRT[i];
                    chrom.sequence = _peptideSequence[i];
                    chrom.rawFile = _rawFromScan[i];

                    chrom.isRescued = false;
                    chrom.RTdifference = 0;

                    peakTableChrom.Add(chrom);

                    //peakTable.Add(peakRow);
                }

                //reader.closeRaw();
            }


            // rawReader.closeRaw();

            return extraScans;
        }

        private int searchPeakStart(double[] _precursorMZ, int i, double[,] spectrum, int totPeaks)
        {
            // warning! this is true only if spectrum[0, peak] are ordered and ascending
            int peakStart = 0;
            int tries = 15;
            double peakMin = 0;
            double peakMax = totPeaks;
            double peakTry = 0;

            for (int n = 0; n < tries; n++)
            {
                peakTry = (double)Math.Truncate((peakMin + peakMax) / 2);
                if (peakTry != (peakMin + peakMax) / 2) peakTry++;
                if (peakTry > totPeaks) peakTry = totPeaks;

                if (spectrum[0, (int)peakTry] == _precursorMZ[i] * (1 - tolerance / 1000000))
                    break;

                if (spectrum[0, (int)peakTry] > _precursorMZ[i] * (1 - tolerance / 1000000))
                    peakMax = peakTry;
                else
                    peakMin = peakTry;
            }

            // this way it will be one or two positions before the peak we are looking for
            peakStart = (int)peakTry - 1;
            return peakStart;
        }

        private static double avgNoise(double[,] spectrum, double sn_f)
        {
            double avgNoise = 0;
            int ttPeaks = spectrum.GetUpperBound(1) + 1;
            for (int peak = 0; peak < ttPeaks; peak++)
                avgNoise += Math.Pow(spectrum[1, peak], sn_f);

            avgNoise = Math.Pow(avgNoise / ttPeaks, 1 / sn_f);
            return avgNoise;
        }

        private double writePercentage(double _percent, DateTime _start, int _totalAmount, int _currentItem, string _percentMessage)
        {
            double roundedPercent = Math.Round(100 * (double)_currentItem / (double)_totalAmount, 2);
            if (roundedPercent > _percent)
            {
                _percent = (double)roundedPercent;
                _percentMessage += _percent.ToString("00.00") + "%";

                // next line is omitted, as apparently time is not properly calculated
                // _percentMessage += getTimeLeft(_percent, _start);

                lblStatus.Text = _percentMessage;
                Application.DoEvents();
            }

            return _percent;
        }

        private static string getTimeLeft(double _percent, DateTime _start)
        {
            string timeLeft = "";

            if (_percent >= 1)
            {
                TimeSpan sinceStart = DateTime.Now - _start;
                TimeSpan toFinish = TimeSpan.FromSeconds((double)sinceStart.TotalSeconds * 100 / _percent) - sinceStart;

                // just in case we get a negative lapse
                if (toFinish.TotalSeconds < 0)
                    toFinish = TimeSpan.FromSeconds(0);

                timeLeft += " (";

                if (toFinish.Minutes > 0)
                {
                    if (toFinish.Hours > 0)
                    {
                        if (toFinish.Days > 0)
                            timeLeft += toFinish.Days.ToString() + " days, ";

                        timeLeft += toFinish.Hours.ToString() + "h ";
                    }

                    timeLeft += toFinish.Minutes.ToString("00") + ":";
                    timeLeft += toFinish.Seconds.ToString("00") + " left).";
                }
                else
                {
                    timeLeft += toFinish.Seconds.ToString() + " secs left).";
                }
            }
            return timeLeft;
        }

        private static int getScanNumberFromFirstScan(int _firstScan, int[] _idScan)
        {
            int scan = -1;

            for (int a = 0; a < _idScan.Count(); a++)
            {
                if (_idScan[a] == _firstScan)
                {
                    scan = a;
                    break;
                }
            }

            return scan;
        }

        private bool hasExtension(string _file, string _extension)
        {
            bool isXML = false;

            if (_file.Length != 0)
            {
                string fileInExtension = _file.Trim().ToLower().Substring(_file.Length - 3);
                isXML = (fileInExtension == _extension);
            }

            return isXML;
        }

        private void dragEnter(object sender, DragEventArgs e, string _extension)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.GetUpperBound(0) == 0 && hasExtension(files[0], _extension))
                    e.Effect = DragDropEffects.All;
            }
        }

        private void txbFile_DragEnter(object sender, DragEventArgs e)
        {
            dragEnter(sender, e, "xml");
        }

        private void lblRaw_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                // make sure the file is a xml file and is unique.
                // (without this, the cursor stays a "NO" symbol)
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.GetUpperBound(0) == 0)
                {
                    //Check wether the file is a folder and it contains RAW files

                    DirectoryInfo myDir = new DirectoryInfo(files[0]);

                    foreach (FileInfo file in myDir.GetFiles())
                    {
                        if (hasExtension(file.ToString(), "raw")) e.Effect = DragDropEffects.All;
                        break;
                    }
                }
            }
        }

        private void txbFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] XMLFileIn = (string[])e.Data.GetData(DataFormats.FileDrop);

            txbFile.Text = XMLFileIn[0];
        }

        private void txbRaw_DragDrop(object sender, DragEventArgs e)
        {
            string[] rawFile = (string[])e.Data.GetData(DataFormats.FileDrop);

            txbRaw.Text = rawFile[0];
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            string XMLFileIn = txbFile.Text.Trim();
            string XSDFileIn = "";
            string rawFileFolder = txbRaw.Text.Trim();
            int numScansToTake = 0;
            int numScansToCheck = 0;

            /*
            //////////////////////////////////////////////

            string zRawFileName = txbRaw.Text;
            double zPrecursorMZ = double.Parse(txbWidthChrom.Text);
            double zTolerance = double.Parse(txbNumScans.Text);
            double zRT = double.Parse(txbTolerance.Text);
            double zRTinterval = double.Parse(txbScansBeforeLeaving.Text);
            double RTstep = 0.05;
            int enlarger = 19; // should be an odd number greater than 1

            object[] resultsForScan = getPeakInformation(zRawFileName,
                            zPrecursorMZ, zTolerance, zRT, zRTinterval, RTstep, enlarger);

            //zRawReader.writeFileWithChromatogramSection
            //    (string.Concat(zRawReader.workingRAWFilePath, "_chOriginal.xls"), zBigSection);

            MessageBox.Show("Réidh le himeacht!");

            return;
            ////////////////////////////////////////////// */

            #region gathering info

            if (rawFileFolder == "")
            {
                MessageBox.Show("No raw file folder selected");
                return;
            }

            if (XMLFileIn == "")
            {
                MessageBox.Show("No XML file selected");
                return;
            }

            if (!cbxSchema.Checked)
            {
                // otherwise the default schema is used (it is different when retention time is used)
                XSDFileIn = txbSchema.Text.Trim();

                if (XSDFileIn == "")
                {
                    MessageBox.Show("No XSD file (schema) selected");
                    return;
                }
            }
            
            ArrayList modList = getModificationsFromForm();

            bool saveFirstScanGraph = cbxFirstScan.Checked && cbxFirstScan.Enabled && cbxFirstScan.Visible;

            try
            {
                cacheSize = int.Parse(txbCacheSize.Text);

                if (cacheSize < 10)
                {
                    MessageBox.Show("The spectum cache size should be at least 10.\n" +
                        "Good figures are between 100 and 1000 (or whatever your machine can withstand).");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem getting the spectrum cache size."
                    + "\n\nError was: " + ex.ToString());
                return;
            }

            // tolerance
            if (method == methodUsed.quadraticSmooth || method == methodUsed.peakSweep)
            {
                try
                {
                    if (txbTolerance.Text == "")
                    {
                        MessageBox.Show("The tolerance was not choosen!", "Error");
                        return;
                    }   //tolerance = 5; // default value when empty
                    else
                    {
                        tolerance = double.Parse(txbTolerance.Text);

                        if (tolerance > 100 || tolerance < 0)
                        {
                            MessageBox.Show("Tolerance is in ppm,\nplease, introduce (small) numbers between 0 and 100 ppm");
                            return;
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Problem while getting tolerance"
                        + "\n\nError was: " + ex.ToString());
                    return;
                }
            }

            if (XSDFileIn == "") XSDFileIn = "conf\\identifications_schema_HR.xsd";

            #endregion

            switch (method)
            {
                case methodUsed.aroundMSMSscan:
                    {
                        #region getting arguments
                        try
                        {
                            numScansToTake = int.Parse(txbNumScans.Text);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Problem getting details\nProbably I could not gather the number of scans to take"
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        if (numScansToTake < 2)
                        {
                            MessageBox.Show("Number of scans to take too low");
                            return;
                        }
                        else
                        {
                            if (Math.Round((double)(numScansToTake) / 2, 0) != (double)(numScansToTake) / 2)
                            {
                                numScansToTake--;
                                // this means no message will be shown when user asks for numScansToTake = odd number,
                                // but it will be decremented by one to suit the program standards
                                // as it should be an even number, to get the same amount of scans each side
                            }
                        }


                        //if (numScansToTake >= 2)
                        // numScansToCheck will not be used, so it is set to 0
                        #endregion

                        openWithScansToTake(XMLFileIn, XSDFileIn, rawFileFolder, 0, numScansToTake, false, mass);
                        break;
                    }

                case methodUsed.XMLdefined:
                    {
                        // numScansToTake == 0 means the file already contains the first and last scan to take
                        // numScansToCheck will not be used, so it is set to 0
                        openWithScansToTake(XMLFileIn, XSDFileIn, rawFileFolder, 0, 0, false, mass);
                        break;
                    }

                case methodUsed.peakSweep:
                    {
                        #region getting arguments
                        // max scans without peak
                        try
                        {
                            maxScansAcceptedWithoutPeak = int.Parse(txbDeltaRTorScansBeforeLeaving.Text);

                            if (maxScansAcceptedWithoutPeak < 0)
                            {
                                MessageBox.Show("The number of scans to admit with no peak should be positive.\nWe recommend using a number between 1 and 8");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Problem getting the number of scans to admit with no peak."
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        // noise ratio
                        try
                        {
                            noiseRatio = double.Parse(txbRTstepOrNoiseRatio.Text);

                            if (noiseRatio < 0)
                            {
                                MessageBox.Show("The noise ratio should be positive or zero,\nwhere zero means no noise will be taken\nand 1 is used to take the standard noise\n(averaging with sn_f = 0.01).");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Problem getting the noise ratio."
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        // chromatographic peak width
                        try
                        {
                            numScansToCheck = int.Parse(txbWidthChrom.Text);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Problem getting width of chromatographix window."
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        // num scans to take
                        try
                        {
                            numScansToTake = int.Parse(txbNumScans.Text);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Problem getting number of scans to take."
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        if (Math.Round((double)(numScansToTake) / 2, 0) != (double)(numScansToTake) / 2)
                        {
                            numScansToTake--;
                            // this means no message will be shown when user asks for numScansToTake = odd number,
                            // but it will be decremented by one to suit the program standards
                            // as it should be an even number, to get the same amount of scans each side
                        }

                        if (numScansToTake < 0)
                        {
                            MessageBox.Show("Number of scans to take should be positive.");
                            return;
                        }

                        if (numScansToCheck < 3 && !firstAndLastIncluded)
                        {
                            MessageBox.Show("Width of chromatographic window too low.\n\nIt should be at least 3,\nbut consider using about 100.");
                            return;
                        }
                        #endregion

                        openWithScansToTake(XMLFileIn, XSDFileIn, rawFileFolder, numScansToCheck, numScansToTake, false, mass);

                        break;
                    }

                case methodUsed.quadraticSmooth:
                    {
                        #region getting arguments

                        // chromatographic width
                        try
                        {
                            chromWidthInRT = double.Parse(txbWidthChrom.Text);

                            if (chromWidthInRT <= 0)
                            {
                                MessageBox.Show("The width of the chromatographic peak should be positive.");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Problem getting the number of scans to admit with no peak."
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        // delta RT
                        try
                        {
                            deltaRT = double.Parse(txbDeltaRTorScansBeforeLeaving.Text);

                            if (deltaRT <= 0)
                            {
                                MessageBox.Show("The deltaRT should be positive.");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Problem getting the number of scans to admit with no peak."
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        // num scans to take
                        try
                        {
                            numScansToTake = int.Parse(txbNumScans.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Problem getting number of scans to take."
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        if (Math.Round((double)(numScansToTake) / 2, 0) != (double)(numScansToTake) / 2)
                        {
                            numScansToTake--;
                            // this means no message will be shown when user asks for numScansToTake = odd number,
                            // but it will be decremented by one to suit the program standards
                            // as it should be an even number, to get the same amount of scans each side
                        }

                        if (numScansToTake < 0)
                        {
                            MessageBox.Show("Number of scans to take should be positive.");
                            return;
                        }

                        // RT step
                        try
                        {
                            RTstep = double.Parse(txbRTstepOrNoiseRatio.Text);

                            if (RTstep < 0)
                            {
                                MessageBox.Show("The RT step should be positive.");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Problem getting the RT step."
                                + "\n\nError was: " + ex.ToString());
                            return;
                        }

                        if (rbnExperimentalMass.Checked)
                            mass = massUsed.experimental;
                        else
                            mass = massUsed.theoretical;

                        #endregion

                        openWithScansToTake(XMLFileIn,
                            XSDFileIn,
                            rawFileFolder,
                            0,
                            numScansToTake,
                            saveFirstScanGraph,
                            mass,
                            modList);

                        break;
                    }
            }

            rawReader.closeRaw();
        }

        private ArrayList getModificationsFromForm()
        {
            ArrayList modList = new ArrayList();

            if (txb18OcharacterK.Text.Trim().Length > 0)
            {
                try
                {
                    Modification modif = new Modification();
                    modif.modificationType = modType.O18;
                    modif.aminoacid = 'K';
                    modif.deltaMass = double.Parse(txb18OdeltaMassK.Text);
                    modif.symbol = char.Parse(txb18OcharacterK.Text);
                    modList.Add(modif);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error while getting modifications for K." +
                        "\nError was:\n\n" + ex.Message + ".");
                }
            }

            if (txb18OcharacterR.Text.Trim().Length > 0)
            {
                try
                {
                    Modification modif = new Modification();
                    modif.modificationType = modType.O18;
                    modif.aminoacid = 'R';
                    modif.deltaMass = double.Parse(txb18OdeltaMassR.Text);
                    modif.symbol = char.Parse(txb18OcharacterR.Text);
                    modList.Add(modif);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error while getting modifications for R." +
                        "\nError was:\n\n" + ex.Message + ".");
                }
            }
            return modList;
        }

        private object[] getPeakInformation(string _rawFileName,
                                            double _precursorMZ,
                                            double _toleranceMZ,
                                            double _RT,
                                            double _RTinterval,
                                            double _RTstep,
                                            bool _saveGraphFile)
        {
            // msnPerFull.rawStats rawReader = new msnPerFull.rawStats();

            rawReader.openRawFast(_rawFileName, cacheSize);
            cacheSize = rawReader.initialiseSpectrumTypes(rawReader.lastSpectrumNumber());
            if (cacheSize.ToString() != txbCacheSize.Text)
            {
                txbCacheSize.Text = cacheSize.ToString();
                Application.DoEvents();
            }

            double[,] bigSection = rawReader.getChromatogramSectionData(_RT,
                _RTinterval, _precursorMZ, _toleranceMZ);

            int maxScanPeak = 0;
            double maxIntensityPeak = 0;
            double RTmaximum = findRTforMaximumOfSmoothed(_RT,
                                                    _RTinterval,
                                                    ref bigSection,
                                                    ref maxScanPeak,
                                                    ref maxIntensityPeak,
                                                    _RTstep,
                                                    _precursorMZ,
                                                    _toleranceMZ,
                                                    rawReader,
                                                    false);

            if (RTmaximum == 0)
                return null; // something went wrong
            // find points of inflection

            int maxScanLeft = 0;
            double maxIntensityLeft = 0;
            double leftRTinflection = getPointOfInflection(ref bigSection,
                                                    ref maxScanLeft,
                                                    ref maxIntensityLeft,
                                                    _RTinterval,
                                                    _RTstep,
                                                    RTmaximum,
                                                    _precursorMZ,
                                                    _toleranceMZ,
                                                    -1,
                                                    rawReader,
                                                    false);

            int maxScanRight = 0;
            double maxIntensityRight = 0;
            // **** protect from end of raw
            double rightRTinflection = getPointOfInflection(ref bigSection,
                                                    ref maxScanRight,
                                                    ref maxIntensityRight,
                                                    _RTinterval,
                                                    _RTstep,
                                                    RTmaximum,
                                                    _precursorMZ,
                                                    _toleranceMZ,
                                                    1,
                                                    rawReader,
                                                    false);

            int firstScan = rawReader.getScanNumberOfPrevOrNextSpectrumByType(leftRTinflection, msnPerFull.rawStats.spectrumPosition.sameOrPrevious, spectrumTypes.Full);
            int lastScan = rawReader.getScanNumberOfPrevOrNextSpectrumByType(rightRTinflection, msnPerFull.rawStats.spectrumPosition.sameOrNext, spectrumTypes.Full);
            if (lastScan == 0) lastScan = firstScan;
            if (firstScan == 0) firstScan = lastScan;

            int maxScan;
            double maxIntensity;
            relativePosition position;
            getMaxScanAndIntensity(maxScanPeak, maxIntensityPeak,
                                    maxScanLeft, maxIntensityLeft,
                                    maxScanRight, maxIntensityRight,
                                    out maxScan, out maxIntensity, out position);

            double RTScanMax = rawReader.getRTfromScanNumber(maxScan);
            if (_saveGraphFile)
            {
                double extraRT = 2;
                bigSection = writeSpectrumGraphFile(_rawFileName,
                    _precursorMZ,
                    _toleranceMZ,
                    _RTinterval,
                    _RTstep,
                    rawReader,
                    bigSection,
                    RTScanMax,
                    leftRTinflection,
                    rightRTinflection,
                    extraRT);
            }

            // rawReader.closeRaw();

            object[] resultsForScan = { maxScan, maxIntensity, firstScan, lastScan, RTScanMax };
            return resultsForScan;
        }

        private static void getMaxScanAndIntensity(int maxScanPeak, double maxIntensityPeak,
                                                int maxScanLeft, double maxIntensityLeft,
                                                int maxScanRight, double maxIntensityRight,
                                                out int maxScan, out double maxIntensity,
                                                out relativePosition position)
        {
            maxScan = maxScanPeak;
            maxIntensity = maxIntensityPeak;
            position = relativePosition.same;

            if (maxIntensityLeft > Math.Max(maxIntensityPeak, maxIntensityRight))
            {
                maxScan = maxScanLeft;
                maxIntensity = maxIntensityLeft;
                position = relativePosition.left;
            }

            if (maxIntensityRight > Math.Max(maxIntensityLeft, maxIntensityPeak))
            {
                maxScan = maxScanRight;
                maxIntensity = maxIntensityRight;
                position = relativePosition.right;
            }
        }

        private double[,] writeSpectrumGraphFile(string _rawFileName,
                                                    double _precursorMZ,
                                                    double _toleranceMZ,
                                                    double _RTinterval,
                                                    double _RTstep,
                                                    msnPerFull.rawStats _rawReader,
                                                    double[,] _bigSection,
                                                    double _RTmaximum,
                                                    double _leftRTinflection,
                                                    double _rightRTinflection,
                                                    double _extraRT)
        {
            int bigSectionLength = _bigSection.GetUpperBound(1);
            double RTstart = _leftRTinflection - _extraRT;
            double RTend = _rightRTinflection + _extraRT;
            string rawFolder = Path.GetDirectoryName(_rawFileName);
            string fileName = string.Concat(
                Path.GetFileNameWithoutExtension(_rawFileName), "_smGraph_mz=",
                _precursorMZ.ToString("0000.000Th"), ".xls");
            fileName = Path.Combine(rawFolder, fileName);
            StreamWriter writer = new StreamWriter(fileName, false, Encoding.Unicode);

            int scanLeft = _rawReader.getScanNumberOfPrevOrNextSpectrumByType(_leftRTinflection,
                msnPerFull.rawStats.spectrumPosition.nearestInRT, spectrumTypes.Full);
            int scanMax = _rawReader.getScanNumberOfPrevOrNextSpectrumByType(_RTmaximum,
                msnPerFull.rawStats.spectrumPosition.nearestInRT, spectrumTypes.Full);
            int scanRight = _rawReader.getScanNumberOfPrevOrNextSpectrumByType(_rightRTinflection,
                msnPerFull.rawStats.spectrumPosition.nearestInRT, spectrumTypes.Full);

            writer.WriteLine(string.Concat("precursor mz = ", _precursorMZ, " Th"));
            writer.WriteLine(string.Concat(@"precursor mz tolerance = ±", _toleranceMZ, " ppm"));
            writer.WriteLine(string.Concat("Left Inflection Point = ", _leftRTinflection, " min, #scan = ", scanLeft));
            writer.WriteLine(string.Concat("Maximum of smoothed = ", _RTmaximum, " min, #scan = ", scanMax));
            writer.WriteLine(string.Concat("Right Inflection Point = ", _rightRTinflection, " min, #scan = ", scanRight));
            writer.WriteLine();

            string header = "exp RT\texp Intensity\tA\tB\tC\tx0\tsm Intensity (a + bx + cx2)\tslope (b + 2cx)\tconcavity (2c)\tinside?";
            writer.WriteLine(header);

            double RTdraw = RTstart;
            while (RTdraw <= RTend)
            {
                Smoother smoothy = new Smoother();
                double[,] smallSection = getSmallSection(ref _bigSection,
                                                            RTdraw,
                                                            _RTinterval,
                                                            _precursorMZ,
                                                            _toleranceMZ,
                                                            0,
                                                            _rawReader);
                smoothy.original = smallSection;
                int index = getIndexFromRT(RTdraw, smallSection);
                double[] coeffs = smoothy.quadraticCoefficientsABC;
                double a = coeffs[0];
                double b = coeffs[1];
                double c = coeffs[2];
                double x0 = smoothy.initialValue;
                double x = smallSection[0, index];
                double yExp = smallSection[1, index];
                double ySmoothed = a + b * (x - x0) + c * Math.Pow((x - x0), 2);
                double slope = b + 2 * c * (x - x0);
                double concavity = 2 * c;
                int inside = 0;
                if (RTdraw >= _leftRTinflection && RTdraw <= _rightRTinflection) inside = 1;
                string line = string.Concat(x, "\t", yExp, "\t", a, "\t", b, "\t", c, "\t", x0, "\t",
                    ySmoothed, "\t", slope, "\t", concavity, "\t", inside);

                writer.WriteLine(line);

                RTdraw = getNextRTposition(RTdraw, _RTstep, 1, _rawReader);
            }

            writer.Close();
            return _bigSection;
        }

        private double getPointOfInflection(ref double[,] _section,
                                                    double _RTinterval,
                                                    double _RTstep,
                                                    double _RTmaximum,
                                                    double _referenceMZ,
                                                    double _tolerance,
                                                    int _direction,
                                                    msnPerFull.rawStats _rawReader,
                                                    bool _saveResults)
        {
            int maxScan = 0;
            double maxIntensity = 0;
            return getPointOfInflection(ref _section,
                                            ref maxScan,
                                            ref maxIntensity,
                                            _RTinterval,
                                            _RTstep,
                                            _RTmaximum,
                                            _referenceMZ,
                                            _tolerance,
                                            _direction,
                                            _rawReader,
                                            _saveResults);
        }

        private double getPointOfInflection(ref double[,] _section,
                                            ref int _maxScan,
                                            ref double _maxIntensity,
                                            double _RTinterval,
                                            double _RTstep,
                                            double _RTmaximum,
                                            double _referenceMZ,
                                            double _tolerance,
                                            int _direction,
                                            msnPerFull.rawStats _rawReader,
                                            bool _saveResults)
        {
            if (_direction == 0) return 0;

            double[,] smallSection = getSmallSection(ref _section,
                                                _RTmaximum,
                                                _RTinterval,
                                                _referenceMZ,
                                                _tolerance,
                                                0,
                                                _rawReader);
            int concavity = getConcavity(smallSection);

            double RTposition = _RTmaximum;

            renewMaxScanAndMaxIntensityFromRTinSmoothed(ref _maxScan,
                                        ref _maxIntensity,
                                        _rawReader,
                                        smallSection,
                                        RTposition);

            if (concavity != 0)
            {
                // as we are at a maximum, the original concavity should be negative
                //
                // smallSection[0, 0] > 0 --> means the first RT should be greater than zero
                // this prevents checking scans previous to the first one in the raw
                while (concavity < 1 && smallSection[0, 0] > 0)
                {
                    RTposition = getNextRTposition(RTposition, _RTstep, _direction, _rawReader);

                    if (RTposition == 0)
                    {// usually means a border of the spectrum has been reached
                        RTposition = _RTmaximum;
                        break;
                    }

                    smallSection = getSmallSection(ref _section,
                                                    RTposition,
                                                    _RTinterval,
                                                    _referenceMZ,
                                                    _tolerance,
                                                    0,
                                                    _rawReader);

                    renewMaxScanAndMaxIntensityFromRTinSmoothed(ref _maxScan,
                                                    ref _maxIntensity,
                                                    _rawReader,
                                                    smallSection,
                                                    RTposition);

                    concavity = getConcavity(smallSection);
                }
            }
            else
            { // usually means the smallSpectrum is empty
                RTposition = _RTmaximum;
            }

            if (_saveResults)
            {
                Smoother smoothy = new Smoother();
                smoothy.original = smallSection;
                _rawReader.writeFileWithChromatogramSection
                    (string.Concat(_rawReader.workingRAWFilePath, "_chBig.xls"), _section);
                _rawReader.writeFileWithChromatogramSection
                    (string.Concat(_rawReader.workingRAWFilePath, "_chSmall.xls"), smallSection);
                _rawReader.writeFileWithChromatogramSection
                    (string.Concat(_rawReader.workingRAWFilePath, "_chSmoothed.xls"), smoothy.quadratic);
            }

            return RTposition;
        }

        private static void renewMaxScanAndMaxIntensityFromRTinSmoothed(ref int _maxScan,
                                                                        ref double _maxIntensity,
                                                                        msnPerFull.rawStats _rawReader,
                                                                        double[,] _smallSection,
                                                                        double _RTposition)
        {
            Smoother smoothy = new Smoother();
            smoothy.original = _smallSection;
            double[,] smoothedGraph = smoothy.quadratic;

            int _smallSectionUpperBound = smoothedGraph.GetUpperBound(1);
            for (int i = 0; i <= _smallSectionUpperBound; i++)
            {
                if (smoothedGraph[0, i] == _RTposition)
                {
                    double intensityPosition = smoothedGraph[1, i];
                    if (intensityPosition > _maxIntensity)
                    {
                        _maxIntensity = intensityPosition;
                        _maxScan = _rawReader.getScanNumberOfPrevOrNextSpectrumByType(_RTposition,
                            msnPerFull.rawStats.spectrumPosition.nearestInRT, spectrumTypes.Full);
                    }

                    return;
                }
            }
        }

        private static int getConcavity(double[,] smallSection)
        {
            if (smallSection.GetUpperBound(1) > 1)
            {
                Smoother smoothTool = new Smoother();
                smoothTool.original = smallSection;
                return Math.Sign(smoothTool.quadraticCoefficientsABC[2]);
            }
            else return 0;
        }

        private double findRTforMaximumOfSmoothed(double _RT,
                                            double _RTinterval,
                                            ref double[,] _section,
                                            double _RTstep,
                                            double _referenceMZ,
                                            double _tolerance,
                                            msnPerFull.rawStats _rawReader,
                                            bool _saveResults)
        {
            int scanMax = 0;
            double maxIntensity = 0;

            return findRTforMaximumOfSmoothed(_RT,
                                            _RTinterval,
                                            ref _section,
                                            ref scanMax,
                                            ref maxIntensity,
                                            _RTstep,
                                            _referenceMZ,
                                            _tolerance,
                                            _rawReader,
                                            _saveResults);
        }

        private double findRTforMaximumOfSmoothed(double _RT,
                                            double _RTinterval,
                                            ref double[,] _section,
                                            ref int _scanMax,
                                            ref double _maxIntensity,
                                            double _RTstep,
                                            double _referenceMZ,
                                            double _tolerance,
                                            msnPerFull.rawStats _rawReader,
                                            bool _saveResults)
        {
            double RTmaximum = 0;
            double[,] smallSection = new double[0, 0];

            // searching direction calculation
            smallSection = getSmallSection(ref _section, _RT, _RTinterval, _referenceMZ, _tolerance, 0, _rawReader);

            if (smallSection == null)
                return 0;

            int searchingDirection = getSlopeDirection(_RT, smallSection);
            
            double RTposition = _RT;

            // get maximum by searching a change in the slope sign
            if (searchingDirection != 0)
            {
                while (RTmaximum == 0)
                {
                    smallSection = getSmallSection(ref _section,
                                                    RTposition,
                                                    _RTinterval,
                                                    _referenceMZ,
                                                    _tolerance,
                                                    0,
                                                    _rawReader);

                    int newSlopeDirection = getSlopeDirection(RTposition, smallSection);

                    int maxIndex = smallSection.GetUpperBound(1);

                    if (newSlopeDirection == searchingDirection) // not yet at maximum
                    {
                        double provRTposition = getNextRTposition(RTposition, _RTstep, searchingDirection, _rawReader);

                        if (provRTposition > 0) RTposition = provRTposition;
                        else
                        {// when provRTposition is 0 usually means the end of the rawFile has been reached
                            _scanMax = _rawReader.getScanNumberOfPrevOrNextSpectrumByType(_RT,
                                msnPerFull.rawStats.spectrumPosition.nearestInRT, spectrumTypes.Full);
                            double[,] provRT = _rawReader.getRTandMaxFromScanNumberOfPrevOrNextFull(_scanMax,
                                _referenceMZ, _tolerance, msnPerFull.rawStats.spectrumPosition.nearestInScanNumber);
                            _scanMax = _rawReader.getScanNumberFromRT(provRT[0, 0]);
                            _maxIntensity = 0; // actually can be different, but as it is probably a border full, this is fine

                            return provRT[0, 0];
                        }
                    }
                    else // local maximum in somewhere inside
                    {
                        RTmaximum = RTposition;

                        if (_rawReader != null && _saveResults)
                        {
                            Smoother smoothy = new Smoother();
                            smoothy.original = smallSection;
                            _rawReader.writeFileWithChromatogramSection
                                (string.Concat(_rawReader.workingRAWFilePath, "_chBig.xls"), _section);
                            _rawReader.writeFileWithChromatogramSection
                                (string.Concat(_rawReader.workingRAWFilePath, "_chSmall.xls"), smallSection);
                            _rawReader.writeFileWithChromatogramSection
                                (string.Concat(_rawReader.workingRAWFilePath, "_chSmoothed.xls"), smoothy.quadratic);
                        }
                    }
                }
            }
            else
            { // when searchingDirection == 0, usually means the spectrum is empty
                _scanMax = _rawReader.getScanNumberOfPrevOrNextSpectrumByType(_RT,
                                msnPerFull.rawStats.spectrumPosition.nearestInRT, spectrumTypes.Full);
                double[,] provRT = _rawReader.getRTandMaxFromScanNumberOfPrevOrNextFull(_scanMax,
                    _referenceMZ, _tolerance, msnPerFull.rawStats.spectrumPosition.nearestInScanNumber);
                _scanMax = _rawReader.getScanNumberFromRT(provRT[0, 0]);
                _maxIntensity = 0; // actually can be different, but as it is probably an empty full, this is fine

                return provRT[0, 0];
            }

            renewMaxScanAndMaxIntensityFromRTinSmoothed(ref _scanMax,
                                            ref _maxIntensity,
                                            _rawReader,
                                            smallSection,
                                            RTmaximum);

            return RTmaximum;
        }

        private static double getNextRTposition(double RTposition,
                                double _RTstep,
                                int searchingDirection,
                                msnPerFull.rawStats _rawReader)
        {
            msnPerFull.rawStats.spectrumPosition sameOrFollowing = msnPerFull.rawStats.spectrumPosition.sameOrNext;
            msnPerFull.rawStats.spectrumPosition following = msnPerFull.rawStats.spectrumPosition.next;

            if (searchingDirection == -1)
            {
                sameOrFollowing = msnPerFull.rawStats.spectrumPosition.sameOrPrevious;
                following = msnPerFull.rawStats.spectrumPosition.previous;
            }

            double provRTposition = RTposition + _RTstep * searchingDirection;
            int nextProvScanPosition = _rawReader.getScanNumberOfPrevOrNextSpectrumByType(provRTposition, sameOrFollowing, spectrumTypes.Full);
            provRTposition = _rawReader.getRTfromScanNumber(nextProvScanPosition);

            int nextScanPosition = _rawReader.getScanNumberOfPrevOrNextSpectrumByType(RTposition, following, spectrumTypes.Full);
            double nextScanRTposition = _rawReader.getRTfromScanNumber(nextScanPosition);
            
            // next line is useful when the end of the raw is reached
            if (nextScanPosition == 0) return 0;
            
            // the step will be at least as small as the distance to the next scan
            // but it can be bigger if RTstep is bigger

            if (searchingDirection == 1)
            {
                if (provRTposition < nextScanRTposition)
                    RTposition = nextScanRTposition;
                else
                    RTposition = provRTposition;
            }
            else
            {
                if (provRTposition > nextScanRTposition)
                    RTposition = nextScanRTposition;
                else
                    RTposition = provRTposition;
            }

            return RTposition;
        }

        private static int getMaxPositionOfSmoothed(double _RT, double[,] _section, int _searchingDirection)
        {
            Smoother smoothTool = new Smoother();
            smoothTool.original = _section;
            double[,] smoothedGraph = smoothTool.quadratic;
            double[] coeffs = smoothTool.quadraticCoefficientsABC;
            double b = coeffs[1];
            double c = coeffs[2];
            double x0 = smoothTool.initialValue;

            int maxPosition = 0;

            if (c >= 0)
            {
                if (_searchingDirection >= 1)
                    maxPosition = smoothedGraph.GetUpperBound(1);
                else
                    maxPosition = 0;
            }
            else
            {
                // get maximum searching a change in the slope

                int currentPosition = getIndexFromRT(_RT, _section);

                int oldSlopeDirection = _searchingDirection;
                while (currentPosition >= 0 && currentPosition <= smoothedGraph.GetUpperBound(1))
                {
                    double slope = b + 2 * c * (smoothedGraph[0, currentPosition] - x0);
                    int newSlopeDirection = Math.Sign(slope);

                    if (newSlopeDirection != oldSlopeDirection)
                    {
                        maxPosition = currentPosition - _searchingDirection;
                        break;
                    }
                    else
                    {
                        maxPosition = currentPosition;
                    }

                    currentPosition += _searchingDirection;
                    oldSlopeDirection = newSlopeDirection;
                }
            }

            return maxPosition;
        }

        private static int getSlopeDirection(double _RT, double[,] _spectrumSection)
        {
            return Math.Sign(getSlope(_RT, _spectrumSection));
        }

        private static double getSlope(double _RT, double[,] _spectrumSection)
        {
            Smoother smoothing = new Smoother();

            smoothing.original = _spectrumSection;
            double[,] smoothedGraph = smoothing.quadratic;


            int RTposition = getIndexFromRT(_RT, smoothedGraph);

            double[] coeffs = smoothing.quadraticCoefficientsABC;

            double a = coeffs[0];
            double b = coeffs[1];
            double c = coeffs[2];
            double x0 = smoothing.initialValue;

            double slope = b + 2 * c * (smoothedGraph[0, RTposition] - x0);
            return slope;
        }

        private static int getIndexFromRT(double _RT, double[,] _graph)
        {
            int RTposition = 0;
            int maxIndex = _graph.GetUpperBound(1);
            while (_graph[0, RTposition] < _RT)
            {
                RTposition++;
                if (RTposition >= maxIndex) break;
            }

            return RTposition;
        }

        private void openWithScansToTake(string XMLFileIn,
                                        string XSDFileIn,
                                        string rawFileFolder,
                                        int numScansToCheck,
                                        int numScansToTake,
                                        bool saveFirstScanGraph,
                                        massUsed mass)
        {
            openWithScansToTake(XMLFileIn,
                                XSDFileIn,
                                rawFileFolder,
                                numScansToCheck,
                                numScansToTake,
                                saveFirstScanGraph,
                                mass,
                                null);
        }

        private void openWithScansToTake(string XMLFileIn,
                                        string XSDFileIn,
                                        string rawFileFolder,
                                        int numScansToCheck,
                                        int numScansToTake,
                                        bool saveFirstScanGraph,
                                        massUsed mass,
                                        ArrayList modificationList)
        {
            //Check wether the file is a folder and it contains RAW files
            bool containsRawFiles = false;

            DirectoryInfo myDir = new DirectoryInfo(rawFileFolder);

            // ****
            //try
            //{
                foreach (FileInfo file in myDir.GetFiles())
                {
                    if (hasExtension(file.ToString(), "raw"))
                    {
                        containsRawFiles = true;
                        if (hasExtension(XMLFileIn, "xml"))
                        {
                            bool errorClean = openFile(XMLFileIn,
                                                        XSDFileIn,
                                                        rawFileFolder,
                                                        numScansToCheck,
                                                        numScansToTake,
                                                        saveFirstScanGraph,
                                                        mass,
                                                        modificationList);
                            if (!errorClean) return;

                            dataSetRecords.Clear();

                            if (!(cbxWritePeakOnly.Enabled &&
                                            cbxWritePeakOnly.Checked) &&
                                            newDataSet != null)
                                newDataSet.Clear();

                            btnGo.Enabled = true;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Only QuiXML files are accepted.");
                            btnGo.Enabled = true;
                            return;
                        }
                    }
                }

                if (!containsRawFiles)
                {
                    MessageBox.Show("No raw files in folder.");
                    btnGo.Enabled = true;
                    return;
                }
                //****!
            //}
            //catch(Exception ex)
            //{
            //    lblStatus.Text = "Problem found, nothing has been written.";
            //    MessageBox.Show("Problem found while reading data.\nError was:\n\n" + ex.Message + ".");
            //    btnGo.Enabled = true;
            //    checkedChanged(null, null, false);
            //    return;
            //}
        }

        public enum massUsed
        {
            experimental,
            theoretical
        }

        public enum methodUsed
        {
            peakSweep,
            XMLdefined,
            aroundMSMSscan,
            quadraticSmooth
        }

        public enum relativePosition
        {
            unknown,
            same,
            left,
            right
        }

        private void rbnAround_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(sender, e);
        }

        private void rbnFindPeak_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(sender, e);
        }

        private void rbnScansInFile_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(sender, e);
        }

        private void cbxWritePeakOnly_CheckedChanged(object sender, EventArgs e)
        {
            checkedChanged(sender, e);
        }

        private void checkedChanged(object sender, EventArgs e)
        {
            checkedChanged(sender, e, true);
        }

        private void checkedChanged(object sender, EventArgs e, bool restartValues)
        {
            if (rbnSmoothing.Checked)
            {
                method = methodUsed.quadraticSmooth;

                gbxMass.Visible = true;
                gbxMass.Enabled = true;
                rbnExperimentalMass.Visible = true;
                rbnExperimentalMass.Enabled = true;
                tipExperimentalMass.SetToolTip(this.rbnExperimentalMass,
                    "This is the mass contained within the <PrecursorMass>\n" +
                    "tag in QuiXML files, and comes from the header of the spectra\n" +
                    "in the raw file.");

                rbnTheoreticalMass.Visible = true;
                rbnTheoreticalMass.Enabled = true;
                tipTheoreticalMass.SetToolTip(this.rbnTheoreticalMass,
                    "This is the mass contained within the <q_peptide_Mass>\n" +
                    "tag in QuiXMLfiles, and is the result of adding up the masses\n" +
                    "of all the amino acids of the identified peptides.");


                lblWidthChrom.Visible = false;
                lblWidthChrom.Enabled = false;
                lblWidthChrom.Text = "width of chromatographic peak (in mins)";
                txbWidthChrom.Visible = false;
                txbWidthChrom.Enabled = false;
                if (restartValues)
                    txbWidthChrom.Text = "2";
                tipWidthChrom.SetToolTip(this.lblWidthChrom,
                    "The lenght of your chromatographic peaks in minutes.\n" +
                    "Allowing a bit more than what you need might be a good practice.");
                tipWidthChrom.SetToolTip(this.txbWidthChrom,
                    tipWidthChrom.GetToolTip(this.lblWidthChrom));

                lblNumScans.Visible = true;
                lblNumScans.Enabled = true;
                lblNumScans.Text = "number of full scans to take per identification scan";
                txbNumScans.Visible = true;
                txbNumScans.Enabled = false;
                if (restartValues)
                    txbNumScans.Text = "0";
                tipNumScans.SetToolTip(this.lblNumScans,
                    "(Note: this feature has not been implemented yet for this method)\n\n" +
                    "If you want, you can take more than one fullscan for each chromatographic peak.\n" +
                    "If this is 0 or 1, you will get only the fullscan corresponding to the most intense\n" +
                    "peak in the chromatographic peak. If this is, for instance, 5, then you will get the\n" +
                    "most intense, and additionally two fullscans each side.");
                tipNumScans.SetToolTip(this.txbNumScans, tipNumScans.GetToolTip(this.lblNumScans));

                lblTolerance.Visible = true;
                lblTolerance.Enabled = true;
                lblTolerance.Text = "mz tolerance (ppm)";
                txbTolerance.Visible = true;
                txbTolerance.Enabled = true;
                if (restartValues)
                    txbTolerance.Text = "5";
                tipTolerance.SetToolTip(this.lblTolerance,
                    "The tolerance in ppm of your precursor ions\n" +
                    "For the precursor mz, the field PrecursorMass of the QuiXML is used\n" +
                    "(which should correspond to the experimental mass).\n" +
                    "(note that the mz window is twice this, as it is ±tolerance)");
                tipTolerance.SetToolTip(this.txbTolerance,
                    tipTolerance.GetToolTip(this.lblTolerance));

                lblDeltaRTorScansBeforeLeaving.Visible = true;
                lblDeltaRTorScansBeforeLeaving.Enabled = true;
                lblDeltaRTorScansBeforeLeaving.Text = "delta RT (in mins)";
                txbDeltaRTorScansBeforeLeaving.Visible = true;
                txbDeltaRTorScansBeforeLeaving.Enabled = true;
                if (restartValues)
                    txbDeltaRTorScansBeforeLeaving.Text = "0.5";
                tipDeltaRTorScansBeforeLeaving.SetToolTip(this.lblDeltaRTorScansBeforeLeaving,
                    "Width of the part of the chromatographic\npeak you take to get the quadratic smooth.");
                tipDeltaRTorScansBeforeLeaving.SetToolTip(this.txbDeltaRTorScansBeforeLeaving,
                    tipDeltaRTorScansBeforeLeaving.GetToolTip(this.lblDeltaRTorScansBeforeLeaving));

                lblRTstepOrNoiseRatio.Visible = true;
                lblRTstepOrNoiseRatio.Enabled = true;
                lblRTstepOrNoiseRatio.Text = "RT step (in mins)";
                txbRTstepOrNoiseRatio.Visible = true;
                txbRTstepOrNoiseRatio.Enabled = true;
                if (restartValues)
                    txbRTstepOrNoiseRatio.Text = "0";
                tipRTstepOrNoiseRatio.SetToolTip(this.txbRTstepOrNoiseRatio,
                    "The RT step used to search maximum and points of inflection.\n" +
                    "If this is shorter than the space between fullScans, then the latter will be taken,\n" +
                    "(this is useful in the case you have many scans and you do not need to check all of them).");
                tipRTstepOrNoiseRatio.SetToolTip(this.lblRTstepOrNoiseRatio,
                    tipRTstepOrNoiseRatio.GetToolTip(this.txbRTstepOrNoiseRatio));

                cbxFirstScan.Enabled = true;
                cbxFirstScan.Visible = true;
                tipFirstScan.SetToolTip(this.cbxFirstScan,
                    "You can save the correspnding graph for the first peptide match in the XML,\n" +
                    "where you will see the chromatogram section before and after smoothing\n" +
                    "including inflection points and maximum.\n\n"+
                    "(The corresponding file is saved in the raw folder,\nunder a name like \"RawFile_smGraph_mz=0961.533Th\").");

                cbxWritePeakOnly.Visible = true;
                cbxWritePeakOnly.Enabled = true;
                cbxWritePeakOnly.Text = "write peak file only (not the XML)";
                tipWritePeakFileOnly.SetToolTip(this.cbxWritePeakOnly,
                    "A tab-separated values file with information about the chromatographic peaks\n" +
                    "is written in the folder containing the raw files. If you want, you can choose to get only this file.");
                
                return;
            }


            if (rbnScansInFile.Checked)
            {
                method = methodUsed.XMLdefined;

                gbxMass.Visible = false;
                gbxMass.Enabled = false;
                rbnExperimentalMass.Visible = false;
                rbnExperimentalMass.Enabled = false;
                rbnTheoreticalMass.Visible = false;
                rbnTheoreticalMass.Enabled = false;

                lblWidthChrom.Visible = false;
                lblWidthChrom.Enabled = false;
                txbWidthChrom.Visible = false;
                txbWidthChrom.Enabled = false;

                lblNumScans.Visible = false;
                lblNumScans.Enabled = false;
                txbNumScans.Visible = false;
                txbNumScans.Enabled = false;

                lblTolerance.Visible = false;
                lblTolerance.Enabled = false;
                txbTolerance.Visible = false;
                txbTolerance.Enabled = false;

                lblDeltaRTorScansBeforeLeaving.Visible = false;
                lblDeltaRTorScansBeforeLeaving.Enabled = false;
                txbDeltaRTorScansBeforeLeaving.Visible = false;
                txbDeltaRTorScansBeforeLeaving.Enabled = false;

                lblRTstepOrNoiseRatio.Visible = false;
                lblRTstepOrNoiseRatio.Enabled = false;
                txbRTstepOrNoiseRatio.Visible = false;
                txbRTstepOrNoiseRatio.Enabled = false;
                
                cbxWritePeakOnly.Visible = false;
                cbxWritePeakOnly.Enabled = false;

                cbxFirstScan.Enabled = false;
                cbxFirstScan.Visible = false;

                return;
            }

            if (rbnFindPeak.Checked)
            {
                method = methodUsed.peakSweep;

                gbxMass.Visible = false;
                gbxMass.Enabled = false;
                rbnExperimentalMass.Visible = false;
                rbnExperimentalMass.Enabled = false;
                rbnTheoreticalMass.Visible = false;
                rbnTheoreticalMass.Enabled = false;

                lblWidthChrom.Visible = true;
                lblWidthChrom.Enabled = true;
                lblWidthChrom.Text = "width of chromatographic peak (in scans)";
                txbWidthChrom.Visible = true;
                txbWidthChrom.Enabled = true;
                if (restartValues)
                    txbWidthChrom.Text = "110";
                tipWidthChrom.SetToolTip(this.lblWidthChrom,
                    "The lenght of your chromatographic peaks.\n" +
                    "Please, note this is in SCANS (including scans of every type),\n" +
                    "so you have to calculate bradly how many scans you will get.");
                tipWidthChrom.SetToolTip(this.txbWidthChrom,
                    tipWidthChrom.GetToolTip(this.lblWidthChrom));

                lblNumScans.Visible = true;
                lblNumScans.Enabled = true;
                lblNumScans.Text = "number of full scans to take per identification scan";
                txbNumScans.Visible = true;
                txbNumScans.Enabled = true;
                if (restartValues)
                    txbNumScans.Text = "0";
                tipNumScans.SetToolTip(this.lblNumScans,
                    "If you want, you can take more than one fullscan for each chromatographic peak.\n" +
                    "If this is 0 or 1, you will get only the fullscan corresponding to the most intense\n" +
                    "peak in the chromatographic peak. If this is, for instance, 5, then you will get the\n" +
                    "most intense, and additionally two fullscans each side.");
                tipNumScans.SetToolTip(this.txbNumScans, tipNumScans.GetToolTip(this.lblNumScans));

                lblTolerance.Visible = true;
                lblTolerance.Enabled = true;
                lblTolerance.Text = "mz tolerance (ppm)";
                txbTolerance.Visible = true;
                txbTolerance.Enabled = true;
                if (restartValues)
                    txbTolerance.Text = "5";
                tipTolerance.SetToolTip(this.lblTolerance,
                    "The tolerance in Th of your precursor ions.");
                tipTolerance.SetToolTip(this.txbWidthChrom,
                    tipTolerance.GetToolTip(this.lblTolerance));

                lblDeltaRTorScansBeforeLeaving.Visible = true;
                lblDeltaRTorScansBeforeLeaving.Enabled = true;
                lblDeltaRTorScansBeforeLeaving.Text = "scans taken before leaving peak";
                txbDeltaRTorScansBeforeLeaving.Visible = true;
                txbDeltaRTorScansBeforeLeaving.Enabled = true;
                if (restartValues)
                    txbDeltaRTorScansBeforeLeaving.Text = "2";
                tipDeltaRTorScansBeforeLeaving.SetToolTip(this.lblDeltaRTorScansBeforeLeaving,
                    "Amount of fullscans allowed to have intensity zero (in any direction)\nbefore the algorithm stops searching within the chromatographic peak.");
                tipDeltaRTorScansBeforeLeaving.SetToolTip(this.txbDeltaRTorScansBeforeLeaving,
                    tipDeltaRTorScansBeforeLeaving.GetToolTip(this.lblDeltaRTorScansBeforeLeaving));

                lblRTstepOrNoiseRatio.Visible = true;
                lblRTstepOrNoiseRatio.Enabled = true;
                lblRTstepOrNoiseRatio.Text = "noise ratio";
                txbRTstepOrNoiseRatio.Visible = true;
                txbRTstepOrNoiseRatio.Enabled = true;
                if (restartValues)
                    txbRTstepOrNoiseRatio.Text = "0";
                tipRTstepOrNoiseRatio.SetToolTip(this.txbRTstepOrNoiseRatio,
                    "1 = noise using the QuiXoT algorithm, 0 = no noise\n(you can use also anything between these figures).");
                tipRTstepOrNoiseRatio.SetToolTip(this.lblRTstepOrNoiseRatio,
                    tipRTstepOrNoiseRatio.GetToolTip(this.txbRTstepOrNoiseRatio));

                cbxWritePeakOnly.Visible = true;
                cbxWritePeakOnly.Enabled = true;
                cbxWritePeakOnly.Text = "write peak file only (not the XML)";
                tipWritePeakFileOnly.SetToolTip(this.cbxWritePeakOnly,
                    "A tab-separated values file with information about the chromatographic peaks\n" +
                    "is written in the folder containing the raw files. If you want, you can choose to get only this file.");

                cbxFirstScan.Enabled = false;
                cbxFirstScan.Visible = false;

                return;
            }

            if (rbnAround.Checked)
            {
                method = methodUsed.aroundMSMSscan;

                gbxMass.Visible = false;
                gbxMass.Enabled = false;
                rbnExperimentalMass.Visible = false;
                rbnExperimentalMass.Enabled = false;
                rbnTheoreticalMass.Visible = false;
                rbnTheoreticalMass.Enabled = false;

                lblWidthChrom.Visible = false;
                lblWidthChrom.Enabled = false;
                txbWidthChrom.Visible = false;
                txbWidthChrom.Enabled = false;

                lblNumScans.Visible = true;
                lblNumScans.Enabled = true;
                lblNumScans.Text = "number of full scans to take per identification scan";
                txbNumScans.Visible = true;
                txbNumScans.Enabled = true;
                if (restartValues)
                    txbNumScans.Text = "2";
                tipNumScans.SetToolTip(this.lblNumScans,
                    "The number of fullscans you want to take arounf your MSMS scan.\n" +
                    "If, for instance, you set this to 6, you will get three each side.");
                tipNumScans.SetToolTip(this.txbNumScans, tipNumScans.GetToolTip(this.lblNumScans));

                lblTolerance.Visible = false;
                lblTolerance.Enabled = false;
                txbTolerance.Visible = false;
                txbTolerance.Enabled = false;

                lblDeltaRTorScansBeforeLeaving.Visible = false;
                lblDeltaRTorScansBeforeLeaving.Enabled = false;
                txbDeltaRTorScansBeforeLeaving.Visible = false;
                txbDeltaRTorScansBeforeLeaving.Enabled = false;

                lblRTstepOrNoiseRatio.Visible = false;
                lblRTstepOrNoiseRatio.Enabled = false;
                txbRTstepOrNoiseRatio.Visible = false;
                txbRTstepOrNoiseRatio.Enabled = false;

                cbxWritePeakOnly.Visible = true;
                cbxWritePeakOnly.Enabled = true;
                cbxWritePeakOnly.Text = "write peak file only (not the XML)";
                tipWritePeakFileOnly.SetToolTip(this.cbxWritePeakOnly,
                    "A tab-separated values file with information about the chromatographic peaks\n" +
                    "is written in the folder containing the raw files. If you want, you can choose to get only this file.");

                cbxFirstScan.Enabled = false;
                cbxFirstScan.Visible = false;

                return;
            }
        }

        private void cbxSchema_CheckedChanged(object sender, EventArgs e)
        {
            lblSchema.Visible = !cbxSchema.Checked;
            txbSchema.Visible = !cbxSchema.Checked;
        }

        private void txbSchema_DragEnter(object sender, DragEventArgs e)
        {
            dragEnter(sender, e, "xsd");
        }

        private void txbSchema_DragDrop(object sender, DragEventArgs e)
        {
            string[] XSDFileIn = (string[])e.Data.GetData(DataFormats.FileDrop);

            txbSchema.Text = XSDFileIn[0];
        }
            //double[,] zBigSection = zRawReader.getChromatogramSectionData(zRT,
            //    zEnlarger * zRTinterval, zPrecursonMZ, zTolerance);

            //double[,] zSmallSection = getSmallSection(zBigSection, zRT, zRTinterval, 0);
        private double[,] getSmallSection(ref double[,] _bigSection,
                                            double _RT,
                                            double _RTinterval,
                                            double _referenceMZ,
                                            double _tolerance,
                                            double _part,
                                            msnPerFull.rawStats _rawReader)
        {
            int firstScan = 0;
            int lastScan = 0;
            double RTstart = _RT + _RTinterval * (_part - 0.5);
            double RTend = _RT + _RTinterval * (_part + 0.5);
            double maxRT = _rawReader.getMaxRT();

            if (maxRT == 0)
                return null; // something weird happened, empty, corrupt or inaccessible raw file

            // **** call here which is the maximum and protect upper part

            bool foundEndOfRaw = false;
            while (RTstart < _bigSection[0, 0] && RTstart > 0 && !foundEndOfRaw)
                _bigSection = addFullOnSide(_bigSection, _referenceMZ, _tolerance, _rawReader, -1, ref foundEndOfRaw);

            while (RTend > _bigSection[0, _bigSection.GetUpperBound(1)] && RTend < maxRT && !foundEndOfRaw)
                _bigSection = addFullOnSide(_bigSection, _referenceMZ, _tolerance, _rawReader, 1, ref foundEndOfRaw);

            if (_bigSection[0, 0] > RTend)
                return null;

            while (_bigSection[0, firstScan] < RTstart)
            {
                if (firstScan >= _bigSection.GetUpperBound(1)) return null;
                firstScan++;
            }

            lastScan = firstScan + 1;
            while (_bigSection[0, lastScan] < RTend)
            {
                lastScan++;
                if (lastScan > _bigSection.GetUpperBound(1)) break;
            }
            lastScan--;

            int smallSectionCount = lastScan - firstScan + 1;
            double[,] smallSection = new double[2, smallSectionCount];

            int si = 0;
            for (int i = 0; i <= 1; i++)
            {
                int sj = 0;
                for (int j = firstScan; j <= lastScan; j++)
                {
                    smallSection[si, sj] = _bigSection[i, j];
                    sj++;
                }
                si++;
            }

            return smallSection;
        }

        private double[,] addFullOnSide(double[,] _bigSection,
                                            double _referenceMZ,
                                            double _tolerance,
                                            msnPerFull.rawStats _rawReader,
                                            int _side, // -1 = left, 1 = right
                                            ref bool endOfRawReached)
        {
            int bigSectionElements = _bigSection.GetUpperBound(1) + 1;
            double[,] newBigSection = new double[2, bigSectionElements + 1];
            double[,] spectrumToAdd;
            endOfRawReached = false;

            switch (_side)
            {
                case -1:
                    {
                        for (int i = 1; i < bigSectionElements + 1; i++)
                            for (int j = 0; j <= 1; j++)
                                newBigSection[j, i] = _bigSection[j, i - 1];

                        int scanNumberOfFirstSpectrum =
                            _rawReader.getScanNumberFromRT(_bigSection[0, 0]);
                        spectrumToAdd =
                            _rawReader.getRTandMaxFromScanNumberOfPrevOrNextFull(scanNumberOfFirstSpectrum,
                            _referenceMZ, _tolerance, msnPerFull.rawStats.spectrumPosition.previous);

                        if (spectrumToAdd == null)
                        {
                            endOfRawReached = true;
                            return _bigSection;
                        }

                        for (int j = 0; j <= 1; j++)
                            newBigSection[j, 0] = spectrumToAdd[j, 0];

                        break;
                    }

                case 1:
                    {
                        for (int i = 0; i < bigSectionElements; i++)
                            for (int j = 0; j <= 1; j++)
                                newBigSection[j, i] = _bigSection[j, i];

                        int scanNumberOfLastSpectrum =
                            _rawReader.getScanNumberFromRT(_bigSection[0, bigSectionElements - 1]);
                        spectrumToAdd =
                            _rawReader.getRTandMaxFromScanNumberOfPrevOrNextFull(scanNumberOfLastSpectrum,
                            _referenceMZ, _tolerance, msnPerFull.rawStats.spectrumPosition.next);

                        if (spectrumToAdd == null)
                        {
                            endOfRawReached = true;
                            return _bigSection;
                        }

                        for (int j = 0; j <= 1; j++)
                            newBigSection[j, bigSectionElements] = spectrumToAdd[j, 0];

                        break;
                    }
            }

            try
            {
                _bigSection = (double[,])newBigSection.Clone();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Exception of type 'System.OutOfMemoryException' was thrown.")
                {
                    cacheSize = _rawReader.sCache.lowerCacheSize(0.9);
                    if (cacheSize != int.Parse(txbCacheSize.Text))
                    {
                        txbCacheSize.Text = cacheSize.ToString();
                        Application.DoEvents();
                    }

                    //MessageBox.Show("Memory is full, the spectrum cache will be lowered to " +
                    //    cacheSize.ToString() + ".");
                    //Application.DoEvents();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    
                    _bigSection = (double[,])newBigSection.Clone();
                }
                else
                {
                    MessageBox.Show("There was an error at " + System.Reflection.MethodInfo.GetCurrentMethod()
                        + ".\n" + "Message was: " + ex.Message);
                    Application.DoEvents();
                }
            }

            return _bigSection;
        }
    }

    public class ChromPeak
    {
        // this class is to replace peakRows in peakTable
        public int MSMS_scan;
        public double precursorMZ;
        public int charge;
        public int maxPeakScan;
        public double peakIntensity;
        public int peakStart;
        public int peakEnd;
        public double peakMaxRT;
        public string sequence;
        public string rawFile;
        public int lastScan;
        public bool isRescued;
        public double RTdifference;
    }
}