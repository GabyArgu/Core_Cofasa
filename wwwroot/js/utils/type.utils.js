'use strict';

$.holdReady(true);
$.getScript('/plugins/adamwdraper-Numeral-js/min/numeral.min.js', function() { $.holdReady(false); });

function safeParseInt(value) {
    return parseInt(value);
}

function safeParseDouble(value) {
    return value;
}

function safeParseBool(value) {
    if(value==='true' || value==='1') {
        return true;
    } else if(value==='false' || value==='0') {
        return false
    } else {
        throw new Error(`Not valid bool value: ${value}`);
    }
}

function formatNumberToCurrency(number) {
    return numeral(number).format('0,0.00');
}