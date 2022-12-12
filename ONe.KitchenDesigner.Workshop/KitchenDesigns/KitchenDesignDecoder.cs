using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kitchen.Layouts;
using Kitchen.Layouts.Features;
using KitchenData;
using ONe.KitchenDesigner.KitchenDesigns.Decoders;
using ONe.KitchenDesigner.Workshop;
using UnityEngine;

namespace ONe.KitchenDesigner.KitchenDesigns;

/// <summary>
/// Loads kitchen design from its string representation.
/// </summary>
public static class KitchenDesignDecoder
{
    public static bool TryDecode(string encoded, out KitchenDesign kitchenDesign, out string message)
    {
        try
        {
            var version = encoded.Substring(0, 1);

            if (version is "1")
            {
                kitchenDesign = V1Decoder.Load(encoded);
                message = null;
                return true;
            } 
            
            if (version is "2")
            {
                kitchenDesign = V2Decoder.Load(encoded);
                message = null;
                return true;
            }
            
            throw new InvalidOperationException("Kitchen design version not recognized. Make sure that you have the newest version of the mod installed.");
        }
        catch (Exception e)
        {
            LogHelper.LogError($"Cannot decode kitchen design. Please make sure that you copied the text correctly.");
            LogHelper.LogError($"Error: {e.Message}");
            
            kitchenDesign = null;
            message = e.Message;
            return false;
        }
    }

    
}