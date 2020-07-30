using AutoMapper;
using AutoMapper.Configuration;
using OnlineChat.Dtos;
using OnlineChat.Dtos.BindingModels;
using OnlineChat.Dtos.ViewModels;
using OnlineChat.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Helpers
{
    public class AutomapperConfig
    {
        public static IMapper Build()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReplyMessage, ReplyMessageViewModel>();

                cfg.CreateMap<Message, MessageViewModel>()
                    .Include<ReplyMessage, ReplyMessageViewModel>()
                    .ForMember(vm => vm.Author, opt => opt.MapFrom(m => m.Author.Nickname))
                    .ForMember(vm => vm.ContentType, opt => opt.MapFrom((m, vm) =>
                    {
                        var type = m.Content switch
                        {
                            TextContent _ => ContentType.Text,
                            _ => throw new ApplicationException($"Unknown content type: {m.Content.GetType().FullName}")
                        };

                        return type.ToString();
                    }))
                    .ForMember(vm => vm.Content, opt => opt.MapFrom((m, vm) =>
                    {
                        var content = m.Content switch
                        {
                            TextContent textContent => textContent.Text,
                            _ => throw new ApplicationException($"Unknown content type: {m.Content.GetType().FullName}")
                        };

                        return content;
                    }));

                cfg.CreateMap<MessageModel, Message>()
                    .ForMember(m => m.SentOn, opt => opt.MapFrom(tm => DateTime.Now))
                    .ForMember(m => m.Content, opt => opt.MapFrom((model, message) =>
                    {
                        MessageContent content = ContentType.Parse<ContentType>(model.ContentType) switch
                        {
                            ContentType.Text => new TextContent() { Text = model.Content.ToString() },
                            _ => throw new ApplicationException($"Unsupported content type: {model.ContentType}")
                        };

                        return content;
                    }));

                cfg.CreateMap<MessageModel, ReplyMessage>()
                    .IncludeBase<MessageModel, Message>()
                    .ForMember(m => m.ReplyTo, opt => opt.Ignore());

                cfg.CreateMap<string, MessageContent>().ConvertUsing(s => new TextContent()
                {
                    Text = s
                });

                cfg.CreateMap<Chat, ChatInfo>()
                    .Include<DirectChat, DirectChatInfo>()
                    .Include<GroupChat, GroupChatInfo>()
                    .ForMember(ci => ci.Members, opt => opt.MapFrom(c => c.Members.Select(user => user.Nickname)));

                cfg.CreateMap<DirectChat, DirectChatInfo>();

                cfg.CreateMap<GroupChat, GroupChatInfo>()
                    .ForMember(ci => ci.Owner, opt => opt.MapFrom(c => c.Owner.Nickname))
                    .ForMember(ci => ci.Name, opt => opt.MapFrom(c => c.Name));

                cfg.CreateMap<EditMessageModel, MessageContent>()
                    .ConstructUsing((model, context) =>
                    {
                        MessageContent content = model.ContentType switch
                        {
                            ContentType.Text => new TextContent() { Text = model.Content.ToString() },
                            _ => throw new ApplicationException("Unsuported content type")
                        };

                        return content;
                    })
                    .ForMember(c => c.Id, opt => opt.Ignore());
            });

            return config.CreateMapper();
        }
    }
}
