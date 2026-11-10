using System.Net.Http.Headers;
using System.Text;

namespace LeagueHelper
{
    internal class LeagueClient
    {
        private HttpClient Http { get; set; }

        public LeagueClient(string lockfilePath)
        {
            string text;
            using (var stream = new FileStream(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
                text = reader.ReadToEnd();

            string[] parts = text.Split(':');
            string port = parts[2];
            string password = parts[3];

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;

            Http = new HttpClient(handler);
            Http.BaseAddress = new Uri("https://127.0.0.1:" + port);
            string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("riot:" + password));
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
        }

        public string Get(string url)
        {
            return Http.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
        }

        public byte[] GetBytes(string url)
        {
            return Http.GetAsync(url).Result.Content.ReadAsByteArrayAsync().Result;
        }

        public string Post(string url, string json)
        {
            return Http.PostAsync(url, Body(json)).Result.Content.ReadAsStringAsync().Result;
        }

        public void Put(string url, string json)
        {
            Http.PutAsync(url, Body(json)).Wait();
        }

        public string Patch(string url, string json)
        {
            return Http.PatchAsync(url, Body(json)).Result.Content.ReadAsStringAsync().Result;
        }

        public void Delete(string url)
        {
            Http.DeleteAsync(url).Wait();
        }

        private StringContent Body(string json)
        {
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}