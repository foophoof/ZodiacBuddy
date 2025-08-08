using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using ZodiacBuddy.BonusLight;

namespace ZodiacBuddy;

/// <summary>
/// Static class used to store debug and check functions for dev.
/// </summary>
public static class DebugTools
{
    /// <summary>
    /// Check that all the territory id have a name in Lumina.
    /// <p/>
    /// If a territory doesn't have a name, it's id have probably changed and will need to be updated in the code.
    /// </summary>
    public static void CheckBonusLightDutyTerritories()
    {
        var dutyWithoutName = BonusLightDuty.GetDataset()
            .Where(it => string.IsNullOrWhiteSpace(it.Value.DutyName))
            .ToList();
        SeStringBuilder sb = new SeStringBuilder()
            .AddUiForeground("[ZodiacBuddy] ", 45);
        if (dutyWithoutName.Count > 0)
        {
            sb.AddText("The following duties have no name in Lumina: ")
                .AddText(string.Join(", ", dutyWithoutName.Select(it => it.Key)));
        }
        else
        {
            sb.AddText("Nothing to report");
        }

        Service.ChatGui.Print(new XivChatEntry
        {
            Type = XivChatType.Echo,
            Message = sb.BuiltString,
        });
    }
}