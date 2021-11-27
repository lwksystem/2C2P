using System;
using System.Collections.Generic;

namespace _2C2P.Core.Attributes
{
    public class TypeAttribute<TAttribute>
    {
        public Type Type { get; set; }
        public IEnumerable<TAttribute> Attributes { get; set; }
    }
}
