using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{

    public interface ISettingsService
    {
        string UserDatabasePath { get; }
        string CortexGraphOutBaseUrl { get; }
        string EventSourcingOutBaseUrl { get; } 
    }
}
