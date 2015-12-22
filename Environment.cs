using Rocket.Hurtworld;
using System;
using System.IO;

namespace Rocket.Hurtworld
{
    public static class Environment
    {
        public static string RocketDirectory;
        public static void Initialize()
        {
            RocketDirectory = String.Format("Servers/{0}/Rocket/", H.Instance.InstanceId);
            if (!Directory.Exists(RocketDirectory)) Directory.CreateDirectory(RocketDirectory);

            Directory.SetCurrentDirectory(RocketDirectory);
        }

        public static readonly string SettingsFile = "Rocket.Hurtworld.config.xml";
        public static readonly string TranslationFile = "Rocket.Hurtworld.{0}.translation.xml";
        public static readonly string ConsoleFile = "{0}.console";
    }
}