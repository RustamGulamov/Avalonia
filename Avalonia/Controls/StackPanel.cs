﻿// -----------------------------------------------------------------------
// <copyright file="StackPanel.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Controls
{
    using System;

    public enum Orientation
    {
        Horizontal,
        Vertical,
    }

    public class StackPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(StackPanel),
                new FrameworkPropertyMetadata(
                    Orientation.Vertical,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Orientation Orientation 
        { 
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size result = new Size();
            Orientation orientation = this.Orientation;

            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(constraint);

                if (orientation == Orientation.Horizontal)
                {
                    result.Width += child.DesiredSize.Width;
                    result.Height = Math.Max(result.Height, child.DesiredSize.Height);
                }
                else
                {
                    result.Height += child.DesiredSize.Height;
                    result.Width = Math.Max(result.Width, child.DesiredSize.Width);
                }
            }

            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Orientation orientation = this.Orientation;
            int xIncrement = (orientation == Orientation.Horizontal) ? 1 : 0;
            int yIncrement = (orientation == Orientation.Vertical) ? 1 : 0;
            Point position = new Point();
            double maxWidth = 0;
            double maxHeight = 0;

            foreach (UIElement child in this.InternalChildren)
            {
                child.Arrange(new Rect(position, child.DesiredSize));
                position.X += child.RenderSize.Width * xIncrement;
                position.Y += child.RenderSize.Height * yIncrement;
                maxWidth = Math.Max(maxWidth, child.RenderSize.Width);
                maxHeight = Math.Max(maxHeight, child.RenderSize.Height);
            }

            if (orientation == Orientation.Horizontal)
            {
                return new Size(position.X, maxHeight);
            }
            else
            {
                return new Size(maxWidth, position.Y);
            }
        }
    }
}
