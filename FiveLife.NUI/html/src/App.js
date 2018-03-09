import React, {Component} from 'react';
import './App.scss';
import Home from './scenes/Home';
import CharacterSelect from './scenes/CharacterSelect';
import IDForm from './scenes/IDForm';
import Game from './scenes/Game';
import {inspect} from 'util';

class App extends Component
{

    state = {
        MyComponent: Home,
        data:        {}
    };

    constructor (props)
    {
        super(props);

        let Scenes = {
            Home:            Home,
            CharacterSelect: CharacterSelect,
            IDForm:          IDForm,
            Game:            Game,
        };

        window.addEventListener('message', (event) => {
            if (!event.data.page) {
                return;
            }
            
            this.setState({
                MyComponent: Scenes[event.data.page],
                data:        event.data.data
            });

        });
    }

    render ()
    {
        return (
            <div>
                <this.state.MyComponent data={this.state.data} />
            </div>
        );
    }
}

export default App;
