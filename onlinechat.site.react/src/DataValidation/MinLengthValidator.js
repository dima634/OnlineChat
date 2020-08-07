import ValidatorBase from './ValidatorBase';

export default class MinLengthValidator extends ValidatorBase {
    validate(component){
        const minLength = component.props.minLength;
        if(minLength && component.props.value.length < minLength) return false;
        else return true;
    }
}