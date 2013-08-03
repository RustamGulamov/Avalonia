// -----------------------------------------------------------------------
// <copyright file="Pen.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Media
{
    using Avalonia.Animation;

    public class Pen : Animatable
    {
        public Pen(Brush brush, double thickness)
        {
            this.Brush = brush;
            this.Thickness = thickness;
        }

        public Brush Brush { get; set; }

        public double Thickness { get; set; }
    }
}
