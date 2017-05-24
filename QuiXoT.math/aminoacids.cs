using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

namespace QuiXoT.math
{

    public struct aminoacidStrt
    {
        private string code1Val;
        private string equivalentVal;
        private Comb.compStrt[] compositionVal;

        public aminoacidStrt(string code1Value,string equivalentValue,Comb.compStrt[] compositionValue)
        {
            code1Val = code1Value;
            equivalentVal = equivalentValue;
            compositionVal = compositionValue; 
        }

        public string code1 
        {
            get 
            {
                return code1Val;
            }
            set 
            {
                code1Val = value;
            }
        }

        public string equivalent
        {
            get 
            {
                return equivalentVal;
            }
            set 
            {
                equivalentVal = value;
            }
        }

        public Comb.compStrt[] composition
        {
            get 
            {
                return compositionVal;
            }
            set 
            {
                compositionVal = value;
            }
        }
    }
    
    public class AminoacidList 
    {
       
        
        //declare the class properties
        public string name;
        public string code1;
        public string code3;
        public string equivalent;
        protected Comb.compStrt[] composition;
        protected int start, end, theSize;

        /// <summary>
        /// construct a new list given the capacity
        /// </summary>
        /// <param name="capacity">(int)total number of elements of the chemical formula</param>
        public AminoacidList(int capacity)
        {
            //allocate memory for components' list
            composition = new Comb.compStrt[capacity];

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
            return theSize >= composition.Length;
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
        public void insert(Comb.compStrt newComp)
        {

            // if insert won't overflow list
            if (theSize < composition.Length)
            {

                // increment start and set element
                composition[start = (start + 1) % composition.Length] = newComp;

                // increment list size (we've added an element)
                theSize++;
            }
 
        }
                
        /// <summary>
        /// peek at an element in the list 
        /// </summary>
        /// <param name="offset">(int)array index to point</param>
        /// <returns>(Comb.compStrt)Element + Number of atoms</returns>
        public Comb.compStrt peek(int offset)
        {
            Comb.compStrt ret=new Comb.compStrt("",0);

            // is someone trying to peek beyond our size?
            if (offset >= theSize)
                return ret;

            // get object we're peeking at (do not remove it)
            return composition[(end + offset + 1) % composition.Length];
        }
        
        
        
        
        /// <summary>
        /// Reads a XML file with the aminoacids' list 
        /// </summary>
        /// <param name="fileXml">XML with the aminoacids' list</param>
        /// <returns>(AminoacidList[])</returns>
        public static AminoacidList[] readXML(string fileXml)
        {

            //Initialize necessary objets for XML reading
            XmlTextReader reader = new XmlTextReader(fileXml);
            XmlNodeType nType = reader.NodeType;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(reader);

            //Initialize the AminoacidList[] tAaList
            XmlNodeList xmlnodeAminoacid = xmldoc.GetElementsByTagName("aminoacid");
            AminoacidList[] tAaList = new AminoacidList[xmlnodeAminoacid.Count];

            XmlNodeList xmlnodeElement = xmldoc.GetElementsByTagName("Element");


            //for each <aminoacid> entry
            for (int i = 0; i< xmlnodeAminoacid.Count; i++)
            {
                
                //for each child node of the <aminoacid> entry
                for (int j = 0; j<xmlnodeAminoacid[i].ChildNodes.Count; j++) 
                {
                    string sNode=xmlnodeAminoacid[i].ChildNodes[j].Name.ToString();
                   
                    
                    
                    if (sNode == "Formula")
                    {
                        int nElements = 0;
                        //Count in <Formula> entry the Element values.
                        for (int k = 0; k < xmlnodeAminoacid[i].ChildNodes[j].ChildNodes.Count; k++) 
                        {                           
                            string sElement = xmlnodeAminoacid[i].ChildNodes[j].ChildNodes[k].Name.ToString();
                            if (sElement == "Element")
                            {
                                nElements++;
                            }
                        }
                        //Initialize the class tAaList[i] with the correct number of elements
                        tAaList[i] = new AminoacidList(nElements);

                       
                        //Search in <Formula> entry for the Element values.
                        for (int k = 0; k < xmlnodeAminoacid[i].ChildNodes[j].ChildNodes.Count; k++)
                        {
                            string sElement = xmlnodeAminoacid[i].ChildNodes[j].ChildNodes[k].Name.ToString();
                            if (sElement == "Element")
                            {
                                string sElem= xmlnodeAminoacid[i].ChildNodes[j].ChildNodes[k].Attributes["id"].Value.ToString();
                                //Search for the number of atoms
                                int iNatoms=0;
                                for(int l=0;l<xmlnodeAminoacid[i].ChildNodes[j].ChildNodes[k].ChildNodes.Count;l++)
                                {
                                    string sAtoms = xmlnodeAminoacid[i].ChildNodes[j].ChildNodes[k].ChildNodes[l].Name.ToString();
                                    if (sAtoms=="Natoms") 
                                    { 
                                        iNatoms=int.Parse(xmlnodeAminoacid[i].ChildNodes[j].ChildNodes[k].ChildNodes[l].InnerText.ToString(),System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                }
                                Comb.compStrt tElement=new Comb.compStrt(sElem,iNatoms);
                                tAaList[i].insert(tElement);


                            }
                        }

                    }
                }


                //Once you have initialized correctly the class, you can search for the <Name> tag, and so on
                for (int j = 0; j < xmlnodeAminoacid[i].ChildNodes.Count; j++)
                {
                    if (xmlnodeAminoacid[i].ChildNodes[j].Name.ToString() == "Name") 
                    {
                        tAaList[i].name = xmlnodeAminoacid[i].ChildNodes[j].InnerText.ToString();
                    }
                    if (xmlnodeAminoacid[i].ChildNodes[j].Name.ToString() == "Code1")
                    {
                        tAaList[i].code1 = xmlnodeAminoacid[i].ChildNodes[j].InnerText.ToString();
                    }
                    if (xmlnodeAminoacid[i].ChildNodes[j].Name.ToString() == "Code3")
                    {
                        tAaList[i].code3 = xmlnodeAminoacid[i].ChildNodes[j].InnerText.ToString();
                    }
                    if (xmlnodeAminoacid[i].ChildNodes[j].Name.ToString() == "equivalent")
                    {
                        tAaList[i].equivalent = xmlnodeAminoacid[i].ChildNodes[j].InnerText.ToString();
                    }
                  
                }
           
            }
                        
            return tAaList;
            
        }

        /// <summary>
        /// Get the equivalent sequence, without any modification
        /// </summary>
        /// <param name="sSeq">sequence with (possible) modifications</param>
        /// <param name="aaList">aminoacid list</param>
        /// <returns>clean sequence</returns>
        public static string getEquivalent(string sSeq, AminoacidList[] aaList)
        {
            sSeq = sSeq.ToUpper();

            ArrayList modifList = new ArrayList();

            for (int i = aaList.GetLowerBound(0); i <= aaList.GetUpperBound(0); i++)
            {
                if (aaList[i].equivalent != null)
                {
                    aminoacidStrt amod = new aminoacidStrt(aaList[i].code1, aaList[i].equivalent, null);
                    modifList.Add(amod);
                }
            }
            

            foreach(aminoacidStrt aa in modifList)
            {
                if(sSeq.Contains(aa.code1))
                {
                   sSeq = sSeq.Replace(aa.code1, aa.equivalent);   
                }
            }

            return sSeq;

        }

        /// <summary>
        /// Given a sequence, it calculates its chemical composition.
        /// </summary>
        /// <param name="sSequence">(string)sequence</param>
        /// <param name="aaList">(AminoacidList[])list of aminoacids</param>
        /// <returns>(Comb.compStrt[])Chemical composition of the sequence</returns>
        public static Comb.compStrt[] calComposition(string sSeq, AminoacidList[] aaList)
        {

            sSeq = sSeq.ToUpper();

            //Adds the fixed C- and N-terminus modifications
            //C- and N-terminus fixed modifications
            string ntr = "ntr";
            string ctr = "ctr";
            string ntrCode1 = ".";
            string ctrCode1 = ";";

            foreach (AminoacidList aa in aaList)
            {
                if (aa.code3 == ntr && aa.code1.Trim() == "") //fixed N-terminus modification
                {
                    aa.code1 = ntrCode1;
                    sSeq += ntrCode1;
                }
                if (aa.code3 == ctr && aa.code1.Trim() == "") //fixed N-terminus modification
                {
                    aa.code1 = ctrCode1;
                    sSeq += ctrCode1;
                }
            }
   

            Comb.compStrt[] comp = new Comb.compStrt[30];
            
            
            char[] cSeq = new char[sSeq.Length];
            string[] parsedSeq = new string[sSeq.Length+1];

            cSeq= sSeq.ToCharArray();

            int iLength = cSeq.GetUpperBound(0);
            int iAaListLength= aaList.GetUpperBound(0);
            
         
            //Count the number of aminoacids in the sequence
            int iNaaSeq=0; //number of aminoacids founded in the sequence
            int iElemComp=0; //number of elements founded in the composition
            bool compoundaaFounded=false; 
            for (int i = 0; i <= iLength; i++) 
            {
                compoundaaFounded = false;
                string sStr = "";
                if (i < iLength)
                {
                    sStr = cSeq[i].ToString() + cSeq[i+1].ToString();
                }
                else
                {
                    sStr = cSeq[i].ToString(); 
                }

                for (int j = 0; j <= iAaListLength; j++) 
                {
                    if (sStr == aaList[j].code1 && sStr.Length > 1)
                    {
                        foreach(Comb.compStrt c in aaList[j].composition)
                        {
                            int ielAdd = elementAdded(c.Elem, comp);
                            if ( ielAdd <= comp.GetUpperBound(0))
                            {
                                comp[ielAdd].Nats += c.Nats;
                            }
                            else
                            {
                                comp[iElemComp].Elem = c.Elem;
                                comp[iElemComp].Nats = c.Nats;
                                iElemComp++;
                            }
                        }                        
                        iNaaSeq++;
                        parsedSeq[iNaaSeq] += aaList[j].code1;
                        compoundaaFounded = true;
                    }
                }
                if(!compoundaaFounded)
                {
                        string sStr1 = "";
 
                        if (sStr.Length > 1)
                        {
                            sStr1 = sStr.Remove(1);
                        }
                        else 
                        {
                            sStr1 = sStr;
                        }
                        for (int j = 0; j <= iAaListLength; j++)
                        {
                            if (sStr1 == aaList[j].code1)
                            {
                                foreach (Comb.compStrt c in aaList[j].composition)
                                {
                                    int ielAdd = elementAdded(c.Elem, comp);
                                    if (ielAdd <= comp.GetUpperBound(0))
                                    {
                                        comp[ielAdd].Nats += c.Nats;
                                    }
                                    else
                                    {
                                        comp[iElemComp].Elem = c.Elem;
                                        comp[iElemComp].Nats = c.Nats;
                                        iElemComp++;
                                    }
                                }
                                if (aaList[j].equivalent != "")
                                {
                                    iNaaSeq++;
                                }
                                parsedSeq[iNaaSeq] = aaList[j].code1;
                            }
                        }
                    }
                
            }



            
            //Reduce the array to the not null items.
            int iNotNulls=0;
            for (int i=0;i<comp.GetUpperBound(0);i++)
            {
                if (comp[i].Elem!=null) 
                {
                    iNotNulls++;
                }
            }
            Comb.compStrt[] composition=new Comb.compStrt[iNotNulls];
            for (int i = 0; i < iNotNulls; i++) 
            {
                composition[i] = comp[i];
            }

            //composition = minusH20(composition, iNaaSeq - 1);
            composition = plusH20(composition, 1);



            //Remove special flag for N- and C-terminus
            foreach (AminoacidList aa in aaList)
            {
                if (aa.code3 == ntr && aa.code1.Trim() == ntrCode1) //fixed N-terminus modification
                {
                    aa.code1 = "";
                }
                if (aa.code3 == ctr && aa.code1.Trim() == ctrCode1) //fixed N-terminus modification
                {
                    aa.code1 = "";
                }
            }


            return composition;
        }

       
        /// <summary>
        /// Substracts n water molecules to the composition
        /// </summary>
        /// <param name="cmpH2O">(Comb.compStrt[]) Composition</param>
        /// <param name="nH20">(int) number of water molecules</param>
        /// <returns></returns>
        public static Comb.compStrt[] minusH20(Comb.compStrt[] cmpH2O, int nH20)
        {
            Comb.compStrt[] cmp = new Comb.compStrt[cmpH2O.GetUpperBound(0) + 1];

            cmpH2O.CopyTo(cmp, 0);

            int iH = elementAdded("H", cmp);
            int iO = elementAdded("O", cmp);

            cmp[iH].Nats -= 2 * nH20;
            cmp[iO].Nats -= 1 * nH20;

            return cmp;
        }


        /// <summary>
        /// Adds n water molecules to the composition
        /// </summary>
        /// <param name="cmpH2O">(Comb.compStrt[]) Composition</param>
        /// <param name="nH20">(int) number of water molecules</param>
        /// <returns></returns>
        public static Comb.compStrt[] plusH20(Comb.compStrt[] cmpH2O, int nH20)
        {
            Comb.compStrt[] cmp = new Comb.compStrt[cmpH2O.GetUpperBound(0) + 1];

            cmpH2O.CopyTo(cmp, 0);

            int iH = elementAdded("H", cmp);
            int iO = elementAdded("O", cmp);

            cmp[iH].Nats += 2 * nH20;
            cmp[iO].Nats += 1 * nH20;

            return cmp;
        }


        /// <summary>
        /// Check wether an element were added to a composition matrix.
        /// </summary>
        /// <param name="Element">(string)element to search</param>
        /// <param name="composition">(Comb.compStrt[])Composition to check</param>
        /// <returns>(int)array position of the element. Returns ( Composition.GetUpperBound + 1 ) if it is not found.</returns>
        private static int elementAdded(string Element, Comb.compStrt[] composition) 
        {
            int iFound = composition.GetUpperBound(0)+1;  //returns this if it is not found

            int iLength = composition.GetUpperBound(0);
            for (int i = 0; i <= iLength; i++) 
            {
                if (Element == composition[i].Elem)
                {
                    iFound = i;
                }
            }
            return iFound;
        }

    }
    
 
}
