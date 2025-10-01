using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System.Linq;
using ZodiacBuddy.BonusLight;
using ZodiacBuddy.Stages.Atma.Data;

namespace ZodiacBuddy;

/// <summary>
///     Static class used to store debug and check functions for dev.
/// </summary>
public static class DebugTools
{
    /// <summary>
    ///     Check that all the territory id in the bonus light duty have a name in Lumina.
    ///     <p />
    ///     If a territory doesn't have a name, it's id have probably changed and will need to be updated in the code.
    /// </summary>
    public static void CheckBonusLightDutyTerritories()
    {
        var dutyWithoutName = BonusLightDuty.GetDataset()
            .Where(it => string.IsNullOrWhiteSpace(it.Value.DutyName))
            .ToList();
        var sb = new SeStringBuilder()
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

        Service.ChatGui.Print(new XivChatEntry {Type = XivChatType.Echo, Message = sb.BuiltString});
    }

    /// <summary>
    ///     Check that all the territory id in the brave books have a name in Lumina.
    ///     <p />
    ///     If a territory doesn't have a name, it's id have probably changed and will need to be updated in the code.
    /// </summary>
    public static void CheckBraveDutyTerritory()
    {
        var braveTerritories = BraveBook.GetAllValues()
            .SelectMany(it => it.Dungeons.Select(dg => dg.Position))
            .Where(it => string.IsNullOrWhiteSpace(it.PlaceName))
            .ToList();
        var sb = new SeStringBuilder()
            .AddUiForeground("[ZodiacBuddy] ", 45);
        if (braveTerritories.Count != 0)
        {
            sb.AddText("The following territory type id have no name in Lumina: ")
                .AddText(string.Join(", ", braveTerritories.Select(it => it.TerritoryType.RowId)));
        }
        else
        {
            sb.AddText("Nothing to report");
        }

        Service.ChatGui.Print(new XivChatEntry {Type = XivChatType.Echo, Message = sb.BuiltString});
    }
}
