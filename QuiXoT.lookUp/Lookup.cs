using System;
using System.Collections;
using System.Text;

namespace QuiXoT.lookUp
{
    [Serializable]
    class Lookup : IComparable
    {
        private object mKey;
        private object mValue;

        //---------------------------------------------------------------------
        // Default Constructor
        //---------------------------------------------------------------------
        public Lookup()
            : this(null, null)
        {
        }

        //---------------------------------------------------------------------
        // Overloaded Constructor
        //---------------------------------------------------------------------
        public Lookup(object key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        //---------------------------------------------------------------------
        // CompareTo
        //---------------------------------------------------------------------
        public int CompareTo(object obj)
        {
            int result = 0;

            if (obj is Lookup)
            {
                result =
                 ((IComparable)this.Value).CompareTo((IComparable)
                                            (((Lookup)obj).Value));
            }

            return result;
        }

        //---------------------------------------------------------------------
        // ToDictionaryEntry
        //---------------------------------------------------------------------
        public DictionaryEntry ToDictionaryEntry()
        {
            return new DictionaryEntry(this.Key, this.Value);
        }

        //=====================================================================
        // PROPERTIES
        //=====================================================================
        public object Key
        {
            get
            {
                return this.mKey;
            }
            set
            {
                if (this.mKey != value)
                {
                    this.mKey = value;
                }
            }
        }

        public object Value
        {
            get
            {
                return this.mValue;
            }
            set
            {
                if (this.mValue != value)
                {
                    this.mValue = value;
                }
            }
        }
    }
}
