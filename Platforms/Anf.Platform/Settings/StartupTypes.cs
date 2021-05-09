using System;

namespace Anf.Platform.Settings
{
    [Flags]
    public enum StartupTypes
    {
        None = 0,
        Proposal = 1,
        Providers = 2
    }
}
