import React from 'react'
import Message from './Message'
import './MessageList.css'

class MessageList extends React.Component {
    constructor(props){
        super(props);

        this.list = React.createRef();

        this.onMessageInViewport = this.onMessageInViewport.bind(this);
    }

    onScroll(ev){
        if(this.list.current.scrollTop === 0){
            this.props.onTop();
        }
    }

    onMessageInViewport(message){
        this.props.onMessageInViewport(message);
    }

    render(){
        return (
            <ul className="message-list" onScroll={() => this.onScroll()} ref={this.list}>
                {this.props.messages.map((message) => <Message onContextMenu={this.props.onContextMenu} 
                                                                message={message} 
                                                                key={message.Id} 
                                                                api={this.props.api}
                                                                onEnterViewport={() => this.onMessageInViewport(message)}
                                                            />)}
            </ul>
        );
    }
}

export default MessageList;