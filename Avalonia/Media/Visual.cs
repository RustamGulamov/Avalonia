// -----------------------------------------------------------------------
// <copyright file="Visual.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence
// See licence.md for more information
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Media
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Visual : DependencyObject
    {
        protected internal virtual int VisualChildrenCount
        {
            get { return 0; }
        }

        protected internal DependencyObject VisualParent
        {
            get;
            private set;
        }

        protected internal Transform VisualTransform 
        { 
            get; 
            protected set; 
        }

        protected void AddVisualChild(Visual child)
        {
            child.VisualParent = this;
        }

        protected void RemoveVisualChild(Visual child)
        {
            child.VisualParent = this;
        }

        protected internal virtual Visual GetVisualChild(int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
