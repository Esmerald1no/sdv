﻿namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[LimitEvent]
[UsedImplicitly]
internal sealed class LimitInputUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LimitInputUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitInputUpdateTickedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!Config.Masteries.LimitBreakKey.IsDown())
        {
            this.Disable();
            return;
        }

        State.LimitBreak!.UpdateInput();
    }
}
