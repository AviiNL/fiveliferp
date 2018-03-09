import React, {Component} from 'react';
import {inspect} from 'util';

class ChatInput extends Component
{

    constructor (props)
    {
        super(props);

        this.onEvent = this.onEvent.bind(this);
    }

    componentWillUnmount ()
    {
        window.removeEventListener('message', this.onEvent);
        window.jQuery(document).unbind('keydown');
    }

    componentDidMount ()
    {
        this.input          = window.jQuery('input[type="text"].chat');
        this.inputContainer = window.jQuery('#chat .input');

        window.addEventListener('message', this.onEvent);

        window.jQuery(document).keydown((e) => {
            if (e.keyCode === 27) {
                this.inputContainer.fadeOut('fast');

                window.jQuery.post('http://fivelife/chat.result', JSON.stringify({state: 'cancel'}));
            }

            if (e.keyCode === 13) {
                this.inputContainer.fadeOut('fast');

                window.jQuery.post('http://fivelife/chat.result', JSON.stringify({message: this.input.val()}));
            }
        });
    }

    onEvent (event)
    {
        let data = event.data;

        if (!data.hasOwnProperty('component') || data.component !== 'chat') {
            return;
        }

        switch (data.type) {
            case 'ON_OPEN':
                this.input.val('');

                this.inputContainer.show();

                this.input.focus();

                break;
        }
    }

    render ()
    {
        return (
            <div className={'input'}>
                <input type={'text'} className="chat" />
            </div>
        );
    }
}

export default ChatInput;