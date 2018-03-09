import React, {Component} from 'react';
import Chat from './components/Chat';

class Game extends Component
{
    render() {
        return (
            <div style={{width: '100%', height: '100%'}}>
                <Chat />
                {/*<Guide />*/}
            </div>
        );
    }
}

export default Game;