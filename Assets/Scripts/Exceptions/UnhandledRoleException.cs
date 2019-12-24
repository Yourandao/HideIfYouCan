using System;

using Assets.Scripts.PlayerScripts.PlayerRoles;

namespace Assets.Scripts.Exceptions
{
    public sealed class UnhandledRoleException : Exception
    {
        public UnhandledRoleException(Roles role) : base($"{role} is not handled in this function") { }
    }
}