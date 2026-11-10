namespace LeagueHelper
{
    internal class PlayerSlot
    {
        public int ChampionId { get; set; }
        public int SkinId { get; set; }
        public string PositionPreference { get; set; } = "";
        public string Perks { get; set; } = "";
        public long Spell1 { get; set; }
        public long Spell2 { get; set; }
    }
}
