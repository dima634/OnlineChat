import React from 'react'
import MessageList from './MessageList'

class Chat extends React.Component {
    constructor(props){
        super(props);

        this.loadedAllMessages = false;

        this.state = {
            messages: []
        };
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
        return (
            <MessageList messages={this.state.messages} api={this.props.api} onTop={() => this.loadMoreMessages()}/>
        );
    }
}

export default Chat;