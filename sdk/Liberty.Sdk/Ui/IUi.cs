namespace Liberty.Sdk
{
    // Liberty UI: GTA IV-style help boxes, notifications, subtitles, list and radial menus, textures and custom
    // drawing (LibertyModule.OnDraw). Menus capture input while open; everything a module opened closes when it stops.
    public interface IUi
    {
        TextureRef LoadTexture(string path);
        TextureRef LoadTexture(byte[] png, string cacheKey);
        // The game's own HUD icon of a weapon (extracted at install time); None when missing.
        TextureRef WeaponIcon(int weapon);
        void ShowHelp(LibertyModule owner, string text, int durationMs);
        void ClearHelp(LibertyModule owner);
        void Notify(string text, int durationMs);
        void Subtitle(string text, int durationMs);
        IMenu OpenList(LibertyModule owner, ListMenu menu);
        IMenu OpenRadial(LibertyModule owner, RadialMenu menu);
        bool AnyMenuOpen { get; }
        // Hides the game HUD and radar while the module wants (screenshots, cutscenes); restored when it stops.
        void SetHudVisible(LibertyModule owner, bool visible);
    }
}