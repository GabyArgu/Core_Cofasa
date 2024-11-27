'use strict';

const CONTA_PARAM_TYPE = {
    DT1: 1,
    DT2: 2,
    DT3: 3,
    DT4: 4,
    DT5: 5,
    DT6: 6,

    CXC1: 7,
    CXC2: 8,
    CXC3: 9,
    CXC4: 10,
    CXC5: 11,
    CXC6: 12,

    CXP1: 13,
    CXP2: 14,
    CXP3: 15,
    CXP4: 16,
    CXP5: 17,
    CXP6: 18,

    EPF1: 19,
    EPF2: 20,
    EPF3: 21,
    EPF4: 22,
    EPF5: 23,
    EPF6: 24,

    IVA1: 25,
    IVA2: 26,
    IVA3: 27,
    IVA4: 28,
    IVA5: 29,
    IVA6: 30,

    IVAD1: 31,
    IVAD2: 32,
    IVAD3: 33,
    IVAD4: 34,
    IVAD5: 35,
    IVAD6: 36,
    
    // TODO: No estan en la tabla.
    RIVA1: 37,
    RIVA2: 38,
    RIVA3: 39,
    RIVA4: 40,
    RIVA5: 41,
    RIVA6: 42,

    CXC_TC1: 43,
    CXC_TC2: 44,
    CXC_TC3: 45,
    CXC_TC4: 46,
    CXC_TC5: 47,
    CXC_TC6: 48,

    DSV1: 49,
    DSV2: 50,
    DSV3: 51,
    DSV4: 52,
    DSV5: 53,
    DSV6: 54,

    CXPE1: 55,
    CXPE2: 56,
    CXPE3: 57,
    CXPE4: 58,
    CXPE5: 59,
    CXPE6: 60,

    AGE1: 61,
    AGE2: 62,
    AGE3: 63,
    AGE4: 64,
    AGE5: 65,
    AGE6: 66,

    DIF1: 66,
    DIF2: 67,
    DIF3: 68,
    DIF4: 69,
    DIF5: 70,
    DIF6: 71,

    DIA1: 71,
    DIA2: 72,
    DIA3: 73,
    DIA4: 74,
    DIA5: 75,
    DIA6: 76,

    OTR1: 77,
    OTR2: 78,
    OTR3: 79,
    OTR4: 80,
    OTR5: 81,
    OTR6: 82,
};

const listOfAccountsContaParams = [
    {ac1: 'DT1', ac2: 'DT2', ac3: 'DT3', ac4: 'DT4', ac5: 'DT5', ac6: 'DT6', name: 'NOM_DEPOSITOS'},
    {ac1: 'CXC1', ac2: 'CXC2', ac3: 'CXC3', ac4: 'CXC4', ac5: 'CXC5', ac6: 'CXC6', name: 'NOM_CXC'},
    {ac1: 'CXP1', ac2: 'CXP2', ac3: 'CXP3', ac4: 'CXP4', ac5: 'CXP5', ac6: 'CXP6', name: 'NOM_CXP'},
    {ac1: 'EPF1', ac2: 'EPF2', ac3: 'EPF3', ac4: 'EPF4', ac5: 'EPF5', ac6: 'EPF6', name: 'NOM_EPF'},
    {ac1: 'IVA1', ac2: 'IVA2', ac3: 'IVA3', ac4: 'IVA4', ac5: 'IVA5', ac6: 'IVA6', name: 'NOM_IVA'},
    {ac1: 'IVAD1', ac2: 'IVAD2', ac3: 'IVAD3', ac4: 'IVAD4', ac5: 'IVAD5', ac6: 'IVAD6', name: 'NOM_IVAD'},
    {ac1: 'RIVA1', ac2: 'RIVA2', ac3: 'RIVA3', ac4: 'RIVA4', ac5: 'RIVA5', ac6: 'RIVA6', name: 'NOM_RIVA'},
    {ac1: 'CXC_TC1', ac2: 'CXC_TC2', ac3: 'CXC_TC3', ac4: 'CXC_TC4', ac5: 'CXC_TC5', ac6: 'CXC_TC6', name: 'NOM_CXCTC'},
    {ac1: 'DSV1', ac2: 'DSV2', ac3: 'DSV3', ac4: 'DSV4', ac5: 'DSV5', ac6: 'DSV6', name: 'NOM_DEVSVTAS'},
    {ac1: 'CXPE1', ac2: 'CXPE2', ac3: 'CXPE3', ac4: 'CXPE4', ac5: 'CXPE5', ac6: 'CXPE6', name: 'NOM_CXPE'},
    {ac1: 'AGE1', ac2: 'AGE2', ac3: 'AGE3', ac4: 'AGE4', ac5: 'AGE5', ac6: 'AGE6', name: 'NOM_AGE'},
    {ac1: 'DIF1', ac2: 'DIF2', ac3: 'DIF3', ac4: 'DIF4', ac5: 'DIF5', ac6: 'DIF6', name: 'NOM_DIF'},
    {ac1: 'DIA1', ac2: 'DIA2', ac3: 'DIA3', ac4: 'DIA4', ac5: 'DIA5', ac6: 'DIA6', name: 'NOM_DIA'},
    {ac1: 'OTR1', ac2: 'OTR2', ac3: 'OTR3', ac4: 'OTR4', ac5: 'OTR5', ac6: 'OTR6', name: 'NOM_OTR'},
];

$(document).ready(function () {
    initContaParamFormValidation();
    loadCurrentCiaContaParams();
    watchListOfAccountNumbers(listOfAccountsContaParams);
});

function loadCurrentCiaContaParams() {
    $.ajax({
        url: '/Params/GetContaParams?ciaCod=' + $('#COD_CIA').val(),
        type:'GET',
        success:function(data){
            if(data.success){
                setContaParamsFormData(data.data);
                $('#btnUpdateContaParamsText').text('Actualizar');
                $('#isUpdating').val('1');
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function setContaParamsFormData(data) {
    $('#DT1').val(data.DT1);
    $('#DT2').val(data.DT2);
    $('#DT3').val(data.DT3);
    $('#DT4').val(data.DT4);
    $('#DT5').val(data.DT5);
    $('#DT6').val(data.DT6);
    if (needAccountName(data.DT1, data.DT2, data.DT3, data.DT4, data.DT5, data.DT6)) {
        makeAccountNameReq(
            '#DT1',
            '#DT2',
            '#DT3',
            '#DT4',
            '#DT5',
            '#DT6',
            '#NOM_DEPOSITOS'
        );
    }

    $('#CXC1').val(data.CXC1);
    $('#CXC2').val(data.CXC2);
    $('#CXC3').val(data.CXC3);
    $('#CXC4').val(data.CXC4);
    $('#CXC5').val(data.CXC5);
    $('#CXC6').val(data.CXC6);
    // NOM_CXC
    if (needAccountName(data.CXC1, data.CXC2, data.CXC3, data.CXC4, data.CXC5, data.CXC6)) {
        makeAccountNameReq(
            '#CXC1',
            '#CXC2',
            '#CXC3',
            '#CXC4',
            '#CXC5',
            '#CXC6',
            '#NOM_CXC'
        );
    }

    $('#CXP1').val(data.CXP1);
    $('#CXP2').val(data.CXP2);
    $('#CXP3').val(data.CXP3);
    $('#CXP4').val(data.CXP4);
    $('#CXP5').val(data.CXP5);
    $('#CXP6').val(data.CXP6);
    // NOM_CXP
    if (needAccountName(data.CXP1, data.CXP2, data.CXP3, data.CXP4, data.CXP5, data.CXP6)) {
        makeAccountNameReq(
            '#CXP1',
            '#CXP2',
            '#CXP3',
            '#CXP4',
            '#CXP5',
            '#CXP6',
            '#NOM_CXP'
        );
    }

    $('#EPF1').val(data.EPF1);
    $('#EPF2').val(data.EPF2);
    $('#EPF3').val(data.EPF3);
    $('#EPF4').val(data.EPF4);
    $('#EPF5').val(data.EPF5);
    $('#EPF6').val(data.EPF6);
    // NOM_EPF
    if (needAccountName(data.EPF1, data.EPF2, data.EPF3, data.EPF4, data.EPF5, data.EPF6)) {
        makeAccountNameReq(
            '#EPF1',
            '#EPF2',
            '#EPF3',
            '#EPF4',
            '#EPF5',
            '#EPF6',
            '#NOM_EPF'
        );
    }

    $('#IVA1').val(data.IVA1);
    $('#IVA2').val(data.IVA2);
    $('#IVA3').val(data.IVA3);
    $('#IVA4').val(data.IVA4);
    $('#IVA5').val(data.IVA5);
    $('#IVA6').val(data.IVA6);
    // NOM_IVA
    if (needAccountName(data.IVA1, data.IVA2, data.IVA3, data.IVA4, data.IVA5, data.IVA6)) {
        makeAccountNameReq(
            '#IVA1',
            '#IVA2',
            '#IVA3',
            '#IVA4',
            '#IVA5',
            '#IVA6',
            '#NOM_IVA'
        );
    }

    $('#IVAD1').val(data.IVAD1);
    $('#IVAD2').val(data.IVAD2);
    $('#IVAD3').val(data.IVAD3);
    $('#IVAD4').val(data.IVAD4);
    $('#IVAD5').val(data.IVAD5);
    $('#IVAD6').val(data.IVAD6);
    // NOM_IVAD
    if (needAccountName(data.IVAD1, data.IVAD2, data.IVAD3, data.IVAD4, data.IVAD5, data.IVAD6)) {
        makeAccountNameReq(
            '#IVAD1',
            '#IVAD2',
            '#IVAD3',
            '#IVAD4',
            '#IVAD5',
            '#IVAD6',
            '#NOM_IVAD'
        );
    }

    // TODO: NO SESTA.
    $('#RIVA1').val(data.RIVA1);
    $('#RIVA2').val(data.RIVA2);
    $('#RIVA3').val(data.RIVA3);
    $('#RIVA4').val(data.RIVA4);
    $('#RIVA5').val(data.RIVA5);
    $('#RIVA6').val(data.RIVA6);
    // NOM_RIVA
    if (needAccountName(data.RIVA1, data.RIVA2, data.RIVA3, data.RIVA4, data.RIVA5, data.RIVA6)) {
        makeAccountNameReq(
            '#RIVA1',
            '#RIVA2',
            '#RIVA3',
            '#RIVA4',
            '#RIVA5',
            '#RIVA6',
            '#NOM_RIVA'
        );
    }

    $('#CXC_TC1').val(data.CXC_TC1);
    $('#CXC_TC2').val(data.CXC_TC2);
    $('#CXC_TC3').val(data.CXC_TC3);
    $('#CXC_TC4').val(data.CXC_TC4);
    $('#CXC_TC5').val(data.CXC_TC5);
    $('#CXC_TC6').val(data.CXC_TC6);
    // NOM_CXCTC
    if (needAccountName(data.CXC_TC1, data.CXC_TC2, data.CXC_TC3, data.CXC_TC4, data.CXC_TC5, data.CXC_TC6)) {
        makeAccountNameReq(
            '#CXC_TC1',
            '#CXC_TC2',
            '#CXC_TC3',
            '#CXC_TC4',
            '#CXC_TC5',
            '#CXC_TC6',
            '#NOM_CXCTC'
        );
    }

    $('#DSV1').val(data.DSV1);
    $('#DSV2').val(data.DSV2);
    $('#DSV3').val(data.DSV3);
    $('#DSV4').val(data.DSV4);
    $('#DSV5').val(data.DSV5);
    $('#DSV6').val(data.DSV6);
    // NOM_DEVSVTAS
    if (needAccountName(data.DSV1, data.DSV2, data.DSV3, data.DSV4, data.DSV5, data.DSV6)) {
        makeAccountNameReq(
            '#DSV1',
            '#DSV2',
            '#DSV3',
            '#DSV4',
            '#DSV5',
            '#DSV6',
            '#NOM_DEVSVTAS'
        );
    }

    $('#CXPE1').val(data.CXPE1);
    $('#CXPE2').val(data.CXPE2);
    $('#CXPE3').val(data.CXPE3);
    $('#CXPE4').val(data.CXPE4);
    $('#CXPE5').val(data.CXPE5);
    $('#CXPE6').val(data.CXPE6);
    // NOM_CXPE
    if (needAccountName(data.CXPE1, data.CXPE2, data.CXPE3, data.CXPE4, data.CXPE5, data.CXPE6)) {
        makeAccountNameReq(
            '#CXPE1',
            '#CXPE2',
            '#CXPE3',
            '#CXPE4',
            '#CXPE5',
            '#CXPE6',
            '#NOM_CXPE'
        );
    }

    $('#AGE1').val(data.AGE1);
    $('#AGE2').val(data.AGE2);
    $('#AGE3').val(data.AGE3);
    $('#AGE4').val(data.AGE4);
    $('#AGE5').val(data.AGE5);
    $('#AGE6').val(data.AGE6);
    // NOM_AGE
    if (needAccountName(data.AGE1, data.AGE2, data.AGE3, data.AGE4, data.AGE5, data.AGE6)) {
        makeAccountNameReq(
            '#AGE1',
            '#AGE2',
            '#AGE3',
            '#AGE4',
            '#AGE5',
            '#AGE6',
            '#NOM_AGE'
        );
    }

    $('#DIF1').val(data.DIF1);
    $('#DIF2').val(data.DIF2);
    $('#DIF3').val(data.DIF3);
    $('#DIF4').val(data.DIF4);
    $('#DIF5').val(data.DIF5);
    $('#DIF6').val(data.DIF6);
    // NOM_DIF
    if (needAccountName(data.DIF1, data.DIF2, data.DIF3, data.DIF4, data.DIF5, data.DIF6)) {
        makeAccountNameReq(
            '#DIF1',
            '#DIF2',
            '#DIF3',
            '#DIF4',
            '#DIF5',
            '#DIF6',
            '#NOM_DIF'
        );
    }

    $('#DIA1').val(data.DIA1);
    $('#DIA2').val(data.DIA2);
    $('#DIA3').val(data.DIA3);
    $('#DIA4').val(data.DIA4);
    $('#DIA5').val(data.DIA5);
    $('#DIA6').val(data.DIA6);
    // NOM_DIA
    if (needAccountName(data.DIA1, data.DIA2, data.DIA3, data.DIA4, data.DIA5, data.DIA6)) {
        makeAccountNameReq(
            '#DIA1',
            '#DIA2',
            '#DIA3',
            '#DIA4',
            '#DIA5',
            '#DIA6',
            '#NOM_DIA'
        );
    }

    $('#OTR1').val(data.OTR1);
    $('#OTR2').val(data.OTR2);
    $('#OTR3').val(data.OTR3);
    $('#OTR4').val(data.OTR4);
    $('#OTR5').val(data.OTR5);
    $('#OTR6').val(data.OTR6);
    // NOM_OTR
    if (needAccountName(data.OTR1, data.OTR2, data.OTR3, data.OTR4, data.OTR5, data.OTR6)) {
        makeAccountNameReq(
            '#OTR1',
            '#OTR2',
            '#OTR3',
            '#OTR4',
            '#OTR5',
            '#OTR6',
            '#NOM_OTR'
        );
    }
}

function validateAccountNumber(type) {
    switch (type) {
        case CONTA_PARAM_TYPE.DT1:
            return validateNumbers('#DT2', '#DT3', '#DT4', '#DT5', '#DT6');
        case CONTA_PARAM_TYPE.DT2:
            return validateNumbers('#DT1', '#DT3', '#DT4', '#DT5', '#DT6');
        case CONTA_PARAM_TYPE.DT3:
            return validateNumbers('#DT1', '#DT2', '#DT4', '#DT5', '#DT6');
        case CONTA_PARAM_TYPE.DT4:
            return validateNumbers('#DT1', '#DT2', '#DT3', '#DT5', '#DT6');
        case CONTA_PARAM_TYPE.DT5:
            return validateNumbers('#DT1', '#DT2', '#DT3', '#DT4', '#DT6');
        case CONTA_PARAM_TYPE.DT6:
            return validateNumbers('#DT1', '#DT2', '#DT3', '#DT4', '#DT5');

        case CONTA_PARAM_TYPE.CXC1:
            return validateNumbers('#CXC2', '#CXC3', '#CXC4', '#CXC5', '#CXC6');
        case CONTA_PARAM_TYPE.CXC2:
            return validateNumbers('#CXC1', '#CXC3', '#CXC4', '#CXC5', '#CXC6');
        case CONTA_PARAM_TYPE.CXC3:
            return validateNumbers('#CXC1', '#CXC2', '#CXC4', '#CXC5', '#CXC6');
        case CONTA_PARAM_TYPE.CXC4:
            return validateNumbers('#CXC1', '#CXC2', '#CXC3', '#CXC5', '#CXC6');
        case CONTA_PARAM_TYPE.CXC5:
            return validateNumbers('#CXC1', '#CXC2', '#CXC3', '#CXC4', '#CXC6');
        case CONTA_PARAM_TYPE.CXC6:
            return validateNumbers('#CXC1', '#CXC2', '#CXC3', '#CXC4', '#CXC5');

        case CONTA_PARAM_TYPE.CXP1:
            return validateNumbers('#CXP2', '#CXP3', '#CXP4', '#CXP5', '#CXP6');
        case CONTA_PARAM_TYPE.CXP2:
            return validateNumbers('#CXP1', '#CXP3', '#CXP4', '#CXP5', '#CXP6');
        case CONTA_PARAM_TYPE.CXP3:
            return validateNumbers('#CXP1', '#CXP2', '#CXP4', '#CXP5', '#CXP6');
        case CONTA_PARAM_TYPE.CXP4:
            return validateNumbers('#CXP1', '#CXP2', '#CXP3', '#CXP5', '#CXP6');
        case CONTA_PARAM_TYPE.CXP5:
            return validateNumbers('#CXP1', '#CXP2', '#CXP3', '#CXP4', '#CXP6');
        case CONTA_PARAM_TYPE.CXP6:
            return validateNumbers('#CXP1', '#CXP2', '#CXP3', '#CXP4', '#CXP5');

        case CONTA_PARAM_TYPE.EPF1:
            return validateNumbers('#EPF2', '#EPF3', '#EPF4', '#EPF5', '#EPF6');
        case CONTA_PARAM_TYPE.EPF2:
            return validateNumbers('#EPF1', '#EPF3', '#EPF4', '#EPF5', '#EPF6');
        case CONTA_PARAM_TYPE.EPF3:
            return validateNumbers('#EPF1', '#EPF2', '#EPF4', '#EPF5', '#EPF6');
        case CONTA_PARAM_TYPE.EPF4:
            return validateNumbers('#EPF1', '#EPF2', '#EPF3', '#EPF5', '#EPF6');
        case CONTA_PARAM_TYPE.EPF5:
            return validateNumbers('#EPF1', '#EPF2', '#EPF3', '#EPF4', '#EPF6');
        case CONTA_PARAM_TYPE.EPF6:
            return validateNumbers('#EPF1', '#EPF2', '#EPF3', '#EPF4', '#EPF5');

        case CONTA_PARAM_TYPE.IVA1:
            return validateNumbers('#IVA2', '#IVA3', '#IVA4', '#IVA5', '#IVA6');
        case CONTA_PARAM_TYPE.IVA2:
            return validateNumbers('#IVA1', '#IVA3', '#IVA4', '#IVA5', '#IVA6');
        case CONTA_PARAM_TYPE.IVA3:
            return validateNumbers('#IVA1', '#IVA2', '#IVA4', '#IVA5', '#IVA6');
        case CONTA_PARAM_TYPE.IVA4:
            return validateNumbers('#IVA1', '#IVA2', '#IVA3', '#IVA5', '#IVA6');
        case CONTA_PARAM_TYPE.IVA5:
            return validateNumbers('#IVA1', '#IVA2', '#IVA3', '#IVA4', '#IVA6');
        case CONTA_PARAM_TYPE.IVA6:
            return validateNumbers('#IVA1', '#IVA2', '#IVA3', '#IVA4', '#IVA5');

        case CONTA_PARAM_TYPE.IVAD1:
            return validateNumbers('#IVAD2', '#IVAD3', '#IVAD4', '#IVAD5', '#IVAD6');
        case CONTA_PARAM_TYPE.IVAD2:
            return validateNumbers('#IVAD1', '#IVAD3', '#IVAD4', '#IVAD5', '#IVAD6');
        case CONTA_PARAM_TYPE.IVAD3:
            return validateNumbers('#IVAD1', '#IVAD2', '#IVAD4', '#IVAD5', '#IVAD6');
        case CONTA_PARAM_TYPE.IVAD4:
            return validateNumbers('#IVAD1', '#IVAD2', '#IVAD3', '#IVAD5', '#IVAD6');
        case CONTA_PARAM_TYPE.IVAD5:
            return validateNumbers('#IVAD1', '#IVAD2', '#IVAD3', '#IVAD4', '#IVAD6');
        case CONTA_PARAM_TYPE.IVAD6:
            return validateNumbers('#IVAD1', '#IVAD2', '#IVAD3', '#IVAD4', '#IVAD5');

        // TODO: No estan en la tabla.
        case CONTA_PARAM_TYPE.RIVA1:
            return validateNumbers('#RIVA2', '#RIVA3', '#RIVA4', '#RIVA5', '#RIVA6');
        case CONTA_PARAM_TYPE.RIVA2:
            return validateNumbers('#RIVA1', '#RIVA3', '#RIVA4', '#RIVA5', '#RIVA6');
        case CONTA_PARAM_TYPE.RIVA3:
            return validateNumbers('#RIVA1', '#RIVA2', '#RIVA4', '#RIVA5', '#RIVA6');
        case CONTA_PARAM_TYPE.RIVA4:
            return validateNumbers('#RIVA1', '#RIVA2', '#RIVA3', '#RIVA5', '#RIVA6');
        case CONTA_PARAM_TYPE.RIVA5:
            return validateNumbers('#RIVA1', '#RIVA2', '#RIVA3', '#RIVA4', '#RIVA6');
        case CONTA_PARAM_TYPE.RIVA6:
            return validateNumbers('#RIVA1', '#RIVA2', '#RIVA3', '#RIVA4', '#RIVA5');

        case CONTA_PARAM_TYPE.CXC_TC1:
            return validateNumbers('#CXC_TC2', '#CXC_TC3', '#CXC_TC4', '#CXC_TC5', '#CXC_TC6');
        case CONTA_PARAM_TYPE.CXC_TC2:
            return validateNumbers('#CXC_TC1', '#CXC_TC3', '#CXC_TC4', '#CXC_TC5', '#CXC_TC6');
        case CONTA_PARAM_TYPE.CXC_TC3:
            return validateNumbers('#CXC_TC1', '#CXC_TC2', '#CXC_TC4', '#CXC_TC5', '#CXC_TC6');
        case CONTA_PARAM_TYPE.CXC_TC4:
            return validateNumbers('#CXC_TC1', '#CXC_TC2', '#CXC_TC3', '#CXC_TC5', '#CXC_TC6');
        case CONTA_PARAM_TYPE.CXC_TC5:
            return validateNumbers('#CXC_TC1', '#CXC_TC2', '#CXC_TC3', '#CXC_TC4', '#CXC_TC6');
        case CONTA_PARAM_TYPE.CXC_TC6:
            return validateNumbers('#CXC_TC1', '#CXC_TC2', '#CXC_TC3', '#CXC_TC4', '#CXC_TC5');

        case CONTA_PARAM_TYPE.DSV1:
            return validateNumbers('#DSV2', '#DSV3', '#DSV4', '#DSV5', '#DSV6');
        case CONTA_PARAM_TYPE.DSV2:
            return validateNumbers('#DSV1', '#DSV3', '#DSV4', '#DSV5', '#DSV6');
        case CONTA_PARAM_TYPE.DSV3:
            return validateNumbers('#DSV1', '#DSV2', '#DSV4', '#DSV5', '#DSV6');
        case CONTA_PARAM_TYPE.DSV4:
            return validateNumbers('#DSV1', '#DSV2', '#DSV3', '#DSV5', '#DSV6');
        case CONTA_PARAM_TYPE.DSV5:
            return validateNumbers('#DSV1', '#DSV2', '#DSV3', '#DSV4', '#DSV6');
        case CONTA_PARAM_TYPE.DSV6:
            return validateNumbers('#DSV1', '#DSV2', '#DSV3', '#DSV4', '#DSV5');

        case CONTA_PARAM_TYPE.CXPE1:
            return validateNumbers('#CXPE2', '#CXPE3', '#CXPE4', '#CXPE5', '#CXPE6');
        case CONTA_PARAM_TYPE.CXPE2:
            return validateNumbers('#CXPE1', '#CXPE3', '#CXPE4', '#CXPE5', '#CXPE6');
        case CONTA_PARAM_TYPE.CXPE3:
            return validateNumbers('#CXPE1', '#CXPE2', '#CXPE4', '#CXPE5', '#CXPE6');
        case CONTA_PARAM_TYPE.CXPE4:
            return validateNumbers('#CXPE1', '#CXPE2', '#CXPE3', '#CXPE5', '#CXPE6');
        case CONTA_PARAM_TYPE.CXPE5:
            return validateNumbers('#CXPE1', '#CXPE2', '#CXPE3', '#CXPE4', '#CXPE6');
        case CONTA_PARAM_TYPE.CXPE6:
            return validateNumbers('#CXPE1', '#CXPE2', '#CXPE3', '#CXPE4', '#CXPE5');

        case CONTA_PARAM_TYPE.AGE1:
            return validateNumbers('#AGE2', '#AGE3', '#AGE4', '#AGE5', '#AGE6');
        case CONTA_PARAM_TYPE.AGE2:
            return validateNumbers('#AGE1', '#AGE3', '#AGE4', '#AGE5', '#AGE6');
        case CONTA_PARAM_TYPE.AGE3:
            return validateNumbers('#AGE1', '#AGE2', '#AGE4', '#AGE5', '#AGE6');
        case CONTA_PARAM_TYPE.AGE4:
            return validateNumbers('#AGE1', '#AGE2', '#AGE3', '#AGE5', '#AGE6');
        case CONTA_PARAM_TYPE.AGE5:
            return validateNumbers('#AGE1', '#AGE2', '#AGE3', '#AGE4', '#AGE6');
        case CONTA_PARAM_TYPE.AGE6:
            return validateNumbers('#AGE1', '#AGE2', '#AGE3', '#AGE4', '#AGE5');

        case CONTA_PARAM_TYPE.DIF1:
            return validateNumbers('#DIF2', '#DIF3', '#DIF4', '#DIF5', '#DIF6');
        case CONTA_PARAM_TYPE.DIF2:
            return validateNumbers('#DIF1', '#DIF3', '#DIF4', '#DIF5', '#DIF6');
        case CONTA_PARAM_TYPE.DIF3:
            return validateNumbers('#DIF1', '#DIF2', '#DIF4', '#DIF5', '#DIF6');
        case CONTA_PARAM_TYPE.DIF4:
            return validateNumbers('#DIF1', '#DIF2', '#DIF3', '#DIF5', '#DIF6');
        case CONTA_PARAM_TYPE.DIF5:
            return validateNumbers('#DIF1', '#DIF2', '#DIF3', '#DIF4', '#DIF6');
        case CONTA_PARAM_TYPE.DIF6:
            return validateNumbers('#DIF1', '#DIF2', '#DIF3', '#DIF4', '#DIF5');

        case CONTA_PARAM_TYPE.DIA1:
            return validateNumbers('#DIA2', '#DIA3', '#DIA4', '#DIA5', '#DIA6');
        case CONTA_PARAM_TYPE.DIA2:
            return validateNumbers('#DIA1', '#DIA3', '#DIA4', '#DIA5', '#DIA6');
        case CONTA_PARAM_TYPE.DIA3:
            return validateNumbers('#DIA1', '#DIA2', '#DIA4', '#DIA5', '#DIA6');
        case CONTA_PARAM_TYPE.DIA4:
            return validateNumbers('#DIA1', '#DIA2', '#DIA3', '#DIA5', '#DIA6');
        case CONTA_PARAM_TYPE.DIA5:
            return validateNumbers('#DIA1', '#DIA2', '#DIA3', '#DIA4', '#DIA6');
        case CONTA_PARAM_TYPE.DIA6:
            return validateNumbers('#DIA1', '#DIA2', '#DIA3', '#DIA4', '#DIA5');

        case CONTA_PARAM_TYPE.OTR1:
            return validateNumbers('#OTR2', '#OTR3', '#OTR4', '#OTR5', '#OTR6');
        case CONTA_PARAM_TYPE.OTR2:
            return validateNumbers('#OTR1', '#OTR3', '#OTR4', '#OTR5', '#OTR6');
        case CONTA_PARAM_TYPE.OTR3:
            return validateNumbers('#OTR1', '#OTR2', '#OTR4', '#OTR5', '#OTR6');
        case CONTA_PARAM_TYPE.OTR4:
            return validateNumbers('#OTR1', '#OTR2', '#OTR3', '#OTR5', '#OTR6');
        case CONTA_PARAM_TYPE.OTR5:
            return validateNumbers('#OTR1', '#OTR2', '#OTR3', '#OTR4', '#OTR6');
        case CONTA_PARAM_TYPE.OTR6:
            return validateNumbers('#OTR1', '#OTR2', '#OTR3', '#OTR4', '#OTR5');
    }

    return false;
}

function initContaParamFormValidation() {
    $('#contaParamsForm').validate({
        rules: {
            COD_CIA: { required: false, minlength: 3, maxlength: 3 },
            DT1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DT1); } } },
            DT2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DT2); } } },
            DT3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DT3); } } },
            DT4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DT4); } } },
            DT5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DT5); } } },
            DT6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DT6); } } },
            NOM_DEPOSITOS: { required: false },

            CXC1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC1); } } },
            CXC2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC2); } } },
            CXC3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC3); } } },
            CXC4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC4); } } },
            CXC5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC5); } } },
            CXC6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC6); } } },

            CXP1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXP1); } } },
            CXP2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXP2); } } },
            CXP3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXP3); } } },
            CXP4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXP4); } } },
            CXP5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXP5); } } },
            CXP6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXP6); } } },

            EPF1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.EPF1); } } },
            EPF2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.EPF2); } } },
            EPF3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.EPF3); } } },
            EPF4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.EPF4); } } },
            EPF5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.EPF5); } } },
            EPF6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.EPF6); } } },

            IVA1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVA1); } } },
            IVA2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVA2); } } },
            IVA3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVA3); } } },
            IVA4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVA4); } } },
            IVA5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVA5); } } },
            IVA6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVA6); } } },

            IVAD1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVAD1); } } },
            IVAD2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVAD2); } } },
            IVAD3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVAD3); } } },
            IVAD4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVAD4); } } },
            IVAD5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVAD5); } } },
            IVAD6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.IVAD6); } } },

            // TODO: No estan en la tabla.
            RIVA1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.RIVA1); } } },
            RIVA2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.RIVA2); } } },
            RIVA3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.RIVA3); } } },
            RIVA4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.RIVA4); } } },
            RIVA5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.RIVA5); } } },
            RIVA6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.RIVA6); } } },

            CXC_TC1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC_TC1); } } },
            CXC_TC2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC_TC2); } } },
            CXC_TC3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC_TC3); } } },
            CXC_TC4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC_TC4); } } },
            CXC_TC5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC_TC5); } } },
            CXC_TC6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXC_TC6); } } },

            DSV1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DSV1); } } },
            DSV2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DSV2); } } },
            DSV3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DSV3); } } },
            DSV4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DSV4); } } },
            DSV5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DSV5); } } },
            DSV6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DSV6); } } },

            CXPE1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXPE1); } } },
            CXPE2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXPE2); } } },
            CXPE3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXPE3); } } },
            CXPE4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXPE4); } } },
            CXPE5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXPE5); } } },
            CXPE6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.CXPE6); } } },

            AGE1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.AGE1); } } },
            AGE2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.AGE2); } } },
            AGE3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.AGE3); } } },
            AGE4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.AGE4); } } },
            AGE5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.AGE5); } } },
            AGE6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.AGE6); } } },

            DIF1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIF1); } } },
            DIF2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIF2); } } },
            DIF3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIF3); } } },
            DIF4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIF4); } } },
            DIF5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIF5); } } },
            DIF6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIF6); } } },

            DIA1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIA1); } } },
            DIA2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIA2); } } },
            DIA3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIA3); } } },
            DIA4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIA4); } } },
            DIA5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIA5); } } },
            DIA6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.DIA6); } } },

            OTR1: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.OTR1); } } },
            OTR2: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.OTR2); } } },
            OTR3: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.OTR3); } } },
            OTR4: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.OTR4); } } },
            OTR5: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.OTR5); } } },
            OTR6: { digits: true, required: { depends: function() { return validateAccountNumber(CONTA_PARAM_TYPE.OTR6); } } },
        },
        showErrors: function(errorMap, errorList) {
            this.defaultShowErrors();

            hideAccountNumberErrorMessages('DT1','DT2','DT3','DT4','DT5','DT6');
            hideAccountNumberErrorMessages('CXC1','CXC2','CXC3','CXC4','CXC5','CXC6');
            hideAccountNumberErrorMessages('CXP1','CXP2','CXP3','CXP4','CXP5','CXP6');
            hideAccountNumberErrorMessages('EPF1','EPF2','EPF3','EPF4','EPF5','EPF6');
            hideAccountNumberErrorMessages('IVA1','IVA2','IVA3','IVA4','IVA5','IVA6');
            hideAccountNumberErrorMessages('IVAD1','IVAD2','IVAD3','IVAD4','IVAD5','IVAD6');
            hideAccountNumberErrorMessages('RIVA1','RIVA2','RIVA3','RIVA4','RIVA5','RIVA6');
            hideAccountNumberErrorMessages('CXC_TC1','CXC_TC2','CXC_TC3','CXC_TC4','CXC_TC5','CXC_TC6');
            hideAccountNumberErrorMessages('DSV1','DSV2','DSV3','DSV4','DSV5','DSV6');
            hideAccountNumberErrorMessages('CXPE1','CXPE2','CXPE3','CXPE4','CXPE5','CXPE6');
            hideAccountNumberErrorMessages('AGE1','AGE2','AGE3','AGE4','AGE5','AGE6');
            hideAccountNumberErrorMessages('DIF1','DIF2','DIF3','DIF4','DIF5','DIF6');
            hideAccountNumberErrorMessages('DIA1','DIA2','DIA3','DIA4','DIA5','DIA6');
            hideAccountNumberErrorMessages('OTR1','OTR2','OTR3','OTR4','OTR5','OTR6');
        },
        submitHandler: function (form, event) {
            isLoading('#btnUpdateContaParams', true);
            saveContaParams();
        }
    });
}

function saveContaParams(){
    const formData = $('#contaParamsForm').serialize();

    $.ajax({
        url: '/Params/SaveContaParams',
        type: 'POST',
        data: formData,
        success:function(data){
            showToast(data.success, data.message);
            isLoading('#btnUpdateContaParams', false);
            if(data.success) { loadCurrentCiaContaParams(); }
        },
        error: function (error) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading('#btnUpdateContaParams', false);
        }
    });
}