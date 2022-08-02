using System;
using System.Reflection;
using HarmonyLib;
using MelonLoader;

namespace KikyoMod;

public class FeatureDisabled : Attribute
{
}

internal class FeatureComponent
{
    protected static readonly HarmonyLib.Harmony Harmony = new("KikyoMod");

    public virtual string FeatureName => "FeatureName";

    public virtual string OriginalAuthor => "OriginalAuthor";

    protected HarmonyMethod GetLocalPatch(string method)
    {
        return GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();
    }
}