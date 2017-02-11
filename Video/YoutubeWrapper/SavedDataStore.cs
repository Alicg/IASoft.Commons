using System;
using System.Globalization;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Json;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;

namespace YoutubeWrapper
{
    public class StoredResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public long? ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public string Issued { get; set; }


        public StoredResponse(string pRefreshToken)
        {
            this.RefreshToken = pRefreshToken;
            this.Issued = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
        }
        public StoredResponse()
        {
            this.Issued = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
        }
    }

    public class SavedDataStore : IDataStore
    {
        public StoredResponse StoredResponse { get; set; }

        public SavedDataStore(string refreshToken)
        {
            this.StoredResponse = new StoredResponse(refreshToken);
        }
        public SavedDataStore()
        {
            this.StoredResponse = new StoredResponse();
        }

        public Task StoreAsync<T>(string key, T value)
        {
            var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
            var jObject = JObject.Parse(serialized);
            // storing access token
            var test = jObject.SelectToken("access_token");
            if (test != null)
            {
                this.StoredResponse.AccessToken = (string)test;
            }
            // storing token type
            test = jObject.SelectToken("token_type");
            if (test != null)
            {
                this.StoredResponse.TokenType = (string)test;
            }
            test = jObject.SelectToken("expires_in");
            if (test != null)
            {
                this.StoredResponse.ExpiresIn = (long?)test;
            }
            test = jObject.SelectToken("refresh_token");
            if (test != null)
            {
                this.StoredResponse.RefreshToken = (string)test;
            }
            test = jObject.SelectToken("Issued");
            if (test != null)
            {
                this.StoredResponse.Issued = (string)test;
            }
            return Task.Delay(0);
        }

        public Task DeleteAsync<T>(string key)
        {
            this.StoredResponse = new StoredResponse();
            return Task.Delay(0);
        }

        public Task<T> GetAsync<T>(string key)
        {
            var tcs = new TaskCompletionSource<T>();
            try
            {
                var deserialize = new TokenResponse {RefreshToken = this.StoredResponse.RefreshToken};
                tcs.SetResult((T)(object)deserialize);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public Task ClearAsync()
        {
            this.StoredResponse = new StoredResponse();
            return Task.Delay(0);
        }
    }
}
