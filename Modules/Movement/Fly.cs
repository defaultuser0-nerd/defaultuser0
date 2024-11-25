﻿using UnityEngine;
using Player = GorillaLocomotion.Player;
using Grate.GUI;
using BepInEx.Configuration;
using Grate.Extensions;
using Grate.Gestures;

namespace Grate.Modules.Movement
{
    public class Fly : GrateModule
    {
        public static readonly string DisplayName = "Fly";
        float speedScale = 10, acceleration = .01f;
        Vector2 xz;
        float y;
        void FixedUpdate()
        {
            // nullify gravity by adding it's negative value to the player's velocity
            var rb = Player.Instance.bodyCollider.attachedRigidbody;
            if (GrateModule.enabledModules.ContainsKey(Bubble.DisplayName)
                && !GrateModule.enabledModules[Bubble.DisplayName])
                rb.AddForce(-UnityEngine.Physics.gravity * rb.mass * Player.Instance.scale);

            xz = GestureTracker.Instance.leftStickAxis.GetValue();
            y = GestureTracker.Instance.rightStickAxis.GetValue().y;

            Vector3 inputDirection = new Vector3(xz.x, y, xz.y);

            // Get the direction the player is facing but nullify the y axis component
            var playerForward = Player.Instance.bodyCollider.transform.forward;
            playerForward.y = 0;

            // Get the right vector of the player but nullify the y axis component
            var playerRight = Player.Instance.bodyCollider.transform.right;
            playerRight.y = 0;

            var velocity =
                inputDirection.x * playerRight +
                y * Vector3.up +
                inputDirection.z * playerForward;
            velocity *= Player.Instance.scale * speedScale;
            rb.velocity = Vector3.Lerp(rb.velocity, velocity, acceleration);
        }

        public override string GetDisplayName()
        {
            return "creative mode";
        }

        public override string Tutorial()
        {
            return "double jump with space";
        }

        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            ReloadConfiguration();
        }

        public static ConfigEntry<int> Speed;
        public static ConfigEntry<int> Acceleration;
        protected override void ReloadConfiguration()
        {
            speedScale = Speed.Value * 2;
            acceleration = Acceleration.Value;
            if (acceleration == 10)
                acceleration = 1;
            else
                acceleration = MathExtensions.Map(Acceleration.Value, 0, 10, 0.0075f, .25f);
        }

        public static void BindConfigEntries()
        {
            Speed = Plugin.configFile.Bind(
                section: DisplayName,
                key: "speed",
                defaultValue: 5,
                description: "how much you vroom vroom"
            );

            Acceleration = Plugin.configFile.Bind(
                section: DisplayName,
                key: "acceleration",
                defaultValue: 5,
                description: "how much you vroom vroom but cooler"
            );
        }

        protected override void Cleanup() { }
    }
}
