﻿namespace DaLion.Professions.Framework.Events.Multiplayer.ModMessageReceived;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class LimitToggledModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LimitToggledModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitToggledModMessageReceivedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != UniqueId || e.Type != "ToggledLimitBreak")
        {
            return;
        }

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} has toggled their LimitBreak ability.");
            return;
        }

        var limitState = e.ReadAs<string>();
        switch (limitState)
        {
            case "Active":
                var id = Data.Read(who, DataKeys.LimitBreakId);
                var limit = LimitBreak.FromName(id);
                Log.D($"{who.Name} activated {limit.Name}.");
                who.startGlowing(limit.Color, false, 0.05f);
                break;

            case "Inactive":
                Log.D($"{who.Name}'s LimitBreak has ended.");
                who.stopGlowing();
                break;
        }
    }
}
