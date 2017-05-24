using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using QuiXoT.lookUp;

//using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using QuiXoT.math;


namespace QuiXoT.DA_Raw
{
    [Serializable]
    public struct scanStrt
    {
        private int scanNumberVal;
        private string rawFileNameVal;
        private Comb.mzI[] spectrumVal;
        private double parentMassVal;

        public scanStrt(int scanNumberValue, string rawFileNameValue ,Comb.mzI[] spectrumValue)
        {
            scanNumberVal = scanNumberValue;
            rawFileNameVal = rawFileNameValue;
            spectrumVal = spectrumValue;
            parentMassVal = 0;
        }
        public scanStrt(int scanNumberValue)
        {
            scanNumberVal = scanNumberValue;
            rawFileNameVal = null;
            spectrumVal = null;
            parentMassVal = 0;
        }
        public string rawFileName
        {
            get { return rawFileNameVal; }
            set { rawFileNameVal = value; }
        }
        public int scanNumber
        {
            get { return scanNumberVal; }
            set { scanNumberVal = value; }
        }
        public Comb.mzI[] spectrum
        {

            get { return spectrumVal; }
            set { spectrumVal = value; }
        }
        public double parentMass
        {
            get { return parentMassVal; }
            set { parentMassVal = value; }
        }


    }


    [Serializable] 
    public class DA_raws
    {
        //declare the class properties
        protected int start, end, theSize;
        public string rawFile;
        public scanStrt[] scan;
        

        /// <summary>
        /// construct a new list given the capacity
        /// </summary>
        /// <param name="capacity">(int)total number of rawfiles</param>
        public DA_raws(int capacity)
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
            //return scan.Length;
            return theSize;
        }

        /// <summary>
        /// insert a new element in the chemical composition
        /// </summary>
        /// <param name="newComp">(Comb.compStrt)Element + Number of atoms</param>
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
        /// <returns>(Comb.compStrt)Element + Number of atoms</returns>
        public scanStrt peek(int offset)
        {
            scanStrt ret = new scanStrt();

            // is someone trying to peek beyond our size?
            if (offset >= theSize)
                return ret;

            // get object we're peeking at (do not remove it)
            return scan[(end + offset + 1) % scan.Length];
        }
        
        
        

        
        /// <summary>
        /// Not in use!!!
        /// </summary>
        /// <param name="fileXml"></param>
        /// <param name="rawPath"></param>
        /// <returns></returns>
        public static DA_raws[] genScansList(string fileXml, string rawPath)
        {
            /*

            //Initialize necessary objets for XML reading
            XmlTextReader reader = new XmlTextReader(fileXml);
            XmlNodeType nType = reader.NodeType;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(reader);
            
            XmlNodeList xmlnodeMatch = xmldoc.GetElementsByTagName("peptide_match");
 
            //Need the Rawfile names
            LookupCollection rawFilesColl = new LookupCollection();
            
            string rawFileName;
            int nRawfiles = 0;
            foreach (XmlNode node in xmlnodeMatch) 
            {       
                foreach(XmlNode chNode in node.ChildNodes)
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

            DA_raws[] tRawList = new DA_raws[nRawfiles];
            ArrayList rawFilesKeys = (ArrayList)rawFilesColl.Keys;
            ArrayList rawFilesValues = (ArrayList)rawFilesColl.Values;

            for (int i = 0; i < nRawfiles; i++) 
            {
               
                tRawList[i] = new DA_raws((int)rawFilesValues[i]);

                tRawList[i].rawFile = rawFilesKeys[i].ToString();

            }

            scanStrt newScan = new scanStrt();
            for (int i = 0; i <= tRawList.GetUpperBound(0); i++)
            {
                foreach (XmlNode node in xmlnodeMatch)
                {

                    if (node["RAWFileName"].InnerText.ToString().Trim() == tRawList[i].rawFile.Trim())
                    {
                        newScan.scanNumber = int.Parse(node["FirstScan"].InnerText.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        tRawList[i].insert(newScan);
                    }
                }

            }


            for (int i = 0; i < nRawfiles; i++)
            {
                int[] scansRawList = new int[tRawList[i].size()];   

                for (int j = 0; j <= scansRawList.GetUpperBound(0); j++)
                {
                    scansRawList[j] = tRawList[i].scan[j].scanNumber-1;
                }
                int t = tRawList[i].size();
                rawFileName = tRawList[i].rawFile;
                DA_raw daRaw1 = new DA_raw();
                Comb.mzI[][] scansRaw = daRaw1.ReadScanRaw(rawPath, rawFileName, scansRawList);
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

            reader.Close();
            

            return tRawList;
            
           */

            return null;

        }


    }
}
