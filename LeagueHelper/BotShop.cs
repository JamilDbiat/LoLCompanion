using System.Text.Json;

namespace LeagueHelper
{
    internal partial class Bot
    {
        public string GetYourShop()
        {
            if (Client == null)
                return "[]";

            LoadSkinData();
            var list = new List<object>();

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-yourshop/v1/offers"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return "[]";

            foreach (JsonElement offer in doc.RootElement.EnumerateArray())
            {
                int skinId = offer.TryGetProperty("skinId", out JsonElement s) && s.ValueKind == JsonValueKind.Number ? s.GetInt32() : 0;
                list.Add(new
                {
                    id = Text(offer, "id"),
                    championId = offer.TryGetProperty("championId", out JsonElement c) && c.ValueKind == JsonValueKind.Number ? c.GetInt32() : 0,
                    skinId,
                    skinName = Text(offer, "skinName"),
                    originalPrice = offer.TryGetProperty("originalPrice", out JsonElement op) && op.ValueKind == JsonValueKind.Number ? op.GetInt32() : 0,
                    discountPrice = offer.TryGetProperty("discountPrice", out JsonElement dp) && dp.ValueKind == JsonValueKind.Number ? dp.GetInt32() : 0,
                    owned = offer.TryGetProperty("owned", out JsonElement ow) && ow.ValueKind == JsonValueKind.True,
                    revealed = offer.TryGetProperty("revealed", out JsonElement rv) && rv.ValueKind == JsonValueKind.True,
                    expirationDate = Text(offer, "expirationDate"),
                    image = SkinTiles.GetValueOrDefault(skinId, ""),
                    splash = SkinSplashes.GetValueOrDefault(skinId, ""),
                    load = SkinLoadScreens.GetValueOrDefault(skinId, ""),
                    rarity = SkinRarities.GetValueOrDefault(skinId, "")
                });
            }

            return JsonSerializer.Serialize(list);
        }

        public string RevealOffer(string offerId)
        {
            if (Client == null)
                return "League isn't open.";
            if (!SafeName.IsMatch(offerId))
                return "Invalid offer.";
            return ShopResult(Client.Post("/lol-yourshop/v1/offers/" + offerId + "/reveal", ""));
        }

        public string BuyOffer(string offerId)
        {
            if (Client == null)
                return "League isn't open.";
            if (!SafeName.IsMatch(offerId))
                return "Invalid offer.";
            StoreCache = "";
            return ShopResult(Client.Post("/lol-yourshop/v1/offers/" + offerId + "/purchase", ""));
        }

        public string GetChampionStore(int championId)
        {
            long summonerId = MySummonerId();
            if (Client == null || summonerId == 0)
                return "[]";

            LoadSkinData();
            var unowned = new List<int>();

            using (JsonDocument skins = JsonDocument.Parse(Client.Get("/lol-champions/v1/inventories/" + summonerId + "/champions/" + championId + "/skins")))
            {
                if (skins.RootElement.ValueKind != JsonValueKind.Array)
                    return "[]";

                foreach (JsonElement skin in skins.RootElement.EnumerateArray())
                {
                    bool isBase = skin.TryGetProperty("isBase", out JsonElement b) && b.GetBoolean();
                    bool owned = skin.TryGetProperty("ownership", out JsonElement o) && o.GetProperty("owned").GetBoolean();
                    if (!isBase && !owned)
                        unowned.Add(skin.GetProperty("id").GetInt32());
                }
            }

            if (unowned.Count == 0)
                return "[]";

            var list = new List<object>();
            string query = Uri.EscapeDataString(JsonSerializer.Serialize(unowned));

            using JsonDocument catalog = JsonDocument.Parse(Client.Get("/lol-store/v1/catalog/CHAMPION_SKIN?itemIds=" + query));
            if (catalog.RootElement.ValueKind != JsonValueKind.Array)
                return "[]";

            foreach (JsonElement item in catalog.RootElement.EnumerateArray())
            {
                int price = RpPrice(item);
                if (price <= 0 || !(item.TryGetProperty("active", out JsonElement active) && active.GetBoolean()))
                    continue;

                int id = item.GetProperty("itemId").GetInt32();
                list.Add(new
                {
                    id,
                    champ = championId,
                    name = SkinNames.GetValueOrDefault(id, "Skin"),
                    rarity = SkinRarities.GetValueOrDefault(id, ""),
                    image = SkinTiles.GetValueOrDefault(id, ""),
                    price
                });
            }

            return JsonSerializer.Serialize(list);
        }

        private string StoreCache { get; set; } = "";
        private DateTime StoreCacheTime { get; set; }

        public string GetSkinStore()
        {
            if (Client == null)
                return "[]";
            if (StoreCache != "" && DateTime.Now - StoreCacheTime < TimeSpan.FromMinutes(5))
                return StoreCache;

            LoadSkinData();
            HashSet<int> owned = OwnedIds("CHAMPION_SKIN");
            HashSet<int> champions = OwnedChampions();
            var list = new List<object>();

            using JsonDocument catalog = JsonDocument.Parse(Client.Get("/lol-store/v1/catalog"));
            if (catalog.RootElement.ValueKind != JsonValueKind.Array)
                return "[]";

            foreach (JsonElement item in catalog.RootElement.EnumerateArray())
            {
                if (Text(item, "inventoryType") != "CHAMPION_SKIN")
                    continue;
                if (!(item.TryGetProperty("active", out JsonElement active) && active.ValueKind == JsonValueKind.True))
                    continue;

                int id = item.GetProperty("itemId").GetInt32();
                if (owned.Contains(id) || BaseSkins.Contains(id) || !SkinNames.ContainsKey(id))
                    continue;

                int price = RpPrice(item);
                if (price <= 0)
                    continue;

                int champ = id / 1000;
                list.Add(new
                {
                    id,
                    champ,
                    name = SkinNames[id],
                    rarity = SkinRarities.GetValueOrDefault(id, ""),
                    image = SkinTiles.GetValueOrDefault(id, ""),
                    splash = SkinSplashes.GetValueOrDefault(id, ""),
                    load = SkinLoadScreens.GetValueOrDefault(id, ""),
                    price,
                    needChamp = !champions.Contains(champ)
                });
            }

            StoreCache = JsonSerializer.Serialize(list);
            StoreCacheTime = DateTime.Now;
            return StoreCache;
        }

        private HashSet<int> OwnedChampions()
        {
            var ids = new HashSet<int>();
            if (Client == null)
                return ids;

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-champions/v1/owned-champions-minimal"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return ids;

            foreach (JsonElement champ in doc.RootElement.EnumerateArray())
            {
                if (champ.TryGetProperty("ownership", out JsonElement ownership) && ownership.GetProperty("owned").GetBoolean())
                    ids.Add(champ.GetProperty("id").GetInt32());
            }

            return ids;
        }

        public string BuySkin(int skinId)
        {
            if (Client == null)
                return "League isn't open.";
            StoreCache = "";

            int price = 0;
            using (JsonDocument catalog = JsonDocument.Parse(Client.Get("/lol-store/v1/catalog/CHAMPION_SKIN?itemIds=%5B" + skinId + "%5D")))
            {
                if (catalog.RootElement.ValueKind == JsonValueKind.Array && catalog.RootElement.GetArrayLength() > 0)
                    price = RpPrice(catalog.RootElement[0]);
            }

            if (price <= 0)
                return "This skin can't be bought with RP right now.";

            string order = JsonSerializer.Serialize(new[] { new { inventoryType = "CHAMPION_SKIN", itemId = skinId, quantity = 1, rpCost = price } });
            return ShopResult(Client.Post("/lol-store/v3/purchase", order));
        }

        public string GetSanctum()
        {
            if (Client == null)
                return "[]";

            LoadSkinData();
            var list = new List<object>();

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-sanctum/v1/banners"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return "[]";

            foreach (JsonElement banner in doc.RootElement.EnumerateArray())
            {
                int skinId = 0;
                if (banner.TryGetProperty("bannerSkin", out JsonElement skin) && skin.TryGetProperty("id", out JsonElement sid))
                    skinId = sid.GetInt32();

                long pity = 0;
                if (banner.TryGetProperty("pityCounter", out JsonElement counter) && counter.TryGetProperty("amount", out JsonElement amount))
                    pity = amount.GetInt64();

                long end = banner.TryGetProperty("endDate", out JsonElement e) && e.ValueKind == JsonValueKind.Number ? e.GetInt64() : 0;
                int threshold = banner.TryGetProperty("chasePityThreshold", out JsonElement t) && t.ValueKind == JsonValueKind.Number ? t.GetInt32() : 0;
                string currency = banner.TryGetProperty("bannerCurrency", out JsonElement cur) ? Text(cur, "name") : "";

                list.Add(new
                {
                    skin = SkinNames.GetValueOrDefault(skinId, "Mythic skin"),
                    image = Text(banner, "bannerBackgroundTexture"),
                    pity,
                    threshold,
                    end,
                    currency
                });
            }

            return JsonSerializer.Serialize(list);
        }

        public string OpenRpStore()
        {
            if (Client == null)
                return "League isn't open.";
            Client.Post("/riotclient/ux-show", "");
            return "ok";
        }

        private static int RpPrice(JsonElement item)
        {
            if (!item.TryGetProperty("prices", out JsonElement prices) || prices.ValueKind != JsonValueKind.Array)
                return 0;

            foreach (JsonElement price in prices.EnumerateArray())
            {
                if (Text(price, "currency") == "RP" && price.TryGetProperty("cost", out JsonElement cost))
                    return cost.GetInt32();
            }

            return 0;
        }

        private string ShopResult(string result)
        {
            if (result.Contains("\"errorCode\""))
                return "League said: " + ErrorMessage(result);
            return "ok";
        }
    }
}
