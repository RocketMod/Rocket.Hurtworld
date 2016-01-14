using Rocket.API;
using Rocket.Core;
using Steamworks;
using System;
using System.ComponentModel;

namespace Rocket.Hurtworld
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Interface
    {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Splash()
        {
            H.Splash();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool Execute(CSteamID player, string command)
        {
            if (R.Commands != null)
                return R.Commands.Execute(new RocketPlayer(player.ToString()), command);
            return false;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool ExecuteConsole(string command)
        {
            if (R.Commands != null)
                return R.Commands.Execute(new ConsolePlayer(), command);
            return false;
        }
    }
}
