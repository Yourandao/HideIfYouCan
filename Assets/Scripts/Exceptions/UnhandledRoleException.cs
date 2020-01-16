using System;

using Scripts.PlayerScripts;

namespace Scripts.Exceptions
{
    public sealed class UnhandledRoleException : Exception
    {
        public UnhandledRoleException(Role role) : base($"Unhandled role '{role}'") { }
    }
}