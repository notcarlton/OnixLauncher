﻿using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Timers;

namespace OnixLauncher
{
    public partial class MainForm : Form
    {
        public static Form Instance;
        private RichPresence _presence;
        public static bool Bypassed;

        public MainForm()
        {
            // init
            InitializeComponent();
            Instance = this;
            _presence = new RichPresence();
            
            // create directories
            Directory.CreateDirectory(Utils.OnixPath);
            Directory.CreateDirectory(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                @"\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\RoamingState\OnixClient\Launcher");
            
            // stupid winforms thing that fixes message boxes
            CheckForIllegalCrossThreadCalls = false;

            // this is a bit useless, might remove soon
            Injector.InjectionCompleted += InjectionCompleted;

            // first time?
            if (!File.Exists(Utils.OnixPath + "\\firstTime"))
            {
                File.Create(Utils.OnixPath + "\\firstTime");
                Utils.ShowMessage("Welcome to Onix Client!", 
                    "Check our Discord's #faq channel if you're having problems.");
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            Winapi.ReleaseCapture();
            Winapi.SendMessage(Handle, Winapi.WM_NCLBUTTONDOWN, Winapi.HT_CAPTION, 0);
        }

        private void CreditsButton_Click(object sender, EventArgs e)
        {
            Utils.ShowMessage("Credits", "Onix Client - by Onix86\nOnix Launcher - by carlton");
        }

        private void BigOnixLogo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Utils.OpenFile();
        }

        private void InjectionCompleted(object sender, EventArgs e)
        {
            LaunchButton.Enabled = true;
            LaunchProgress.Visible = false;
        }

        private void LaunchButton_Click(object sender, EventArgs e)
        {
            Utils.ShowMessage("weird", "bug fix");
            Utils.MessageF.Hide(); // gotta show this once to make it work
            try
            {
                // let's go!
                LaunchButton.Enabled = false;
                LaunchProgress.Visible = true;

                var injectThread = new Thread(() =>
                {
                    var injectClient = new WebClient();
                    var dllPath = Utils.OnixPath + "\\OnixClient.dll";

                    // architecture detection
                    var arch = Utils.GetArchitecture();

                    if (arch == string.Empty)
                    {
                        // wtf the game not installed
                        Utils.ShowMessage("????????", "You don't even have Minecraft Bedrock installed!");
                        LaunchButton.Enabled = true;
                        LaunchProgress.Visible = false;
                        return;
                    }
                    
                    if (arch != "X64")
                    {
                        Utils.ShowMessage("Architecture Error", "You have a version of Minecraft that isn't 64-bit.");
                        LaunchButton.Enabled = true;
                        LaunchProgress.Visible = false;
                        return;
                    }

                    // version detection
                    var version = Utils.GetVersion();
                    var latestSupported = injectClient.DownloadString(
                        "https://raw.githubusercontent.com/bernarddesfosse/onixclientautoupdate/main/LatestSupportedVersion");
                    var stringTable = latestSupported.Split('\n');
                    var supported = false;

                    //version = "eaghruyehruger"; // test

                    foreach (var ver in stringTable)
                    {
                        if (version == ver)
                            supported = true;
                    }

                    if (!supported && !Bypassed)
                    {
                        LaunchButton.Enabled = true;
                        LaunchProgress.Visible = false;
                        Utils.ShowMessage("Unsupported Version",
                            "Onix Client cannot be used with your current Minecraft version.");
                    }
                    else
                    {
                        if (File.Exists(dllPath) && Process.GetProcessesByName("Minecraft.Windows").Length == 0)
                            File.Delete(dllPath);

                        if (!File.Exists(dllPath))
                            injectClient.DownloadFile(
                                "https://github.com/bernarddesfosse/onixclientautoupdate/raw/main/OnixClient.dll",
                                dllPath);

                        if (Bypassed && Utils.SelectedPath != "no file")
                            Injector.Inject(Utils.SelectedPath);
                        else
                            Injector.Inject(dllPath);

                        _presence.ChangePresence("In the menus", Utils.GetVersion(), Utils.GetXboxGamertag());
                        PresenceTimer.Start();
                    }
                    
                    injectClient.Dispose(); // HOLY SHIT!!!!
                });
                injectThread.Start();
            }
            catch
            {
                Utils.ShowMessage("Launch Error", "Failed to launch Onix Client. Please try again later.");
            }
        }

        private string _previousServer; // ok
        private bool _once;

        private void ChangeServer()
        {
            try
            {
                _previousServer = File.ReadAllText(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    + @"\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\RoamingState\OnixClient\Launcher\server.txt");
            }
            catch
            {
                if (!_once)
                {
                    Utils.ShowMessage("RPC Error", "Discord Rich presence is currently unavailable.");
                    _once = true;
                }
            }

            if (!_once)
            {
                var server = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                                          + @"\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\RoamingState\OnixClient\Launcher\server.txt");
            if (server == _previousServer) return;
            _previousServer = server;

            if (server == "")
                _presence.ChangePresence("In the menus", Utils.GetVersion(), Utils.GetXboxGamertag());
            else if (server.Contains("In a World, "))
                _presence.ChangePresence("In a world: " + server.Remove(0, 12), Utils.GetVersion(),
                    Utils.GetXboxGamertag());
            else
            {
                switch (server)
                {
                    case "geo.hivebedrock.network":
                    case "fr.hivebedrock.network":
                    case "ca.hivebedrock.network":
                    case "sg.hivebedrock.network":
                    case "jp.hivebedrock.network":
                        _presence.ChangePresence("Playing on The Hive", Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                    case "play.inpvp.net":
                        _presence.ChangePresence("Playing on Mineville", Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                    case "mco.cubecraft.net":
                        _presence.ChangePresence("Playing on CubeCraft", Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                    case "mco.mineplex.com":
                        _presence.ChangePresence("Playing on Mineplex", Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                    case "play.galaxite.net":
                        _presence.ChangePresence("Playing on Galaxite", Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                    case "mco.lbsg.net":
                        _presence.ChangePresence("Playing on Lifeboat", Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                    case "play.nethergames.org":
                        _presence.ChangePresence("Playing on NetherGames", Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                    case "play.hyperlandsmc.net":
                        _presence.ChangePresence("Playing on HyperLands", Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                    default:
                        _presence.ChangePresence("Playing on " + server, Utils.GetVersion(), Utils.GetXboxGamertag());
                        break;
                }
            }
            }
        }

        private void PresenceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var minecraftIndex = Process.GetProcessesByName("Minecraft.Windows");

            if (minecraftIndex.Length == 0)
            {
                _presence.ResetPresence();
                PresenceTimer.Stop();
            }
            else
            {
                ChangeServer();
            }
        }
    }
}