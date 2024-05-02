﻿namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreDayEndingEvent(EventManager? manager = null)
        : base(manager ?? CoreMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        Reflector.NewCacheInterval();
    }
}
