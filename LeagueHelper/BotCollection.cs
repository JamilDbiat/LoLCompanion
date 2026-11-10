using System.Text.Json;

namespace LeagueHelper
{
    internal partial class Bot
    {
        private Dictionary<int, string> SkinRarities { get; set; } = new Dictionary<int, string>();
        private Dictionary<int, string> SkinNames { get; set; } = new Dictionary<int, string>();
        private Dictionary<int, string> SkinTiles { get; set; } = new Dictionary<int, string>();
        private HashSet<int> BaseSkins { get; set; } = new HashSet<int>();
        private Dictionary<int, (string Name, string Image, int Champ)> Chromas { get; set; } = new Dictionary<int, (string Name, string Image, int Champ)>();

        public string GetSummoner() => Client == null ? "{}" : Client.Get("/lol-summoner/v1/current-summoner");
        public string GetRanked() => Client == null ? "{}" : Client.Get("/lol-ranked/v1/current-ranked-stats");
        public string GetMastery() => Client == null ? "[]" : Client.Get("/lol-champion-mastery/v1/local-player/champion-mastery");

        public string GetWallet()
        {
            if (Client == null)
                return "{}";

            long rp = 0;
            long be = 0;
            using (JsonDocument wallet = JsonDocument.Parse(Client.Get("/lol-inventory/v1/wallet?currencyTypes=%5B%22RP%22,%22lol_blue_essence%22%5D")))
            {
                if (wallet.RootElement.TryGetProperty("RP", out JsonElement r))
                    rp = r.GetInt64();
                if (wallet.RootElement.TryGetProperty("lol_blue_essence", out JsonElement b))
                    be = b.GetInt64();
            }

            long oe = 0;
            long me = 0;
            long keys = 0;
            long fragments = 0;
            using (JsonDocument loot = JsonDocument.Parse(Client.Get("/lol-loot/v1/player-loot")))
            {
                if (loot.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement item in loot.RootElement.EnumerateArray())
                    {
                        string id = Text(item, "lootId");
                        long count = item.TryGetProperty("count", out JsonElement c) ? c.GetInt64() : 0;
                        if (id == "CURRENCY_cosmetic") oe = count;
                        if (id == "CURRENCY_mythic") me = count;
                        if (id == "MATERIAL_key") keys = count;
                        if (id == "MATERIAL_key_fragment") fragments = count;
                    }
                }
            }

            return JsonSerializer.Serialize(new { rp, be, oe, me, keys, fragments });
        }

        public string GetOwnedSkins()
        {
            if (Client == null)
                return "[]";

            LoadSkinData();
            var list = new List<object>();

            foreach (int id in OwnedIds("CHAMPION_SKIN"))
            {
                if (SkinNames.ContainsKey(id))
                {
                    if (BaseSkins.Contains(id))
                        continue;
                    list.Add(new
                    {
                        id,
                        champ = id / 1000,
                        name = SkinNames[id],
                        image = SkinTiles.GetValueOrDefault(id, ""),
                        rarity = SkinRarities.GetValueOrDefault(id, ""),
                        chroma = false
                    });
                }
                else if (Chromas.TryGetValue(id, out var chroma))
                {
                    list.Add(new
                    {
                        id,
                        champ = chroma.Champ,
                        name = chroma.Name,
                        image = chroma.Image,
                        rarity = "",
                        chroma = true
                    });
                }
            }

            return JsonSerializer.Serialize(list);
        }

        public string GetOwnedEmotes()
        {
            if (Client == null)
                return "[]";

            HashSet<int> owned = OwnedIds("EMOTE");
            var list = new List<object>();

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-game-data/assets/v1/summoner-emotes.json"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return "[]";

            foreach (JsonElement emote in doc.RootElement.EnumerateArray())
            {
                int id = emote.GetProperty("id").GetInt32();
                if (owned.Contains(id))
                    list.Add(new { id, name = Text(emote, "name"), image = Text(emote, "inventoryIcon") });
            }

            return JsonSerializer.Serialize(list);
        }

        public string SetIcon(int iconId)
        {
            if (Client == null)
                return "League isn't open.";
            if (!OwnedIds("SUMMONER_ICON").Contains(iconId))
                return "You don't own that icon.";

            Client.Put("/lol-summoner/v1/current-summoner/icon", "{\"profileIconId\":" + iconId + "}");
            return "ok";
        }

        public string GetOwnedIcons()
        {
            if (Client == null)
                return "[]";
            return JsonSerializer.Serialize(OwnedIds("SUMMONER_ICON").OrderByDescending(x => x));
        }

        public string GetItems()
        {
            if (Client == null)
                return "[]";

            var names = new HashSet<string>();
            var list = new List<object>();

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-game-data/assets/v1/items.json"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return "[]";

            foreach (JsonElement item in doc.RootElement.EnumerateArray())
            {
                string name = Text(item, "name");
                string icon = Text(item, "iconPath");
                int price = item.TryGetProperty("priceTotal", out JsonElement p) && p.ValueKind == JsonValueKind.Number ? p.GetInt32() : 0;
                if (name == "" || icon == "" || price <= 0 || !names.Add(name))
                    continue;
                list.Add(new { id = item.GetProperty("id").GetInt32(), name, image = icon, price });
            }

            return JsonSerializer.Serialize(list);
        }

        private long MySummonerId()
        {
            if (Client == null)
                return 0;

            using JsonDocument me = JsonDocument.Parse(Client.Get("/lol-summoner/v1/current-summoner"));
            return me.RootElement.TryGetProperty("summonerId", out JsonElement id) ? id.GetInt64() : 0;
        }

        private HashSet<int> OwnedIds(string inventoryType)
        {
            var ids = new HashSet<int>();
            if (Client == null)
                return ids;

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-inventory/v2/inventory/" + inventoryType));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return ids;

            foreach (JsonElement item in doc.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("owned", out JsonElement owned) && owned.GetBoolean())
                    ids.Add(item.GetProperty("itemId").GetInt32());
            }

            return ids;
        }

        private void LoadSkinData()
        {
            if (Client == null || SkinRarities.Count > 0)
                return;

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-game-data/assets/v1/skins.json"));
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return;

            foreach (JsonProperty entry in doc.RootElement.EnumerateObject())
            {
                if (!int.TryParse(entry.Name, out int id))
                    continue;
                SkinRarities[id] = Text(entry.Value, "rarity");
                SkinNames[id] = Text(entry.Value, "name");
                SkinTiles[id] = Text(entry.Value, "tilePath");

                if (entry.Value.TryGetProperty("isBase", out JsonElement isBase) && isBase.ValueKind == JsonValueKind.True)
                    BaseSkins.Add(id);

                if (entry.Value.TryGetProperty("chromas", out JsonElement chromas) && chromas.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement chroma in chromas.EnumerateArray())
                        Chromas[chroma.GetProperty("id").GetInt32()] = (Text(chroma, "name"), Text(chroma, "chromaPath"), id / 1000);
                }
            }
        }

        private static string Text(JsonElement element, string name)
        {
            return element.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String
                ? value.GetString() ?? ""
                : "";
        }
    }
}
