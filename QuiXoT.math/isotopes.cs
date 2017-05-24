using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace QuiXoT.math
{
    /// <summary>
    /// data structure of an isotope.
    /// </summary>
    public struct isotList
    {
        private Double DMassVal;
        private Double MassVal;
        private Double fVal;
        private string ElemVal;


        public isotList(double DMassValue, double MassValue, double fValue, string ElemValue)
        {
            DMassVal = DMassValue;
            MassVal = MassValue;
            fVal = fValue;
            ElemVal = ElemValue;

        }

        public double DMass
        {
            get
            {
                return DMassVal;
            }
            set
            {
                DMassVal = value;
            }
        }

        public double Mass
        {
            get 
            {
                return MassVal;
            }
            set 
            {
                MassVal = value;
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

        public override string ToString()
        {
            return (String.Format("{0}, {1}, {2}, {3}", ElemVal,MassVal ,DMassVal, fVal));
        }


    }

    public class Isotopes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileXml">XML file that contains the isotopic information</param>
        /// <returns>(isotlist[i][j])Matrix with the isotopic information of i elements, each one with their j isotopes.</returns>
        public static isotList[][] readXML(string fileXml) 
        {
            XmlTextReader reader = new XmlTextReader(fileXml);
            XmlNodeType nType = reader.NodeType;
            XmlDocument xmldoc = new XmlDocument();

            xmldoc.Load(reader);

            XmlNodeList xmlnodeElement = xmldoc.GetElementsByTagName("Element");
            XmlNodeList xmlnodeIsotope = xmldoc.GetElementsByTagName("Isotope");
            string lastElement = "";
            string actElement = "";
            short numIsotopes = 1;
            short numElement = 0;

            isotList[][] isotope = new isotList[xmlnodeElement.Count][];

            for (int i = 1; i < xmlnodeIsotope.Count; i++)
            {
                XmlAttributeCollection xmlattrc = xmlnodeIsotope[i].Attributes;

                lastElement = xmlnodeIsotope[i - 1].ParentNode.Attributes["id"].Value.ToString();
                actElement = xmlnodeIsotope[i].ParentNode.Attributes["id"].Value.ToString();

                if (lastElement == actElement)
                {
                    numIsotopes++;
                }
                else
                {
                    isotope[numElement] = new isotList[numIsotopes];
                    for (int j = 0; j < numIsotopes; j++)
                    {
                        isotope[numElement][j].Elem = lastElement;
                    }
                    numElement++;
                    numIsotopes = 1;
                }

                lastElement = xmlnodeIsotope[i].ParentNode.Attributes["id"].Value.ToString();

            }


            //dim the last Element of the array
            isotope[numElement] = new isotList[numIsotopes];
            for (int j = 0; j < numIsotopes; j++)
            {
                isotope[numElement][j].Elem = lastElement;
            }


            //fill the first element-isotope of the array isotopes[element][isotope]
            double firstMass = 0;
            for (int k = 0; k < xmlnodeIsotope[0].ChildNodes.Count; k++)
            {
                double actMass = 0;
                double actf = 0;

                if (xmlnodeIsotope[0].ChildNodes[k].Name == "Mass")
                {
                    actMass = double.Parse(xmlnodeIsotope[0].ChildNodes[k].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                    firstMass = actMass;
                    isotope[0][0].Mass = firstMass;
                    isotope[0][0].DMass = actMass - firstMass;
                }
                if (xmlnodeIsotope[0].ChildNodes[k].Name == "Abundance")
                {
                    actf = double.Parse(xmlnodeIsotope[0].ChildNodes[k].InnerText, System.Globalization.CultureInfo.InvariantCulture);

                }
                isotope[0][0].f = actf;
            }



            //fill the array isotopes[element][isotope]
            numElement = 0;
            int actIsotope = 1;
            for (int nodeIsot = 1; nodeIsot < xmlnodeIsotope.Count; nodeIsot++)
            {

                double actMass = 0;
                double actf = 0;

                lastElement = xmlnodeIsotope[nodeIsot - 1].ParentNode.Attributes["id"].Value.ToString();
                actElement = xmlnodeIsotope[nodeIsot].ParentNode.Attributes["id"].Value.ToString();

                if (lastElement != actElement)
                {
                    numElement++;
                    actIsotope = 0;
                }

                for (int k = 0; k < xmlnodeIsotope[nodeIsot].ChildNodes.Count; k++)
                {

                    if (xmlnodeIsotope[nodeIsot].ChildNodes[k].Name == "Mass")
                    {

                        actMass = double.Parse(xmlnodeIsotope[nodeIsot].ChildNodes[k].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        if (actIsotope == 0)
                        {
                            firstMass = actMass;
                        }
                    }
                    if (xmlnodeIsotope[nodeIsot].ChildNodes[k].Name == "Abundance")
                    {
                        actf = double.Parse(xmlnodeIsotope[nodeIsot].ChildNodes[k].InnerText, System.Globalization.CultureInfo.InvariantCulture);

                    }
                }

                isotope[numElement][actIsotope].f = actf;
                isotope[numElement][actIsotope].DMass = actMass - firstMass;
                isotope[numElement][actIsotope].Mass = actMass;

                actIsotope++;
            }

            return isotope;
        }
        
    }
}
