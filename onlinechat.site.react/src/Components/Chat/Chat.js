import React from 'react'
import MessageList from './MessageList'
import { TextField } from '@material-ui/core';
import SendIcon from '@material-ui/icons/Send';
import EditIcon from '@material-ui/icons/Edit';
import CloseIcon from '@material-ui/icons/Close';
import IconButton from '@material-ui/core/IconButton';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import './Chat.css';
import TextContent from './TextContent';

class Chat extends React.Component {
    constructor(props){
        super(props);

        this.loadedAllMessages = false;

        this.state = {
            messages: [],
            messageText: "",
            editMessage: null,
            contextMenu: false,
            mouseY: 0,
            mouseX: 0
        };

        this.handleChange = this.handleChange.bind(this);
        this.onMessageReceived = this.onMessageReceived.bind(this);
        this.onMessageDeleted = this.onMessageDeleted.bind(this);
        this.onMessageEdited = this.onMessageEdited.bind(this);

        this.props.messager.messageReceived.push(this.onMessageReceived);
        this.props.messager.messageDeleted.push(this.onMessageDeleted);
        this.props.messager.messageEdited.push(this.onMessageEdited);
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
            }
        }
    }

    onMessageEdited(args){
        if(args.chatId === this.props.chatId) {
            let idx = this.state.messages.findIndex(mes => mes.Id === args.message.Id);
            this.state.messages[idx].Content = args.message.Content;
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
            ReplyTo: null
        };

        this.props.api.sendMessageAsync(model, this.props.chatId);
        this.setState({messageText: ""});
    }

    sendOrEditMessage(){
        if(this.state.editMessage !== null){
            this.editMessage();
        }
        else{
            this.sendMessage();
        }
    }

    onContextMenu(ev, message){
        if(message.Author === this.props.api.username){
            this.contextMenuMessage = message;
            this.setState({
                contextMenu: true,
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
            editMessage: this.contextMenuMessage,
            messageText: this.contextMenuMessage.Content,
            contextMenu: false
        });
    }

    componentDidMount(){
        this.props.api.getChatMessagesAsync(this.props.chatId, 0)
            .then(messages => this.setState({messages: messages}));
    }

    componentDidUpdate(prevProps, prevState, snapshot){
        if(prevProps.chatId !== this.props.chatId){
            this.loadedAllMessages = false;
            this.props.api.getChatMessagesAsync(this.props.chatId, 0)
                .then(messages => this.setState({messages: messages}));
        }
    }

    render() {
        let replyBox;

        if(this.state.editMessage !== null){
            replyBox = (
                <div className="chat-reply-box">
                    <EditIcon color="primary"/>

                    <div className="chat-reply-box-content">
                        <TextContent text={this.state.editMessage.Content}/>
                    </div>
                    
                    <IconButton onClick={() => this.setState({ editMessage: null, messageText: "" })} color="primary">
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
                    onTop={() => this.loadMoreMessages()}
                    onContextMenu={ (ev, mes) => this.onContextMenu(ev, mes) }
                />

                {replyBox}

                <div className="chat-message-input-box">
                    <TextField 
                        onChange={this.handleChange}
                        value={this.state.messageText} 
                        size="medium" 
                        className="chat-message-input" 
                        rowsMax={10} 
                        placeholder="Send message..." 
                        multiline
                        autoFocus
                    />
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
                </Menu>
            </div>
        );
    }
}

export default Chat;