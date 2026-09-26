namespace Liberty.Sdk
{
    // A clip in one of the game's animation dictionaries (anim.img), e.g. new AnimClip("amb@car_stash", "open_boot").
    public struct AnimClip
    {
        public readonly string Dictionary;
        public readonly string Name;
        public AnimClip(string dictionary, string name) { Dictionary = dictionary; Name = name; }
        public override string ToString() { return Dictionary + "/" + Name; }
    }
}