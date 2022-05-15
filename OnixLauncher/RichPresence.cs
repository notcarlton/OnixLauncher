﻿using DiscordRPC;
using System;

namespace OnixLauncher
{
    public static class RichPresence
    {
        public static DiscordRpcClient Client;
        private static string _discordTime = "";

        public static void Initialize()
        {
            dynamic dateTimestampEnd = null;

            if (_discordTime != "" && int.TryParse(_discordTime, out int timestampEnd))
                dateTimestampEnd = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestampEnd);

            Client = new DiscordRpcClient("845463201550434344");
            Client.Initialize();
            Client.SetPresence(new DiscordRPC.RichPresence
            {
                Details = "Ready to play",

                Assets = new Assets
                {
                    LargeImageKey = GetLargeImage(),
                    LargeImageText = "Onix Launcher"
                },
                Timestamps = new Timestamps
                {
                    Start = _discordTime != "" && int.TryParse(_discordTime, out int timestampStart) ?
                        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                            .AddSeconds(timestampStart) : DateTime.UtcNow,
                    End = dateTimestampEnd
                }
            });

            Log.Write("Initialized Rich Presence");
        }

        private static string GetLargeImage()
        {
            // Moved it to a variable so it can have support for more seasons later
            // You can use Dec-Feb for winter logo, Mar-Jun for spring logo, christmas,kwanza, hanukkah, etc
            string largeImgKey = "onix";
            if (DateTime.Today.Month == 10) largeImgKey = "onix-halloween";

            return largeImgKey;
        }

        public static void ChangePresence(string server, string version, string gamertag)
        {
            dynamic dateTimestampEnd = null;

            if (_discordTime != "" && int.TryParse(_discordTime, out int timestampEnd))
                dateTimestampEnd = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(timestampEnd);

            var smallimage = "minecraft";
            if (!server.Contains("In a world:") || server != "In the menus" || !server.Contains("."))
            {
                smallimage = server.Contains("The Hive") ? "hive" : server.Split(' ')[2].ToLower();
            }

            Client.SetPresence(new DiscordRPC.RichPresence
            {
                Details = server,
                State = "as " + gamertag,

                Assets = new Assets
                {

                    LargeImageKey = GetLargeImage(),
                    LargeImageText = "Onix Client",
                    SmallImageKey = smallimage,
                    SmallImageText = version
                },
                Timestamps = new Timestamps
                {
                    Start = _discordTime != "" && int.TryParse(_discordTime, out int timestampStart) ?
                        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                            .AddSeconds(timestampStart) : DateTime.UtcNow,
                    End = dateTimestampEnd
                }
            });
            Log.Write("Updated rich presence: " + server);
        }

        public static void ResetPresence()
        {
            dynamic dateTimestampEnd = null;

            if (_discordTime != "" && int.TryParse(_discordTime, out int timestampEnd))
                dateTimestampEnd = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestampEnd);

            Client.SetPresence(new DiscordRPC.RichPresence
            {
                Details = "Ready to play",

                Assets = new Assets
                {
                    LargeImageKey = GetLargeImage(),
                    LargeImageText = "Onix Launcher"
                },
                Timestamps = new Timestamps
                {
                    Start = _discordTime != "" && int.TryParse(_discordTime, out int timestampStart) ?
                        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                            .AddSeconds(timestampStart) : DateTime.UtcNow,
                    End = dateTimestampEnd
                }
            });
            Log.Write("Rich presence was reset to default values");
        }
    }
}