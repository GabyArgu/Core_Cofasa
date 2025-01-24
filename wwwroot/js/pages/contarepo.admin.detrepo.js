'use strict';

let detRepoGridButtons = '';
let detRepoDataTable;
let detRepoDialog = null;

const detRepoGridHtmlId = '#detRepoGridHtml';

const detRepoAddButtonId = '#detRepoAddButton';
const detRepoDataTableId = '#detRepoDataTable';
const detRepoDataTableActionsContainerId = '#detRepositoryDataTableActionsContainer';

const detRepoFormHtmlId = '#detRepoFormHtml';
const detRepoFormId = '#detRepoForm';
const detRepoFormAddButtonId = '#detRepoFormAddButton';
const detRepoFormAddButtonTextId = '#detRepoFormAddButtonText';

// Offline variables.
let DT_LOCAL_DATA = [];
let IS_ADDING = false;
let detRepoGridButtonsOffline = null;
let detToEdit;
let isConfirmDialogShown = false;

const detRepoAPI = {
    TABLE: '/DetRepository/GetForDt?codCia={codCia}&period={period}&doctoType={doctoType}&numPoliza={numPoliza}',
    SAVE: '/DetRepository/SaveOrUpdate',
    DETAIL: '',
};

const detRepoId = {
    codCia: null,
    period: null,
    doctoType: null,
    numPoliza: null,
};

function listenEscKey() {
    $(document).keyup(function(e) {
        if (detRepoDialog === null || isConfirmDialogShown) return;
        if (detRepoDialog.is(':visible') && e.keyCode === 27) {
            console.log('ESC key pressed');

            if (IS_ADDING) {
                isConfirmDialogShown = true;
                showConfirm({
                    message: '¿Desea salir sin guardar los cambios?',
                    result: function (result) {
                        isConfirmDialogShown = false;
                        if(result) { detRepoDialog.modal('hide'); }
                    }
                });
            } else {
                detRepoDialog.modal('hide');
            }
        }
    });
}

function detRepoPrepareButtons() {
    const bodyButtons = $(detRepoDataTableActionsContainerId).val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    detRepoGridButtons = '<center>' + tags.html() + '<center>';
}

function detRepoBindButtons() {
    if (!IS_ADDING) {
        $(`${detRepoDataTableId} tbody tr td button`).unbind('click').on('click', function (event) {
            if (event.preventDefault) { event.preventDefault(); }
            if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

            const obj = JSON.parse(Base64.decode($(this).parent().attr('data-row')));
            const action = $(this).attr('data-action');

            // if (action === 'editdetrepo') { detRepoShowForm(obj); }
            if (action === 'editdetrepo') { setDetRepoFormData(obj); }
            if (action === 'deletedetrepo') { makeDeleteDetRepoData(obj); }
        });
    }
}

function detRepoInitGrid() {
    if (!IS_ADDING) {
        detRepoDataTable = $(detRepoDataTableId)
            .on('draw.dt', function () {
                // drawRowNumbers(detRepoDataTableId, detRepoDataTable);
                setTimeout(function(){detRepoBindButtons();},500);
            })
            .DataTable({
                'autoWidth': false,
                'ajax': {
                    'url': detRepoAPI.TABLE.replaceAll('{codCia}', detRepoId.codCia)
                        .replaceAll('{period}', detRepoId.period)
                        .replaceAll('{doctoType}', detRepoId.doctoType)
                        .replaceAll('{numPoliza}', detRepoId.numPoliza),
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'rowCallback': function(row, data, index) {
                    $(row).find('td:eq(9)').addClass('break-word');
                    $(row).find('td:eq(10)').addClass('text-right');
                    $(row).find('td:eq(11)').addClass('text-right');
                },
                'footerCallback': function(tfoot, data, start, end, display) {
                    const api = this.api();

                    const totCargos = api.column(13).data().reduce(function (a, b) {
                        return parseDoubleOrReturnZero(a) + parseDoubleOrReturnZero(b);}, 0);
                    const totAbonos = api.column(14).data().reduce(function (a, b) {
                        return parseDoubleOrReturnZero(a) + parseDoubleOrReturnZero(b);
                    }, 0);

                    temp_PARTIDA_DIFF = totCargos - totAbonos;

                    $(api.column(13).footer()).html(formatMoney(totCargos));
                    $(api.column(14).footer()).html(formatMoney(totAbonos));
                },
                'columns': [
                    { sortable: false, searchable: false, 'data': 'CORRELAT' },
                    { sortable: false, searchable: false, visible: false, 'data': 'CENTRO_COSTO' },
                    { sortable: false, searchable: false, visible: false, 'data': 'CENTRO_CONTABLE_ID' },
                    { sortable: false, searchable: false, visible: false, 'data': 'CENTRO_CONTABLE_TEXT' },
                    // { 'data': 'NUM_POLIZA' },
                    { 'data': 'Desc_CCosto' }, // CENTRO_COSTO
                    { 'data': 'CTA_1' },
                    { 'data': 'CTA_2' },
                    { 'data': 'CTA_3' },
                    { 'data': 'CTA_4' },
                    { 'data': 'CTA_5' },
                    { 'data': 'CTA_6' },
                    { 'data': 'Desc_CContable' }, // NOMBRE
                    { 'data': 'CONCEPTO' },
                    {
                        'data': 'CARGO',
                        render: function (data, type, row) { return formatMoney(data); }
                    },
                    {
                        'data': 'ABONO',
                        render: function (data, type, row) { return formatMoney(data); }
                    },
                    {
                        sortable: false, searchable: false,
                        render: function (data, type, row) {
                            return detRepoGridButtons.replace('{data-child}',Base64.encode($.toJSON(row)));
                        }
                    }
                ]
            });
    } else {
        detRepoDataTable = $(detRepoDataTableId)
            .on('draw.dt', function () {
                drawRowNumbers(detRepoDataTableId, detRepoDataTable);
                // setTimeout(function(){detRepoBindButtons();},500);
                $('#repositoryFormAddAllButton').unbind('click').click(function () {
                    if (detRepoDataTable.data().any()) { saveAll(); }
                });
            })
            .DataTable({
                'autoWidth': false,
                'data': DT_LOCAL_DATA,
                'rowCallback': function(row, data, index) {
                    $(row).find('td:eq(9)').addClass('break-word');
                    $(row).find('td:eq(10)').addClass('text-right');
                    $(row).find('td:eq(11)').addClass('text-right');
                },
                'footerCallback': function(tfoot, data, start, end, display) {
                    const api = this.api();

                    const totCargos = api.column(13).data().reduce(function (a, b) {
                        return parseDoubleOrReturnZero(a) + parseDoubleOrReturnZero(b);}, 0);
                    const totAbonos = api.column(14).data().reduce(function (a, b) {
                        return parseDoubleOrReturnZero(a) + parseDoubleOrReturnZero(b);
                    }, 0);

                    if (totAbonos > 0 && totCargos > 0) {
                        temp_PARTIDA_DIFF = totCargos - totAbonos;
                    } else {
                        temp_PARTIDA_DIFF = null;
                    }

                    $(api.column(13).footer()).html(formatMoney(totCargos));
                    $(api.column(14).footer()).html(formatMoney(totAbonos));
                },
                'columns': [
                    { sortable: false, searchable: false, 'data': 'det_CORRELAT' },
                    { sortable: false, searchable: false, visible: false, 'data': 'CENTRO_COSTO' },
                    { sortable: false, searchable: false, visible: false, 'data': 'CENTRO_CONTABLE_ID' },
                    { sortable: false, searchable: false, visible: false, 'data': 'CENTRO_CONTABLE_TEXT' },
                    { 'data': 'Desc_CCosto' },
                    { 'data': 'CTA_1' },
                    { 'data': 'CTA_2' },
                    { 'data': 'CTA_3' },
                    { 'data': 'CTA_4' },
                    { 'data': 'CTA_5' },
                    { 'data': 'CTA_6' },
                    { 'data': 'Desc_CContable' },
                    { 'data': 'det_CONCEPTO' },
                    {
                        'data': 'CARGO',
                        render: function (data, type, row) { return formatMoney(data); }
                    },
                    {
                        'data': 'ABONO',
                        render: function (data, type, row) { return formatMoney(data); }
                    },
                    {
                        sortable: false, searchable: false,
                        render: function (data, type, row, meta) {
                            return detRepoGridButtonsOffline
                                .replace('{data-child}', Base64.encode($.toJSON(row)));
                        }
                    }
                ]
            });

        deleteDetRepoRow();
        modifyDetRepoRow();
    }

    $(detRepoDataTableId)
        .removeClass('display')
        .addClass('table table-bordered table-hover dataTable');
}

/**
 * 
 * Settea los datos desde el GRID hijo al formulario de agregar cuenta.
 * @param data Fila completa desde JQ DT.
 */
function setDetRepoFormData(data) {
    $('#detRepositoryAddAsientoButtonText').text('Editar cuenta');
    $('#detOPERACION').val('ACTUALIZAR');

    $('#det_CORRELAT').val(data.CORRELAT);
    $('#CTA_1').val(data.CTA_1);
    $('#CTA_2').val(data.CTA_2);
    $('#CTA_3').val(data.CTA_3);
    $('#CTA_4').val(data.CTA_4);
    $('#CTA_5').val(data.CTA_5);
    $('#CTA_6').val(data.CTA_6);
    $('#NOMBRE').val(data.Desc_CContable);
    $('#det_CONCEPTO').val(data.CONCEPTO);
    $('#CARGO').val(data.CARGO);
    $('#ABONO').val(data.ABONO);
    setDataToSingleSelect2('#CENTRO_COSTO', data.selCentroCosto, 'Centro costo');
    setDataToSingleSelect2('#CENTRO_CUENTA', data.selCentroCuenta, 'Cuenta contable');
    focusOnInput('#CTA_1');
}

function makeDeleteDetRepoData(data) {
    showConfirm({
        message: '¿Está seguro de eliminar el registro?',
        result: function (result) {
            if (!result) return;

            isLoading('#detRepositoryAddAsientoButton', true);
            const tipoDocto = $('#selTIPO_DOCTO').val();
            const centroCosto = data.selCentroCosto.id;
            const formData = `det_COD_CIA=${data.COD_CIA}&det_PERIODO=${data.PERIODO}&det_TIPO_DOCTO=${tipoDocto}&det_NUM_POLIZA=${data.NUM_POLIZA}&det_CORRELAT=${data.CORRELAT}&detOPERACION=ELIMINAR&CTA_1=${data.CTA_1}&CTA_2=${data.CTA_2}&CTA_3=${data.CTA_3}&CTA_4=${data.CTA_4}&CTA_5=${data.CTA_5}&CTA_6=${data.CTA_6}&det_CONCEPTO=${data.CONCEPTO}&CARGO=${data.CARGO}&ABONO=${data.ABONO}&CENTRO_COSTO=${centroCosto}`;

            detRepoSaveModifyOrDelete(formData);
        }
    });
}

function detRepoStartValidation() {
    $(detRepoFormId).validate({ // number: true
        rules: {
            det_COD_CIA: { required: false, minlength: 3, maxlength: 3 },
            det_PERIODO: { required: false, digits: true },
            det_TIPO_DOCTO: { required: false, maxlength: 2 },
            det_NUM_POLIZA: { required: false, digits: true },
            det_CORRELAT: { required: false, digits: true },
            CTA_1: { required: true, digits: true },
            CTA_2: { required: true, digits: true },
            CTA_3: { required: true, digits: true },
            CTA_4: { required: true, digits: true },
            CTA_5: { required: true, digits: true },
            CTA_6: { required: true, digits: true },
            NOMBRE: { required: true },
            det_CONCEPTO: { required: true, maxlength: 400 },
            CARGO: { required: true, number: true },
            ABONO: { required: true, number: true },
            CENTRO_COSTO: { required: true, maxlength: 45 },
            CENTRO_CUENTA: { required: true, maxlength: 45 },
            detOPERACION: { required: false },
        },
        showErrors: function(errorMap, errorList) {
            this.defaultShowErrors();
            hideFieldsErrorsMessages(['CENTRO_COSTO', 'CENTRO_CUENTA', 'CTA_1','CTA_2','CTA_3','CTA_4','CTA_5','CTA_6','NOMBRE']);
        },
        submitHandler: function (form, event) {
            const correlative = $('#det_CORRELAT').val();
            if (isUndefinedNullOrEmpty(correlative)) { // Adding online.
                const newCorrelative = detRepoDataTable.rows().count() + 1;
                $('#det_CORRELAT').val(newCorrelative);
            }

            if (IS_ADDING) {
                $('#det_PERIODO').val($('#PERIODO').val());
                $('#det_TIPO_DOCTO').val($('#TIPO_DOCTO').val());
                $('#det_NUM_POLIZA').val($('#NUM_POLIZA').val());
                addDetRepoRow();
            } else { // Call add det_repo API.
                isLoading('#detRepositoryAddAsientoButton', true);
                const formData = $(detRepoFormId).serialize();
                detRepoSaveModifyOrDelete(formData);
            }
        }
    });
}

function validateDetRepoFormSel2() {
    console.log($('#CENTRO_COSTO').val())
    if ($('#CENTRO_COSTO').val() === '') {
        readOnlySelect2('#CENTRO_CUENTA', true);
        setSelect2Data('#CENTRO_CUENTA', null);
        cleanAccountNumbers();
    } else {
        readOnlySelect2('#CENTRO_CUENTA', false);
    }
}

function cleanAccountNumbers() {
    $('#CTA_1').val('');
    $('#CTA_2').val('');
    $('#CTA_3').val('');
    $('#CTA_4').val('');
    $('#CTA_5').val('');
    $('#CTA_6').val('');
    $('#NOMBRE').val('');
}

function centroCuentaNumbersToInputs() {
    $('#CENTRO_CUENTA').on('change', function () {
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

function cleanDetRepoForm() {
    // clearForm(detRepoFormId);
    $('#CENTRO_COSTO').select2('val', '');
    $('#CENTRO_CUENTA').select2('val', '');
    cleanAccountNumbers();
    $('#detOPERACION').val('');
    $('#CARGO').val('');
    $('#ABONO').val('');
    $('#det_CONCEPTO').val('');
    $('#CARGO, #ABONO').removeAttr('readonly');
    $('#detRepositoryAddAsientoButtonText').text('Agregar cuenta');

    $('#det_CORRELAT').val('');
    $('#NOMBRE').val('');
}

function detRepoSaveModifyOrDelete(formData, callback, errorCallback) {
    $.ajax({
        url: detRepoAPI.SAVE,
        type:'POST',
        data: formData,
        success:function(data){
            if (isFunction(callback)) callback(data); // Cuando se esta agregando.
            else { // Cuando se esta modificando.
                showToast(data.success, data.message);
                isLoading('#detRepositoryAddAsientoButton', false);

                if(data.success){
                    detRepoDataTable.ajax.reload();
                    cleanDetRepoForm();
                    setSelect2Focus('#CENTRO_COSTO');
                }
            }
        },
        error: function (error) {
            if (isFunction(errorCallback)) {
                errorCallback(error);
            } else {
                showToast(false, 'Ocurrió un error al procesar la solicitud');
                isLoading('#detRepositoryAddAsientoButton', false);
                cleanDetRepoForm();
            }
        }
    });
}

/**
 * FUNCIONES CUANDO LA TABLA HIJA ES OFFLINE
 */
function destroyAndInit() {
    detRepoDataTable.destroy();
    detRepoInitGrid();
}

function prepareDetRepoButtonsOffline() {
    const bodyButtons = $('#detRepositoryDataTableActionsContainerOffline').val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    detRepoGridButtonsOffline = '<center>' + tags.html() + '</center>';
}

function getTextAfterSeparator(text, separator) {
    if(isUndefinedNullOrEmpty(text) || isUndefinedNullOrEmpty(separator)) return;
    const result = text.split(separator);
    if (result.length===1) return result[0].trim();
    else if (result.length===2) return result[1].trim();
}

function addDetRepoRow() {
    DT_LOCAL_DATA = detRepoDataTable.data().toArray();
    let result = {};
    // result['det_CORRELAT'] = detRepoDataTable.rows().count() + 1;
    result['CENTRO_COSTO'] = $('#CENTRO_COSTO').select2('data').id;
    result['CENTRO_CONTABLE_ID'] = $('#CENTRO_CUENTA').select2('data').id;
    result['CENTRO_CONTABLE_TEXT'] = $('#CENTRO_CUENTA').select2('data').text;
    result['Desc_CCosto'] = getTextAfterSeparator($('#CENTRO_COSTO').select2('data').text, ' - ');
    result['CTA_1'] = $('#CTA_1').val();
    result['CTA_2'] = $('#CTA_2').val();
    // result['CTA_3'] = $('#CTA_3').select2('data').text;
    result['CTA_3'] = $('#CTA_3').val();
    result['CTA_4'] = $('#CTA_4').val();
    result['CTA_5'] = $('#CTA_5').val();
    result['CTA_6'] = $('#CTA_6').val();
    result['Desc_CContable'] = $('#NOMBRE').val();
    result['det_CONCEPTO'] = $('#det_CONCEPTO').val();
    result['CARGO'] = $('#CARGO').val();
    result['ABONO'] = $('#ABONO').val();

    DT_LOCAL_DATA.push(result);
    detToEdit = null;
    cleanDetRepoForm();
    destroyAndInit();
    $('#detRepositoryAddAsientoButtonText').text('Agregar cuenta');
}

function modifyDetRepoRow() {
    $(`${detRepoDataTableId} tbody`).on('click', '.button-edit-det-off', function () {
        detToEdit = detRepoDataTable.row($(this).parents('tr')).data();
        const toRemove = detRepoDataTable.row($(this).parents('tr'));
        toRemove.remove();
        setModifyFormOffline(detToEdit);
    });
}

function setModifyFormOffline(data) {
    $('#detRepositoryAddAsientoButtonText').text('Editar cuenta');
    // $('#det_CORRELAT').val(data.det_CORRELAT);
    $('#CTA_1').val(data.CTA_1);
    $('#CTA_2').val(data.CTA_2);
    $('#CTA_3').val(data.CTA_3);
    $('#CTA_4').val(data.CTA_4);
    $('#CTA_5').val(data.CTA_5);
    $('#CTA_6').val(data.CTA_6);
    $('#NOMBRE').val(data.Desc_CContable);
    $('#det_CONCEPTO').val(data.det_CONCEPTO);
    $('#CARGO').val(data.CARGO);
    $('#ABONO').val(data.ABONO);
    setDataToSingleSelect2('#CENTRO_COSTO', {id: data.CENTRO_COSTO, text: data.Desc_CCosto});
    setDataToSingleSelect2('#CENTRO_CUENTA', {id: data.CENTRO_CONTABLE_ID, text: data.CENTRO_CONTABLE_TEXT});
}

function deleteDetRepoRow() {
    $(`${detRepoDataTableId} tbody`).unbind('click').on('click', '.button-delete-det-off', function () {
        const toRemove = detRepoDataTable.row($(this).parents('tr'));
        bootbox.confirm('¿Está seguro de eliminar el registro?', function (result) {
            if (result) {
                if (detToEdit!==null) {
                    cleanDetRepoForm();
                    detToEdit = null;
                }
                toRemove.remove();
                DT_LOCAL_DATA = detRepoDataTable.data().toArray();
                destroyAndInit();
                clearForm(detRepoFormId);
                $('#CENTRO_COSTO').select2('val', '');
                $('#CENTRO_CUENTA').select2('val', '');
            }
        });
    });
}

function saveAll() {
    const formHeader = formToJsonObject(`#${initValues.formID}`);
    DT_LOCAL_DATA = detRepoDataTable.data().toArray();
    const jsonData = { header: formHeader, detRepoList: DT_LOCAL_DATA };

    $.ajax({
        url: '/Repository/SaveList',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        success: function (data) {
            showToast(data.success, data.message);
            if (data.success) {
                setTempPartidaDiffValues(
                    data.data.codCia, data.data.periodo, data.data.tipoDocto, data.data.numPoliza);
                cleanDetRepoForm();
                detRepoDialog.modal('hide');
            }
        },
        error: function (error) { showToast(false, 'Ocurrió un error al procesar la solicitud'); }
    });
}

function formToJsonObject(formId) {
    const formArray = $(formId).serializeArray();
    const json = {};
    $.each(formArray, function () {
        json[this.name] = this.value;
    });
    return json;
}

function parseIntOrReturnZero(value) {
    return parseInt(value) || 0;
}

function parseDoubleOrReturnZero(value) {
    return parseFloat(value) || 0;
}