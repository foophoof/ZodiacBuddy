using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ZodiacBuddy.BonusLight;
using ZodiacBuddy.Stages.Atma;
using ZodiacBuddy.Stages.Brave;
using ZodiacBuddy.Stages.Novus;

namespace ZodiacBuddy;

/// <summary>
///     Main plugin implementation.
/// </summary>
public sealed class ZodiacBuddyPlugin : IDalamudPlugin
{
    private const string Command = "/pzodiac";

    private readonly AtmaManager animusBuddy;
    private readonly BraveManager braveManager;
    private readonly ConfigWindow configWindow;
    private readonly NovusManager novusManager;

    private readonly WindowSystem windowSystem;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ZodiacBuddyPlugin" /> class.
    /// </summary>
    /// <param name="pluginInterface">Dalamud plugin interface.</param>
    public ZodiacBuddyPlugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        Service.Plugin = this;
        Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();

        windowSystem = new WindowSystem("ZodiacBuddy");
        windowSystem.AddWindow(configWindow = new ConfigWindow());

        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;

        Service.CommandManager.AddHandler(Command,
            new CommandInfo(OnCommand) {HelpMessage = "Open a window to edit various settings.", ShowInHelp = true});

        Service.BonusLightManager = new BonusLightManager();
        animusBuddy = new AtmaManager();
        novusManager = new NovusManager();
        braveManager = new BraveManager();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(Command);

        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;

        animusBuddy.Dispose();
        novusManager.Dispose();
        braveManager.Dispose();
        Service.BonusLightManager.Dispose();
    }

    /// <summary>
    ///     Print a message.
    /// </summary>
    /// <param name="message">Message to send.</param>
    public void PrintMessage(SeString message)
    {
        var sb = new SeStringBuilder()
            .AddUiForeground("[ZodiacBuddy] ", 45)
            .Append(message);

        Service.ChatGui.Print(new XivChatEntry {Type = Service.Configuration.ChatType, Message = sb.BuiltString});
    }

    /// <summary>
    ///     Print an error message.
    /// </summary>
    /// <param name="message">Message to send.</param>
    public static void PrintError(string message)
    {
        Service.ChatGui.PrintError($"[ZodiacBuddy] {message}");
    }

    private void OnOpenConfigUi()
    {
        configWindow.IsOpen = true;
    }

    private void OnCommand(string command, string arguments)
    {
        configWindow.IsOpen = true;
    }
}
