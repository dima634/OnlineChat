using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineChat.Dtos.BindingModels
{
    public class MessageModel
    {
        [ModelBinder(BinderType = typeof(ContentBinder))]
        public object Content { get; set; }

        public string ContentType { get; set; }

        public int? ReplyTo { get; set; }
    }

    public class ContentBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var form = bindingContext.HttpContext.Request.Form;
            Enum.TryParse<ContentType>(form[nameof(MessageModel.ContentType)], out var type);

            if (type == ContentType.File)
            {
                var value = form.Files[bindingContext.FieldName];
                bindingContext.Result = ModelBindingResult.Success(value);
            } 
            else if (type == ContentType.Text)
            {
                var valueProvider = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
                bindingContext.Result = ModelBindingResult.Success(valueProvider.FirstValue);
            }

            return Task.CompletedTask;
        }
    }
}
