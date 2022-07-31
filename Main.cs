using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace KikyoMod;

internal static class ModInfo
{
    public const string Name = "KikyoMod";
    public const string Author = "abbey";
    public const string Version = "1.0.0.4";
}

public class Main : MelonMod
{
    private static readonly List<FeatureComponent> FeatureComponents = new();

    public override void OnApplicationStart()
    {
        InitializeFeatures();
    }

    //https://github.com/RequiDev/ReModCE/blob/85c7eacbad74e2fbdfac2b2a8d8d266c86a08fea/ReModCE/ReModCE.cs#L327
    private static void InitializeFeatures()
    {
        var assembly = Assembly.GetExecutingAssembly();
        IEnumerable<Type> types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException reflectionTypeLoadException)
        {
            types = reflectionTypeLoadException.Types.Where(t => t != null);
        }

        var loadableComponents = new List<LoadableComponent>();
        foreach (var t in types)
        {
            if (t.IsAbstract)
                continue;
            if (t.BaseType != typeof(FeatureComponent))
                continue;
            if (t.IsDefined(typeof(FeatureDisabled), false))
                continue;

            loadableComponents.Add(new LoadableComponent
            {
                Component = t
            });
        }

        foreach (var featureComp in loadableComponents)
        {
            var cType = featureComp.Component;

            try
            {
                if (Activator.CreateInstance(cType) is not FeatureComponent newFeatureComponent)
                    throw new NullReferenceException("newFeatureComponent is null");
#if DEBUG
                KikyoLogger.Msg(ConsoleColor.Cyan,
                    $"Loaded \"{newFeatureComponent.FeatureName}\" by {newFeatureComponent.OriginalAuthor}");
#endif
                FeatureComponents.Add(newFeatureComponent);
            }
            catch (Exception e)
            {
                KikyoLogger.Error($"Failed creating {cType.Name}:\n{e}");
            }
        }

        KikyoLogger.Msg($"Created {FeatureComponents.Count} feature components");
    }

    private class LoadableComponent
    {
        public Type Component;
    }
}