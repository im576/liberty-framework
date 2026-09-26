using System.Collections;

namespace Liberty.Sdk
{
    // Coroutines: write sequences as straight-line code that yields Wait objects (null = next frame). A coroutine
    // stops with its owning module; Cancel stops it early.
    public interface IScheduler
    {
        ICoroutine Start(LibertyModule owner, string name, IEnumerator routine);
        int Running { get; }
    }

}
