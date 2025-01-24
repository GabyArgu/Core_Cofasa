'use strict';

var ccAccountsGridButtons = '';
var ccAccountsDataTable;
var ccAccountsDialog = null;

const ccAccountsGridHtmlId = '#ccAccountsGridHtml';

const ccAccountsAddButton = '#ccAccountsAddButton';
const ccAccountsDataTableID = '#ccAccountsDataTable';
const ccAccountsDataTableActionsContainerID = '#ccAccountsDataTableActionsContainer';

const ccAccountsFormHtmlID = '#ccAccountsFormHtml';
const ccAccountsFormID = '#ccAccountsForm';
const ccAccountsFormAddButtonId = '#ccAccountsFormAddButton';
const ccAccountsFormAddButtonTextId = '#ccAccountsFormAddButtonText';

const CC_API = {
    TABLE: `/CentroCuenta/GetAll?codCia={codCia}&codCC={codCC}`,
    SAVE: '/CentroCuenta/SaveOrUpdate',
    DETAIL: '/CentroCuenta/GetOne?codCia={codCia}&codCC={codCC}&cta1={cta1}&cta2={cta2}&cta3={cta3}&cta4={cta4}&cta5={cta5}&cta6={cta6}',
};

const listOfAccountsForCc = [
    {ac1: 'CTA_1', ac2: 'CTA_2', ac3: 'CTA_3', ac4: 'CTA_4', ac5: 'CTA_5', ac6: 'CTA_6', name: 'NOMBRE'},
];

function ccAccountsPrepareButtons() {
    const bodyButtons = $(ccAccountsDataTableActionsContainerID).val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    $(ccAccountsAddButton).click(function(){ ccAccountsShowForm(); });
    ccAccountsGridButtons = '<center>' + tags.html() + '<center>';
}

function ccAccountsBindButtons() {
    $(`${ccAccountsDataTableID} tbody tr td button`).unbind('click').on('click', function (event) {
        if (event.preventDefault) { event.preventDefault(); }
        if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

        const obj = JSON.parse(Base64.decode($(this).parent().attr('data-row')));
        const action = $(this).attr('data-action');

        if (action === 'edit') { ccAccountsShowForm(obj); }
    });
}

function ccAccountsInitGrid() {
    ccAccountsDataTable = $(ccAccountsDataTableID)
        .on('draw.dt', function () {
            drawRowNumbers(ccAccountsDataTableID, ccAccountsDataTable);
            setTimeout(function(){ccAccountsBindButtons();},500);
        })
        .DataTable({
            'ajax': {
                'url': CC_API.TABLE
                    .replaceAll('{codCia}', $('#codCia').val())
                    .replaceAll('{codCC}', $('#codCC').val()),
                'type': 'GET',
                'datatype': 'JSON',
            },
            'columns': [
                { 'data': 'COD_CIA' },
                // { 'data': 'CENTRO_COSTO' },
                { 'data': 'CTA_1' },
                { 'data': 'CTA_2' },
                { 'data': 'CTA_3' },
                { 'data': 'CTA_4' },
                { 'data': 'CTA_5' },
                { 'data': 'CTA_6' },
                {
                    'data': 'ESTADO',
                    render: function (data, type, row) {
                        return data==='A'? 'Activo' : 'Inactivo';
                    }
                },
                { 'data': 'Descripcion_CTA' },
                {
                    sortable: false, searchable: false,
                    render: function (data, type, row) {
                        return ccAccountsGridButtons.replace('{data-child}',Base64.encode($.toJSON(row)));
                    }
                }
            ]
        });

    $(ccAccountsDataTableID)
        .removeClass('display')
        .addClass('table table-bordered table-hover dataTable');
}

function ccAccountsStartValidation() {
    $(ccAccountsFormID).validate({ // number: true
        rules: {
            COD_CIA: { required: true, minlength: 3, maxlength: 3 },
            CENTRO_COSTO: { required: true, minlength: 9, maxlength: 9 },
            CTA_1: { required: true, digits: true },
            CTA_2: { required: true, digits: true },
            CTA_3: { required: true, digits: true },
            CTA_4: { required: true, digits: true },
            CTA_5: { required: true, digits: true },
            CTA_6: { required: true, digits: true },
            NOMBRE: { required: true },
            ESTADO: { required: false, minlength: 1, maxlength: 1 },
        },
        showErrors: function(errorMap, errorList) {
            this.defaultShowErrors();
            hideFieldsErrorsMessages(['CTA_1','CTA_2','CTA_3','CTA_4','CTA_5','CTA_6','NOMBRE']);
        },
        submitHandler: function (form, event) {
            ccAccountsSave();
        }
    });
}

//Código de Select2
function initSelects() {
    initSelect2Paginated(
        'CENTRO_CUENTA',
        '/CentroCuenta/GetToSelect2',
        'Cuenta contable',
        false,
        function (term, page) {
            return {
                codCia: $('#COD_CIA').val(),
                centroCosto: "00-000-00",
                q: term,
                page: page || 1,
                pageSize: 10
            };
        });

    //Al cambiar centro de cuenta llena los numero de cuentas
    $('#CENTRO_CUENTA').on('change', function () {
        console.log($('#CENTRO_CUENTA').select2("data").text.split(" ")[0]);

        if ($(this).val() !== '') {
            const dataList = getAccountNumbersFromString('|', $(this).val());
            listStringToAccountObject(dataList, function (dataObj) {
                $('#CTA_1').val(dataObj.CTA_1);
                $('#CTA_2').val(dataObj.CTA_2);
                $('#CTA_3').val(dataObj.CTA_3);
                $('#CTA_4').val(dataObj.CTA_4);
                $('#CTA_5').val(dataObj.CTA_5);
                $('#CTA_6').val(dataObj.CTA_6);

                makeAccountNameReq(
                    '#CTA_1',
                    '#CTA_2',
                    '#CTA_3',
                    '#CTA_4',
                    '#CTA_5',
                    '#CTA_6',
                    '#NOMBRE'
                );
            });
        } else {
            cleanAccountNumbers();
        }
    });
}

function ccAccountsShowForm(data) {
    const isEditing = !(typeof (data) === 'undefined' || data === null);
    var title = '';

    if (isEditing) {
        title = 'Editar cuenta de centro de costos';
    } else {
        title = 'Agregar cuenta al centro de costos';
    }

    ccAccountsDialog = bootbox.dialog({
        title: title,
        message: $(ccAccountsFormHtmlID).val(),
        onEscape: true,
        className: 'modalMedium',
    });

    ccAccountsStartValidation();
    watchListOfAccountNumbers(listOfAccountsForCc);

    if (isEditing) {
        $('#isUpdating').val(data.COD_CIA);
        $('#COD_CIA').val(data.COD_CIA);
        $(ccAccountsFormAddButtonTextId).text('Editar');
        ccAccountsLoadOne(data);
    } else {
        $('#COD_CIA').val($('#codCia').val());
    }

    //Iniciar y cargar selects
    initSelects();

    //Definir centro de costo a editar
    $('#CENTRO_COSTO').val(codigoCentroCosto);
}

function ccAccountsSave() {
    let formData = $(ccAccountsFormID).serializeArray();

    // Verificar el estado y asignar "A" o "I"
    const estadoField = formData.find(field => field.name === 'ESTADO');
    if (estadoField) {
        estadoField.value = $('#ESTADO').is(':checked') ? 'A' : 'I';
    } else {
        formData.push({ name: 'ESTADO', value: $('#ESTADO').is(':checked') ? 'A' : 'I' });
    }

    isLoading(ccAccountsFormAddButtonId, true);

    $.ajax({
        url: CC_API.SAVE,
        type: 'POST',
        data: $.param(formData), // Convertir de nuevo a formato de string
        success: function (data) {
            showToast(data.success, data.message);
            isLoading(ccAccountsFormAddButtonId, false);

            if (data.success) {
                ccAccountsDialog.modal('hide');
                ccAccountsDataTable.ajax.reload();
            }
        },
        error: function (error) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading(ccAccountsFormAddButtonId, false);
        }
    });
}


function ccAccountsLoadOne(data) {
    if (!isDefined(data)) return;

    GET({
        url: CC_API.DETAIL
            .replaceAll('{codCia}', data.COD_CIA)
            .replaceAll('{codCC}', data.CENTRO_COSTO)
            .replaceAll('{cta1}', data.CTA_1)
            .replaceAll('{cta2}', data.CTA_2)
            .replaceAll('{cta3}', data.CTA_3)
            .replaceAll('{cta4}', data.CTA_4)
            .replaceAll('{cta5}', data.CTA_5)
            .replaceAll('{cta6}', data.CTA_6),
        success: function (data) {
            if (data.success) {
                ccAccountsSetDataToForm(data.data);
            }
        },
        error: function (err) {
            console.log(err);
            showToast(false, 'Ocurrió un error al cargar el detalle');
        }
    });
}

function ccAccountsSetDataToForm(data) {
    if (!isDefined(data)) return;

    //Definir cuenta contable seleccionada
    let cuentaContableSelect = {
        formattedText: data.CuentaContable,
        id: data.CuentaContable,
        more: false,
        text: data.CuentaContable
    }
    setDataToSingleSelect2('#CENTRO_CUENTA', cuentaContableSelect, 'Cuenta contable');


    if (isDefined(data.CTA_1)) $('#CTA_1').val(data.CTA_1);
    if (isDefined(data.CTA_2)) $('#CTA_2').val(data.CTA_2);
    if (isDefined(data.CTA_3)) $('#CTA_3').val(data.CTA_3);
    if (isDefined(data.CTA_4)) $('#CTA_4').val(data.CTA_4);
    if (isDefined(data.CTA_5)) $('#CTA_5').val(data.CTA_5);
    if (isDefined(data.CTA_6)) $('#CTA_6').val(data.CTA_6);
    if (isDefined(data.Descripcion_CTA)) $('#NOMBRE').val(data.Descripcion_CTA);
    if (isDefined(data.ESTADO) && data.ESTADO === 'A') { $('#ESTADO').prop('checked', true) }
}