using System.Text.Json.Serialization;

namespace CodeToImageGenerator.Web.Models
{
    public class TelegramMiniAppData
    {
        [JsonPropertyName("query_id")]
        public string QueryId { get; set; }

        [JsonPropertyName("user")]
        public TelegramUser User { get; set; }

        [JsonPropertyName("auth_date")]
        public long AuthDate { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        public override string ToString()
        {
            return $"queryID: {QueryId}{Environment.NewLine}telegramUser: {User}{Environment.NewLine}AuthDate: {AuthDate}{Environment.NewLine}Hash: {Hash} ";
        }
    }
}
