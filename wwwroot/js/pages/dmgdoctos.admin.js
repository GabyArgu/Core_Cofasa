'use strict';

var dmgDoctosGridSelect = '';
overrideIds({
    tableId: 'dmgDoctosDt',
    formId: 'dmgDoctosFormBody',
    formValuesId: 'dmgDoctosForm',
    addButtonId: 'addDmgDoctos',
    gridButtonsId: 'dmgDoctosGridButtons'
});
API.saveOneURL = '/DmgDoctos/SaveOrUpdate';

$(document).ready(function () {
    prepareDmgDoctosSelect();
    overridePrepareButtons();
    overrideInitDt();
});

function overridePrepareButtons() {
    prepareButtons(function () {
        $(`#${initValues.addButtonId}`).click(function(){ overrideShowForm(); });
    });
}

function prepareDmgDoctosSelect() {
    const bodyButtons = $('#dmgDoctosGridSelect').val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    dmgDoctosGridSelect = '<center>' + tags.html() + '<center>';
}

function bindDmgDoctosSelect() {
    $(`#${initValues.dataTableId} tbody tr td select`).unbind('change').on('change', function (event) {
        if (event.preventDefault) { event.preventDefault(); }
        if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

        const obj = JSON.parse(Base64.decode($(this).attr('data-row')));
        callDmgDoctosFromSelect(obj, $(this).val());
    });
}

function overrideInitDt() {
    initDt(
        function (table) {
            dtTable = table.DataTable({
                'ajax': {
                    'url': `/DmgDoctos/GetDmgDoctos?ciaCod=${$('#codCia').val()}`,
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    {'data': 'COD_CIA'},
                    {'data': 'COD_CIA'},
                    {'data': 'TIPO_DOCTO'},
                    {'data': 'CONTADOR_POLIZA'},
                    {'data': 'DESCRIP_TIPO'},
                    {'data': 'PROCESO'},
                    {
                        'data': 'POLIZA_MANUAL',
                        sortable: false, searchable: false,
                        render: function (data, type, row, meta) {
                            return dmgDoctosGridSelect
                                .replaceAll('{codCia}', `${row.COD_CIA}_${meta.row}`)
                                .replaceAll('{data}', Base64.encode($.toJSON(row)))
                                .replaceAll('{0}', row.POLIZA_MANUAL === '' || row.POLIZA_MANUAL === null ? '' : 'selected')
                                .replaceAll('{1}', row.POLIZA_MANUAL === 'M' ? 'selected' : '')
                                .replaceAll('{2}', row.POLIZA_MANUAL === 'A' ? 'selected' : '');
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
            bindDmgDoctosSelect();
            bindButtons(function (action, data) {
                if (action === 'edit') { overrideShowForm(data.TIPO_DOCTO); }
            });
        }
    );
}

/**
 * 
 * @param id TIPO_DOCTO.
 */
function overrideShowForm(id) {
    showForm(id, function (isEditing) {
        startDmgDoctosValidation();
        $('#COD_CIA').val($('#codCia').val());

        if (isEditing) {
            $('#isUpdating').val(id);
            $('#btnSaveDmgDoctosText').text('Editar');
            overrideLoadOne(id);
        }
    });
}

function callDmgDoctosFromSelect(data, newStatus) {
    const formData = `isUpdating=1&COD_CIA=${data.COD_CIA}&TIPO_DOCTO=${data.TIPO_DOCTO}
        &CONTADOR_POLIZA=${data.CONTADOR_POLIZA}&DESCRIP_TIPO=${data.DESCRIP_TIPO}&PROCESO=${data.PROCESO}
        &POLIZA_MANUAL=${newStatus}`;
    
    POST({
        url: '/DmgDoctos/SaveOrUpdate',
        data: formData,
        success: function (data) {
            showToast(data.success, data.message);
            if(data.success) reloadTable();
        },
        error: function (error) { console.log(error); }
    });
}

function startDmgDoctosValidation() {
    $(`#${initValues.formID}`).validate({
        rules: {
            isUpdating: {required: false, minlength: 1, maxlength: 2},
            COD_CIA: {required: true, minlength: 3, maxlength: 3},
            TIPO_DOCTO: {required: true, minlength: 2, maxlength: 2},
            CONTADOR_POLIZA: {required: true, digits: true},
            DESCRIP_TIPO: {required: true, minlength: 4, maxlength: 40},
            PROCESO: {required: true, minlength: 1, maxlength: 1},
            POLIZA_MANUAL: {required: true, minlength: 1, maxlength: 1},
        },
        submitHandler: function (form, event) {
            doSave({});
        }
    });
}

function overrideLoadOne(TIPO_DOCTO) {
    API.getOneURL = `/DmgDoctos/GetOneDmgDocto?ciaCod=${$('#codCia').val()}&doctoType=${TIPO_DOCTO}`;

    loadOne({
        success: function (data) {
            if(data.success){ setDataToDmgDoctoForm(data.data); }
        }
    });
}

function setDataToDmgDoctoForm(data) {
    $('#TIPO_DOCTO').val(data.TIPO_DOCTO);
    $('#CONTADOR_POLIZA').val(data.CONTADOR_POLIZA);
    $('#DESCRIP_TIPO').val(data.DESCRIP_TIPO);
    $('#PROCESO').val(data.PROCESO);
    $('#POLIZA_MANUAL').val(data.POLIZA_MANUAL);
}