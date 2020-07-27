import React from 'react';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Dialog from '@material-ui/core/Dialog';
import DialogContent from '@material-ui/core/DialogContent';
import Avatar from '@material-ui/core/Avatar';
import CssBaseline from '@material-ui/core/CssBaseline';
import Link from '@material-ui/core/Link';
import PersonOutlineIcon from '@material-ui/icons/PersonOutline';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import { withSnackbar } from 'notistack';
import './RegisterDialog.css'
import Api from '../../WebApi/WebApiClient'

class RegisterDialog extends React.Component {
    constructor(props){
        super(props);

        this.state = {
            username: "",
            password: "",
            confirmPassword: "",
            errorMessage: null
        };

        this.onUsernameChanged = this.onUsernameChanged.bind(this);
        this.onPasswordChanged = this.onPasswordChanged.bind(this);
        this.onPasswordChanged = this.onConfirmPasswordChanged.bind(this);
    }

    onUsernameChanged(event){
        this.setState({username: event.target.value});
    }

    onPasswordChanged(event){
        this.setState({password: event.target.value});
    }

    onConfirmPasswordChanged(event){
        this.setState({confirmPassword: event.target.value});
    }

    async onLoginClickAsync(){
        var api = Api.instance();

        try {
            await api.loginAsync(this.state.username, this.state.password);   
            this.props.onSuccessLogin();
        } catch (error) {
            this.props.enqueueSnackbar(error.errorMessage, { 
                variant: 'error',
                anchorOrigin: {
                    vertical: 'bottom',
                    horizontal: 'center',
            }});
        }
    }

    render() {
      return (
        <Dialog onClose={this.props.onClose} open={this.props.isOpen}>
            <DialogContent>
                <Container component="main" maxWidth="xs">
                    <CssBaseline />
                    <div className="paper">
                        <Avatar className="avatar">
                            <PersonOutlineIcon/>
                        </Avatar>
                        <Typography component="h1" className="text-center" variant="h5">
                            Sign Up
                        </Typography>
                        <TextField
                            variant="outlined"
                            margin="normal"
                            required
                            fullWidth
                            label="Username"
                            name="username"
                            autoFocus
                            value={this.state.username}
                            onChange={this.onUsernameChanged}
                        />
                        <TextField
                            variant="outlined"
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            label="Password"
                            type="password"
                            value={this.state.password}
                            onChange={this.onPasswordChanged}
                        />
                        <TextField
                            variant="outlined"
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            label="Password"
                            type="password"
                            value={this.state.confirmPassword}
                            onChange={this.onConfirmPasswordChanged}
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            color="primary"
                            className="submit"
                            onClick={() => this.onLoginClickAsync()}
                        >
                            Sign Up
                        </Button>
                        <Link onClick={() => this.props.onLinkClicked()}>
                            <Typography className="text-center">
                                Already have an account? Sign in
                            </Typography>
                        </Link>
                    </div>
                </Container>
            </DialogContent>
        </Dialog>
      );
    }
  }
  
  export default withSnackbar(RegisterDialog);