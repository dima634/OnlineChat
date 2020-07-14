using Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineChat.Dtos.BindingModels;
using OnlineChat.Dtos.ViewModels;
using OnlineChat.Site.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineChat.Site.WebApi
{
    public class WebApiClient
    {
        public string AccessToken { get; private set; }
        private HttpClient _httpClient;
        private const string BASE_URI =

#if DEBUG
        @"https://localhost:44305/api/";
#else
        @"https://onlinechatwebapi.azurewebsites.net/api/";
#endif

        public WebApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BASE_URI);
            _httpClient.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ClaimsIdentity> Login(string username, string password)
        {
            var response = await _httpClient.PostAsync("account/token", new
            {
                Username = username,
                Password = password
            });
            string stringResponse = await response.Content.ReadAsStringAsync();
            var json = (JObject)JsonConvert.DeserializeObject(stringResponse);

            if (!json.ContainsKey("message"))
            {
                AccessToken = $"{json["token"]}";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", AccessToken);
                var identity = new ClaimsIdentity("Password"); 
                identity.AddClaim(new Claim(ClaimTypes.Name, username));

                return identity;
            }

            throw new ApplicationException(json["message"].ToString());
        }

        public async Task<List<string>> GetUsernamesStartsWithAsync(string str)
            => await _httpClient.GetAsync<List<string>>($"user/search/{str}");

        public async Task<List<ChatInfo>> GetChatsAsync()
            => await _httpClient.GetAsync<List<ChatInfo>>($"chat/list");

        public async Task<ChatInfo> GetChatAsync(int chatId)
            => await _httpClient.GetAsync<ChatInfo>($"chat/{chatId}");
        public async Task<DirectChatInfo> GetDirectChatWithAsync(string username)
            => await _httpClient.GetAsync<DirectChatInfo>($"chat/direct/{username}");

        public async Task<List<MessageViewModel>> GetMessagesByPage(int chatId, int page, int resultsPerPage = 20)
            => await _httpClient.GetAsync<List<MessageViewModel>>($"chat/{chatId}/messages/?page={page}&resultsPerPage={resultsPerPage}");
        public async Task<List<MessageViewModel>> GetMessagesByOffset(int chatId, int offset, int resultsCount = 20)
            => await _httpClient.GetAsync<List<MessageViewModel>>($"chat/{chatId}/messages/?offset={offset}&resultsPerPage={resultsCount}");
        public async Task<HttpResponseMessage> RegisterUser(RegisterModel model)
            => await _httpClient.PostAsync("account/register", model);

        public async Task<HttpResponseMessage> SendMessage(MessageModel model, int chatId)
            => await _httpClient.PostAsync($"chat/{chatId}/messages/send", model);

        public async Task<HttpResponseMessage> SendMessageToDirect(MessageModel model, string username)
            => await _httpClient.PostAsync($"chat/direct/{username}/messages/send", model);

        public async Task<HttpResponseMessage> DeleteMessage(DeleteMessageModel model)
            => await _httpClient.PostAsync($"message/delete", model);

        public async Task<HttpResponseMessage> EditMessage(EditMessageModel model)
            => await _httpClient.PostAsync($"message/edit", model);

        public async Task<HttpResponseMessage> CreateChat(CreateChatModel model)
            => await _httpClient.PostAsync("chat/create", model);

        #region Helpers

        private FormUrlEncodedContent FormUrlEncodedContent(object content)
        {
            var nameValue = new List<KeyValuePair<string, string>>();

            if(content == null) return new FormUrlEncodedContent(nameValue);

            foreach (var item in content.GetType().GetProperties())
            {
                nameValue.Add(new KeyValuePair<string, string>(item.Name, item.GetValue(content).ToString()));
            }

            return new FormUrlEncodedContent(nameValue);
        }

        #endregion
    }
}
