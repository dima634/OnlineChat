import React from 'react'
import './Navbar.css'
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import Api from '../../WebApi/WebApiClient'

class Navbar extends React.Component {
    render() {
        return (
            <List component="nav">
                {this.props.chats.map((chat) => <ChatLink onChatClick={this.props.onChatSelected} key={chat.Id} chat={chat}/>)}
            </List>
        );
    }
}

function ChatLink(props) {
    let chatName;
    if(props.chat.Name !== undefined){
        chatName = props.chat.Name
    }
    else{
        chatName = props.chat.Members.find(user => user !== Api.instance().username)
    }

    return (
        <ListItem onClick={() => props.onChatClick(props.chat.Id)} button>
            <ListItemText primary={chatName}/>
        </ListItem>
    );
}

export default Navbar;