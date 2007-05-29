using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ScriptCoreLib.JavaScript.BCLImplementation.System.Collections.Generic
{
    using ScriptCoreLib.JavaScript.Runtime;

    [Script(Implements = typeof(global::System.Collections.Generic.Dictionary<,>))]
    internal class __Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable
    {
        readonly global::System.Collections.Generic.List<TKey> _keys = new global::System.Collections.Generic.List<TKey>();
        readonly global::System.Collections.Generic.List<TValue> _values = new global::System.Collections.Generic.List<TValue>();

        //Expando list = new Expando();

        public __Dictionary()
        {

        }

        public __Dictionary(IEqualityComparer<TKey> comparer)
        {

        }

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            //if (list.Contains(key))
            if (this.ContainsKey(key))
                throw new global::System.Exception("Argument_AddingDuplicate");


            _keys.Add(key);
            _values.Add(value);
        }



        public bool ContainsKey(TKey key)
        {
            return _keys.Contains(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {

                // global::System.Collections.Generic.List<TKey> a = new  global::System.Collections.Generic.List<TKey>();

                //foreach (var v in list.GetMemberNames())
                //{
                //    a.Add((TKey)v);
                //} 

                return _keys;
            }
        }

        public bool Remove(TKey key)
        {
            if (!ContainsKey(key))
                return false;

            var i = _keys.IndexOf(key);

            _keys.RemoveAt(i);
            _values.RemoveAt(i);

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new global::System.Exception("The method or operation is not implemented.");
        }

        public ICollection<TValue> Values
        {
            get
            {
                return this.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var i = _keys.IndexOf(key);

                if (i == -1)
                    throw new Exception("Not found.");

                return _values[i];


            }
            set
            {
                var i = _keys.IndexOf(key);

                if (i == -1)
                {
                    _keys.Add(key);
                    _values.Add(value);
                }
                else
                {
                    _values[i] = value;
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _keys.Clear();
            _values.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new global::System.Exception("The method or operation is not implemented.");
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new global::System.Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { return _keys.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new global::System.Exception("The method or operation is not implemented."); }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new global::System.Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members


        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public __Dictionary<TKey, TValue>.__Enumerator GetEnumerator()
        {
            return new __Enumerator(this);
        }

        [Script(ImplementationType = typeof(global::System.Collections.Generic.Dictionary<,>.Enumerator))]
        public class __Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
        {
            IEnumerator<KeyValuePair<TKey, TValue>> list;

            public __Enumerator(__Dictionary<TKey, TValue> e)
            {
                global::System.Collections.Generic.List<KeyValuePair<TKey, TValue>> a = new global::System.Collections.Generic.List<KeyValuePair<TKey, TValue>>();

                foreach (var v in e.Keys)
                {
                    a.Add(new KeyValuePair<TKey, TValue>(v, e[v]));
                }

                this.list = a.GetEnumerator();
            }

            public KeyValuePair<TKey, TValue> Current { get { return list.Current; } }

            public void Dispose()
            {
                list.Dispose();
            }

            public bool MoveNext()
            {
                return list.MoveNext();
            }



            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Reset()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }
    }

}
