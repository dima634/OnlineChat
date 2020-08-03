import React from 'react'
import './Navbar.css'
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import Api from '../../WebApi/WebApiClient';
import Badge from '@material-ui/core/Badge';
import CreateChatDialog from './CreateChatView';
import { Button, Box } from '@material-ui/core';

class Navbar extends React.Component {
    constructor(props){
        super(props);

        this.state = {
            isCreateChatDialogOpen: false
        };
    }

    render() {
        return (
            <Box>
                <Button className="navbar-create-chat-button" variant="outlined" color="primary" onClick={() => this.setState({ isCreateChatDialogOpen: true })}>New chat</Button>
                <List component="nav">
                    {this.props.chats.map((chat) => <ChatLink onChatClick={this.props.onChatSelected} 
                                                                messager={this.props.messager} 
                                                                key={chat.Id} 
                                                                chat={chat}
                                                                api={this.props.api}/>)}
                </List>

                <CreateChatDialog 
                    isOpen={this.state.isCreateChatDialogOpen}
                    onClose={() => this.setState({ isCreateChatDialogOpen: false })}
                    api = {this.props.api}
                 />
            </Box>
        );
    }
}

class ChatLink extends React.Component {
    constructor(props){
        super(props);

        this.state = {
            unreadMessagesCount: this.props.chat.UnreadByCurrentUserMessagesCount
        };

        this.onMessageRead = this.onMessageRead.bind(this);
        this.onMessageReceived = this.onMessageReceived.bind(this);
    }

    onMessageRead(args){
        if(this.props.chat.Id === args.chatId && this.props.api.username === args.readBy){
            this.setState({unreadMessagesCount: this.state.unreadMessagesCount - 1});
        }
    }

    onMessageReceived(args){
        if(this.props.chat.Id === args.chatId && !args.message.IsReadByCurrentUser){
            this.setState({unreadMessagesCount: this.state.unreadMessagesCount + 1});
        }
    }

    componentDidMount(){
        this.props.messager.messageRead.push(this.onMessageRead);
        this.props.messager.messageReceived.push(this.onMessageReceived);
    }

    render (){
        let chatName;
        if(this.props.chat.Name !== undefined){
            chatName = this.props.chat.Name
        }
        else{
            chatName = this.props.chat.Members.find(user => user !== Api.instance().username)
        }

        return (
            <ListItem onClick={() => this.props.onChatClick(this.props.chat.Id)} button>
                <ListItemText primary={chatName}/>
                <Badge badgeContent={this.state.unreadMessagesCount} color="primary"/>
            </ListItem>
        );
    }
}

export default Navbar;