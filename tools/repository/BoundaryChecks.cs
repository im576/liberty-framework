using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class BoundaryChecks
{
    private static int passed, failed;
    private static void Check(string name, bool value)
    {
        if (value) { passed++; Console.WriteLine("PASS " + name); }
        else { failed++; Console.WriteLine("FAIL " + name); }
    }
    private static string[] Modules(Assembly assembly)
    {
        List<string> ids = new List<string>();
        foreach (Type type in assembly.GetTypes())
        {
            foreach (CustomAttributeData attribute in CustomAttributeData.GetCustomAttributes(type))
            {
                if (attribute.AttributeType.FullName == "Liberty.Sdk.ModuleAttribute")
                {
                    ids.Add((string)attribute.ConstructorArguments[0].Value);
                    Check("module remains discoverable: " + type.FullName, !type.IsAbstract && type.IsPublic);
                }
            }
        }
        return ids.OrderBy(x => x).ToArray();
    }
    private static int Main(string[] args)
    {
        if (args.Length != 4) { return 2; }
        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += delegate(object sender, ResolveEventArgs e)
        { return Assembly.ReflectionOnlyLoad(e.Name); };
        Assembly.ReflectionOnlyLoadFrom(args[0]); // SDK
        Assembly.ReflectionOnlyLoadFrom(args[3]); // SHDN, metadata only
        Assembly engine = Assembly.ReflectionOnlyLoadFrom(args[1]);
        Assembly mod = Assembly.ReflectionOnlyLoadFrom(args[2]);
        Check("framework has no LibertyPlus assembly dependency", !engine.GetReferencedAssemblies().Any(x => x.Name == "LibertyPlus"));
        Check("mod references framework", mod.GetReferencedAssemblies().Any(x => x.Name == "LibertyFramework.net"));
        Check("mod references shared SDK", mod.GetReferencedAssemblies().Any(x => x.Name == "Liberty.Sdk"));
        Check("framework contains no showcase controller", !engine.GetTypes().Any(x => x.Name == "GunplayController" || x.Name == "ArsenalCore" || x.Name == "HudModule" || x.Name == "CombatEffectsController"));
        string[] engineIds = Modules(engine), modIds = Modules(mod);
        Check("framework only owns diagnostic probe", engineIds.SequenceEqual(new[] { "probe" }));
        Check("all nine preview module identities preserved", modIds.SequenceEqual(new[] {
            "arsenal", "atmosphere", "combat", "devtools", "gunplay", "holsters", "weapon-catalog", "weapon-probe", "weapon-wheel" }));
        Check("module identities do not overlap", !engineIds.Intersect(modIds).Any());
        Console.WriteLine("RESULT passed=" + passed + " failed=" + failed + " (metadata only; no game calls)");
        return failed == 0 ? 0 : 1;
    }
}
