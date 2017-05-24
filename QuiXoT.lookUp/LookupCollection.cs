using System;
using System.Collections;
using System.Text;

namespace QuiXoT.lookUp
{
    [Serializable]
    public class LookupCollection : ICollection, IDictionary, IEnumerable
    {
        private ArrayList mItems = new ArrayList();

        //---------------------------------------------------------------------
        // Constructor
        //---------------------------------------------------------------------
        public LookupCollection()
        {
        }

        //---------------------------------------------------------------------
        // Add
        //---------------------------------------------------------------------
        public void Add(object key, object value)
        {
            // do some validation
            if (key == null)
                throw new ArgumentNullException("key is a null reference");
            else if (this.Contains(key))
                throw new
                 ArgumentException("An element with the same key already exists");

            // add the new item
            Lookup newItem = new Lookup();

            newItem.Key = key;
            newItem.Value = value;

            this.mItems.Add(newItem);
            this.mItems.Sort();
        }

        //---------------------------------------------------------------------
        // Clear
        //---------------------------------------------------------------------
        public void Clear()
        {
            this.mItems.Clear();
        }

        //---------------------------------------------------------------------
        // Contains
        //---------------------------------------------------------------------
        public bool Contains(object key)
        {
            return (this.GetByKey(key) != null);
        }

        //---------------------------------------------------------------------
        // CopyTo
        //---------------------------------------------------------------------
        public void CopyTo(Array array, int index)
        {
            this.mItems.CopyTo(array, index);
        }

        //---------------------------------------------------------------------
        // GetEnumerator (1)
        //---------------------------------------------------------------------
        public IDictionaryEnumerator GetEnumerator()
        {
            return new LookupEnumerator(this.mItems);
        }

        //---------------------------------------------------------------------
        // GetEnumerator (2)
        //---------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LookupEnumerator(this.mItems);
        }

        //---------------------------------------------------------------------
        // Remove
        //---------------------------------------------------------------------
        public void Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key is a null reference");

            Lookup deleteItem = this.GetByKey(key);
            if (deleteItem != null)
            {
                this.mItems.Remove(deleteItem);
                this.mItems.Sort();
            }
        }

        //=====================================================================
        // PRIVATE
        //=====================================================================
        private Lookup GetByKey(object key)
        {
            Lookup result = null;
            int keyIndex = -1;
            ArrayList keys = (ArrayList)this.Keys;

            if (this.mItems.Count > 0)
            {
                keyIndex = keys.IndexOf(key);

                if (keyIndex >= 0)
                {
                    result = (Lookup)this.mItems[keyIndex];
                }
            }

            return result;
        }

        //=====================================================================
        // PROPERTIES
        //=====================================================================
        public int Count
        {
            get
            {
                return this.mItems.Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object this[object key]
        {
            get
            {

                if (key == null)
                    throw new ArgumentNullException("key is a null reference");

                object result = null;

                Lookup findItem = this.GetByKey(key);
                if (findItem != null)
                {
                    result = findItem.Value;
                }

                return result;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key is a null reference");

                this.GetByKey(key).Value =value;

            }
        }

        public ICollection Keys
        {
            get
            {
                ArrayList result = new ArrayList();

                this.mItems.Sort();

                foreach (Lookup curItem in this.mItems)
                {
                    result.Add(curItem.Key);
                }

                return result;
            }
        }

        public ICollection Values
        {
            get
            {
                ArrayList result = new ArrayList();

                foreach (Lookup curItem in this.mItems)
                {
                    result.Add(curItem.Value);
                }

                return result;
            }
        }
 
    }
}
