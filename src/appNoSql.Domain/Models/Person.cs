using appNoSql.Domain.Core.Models;
using System.Text.Json.Serialization;

namespace appNoSql.Domain.Models
{
    public class Person : Entity
    {        
        public string? Name { get; set; }

        [JsonPropertyName("expire_at")]
        public DateTime? ExpireAt { get; set; }
    }
}
