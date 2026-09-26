using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Liberty SDK")]
[assembly: AssemblyDescription("Public API for Liberty Framework mods (GTA IV: The Complete Edition 1.2.0.59)")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
// Only the engine may drive module lifecycles and install itself as the host.
[assembly: InternalsVisibleTo("LibertyFramework.net")]