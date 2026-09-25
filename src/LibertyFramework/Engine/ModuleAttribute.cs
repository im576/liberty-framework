using System;

namespace LibertyFramework.Engine
{
    // Marks a Module for discovery. Id names its log lines and state; lower Order starts and ticks first.
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string id) { Id = id; }
        public string Id { get; private set; }
        public int Order { get; set; }
    }
}