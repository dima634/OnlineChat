import React, { Fragment } from 'react';
import Dialog from '@material-ui/core/Dialog';
import DialogContent from '@material-ui/core/DialogContent';
import { DialogTitle, MenuItem, DialogActions, Button, TextField, InputAdornment, Divider, Box, Chip } from '@material-ui/core';
import Select from '@material-ui/core/Select';
import AccountCircle from '@material-ui/icons/AccountCircle';
import GroupIcon from '@material-ui/icons/Group';
import Autocomplete from '@material-ui/lab/Autocomplete';
import CircularProgress from '@material-ui/core/CircularProgress';
import { withSnackbar } from 'notistack';
import './CreateChatView.css';

class CreateChatDialog extends React.Component {
    constructor(props){
        super(props);

        this.state = {
            chatType: "Direct",
            loadingSuggestions: false,
            suggestedUsernames: [],
            members: [],
            selectedUser: null,
            groupName: ""
        };
        
        this.submit = this.submit.bind(this);
        this.onChange = this.onChange.bind(this);
    }

    submit(){
        if(this.state.chatType === "Direct"){
            this.props.api.createDirectChatAsync(this.state.selectedUser);
        }
        else{
            this.props.api.createGroupAsync(this.state.members, this.state.groupName);
        } 

        this.props.onClose();
    }

    onChange(event){
        if(event.target.value !== ""){
            this.setState({
                loadingSuggestions: true
            });

            this.props.api.getUsenamesStartsWithAsync(event.target.value)
                .then(data => {
                    this.setState({
                        loadingSuggestions: false, 
                        suggestedUsernames: data
                    })
                });
        }
    }

    render(){
        let afterSelect;
        const usernameAutocomplete = (
            <Autocomplete
                className="create-chat-autocomplete"
                options={this.state.suggestedUsernames}
                loading={this.state.loadingSuggestions}
                onChange={(event, newValue) => this.setState({selectedUser: newValue})}
                renderInput={(params) => (
                    <TextField
                        {...params}
                        label="Username"
                        variant="outlined"
                        onChange={this.onChange}
                        InputProps={{
                            ...params.InputProps,
                            endAdornment: (
                            <React.Fragment>
                                {this.state.loadingSuggestions ? <CircularProgress color="inherit" size={20} /> : null}
                                {params.InputProps.endAdornment}
                            </React.Fragment>
                            ),
                        }}
                    />)}
            />
        );

        if(this.state.chatType === 'Direct'){
            afterSelect = usernameAutocomplete;
        }
        else {
            afterSelect = (
                <Fragment>
                    <TextField 
                        className="create-chat-input" 
                        variant="outlined" 
                        label="Group name"
                        value={this.state.groupName}
                        onChange={(event) => this.setState({groupName: event.target.value})}
                        InputProps={{
                            startAdornment: (
                                <InputAdornment position="start">
                                    <GroupIcon color="primary"/>
                                </InputAdornment>
                            ),
                        }}
                    />

                    <Divider/>

                    <Box display="flex">
                        {usernameAutocomplete}
                        <Button onClick={() => {
                                    if(this.state.selectedUser && 
                                       this.state.selectedUser !== null && 
                                       this.state.selectedUser !== "" &&
                                       !this.state.members.find(member => member === this.state.selectedUser) &&
                                       this.state.selectedUser !== this.props.api.username)
                                    {
                                        this.state.members.push(this.state.selectedUser);
                                        this.setState({members: this.state.members});
                                    }
                                }} 
                        className="create-chat-input">Add</Button>
                    </Box>

                    <Box display="flex" flexWrap="wrap">
                        <Chip className="create-chat-chip" color="primary" label={this.props.api.username}/>
                        {this.state.members.map(username => 
                            <Chip 
                                className="create-chat-chip" 
                                color="primary" 
                                variant="outlined" 
                                label={username} 
                                onDelete={() => {
                                    const idx = this.state.members.findIndex(member => member === username);
                                    this.state.members.splice(idx, 1);
                                    this.setState({members: this.state.members});
                                }}
                            />)
                        }
                    </Box>
                </Fragment>
            );
        }

        return (
            <Dialog onClose={this.props.onClose} open={this.props.isOpen}>
                <DialogTitle>Create chat</DialogTitle>
                <DialogContent>
                    <Select className="create-chat-input" value={this.state.chatType} variant="outlined" onChange={(event) => this.setState({chatType: event.target.value})}>
                        <MenuItem value={"Direct"}>Direct</MenuItem>
                        <MenuItem value={"Group"}>Group</MenuItem>
                    </Select>
                    {afterSelect}
                </DialogContent>
                <DialogActions>
                    <Button color="primary" onClick={() => this.submit()}>Create</Button>
                </DialogActions>
            </Dialog>
        );
    }
}

export default withSnackbar(CreateChatDialog);