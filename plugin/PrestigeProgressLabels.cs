using HarmonyLib;
using System;
using UnityEngine;

namespace ScritchyScratchyAP
{
    // Rewrites each prestige perk's "bought/max" label to "bought/AP-unlocked/max" and tints it
    // green while an unlocked level is waiting to be purchased.
    internal static class PrestigeProgressLabels
    {
        static readonly Color Available = new Color(0.4f, 1f, 0.4f, 1f);

        public static void Refresh(PrestigeUpgradePanel panel)
        {
            try
            {
                if (panel == null || !ArchipelagoManager.Connected) return;
                var label = panel.countLabel;
                var data = panel.Data;
                if (label == null || data == null || !label.gameObject.activeInHierarchy) return;

                string id = data.id;
                var received = TrackingManager.GetReceivedItemCounts();
                int max, unlocked;

                bool single = Array.IndexOf(Locations.PrestigeSinglePerks, id) >= 0;
                if (single)
                {
                    max = 1;
                    unlocked = received.TryGetValue($"Unlock {id}", out int u) ? Math.Min(u, 1) : 0;
                }
                else
                {
                    max = -1;
                    foreach (var (perkId, perkMax) in Locations.PrestigeMultiPerks)
                        if (perkId == id) { max = perkMax; break; }
                    if (max < 0) return;
                    unlocked = received.TryGetValue($"Progressive {id}", out int p) ? Math.Min(p, max) : 0;
                }

                int bought = SaveData.Current?.GetPrestigeUpgradeBuyCount(id) ?? 0;
                if (bought >= max) return; // leave the game's own maxed display alone

                label.text = $"{bought}/{unlocked}/{max}";
                label.color = HasUnsentCheck(id, unlocked, single) ? Available : Color.white;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"AP: PrestigeProgressLabels.Refresh failed: {ex.Message}");
            }
        }

        // Based on sent checks, not the buy count, so respeccing doesn't re-highlight already-checked levels.
        static bool HasUnsentCheck(string id, int unlocked, bool single)
        {
            if (single)
                return unlocked >= 1 && !TrackingManager.IsLocationSent($"Buy Prestige Perk {id}");
            for (int l = 1; l <= unlocked; l++)
                if (!TrackingManager.IsLocationSent($"Buy Prestige Perk {id} Level {l}")) return true;
            return false;
        }

        public static void RefreshAll()
        {
            var panels = UnityEngine.Object.FindObjectsOfType<PrestigeUpgradePanel>();
            foreach (var panel in panels) Refresh(panel);
        }
    }

    [HarmonyPatch(typeof(PrestigeUpgradePanel), nameof(PrestigeUpgradePanel.UpdateData))]
    public class Patch_PrestigePanelUpdateData
    {
        static void Postfix(PrestigeUpgradePanel __instance) => PrestigeProgressLabels.Refresh(__instance);
    }
}

