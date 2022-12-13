using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kitchen.Layouts;
using Kitchen.Layouts.Features;
using KitchenData;

namespace ONe.KitchenDesigner.KitchenDesigns.Decoders;

public static class V1Decoder
{
    public static KitchenDesign Load(string encoded)
    {
        var decoded = DecoderUtils.Base64UrlDecode(encoded.Substring(1));
        var sections = decoded.Split(':');
        
        // Layout profile
        var layoutProfileIndex = int.Parse(sections[0]);
        var layoutProfile = GetLayoutProfile(layoutProfileIndex);
        
        // Restaurant setting
        var restaurantSettingIndex = int.Parse(sections[1]);
        var restaurantSetting = GetRestaurantSetting(restaurantSettingIndex);
        
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

    private static LayoutProfile GetLayoutProfile(int index)
    {
        var indexToIdMapping = new Dictionary<int, int>()
        {
            { 1, 222370461 }, // Basic
            { 2, -80202533 }, // Diner
            { 3, -2045800810 }, // Medium
            { 4, 557943155 }, // Extended
            { 5, 154938708 }, // Huge
        };

        if (!indexToIdMapping.TryGetValue(index, out var id))
        {
            throw new InvalidOperationException($"Invalid layout profile index {index}");
        }

        if (!GameData.Main.TryGet<LayoutProfile>(id, out var profile))
        {
            throw new InvalidOperationException($"Layout profile with id {id} not found");
        }

        return profile;
    }
    
    private static RestaurantSetting GetRestaurantSetting(int index)
    {
        if (index == 0)
        {
            var random = new Random();
            index = random.Next(1, 4);
        }
        
        var indexToIdMapping = new Dictionary<int, int>()
        {
            { 1, 2002876295 }, // City
            { 2, 447437163 }, // Country
            { 3, -1864906012 }, // Alpine
        };

        if (!indexToIdMapping.TryGetValue(index, out var id))
        {
            throw new InvalidOperationException($"Invalid restaurant setting index {index}");
        }

        if (!GameData.Main.TryGet<RestaurantSetting>(id, out var setting))
        {
            throw new InvalidOperationException($"Restaurant setting with id {id} not found");
        }

        return setting;
    }
}