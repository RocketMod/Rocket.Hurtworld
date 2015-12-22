using Rocket.API;
using UnityEngine;

namespace Rocket.Hurtworld.Events
{
    public class HurtworldEvents : MonoBehaviour, IRocketImplementationEvents
    {
        public event ImplementationShutdown OnShutdown;
    }
}
