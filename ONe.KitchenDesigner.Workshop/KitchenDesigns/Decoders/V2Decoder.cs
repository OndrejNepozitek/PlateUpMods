using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kitchen;
using Kitchen.Layouts;
using Kitchen.Layouts.Features;
using KitchenData;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = System.Random;

namespace ONe.KitchenDesigner.KitchenDesigns.Decoders;

public static class V2Decoder
{
    private static Random _random = new Random();
    
    public static KitchenDesign Load(string encoded)
    {
        var decoded = DecoderUtils.Base64UrlDecode(encoded.Substring(1));
        var sections = decoded.Split(':');
        
        // Layout profile
        var layoutProfileIndex = int.Parse(sections[0]);
        var layoutProfile = GetLayoutProfile(layoutProfileIndex);
        
        // Restaurant setting
        var restaurantSettingIndex = int.Parse(sections[1]);
        var restaurantSetting = restaurantSettingIndex == 0 
            ? GetRandomRestaurantSetting()
            : GetRestaurantSetting(restaurantSettingIndex);
        
        // Construct layout blueprint
        var layout = new LayoutBlueprint();
        
        // Load rooms
        var rooms = sections[2].Split(';').Select(x =>
        {
            var split = x.Split(',');
            var id = int.Parse(split[0]);
            var roomType = (RoomType) int.Parse(split[1]);
            return new Room(id, roomType);
        }).ToList();

        // Load layout size
        var sizes = sections[3].Split(',');
        var sizeX = int.Parse(sizes[0]);
        var sizeY = int.Parse(sizes[1]);
        
        // Tiles
        var roomSections = sections[4].Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).ToList();

        var counter = 0;
        for (var i = 0; i < roomSections.Count; i++)
        {
            var roomSection = roomSections[i];
            if (!roomSection.Contains("."))
            {
                roomSection += ".1";
            }

            var parts = roomSection.Split('.').Select(int.Parse).ToList();
            var roomId = parts[0];
            var repeat = parts[1];
            var room = rooms.SingleOrDefault(x => x.ID == roomId);

            for (int j = 0; j < repeat; j++)
            {            
                var y = counter % sizeY;
                var x = counter / sizeY;

                if (roomId != 0)
                {
                    layout.Tiles.Add(new LayoutPosition(x, y), room);
                }

                counter++;
            }
        }
        
        // Features
        layout.Features = new List<Feature>();
        var features = sections[5].Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (var featureString in features)
        {
            var featureSplit = featureString.Split(',');
            var from = new LayoutPosition(int.Parse(featureSplit[0]), int.Parse(featureSplit[1]));
            var to = new LayoutPosition(int.Parse(featureSplit[2]), int.Parse(featureSplit[3]));
            var featureType = (FeatureType)int.Parse(featureSplit[4]);
            var feature = new Feature(from, to, featureType);

            if (featureType == FeatureType.FrontDoor && from.y < to.y)
            {
                feature = new Feature(to, from, featureType);
            }
            
            layout.Features.Add(feature);
        }

        return new KitchenDesign(layout, layoutProfile, restaurantSetting);
    }

    private static LayoutProfile GetLayoutProfile(int id)
    {
        if (!GameData.Main.TryGet<LayoutProfile>(id, out var profile))
        {
            throw new InvalidOperationException($"Layout profile with id {id} not found");
        }

        return profile;
    }

    private static RestaurantSetting GetRandomRestaurantSetting()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        using var settingUpgrades = entityManager
            .CreateEntityQuery((ComponentType) typeof (CSettingUpgrade))
            .ToComponentDataArray<CSettingUpgrade>(Allocator.Temp);

        if (settingUpgrades.Length != 0)
        {
            var randomIndex = _random.Next(settingUpgrades.Length);
            var settingId = settingUpgrades[randomIndex].SettingID;
            return GameData.Main.Get<RestaurantSetting>(settingId);
        }

        {
            var knownSettingIds = new List<int>()
            {
                2002876295,
                447437163,
                -1864906012
            };
            
            var randomIndex = _random.Next(knownSettingIds.Count);
            var settingId = knownSettingIds[randomIndex];
            return GameData.Main.Get<RestaurantSetting>(settingId);
        }
    }
    
    private static RestaurantSetting GetRestaurantSetting(int id)
    {
        if (!GameData.Main.TryGet<RestaurantSetting>(id, out var setting))
        {
            throw new InvalidOperationException($"Restaurant setting with id {id} not found");
        }

        return setting;
    }
}