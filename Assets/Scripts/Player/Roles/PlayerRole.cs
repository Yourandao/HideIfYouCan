using System;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts.PlayerRoles
{
    [Serializable]
    public class PlayerRole
    {
        [SyncVar] public Roles role;

        [SerializeField] private Behaviour[] unassignedRoleSet = default;
        [SerializeField] private Behaviour[] hiderRoleSet      = default;
        [SerializeField] private Behaviour[] seekerRoleSet     = default;
        [SerializeField] private Behaviour[] spectatorRoleSet  = default;

        public Behaviour[] GetRoleSet()
        {
            switch (role)
            {
                case Roles.Unassigned: return unassignedRoleSet;
                case Roles.Hider:      return hiderRoleSet;
                case Roles.Seeker:     return seekerRoleSet;
                case Roles.Spectator:  return spectatorRoleSet;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}