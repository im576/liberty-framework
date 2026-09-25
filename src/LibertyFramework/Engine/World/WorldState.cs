using System.Collections.Generic;

namespace LibertyFramework.Engine.World
{
    // The engine's per-frame view of the game (ADR-0006). Built once per frame by the native core (or the SHDN
    // fallback); modules read it instead of calling natives for the same facts.
    public sealed class WorldState
    {
        private readonly List<PedState> peds = new List<PedState>(128);
        private readonly Dictionary<int, int> pedIndex = new Dictionary<int, int>(128);

        public int Frame { get; internal set; }
        public bool FromCore { get; internal set; }
        public bool HasPlayer { get; internal set; }
        public bool HasWorld { get; internal set; }
        public bool HasPeds { get; internal set; }
        public PlayerState Player { get; internal set; }
        public WorldInfo Info { get; internal set; }
        public IReadOnlyList<PedState> Peds { get { return peds; } }
        public float CoreMicroseconds { get; internal set; }

        public bool TryGetPed(int handle, out PedState ped)
        {
            int index;
            if (pedIndex.TryGetValue(handle, out index)) { ped = peds[index]; return true; }
            ped = default(PedState);
            return false;
        }

        internal void ClearPeds() { peds.Clear(); pedIndex.Clear(); }

        internal void AddPed(PedState ped)
        {
            pedIndex[ped.Handle] = peds.Count;
            peds.Add(ped);
        }
    }
}