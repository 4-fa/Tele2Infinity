using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Tele2Infinity.Models.Account;
using Tele2Infinity.Models.AvailableRoamingBundles;
using Tele2Infinity.Models.BundleStatus;
using Tele2Infinity.Models.CreateToken;
using Tele2Infinity.Models.LinkedSubscriptions;
using Tele2Infinity.Models.Login;
using Tele2Infinity.Models.RoamingBundles;
using Tele2Infinity.Models.Subscription;
using Bundle = Tele2Infinity.Models.RoamingBundles.Bundle;

namespace Tele2Infinity
{
    public class Tele2
    {
        // Todo: Configurable?
        private const string AppBasicAuth = "NTdvdzJxYnlkYTA1NDkxODo2ODA4eTE5bDlvNHA1NTU0";
        private const string AppClientId = "57ow2qbyda054918";

        private readonly HttpClientHandler _handler;
        private readonly HttpClient _client;

        public string? SubscriptionUrl { get; set; }
        public string CustomerId { get; set; }
        public string Phone { get; set; }

        public Tele2()
        {
            var cookieContainer = GetCookies();
            _handler = new HttpClientHandler();
            _handler.AllowAutoRedirect = false;
            _handler.UseCookies = true;
            _handler.CookieContainer = cookieContainer;
            _handler.AutomaticDecompression = DecompressionMethods.All; // Adds header: "accept-encoding", "gzip, deflate, br" 
#if DEBUG
            _handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
#endif

            _client = new HttpClient(_handler, true);
            var headers = _client.DefaultRequestHeaders;

            headers.Add("user-agent", "Tele2/1.4.10 (build:18510; iOS 15.5.0)");
            headers.TryAddWithoutValidation("accept-language", "en-GB,en;q=0.9");
        }

        public void SetBearer(string token)
        {
            _client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<BundleStatusResponse?> GetBundleStatus()
        {
            var methodName = "postpaiddatabundlestatus";
            var url =
                $"{SubscriptionUrl}/{methodName}?resourceLabel=AvailableFupResets%2CFupReset";

            return await DoRequest<BundleStatusResponse>(url, $"{methodName}.v9");
        }

        public async Task<AvailableRoamingBundlesResponse?> GetAvailableRoamingBundles()
        {
            var methodName = "availableroamingbundles";
            var url =
                $"{SubscriptionUrl}/{methodName}";

            return await DoRequest<AvailableRoamingBundlesResponse>(url, $"{methodName}.v4");
        }

        public async Task<AccountResponse?> GetAccount()
        {
            var accept = "account.v1";
            var url =
                "https://capi.tele2.nl/account/current?resourceLabel=Subscription%2CCustomer%2CLinkedSubscriptions%2CCustomerBenefits";

            var request = CreateRequest(url, accept);
            var result = await _client.SendAsync(request);

            url = result.Headers.Location?.ToString();

            if (url == null)
                throw new Exception("Could not get account redirection");

            return await DoRequest<AccountResponse>(url, accept);
        }

        public async Task<LinkedSubscriptionsResponse?> GetLinkedSubscriptions(string url)
        {
            var accept = "linkedsubscriptions.v1";
            url += "?resourceLabel=";

            return await DoRequest<LinkedSubscriptionsResponse>(url, accept);
        }

        public async Task<SubscriptionResponse?> GetSubscription()
        {
            var accept = "subscription.v2";
            var url = SubscriptionUrl;
            url += "?resourceLabel=NetworkEvent%2CPrepaidBundleStatus%2CSubscriptionDevice%2CVoicemailSettings%2CPrepaidSubscription%2CPostpaidSubscription%2CPaymentStatus%2CSubscriber%2CPostpaidSubscriptions%2CNetworkScan%2CPaymentCreate%2CRoamingBundles%2CESimReservationStatus%2CPostpaidBundleStatus%2CCustomer%2CRegisterDevice%2CAvailableRoamingBundles%2CPostpaidBundleStatusDetails%2CPostpaidDataBundleStatus%2CCommunity";

            return await DoRequest<SubscriptionResponse>(url, accept);
        }

        public async Task LoadProfile()
        {
            var account = await GetAccount();
            var linkedSubscriptionsUrl = account?.Resources.FirstOrDefault(x => x.Label == "LinkedSubscriptions")?.Url;
            if (linkedSubscriptionsUrl == null)
                throw new Exception("Could not find linked subscriptions!");

            var linked = await GetLinkedSubscriptions(linkedSubscriptionsUrl);
            SubscriptionUrl = linked?.Subscriptions.FirstOrDefault()?.SubscriptionURL;

            if (SubscriptionUrl == null)
                throw new Exception("Could not find any subscription!");

            var subscription = await GetSubscription();
            if (subscription == null)
                throw new Exception($"Could not load subscription for url: {SubscriptionUrl}!");

            Phone = subscription.Msisdn;
            CustomerId = subscription.CustomerNumber;
        }

        public async Task BuyRoamingBundle(string buyingCode)
        {
            var methodName = "roamingbundles";
            var url =
                $"{SubscriptionUrl}/{methodName}";

            var request = new RoamingBundlesRequest
            {
                Bundles = new List<Bundle>()
            };
            request.Bundles.Add(new Bundle { BuyingCode = buyingCode });

            await DoPostRequest(url, $"{methodName}.v4", request);
        }

        public async Task<string> Login(string username, string password)
        {
            var authorizationToken = await GetAuthorizationCode(username, password);

            var url = "https://capi.tele2.nl/createtoken";

            var appAuth = new AuthenticationHeaderValue(
                "Basic",
               AppBasicAuth
            );

            var data = new CreateTokenRequest
            {
                AuthorizationCode = authorizationToken
            };

            var request = CreateRequest(url, "createtoken.v1", true);
            using var content = CreateContent(data);

            request.Content = content;
            request.Headers.Authorization = appAuth;
            request.Headers.Add("grant_type", "authorization_code");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var accessToken = response.Headers.FirstOrDefault(x => x.Key.ToLower() == "accesstoken").Value.FirstOrDefault();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Could not find access (bearer) token!");
            }

            SetBearer(accessToken);

            return accessToken;
        }

        private async Task<string> GetAuthorizationCode(string username, string password)
        {
            var url = "https://capi.tele2.nl/login?response_type=code";

            var appAuth = new AuthenticationHeaderValue(
                "Basic",
               AppBasicAuth
            );

            var data = new LoginRequest
            {
                ClientId = AppClientId,
                Password = password,
                Scope = "usage+readfinancial+readsubscription+readpersonal+changesubscription+weblogin",
                Username = username,
            };

            var request = CreateRequest(url, "login.v2", true);
            using var content = CreateContent(data);

            request.Content = content;
            request.Headers.Authorization = appAuth;
            request.Headers.Add("grant_type", "authorization_code");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var authCode = response.Headers.FirstOrDefault(x => x.Key.ToLower() == "authorizationcode").Value.FirstOrDefault();
            if (string.IsNullOrEmpty(authCode))
            {
                throw new Exception("Could not find auth code!");
            }

            return authCode;
        }

        private HttpRequestMessage CreateRequest(string url, string accept, bool isPost = false)
        {
            var request = new HttpRequestMessage(isPost ? HttpMethod.Post : HttpMethod.Get, url);
            request.Headers.Add("accept", GetAcceptHeader(accept));
            return request;
        }

        private StringContent CreateContent<T>(T data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            return httpContent;
        }

        public async Task<T?> DoRequest<T>(string url, string accept)
        {
            var request = CreateRequest(url, accept);
            var response = await _client.SendAsync(request);

            var json = JsonConvert.SerializeObject(_handler.CookieContainer.GetAllCookies(), Formatting.Indented);
            await File.WriteAllTextAsync("cookies.json", json);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<HttpResponseMessage> DoPostRequest<T>(string url, string accept, T data)
        {
            var request = CreateRequest(url, accept, true);
            using var content = CreateContent(data);
            request.Content = content;
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = JsonConvert.SerializeObject(_handler.CookieContainer.GetAllCookies(), Formatting.Indented);
            await File.WriteAllTextAsync("cookies.json", json);

            return response;
        }

        private static string GetAcceptHeader(string accept)
        {
            return $"application/json,application/vnd.capi.tmobile.nl.{accept}+json";
        }

        private static CookieContainer GetCookies(string file = "cookies.json")
        {
            var cookieContainer = new CookieContainer();
            if (!File.Exists(file))
                return cookieContainer;

            var json = File.ReadAllText(file);
            var cookieCollection = JsonConvert.DeserializeObject<CookieCollection>(json);
            cookieContainer.Add(cookieCollection);

            return cookieContainer;
        }
    }
}
