import React, { Component } from 'react';
import * as objectPath from 'object-path';
import './style.scss';

class IDForm extends Component {

    constructor(props) {
        super(props);

        console.log(JSON.stringify(this.props.data));


        this.state = this.props.data;
    }

    handleChange(key) {
        return (event) => {
            let d = this.state;
            objectPath.set(d, key, event.target.value.trim());
            this.setState(d);
        }
    }

    handleDoB(key) {
        return (event) => {

            console.log(event.target.value);

            let date = event.target.value.split('-');
            let year = parseInt(date.shift());
            let month = parseInt(date.shift());
            let day = parseInt(date.shift());

            let d = this.state;

            objectPath.set(d, key, new Date(year, month - 1, day + 1));

            objectPath.set(d, "Cash", 500);
            objectPath.set(d, "Bank", 500);

            this.setState(d);
        }
    }

    onSubmit() {
        window.jQuery.post('http://fivelife/character_finished', JSON.stringify(this.state));
    }

    onCancel() {
        window.jQuery.post('http://fivelife/character_cancel');
    }

    render() {
        return (
            <div id="IDForm">
                <div className="outer">
                    <div className="middle">
                        <div className="container">
                            <div className="card">
                                <h1>Identification</h1>
                                <p>
                                    <label>
                                        First name:
                                        <input type={"text"} value={this.state.FirstName} onChange={this.handleChange("FirstName").bind(this)} />
                                    </label>
                                </p>
                                <p>
                                    <label>
                                        Last name:
                                        <input type={"text"} value={this.state.LastName} onChange={this.handleChange("LastName").bind(this)} />
                                    </label>
                                </p>
                                <p>
                                    <label>
                                        Date of Birth:
                                        <input type={"date"} onBlur={this.handleDoB("DateOfBirth").bind(this)} />
                                    </label>
                                </p>
                                <p style={{ float: "right" }}>
                                    <input type={"button"} onClick={this.onCancel.bind(this)} value={"Cancel"} />
                                    <input type={"button"} onClick={this.onSubmit.bind(this)} value={"Submit"} />
                                </p>
                                <div style={{ clear: "both" }}>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default IDForm;

/*
<div>
    <input type={"text"} value={this.state.Name.FirstName} onChange={this.handleChange("Name.FirstName").bind(this)}/><br/>
    <input type={"text"} value={this.state.Name.LastName} onChange={this.handleChange("Name.LastName").bind(this)} /><br/>
    <input type={"button"} onClick={this.onSubmit.bind(this)} value={"Submit"} /><br/>
    <input type={"button"} onClick={this.onCancel.bind(this)} value={"Cancel"} /><br/>
</div>
 */