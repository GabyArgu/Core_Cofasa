'use strict';

overrideIds({
    tableId: 'currenciesDt',
    formId: 'currencyFormBody',
    formValuesId: 'currencyForm',
    addButtonId: 'addCurrencyBtn',
    gridButtonsId: 'currenciesGridButtons',
})

API.saveOneURL = '/Currency/SaveOrUpdate';

$(document).ready(function () {
    overridePrepareButtons();
    overrideInitDt();
});

function overridePrepareButtons() {
    prepareButtons(function () {
        $(`#${initValues.addButtonId}`).click(function(){ overrideShowForm(); });
    });
}

function overrideInitDt() {
    initDt(
        function (table) {
            dtTable = table.DataTable({
                'ajax': {
                    'url': `/Currency/GetToDt`,
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    {'data': 'MON_CODIGO'},
                    {'data': 'MON_CODIGO'},
                    {'data': 'MON_NOMBRE'},
                    {'data': 'MON_SIGLAS'},
                    {'data': 'MON_SIMBOLO'},
                    {
                        sortable: false, searchable: false,
                        render: function (data, type, row) {
                            return gridButtons.replace('{data}',Base64.encode($.toJSON(row)));
                        }
                    }
                ]
            });
        },
        function () {
            bindButtons(function (action, data) {
                if (action === 'edit') { overrideShowForm(data.MON_CODIGO); }
            });
        }
    );
}

function overrideShowForm(id) {
    showForm(id, function (isEditing) {
        currencyFormValidation();

        if (isEditing) {
            $('#isUpdating').val(id);
            $('#btnSaveCurrencyText').text('Editar');
            overrideLoadOne(id);
        }
    });
}

function currencyFormValidation() {
    $(`#${initValues.formID}`).validate({
        rules: {
            isUpdating: {required: false},
            MON_CODIGO: {required: true, minlength: 2, maxlength: 4},
            MON_NOMBRE: {required: true, minlength: 2, maxlength: 50},
            MON_SIGLAS: {required: true, minlength: 1, maxlength: 4},
            MON_SIMBOLO: {required: true, minlength: 1, maxlength: 4},
        },
        submitHandler: function (form, event) {
            doSave({});
        }
    });
}

function overrideLoadOne(MON_CODIGO) {
    API.getOneURL = `/Currency/GetOneCurrency?codCurrency=${MON_CODIGO}`;

    loadOne({
        success: function (data) {
            if(data.success){ setDataToDmgDoctoForm(data.data); }
        }
    });
}

function setDataToDmgDoctoForm(data) {
    if (!isDefined(data)) return;

    if (isDefined(data.MON_CODIGO)) $('#MON_CODIGO').val(data.MON_CODIGO);
    if (isDefined(data.MON_NOMBRE)) $('#MON_NOMBRE').val(data.MON_NOMBRE);
    if (isDefined(data.MON_SIGLAS)) $('#MON_SIGLAS').val(data.MON_SIGLAS);
    if (isDefined(data.MON_SIMBOLO)) $('#MON_SIMBOLO').val(data.MON_SIMBOLO);
}