using System;

namespace ei8.Cortex.IdentityAccess.Application.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsNotEmpty(this Guid guid) => guid != Guid.Empty;
    }
}
