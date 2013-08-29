// -----------------------------------------------------------------------
// <copyright file="DependencyProperty.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public delegate bool ValidateValueCallback(object value);

    [TypeConverter(typeof(DependencyPropertyConverter))]
    public sealed class DependencyProperty
    {
        public static readonly object UnsetValue = new object();
        private Dictionary<Type, PropertyMetadata> metadataByType = new Dictionary<Type, PropertyMetadata>();

        private DependencyProperty(
                    bool isAttached,
                    string name,
                    Type propertyType,
                    Type ownerType,
                    PropertyMetadata defaultMetadata,
                    ValidateValueCallback validateValueCallback)
        {
            if (defaultMetadata == null)
            {
                throw new ArgumentNullException("defaultMetadata");
            }

            this.IsAttached = isAttached;
            this.DefaultMetadata = defaultMetadata;
            this.Name = name;
            this.OwnerType = ownerType;
            this.PropertyType = propertyType;
            this.ValidateValueCallback = validateValueCallback;
        }

        public bool ReadOnly { get; private set; }

        public PropertyMetadata DefaultMetadata { get; private set; }

        public string Name { get; private set; }

        public Type OwnerType { get; private set; }

        public Type PropertyType { get; private set; }

        public ValidateValueCallback ValidateValueCallback { get; private set; }

        public int GlobalIndex
        {
            get { throw new NotImplementedException(); }
        }

        internal bool IsAttached { get; set; }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, null, null);
        }

        public static DependencyProperty Register(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata)
        {
            return Register(name, propertyType, ownerType, typeMetadata, null);
        }

        public static DependencyProperty Register(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            PropertyMetadata defaultMetadata;
            
            if (typeMetadata == null)
            {
                defaultMetadata = typeMetadata = new PropertyMetadata();
            }
            else
            {
                defaultMetadata = new PropertyMetadata(typeMetadata.DefaultValue);
            }

            DependencyProperty dp = new DependencyProperty(
                false,
                name,
                propertyType,
                ownerType,
                defaultMetadata,
                validateValueCallback);

            DependencyObject.Register(ownerType, dp);

            dp.OverrideMetadata(ownerType, typeMetadata);

            return dp;
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType)
        {
            return RegisterAttached(name, propertyType, ownerType, null, null);
        }

        public static DependencyProperty RegisterAttached(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata)
        {
            return RegisterAttached(name, propertyType, ownerType, defaultMetadata, null);
        }

        public static DependencyProperty RegisterAttached(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            if (defaultMetadata == null)
            {
                defaultMetadata = new PropertyMetadata();
            }

            DependencyProperty dp = new DependencyProperty(
                true,
                name,
                propertyType,
                ownerType,
                defaultMetadata,
                validateValueCallback);
            DependencyObject.Register(ownerType, dp);
            return dp;
        }

        public static DependencyPropertyKey RegisterAttachedReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata)
        {
            throw new NotImplementedException("RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata)");
        }

        public static DependencyPropertyKey RegisterAttachedReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            throw new NotImplementedException("RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback)");
        }

        public static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata)
        {
            return RegisterReadOnly(name, propertyType, ownerType, typeMetadata, null);
        }

        public static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            DependencyProperty prop = Register(name, propertyType, ownerType, typeMetadata, validateValueCallback);
            prop.ReadOnly = true;
            return new DependencyPropertyKey(prop);
        }

        public DependencyProperty AddOwner(Type ownerType)
        {
            return this.AddOwner(ownerType, null);
        }

        public DependencyProperty AddOwner(Type ownerType, PropertyMetadata typeMetadata)
        {
            if (typeMetadata == null)
            {
                typeMetadata = new PropertyMetadata();
            }

            this.OverrideMetadata(ownerType, typeMetadata);
            DependencyObject.Register(ownerType, this);

            // MS seems to always return the same DependencyProperty
            return this;
        }

        public PropertyMetadata GetMetadata(Type forType)
        {
            Type type = forType;

            while (type != null)
            {
                PropertyMetadata result;

                if (this.metadataByType.TryGetValue(type, out result))
                {
                    return result;
                }

                type = type.BaseType;
            }

            return this.DefaultMetadata;
        }

        public PropertyMetadata GetMetadata(DependencyObject dependencyObject)
        {
            return this.GetMetadata(dependencyObject.GetType());
        }

        public PropertyMetadata GetMetadata(DependencyObjectType dependencyObjectType)
        {
            return this.GetMetadata(dependencyObjectType.SystemType);
        }

        public bool IsValidType(object value)
        {
            if (value == null)
            {
                return !this.PropertyType.IsValueType || 
                    Nullable.GetUnderlyingType(this.PropertyType) != null;
            }
            else
            {
                return this.PropertyType.IsInstanceOfType(value);
            }
        }

        public bool IsValidValue(object value)
        {
            if (!this.IsValidType(value))
            {
                return false;
            }

            if (this.ValidateValueCallback == null)
            {
                return true;
            }

            return this.ValidateValueCallback(value);
        }

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            if (typeMetadata == null)
            {
                throw new ArgumentNullException("typeMetadata");
            }

            if (this.ReadOnly)
            {
                throw new InvalidOperationException(string.Format("Cannot override metadata on readonly property '{0}' without using a DependencyPropertyKey", this.Name));
            }

            typeMetadata.Merge(this.DefaultMetadata, this, forType);
            this.metadataByType.Add(forType, typeMetadata);
        }

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata, DependencyPropertyKey key)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            if (typeMetadata == null)
            {
                throw new ArgumentNullException("typeMetadata");
            }

            // further checking?  should we check
            // key.DependencyProperty == this?
            typeMetadata.Merge(this.DefaultMetadata, this, forType);
            this.metadataByType.Add(forType, typeMetadata);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.PropertyType.GetHashCode() ^ this.OwnerType.GetHashCode();
        }
    }
}