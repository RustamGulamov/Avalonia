﻿// -----------------------------------------------------------------------
// <copyright file="PlatformFactory.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Platform
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Provides platform-specific implementations.
    /// </summary>
    public abstract class PlatformFactory
    {
        /// <summary>
        /// The platform factory instance.
        /// </summary>
        private static PlatformFactory instance;

        /// <summary>
        /// Gets or sets the application-wide instance of the <see cref="PlatformFactory"/>.
        /// </summary>
        public static PlatformFactory Instance
        {
            get
            {
                if (instance == null)
                {
#if WINDOWS
                    string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    Assembly platform = Assembly.LoadFile(Path.Combine(path, "Avalonia.Direct2D1.dll"));
                    Type factoryType = platform.GetType("Avalonia.Direct2D1.Direct2DPlatformFactory");
                    instance = (PlatformFactory)Activator.CreateInstance(factoryType);
#else
                    throw new NotSupportedException("This platform is not supported.");
#endif
                }

                return instance;
            }
            
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Gets the platform-specific dispatcher implementation.
        /// </summary>
        public abstract IPlatformDispatcher Dispatcher
        {
            get;
            protected set;
        }

        /// <summary>
        /// Creates a new platform-specific <see cref="PresentationSource"/>.
        /// </summary>
        /// <returns>
        /// The newly created presentation source.
        /// </returns>
        public abstract PlatformPresentationSource CreatePresentationSource();
    }
}
