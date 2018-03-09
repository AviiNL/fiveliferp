import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';

ReactDOM.render(<App />, document.getElementById('root'));

//let m = new MessageEvent("message", { data: { page: "CharacterSelect", data: {"Id":"steam:110000103d1999b","Characters":[{"Name":{"FirstName":"Mickey","LastName":"de Graaff"}}]}}}); window.dispatchEvent(m);




// let m = new MessageEvent("message", { data: { page: "IDForm", data: {"SSN": {"ITwo": 22,"IThree": 659,"IFour": 2400},"Name": {"FirstName": "Mickey","LastName": "de Graaff"},"DateOfBirth": {"Day": 0,"Month": 0,"Year": 0},"Inventory": {},"Position": {"X": -1037.716,"Y": -2737.756,"Z": 20.16927,"IsNormalized": false,"IsZero": false},"Heading": 323.4483}}}); window.dispatchEvent(m);


// let m = new MessageEvent("message", { data: { page: "Game", data: {}}}); window.dispatchEvent(m);