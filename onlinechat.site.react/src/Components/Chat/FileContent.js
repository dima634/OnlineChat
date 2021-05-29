import React from 'react'
import { Chip } from '@material-ui/core';
import InsertDriveFileIcon from '@material-ui/icons/InsertDriveFile';

class FileContent extends React.Component {
    render(){
        const isImage = this.props.filename.match(/.(jpg|jpeg|png)$/i);
        let content;
        if (isImage) {
            content = <img style={{maxWidth: 400}} src={this.props.url} alt="Boobs..."></img>;
        } else {
            content = <Chip
                color="primary"
                icon={<InsertDriveFileIcon/>}
                label={<a style={{textDecoration: "none", color: "inherit"}} href={this.props.url} download={this.props.filename}>{this.props.filename}</a>}>
            </Chip>;
        }

        return content;
    }
}

export default FileContent;