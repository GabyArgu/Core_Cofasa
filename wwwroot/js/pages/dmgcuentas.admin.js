'use strict';

overrideIds({
    addButtonId: 'addDmgCuentas',
    tableId: 'dmgCuentasDt',
    formId: 'dmgCuentasFormBody',
    gridButtonsId: 'dmgDoctosGridButtons',
    formValuesId: 'dmgCuentasForm',
    btnSaveIdn: 'btnSaveDmgCuentas',
    btnSaveTextId: 'btnSaveDmgCuentasText',
});

API.tableURL = '/DmgCuentas/GetAll?ciaCod={codCia}';
API.saveOneURL = '/DmgCuentas/SaveOrUpdate';
API.getOneURL = '/DmgCuentas/GetOne';

initValues.formModalSize = 'modalLarge';

$(document).ready(function () {
    overridePrepareButtons();
    overrideInitDt();
});

function overridePrepareButtons() {
    prepareButtons(function () {
        $(`#${initValues.addButtonId}`).click(function () { overrideShowForm(); });
    });
}

function overrideInitDt() {
    initDt(
        function (table) {
            dtTable = table.DataTable({
                'ajax': {
                    'url': API.tableURL.replace('{codCia}', $('#codCia').val()),
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    { 'data': 'COD_CIA' },
                    // {'data': 'COD_CIA'},
                    { 'data': 'CTA_1' },
                    { 'data': 'CTA_2' },
                    { 'data': 'CTA_3' },
                    { 'data': 'CTA_4' },
                    { 'data': 'CTA_5' },
                    { 'data': 'CTA_6' },
                    { 'data': 'DESCRIP_ESP' },
                    {
                        sortable: false, searchable: false,
                        render: function (data, type, row) {
                            return gridButtons.replace('{data}', Base64.encode($.toJSON(row)));
                        }
                    }
                ]
            });
        },
        function () {
            bindButtons(function (action, data) {
                if (action === 'edit') { overrideShowForm(data); }
            });
        }
    );
}

function overrideShowForm(data) {
    showForm(data, function (isEditing) {
        overrideFormValidation();
        $('#COD_CIA').val($('#codCia').val());

        watchAccountNumbers({
            accountObj: { ac1: 'CTA_1', ac2: 'CTA_2', ac3: 'CTA_3', ac4: 'CTA_4', ac5: 'CTA_5', ac6: 'CTA_6', name: '' },
            valid: function (object) { startSetPrincipalAccountNumbers(object) },
            invalid: function (object) { resetPrincipalAccountNumbers(); }
        });

        if (isEditing) {
            $('#isUpdating').val(data.COD_CIA);
            $(`#${initValues.formAddButtonTextId}`).text('Editar');
            overrideLoadOne(data);
        }
    });
}

function resetPrincipalAccountNumbers() {
    $('#CTA_1P').val('');
    $('#CTA_2P').val('');
    $('#CTA_3P').val('');
    $('#CTA_4P').val('');
    $('#CTA_5P').val('');
    $('#CTA_6P').val('');
}

function finishSetPrincipalAccountNumbers(ac1, ac2, ac3, ac4, ac5, ac6) {
    $('#CTA_1P').val(ac1);
    $('#CTA_2P').val(ac2);
    $('#CTA_3P').val(ac3);
    $('#CTA_4P').val(ac4);
    $('#CTA_5P').val(ac5);
    $('#CTA_6P').val(ac6);
}

function startSetPrincipalAccountNumbers(data) {
    resetPrincipalAccountNumbers();

    if ($(`#${data.ac1}`).val() !== '0' && $(`#${data.ac2}`).val() !== '0' && $(`#${data.ac3}`).val() !== '0'
        && $(`#${data.ac4}`).val() !== '0' && $(`#${data.ac5}`).val() !== '0' && $(`#${data.ac6}`).val() !== '0') {
        finishSetPrincipalAccountNumbers(
            $(`#${data.ac1}`).val(),
            $(`#${data.ac2}`).val(),
            $(`#${data.ac3}`).val(),
            $(`#${data.ac4}`).val(),
            $(`#${data.ac5}`).val(),
            '0'
        );
    }

    if ($(`#${data.ac1}`).val() !== '0' && $(`#${data.ac2}`).val() !== '0' && $(`#${data.ac3}`).val() !== '0'
        && $(`#${data.ac4}`).val() !== '0' && $(`#${data.ac5}`).val() !== '0' && $(`#${data.ac6}`).val() === '0') {
        finishSetPrincipalAccountNumbers(
            $(`#${data.ac1}`).val(),
            $(`#${data.ac2}`).val(),
            $(`#${data.ac3}`).val(),
            $(`#${data.ac4}`).val(),
            '0',
            '0'
        );
    }

    if ($(`#${data.ac1}`).val() !== '0' && $(`#${data.ac2}`).val() !== '0' && $(`#${data.ac3}`).val() !== '0'
        && $(`#${data.ac4}`).val() !== '0' && $(`#${data.ac5}`).val() === '0' && $(`#${data.ac6}`).val() === '0') {
        finishSetPrincipalAccountNumbers(
            $(`#${data.ac1}`).val(),
            $(`#${data.ac2}`).val(),
            $(`#${data.ac3}`).val(),
            '0',
            '0',
            '0'
        );
    }

    if ($(`#${data.ac1}`).val() !== '0' && $(`#${data.ac2}`).val() !== '0' && $(`#${data.ac3}`).val() !== '0'
        && $(`#${data.ac4}`).val() === '0' && $(`#${data.ac5}`).val() === '0' && $(`#${data.ac6}`).val() === '0') {
        finishSetPrincipalAccountNumbers(
            $(`#${data.ac1}`).val(),
            $(`#${data.ac2}`).val(),
            '0',
            '0',
            '0',
            '0'
        );
    }

    if ($(`#${data.ac1}`).val() !== '0' && $(`#${data.ac2}`).val() !== '0' && $(`#${data.ac3}`).val() === '0'
        && $(`#${data.ac4}`).val() === '0' && $(`#${data.ac5}`).val() === '0' && $(`#${data.ac6}`).val() === '0') {
        finishSetPrincipalAccountNumbers(
            $(`#${data.ac1}`).val(),
            '0',
            '0',
            '0',
            '0',
            '0'
        );
    }

    if (($(`#${data.ac1}`).val() !== '0' || $(`#${data.ac1}`).val() === '0') && $(`#${data.ac2}`).val() === '0' && $(`#${data.ac3}`).val() === '0'
        && $(`#${data.ac4}`).val() === '0' && $(`#${data.ac5}`).val() === '0' && $(`#${data.ac6}`).val() === '0') {
        finishSetPrincipalAccountNumbers(
            '0',
            '0',
            '0',
            '0',
            '0',
            '0'
        );
    }
}

function overrideFormValidation() {
    // $(`#${initValues.formValuesId}`).validate({
    //     rules: {
    //         isUpdating: {required: false, minlength: 1, maxlength: 2},
    //         COD_CIA: {required: true, minlength: 3, maxlength: 3},
    //
    //         DESCRIP_ESP: {required: true, minlength: 2, maxlength: 70},
    //         DESCRIP_ING: {required: false, minlength: 2, maxlength: 70},
    //
    //         CLASE_SALDO: {required: false, minlength: 1, maxlength: 2},
    //         GRUPO_CTA: {required: false, minlength: 1, maxlength: 2},
    //         Sub_Grupo: {required: false, minlength: 1, maxlength: 2},
    //         BANDERA: {required: false, minlength: 1, maxlength: 2},
    //
    //         CTA_1: {required: true, digits: true},
    //         CTA_2: {required: true, digits: true},
    //         CTA_3: {required: true, digits: true},
    //         CTA_4: {required: true, digits: true},
    //         CTA_5: {required: true, digits: true},
    //         CTA_6: {required: true, digits: true},
    //
    //         CTA_1P: {required: true, digits: true},
    //         CTA_2P: {required: true, digits: true},
    //         CTA_3P: {required: true, digits: true},
    //         CTA_4P: {required: true, digits: true},
    //         CTA_5P: {required: true, digits: true},
    //         CTA_6P: {required: true, digits: true},
    //
    //         ACEP_MOV: {required: false, maxlength: 1},
    //         ACEP_PRESUP: {required: false, maxlength: 1},
    //         ACEP_PRESUP_COMPRAS: {required: false, maxlength: 1},
    //         CATALOGO: {required: false, maxlength: 1},
    //
    //         UTIL_CTA: {required: false, maxlength: 250},
    //     },
    //     showErrors: function(errorMap, errorList) {
    //         this.defaultShowErrors();
    //         hideAccountNumberErrorMessages('CTA_1','CTA_2','CTA_3','CTA_4','CTA_5','CTA_6');
    //     },
    //     submitHandler: function (form, event) {
    //         doSave({});
    //     }
    // });

    initFormValidation({
        showErrorsCb: function (errorMap, errorList, validator) {
            validator.defaultShowErrors();
            hideAccountNumberErrorMessages('CTA_1', 'CTA_2', 'CTA_3', 'CTA_4', 'CTA_5', 'CTA_6');
            hideAccountNumberErrorMessages('CTA_1P', 'CTA_2P', 'CTA_3P', 'CTA_4P', 'CTA_5P', 'CTA_6P');
        },
        submitHandlerCb: function (form, event) {
            doSave({});
        }
    });
}

function overrideLoadOne(data) {
    const formData = `CodCia=${data.COD_CIA}&Cta1=${data.CTA_1}&Cta2=${data.CTA_2}&Cta3=${data.CTA_3}&Cta4=${data.CTA_4}&Cta5=${data.CTA_5}&Cta6=${data.CTA_6}`;

    loadOne({
        type: 'POST',
        data: formData,
        success: function (data) {
            if (data.success) { setFormData(data.data); }
        }
    });
}

function setFormData(data) {
    $('#COD_CIA').val(data.COD_CIA);

    $('#DESCRIP_ESP').val(data.DESCRIP_ESP);
    $('#DESCRIP_ING').val(data.DESCRIP_ING);

    $('#CLASE_SALDO').val(data.CLASE_SALDO);
    $('#GRUPO_CTA').val(data.GRUPO_CTA);
    $('#Grupo_Nivel').val(data.Grupo_Nivel);
    $('#Sub_Grupo').val(data.Sub_Grupo);

    $('#CTA_1').val(data.CTA_1);
    $('#CTA_2').val(data.CTA_2);
    $('#CTA_3').val(data.CTA_3);
    $('#CTA_4').val(data.CTA_4);
    $('#CTA_5').val(data.CTA_5);
    $('#CTA_6').val(data.CTA_6);

    $('#CTA_1P').val(data.CTA_1P);
    $('#CTA_2P').val(data.CTA_2P);
    $('#CTA_3P').val(data.CTA_3P);
    $('#CTA_4P').val(data.CTA_4P);
    $('#CTA_5P').val(data.CTA_5P);
    $('#CTA_6P').val(data.CTA_6P);

    if (data.ACEP_MOV === 'S') { $('#ACEP_MOV').prop('checked', true) }
    if (data.ACEP_PRESUP === 'S') { $('#ACEP_PRESUP').prop('checked', true) }
    if (data.ACEP_PRESUP_COMPRAS === 'S') { $('#ACEP_PRESUP_COMPRAS').prop('checked', true) }
    if (data.Catalogo === 'S') { $('#CATALOGO').prop('checked', true) }

    $('#UTIL_CTA').val(data.UTIL_CTA);
}