using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace KeystrokeCounter.Collections
{
    public partial class ObservableConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private ConcurrentDictionary<TKey, TValue> _data = new ConcurrentDictionary<TKey, TValue>();
        private List<TKey> _strokes = new List<TKey>();

        public TValue this[TKey key]
        {
            get => _data[key];
            set => AddOrUpdate(key, value, (k, v) => value);
        }

        public ICollection<TKey> Keys => _data.Keys;

        public ICollection<TValue> Values => _data.Values;

        public int Count => _data.Count;

        public bool IsReadOnly => false;

        public KeyValuePair<TKey, TValue> this[int index]
        {
            get
            {
                var stroke = _strokes[index];
                return new KeyValuePair<TKey, TValue>(stroke, _data[stroke]);
            }
            set => throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            if (_data.TryAdd(key, value))
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (_data.TryAdd(item.Key, item.Value))
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
        }

        public void Clear()
        {
            _data.Clear();
            _strokes.Clear();
            OnCollectionChanged();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _data.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _data.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (_data as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _data.OrderBy(item => _strokes.IndexOf(item.Key)).GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            if (_data.TryRemove(key, out var value))
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_data.TryRemove(item))
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return _data.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.OrderBy(item => _strokes.IndexOf(item.Key)).GetEnumerator();
        }

        public TValue AddOrUpdate<TArg>(
            TKey key, Func<TKey, TArg, TValue> addValueFactory, Func<TKey, TValue, TArg, TValue> updateValueFactory, TArg factoryArgument)
        {
            KeyValuePair<TKey, TValue>? oldItem = null;
            var result = _data.AddOrUpdate(key, addValueFactory, (k, v, a) =>
            {
                var value = updateValueFactory(k, v, a);
                oldItem = new KeyValuePair<TKey, TValue>(k, v);
                return value;
            }, factoryArgument);
            var newItem = new KeyValuePair<TKey, TValue>(key, result);
            if (oldItem != null)
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, newItem,
                    oldItem ?? throw new ArgumentNullException("Old item is null, this state should be impossible, as it was checked for. Someone did mess up here."));
            else
                OnCollectionChanged(NotifyCollectionChangedAction.Add, newItem);
            return result;
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            KeyValuePair<TKey, TValue>? oldItem = null;
            var result = _data.AddOrUpdate(key, addValue, (k, v) =>
            {
                var value = updateValueFactory(k, v);
                oldItem = new KeyValuePair<TKey, TValue>(k, v);
                return value;
            });
            var newItem = new KeyValuePair<TKey, TValue>(key, result);
            if (oldItem != null)
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, newItem,
                    oldItem ?? throw new ArgumentNullException("Old item is null, this state should be impossible, as it was checked for. Someone did mess up here."));
            else
                OnCollectionChanged(NotifyCollectionChangedAction.Add, newItem);
            return result;
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            KeyValuePair<TKey, TValue>? oldItem = null;
            var result = _data.AddOrUpdate(key, addValueFactory, (k, v) =>
            {
                var value = updateValueFactory(k, v);
                oldItem = new KeyValuePair<TKey, TValue>(k, v);
                return value;
            });
            var newItem = new KeyValuePair<TKey, TValue>(key, result);
            if (oldItem != null)
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, newItem,
                    oldItem ?? throw new ArgumentNullException("Old item is null, this state should be impossible, as it was checked for. Someone did mess up here."));
            else
                OnCollectionChanged(NotifyCollectionChangedAction.Add, newItem);
            return result;
        }

    }
}
