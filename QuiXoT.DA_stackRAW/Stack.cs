using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using QuiXoT.math;
using QuiXoT.lookUp;
using QuiXoT.DA_Raw;


namespace QuiXoT.DA_stackRAW
{
      
    [Serializable]
    public struct scanIndexStrt
    {
        private int FirstScanVal;
        private int frameVal;
 
        public scanIndexStrt(int FirstScanValue, int frameValue)
        {
            FirstScanVal = FirstScanValue;
            frameVal = frameValue;
        }

        public int FirstScan
        {
            get { return FirstScanVal; }
            set { FirstScanVal = value; }
        }

        public int frame
        {
            get { return frameVal; }
            set { frameVal = value; }
        }
    }

    [Serializable]
    public class binFrame
    {
        //declare the class properties
        protected int start, end, theSize;
        public scanStrt[] scan;
        public int frame;
        public int access;

        #region general methods of binFrame class
        /// <summary>
        /// construct a new list given the capacity
        /// </summary>
        /// <param name="capacity">(int)number of scans in frame</param>
        public binFrame(int capacity)
        {
            //allocate memory for components' list
            scan = new scanStrt[capacity];

            //start, end and size ar 0 (list is empty)
            start = end = theSize = 0;  
                       
        }
        /// <summary>
        /// check wether this list is empty
        /// </summary>
        /// <returns>(bool)true if the list is empty</returns>
        public bool isEmpty()
        {
            return theSize == 0;
        }
        /// <summary>
        /// check wether this list is full
        /// </summary>
        /// <returns>(bool)true if the list is full</returns>
        public bool isFull() 
        {
            return theSize >= scan.Length;
        }
        /// <summary>
        /// get the size of this list
        /// </summary>
        /// <returns>(int)size of list</returns>
        public int size() 
        {
            return theSize;
        }
        /// <summary>
        /// insert a new scan spectrum
        /// </summary>
        /// <param name="newScan">(QuiXoT.DA_Raw.scanStrt)scan</param>
        public void insert(scanStrt newScan)
        {

            // if insert won't overflow list
            if (theSize < scan.Length)
            {

                // increment start and set element
                scan[start = (start + 1) % scan.Length] = newScan;

                // increment list size (we've added an element)
                theSize++;
            }
 
        }
        /// <summary>
        /// peek at an element in the list 
        /// </summary>
        /// <param name="offset">(int)array index to point</param>
        /// <returns>(QuiXoT.DA_Raw.scanStrt)selected scan</returns>
        public scanStrt peek(int offset)
        {
            scanStrt ret = new scanStrt();

            // is someone trying to peek beyond our size?
            if (offset >= theSize)
                return ret;

            // get object we're peeking at (do not remove it)
            return scan[(end + offset + 1) % scan.Length];
        }
        #endregion
    }

    [Serializable]
    public class binStack
    {
  
        //declare the class properties
        protected int start, end, theSize;
        public scanIndexStrt[] scan;
        public string rawFileName;

        public static binStack[] genIndex(string fileXml, string rawPath, int scansbyframe)
        {
            int numMatches = countMatches(fileXml);

            int numFrames = countFrames(fileXml, scansbyframe);
           
            int numRawFiles;
            LookupCollection RawFilesColl = countRawFiles(fileXml, out numRawFiles);

            //create index
            binStack[] stackIndex = new binStack[numRawFiles];

            //fill rawfilenames in the index
            ArrayList rawFilesKeys = (ArrayList)RawFilesColl.Keys;
            ArrayList rawFilesValues = (ArrayList)RawFilesColl.Values;
            for (int i = 0; i < numRawFiles; i++)
            {
                stackIndex[i] = new binStack((int)rawFilesValues[i]);
                stackIndex[i].rawFileName = rawFilesKeys[i].ToString();    
                
            }
            fillScans(stackIndex,fileXml);
            
            //share out the spectra amongst the frames
            int currFrame=1;
            int currScanofFrame=1;
            for (int i = 0; i < numRawFiles; i++)
            {
                for (int j = 0; j <= stackIndex[i].scan.GetUpperBound(0); j++)
                {
                    stackIndex[i].scan[j].frame = currFrame;
                    currScanofFrame++;
                    if (currScanofFrame > scansbyframe)
                    {
                        currFrame++;
                        currScanofFrame = 1;
                    }
                }
            }

            return stackIndex;

        }

        private static void fillScans(binStack[] stackIndex, string fileXml)
        {
            //Initialize necessary objets for XML reading
            XmlTextReader reader = new XmlTextReader(fileXml);
            XmlNodeType nType = reader.NodeType;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(reader);

            XmlNodeList xmlnodeMatch = xmldoc.GetElementsByTagName("peptide_match");

            scanIndexStrt newScan = new scanIndexStrt();
            for (int i = 0; i <= stackIndex.GetUpperBound(0); i++)
            {
                foreach (XmlNode node in xmlnodeMatch)
                {

                    if (node["RAWFileName"].InnerText.ToString().Trim() == stackIndex[i].rawFileName.Trim())
                    {
                        newScan.FirstScan = int.Parse(node["FirstScan"].InnerText.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        stackIndex[i].insert(newScan);
                    }
                }

            }

        }

        public static binFrame genFrame(binStack[] stackIndex, int frame, int scansbyframe, string rawPath, string spectrumType, string spectrumPosition)
        {

            binFrame currFrame = new binFrame(scansbyframe);
            currFrame.frame = frame;

            int currScanofFrame = 0;
            for (int j = 0; j <= stackIndex.GetUpperBound(0); j++)
            {
                for (int k = 0; k <= stackIndex[j].scan.GetUpperBound(0); k++)
                {
                    if (currFrame.frame == stackIndex[j].scan[k].frame)
                    {
                        currFrame.scan[currScanofFrame].rawFileName = stackIndex[j].rawFileName;
                        currFrame.scan[currScanofFrame].scanNumber = stackIndex[j].scan[k].FirstScan;
                        currScanofFrame++;
                    }
                }

            }

            int nRawsinFrame;
            LookupCollection rawsinFrame = RawFilesinFrame(currFrame,out nRawsinFrame);

            DA_raws[] tRawList = new DA_raws[nRawsinFrame];
            ArrayList rawFilesKeys = (ArrayList)rawsinFrame.Keys;
            ArrayList rawFilesValues = (ArrayList)rawsinFrame.Values;

            for (int i = 0; i < nRawsinFrame; i++)
            {

                tRawList[i] = new DA_raws((int)rawFilesValues[i]);

                tRawList[i].rawFile = rawFilesKeys[i].ToString();

                for (int j = tRawList[i].scan.GetLowerBound(0); j <= tRawList[i].scan.GetUpperBound(0); j++)
                {
                    tRawList[i].scan[j].rawFileName = rawFilesKeys[i].ToString();
                }
                
            }

            for (int i = 0; i <= currFrame.scan.GetUpperBound(0); i++)
            {
                for (int j = tRawList.GetLowerBound(0); j <= tRawList.GetUpperBound(0); j++)
                {
                    if (currFrame.scan[i].rawFileName != null)
                    {
                        if (currFrame.scan[i].rawFileName.Trim() == tRawList[j].rawFile.Trim())
                        {
                            for (int k = tRawList[j].scan.GetLowerBound(0); k <= tRawList[j].scan.GetUpperBound(0); k++)
                            {
                                if (tRawList[j].scan[k].scanNumber == 0)
                                {
                                    tRawList[j].scan[k].scanNumber = currFrame.scan[i].scanNumber;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < nRawsinFrame; i++)
            {

                int[] scansRawList = new int[tRawList[i].scan.GetLength(0)];

                for (int j = 0; j <= scansRawList.GetUpperBound(0); j++)
                {
                    scansRawList[j] = tRawList[i].scan[j].scanNumber; //These are the MSMS identified spectra
                }
                int t = tRawList[i].size();
                string rawFileName = tRawList[i].rawFile;
                DA_raw daRaw1 = new DA_raw();
                Comb.mzI[][] scansRaw = daRaw1.ReadScanRaw(rawPath, rawFileName, scansRawList,spectrumType,spectrumPosition);
                daRaw1 = null;

                for (int j = 0; j <= scansRawList.GetUpperBound(0); j++)
                {
                    if (scansRaw[j] != null)
                    {
                        tRawList[i].scan[j].spectrum = (Comb.mzI[])scansRaw[j];
                        tRawList[i].scan[j].rawFileName = tRawList[i].rawFile;
                    }
                }

            }

            for (int i = currFrame.scan.GetLowerBound(0); i <= currFrame.scan.GetUpperBound(0); i++)
            {
                for (int j = tRawList.GetLowerBound(0); j <= tRawList.GetUpperBound(0); j++)
                {
                    if (currFrame.scan[i].rawFileName != null)
                    {
                        if (currFrame.scan[i].rawFileName.Trim() == tRawList[j].rawFile.Trim())
                        {
                            for (int k = tRawList[j].scan.GetLowerBound(0); k <= tRawList[j].scan.GetUpperBound(0); k++)
                            {
                                if (currFrame.scan[i].scanNumber == tRawList[j].scan[k].scanNumber)
                                {
                                    currFrame.scan[i].spectrum = tRawList[j].scan[k].spectrum;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                } 
            }


          
            return currFrame;

 
        }
        public static binFrame loadFrame(int frame, string framesPath)
        {
            binFrame tFrame;
            try 
            {
                string frameFile = framesPath + frame.ToString().Trim() + ".bfr";
                FileStream q = new FileStream(frameFile, FileMode.Open, FileAccess.Read);
                BinaryFormatter b = new BinaryFormatter();
                tFrame = (binFrame)b.Deserialize(q);
                q.Close();
            }
            catch 
            {
                tFrame = null;
            }

            return tFrame;
        }
        public static binFrame[] delOneFrame(binFrame[] stackFrames)
        {

            int maxAccess = 0;
            int maxAccessFrame = 0;
            //search the most accessed frame
            for (int i = stackFrames.GetLowerBound(0); i <= stackFrames.GetUpperBound(0); i++)
            {
                if (stackFrames[i].access >= maxAccess)
                {
                    maxAccess = stackFrames[i].access;
                    maxAccessFrame = stackFrames[i].frame;
                }

            }

            //del the most accessed frame
            for (int i = stackFrames.GetLowerBound(0); i <= stackFrames.GetUpperBound(0); i++)
            {
                if (stackFrames[i].frame == maxAccessFrame)
                {
                    stackFrames[i] = null;
                }
            }

            return stackFrames;

        }

        private static bool checkFrameCollector(int frame, binFrame[] frameCollector)
        {
            bool isInCollector = false;

            for (int i = frameCollector.GetLowerBound(0); i <= frameCollector.GetUpperBound(0); i++)
            {
                if (frameCollector[i]!=null)
                {
                    if (frameCollector[i].frame == frame)
                    {
                    isInCollector = true;
                    break;
                    }
                }
            }

            return isInCollector;
        }

        public static Comb.mzI[] peakSpectrum(      binStack[] stackIndex, 
                                                    binFrame[] frameCollector, 
                                                    string framesPath,
                                                    string rawFileName,
                                                    int FirstScan)
        {

            Comb.mzI[] spectrum = null;
            
            //locate scan in the stack index (search the frame)
            bool frameFound = false;
            int frametopeak = 0;
            for (int i = stackIndex.GetLowerBound(0); i <= stackIndex.GetUpperBound(0); i++)
            {
                if (stackIndex[i].rawFileName.Trim() == rawFileName.Trim())
                {
                    for (int j = stackIndex[i].scan.GetLowerBound(0); j <= stackIndex[i].scan.GetUpperBound(0); j++)
                    {
                        if (stackIndex[i].scan[j].FirstScan == FirstScan)
                        {
                            frameFound = true;
                            frametopeak = stackIndex[i].scan[j].frame;
                            break;
                        }
                    }
                    break;
                }
            }

            if (!frameFound)
            {
                return null;
            }

            //check if the frame is in collector, if not, load it.
            bool frameincollector = checkFrameCollector(frametopeak, frameCollector);
            if (!frameincollector)
            {
                for (int i = frameCollector.GetLowerBound(0); i <= frameCollector.GetUpperBound(0); i++)
                {
                    if (frameCollector[i]== null)
                    {
                        frameCollector[i] = loadFrame(frametopeak, framesPath);
                        break;
                    }
                }
            }


            //locate spectrum at the frame
            bool spectrumLocated = false;
            for (int i = frameCollector.GetLowerBound(0); i <= frameCollector.GetUpperBound(0); i++)
            {
                if (frameCollector[i].frame == frametopeak)
                {
                    for (int j = frameCollector[i].scan.GetLowerBound(0); j <= frameCollector[i].scan.GetUpperBound(0); j++)
                    {
                        if (frameCollector[i].scan[j].rawFileName == rawFileName && frameCollector[i].scan[j].scanNumber == FirstScan)
                        {
                            spectrum = (Comb.mzI[])frameCollector[i].scan[j].spectrum.Clone();
                            spectrumLocated = true;
                            break;
                        }
                    }
                    break;
                }
            }

            if (!spectrumLocated) 
            {
                return null;
            }

            return spectrum;

        }


        public static int countFrames(string fileXml, int scansbyframe)
        {
            int iMatches = countMatches(fileXml);
            double dFrames = (double)iMatches / (double)scansbyframe;
            int iFrames = (int)Math.Ceiling(dFrames);

            return iFrames;
        }
        public static int countMatches(string fileXml)
        {
            //Initialize necessary objets for XML reading
            XmlTextReader reader = new XmlTextReader(fileXml);
            XmlNodeType nType = reader.NodeType;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(reader);

            //Count peptide_match
            XmlNodeList xmlnodeMatch = xmldoc.GetElementsByTagName("peptide_match");

            int numMatches = xmlnodeMatch.Count;

            reader.Close();

            return numMatches;

        }
        public static LookupCollection countRawFiles(string fileXml,out int nRawfiles)
        {

            //Initialize necessary objets for XML reading
            XmlTextReader reader = new XmlTextReader(fileXml);
            XmlNodeType nType = reader.NodeType;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(reader);

            //Initialize the AminoacidList[] tAaList
            XmlNodeList xmlnodeMatch = xmldoc.GetElementsByTagName("peptide_match");

            //Need the Rawfile names
            LookupCollection rawFilesColl = new LookupCollection();

            string rawFileName;
            nRawfiles = 0;
            foreach (XmlNode node in xmlnodeMatch)
            {
                foreach (XmlNode chNode in node.ChildNodes)
                {
                    if (chNode.Name == "RAWFileName")
                    {
                        rawFileName = chNode.InnerText.ToString().Trim();

                        if (!rawFilesColl.Contains(rawFileName))
                        {
                            int initScansNum = 1;
                            rawFilesColl.Add(rawFileName, initScansNum);
                        }
                        else
                        {
                            int oneMore = (int)rawFilesColl[rawFileName] + 1;
                            rawFilesColl[rawFileName] = oneMore;
                        }

                    }
                }
            }
            nRawfiles = rawFilesColl.Count;

            return rawFilesColl;

        }
        private static LookupCollection RawFilesinFrame(binFrame frame, out int nRawfiles)
        {
            //Need the Rawfile names
            LookupCollection rawFilesColl = new LookupCollection();

            string rawFileName;
            nRawfiles = 0;
            for(int i=frame.scan.GetLowerBound(0);i<=frame.scan.GetUpperBound(0);i++)
            {
                if (frame.scan[i].rawFileName != null)
                {
                    rawFileName = frame.scan[i].rawFileName.Trim();
                    if (!rawFilesColl.Contains(rawFileName))
                    {
                        int initScansNum = 1;
                        rawFilesColl.Add(rawFileName, initScansNum);
                    }
                    else
                    {
                        int oneMore = (int)rawFilesColl[rawFileName] + 1;
                        rawFilesColl[rawFileName] = oneMore;
                    }
                }
            }

            nRawfiles = rawFilesColl.Count;

            return rawFilesColl;

        }


        #region general methods of binStack class
        /// <summary>
        /// construct a new list given the capacity
        /// </summary>
        /// <param name="capacity">(int)number of scans in raw</param>
        public binStack(int capacity)
        {
            //allocate memory for components' list
            scan = new scanIndexStrt[capacity];

            //start, end and size ar 0 (list is empty)
            start = end = theSize = 0;             
        }
        /// <summary>
        /// check wether this list is empty
        /// </summary>
        /// <returns>(bool)true if the list is empty</returns>
        public bool isEmpty()
        {
            return theSize == 0;
        }
        /// <summary>
        /// check wether this list is full
        /// </summary>
        /// <returns>(bool)true if the list is full</returns>
        public bool isFull() 
        {
            return theSize >= scan.Length;
        }
        /// <summary>
        /// get the size of this list
        /// </summary>
        /// <returns>(int)size of list</returns>
        public int size() 
        {
            return theSize;
        }
        /// <summary>
        /// insert a new element in the chemical composition
        /// </summary>
        /// <param name="newComp">(Comb.compStrt)Element + Number of atoms</param>
        public void insert(scanIndexStrt newScan)
        {

            // if insert won't overflow list
            if (theSize < scan.Length)
            {

                // increment start and set element
                scan[start = (start + 1) % scan.Length] = newScan;

                // increment list size (we've added an element)
                theSize++;
            }
 
        }
        /// <summary>
        /// peek at an element in the list 
        /// </summary>
        /// <param name="offset">(int)array index to point</param>
        /// <returns>(Comb.compStrt)Element + Number of atoms</returns>
        public scanIndexStrt peek(int offset)
        {
            scanIndexStrt ret = new scanIndexStrt();

            // is someone trying to peek beyond our size?
            if (offset >= theSize)
                return ret;

            // get object we're peeking at (do not remove it)
            return scan[(end + offset + 1) % scan.Length];
        }
        #endregion



    }
}
