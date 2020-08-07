import ValidatorBase from './ValidatorBase';

export default class RegExValidator extends ValidatorBase {
    validate(component){
        const regex = component.props.regex;
        if(regex) return regex.test(component.props.value);
        else return true;
    }
}