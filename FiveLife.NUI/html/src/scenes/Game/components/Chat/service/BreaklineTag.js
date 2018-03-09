import React from 'react';
import parser, { Tag } from 'bbcode-to-react';

export default class BreaklineTag extends Tag {
    constructor(renderer, settings = {}) {
        super(renderer, settings);
        this.SELF_CLOSE = true;
        this.STRIP_OUTER = true;
    }

    toHTML() {
        return '<br />';
    }

    toReact() {
        return <br />;
    }
}

parser.registerTag('br', BreaklineTag);