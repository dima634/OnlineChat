
function scrollToView(element, params) {
    if (element == null) return true;
    element.scrollIntoView(params);
    return true;
}

class InfiniteScroll {

    constructor(elementid, DotNetHelper) {
        this.raised = false;
        this.element = elementid;//document.getElementById(elementid);

        this.DotNetHelper = DotNetHelper;

        this.ListenToScroll = function (event, element, DotNetHelper) {
            var bounding = element.getBoundingClientRect();
            if (
                bounding.top >= 0 &&
                bounding.left >= 0 &&
                bounding.right <= (window.innerWidth || element.clientWidth) &&
                bounding.bottom <= (window.innerHeight || element.clientHeight)
            ) {
                if (!this.raised) {
                    this.raised = true;
                    console.log('In the viewport!');
                    DotNetHelper.invokeMethodAsync("LoadMore");
                }
            }
        };

        this.handler = (ev) => { this.ListenToScroll(ev, this.element, this.DotNetHelper) };

        document.addEventListener("scroll", this.handler, true);

        this.RemoveListener = function () {
            document.removeEventListener("scroll", this.handler, true);
            console.log('Listener removed!');
        }
    }
}

function resize(textarea) {
    if (textarea == undefined) {
        txt = $('textarea')[0];
        resize(txt);
    }
    else {
        textarea.style.height = 'auto';
        textarea.style.height = textarea.scrollHeight + 'px';
    }
}

function handleInput(event) {
    txt = event.currentTarget;
    if (event.code == "Enter") {
        event.preventDefault();
        if (event.ctrlKey) {
            selectAt = txt.selectionStart + 1;
            txt.value =
                txt.value.substring(0, txt.selectionStart) +
                '\n' +
                txt.value.substring(txt.selectionEnd);
            txt.setSelectionRange(selectAt, selectAt);
        }
        else {
            window.Chat.invokeMethodAsync("OnSendClickAsync");
        }
    }
}

function InitChat(dotnetreference) {
    window.Chat = dotnetreference;
}