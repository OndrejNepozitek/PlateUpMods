using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using UnityEngine;
using RandomExtensions = Kitchen.RandomExtensions;

namespace ONe.KitchenDesigner.KitchenDesigns;

[HarmonyPatch(typeof(KitchenDecorator), nameof(KitchenDecorator.Decorate))]
public class KitchenDecoratorPatch
{
  [HarmonyPrefix]
  public static bool Decorate(Room room, LayoutProfile ___Profile, LayoutBlueprint ___Blueprint, List<CLayoutAppliancePlacement> ___Decorations, ref bool __result)
  {
    if (!KitchenDesignLoader.ShouldPatchDecorations)
    {
      return true;
    }

    KitchenDesignLoader.ShouldPatchDecorations = false;
    
    Queue<GameDataObject> gameDataObjectQueue = new Queue<GameDataObject>((IEnumerable<GameDataObject>) ___Profile.RequiredAppliances);
    List<Vector3> used_tiles = new List<Vector3>(___Decorations.Select<CLayoutAppliancePlacement, Vector3>((Func<CLayoutAppliancePlacement, Vector3>) (d => d.Position)));
    HashSet<LayoutPosition> room_tiles = ___Blueprint.TilesOfRoom(room);

    // Correct placement - no features, etc
    // Catch exceptions
    try
    {
      LayoutPosition layoutPosition1 = next_tile(RandomExtensions.Random<LayoutPosition>(room_tiles
        .Where<LayoutPosition>((Func<LayoutPosition, bool>)(x => !used_tiles.Contains((Vector3)x)))
        .ToList<LayoutPosition>()));
      while (gameDataObjectQueue.Count > 0)
      {
        ___Decorations.Add(new CLayoutAppliancePlacement()
        {
          Appliance = gameDataObjectQueue.Dequeue().ID,
          Position = (Vector3)layoutPosition1,
          Rotation = FindWallRotation(layoutPosition1, ___Blueprint)
        });
        layoutPosition1 = next_tile(layoutPosition1);
      }
    }
    catch (Exception)
    {
      
    }

    // Incorrect placement - hatch feature still valid
    LayoutPosition layoutPosition2 = next_tile_hatch(RandomExtensions.Random<LayoutPosition>(room_tiles
      .Where<LayoutPosition>((Func<LayoutPosition, bool>)(x => !used_tiles.Contains((Vector3)x)))
      .ToList<LayoutPosition>()));
    while (gameDataObjectQueue.Count > 0)
    {
      ___Decorations.Add(new CLayoutAppliancePlacement()
      {
        Appliance = gameDataObjectQueue.Dequeue().ID,
        Position = (Vector3)layoutPosition2,
        Rotation = FindWallRotation(layoutPosition2, ___Blueprint)
      });
      layoutPosition2 = next_tile_hatch(layoutPosition2);
    }

    // Handle hatches, make sure that that tile is not already used
    foreach (LayoutPosition layoutPosition in room_tiles)
    {
      if (used_tiles.Contains(layoutPosition))
      {
        continue;
      }
      
      if (___Blueprint.HasFeature(layoutPosition, FeatureType.Hatch))
      {
        ___Decorations.Add(new CLayoutAppliancePlacement()
        {
          Appliance = ___Profile.Counter.ID,
          Position = (Vector3) layoutPosition,
          Rotation = FindWallRotation(layoutPosition, ___Blueprint)
        });
        used_tiles.Add((Vector3) layoutPosition);
      }
    }

    __result = true;
    return false;

    LayoutPosition next_tile(LayoutPosition tile)
    {
      foreach (LayoutPosition tile1 in ___Blueprint.AdjacentInRoom(tile).Concat<LayoutPosition>((IEnumerable<LayoutPosition>) room_tiles))
      {
        if (___Blueprint.IsTileAccessible(tile1) && !___Blueprint.HasFeature(tile1) && ___Blueprint.IsTileFlatWall(tile1) && !used_tiles.Contains((Vector3) tile1))
        {
          used_tiles.Add((Vector3) tile1);
          return tile1;
        }
      }
      throw new LayoutFailureException("Not enough spaces to place kitchen equipment");
    }
    
    LayoutPosition next_tile_hatch(LayoutPosition tile)
    {
      foreach (LayoutPosition tile1 in ___Blueprint.AdjacentInRoom(tile).Concat<LayoutPosition>((IEnumerable<LayoutPosition>) room_tiles))
      {
        if (___Blueprint.IsTileAccessible(tile1) && (!___Blueprint.HasFeature(tile1) || ___Blueprint.HasFeature(tile1, FeatureType.Hatch)) && ___Blueprint.IsTileFlatWall(tile1) && !used_tiles.Contains((Vector3) tile1))
        {
          used_tiles.Add((Vector3) tile1);
          return tile1;
        }
      }
      throw new LayoutFailureException("Not enough spaces to place kitchen equipment");
    }
  }

  private static Quaternion FindWallRotation(LayoutPosition pos, LayoutBlueprint ___Blueprint)
  {
    Room room = ___Blueprint[pos];
    foreach (LayoutPosition direction in LayoutHelpers.Directions)
    {
      if (___Blueprint[direction + pos].ID != room.ID)
        return Quaternion.LookRotation(new Vector3((float) direction.x, 0.0f, (float) direction.y), Vector3.up);
    }
    return Quaternion.identity;
  }
}
