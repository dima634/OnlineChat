
export default class ValidatorBase {
    validateHierarchy(component, errors){
        if(!(component instanceof Object)) return true;

        let componentValidationResult;
        
        if(component instanceof Array){
            componentValidationResult = this.validateArray(component, errors);
        }
        else {
            componentValidationResult = this.validate(component);

            if(!componentValidationResult) {
                const errorMessage = component.props.errorMessage;

                if(errorMessage) errors.push(errorMessage);
            }

            componentValidationResult = this.validateChildren(component, errors) && componentValidationResult;
        }

        return componentValidationResult;
    }

    validateChildren(component, errors) {
        let componentValidationResult = true;

        if(component.props.children){
            if(component.props.children instanceof Array){
                component.props.children.forEach(child => {
                    componentValidationResult = this.validateHierarchy(child, errors) && componentValidationResult;
                });
            }
            else {
                componentValidationResult = this.validateHierarchy(component.props.children, errors) && componentValidationResult;
            }
        }

        return componentValidationResult;
    }

    validateArray(array, errors){
        let result = true;

        array.forEach(element => {
            result = this.validateHierarchy(element, errors) && result;
        });

        return result;
    }
}