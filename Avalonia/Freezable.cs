// -----------------------------------------------------------------------
// <copyright file="Freezable.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia
{
    using System;

    public abstract class Freezable : DependencyObject
    {
        protected Freezable()
        {
        }

        public bool CanFreeze
        {
            get { return this.FreezeCore(true); }
        }

        public bool IsFrozen
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Freezable Clone()
        {
            throw new NotImplementedException();
        }

        public Freezable CloneCurrentValue()
        {
            throw new NotImplementedException();
        }

        public void Freeze()
        {
            throw new NotImplementedException();
        }

        public Freezable GetAsFrozen()
        {
            throw new NotImplementedException();
        }

        public Freezable GetCurrentValueAsFrozen()
        {
            throw new NotImplementedException();
        }

        protected static bool Freeze(Freezable freezable, bool isChecking)
        {
            throw new NotImplementedException();
        }

        protected virtual void CloneCore(Freezable sourceFreezable)
        {
            throw new NotImplementedException();
        }

        protected virtual void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            throw new NotImplementedException();
        }

        protected Freezable CreateInstance()
        {
            throw new NotImplementedException();
        }

        protected abstract Freezable CreateInstanceCore();

        protected virtual bool FreezeCore(bool isChecking)
        {
            throw new NotImplementedException();
        }

        protected virtual void GetAsFrozenCore(Freezable sourceFreezable)
        {
            throw new NotImplementedException();
        }

        protected virtual void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnChanged()
        {
            throw new NotImplementedException();
        }

        protected void OnFreezablePropertyChanged(DependencyObject oldValue, DependencyObject newValue)
        {
            throw new NotImplementedException();
        }

        protected void OnFreezablePropertyChanged(
            DependencyObject oldValue,
            DependencyObject newValue,
            DependencyProperty property)
        {
            throw new NotImplementedException();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void ReadPreamble()
        {
            throw new NotImplementedException();
        }

        protected void WritePostscript()
        {
            throw new NotImplementedException();
        }

        protected void WritePreamble()
        {
            throw new NotImplementedException();
        }
    }
}
