import React, { Fragment } from 'react';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import './App.css';
import { Typography, IconButton, Button } from '@material-ui/core';
import LoginDialog from './Components/Auth/LoginDialog';
import { SnackbarProvider } from 'notistack';
import Api from './WebApi/WebApiClient'

class App extends React.Component {
  constructor(props){
    super(props);

    this.state = {
      isLoginDialogOpen: false
    };
  }

  render() {
    let username = Api.instance().username;
    let authBar;

    if(username == null) {
      authBar = <Button onClick={() => this.setState({isLoginDialogOpen: true})}>Log in</Button>;
    }
    else {
      const onLogoutClick = () => {
        Api.instance().logout();
        this.forceUpdate();
      };

      authBar = (
        <Fragment>
          <Typography>Hello, {username}</Typography>
          <Button onClick={() => onLogoutClick()}>Log out</Button>
        </Fragment>
      );
    }

    const onSuccessLogin = () => {
      this.setState({isLoginDialogOpen: false});
      this.forceUpdate()
    };

    return (
      <SnackbarProvider maxSnack={5}>
        <AppBar position="static">
          <Toolbar>
            <Typography variant="h6">Online chat</Typography>
            {authBar}
          </Toolbar>

          <LoginDialog 
            onClose={() => this.setState({isLoginDialogOpen: false}) } 
            onSuccessLogin={() => onSuccessLogin()}
            isOpen={this.state.isLoginDialogOpen}
            />
        </AppBar>
      </SnackbarProvider>
    );
  }
}

export default App;
