﻿// -----------------------------------------------------------------------
// <copyright file="IInputElement.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IInputElement
    {
        bool IsIsKeyboardFocused { get; }
    }
}
