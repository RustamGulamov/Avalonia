﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Media
{
    public sealed class Colors
    {
        public static Color Black
        {
            get { return Color.FromRgb(0, 0, 0); }
        }

        public static Color Blue
        {
            get { return Color.FromRgb(0, 0, 0xff); }
        }

        public static Color Green
        {
            get { return Color.FromRgb(0, 0xff, 0); }
        }

        public static Color Red
        {
            get { return Color.FromRgb(0xff, 0, 0); }
        }

        public static Color White
        {
            get { return Color.FromRgb(0xff, 0xff, 0xff); }
        }
    }
}
