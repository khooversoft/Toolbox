// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Custom string type for validation
    /// </summary>
    [DebuggerDisplay("Value={Value}")]
    public abstract class StringType : CustomTypeBase<string>
    {
        protected StringType(string value, int? maxLength = null)
            : base(value)
        {
            Verify.IsNotEmpty(nameof(Value), Value);

            MaxLength = maxLength;
        }

        public int? MaxLength { get; }

        public override bool IsValueValid()
        {
            bool valid = base.IsValueValid();
            if (!valid)
            {
                return valid;
            }

            if (MaxLength != null)
            {
                valid = Value.IsNotEmpty() && Value.Length <= MaxLength;
            }

            return valid;
        }

        public override bool Equals(object obj)
        {
            StringType stringObj = obj as StringType;
            if (stringObj == null)
            {
                return false;
            }

            if (Value == null && Value == stringObj.Value)
            {
                return true;
            }

            return Value.Equals(stringObj.Value);
        }

        public override int GetHashCode()
        {
            if (Value == null)
            {
                return 0;
            }

            return Value.GetHashCode();
        }
    }
}
