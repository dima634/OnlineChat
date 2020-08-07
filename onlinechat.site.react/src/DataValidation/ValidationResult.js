import React from 'react';
import './ValidationResult.css'

class ValidationError extends React.Component {
    render(){
        return (
            <li className="validation-result-message">
                <p>{this.props.message}</p>
            </li>
        );
    }
}

export default class ValidationResult extends React.Component {
    render(){
        return (
            <ul>
                {this.props.messages.map(message => <ValidationError message={message}/>)}
            </ul>
        );
    }
}