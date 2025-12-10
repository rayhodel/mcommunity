using System.Text.Json.Serialization;

namespace MCommunityWeb.Models
{
    public class TokenRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class TokenResponse
    {
        [JsonPropertyName("access")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class PersonResponse
    {
        [JsonPropertyName("entry_dn")]
        public string EntryDn { get; set; } = string.Empty;

        [JsonPropertyName("uid")]
        public string Uniqname { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("mail")]
        public List<string> Mail { get; set; } = new();

        [JsonPropertyName("title")]
        public List<string> Title { get; set; } = new();

        [JsonPropertyName("telephoneNumber")]
        public List<string> TelephoneNumber { get; set; } = new();

        [JsonPropertyName("umichInstRoles")]
        public List<string> Affiliations { get; set; } = new();
    }

    public class GroupResponse
    {
        [JsonPropertyName("cn")]
        public string GroupName { get; set; } = string.Empty;

        [JsonPropertyName("umichGroupEmail")]
        public string GroupEmail { get; set; } = string.Empty;

        [JsonPropertyName("umichDescription")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("owner")]
        public List<string> Owners { get; set; } = new();

        [JsonPropertyName("member")]
        public List<string> Members { get; set; } = new();
    }

    public class SearchRequest
    {
        public string Query { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}
