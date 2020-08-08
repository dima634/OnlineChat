import React, { Fragment } from 'react';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import './App.css';
import { Typography, Button, IconButton, Divider } from '@material-ui/core';
import LoginDialog from './Components/Auth/LoginDialog';
import { SnackbarProvider } from 'notistack';
import Api from './WebApi/WebApiClient';
import InstantMessager from './InstantMessaging/InstantMessager';
import RegisterDialog from './Components/Auth/RegisterDialog';
import Navbar from './Components/Chat/Navbar';
import Chat from './Components/Chat/Chat';
import Drawer from '@material-ui/core/Drawer';
import MenuIcon from '@material-ui/icons/Menu';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';

class App extends React.Component {
  constructor(props){
    super(props);

    this.messager = new InstantMessager();

    this.state = {
      isLoginDialogOpen: true,
      isRegisterDialogOpen: false,
      chats: [],
      selectedChatId: null,
      sidebarOpen: false
    };

    this.onChatCreated = this.onChatCreated.bind(this);
  }

  onChatCreated(chat){
    this.state.chats.push(chat);
    this.forceUpdate();
  }

  resetState(){
    this.setState({
      isLoginDialogOpen: true,
      isRegisterDialogOpen: false,
      chats: [],
      selectedChatId: null
    });
  }

  componentDidMount(){
    this.messager.chatCreated.push(this.onChatCreated);
  }

  render() {
    let username = Api.instance().username;
    let authBar;
    let chat = null;

    if(username == null) {
      authBar = <Button color="inherit" onClick={() => this.setState({isLoginDialogOpen: true})}>Log in</Button>;
    }
    else {
      const onLogoutClick = () => {
        this.messager.offlineAsync();
        Api.instance().logout();
        this.resetState();
      };

      authBar = (
        <Fragment>
          <Typography>Hello, {username}</Typography>
          <Button color="inherit" onClick={() => onLogoutClick()}>Log out</Button>
        </Fragment>
      );
    }

    const onSuccessLogin = async () => {
      this.messager.onlineAsync();
      this.setState({
        isLoginDialogOpen: false,
        chats: await Api.instance().getChatsAsync(),
        sidebarOpen: true
      });
    };

    if(this.state.selectedChatId != null){
      chat = <Chat chatId={this.state.selectedChatId} api={Api.instance()} messager={this.messager}/>
    }

    let contentClass = "content";
    let appbarClass = "appbar";

    if(this.state.sidebarOpen) { 
      contentClass+=" shift";
      appbarClass+=" shift";
    }

    return (
      <SnackbarProvider maxSnack={5}>
        <AppBar className={appbarClass} position="static">
          <Toolbar>
            <IconButton
              disabled={this.state.sidebarOpen}
              color="inherit"
              edge="start"
              onClick={() => this.setState({sidebarOpen: true})}
            >
              <MenuIcon/>
            </IconButton>
            <Typography className="title" variant="h6">Online chat</Typography>
            {authBar}
          </Toolbar>

          <LoginDialog 
            onClose={() => this.setState({isLoginDialogOpen: false}) } 
            onSuccessLogin={() => onSuccessLogin()}
            onLinkClicked={() => this.setState({isRegisterDialogOpen: true, isLoginDialogOpen: false})}
            isOpen={this.state.isLoginDialogOpen}
            />
          <RegisterDialog
            onClose={() => this.setState({isRegisterDialogOpen: false}) }  
            onLinkClicked={() => this.setState({isRegisterDialogOpen: false, isLoginDialogOpen: true})}
            isOpen={this.state.isRegisterDialogOpen}
          />
        </AppBar>

        <Drawer
          className="sidebar"
          variant="persistent"
          anchor="left"
          open={this.state.sidebarOpen}
        >
          <div className="sidebar-header">
            <IconButton onClick={() => this.setState({sidebarOpen: false})}>
              <ChevronLeftIcon/>
            </IconButton>
          </div>

          <Divider/>
          
          <Navbar chats={this.state.chats} api={Api.instance()} messager={this.messager} onChatSelected={(chatId) => this.setState({selectedChatId: chatId})}/>
        </Drawer>
        
        <main className={contentClass}>
          {chat}
        </main>
      </SnackbarProvider>
    );
  }
}

export default App;
