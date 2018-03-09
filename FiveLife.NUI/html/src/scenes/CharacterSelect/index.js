import {Component} from 'react';
import React from 'react';
import Deck from './components/Deck';
import './style.scss';

// var m = new MessageEvent("message", { data: { page: "CharacterSelect", data: {"Id":"steam:110000103d1999b","Characters":[{"Name":{"FirstName":"Mickey","LastName":"de Graaff"}}]}}}); window.dispatchEvent(m);

class CharacterSelect extends Component
{

    deck = null;

    onSubmit ()
    {

        let character = this.deck.baraja.item.data('character');

        if (!character) {

            window.jQuery('#characterSelect .outer').fadeOut('slow', () => {
                window.jQuery.post('http://fivelife/fivelife.nui.character.create');
            });

            return;
        }

        window.jQuery('#characterSelect .outer').fadeOut('slow', () => {
            window.jQuery.post('http://fivelife/fivelife.nui.character.selected', JSON.stringify(character));
        });

    }

    render ()
    {
        return (
            <div id="characterSelect">
                <div className="outer">
                    <div className="middle">
                        <div className="container">
                            <Deck characters={this.props.data.Characters} ref={e => this.deck = e} />
                        </div>
                    </div>

                    <div className="controls">
                        <input type="button" value="Enter" onClick={this.onSubmit.bind(this)} />
                    </div>
                </div>
            </div>
        );
    }
}

export default CharacterSelect;