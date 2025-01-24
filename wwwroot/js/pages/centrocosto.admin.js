'use strict';

let ccAccountsGridDialog;
let dialogCopyCC;

overrideIds({
    tableId: 'centroCostoDt',
    formId: 'centroCostoFormBody',
    formValuesId: 'centroCostoForm',
    addButtonId: 'centroCostoAddBtn',
    gridButtonsId: 'centroCostoGridButtons',
    btnSaveId: 'centroCostoFormBtnSave',
    btnSaveTextId: 'centroCostoFormBtnSaveText',
})

API.tableURL = '/CentroCosto/GetAll?ciaCod={codCia}';
API.saveOneURL = '/CentroCosto/SaveOrUpdate';
API.getOneURL = '/CentroCosto/GetOne?codCia={codCia}&codCentroCosto={codCentroCosto}';

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
                    'url': API.tableURL.replace('{codCia}', $('#codCia').val()),
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    {'data': 'COD_CIA'},
                    {'data': 'CENTRO_COSTO'},
                    {'data': 'DESCRIPCION'},
                    {
                        'data': 'ACEPTA_DATOS',
                        render: function (data, type, row) {
                            return data==='S' ? 'Sí' : 'No';
                        }
                    },
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
                if (action === 'edit') { overrideShowForm(data.COD_CIA, data.CENTRO_COSTO); }
                if (action === 'accounts') { showCCGrid(data.COD_CIA, data.CENTRO_COSTO, data.DESCRIPCION); }
                if (action === 'cc-do-copy') { showCopyForm(data.CENTRO_COSTO, data.DESCRIPCION); }
            });
        }
    );
}

function initCentroCostoInputMask() {
    $('#CENTRO_COSTO').mask('99-999-99');
}

function overrideShowForm(codCia, codCentroCosto) {
    showForm(codCia, function (isEditing) {
        initCentroCostoInputMask();
        
        overrideFormValidation();
        $('#COD_CIA').val($('#codCia').val());

        if (isEditing) {
            $('#isUpdating').val(codCia);
            $(`#${initValues.formAddButtonTextId}`).text('Editar');

            overrideLoadOne(codCia, codCentroCosto);
        }
    });
}

//Se usa en form de cuentas
let codigoCentroCosto;
function showCCGrid(codCia, codCentroCosto, description) {
    $('#codCC').val(codCentroCosto);
    codigoCentroCosto = codCentroCosto;

    ccAccountsGridDialog = bootbox.dialog({
        title: `Cuentas asociadas al centro de costo<br/><small class="text-capitalize text-secondary">${codCentroCosto} - ${description}</small>`,
        message: $(ccAccountsGridHtmlId).val(),
        onEscape: true,
        className: 'modalLarge',
    });

    ccAccountsPrepareButtons();
    ccAccountsInitGrid();
}

function overrideFormValidation() {
    initFormValidation({
        showErrorsCb: function (errorMap, errorList, validator) { validator.defaultShowErrors(); },
        submitHandlerCb: function (form, event) { doSave({}); }
    });
}

function overrideLoadOne(codCia, codCentroCosto) {
    const newURL = API.getOneURL
        .replaceAll('{codCia}', codCia)
        .replaceAll('{codCentroCosto}', codCentroCosto);

    loadOne({
        url: newURL,
        success: function (data) {
            if(data.success){ setDataToForm(data.data); }
        }
    });
}

function setDataToForm(data) {
    if (!isDefined(data)) return;

    if (isDefined(data.CENTRO_COSTO)) $('#CENTRO_COSTO').val(data.CENTRO_COSTO);
    if (isDefined(data.DESCRIPCION)) $('#DESCRIPCION').val(data.DESCRIPCION);
    if (isDefined(data.ACEPTA_DATOS) && data.ACEPTA_DATOS === 'S') { $('#ACEPTA_DATOS').prop('checked', true) }
}

function validateCentroCostoCopyForm() {
    $('#copyCcForm').validate({
        rules: {
            centroCosto: {required: true},
            centroCosto2: {required: true}
        },
        submitHandler: function (form, event) {
            if ($('#centroCosto').val() === $('#centroCosto2').val()) {
                showToast(false, 'No puedes copiar el centro de costos en sí mismo');
            } else {
                doCcCopy();
            }
        }
    });
}

function doCcCopy() {
    isLoading('#copyCcButton', true);
    const formData = $('#copyCcForm').serialize();

    POST({
        url: '/CentroCosto/DoCopy',
        data: formData,
        success: function (data) {
            isLoading('#copyCcButton', false);
            showToast(data.success, data.message);

            if(data.success) {
                hideForm(dialogCopyCC);
                reloadTable();
            }
        },
        error: function (err) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading('#copyCcButton', false);
        }
    });
}

function initCentroCostoSelects(centroCostoId, centroCostoName) {
    // initSelect2Local('centroCosto', {id: centroCostoId,text: centroCostoName}, 'Centro de costos');
    initSelect2Local('centroCosto', [], 'Centro de costos');
    $('#centroCosto').select2('data', {id: centroCostoId,text: `${centroCostoId} - ${centroCostoName}`}).change();
    readOnlySelect2('#centroCosto', true);

    initSelect2Paginated('centroCosto2', '/CentroCosto/GetToSelect2', 'Centro costo');
}

function showCopyForm(centroCostoId, centroCostoName) {
    dialogCopyCC = bootbox.dialog({
        title: 'Copiar centro de costos',
        message: $('#ccCopyHtml').val(),
        className: 'modalMedium',
        onEscape: true
    });

    initCentroCostoSelects(centroCostoId, centroCostoName);
    validateCentroCostoCopyForm();
}