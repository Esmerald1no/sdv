﻿namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntUpdateTickedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        State.ProspectorHunt!.Update(e.Ticks);
        if (Game1.player.HasProfession(Profession.Prospector, true))
        {
            Game1.gameTimeInterval = 0;
        }
    }
}
