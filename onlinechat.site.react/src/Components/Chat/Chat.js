import React from 'react'
import MessageList from './MessageList'
import { TextField } from '@material-ui/core';
import SendIcon from '@material-ui/icons/Send';
import EditIcon from '@material-ui/icons/Edit';
import ReplyIcon from '@material-ui/icons/Reply';
import CloseIcon from '@material-ui/icons/Close';
import IconButton from '@material-ui/core/IconButton';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import './Chat.css';
import TextContent from './TextContent';
import AttachmentIcon from '@material-ui/icons/Attachment';

class Chat extends React.Component {
    fileInput = React.createRef();

    constructor(props){
        super(props);

        this.loadedAllMessages = false;

        this.state = {
            messages: [],
            attachment: null,
            messageText: "",
            editMessage: null,
            replyTo: null,
            contextMenu: false,
            replyContextMenu: false,
            mouseY: 0,
            mouseX: 0
        };

        this.handleChange = this.handleChange.bind(this);
        this.onMessageReceived = this.onMessageReceived.bind(this);
        this.onMessageDeleted = this.onMessageDeleted.bind(this);
        this.onMessageEdited = this.onMessageEdited.bind(this);
        this.onMessageRead = this.onMessageRead.bind(this);
    }

    loadMoreMessages(){
        if(!this.loadedAllMessages){
            this.props.api.getChatMessagesAsync(this.props.chatId, this.state.messages.length)
                .then((fetched => { 
                    if(fetched.length === 0){
                        this.loadedAllMessages  = true;
                    }
                    else{
                        this.setState({messages: this.state.messages.concat(fetched)});
                    }
                }));
        }
    }
    
    handleChange(event) {
        this.setState({messageText: event.target.value});
    }

    onInput(event) {
        if(event.key === '\n'){
            event.preventDefault();
            this.setState({messageText: event.target.value + "\n"});
        }

        if(event.key === 'Enter') {
            event.preventDefault();
            this.sendOrEditMessage();
        }
    }

    onMessageReceived(args){
        if(args.chatId === this.props.chatId) {
            this.state.messages.unshift(args.message);
            this.setState({messages: this.state.messages});
        }
    }
    
    onMessageDeleted(args){
        if(args.chatId === this.props.chatId) {
            if(args.author === this.props.api.username || args.forAll){
                let idx = this.state.messages.findIndex(mes => mes.Id === args.messageId);
                this.state.messages.splice(idx, 1);
                this.setState({messages: this.state.messages});
            }
        }
    }

    onMessageEdited(args){
        if(args.chatId === this.props.chatId) {
            let idx = this.state.messages.findIndex(mes => mes.Id === args.message.Id);
            // eslint-disable-next-line
            this.state.messages[idx].Content = args.message.Content;
            // eslint-disable-next-line
            this.state.messages[idx].IsEdited = true;
            this.forceUpdate();
        }
    }

    onMessageRead(args){
        if(args.chatId === this.props.chatId) {
            let idx = this.state.messages.findIndex(mes => mes.Id === args.messageId);
            // eslint-disable-next-line
            this.state.messages[idx].IsRead = true;
            this.forceUpdate();
        }
    }

    editMessage(){
        let message = this.state.messageText.trim();

        if(message === "" || !message) return;

        this.props.api.editMessageAsync(this.state.editMessage.Id, message, "Text");
        this.setState({messageText: "", editMessage: null});
    }

    sendMessage(){
        let message = this.state.messageText.trim();

        if(message === "" || !message) return;

        let model = {
            Content: message,
            ContentType: "Text",
            ReplyTo: this.state.replyTo ? this.state.replyTo.Id  : ""
        };

        this.props.api.sendMessageAsync(model, this.props.chatId);
        this.setState({messageText: "", replyTo: null});
    }

    
    sendOrEditMessage(){
        if(this.state.editMessage !== null){
            this.editMessage();
        }
        else{
            this.sendMessage();
        }
    }

    sendAttachment(){
        // if(this.state.editMessage !== null){
        //     this.editMessage();
        // }
        // else{
        //     this.sendMessage();
        // }

        this.fileInput.current.click();
    }

    filePicked(){
        let file = this.fileInput.current.files[0];

        if(!file) return;

        let model = {
            Content: file,
            ContentType: "File",
            ReplyTo: this.state.replyTo ? this.state.replyTo.Id  : ""
        };

        this.props.api.sendMessageAsync(model, this.props.chatId);
        this.setState({attachment: null, replyTo: null});
    }

    onContextMenu(ev, message){
        this.contextMenuMessage = message;

        if(message.Author === this.props.api.username){
            this.setState({
                contextMenu: true,
                mouseY: ev.clientY - 4,
                mouseX: ev.clientX - 2
            });
        }
        else {
            this.setState({
                replyContextMenu: true,
                mouseY: ev.clientY - 4,
                mouseX: ev.clientX - 2
            });
        }
    }

    onDeleteClick(forAll){
        this.props.api.deleteMessageAsync(this.contextMenuMessage.Id, forAll)
            .then(() => this.setState({ contextMenu: false }));
    }

    onEditClick(){
        this.setState({
            replyTo: null,
            editMessage: this.contextMenuMessage,
            messageText: this.contextMenuMessage.Content,
            contextMenu: false
        });
    }

    onReplyClick(){
        this.setState({
            editMessage: null,
            replyTo: this.contextMenuMessage,
            replyContextMenu: false
        });
    }

    componentDidMount(){
        this.props.api.getChatMessagesAsync(this.props.chatId, 0)
            .then(messages => {
                this.props.messager.messageReceived.push(this.onMessageReceived);
                this.props.messager.messageDeleted.push(this.onMessageDeleted);
                this.props.messager.messageEdited.push(this.onMessageEdited);
                this.props.messager.messageRead.push(this.onMessageRead);
                this.setState({messages: messages});
            });
    }

    componentDidUpdate(prevProps, prevState, snapshot){
        if(prevProps.chatId !== this.props.chatId){
            this.loadedAllMessages = false;
            this.props.api.getChatMessagesAsync(this.props.chatId, 0)
                .then(messages => this.setState({
                    messages: messages,
                    messageText: "",
                    editMessage: null,
                    contextMenu: false,
                    mouseY: 0,
                    mouseX: 0
                }));
        }
    }

    render() {
        let replyBox;

        if(this.state.editMessage !== null || this.state.replyTo !== null){
            let icon;
            let messageText;

            if(this.state.editMessage !== null){
                icon = <EditIcon color="primary"/>
                messageText = this.state.editMessage.Content;
            }
            else{
                icon = <ReplyIcon color="primary"/>
                messageText = this.state.replyTo.Content;
            }

            replyBox = (
                <div className="chat-reply-box">
                    {icon}

                    <div className="chat-reply-box-content">
                        <TextContent text={messageText}/>
                    </div>
                    
                    <IconButton onClick={() => this.setState({ editMessage: null, replyTo: null, messageText: "" })} color="primary">
                        <CloseIcon/>
                    </IconButton>
                </div>
            );
        }

        return (
            <div className="chat-message-area">
                <MessageList 
                    messages={this.state.messages} 
                    api={this.props.api} 
                    onMessageInViewport={(message) => {
                        if(!message.IsReadByCurrentUser && message.Author !== this.props.api.username) {
                            message.IsReadByCurrentUser = true;
                            this.props.messager.markMessageAsReadAsync(message.Id, this.props.chatId);
                        }
                    }}
                    onTop={() => this.loadMoreMessages()}
                    onContextMenu={ (ev, mes) => this.onContextMenu(ev, mes) }
                />

                {replyBox}

                <div className="chat-message-input-box">
                    <TextField 
                        onKeyPress={(event) => this.onInput(event)}
                        onChange={this.handleChange}
                        value={this.state.messageText} 
                        size="medium" 
                        className="chat-message-input" 
                        rowsMax={10} 
                        placeholder="Send message..." 
                        multiline
                        autoFocus
                    />
                    <IconButton onClick={() => this.sendAttachment()} color="primary">
                        <AttachmentIcon/>
                        <input ref={this.fileInput} onChange={() => this.filePicked()} hidden type="file"></input>
                    </IconButton>
                    <IconButton onClick={() => this.sendOrEditMessage()} color="primary">
                        <SendIcon/>
                    </IconButton>
                </div>

                <Menu
                    anchorReference="anchorPosition"
                    anchorPosition={
                      this.state.mouseY !== null && this.state.mouseX !== null
                        ? { top: this.state.mouseY, left: this.state.mouseX }
                        : undefined
                    }
                    keepMounted
                    open={this.state.contextMenu}
                    onClose={() => this.setState({contextMenu: false})}
                >
                    <MenuItem onClick={() => this.onEditClick()}>Edit</MenuItem>
                    <MenuItem onClick={() => this.onDeleteClick(true)}>Delete</MenuItem>
                    <MenuItem onClick={() => this.onDeleteClick(false)}>Delete for me</MenuItem>
                </Menu>

                <Menu
                    anchorReference="anchorPosition"
                    anchorPosition={
                      this.state.mouseY !== null && this.state.mouseX !== null
                        ? { top: this.state.mouseY, left: this.state.mouseX }
                        : undefined
                    }
                    keepMounted
                    open={this.state.replyContextMenu}
                    onClose={() => this.setState({replyContextMenu: false})}
                >
                    <MenuItem onClick={() => this.onReplyClick()}>Reply</MenuItem>
                </Menu>
            </div>
        );
    }
}

export default Chat;