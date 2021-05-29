using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OnlineChat.Dtos.BindingModels;
using OnlineChat.Dtos.ViewModels;
using OnlineChat.WebApi.Models;
using OnlineChat.WebApi.Services;
using OnlineChat.WebApi.Services.InstantMessaging;

namespace OnlineChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IChatService _chatService;
        private IMapper _mapper;
        private IHubContext<InstantMessager, IInstantMessaging> _hub;
        private IChatManager _subscriptionManager;

        public MessageController(IChatService chatService, IMapper mapper, IHubContext<InstantMessager, IInstantMessaging> hub, IChatManager subscriptionManager)
        {
            _chatService = chatService;
            _mapper = mapper;
            _hub = hub;
            _subscriptionManager = subscriptionManager;
        }

        [HttpPost]
        [Route("edit")]
        public ActionResult EditMessage([FromBody] EditMessageModel model)
        {
            if (!_chatService.IsMessageAuthor(model.Id, User.Identity.Name)) return Unauthorized();
            
            var newContent = _mapper.Map<MessageContent>(model);
            var edited = _chatService.EditMessage(model.Id, newContent);

            var chatId = _chatService.GetChatByMessageId(model.Id).Id;
            var subIds = _subscriptionManager.GetActiveChatMembers(chatId);
            _hub.Clients.Clients(subIds.ToList()).MessageEdited(_mapper.Map<MessageViewModel>(edited), chatId);

            return Ok();
        }

        [HttpPost]
        [Route("delete")]
        public ActionResult DeleteMessage([FromBody] DeleteMessageModel model)
        {
            if (!_chatService.IsMessageAuthor(model.Id, User.Identity.Name)) return Unauthorized();

            var chatId = _chatService.GetChatByMessageId(model.Id).Id;

            _chatService.DeleteMessage(model.Id, model.DeleteForAll);

            var subIds = _subscriptionManager.GetActiveChatMembers(chatId);
            _hub.Clients.Clients(subIds.ToList()).MessageDeleted(model.Id, chatId, model.DeleteForAll, User.Identity.Name);

            return Ok();
        }
    }
}