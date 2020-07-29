let BASE_ADDRESS = null;

if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
    BASE_ADDRESS = "https://localhost:44305/api/";
} else {
    BASE_ADDRESS = "https://onlinechatwebapi.azurewebsites.net/api/";
}

class Api {
    static _instance = null;

    /**
     * @returns {Api} Sum of a and b or an array that contains a, b and the sum of a and b.
     */
    static instance() {
        if(Api._instance == null){
            Api._instance = new Api();
        }

        return Api._instance;
    }

    constructor(){
        this.token = null;
        this.username = null;
    }

    async loginAsync(username, password){
        var response = await this.postAsync("account/token", {
            Username: username,
            Password: password
        });

        this.ensureOk(response);

        this.token = (await response.json()).token;
        this.username = username;
    }

    async registerAsync(username, password, confirmPassword){
        var response = await this.postAsync("account/register", {
            Username: username,
            Password: password,
            ConfirmPassword: confirmPassword
        });

        this.ensureOk(response);
    }

    logout(){
        this.username = null;
        this.token = null;
    }

    async getChatsAsync(){
        let response = await this.getAsync('chat/list');
        this.ensureOk(response);
        return response.json();
    }

    async getChatMessagesAsync(chatId, offset, resultsCount = 20){
        let response = await this.getAsync(`chat/${chatId}/messages/?offset=${offset}&resultsPerPage=${resultsCount}`);
        this.ensureOk(response);
        return response.json();
    }

    ensureOk(response) {
        if(response.status !== 200) throw new Error(response.json().message);
    }

    requestParams(method = "get", obj = null){
        var params = {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            }
        };

        if(obj != null){
            params.body = JSON.stringify(obj);
        };

        if(this.token != null){
            params.headers['Authorization'] = 'Bearer ' + this.token;
        }

        return params;
    }

    async getAsync(url) {
        return await fetch(BASE_ADDRESS + url, this.requestParams());
    }

    async postAsync(url, obj){
        var params = this.requestParams('post', obj);

        return await fetch(BASE_ADDRESS + url, params);
    }
}

export default Api;