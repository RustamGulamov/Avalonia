// -----------------------------------------------------------------------
// <copyright file="Control.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Avalonia.Media;

    public class Control : FrameworkElement
    {
        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(Colors.White),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FontFamilyProperty =
            TextBlock.FontFamilyProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    new FontFamily("Segoe UI"),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty FontSizeProperty =
            TextBlock.FontSizeProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    12.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty FontStretchProperty =
            TextBlock.FontStretchProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    new FontStretch(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty FontStyleProperty =
            TextBlock.FontStyleProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    FontStyles.Normal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty FontWeightProperty =
            TextBlock.FontWeightProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    FontWeights.Normal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalContentAlignment",
                typeof(HorizontalAlignment),
                typeof(Control),
                new FrameworkPropertyMetadata(HorizontalAlignment.Left));

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(
                "Template",
                typeof(ControlTemplate),
                typeof(Control),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(
                "VerticalContentAlignment",
                typeof(VerticalAlignment),
                typeof(Control),
                new FrameworkPropertyMetadata(VerticalAlignment.Top));

        private bool templateApplied;
        private List<Visual> visualChildren;

        public Control()
        {
            this.visualChildren = new List<Visual>();
            this.Background = new SolidColorBrush(Colors.White);
        }

        public Brush Background
        {
            get { return (Brush)this.GetValue(BackgroundProperty); }
            set { this.SetValue(BackgroundProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)this.GetValue(FontFamilyProperty); }
            set { this.SetValue(FontFamilyProperty, value); }
        }

        public double FontSize
        {
            get { return (double)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)this.GetValue(FontStretchProperty); }
            set { this.SetValue(FontStretchProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)this.GetValue(FontStyleProperty); }
            set { this.SetValue(FontStyleProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)this.GetValue(FontWeightProperty); }
            set { this.SetValue(FontWeightProperty, value); }
        }

        public HorizontalAlignment HorizontalContentAlignment 
        {
            get { return (HorizontalAlignment)this.GetValue(HorizontalContentAlignmentProperty); } 
            set { this.SetValue(HorizontalContentAlignmentProperty, value); } 
        }

        public ControlTemplate Template 
        {
            get { return (ControlTemplate)this.GetValue(TemplateProperty); }
            set { this.SetValue(TemplateProperty, value); }
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)this.GetValue(VerticalContentAlignmentProperty); }
            set { this.SetValue(VerticalContentAlignmentProperty, value); }
        }

        protected internal override int VisualChildrenCount
        {
            get { return this.visualChildren.Count; }
        }

        public override bool ApplyTemplate()
        {
            if (!this.templateApplied)
            {
                this.templateApplied = true;

                if (this.Template == null)
                {
                    this.ApplyTheme();
                }

                if (this.Template != null)
                {
                    FrameworkElement child = this.Template.CreateVisualTree(this);
                    this.visualChildren.Add(child);
                    this.AddVisualChild(child);
                    return true;
                }
            }

            return false;
        }

        protected internal override Visual GetVisualChild(int index)
        {
            return this.visualChildren[index];
        }

        private void ApplyTheme()
        {
            object defaultStyleKey = this.DefaultStyleKey;

            if (defaultStyleKey == null)
            {
                throw new InvalidOperationException("DefaultStyleKey must be set.");
            }

            Style style = (Style)this.FindResource(this.DefaultStyleKey);
            style.Attach(this);
        }
    }
}
