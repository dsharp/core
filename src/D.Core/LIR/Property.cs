﻿using System;

namespace D
{
    public class Property : IObject
    {
        public Property(string name, IType type, bool isMutable = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsMutable = isMutable;
        }

        public string Name { get; }

        // String
        // String | Number
        // A & B
        public IType Type { get; }

        public bool IsMutable { get; }

        Kind IObject.Kind => Kind.Property;

        #region ITypeMember

        public IType DeclaringType { get; set; }

        #endregion
    }
}
