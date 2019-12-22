using System;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts.PlayerRoles
{
    [Serializable]
    public class PlayerRole
    {
        [HideInInspector]
        [SyncVar] public Roles role;

        [SerializeField] private Behaviour[] hiderRoleSet   = default;
        [SerializeField] private Behaviour[] seekerRoleSet  = default;
        public                   Behaviour[] defaultRoleSet = default;

        public Behaviour[] GetRoleSet()
        {
            switch (role)
            {
                case Roles.Hider:  return hiderRoleSet;
                case Roles.Seeker: return seekerRoleSet;
                default:           return defaultRoleSet;
            }
        }
    }
}