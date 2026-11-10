using System.Text.Json;

namespace LeagueHelper
{
    internal partial class Bot
    {
        private int LastUnread { get; set; } = -1;

        public string GetConversations() => Client == null ? "[]" : Client.Get("/lol-chat/v1/conversations");

        public string GetDirectMessages(string friendId)
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-chat/v1/conversations/" + Uri.EscapeDataString(friendId) + "/messages");
        }

        public string SendDirectMessage(string friendId, string text)
        {
            if (Client == null)
                return "League isn't open.";
            if (text.Trim() == "")
                return "ok";

            string url = "/lol-chat/v1/conversations/" + Uri.EscapeDataString(friendId) + "/messages";
            string message = JsonSerializer.Serialize(new { body = text, type = "chat" });
            string result = Client.Post(url, message);

            if (result.Contains("\"errorCode\""))
            {
                Client.Post("/lol-chat/v1/conversations", JsonSerializer.Serialize(new { id = friendId, type = "chat" }));
                result = Client.Post(url, message);
            }

            if (result.Contains("\"errorCode\""))
                return "League said: " + ErrorMessage(result);
            return "ok";
        }

        private void CheckMessages(LeagueClient client)
        {
            using JsonDocument doc = JsonDocument.Parse(client.Get("/lol-chat/v1/conversations"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return;

            int total = 0;
            string from = "";
            string body = "";
            string newest = "";

            foreach (JsonElement conversation in doc.RootElement.EnumerateArray())
            {
                if (!conversation.TryGetProperty("type", out JsonElement type) || type.GetString() != "chat")
                    continue;

                int count = conversation.TryGetProperty("unreadMessageCount", out JsonElement unread) ? unread.GetInt32() : 0;
                total += count;

                if (count > 0 && conversation.TryGetProperty("lastMessage", out JsonElement last) && last.ValueKind == JsonValueKind.Object)
                {
                    string time = last.TryGetProperty("timestamp", out JsonElement stamp) ? stamp.ToString() : "";
                    if (string.CompareOrdinal(time, newest) >= 0)
                    {
                        newest = time;
                        body = last.TryGetProperty("body", out JsonElement text) ? text.GetString() ?? "" : "";
                        from = conversation.TryGetProperty("gameName", out JsonElement name) ? name.GetString() ?? "" : "";
                        if (from == "" && conversation.TryGetProperty("name", out JsonElement altName))
                            from = altName.GetString() ?? "";
                    }
                }
            }

            if (LastUnread >= 0 && total > LastUnread && body != "")
                Notify("Message from " + (from == "" ? "a friend" : from), body, false);
            LastUnread = total;
        }
    }
}
