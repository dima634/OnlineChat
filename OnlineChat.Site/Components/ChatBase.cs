using BlazorContextMenu;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using OnlineChat.Dtos;
using OnlineChat.Dtos.BindingModels;
using OnlineChat.Dtos.ViewModels;
using OnlineChat.Site.Auth;
using OnlineChat.Site.InstantMessaging;
using OnlineChat.Site.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.Site.Components
{
    public class ChatBase : ComponentBase
    {
        [Parameter]
        public int ChatId { get; set; }

        #region inject

        [Inject]
        protected WebApiClient Api { get; set; }

        [Inject]
        protected NavigationManager Navigation { get; set; }

        [Inject]
        protected InstantMessager InstantMessager { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthProvider { get; set; }

        [Inject]
        protected IMatToaster Toaster { get; set; }

        [Inject]
        protected IJSRuntime Js { get; set; }

        #endregion inject

        #region fileds

        protected bool _loading = true;
        protected bool _firstLoad = true;
        protected bool _messageSent = false;
        protected bool _allMessagesLoaded = true;
        protected bool _loadedMore = false;
        protected bool _isNewMessagesBarVisible = false;
        protected List<MessageViewModel> _messages = new List<MessageViewModel>();
        protected MessageViewModel _replyTo = null;
        protected MessageViewModel _edit = null;
        protected ChatInfo _chat;
        protected string _messageText = string.Empty;
        protected int _scrollTo;
        protected int _messagesPerRequest = 20;

        protected ElementReference _textarea;

        #endregion fileds

        protected ElementReference NewMessagesDivider { get; set; }
        protected ElementReference _scroll;

        protected string Username => (AuthProvider as AuthStateProvider).Username;

        #region lifetime methods

        protected override void OnInitialized()
        {
            InstantMessager.MessageReceived += (sender, args) =>
            {
                if (args.ChatId == ChatId)
                {
                    _messages.Insert(0, args.Message);
                    if (args.Message.Author == Username)
                    {
                        _messageSent = true;
                    }
                    StateHasChanged();
                }
            };

            InstantMessager.MessageEdited += (sender, args) =>
            {
                if (args.ChatId == ChatId)
                {
                    var editedIdx = _messages.FindIndex(m => m.Id == args.Message.Id);

                    _messages.RemoveAt(editedIdx);
                    _messages.Insert(editedIdx, args.Message);

                    StateHasChanged();
                }
            };

            InstantMessager.MessageDeleted += (sender, args) =>
            {
                if (args.ChatId == ChatId)
                {
                    var messageIdx = _messages.FindIndex(m => m.Id == args.MessageId);

                    if (args.DeletedForAll || args.Author == (AuthProvider as AuthStateProvider).Username)
                    {
                        _messages.RemoveAt(messageIdx);
                    }

                    StateHasChanged();
                }
            };

            InstantMessager.MessageRead += (sender, args) =>
            {
                _messages.Find(m => m.Id == args.MessageId).IsRead = true;
            };

            Js.InvokeVoidAsync("InitChat", DotNetObjectReference.Create(this));
            //Js.InvokeVoidAsync("initScrollListening", DotNetObjectReference.Create(this), nameof(OnMessageInViewportAsync));
        }

        protected override void OnParametersSet()
        {
            Console.WriteLine("param set");
            _isNewMessagesBarVisible = false;
            _edit = null;
            _loadedMore = false;
            _firstLoad = true;
            _messages.Clear();
            //_messageElements.Clear();
            _allMessagesLoaded = false;
            _scrollTo = 0;
            _loading = true;

            var task = Task.Run(async () =>
            {
                _chat = await Api.GetChatAsync(ChatId);
                if (_messages.Count == 0)
                {
                    var firstBunch = await Api.GetMessagesByOffset(ChatId, 0, _messagesPerRequest);

                    if (_messagesPerRequest > firstBunch.Count) _allMessagesLoaded = true;

                    _messages.AddRange(firstBunch);

                }

                _loading = false;
                Console.WriteLine("first bunch");

                StateHasChanged();
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_messages.Count != 0 && !_loading)
            {
                if (_isNewMessagesBarVisible && _firstLoad)
                {
                    _firstLoad = false;
                    await Js.InvokeAsync<bool>("scrollToView", NewMessagesDivider, new { behaviour = "auto" });
                }
                else if (_loadedMore)
                {
                    _loadedMore = false;
                    //await Js.InvokeAsync<bool>("scrollToView", _messageElements.First(), new { behaviour = "auto" });
                }
            }
        }

        #endregion lifetime methods

        [JSInvokable]
        public async Task LoadMore()
        {
            if (!_allMessagesLoaded)
            {
                _scrollTo = _messages.Count;
                var nextBunch = await Api.GetMessagesByOffset(ChatId, _messages.Count, _messagesPerRequest);
                if (nextBunch.Count == 0) _allMessagesLoaded = true;
                _messages.AddRange(nextBunch);
                _loadedMore = true;
                _isNewMessagesBarVisible = false;
                _scrollTo = _messages.Count - _scrollTo;

                StateHasChanged();
            }
        }

        [JSInvokable]
        public async void OnSendClickAsync()
        {
            _messageText = _messageText.TrimEnd();
            if (string.IsNullOrWhiteSpace(_messageText) || string.IsNullOrEmpty(_messageText)) return;

            if (_edit != null)
            {
                var model = new EditMessageModel()
                {
                    ContentType = ContentType.Text,
                    Id = _edit.Id,
                    Content = _messageText
                };

                var response = await Api.EditMessage(model);

                if (!response.IsSuccessStatusCode)
                {
                    Toaster.Add("Error occured while trying edit message", MatToastType.Danger);
                }

                _edit = null;
                _messageText = string.Empty;
            }
            else
            {
                var message = new MessageModel()
                {
                    Content = _messageText,
                    ContentType = ContentType.Text,
                    ReplyTo = _replyTo?.Id
                };

                var result = await Api.SendMessage(message, ChatId);

                if (!result.IsSuccessStatusCode)
                {
                    Toaster.Add("Error occured while trying send message", MatToastType.Danger);
                }

                _replyTo = null;
                _messageText = string.Empty;
            }

            StateHasChanged();

            await Js.InvokeAsync<string>("resize", _textarea);
        }

        protected void OnDeleteForAllClick(ItemClickEventArgs args)
            => DeleteMessageAsync(args.Data as MessageViewModel, true);

        protected void OnDeleteForMeClick(ItemClickEventArgs args)
            => DeleteMessageAsync(args.Data as MessageViewModel, false);

        protected async void DeleteMessageAsync(MessageViewModel message, bool deleteForAll)
        {
            var deleteModel = new DeleteMessageModel
            {
                DeleteForAll = deleteForAll,
                Id = message.Id
            };

            var result = await Api.DeleteMessage(deleteModel);

            if (!result.IsSuccessStatusCode) Toaster.Add("Error occured while trying delete message", MatToastType.Danger);
            else Toaster.Add("Message deleted", MatToastType.Success);
        }

        protected async void OnReplyToDirectClickAsyn(ItemClickEventArgs args)
        {
            var message = args.Data as MessageViewModel;
            _replyTo = message;
            var direct = await Api.GetDirectChatWithAsync(message.Author);
            var directUri = $"chat/{direct.Id}";
            Navigation.NavigateTo(directUri);
        }

        protected void OnEditClick(ItemClickEventArgs args)
        {
            _edit = args.Data as MessageViewModel;
            _messageText = _edit.Content.ToString();
        }

        [JSInvokable]
        public async Task OnMessageInViewportAsync(int boxIdx)
        {
            if (boxIdx == _messages.Count - 1)
            {
                await LoadMore();
            }

            if (!_messages[boxIdx].IsRead && _messages[boxIdx].Author != Username)
            {
                await InstantMessager.MarkMessageAsReadAsync(_messages[boxIdx].Id, ChatId);
                _messages[boxIdx].IsRead = true;
            }
        }

        protected async Task OnScrollAsync()
        {
            var scrollAtTop = await Js.InvokeAsync<bool>("scrollAtTop", _scroll);

            if (scrollAtTop)
            {
                await LoadMore();
            }
        }

        protected async Task OnMouseOverMessageAsync(MessageViewModel message)
        {
            if (!message.IsRead && message.Author != Username)
            {
                await InstantMessager.MarkMessageAsReadAsync(message.Id, ChatId);
                message.IsRead = true;
            }
        }
    }
}
