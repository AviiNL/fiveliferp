import React, {Component} from 'react';
import ReactDOMServer from 'react-dom/server';

import Card from './Card';
import './Deck.scss';

class Deck extends Component
{

    componentDidMount ()
    {
        this.$el   = window.jQuery(this.el);
        this.baraja = this.$el.baraja();

        this.baraja.fanSettings = {
            // speed for opening/closing
            speed:       250,
            // easing for opening/closing
            easing:      'ease-out',
            // difference/range of possible angles that the items will have
            // example: with range:90 and center:false the first item
            // will have 0deg and the last one 90deg;
            // if center:true, then the first one will have 45deg
            // and the last one -45deg; in both cases the difference is 90deg
            range:       70,
            // this defines the position of the first item
            // (to the right, to the left)
            // and its angle (clockwise / counterclockwise)
            direction:   'right',
            // transform origin:
            // you can also pass a minX and maxX, meaning the left value
            // will vary between minX and maxX
            origin:      {x: 15, y: 95},
            // additional translation of each item
            translation: 25,
            // if the cards should be centered after the transform
            // is applied
            center:      true,
            // add a random factor to the final transform
            scatter:     true
        };

        let elements = '';

        for (let character of Object.values(this.props.characters)) {
            elements += ReactDOMServer.renderToStaticMarkup(<Card character={character} />);
        }

        this.baraja.add(window.jQuery(elements), () => {
            this.baraja.fan();
        });
        
    }

    render ()
    {
        return (
            <div>
                <ul ref={el => this.el = el} className="deck">
                    <li id="new" className="card">
                        <h4>Create new character</h4>
                    </li>
                </ul>
            </div>
        );
    }
}

export default Deck;