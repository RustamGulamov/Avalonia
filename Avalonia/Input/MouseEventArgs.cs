// -----------------------------------------------------------------------
// <copyright file="MouseEventArgs.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    public class MouseEventArgs : RoutedEventArgs
    {
        [AvaloniaSpecific]
        public Point AbsolutePosition { get; set; }
    }
}
