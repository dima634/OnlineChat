#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineChat.Dtos.BindingModels;
using OnlineChat.Dtos.ViewModels;
using OnlineChat.WebApi.Models;
using OnlineChat.WebApi.Models.Repos;
using OnlineChat.WebApi.Services;
using OnlineChat.WebApi.Helpers;
using Microsoft.AspNetCore.SignalR;
using OnlineChat.WebApi.Services.InstantMessaging;
using OnlineChat.Dtos;

namespace OnlineChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IChatService _chatService;
        private IMapper _mapper;
        private IUserService _userService;
        private IHubContext<InstantMessager, IInstantMessaging> _hub;
        private IChatManager _subscriptionManager;

        public ChatController (IChatService chatService, IUserService userService, IMapper mapper, IHubContext<InstantMessager, IInstantMessaging> hub, IChatManager subscriptionManager)
        {
            _chatService = chatService;
            _mapper = mapper;
            _userService = userService;
            _hub = hub;
            _subscriptionManager = subscriptionManager;
        }

        [HttpPost]
        [Route("direct/{username?}/messages/send")]
        [Route("{chatId?}/messages/send")]
        public ActionResult SendMessage([FromForm] MessageModel model, [FromRoute] int chatId, [FromRoute] string username)
        {
            MessageViewModel response;

            if(username != null)
            {
                chatId = _chatService.GetDirectChat(User.Identity.Name, username).Id;
            }

            var author = _userService.GetUser(User.Identity.Name);
            var message = _chatService.SendMessage(model.Content,
                                                   Enum.Parse<ContentType>(model.ContentType),
                                                   chatId,
                                                   author,
                                                   model.ReplyTo);

            if (model.ReplyTo == null)
            {
                response = _mapper.Map<MessageViewModel>(message);
            }
            else
            {
                response = _mapper.Map<ReplyMessageViewModel>(message);
            }

            var subIds = _subscriptionManager.GetActiveChatMembers(chatId);
            _hub.Clients.Clients(subIds.ToList()).MessageSent(response, chatId);

            return Ok();
        }

        [HttpGet]
        [Route("{chatId}/messages/")]
        public ActionResult<IEnumerable<MessageViewModel>> GetChatMessages([FromRoute] int chatId, [FromQuery] int? page, [FromQuery] int? offset, [FromQuery] int resultsPerPage)
        {
            if (!_chatService.IsChatMember(chatId, User.Identity.Name)) return Unauthorized();

            var messages = (page, offset, resultsPerPage) switch
            {
                (null, _, _) => _chatService.GetChatMessagesByOffset(chatId, User.Identity.Name, offset.Value, resultsPerPage),
                (_, null, _) => _chatService.GetChatMessagesByPage(chatId, User.Identity.Name, page.Value, resultsPerPage),
                _ => null
            };

            if (messages == null) return BadRequest();

            var mapped = _mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(messages, opt => opt.AfterMap((src, dest) =>
            {
                foreach ((Message message, MessageViewModel viewModel) in src.Zip(dest))
                {
                    viewModel.IsReadByCurrentUser = _chatService.IsMessageReadByUser(message.Id, User.Identity.Name);
                }
            }));

            return this.JsonContent(mapped, TypeNameHandling.Auto);
        }

        [HttpPost]
        [Route("create/group")]
        public ActionResult<ChatInfo> CreateGroupChat([FromBody] CreateGroupModel model)
        {
            model.Members.Add(User.Identity.Name);
            var chat = _chatService.CreateGroupChat(User.Identity.Name, model.Name, model.Members);
            var chatInfo = _mapper.Map<GroupChatInfo>(chat);

            var activeChatUsers = _subscriptionManager.GetActiveChatMembers(chatInfo.Id);
            _hub.Clients.Clients(activeChatUsers.ToList()).ChatCreated(chatInfo);

            return Ok();
        }

        [HttpPost]
        [Route("create/direct")]
        public ActionResult<ChatInfo> CreateDirectChat([FromBody] CreateDirectChatModel model)
        {
            ChatInfo chatInfo;

            try
            {
                var chat = _chatService.CreateDirectChat(User.Identity.Name, model.WithUser);
                chatInfo = _mapper.Map<DirectChatInfo>(chat);
            }
            catch (ChatAlreadyExistException)
            {
                return BadRequest($"You already have direct chat with {model.WithUser}");
            }

            var activeChatUsers = _subscriptionManager.GetActiveChatMembers(chatInfo.Id);
            _hub.Clients.Clients(activeChatUsers.ToList()).ChatCreated(chatInfo);

            return Ok();
        }

        [HttpGet]
        [Route("list")]
        public ActionResult<List<ChatInfo>> GetChats()
        {
            var chats = _chatService.GetChats(User.Identity.Name);
            var mapped = _mapper.Map<List<Chat>, List<ChatInfo>>(chats);
            return this.JsonContent(mapped, TypeNameHandling.Auto);
        }

        [HttpGet]
        [Route("direct/{username}")]
        public ActionResult<DirectChatInfo> GetDirectChatWith([FromRoute] string username)
        {
            var chat = _chatService.GetDirectChat(User.Identity.Name, username);

            if (chat == null) return NotFound();

            return _mapper.Map<DirectChatInfo>(chat);
        }

        [HttpGet]
        [Route("{chatId}")]
        public ActionResult<ChatInfo> GetChat([FromRoute] int chatId)
        {
            var chat = _chatService.GetChats(User.Identity.Name).FirstOrDefault(chat => chat.Id == chatId);

            if (chat == null) return NotFound();

            return this.JsonContent(_mapper.Map<ChatInfo>(chat), TypeNameHandling.Auto);
        }
    }
}