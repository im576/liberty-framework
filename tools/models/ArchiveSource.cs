using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;

namespace LibertyFramework.Models
{
    // One read API over IMG and RPF archives, opened relative to the game directory.
    internal sealed class ArchiveSource
    {
        internal readonly List<string> Names;
        private readonly Func<string, byte[]> extract;

        private ArchiveSource(List<string> names, Func<string, byte[]> extract) { Names = names; this.extract = extract; }

        internal byte[] Extract(string name) { return extract(name); }

        internal static ArchiveSource Open(string game, string archive)
        {
            string path = Path.IsPathRooted(archive) ? archive : Path.Combine(game, archive);
            byte[] key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe"));
            if (path.EndsWith(".rpf", StringComparison.OrdinalIgnoreCase))
            {
                RpfArchive rpf = RpfArchive.Open(path, key);
                return new ArchiveSource(rpf.Names.ToList(), rpf.Extract);
            }
            ImgArchive img = ImgArchive.Open(path, key);
            return new ArchiveSource(img.Entries.Select(e => e.Name).ToList(), img.Extract);
        }
    }
}
