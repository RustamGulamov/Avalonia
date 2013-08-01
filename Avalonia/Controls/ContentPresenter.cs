// -----------------------------------------------------------------------
// <copyright file="ContentPresenter.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Avalonia.Media;

    public class ContentPresenter : FrameworkElement
    {
        private Visual visualChild;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenter"/> class.
        /// </summary>
        public ContentPresenter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenter"/> class.
        /// </summary>
        internal ContentPresenter(ContentControl templatedParent)
        {
            this.Content = templatedParent.Content;
        }

        public object Content { get; set; }

        protected internal override int VisualChildrenCount
        {
            get { return (this.visualChild != null) ? 1 : 0; }
        }

        public override bool ApplyTemplate()
        {
            if (this.visualChild == null)
            {
                this.visualChild = this.Content as Visual;

                if (this.visualChild != null)
                {
                    this.AddVisualChild(this.visualChild);
                }

                return true;
            }

            return false;
        }

        protected internal override Visual GetVisualChild(int index)
        {
            if (index > 0 || this.visualChild == null)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.visualChild;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (this.VisualChildrenCount > 0)
            {
                UIElement ui = this.GetVisualChild(0) as UIElement;

                if (ui != null)
                {
                    ui.Measure(constraint);
                    return ui.DesiredSize;
                }
            }

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.VisualChildrenCount > 0)
            {
                UIElement ui = this.GetVisualChild(0) as UIElement;

                if (ui != null)
                {
                    ui.Arrange(new Rect(finalSize));
                    return finalSize;
                }
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
