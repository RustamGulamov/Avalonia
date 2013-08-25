﻿// -----------------------------------------------------------------------
// <copyright file="TextBox.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Controls
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Avalonia.Controls.Primitives;
    using Avalonia.Media;

    public class TextBox : TextBoxBase
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(TextBox),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        private TextBoxView textBoxView;

        static TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.CreateTextBoxView();
        }

        private void CreateTextBoxView()
        {
            // TODO: This should be a ScrollViewer but that's not implemented yet...
            Decorator contentHost = this.GetTemplateChild("PART_ContentHost") as Decorator;

            if (contentHost != null)
            {
                this.textBoxView = new TextBoxView();
                contentHost.Child = this.textBoxView;
            }
        }
    }
}
