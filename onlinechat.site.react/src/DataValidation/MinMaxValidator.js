import ValidatorBase from './ValidatorBase';

export default class MinMaxValidator extends ValidatorBase {
    validate(component) {
        const minProp = component.props.min;
        const maxProp = component.props.max;
        
        if(minProp && component.props.value < minProp) return false;
        if(maxProp && component.props.value > maxProp) return false;

        return true;
    }
}