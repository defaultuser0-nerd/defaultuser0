﻿using Grate.GUI;
using Grate.Tools;
using BepInEx.Configuration;
using GorillaGameModes;
using GorillaLocomotion;
using System;

namespace Grate.Modules
{
    public class SpeedBoost : GrateModule
    {
        public static readonly string DisplayName = "Speed Boost";
        public static float baseVelocityLimit, scale = 1.5f;
        public static bool active = false;

        void FixedUpdate()
        {
            string progress = "";
            try
            {
                progress = "Getting Gamemode\n";
                var gameMode = GorillaGameManager.instance?.GameModeName();
                progress = "Checking status\n";
                if (active && (gameMode is null || gameMode == "NONE" || gameMode == "CASUAL"))
                {
                    progress = "Setting multiplier\n";
                    Player.Instance.jumpMultiplier = 1.3f * scale;
                    Player.Instance.maxJumpSpeed = 8.5f * scale;
                }
            }
            catch (Exception e)
            {
                Logging.Debug("GorillaGameManager.instance is null:", GorillaGameManager.instance is null);
                Logging.Debug("GorillaGameManager.instance.GameMode() is null:", GorillaGameManager.instance?.GameModeName() is null);
                Logging.Debug(progress);
                Logging.Exception(e);
            }
        }

        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            active = true;
            baseVelocityLimit = Player.Instance.velocityLimit;
            ReloadConfiguration();
        }

        protected override void Cleanup()
        {
            if (active)
            {
                scale = 1;
                Player.Instance.velocityLimit = baseVelocityLimit;
                active = false;
            }
        }
        protected override void ReloadConfiguration()
        {
            scale = 1 + (Speed.Value / 20f);
            if(this.enabled)
                Player.Instance.velocityLimit = baseVelocityLimit * scale;
        }

        public static ConfigEntry<int> Speed;
        public static void BindConfigEntries()
        {
            Speed = Plugin.configFile.Bind(
                section: DisplayName,
                key: "speed",
                defaultValue: 5,
                description: "How fast you run while speed boost is active"
            );
        }

        public override string GetDisplayName()
        {
            return "very speedy";
        }
        public override string Tutorial()
        {
            return "Effect: Increases your jump strength.";
        }

    }
}
