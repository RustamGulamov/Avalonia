// -----------------------------------------------------------------------
// <copyright file="Application.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Avalonia.Threading;
    using System.Xaml;
    using System.Xml;

    public class Application : DispatcherObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        static Application()
        {
            RegisterDependencyProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public Application()
        {
            Application.Current = this;
        }

        public static Application Current { get; private set; }

        public Window MainWindow { get; set; }

        public Type PresentationSourceType { get; set; }

        public static void LoadComponent(object component, Uri resourceLocator)
        {
            DependencyObject dependencyObject = component as DependencyObject;
            NameScope nameScope = new NameScope();

            if (dependencyObject != null)
            {
                NameScope.SetNameScope(dependencyObject, nameScope);
            }

            XmlReader xml = XmlReader.Create(resourceLocator.OriginalString);
            XamlXmlReader reader = new XamlXmlReader(xml);
            XamlObjectWriter writer = new XamlObjectWriter(
                new XamlSchemaContext(),
                new XamlObjectWriterSettings
                {
                    RootObjectInstance = component,
                    ExternalNameScope = nameScope,
                    RegisterNamesOnExternalNamescope = true,
                });

            while (reader.Read())
            {
                writer.WriteNode(reader);
            }
        }

        public void Run()
        {
            this.Run(this.MainWindow);
        }

        public void Run(Window window)
        {
            if (window != null)
            {
                window.Closed += (s, e) => this.Dispatcher.InvokeShutdown();
                window.Show();
            }

            if (this.MainWindow == null)
            {
                this.MainWindow = window;
            }

            Dispatcher.Run();
        }

        /// <summary>
        /// Ensures that all dependency properties are registered.
        /// </summary>
        private static void RegisterDependencyProperties()
        {
            IEnumerable<Type> types = from type in Assembly.GetCallingAssembly().GetTypes()
                                      where typeof(DependencyObject).IsAssignableFrom(type)
                                      select type;

            BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

            foreach (Type type in types)
            {
                FieldInfo firstStaticField = type.GetFields(flags).FirstOrDefault();

                if (firstStaticField != null)
                {
                    object o = firstStaticField.GetValue(null);
                }
            }
        }
    }
}
