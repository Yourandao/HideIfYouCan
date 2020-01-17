using System;

using Discord;

using UnityEngine;

namespace Scripts.UI
{
    public sealed class DiscordController : MonoBehaviour
    {
        [SerializeField] private UserInterface userInterface;

        [SerializeField] private int skipUpdates = 100;
        private                  int skippedUpdates;

        private Discord.Discord discord;
        private ActivityManager activityManager;
        private Activity        activity;

        private void Start()
        {
            discord = new Discord.Discord(667670050039267329, (ulong) Discord.CreateFlags.Default);

            activityManager = discord.GetActivityManager();

            activity = new Activity
            {
                Name = Application.productName
            };

            UpdateActivity();
        }

        private void FixedUpdate()
        {
            skippedUpdates++;

            if (skippedUpdates == skipUpdates)
            {
                UpdateActivity();

                skippedUpdates = 0;
            }

            discord.RunCallbacks();
        }

        private void OnDestroy()
        {
            activityManager.ClearActivity(ActivityActionCallback);

            discord.Dispose();
        }

        private void UpdateActivity()
        {
            activity.State   = userInterface.gameState.text;
            activity.Details = $"Role: {userInterface.role.text}";

            activityManager.UpdateActivity(activity, ActivityActionCallback);
        }

        private static void ActivityActionCallback(Result result) =>
            Debug.Log($"Discrod activity response: {result.ToString()}");
    }
}