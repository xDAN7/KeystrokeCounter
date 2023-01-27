using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeystrokeCounter.Collections
{
    public partial class ObservableConcurrentDictionary<TKey, TValue> : INotifyPropertyChanged, INotifyCollectionChanged
    {

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged()
        {
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        private void OnCollectionChanged()
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            if (action == NotifyCollectionChangedAction.Add)
                _strokes.Add(changedItem.Key);
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem, _strokes.IndexOf(changedItem.Key)));
            if (action == NotifyCollectionChangedAction.Remove)
                _strokes.Remove(changedItem.Key);
        }


        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, _strokes.IndexOf(newItem.Key)));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList<KeyValuePair<TKey, TValue>> newItems)
        {
            int startIndex = _strokes.Count;
            for (int i = 0; i < newItems.Count; i++)
                _strokes.Add(newItems[i].Key);
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems, startIndex));
        }

    }
}
