using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    internal static class ExtensionMethods
    {
        internal static bool EqualsString(this Guid? guid, string value)
        {
            return (
                        value == null &&
                        !guid.HasValue
                    ) ||
                    (
                        value != null &&
                        guid.HasValue &&
                        guid.Value.ToString() == value
                    );
        }
    }
}
