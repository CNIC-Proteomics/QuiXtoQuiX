using System;
using System.Collections;
using System.Text;

namespace QuiXoT.lookUp
{
    class LookupEnumerator : IDictionaryEnumerator  
    {
        private int index = -1;
        private ArrayList items;

        //---------------------------------------------------------------------
        // Constructor
        //---------------------------------------------------------------------
        public LookupEnumerator(ArrayList list)
        {
            this.items = list;
        }

        //---------------------------------------------------------------------
        // MoveNext
        //---------------------------------------------------------------------
        public bool MoveNext()
        {
            this.index++;
            if (index >= this.items.Count)
                return false;

            return true;
        }

        //=====================================================================
        // PROPERTIES
        //=====================================================================
        //---------------------------------------------------------------------
        // Reset
        //---------------------------------------------------------------------
        public void Reset()
        {
            this.index = -1;
        }

        //---------------------------------------------------------------------
        // Current
        //---------------------------------------------------------------------
        public object Current
        {
            get
            {
                if (this.index < 0 || index >= this.items.Count)
                    throw new InvalidOperationException();

                return this.items[index];
            }
        }

        //---------------------------------------------------------------------
        // Entry
        //---------------------------------------------------------------------
        public DictionaryEntry Entry
        {
            get
            {
                return ((Lookup)this.Current).ToDictionaryEntry();
            }
        }

        //---------------------------------------------------------------------
        // Key
        //---------------------------------------------------------------------
        public object Key
        {
            get
            {
                return this.Entry.Key;
            }
        }

        //---------------------------------------------------------------------
        // Value
        //---------------------------------------------------------------------
        public object Value
        {
            get
            {
                return this.Entry.Value;
            }
        }
    }
}
