using Scripts.Components;
using Scripts.Exceptions;
using Scripts.Management.Network;

using UnityEngine;

namespace Assets.Scripts.Management.Network
{
    public sealed class NetworkSpawn : MonoBehaviour
    {
        [SerializeField] private Role targetRole;

        private void Awake()
        {
            switch (targetRole)
            {
                case Role.Hider:
                    ServerManager.RegisterHiderSpawn(transform);

                    break;
                case Role.Seeker:
                    ServerManager.RegisterSeekerSpawn(transform);

                    break;
                default: throw new UnhandledRoleException(targetRole);
            }
        }

        private void OnDestroy()
        {
            switch (targetRole)
            {
                case Role.Hider:
                    ServerManager.UnregisterHiderSpawn(transform);

                    break;
                case Role.Seeker:
                    ServerManager.UnregisterSeekerSpawn(transform);

                    break;
                default: throw new UnhandledRoleException(targetRole);
            }
        }
    }
}