using System.Text.Json;

namespace LeagueHelper
{
    internal partial class Bot
    {
        private static readonly string[] Roles = { "UNSELECTED", "TOP", "JUNGLE", "MIDDLE", "BOTTOM", "UTILITY", "FILL" };

        private string LastInvites { get; set; } = "";

        public string GetLobby() => Client == null ? "{}" : Client.Get("/lol-lobby/v2/lobby");
        public string GetQueues() => Client == null ? "[]" : Client.Get("/lol-game-queues/v1/queues");
        public string GetFriends() => Client == null ? "[]" : Client.Get("/lol-chat/v1/friends");
        public string GetInvites() => Client == null ? "[]" : Client.Get("/lol-lobby/v2/received-invitations");
        public string GetSearch() => Client == null ? "{}" : Client.Get("/lol-matchmaking/v1/search");

        public string CreateLobby(int queueId)
        {
            return LobbyPost("/lol-lobby/v2/lobby", "{\"queueId\":" + queueId + "}");
        }

        public string LeaveLobby()
        {
            if (Client == null)
                return "League isn't open.";
            Client.Delete("/lol-lobby/v2/lobby");
            return "ok";
        }

        public string StartSearch()
        {
            return LobbyPost("/lol-lobby/v2/lobby/matchmaking/search", "");
        }

        public string CancelSearch()
        {
            if (Client == null)
                return "League isn't open.";
            Client.Delete("/lol-lobby/v2/lobby/matchmaking/search");
            return "ok";
        }

        public string Invite(long summonerId)
        {
            return LobbyPost("/lol-lobby/v2/lobby/invitations", "[{\"toSummonerId\":" + summonerId + "}]");
        }

        public string Kick(long summonerId)
        {
            return LobbyPost("/lol-lobby/v2/lobby/members/" + summonerId + "/kick", "");
        }

        public string AnswerInvite(string id, bool accept)
        {
            string verb = accept ? "/accept" : "/decline";
            return LobbyPost("/lol-lobby/v2/received-invitations/" + Uri.EscapeDataString(id) + verb, "");
        }

        public string SetRoles(string first, string second)
        {
            if (Client == null)
                return "League isn't open.";
            if (!Roles.Contains(first) || !Roles.Contains(second))
                return "Unknown role.";

            string json = "{\"firstPreference\":\"" + first + "\",\"secondPreference\":\"" + second + "\"}";
            Client.Put("/lol-lobby/v2/lobby/members/localMember/position-preferences", json);
            return "ok";
        }

        private string LobbyPost(string url, string json)
        {
            if (Client == null)
                return "League isn't open.";

            string result = Client.Post(url, json);
            if (result.Contains("\"errorCode\""))
                return "League said: " + ErrorMessage(result);
            return "ok";
        }

        private void CheckInvites(LeagueClient client)
        {
            using JsonDocument doc = JsonDocument.Parse(client.Get("/lol-lobby/v2/received-invitations"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return;

            var ids = new List<string>();
            foreach (JsonElement invite in doc.RootElement.EnumerateArray())
            {
                if (invite.TryGetProperty("state", out JsonElement state) && state.GetString() == "Pending"
                    && invite.TryGetProperty("invitationId", out JsonElement id))
                {
                    ids.Add(id.GetString() ?? "");
                }
            }

            string key = string.Join(",", ids);
            if (ids.Count > 0 && key != LastInvites)
                Notify("Party invite!", "A friend invited you to their lobby", false);
            LastInvites = key;
        }
    }
}
