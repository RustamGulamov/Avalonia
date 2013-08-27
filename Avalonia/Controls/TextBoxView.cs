﻿// -----------------------------------------------------------------------
// <copyright file="TextBoxView.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Controls
{
    using System.Globalization;
    using Avalonia.Media;

    internal class TextBoxView : FrameworkElement
    {
        private TextBox parent;

        private FormattedText formattedText;

        public TextBoxView(TextBox parent)
        {
            this.parent = parent;
        }

        public FormattedText FormattedText
        {
            get
            {
                if (this.formattedText == null)
                {
                    this.formattedText = this.CreateFormattedText();
                }

                return this.formattedText;
            }
        }

        public void InvalidateText()
        {
            this.formattedText = null;
            this.InvalidateMeasure();
        }

        protected internal override void OnRender(DrawingContext drawingContext)
        {
            Rect rect = new Rect(new Size(this.ActualWidth, this.ActualHeight));

            drawingContext.DrawText(this.FormattedText, new Point());

            if (this.parent.IsKeyboardFocused)
            {
                Point caretPos = this.FormattedText.GetCaretPosition(this.parent.CaretIndex);
                Brush caretBrush = this.parent.CaretBrush;

                if (caretBrush == null)
                {
                    Color color = Colors.Black;
                    SolidColorBrush background = this.parent.Background as SolidColorBrush;

                    if (background != null)
                    {
                        color = Color.FromUInt32(0x00ffffffu ^ background.Color.ToUint32());
                    }

                    caretBrush = new SolidColorBrush(color);
                }

                drawingContext.DrawLine(
                    new Pen(caretBrush, 1),
                    caretPos,
                    caretPos + new Vector(0, this.FormattedText.Height));
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(this.FormattedText.Width, this.FormattedText.Height);
        }

        private FormattedText CreateFormattedText()
        {
            return new FormattedText(
                this.parent.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(this.parent.FontFamily, this.parent.FontStyle, this.parent.FontWeight, this.parent.FontStretch),
                this.parent.FontSize,
                this.parent.Foreground);
        }
    }
}
