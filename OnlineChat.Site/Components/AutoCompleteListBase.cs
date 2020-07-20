using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.Site.Components
{
    public class AutoCompleteListBase<T> : ComponentBase
    {
        [Parameter]
        public Func<string, Task<List<T>>> SuggestionsOnInput { get; set; }

        private T _value;
        [Parameter]
        public T Value
        {
            get
            {
                if (_value == null) _value = Suggestions.Find(item => item.ToString() == value);

                return _value;
            }

            set
            {
                _value = value;
            }
        }

        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        [Parameter]
        public string Class { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        protected string value;
        protected bool suggestionsLoading = false;

        protected List<T> Suggestions { get; private set; } = new List<T>();

        protected void OnInput(ChangeEventArgs args)
        {
            Suggestions.Clear();
            Value = default;
            value = args.Value.ToString();
            if (value != string.Empty)
            {
                suggestionsLoading = true;
                Task.Run(async () =>
                {
                    Suggestions = await SuggestionsOnInput(args.Value.ToString());
                    suggestionsLoading = false;
                    Value = Suggestions.Find(item => item.ToString() == value);
                    await ValueChanged.InvokeAsync(Value);
                    StateHasChanged();
                });
                StateHasChanged();
            }
        }

        protected void OnSuggestionClick(T value)
        {
            this.value = value.ToString();
            Value = value;
            ValueChanged.InvokeAsync(Value);
            Suggestions.Clear();
            StateHasChanged();
        }

        protected void OnFocusLost()
        {
            Suggestions.Clear();
            StateHasChanged();
        }
    }
}
