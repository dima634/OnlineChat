import React from 'react'

class TextContent extends React.Component {
    render(){
        return (
            <p>{this.props.text.split('\n').map((line) => <>{line}<br/></>)}</p>
        );
    }
}

export default TextContent;