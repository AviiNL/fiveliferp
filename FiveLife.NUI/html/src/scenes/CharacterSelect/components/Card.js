import React, {Component} from 'react';
import './Card.scss';
import {inspect} from 'util';

class Card extends Component
{
    render ()
    {
        return (
            <li className="card" data-character={JSON.stringify(this.props.character)}>
                <i>{this.props.character.FirstName[0]}{this.props.character.LastName[0]}</i>
                <h1>{this.props.character.LastName}, {this.props.character.FirstName}</h1>
                <p>Cash: ${this.props.character.Cash}</p>
                <p>Phone: 215-572-4427</p>
                <p>Location: N/A</p>
            </li>
        );
    }
}

export default Card;