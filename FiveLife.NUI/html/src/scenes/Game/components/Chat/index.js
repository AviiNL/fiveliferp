import React, {Component} from 'react';
import ChatMessages from './components/ChatMessages';
import ChatInput from './components/ChatInput';

import './style.scss';

class Chat extends Component
{
    render ()
    {
        return (
            <div id={'chat'}>
                <ChatMessages />
                <ChatInput />
            </div>
        );
    }
}

export default Chat;