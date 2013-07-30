﻿namespace Avalonia
{
    using System;
using Avalonia.Input;
using Avalonia.Media;

    [AvaloniaSpecific]
    public abstract class AvaloniaPresentationSource : PresentationSource
    {
        public abstract Size ClientSize { get; }
        public abstract Rect BoundingRect { get; set; }

        public event EventHandler Closed;
        public event MouseButtonEventHandler MouseLeftButtonDown;
        public event EventHandler Resized;

        public abstract DrawingContext CreateDrawingContext();
        public abstract void Show();

        protected void OnClosed()
        {
            if (this.Closed != null)
            {
                this.Closed(this, EventArgs.Empty);
            }
        }

        protected void OnMouseButtonDown(MouseButtonEventArgs e)
        {
            if (this.MouseLeftButtonDown != null)
            {
                this.MouseLeftButtonDown(this, e);
            }
        }

        protected void OnResized()
        {
            if (this.Resized != null)
            {
                this.Resized(this, EventArgs.Empty);
            }
        }
    }
}
