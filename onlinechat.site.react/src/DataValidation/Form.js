import React from 'react';
import MinMaxValidator from './MinMaxValidator';
import RegExValidator from './RegExValidator';
import MinLengthValidator from './MinLengthValidator';
import CustomValidator from './CustomValidator';

export default class Form extends React.Component {
    constructor(props){
        super(props);

        this.onSubmit = this.onSubmit.bind(this);
        this.validators = [
            new MinMaxValidator(),
            new RegExValidator(),
            new MinLengthValidator(),
            new CustomValidator()
        ];
    }

    onSubmit(event){
        event.preventDefault();

        let errors = [];
        let validationResult = true;

        this.validators.forEach(validator => {
            validationResult = validator.validateHierarchy(this, errors) && validationResult;
        });

        if(validationResult) this.props.onSuccessfulSubmit();
        else this.props.onValidationError(errors);
    }

    render(){
        return (
            <form onSubmit={(event) => this.onSubmit(event)}>
                {this.props.children}
            </form>
        );
    }
}