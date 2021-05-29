import React from 'react'
import './Message.css'
import DoneIcon from '@material-ui/icons/Done';
import DoneAllIcon from '@material-ui/icons/DoneAll';
import TextContent from './TextContent'
import FileContent from './FileContent'
import handleViewport from 'react-in-viewport';

class Message extends React.Component {
    constructor(props){
        super(props);

        this.element = React.createRef();
    }

    renderContent(message){
        if (message.ContentType === 'Text') {
            return <TextContent text={message.Content}/>
        } else if (message.ContentType === 'File') {
            console.log(message);
            return <FileContent url={message.Content.Url} filename={message.Content.Filename}/>
        } else {
            throw Error('Unknown content type');
        }
    }

    render(){
        let readStatus;
        let editStatus;
        const messageContent = this.renderContent(this.props.message);;
        let messageBoxClass = "message-box";
        let reply;

        if(this.props.message.ReplyTo) {
            if (this.props.message.ReplyTo == null) {
                reply = <p>Deleted</p>
            } else {
                reply = (
                    <div className="message-reply">
                        {this.renderContent(this.props.message.ReplyTo)}
                    </div>
                );
            }
        }

        if(this.props.message.Author === this.props.api.username){
            messageBoxClass += " message-box-mine";
        }

        if(this.props.message.IsEdited){
            editStatus = <p>Edited</p>;
        }

        if(this.props.message.Author === this.props.api.username){
            if(this.props.message.IsRead){
                readStatus = <DoneAllIcon/>;
            }
            else{
                readStatus = <DoneIcon/>
            }
        }

        return (
            <li 
                onContextMenu={(ev) => { ev.preventDefault(); this.props.onContextMenu(ev, this.props.message); }} 
                className={messageBoxClass}
                ref={this.element}
            >
                <div className="message-header">
                    <p>{this.props.message.Author}</p>
                    <p>{this.props.message.SentOn}</p>
                </div>

                {reply}

                {messageContent}

                <div className="message-footer">
                    {readStatus}
                    {editStatus}
                </div>
            </li>
        );
    }
}

export default handleViewport(Message, {}, {disconnectOnLeave: true});