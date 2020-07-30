import * as signalR from "@microsoft/signalr";
import Api from '../WebApi/WebApiClient'

let HUB_ADDRESS = null;

if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
    HUB_ADDRESS = "https://localhost:44305/instantmessaging";
} else {
    HUB_ADDRESS = "https://onlinechatwebapi.azurewebsites.net/instantmessaging";
}

class InstantMessager {
    constructor(){
        this.messageReceived = [];
        this.messageEdited = [];
        this.messageDeleted = [];
        this.messageRead = [];
        this.chatCreated = [];

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_ADDRESS, { accessTokenFactory: () => Api.instance().token})
            .withAutomaticReconnect()
            .build();

        this.onMessageReceived = this.onMessageReceived.bind(this);
        this.onMessageEdited = this.onMessageEdited.bind(this);
        this.onMessageDeteled = this.onMessageDeteled.bind(this);
        this.onMessageRead = this.onMessageRead.bind(this);
        this.onChatCreated = this.onChatCreated.bind(this);

        this.connection.on("MessageSent", this.onMessageReceived);
        this.connection.on("MessageEdited", this.onMessageEdited);
        this.connection.on("MessageDeleted", this.onMessageDeteled);
        this.connection.on("MessageRead", this.onMessageRead);
        this.connection.on("ChatCreated", this.onChatCreated);
    }

    async onlineAsync() {
        await this.connection.start();
    }

    async offlineAsync() {
        await this.connection.stop();
    }

    onMessageReceived(message, chatId) {
        this.messageReceived.forEach(callback => {
            callback({
                message: message,
                chatId: chatId
            });
        });
    }

    onMessageEdited(message, chatId) {
        this.messageEdited.forEach(callback => {
            callback({
                message: message,
                chatId: chatId
            });
        });
    }

    onMessageDeteled(messageId, chatId, forAll, author) {
        this.messageDeleted.forEach(callback => {
            callback({
                messageId: messageId,
                chatId: chatId,
                forAll: forAll,
                author: author
            });
        });
    }

    onChatCreated(chat) {
        this.chatCreated.forEach(callback => {
            callback(chat);
        });
    }

    onMessageRead(messageId, chatId) {
        this.messageRead.forEach(callback => {
            callback({
                messageId: messageId,
                chatId: chatId
            });
        });
    }
}

export default InstantMessager;