using System.Runtime.CompilerServices;

namespace GlowCare.Tests;

internal static class TestBootstrapper
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        string? current = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(current))
        {
            bool hasSolution = File.Exists(Path.Combine(current, "GlowCare.sln"));
            bool hasEntities = Directory.Exists(Path.Combine(current, "GlowCare.Entities"));
            bool hasWebProject = Directory.Exists(Path.Combine(current, "GlowCare"));

            if (hasSolution && hasEntities && hasWebProject)
            {
                string desiredCurrentDirectory = Path.Combine(current, "GlowCare");
                Directory.SetCurrentDirectory(desiredCurrentDirectory);
                return;
            }

            DirectoryInfo? parent = Directory.GetParent(current);
            current = parent?.FullName;
        }
    }
}
