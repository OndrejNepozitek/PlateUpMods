using System;
using System.Text;

namespace ONe.KitchenDesigner.KitchenDesigns.Decoders;

public static class DecoderUtils
{
    public static string Base64UrlDecode(string base64)
    {
        string padded = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
    }
}