using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Numerics;
using ZodiacBuddy.BonusLight;

namespace ZodiacBuddy.InformationWindow;

/// <summary>
///     Default information window.
/// </summary>
public abstract class InformationWindow
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InformationWindow" /> class.
    /// </summary>
    /// <param name="name">Name of the window.</param>
    protected InformationWindow(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether to show this window.
    /// </summary>
    public bool ShowWindow { get; set; }

    /// <summary>
    ///     Gets or sets the mainhand item.
    /// </summary>
    public InventoryItem MainHandItem { get; set; }

    /// <summary>
    ///     Gets or sets the offhand item.
    /// </summary>
    public InventoryItem OffhandItem { get; set; }

    private static BonusLightConfiguration BonusConfiguration => Service.Configuration.BonusLight;

    private static InformationWindowConfiguration InfoWindowConfiguration => Service.Configuration.InformationWindow;

    /// <summary>
    ///     Gets name of the information window.
    /// </summary>
    private string Name { get; }

    /// <summary>
    ///     Draw information window.
    /// </summary>
    public void Draw()
    {
        if (!ShowWindow)
        {
            return;
        }

        var flags = ImGuiWindowFlags.NoFocusOnAppearing |
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoScrollbar;

        flags |= InfoWindowConfiguration.ClickThrough
            ? ImGuiWindowFlags.NoInputs
            : ImGuiWindowFlags.None;

        if (ImGui.Begin(Name, flags))
        {
            DisplayRelicInfo(MainHandItem);
            DisplayRelicInfo(OffhandItem);
            DisplayBonusLight();

            if (!InfoWindowConfiguration.ManualSize)
            {
                ImGui.SetWindowSize(Name, Vector2.Zero);
            }
        }

        ImGui.End();
    }

    /// <summary>
    ///     Display information about the specified relic.
    /// </summary>
    /// <param name="item">Relic to display.</param>
    protected abstract void DisplayRelicInfo(InventoryItem item);

    /// <summary>
    ///     Determine the size of the progress bar.
    /// </summary>
    /// <param name="relicName">Name of the relic.</param>
    /// <returns>Vector2 of the determined size.</returns>
    protected static Vector2 DetermineProgressSize(string relicName)
    {
        if (!InfoWindowConfiguration.ProgressAutoSize)
        {
            return Vector2.Zero with {X = InfoWindowConfiguration.ProgressSize};
        }

        if (InfoWindowConfiguration.ManualSize ||
            BonusConfiguration is {DisplayBonusDuty: true, ActiveBonus.Count: > 0})
        {
            return Vector2.Zero with {X = ImGui.GetContentRegionAvail().X - 1};
        }

        var vector = ImGui.CalcTextSize(relicName) with {Y = 0f};

        return vector.X < 80 ? new Vector2(80, 0) : vector;
    }

    private void DisplayBonusLight()
    {
        if (!BonusConfiguration.DisplayBonusDuty)
        {
            return;
        }

        if (BonusConfiguration.ActiveBonus.Count > 0)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.TextColored(ImGuiColors.DalamudYellow, FontAwesomeIcon.Lightbulb.ToIconString());
            ImGui.PopFont();

            var bonusLightWindow = Util.CurrentBonusLightWindow();
            if (bonusLightWindow.HasValue)
            {
                var (startWindow, endWindow) = bonusLightWindow.Value;

                var startWindowServerTime = startWindow.ToUniversalTime().ToString(@"HH\:mm");
                var endWindowServerTime = endWindow.ToUniversalTime().ToString(@"HH\:mm");
                var startWindowLocal = startWindow.ToLocalTime().ToString(@"HH\:mm");
                var endWindowLocal = endWindow.ToLocalTime().ToString(@"HH\:mm");

                ImGui.SameLine();
                ImGui.Text(
                    $"Current bonus light window: {startWindowLocal} - {endWindowLocal} ({startWindowServerTime} - {endWindowServerTime} Server Time)");
            }
            else
            {
                ImGui.SameLine();
                ImGui.Text($"Bonus light window could not be found, this is a bug.");
            }

            foreach (var territoryId in BonusConfiguration.ActiveBonus)
            {
                var dutyName = BonusLightDuty.GetValue(territoryId).DutyName
                    .Replace("Œ", "Oe")
                    .Replace("œ", "oe");
                ImGui.Text($"\"{dutyName}\"");
            }
        }
    }
}
