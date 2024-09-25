using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CodeToImageGenerator.Web.Models
{
    public class TelegramUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("language_code")]
        public string LanguageCode { get; set; }

        [JsonPropertyName("is_premium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("allows_write_to_pm")]
        public bool AllowsWriteToPm { get; set; }

        public override string? ToString()
        {
            return $"id: {Id}{Environment.NewLine}firstName: {FirstName}{Environment.NewLine}lastName: {LastName}{Environment.NewLine}username: {Username}{Environment.NewLine}langCode: {LanguageCode}{Environment.NewLine}isPremium: {IsPremium}{Environment.NewLine}allowsWritePM: {AllowsWriteToPm}";
        }
    }
}
