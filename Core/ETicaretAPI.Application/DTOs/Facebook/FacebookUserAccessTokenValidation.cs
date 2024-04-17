using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.Facebook
{
    public class FacebookUserAccessTokenValidation
    {
        [JsonPropertyName("data")]
        public rootData Data { get; set; }
    }

    public class rootData
    {

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}


//{ "data":{ "app_id":"9252805894","type":"USER","application":"Mini E-Ticaret","data_access_expires_at":1717422435,"expires_at":17650800,"is_valid":true,"scopes":"email","public_profile"],"user_id":"750416299381"} }