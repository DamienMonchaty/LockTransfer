using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LockSelf.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void SetValue<T>(ref T backingField, T value,  string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value)) return;
            backingField = value;
            RaisePropertyChanged(propertyName);
        }
    }
}
