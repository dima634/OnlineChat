import ValidatorBase from './ValidatorBase';

export default class CustomValidator extends ValidatorBase {
    validate(component){
        const validationFunction = component.props.validate;
        if(validationFunction && !validationFunction(component)) return false;
        else return true;
    }
}