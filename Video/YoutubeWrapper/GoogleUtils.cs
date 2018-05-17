using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace YoutubeWrapper
{
    public class GoogleUtils
    {
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        
        private static readonly Dictionary<GoogleScopes, string> GoogleScopes = new Dictionary<GoogleScopes, string>
        {
            {YoutubeWrapper.GoogleScopes.Youtube, HttpUtility.UrlEncode("https://www.googleapis.com/auth/youtube")}
        };
        
        public static async Task<string> GetRefreshToken(string clientId, string clientSecret, string appName, GoogleScopes googleScope)
        {
            // Generates state and PKCE values.
            var state = RandomDataBase64Url(32);
            var codeVerifier = RandomDataBase64Url(32);
            var codeChallenge = Base64UrlencodeNoPadding(Sha256(codeVerifier));
            const string CodeChallengeMethod = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            var redirectUri = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            http.Start();
            var scope = GoogleScopes[googleScope];
            // Creates the OAuth 2.0 authorization request.
            var authorizationRequest =
                $"{AuthorizationEndpoint}?response_type=code" +
                $"&scope=openid%20{scope}" +
                $"&redirect_uri={System.Uri.EscapeDataString(redirectUri)}" +
                $"&client_id={clientId}" +
                $"&state={state}" +
                $"&code_challenge={codeChallenge}" +
                $"&code_challenge_method={CodeChallengeMethod}";

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            var responseString = $"<html><head><meta http-equiv=\'refresh\' content=\'10;url=https://google.com\'></head><body>Please return to the {appName}.</body></html>";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) => 
            {
                responseOutput.Close();
                http.Stop();
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                throw new GoogleException($"OAuth authorization error: {context.Request.QueryString.Get("error")}.");
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                throw new GoogleException("Malformed authorization response. " + context.Request.QueryString);
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incomingState = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incomingState != state)
            {
                throw new GoogleException($"Received request with invalid state ({incomingState})");
            }

            // Starts the code exchange at the Token Endpoint.
            return await PerformCodeExchange(clientId, clientSecret, code, codeVerifier, redirectUri);
        }

        private static async Task<string> PerformCodeExchange(string clientId,
            string clientSecret,
            string code,
            string codeVerifier,
            string redirectUri)
        {
            // builds the  request
            var tokenRequestUri = "https://www.googleapis.com/oauth2/v4/token";
            var tokenRequestBody =
                $"code={code}" +
                $"&redirect_uri={System.Uri.EscapeDataString(redirectUri)}" +
                $"&client_id={clientId}" +
                $"&code_verifier={codeVerifier}" +
                $"&client_secret={clientSecret}" +
                "&scope=&grant_type=authorization_code";

            // sends the request
            var tokenRequest = (HttpWebRequest) WebRequest.Create(tokenRequestUri);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            var byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = byteVersion.Length;
            var stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                var tokenResponse = await tokenRequest.GetResponseAsync();
                using (var reader = new StreamReader(tokenResponse.GetResponseStream() ??
                                                     throw new GoogleException("Error while selecting refresh token to user's account.")))
                {
                    // reads response body
                    var responseText = await reader.ReadToEndAsync();

                    // converts to dictionary
                    var tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    return tokenEndpointDecoded["refresh_token"];
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream() ?? 
                                                             throw new GoogleException("Error while selecting response stream."))
                        )
                        {
                            // reads response body
                            var responseText = await reader.ReadToEndAsync();
                            throw new GoogleException(
                                $"Error while getting refresh token to user's account. {responseText}");
                        }
                    }
                }
                throw new GoogleException($"Error while getting refresh token to user's account. No response text.");
            }
        }

        // ref http://stackoverflow.com/a/3978040
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        private static string RandomDataBase64Url(uint length)
        {
            var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64UrlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static string Base64UrlencodeNoPadding(byte[] buffer)
        {
            var base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        private static byte[] Sha256(string inputStirng)
        {
            var bytes = Encoding.ASCII.GetBytes(inputStirng);
            var sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }
    }
}