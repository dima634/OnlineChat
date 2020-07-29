import React from 'react'
import './Message.css'
import DoneIcon from '@material-ui/icons/Done';
import DoneAllIcon from '@material-ui/icons/DoneAll';
import TextContent from './TextContent'

class Message extends React.Component {
    render(){
        let readStatus;
        let editStatus;
        let messageContent;
        let messageBoxClass = "message-box";

        if(this.props.message.Author === this.props.api.username){
            messageBoxClass += " message-box-mine";
        }

        if(this.props.message.IsEdited){
            editStatus = <p>Edited</p>;
        }

        if(this.props.message.IsRead){
            readStatus = <DoneAllIcon/>;
        }
        else{
            readStatus = <DoneIcon/>
        }

        if(this.props.message.ContentType === 'Text'){
            messageContent = <TextContent text={this.props.message.Content}/>
        }
        else {
            throw Error('Unknown content type');
        }

        return (
            <li className={messageBoxClass}>
                <div className="message-header">
                    <p>{this.props.message.Author}</p>
                    <p>{this.props.message.SentOn}</p>
                </div>

                {messageContent}

                <div className="message-footer">
                    {readStatus}
                    {editStatus}
                </div>
            </li>
        );
    }
}

export default Message;