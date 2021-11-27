using System;
using System.Collections.Generic;
using System.Text;

namespace _2C2P.Core.Data
{
    public enum FieldAction
    {
        Exclude = 0,
        Include = 1
    }

    public enum SqlTransType
    {
        None = 0,
        Commit = 1,
        Rollback = 2
    }
}
