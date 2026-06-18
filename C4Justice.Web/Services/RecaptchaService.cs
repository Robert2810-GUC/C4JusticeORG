using System.Text.Json;

namespace C4Justice.Web.Services
{
    public class RecaptchaService
    {
        private readonly HttpClient _http;
        private readonly string _secretKey;
        private readonly double _minScore;
        private readonly bool _enabled;

        public RecaptchaService(HttpClient http, IConfiguration config)
        {
            _http      = http;
            _enabled   = config.GetValue<bool>("Recaptcha:Enabled");
            _secretKey = config["Recaptcha:SecretKey"] ?? "";
            _minScore  = double.TryParse(config["Recaptcha:MinScore"], out var s) ? s : 0.3;
        }

        public async Task<bool> VerifyAsync(string? token)
        {
            if (!_enabled) return true;
            if (string.IsNullOrWhiteSpace(token)) return false;

            try
            {
                var body = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret",   _secretKey),
                    new KeyValuePair<string, string>("response", token)
                });

                var res = await _http.PostAsync(
                    "https://www.google.com/recaptcha/api/siteverify", body);

                if (!res.IsSuccessStatusCode) return false;

                var json = await res.Content.ReadAsStringAsync();
                using var doc  = JsonDocument.Parse(json);
                var root  = doc.RootElement;

                var success = root.TryGetProperty("success", out var sv) && sv.GetBoolean();
                var score   = root.TryGetProperty("score",   out var sc) ? sc.GetDouble() : 0.0;

                return success && score >= _minScore;
            }
            catch
            {
                return false;
            }
        }
    }
}
