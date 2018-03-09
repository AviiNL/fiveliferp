import React, {Component} from 'react';
import ChatMessageService from '../service/ChatMessageService';
import parser from 'bbcode-to-react';
import '../service/BreaklineTag';

class ChatMessages extends Component
{

    state = {
        messages: []
    };

    constructor (props)
    {
        super(props);
        this.timeout = null;
        this.isOpen  = false;
        this.onEvent = this.onEvent.bind(this);
    }

    componentDidMount ()
    {
        // Load existing messages
        this.setState({
            messages: ChatMessageService.messages
        });

        this.chat    = window.jQuery('#chat .messages');

        window.addEventListener('message', this.onEvent);
    }

    componentWillUnmount ()
    {
        window.removeEventListener('message', this.onEvent);
    }

    onEvent (event)
    {
        let data = event.data;
        if (!data.hasOwnProperty('component') || data.component !== 'chat') {
            return;
        }

        switch (data.type) {
            case 'ON_MESSAGE':
                ChatMessageService.messages.push(data.message);
                this.setState({
                    messages: ChatMessageService.messages
                });
                this.chat.show();
                if (!this.isOpen) {
                    clearTimeout(this.timeout);
                    this.timeout = setTimeout(() => this.chat.fadeOut('slow'), 5000);
                }
                break;
            case 'ON_OPEN':
                this.isOpen = true;
                clearTimeout(this.timeout);
                this.chat.show();
                break;
            case 'ON_CLOSE':
                this.isOpen = false;
                clearTimeout(this.timeout);
                this.timeout = setTimeout(() => this.chat.fadeOut('slow'), 5000);
                break;
        }
    }

    render ()
    {
        return (
            <div className={'messages'}>
                <div className={'container'}>
                    <ul>
                        {this.state.messages.map((m, i) => {
                            return <li key={i}>{parser.toReact(m)}</li>;
                        })}
                    </ul>
                </div>
            </div>
        );
    }
}

export default ChatMessages;


/*
var scrolled = false;
function updateScroll(){
    if(!scrolled){
        var element = document.getElementById("yourDivID");
        element.scrollTop = element.scrollHeight;
    }
}

$("#yourDivID").on('scroll', function(){
    scrolled=true;
    if(element.scrollTop == element.scrollHeight) {
        scrolled = false;
    }
});
 */