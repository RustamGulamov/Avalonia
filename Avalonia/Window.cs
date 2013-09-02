// -----------------------------------------------------------------------
// <copyright file="Window.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Input;
    using Avalonia.Media;
    using Avalonia.Platform;

    public class Window : ContentControl
    {
        private bool isShown;

        static Window()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
            WidthProperty.OverrideMetadata(typeof(Window), new PropertyMetadata(WidthChanged));
            HeightProperty.OverrideMetadata(typeof(Window), new PropertyMetadata(HeightChanged));
        }

        public Window()
        {
            this.PresentationSource = PlatformInterface.Instance.CreatePresentationSource();
            this.PresentationSource.RootVisual = this;
            this.PresentationSource.Closed += (s, e) => this.OnClosed(EventArgs.Empty);
            this.PresentationSource.Resized += (s, e) => this.DoMeasureArrange();

            this.Background = new SolidColorBrush(Colors.White);
            this.Width = this.PresentationSource.BoundingRect.Width;
            this.Height = this.PresentationSource.BoundingRect.Height;
        }

        public event EventHandler Closed;

        internal PlatformPresentationSource PresentationSource
        {
            get;
            private set;
        }

        public void Show()
        {
            this.PresentationSource.Show();
            this.isShown = true;
            this.DoMeasureArrange();
        }

        public void DoMeasureArrange()
        {
            if (this.isShown)
            {
                Size clientSize = this.PresentationSource.ClientSize;
                this.Measure(clientSize);
                this.Arrange(new Rect(clientSize));
                this.DoRender();
            }
        }

        protected virtual void OnClosed(EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed(this, e);
            }
        }

        private static void WidthChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Window window = (Window)sender;
            Rect rect = window.PresentationSource.BoundingRect;
            rect.Width = (double)e.NewValue;
            window.PresentationSource.BoundingRect = rect;
        }

        private static void HeightChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Window window = (Window)sender;
            Rect rect = window.PresentationSource.BoundingRect;
            rect.Height = (double)e.NewValue;
            window.PresentationSource.BoundingRect = rect;
        }

        private void DoRender()
        {
            using (DrawingContext drawingContext = this.PresentationSource.CreateDrawingContext())
            {
                this.DoRender(drawingContext, this);
            }
        }

        private void DoRender(DrawingContext drawingContext, DependencyObject o)
        {
            Visual visual = o as Visual;
            UIElement uiElement = o as UIElement;
            bool performPop = false;

            if (uiElement != null)
            {
                TranslateTransform translate = new TranslateTransform(uiElement.VisualOffset);
                drawingContext.PushTransform(translate);
                performPop = true;
                uiElement.OnRender(drawingContext);
            }

            if (visual != null)
            {
                for (int i = 0; i < visual.VisualChildrenCount; ++i)
                {
                    this.DoRender(drawingContext, visual.GetVisualChild(i));
                }
            }

            if (performPop)
            {
                drawingContext.Pop();
            }
        }
    }
}
