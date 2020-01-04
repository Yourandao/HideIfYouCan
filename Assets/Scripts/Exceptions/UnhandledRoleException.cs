using System;

using Scripts.Components;

namespace Scripts.Exceptions
{
    public sealed class UnhandledRoleException : Exception
    {
        public UnhandledRoleException(Role role) : base($"{role} is not handled in this function") { }
    }
}