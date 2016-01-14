using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.Core;
using Rocket.Core.Assets;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Hurtworld.Events;
using Rocket.Hurtworld.Serialisation;
using Steamworks;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rocket.Hurtworld
{
    public class H : MonoBehaviour, IRocketImplementation
    {
        private static GameObject rocketGameObject; 
        public static H Instance;

        private static readonly TranslationList defaultTranslations = new TranslationList(){
                { "command_generic_failed_find_player","Failed to find player"},
                { "command_generic_invalid_parameter","Invalid parameter"},
                { "command_generic_target_player_not_found","Target player not found"},
                { "command_rocket_plugins_loaded","Loaded: {0}"},
                { "command_rocket_plugins_unloaded","Unloaded: {0}"},
                { "command_rocket_plugins_failure","Failure: {0}"},
                { "command_rocket_plugins_cancelled","Cancelled: {0}"},
                { "command_rocket_reload_plugin","Reloading {0}"},
                { "command_rocket_not_loaded","The plugin {0} is not loaded"},
                { "command_rocket_unload_plugin","Unloading {0}"},
                { "command_rocket_load_plugin","Loading {0}"},
                { "command_rocket_already_loaded","The plugin {0} is already loaded"},
                { "command_rocket_reload","Reloading Rocket"},
                { "command_p_group_not_found","Group not found"},
                { "command_p_group_assigned","{0} was assigned to the group {1}"},
                { "command_rocket_plugin_not_found","Plugin {0} not found"},
                { "command_generic_invalid_character_name","invalid character name"}
        }; 

        public static XMLFileAsset<HurtworldSettings> Settings;
        public static XMLFileAsset<TranslationList> Translations;

        public IRocketImplementationEvents ImplementationEvents { get { return Events; } }
        public static HurtworldEvents Events;

        public event RocketImplementationInitialized OnRocketImplementationInitialized;

        public static string Translate(string translationKey, params object[] placeholder)
        {
            return Translations.Instance.Translate(translationKey, placeholder);
        }

#if !linux

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
#endif

        internal static void Splash()
        {
#if !linux
            AllocConsole();
#endif
            rocketGameObject = new GameObject("Rocket");
            DontDestroyOnLoad(rocketGameObject);

            StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
            sw.AutoFlush = true;
            Console.SetOut(sw);
            

            StreamReader sr = new StreamReader(Console.OpenStandardInput());
            Console.SetIn(sr);

            System.Console.Clear();
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("Rocket Hurtworld v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " for Hurtworld v" + "UNKNOWN" + "\n");
            


            //Core.Logging.Logger.ExternalLog(message, color);


            R.OnRockedInitialized += () =>
            {
                Instance.Initialize();
            };
            
            rocketGameObject.TryAddComponent<H>();
            rocketGameObject.TryAddComponent<R>();
        }
        
        private void Awake()
        {
            Instance = this;
            Environment.Initialize();
        }

        internal void Initialize()
        {
            try
            {
                Settings = new XMLFileAsset<HurtworldSettings>(Environment.SettingsFile);
                Translations = new XMLFileAsset<TranslationList>(String.Format(Environment.TranslationFile, Core.R.Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(TranslationListEntry) }, defaultTranslations);
                Events = gameObject.TryAddComponent<HurtworldEvents>();


                RocketPlugin.OnPluginLoading += (IRocketPlugin plugin, ref bool cancelLoading) =>
                {
                    //
                };

                RocketPlugin.OnPluginUnloading += (IRocketPlugin plugin) =>
                {
                    //
                };

                R.Commands.RegisterFromAssembly(Assembly.GetExecutingAssembly());
                
                OnRocketImplementationInitialized.TryInvoke();

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        
        public void Reload()
        {
            Translations.Reload();
            Settings.Reload();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        private bool steamInitialized = false;
        private void FixedUpdate()
        {
            if (!steamInitialized && SteamworksManagerClient.Instance.InitializedServer)
            {
                steamInitialized = true;
                try
                {
                    R.Plugins.OnPluginsLoaded += () =>
                    {
                        SteamGameServer.SetKeyValue("rocketplugins", String.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
                    };

                    SteamGameServer.SetKeyValue("rocket", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    SteamGameServer.SetBotPlayerCount(1);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Steam can not be initialized: " + ex.Message);
                }
            }
        }

        public string InstanceId
        {
            get
            {
                return Singleton<GameManager>.Instance.ServerConfig.GameName;
            } 
        }
    }
}
