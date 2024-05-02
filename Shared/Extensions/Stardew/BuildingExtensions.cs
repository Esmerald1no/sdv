﻿namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Building"/> class.</summary>
public static class BuildingExtensions
{
    /// <summary>Gets the <see cref="Farmer"/> instance who owns this <paramref name="building"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who constructed the <paramref name="building"/>, or the host of the game session if not found.</returns>
    public static Farmer GetOwner(this Building building)
    {
        return Game1.getFarmerMaybeOffline(building.owner.Value) ?? Game1.MasterPlayer;
    }

    /// <summary>Checks whether the <paramref name="building"/> is owned by the specified <see cref="Farmer"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="building"/>'s owned value is equal to the unique ID of the <paramref name="farmer"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsOwnedBy(this Building building, Farmer farmer)
    {
        return building.owner.Value == farmer.UniqueMultiplayerID;
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and the target <paramref name="tile"/>.
    /// </summary>
    /// <param name="building">The <see cref="Character"/>.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The tile distance between <paramref name="building"/> and the <paramref name="tile"/>.</returns>
    public static double DistanceTo(this Building building, Vector2 tile)
    {
        return (new Vector2(building.tileX.Value, building.tileY.Value) - tile).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and some <paramref name="other"/> in the
    ///     same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="other">The target <see cref="Building"/>.</param>
    /// <returns>The tile distance between <paramref name="building"/> and <paramref name="other"/>.</returns>
    public static double DistanceTo(this Building building, Building other)
    {
        return building.DistanceTo(new Vector2(other.tileX.Value, other.tileY.Value));
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and the target <paramref name="character"/>
    ///     in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="character">The target <see cref="Character"/>.</param>
    /// <returns>The tile distance between <paramref name="building"/> and <paramref name="character"/>.</returns>
    public static double DistanceTo(this Building building, Character character)
    {
        return building.DistanceTo(character.Tile);
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and the target <paramref name="obj"/> in
    ///     the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="obj">The target <see cref="SObject"/>.</param>
    /// <returns>The tile distance between <paramref name="building"/> and <paramref name="obj"/>.</returns>
    public static double DistanceTo(this Building building, SObject obj)
    {
        return building.DistanceTo(obj.TileLocation);
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and the target
    ///     <paramref name="terrainFeature"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="terrainFeature">The target <see cref="TerrainFeature"/>.</param>
    /// <returns>The tile distance between <paramref name="building"/> and <paramref name="terrainFeature"/>.</returns>
    public static double DistanceTo(this Building building, TerrainFeature terrainFeature)
    {
        return building.DistanceTo(terrainFeature.Tile);
    }

    /// <summary>
    ///     Finds the closest target from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="building"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the tile coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="building"/>.</returns>
    public static T? GetClosest<T>(
        this Building building,
        IEnumerable<T> candidates,
        Func<T, Vector2> getPosition,
        out double distance,
        Func<T, bool>? predicate = null)
        where T : class
    {
        predicate ??= _ => true;
        candidates = candidates.Where(c => predicate(c));
        T? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(getPosition(candidate));
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        distance = distanceToClosest;
        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="Building"/> to this one in the current <see cref="GameLocation"/>, and of the
    ///     specified sub-type.
    /// </summary>
    /// <typeparam name="TBuilding">A sub-type of <see cref="Building"/>.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    /// </remarks>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this Building building,
        IEnumerable<TBuilding>? candidates = null,
        Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        predicate ??= _ => true;
        candidates ??= building.GetParentLocation().buildings
            .OfType<TBuilding>()
            .Where(t => predicate(t));
        TBuilding? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="Character"/> of sub-type <typeparamref name="TCharacter"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TCharacter">A sub-type of <see cref="Character"/>.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Character"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    /// </remarks>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this Building building,
        IEnumerable<TCharacter>? candidates = null,
        Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().characters
            .OfType<TCharacter>()
            .Where(t => predicate(t));
        TCharacter? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="Farmer"/> to this <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Character"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Farmer"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    ///     This version is required as <see cref="Farmer"/> references are stored in a different field of <see cref="GameLocation"/>.
    /// </remarks>
    public static Farmer? GetClosestFarmer(
        this Building building,
        IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().farmers
            .Where(t => predicate(t));
        Farmer? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="SObject"/> of sub-type <typeparamref name="TObject"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TObject">A sub-type of <see cref="SObject"/>.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="SObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    /// </remarks>
    public static TObject? GetClosestObject<TObject>(
        this Building building,
        IEnumerable<TObject>? candidates = null,
        Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().Objects.Values
            .OfType<TObject>()
            .Where(o => predicate(o));
        TObject? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Find the closest <see cref="TerrainFeature"/> of sub-type <typeparamref name="TTerrainFeature"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A sub-type of <see cref="TerrainFeature"/>.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="TerrainFeature"/>, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    /// </remarks>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this Building building,
        IEnumerable<TTerrainFeature>? candidates = null,
        Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().terrainFeatures.Values
            .OfType<TTerrainFeature>()
            .Where(t => predicate(t));
        TTerrainFeature? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }
}
