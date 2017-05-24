using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace ProteomicUtilities
{
    class Modification
    {
        public double deltaMass;
        public char symbol;
        public char aminoacid;
        public string name;
        public modType modificationType;
    }

    class ModificationUtils
    {
        public string getCounterSequence(string _sequence, ArrayList _modifications)
        {
            double dummyWeight = 0;
            int dummyCharge = 1;
            return getCounterSequence(_sequence, ref dummyWeight, dummyCharge, _modifications);
        }

        public string getCounterSequence(string _sequence,
                                            ref double _weight,
                                            int _charge,
                                            ArrayList _modifications)
        {
            string resultingSequence;

            string firstPart = "";
            string lastPart = "";
            string centralPart = "";

            string[] splitSequence = _sequence.Split('.');
            switch (splitSequence.Length)
            {
                case 1:
                    firstPart = "";
                    centralPart = splitSequence[0];
                    lastPart = "";
                    break;

                case 2:
                    if (splitSequence[0].Length < 2 && splitSequence[1].Length > 1)
                    {
                        firstPart = splitSequence[0] + ".";
                        centralPart = splitSequence[1];
                        lastPart = "";
                    }
                    else if (splitSequence[0].Length > 1 && splitSequence[1].Length < 2)
                    {
                        firstPart = "";
                        centralPart = splitSequence[0];
                        lastPart = "." + splitSequence[1];
                    }
                    else
                    {
                        firstPart = "";
                        centralPart = "**ERROR_CHECKTHIS**"; //***
                        lastPart = "";
                    }
                    break;

                case 3:
                    firstPart = splitSequence[0] + ".";
                    centralPart = splitSequence[1];
                    lastPart = "." + splitSequence[2];
                    break;
            }

            if (get18OType(_sequence, _modifications) == ion18OType.light)
            {
                foreach (Modification mod in _modifications)
                {
                    if (centralPart.EndsWith(mod.aminoacid.ToString()))
                    {
                        centralPart += mod.symbol.ToString();
                        _weight += mod.deltaMass / _charge;
                        break;
                    }
                }
            }
            else
            {
                centralPart = centralPart.Remove(centralPart.Length - 1);

                foreach (Modification mod in _modifications)
                {
                    if (centralPart.EndsWith(mod.aminoacid.ToString()))
                    {
                        _weight -= mod.deltaMass / _charge;
                        break;
                    }
                }
            }

            resultingSequence = firstPart + centralPart + lastPart;

            return resultingSequence;
        }

        public ion18OType get18OType(string _sequence, ArrayList _modifications)
        {
            ion18OType ionType = ion18OType.light;
            _sequence = getCentralSequence(_sequence);

            foreach (Modification mod in _modifications)
            {
                if (mod.modificationType == modType.O18)
                {
                    string heavyTail = mod.aminoacid.ToString() + mod.symbol.ToString();
                    if (_sequence.EndsWith(heavyTail))
                    {
                        ionType = ion18OType.heavy;
                        break;
                    }
                }
            }

            return ionType;
        }

        private string getCentralSequence(string _sequence)
        {
            if (_sequence.Contains("."))
                _sequence = _sequence.Split('.')[1];

            return _sequence;
        }
    }

    public enum modType
    {
        unknown,
        O18,
        other,
    }

    public enum ion18OType
    {
        unknown,
        light,
        heavy
    }
}