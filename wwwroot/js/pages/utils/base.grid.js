'use strict';

$.holdReady(true);
$.getScript('/js/utils/type.utils.js', function() { $.holdReady(false); });

let dtTable;
let gridButtons;
let dialog;
let initValues = {
    addButtonId: '', // ID DEL BOTON AGREGAR.
    dataTableId: '', // DATA TABLE.
    dataTableActionsContainerId: '', // ID DEL TEXTAREA QUE CONTIENE LOS BOTONES DE ACCION PARA EL GRID.
    formHtmlId: '', // TEXTAREA QUE CONTIENE EL HTML DEL FORMULARIO.
    formID: '', // ID DEL FORMULARIO.
    formAddButtonId: '',
    formAddButtonTextId: '',
    // Puede quedar vacio por defecto.
    // Otros valores:
    // - modalSmall -> 400px
    // - modalMedium -> 700px
    // - modalBig -> 900px
    // - modalLarge -> 1000px
    formModalSize: '',
}

let API = {
    tableURL: '',
    getOneURL: '',
    saveOneURL: '',
    deleteOne: '',
}

let messages = {
    save: {
        success: 'Registro guardado correctamente',
        error: 'Ocurrió un error al guardar el registro',
    },
    update: {
        success: 'Registro actualizado correctamente',
        error: 'Ocurrió un error al actualizar el registro',
    },
    getOne: {
        success: '',
        error: '',
    },
    form: {
        saveTitle: 'Agregar registro',
        editTitle: 'Editar registro'
    }
}

function overrideMessages({saveSuccess, saveError, updateSuccess, updateError, getOneSuccess, getOneError, saveTitle, editTitle}) {
    messages.save.success = saveSuccess;
    messages.save.error = saveError;
    messages.update.success = updateSuccess;
    messages.update.error = updateError;
    messages.getOne.success = getOneSuccess;
    messages.getOne.error = getOneError;
    messages.form.saveTitle = saveTitle;
    messages.form.editTitle = editTitle;
}

function overrideIds({addButtonId, tableId, formId, gridButtonsId, formValuesId, btnSaveId, btnSaveTextId}) {
    if (isDefined(initValues.dataTableId)) initValues.dataTableId = tableId;
    if (isDefined(initValues.formHtmlId)) initValues.formHtmlId = formId;
    if (isDefined(initValues.formID)) initValues.formID = formValuesId;
    if (isDefined(initValues.addButtonId)) initValues.addButtonId = addButtonId;
    if (isDefined(initValues.dataTableActionsContainerId)) initValues.dataTableActionsContainerId = gridButtonsId;
    if (isDefined(initValues.formAddButtonId)) initValues.formAddButtonId = btnSaveId;
    if (isDefined(initValues.formAddButtonTextId)) initValues.formAddButtonTextId = btnSaveTextId;
}

function prepareButtons(callback) {
    const bodyButtons = $(`#${initValues.dataTableActionsContainerId}`).val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    gridButtons = '<center>' + tags.html() + '<center>';
    if (isFunction(callback)) callback();
}

function bindButtons(callback) {
    $(`#${initValues.dataTableId} tbody tr td button`).unbind('click').on('click', function (event) {
        if (event.preventDefault) { event.preventDefault(); }
        if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

        const obj = JSON.parse(Base64.decode($(this).parent().attr('data-row')));
        const action = $(this).attr('data-action');

        if (isFunction(callback)) callback(action, obj);
    });
}

function initDt(completeInit, completeDraw) {
    const DT_ID = `#${initValues.dataTableId}`;

    dtTable = $(DT_ID)
        .on('draw.dt', function () {
            drawRowNumbers(`#${initValues.dataTableId}`, dtTable);
            setTimeout(function () {
                if (isFunction(completeDraw)) completeDraw();
            }, 500);
        });

    $(DT_ID)
        .removeClass('display')
        .addClass('table table-bordered table-hover dataTable');

    if (isFunction(completeInit)) completeInit(dtTable);
}

function GET({url, success, error}) {
    $.ajax({
        url: url,
        type: 'GET',
        success:function(data){
            if (isFunction(success)) success(data);
        },
        error: function (err) {
            console.log(error);
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            if (isFunction(error)) error(err);
        }
    });
}

function POST({url, data, success, error}) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success:function(data){ if (isFunction(success)) success(data); },
        error: function (err) { if (isFunction(error)) error(err); }
    });
}

function DELETE({url, success, error}) {
    $.ajax({
        url: url,
        type: 'DELETE',
        success:function(data){ if (isFunction(success)) success(data); },
        error: function (err) { if (isFunction(error)) error(err); }
    });
}

function showForm(id, callback) {
    const isEditing = !(typeof (id) === 'undefined' || id === '' || id===null);
    var title = '';

    if (isEditing) {
        title = messages.form.editTitle;
    } else {
        title = messages.form.saveTitle;
    }

    dialog = bootbox.dialog({
        title: title,
        message: $(`#${initValues.formHtmlId}`).val().replaceAll('ttextarea', 'textarea'),
        className: initValues.formModalSize,
        onEscape: true
    });

    if (isFunction(callback)) callback(isEditing);
}

function buildRulesForValidators(result) {
    let data = {};
    const inputsList = [];

    $(`#${initValues.formID} input, #${initValues.formID} select,
    #${initValues.formID} textarea`).each(function() { inputsList.push($(this)) });

    if (inputsList.length > 0) {
        $.each(inputsList, function (index, value) {
            // data[value.attr('name')] = {
            //     required: safeParseBool(value.attr('jv-required')),
            //     digits: safeParseBool(value.attr('jv-digits')),
            //     minlength: safeParseInt(value.attr('jv-minlength')),
            //     maxlength: safeParseInt(value.attr('jv-maxlength')),
            // }
            data[value.attr('name')] = {};
            if (isDefined(value.attr('jv-required'))) data[value.attr('name')]['required'] = safeParseBool(value.attr('jv-required'));
            if (isDefined(value.attr('jv-digits'))) data[value.attr('name')]['digits'] = safeParseBool(value.attr('jv-digits'));
            if (isDefined(value.attr('jv-minlength'))) data[value.attr('name')]['minlength'] = safeParseInt(value.attr('jv-minlength'));
            if (isDefined(value.attr('jv-maxlength'))) data[value.attr('name')]['maxlength'] = safeParseInt(value.attr('jv-maxlength'));
        });
    }

    result(data);
}

function initFormValidation({rules, showErrorsCb, submitHandlerCb}) {
    if (isDefined(rules)) {
        $(`#${initValues.formID}`).validate({
            ignore: [],
            rules: rules,
            showErrors: function(errorMap, errorList) { if (isFunction(showErrorsCb)) showErrorsCb(errorMap, errorList, this) },
            submitHandler: function (form, event) { if (isFunction(submitHandlerCb)) submitHandlerCb(form, event); }
        });
    } else {
        buildRulesForValidators(function (data) {
            $(`#${initValues.formID}`).validate({
                ignore: [],
                rules: data,
                showErrors: function(errorMap, errorList) { if (isFunction(showErrorsCb)) showErrorsCb(errorMap, errorList, this) },
                submitHandler: function (form, event) { if (isFunction(submitHandlerCb)) submitHandlerCb(form, event); }
            });
        });
    }
}

function hideForm(dialogObj) {
    if (typeof (dialogObj)!=='undefined' && dialogObj!==null) {
        dialogObj.modal('hide');
    } else if (typeof (dialog)!=='undefined' && dialog!==null) {
        dialog.modal('hide');
    }
}

function reloadTable() {
    if (typeof (dtTable)!=='undefined' && dtTable!==null) {
        dtTable.ajax.reload();
    }
}

function reloadTableWithNewURL(url) {
    if (typeof (dtTable)!=='undefined' && dtTable!==null) {
        dtTable.ajax.url(url).load();
    }
}

function isFunction(val) {
    return (typeof val === 'function')
}

function isDefined(val) {
    return !(typeof val === 'undefined')
}

function isUndefinedNullOrEmpty(val) {
    return typeof val==='undefined' || val === null || val === 'null' || val === '';
}

function serializeFormWithDisabled(formId) {
    const $form = $(formId);
    const disabled = $form.find(':input:disabled').removeAttr('disabled');
    const serialized = $form.serialize();
    disabled.attr('disabled', 'disabled');
    return serialized;
}

function doSave({success, error}) {
    const formData = $(`#${initValues.formID}`).serialize();
    completeDoSave({success, error, formData});
}

function doSaveWithDisabledFormData({success, error}) {
    const formData = serializeFormWithDisabled(`#${initValues.formID}`);
    completeDoSave({success, error, formData});
}

function completeDoSave({success, error, formData}) {
    isLoading(`#${initValues.btnSaveId}`, true);

    POST({
        url: API.saveOneURL,
        data: formData,
        success: function (data) {
            isLoading(`#${initValues.btnSaveId}`, false);
            showToast(data.success, data.message);

            if(data.success) {
                hideForm();
                reloadTable();
            }

            if (isFunction(success)) success(data);
        },
        error: function (err) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading(`#${initValues.btnSaveId}`, false);

            if (isFunction(error)) error(err)
        }
    });
}

function loadOne({url, type, data, success, error}) {
    const finalURL = isDefined(url) ? url : API.getOneURL;

    if (!isDefined(type) || type === 'GET') {
        GET({
            url: finalURL,
            success: function (data) {
                if (isFunction(success)) success(data);
            },
            error: function (err) {
                console.log(err);
                showToast(false, 'Ocurrió un error al procesar la solicitud');
                if (isFunction(error)) error(err);
            }
        });
    } else if (isDefined(type) && type === 'POST') {
        POST({
            url: finalURL,
            data: data,
            success: function (data) {
                if (isFunction(success)) success(data);
            },
            error: function (err) {
                console.log(err);
                showToast(false, 'Ocurrió un error al procesar la solicitud');
                if (isFunction(error)) error(err);
            }
        });
    }
}

function showConfirm({message, result}) {
    bootbox.confirm(message, function(res) { result(res) });
}

function deleteOne({message, url, success, error}) {
    showConfirm({
        message: message,
        result: function(result) {
            if(!result) return;
            const finalURL = isDefined(url) ? url : API.deleteOne;

            DELETE({
                url: finalURL,
                success: function (data) {
                    if (isFunction(success)) success(data);
                },
                error: function (err) {
                    console.log(err);
                    showToast(false, 'Ocurrió un error al procesar la solicitud');
                    if (isFunction(error)) error(err);
                }
            });
        }
    });
}

/**
 *
 * Some money functions.
 */

function formatMoney(data) {
    return `${CONSTANTS.defaults.currency.symbol} ${formatNumberToCurrency(data)}`;
}

function getYearFromDate(strDate) {
    return moment(strDate, CONSTANTS.defaults.date.formats.date).format(CONSTANTS.defaults.date.formats.year);
}

function getMonthFromDate(strDate) {
    return moment(strDate, CONSTANTS.defaults.date.formats.date).format(CONSTANTS.defaults.date.formats.month);
}

function disableSelect2(selectId) {
    $(selectId).select2('disable');
    // if(!isDefined(setEnabled) || !setEnabled) $(selectId).select2('disable');
    // else if(isDefined(setEnabled) && setEnabled) $(selectId).select2('enable');
}

function readOnlySelect2(selectId, setReadOnly) {
    $(selectId).select2('readonly', setReadOnly);
}

// Data example: {'id': '1', 'text': 'text'}
function setSelect2Data(selectId, data) {
    // $(selectId).select2({ data: data });
    // $('#selTIPO_DOCTO').select2('data', data.selTIPO_DOCTO).change();
    // $('#selSTAT_POLIZA').select2({ data: CONSTANTS.defaults.select.estadoPoliza });
    $(selectId).select2('data', data);
}

function disableInput(inputId, setDisabled) {
    $(inputId).prop('disabled', setDisabled);
}

function readOnlyInput(inputId, setReadOnly) {
    $(inputId).prop('readonly', setReadOnly);
}

function hideElement(elementId) {
    $(elementId).hide();
}

function focusOnInput(inputId) {
    $(inputId).focus();
}

function clearForm(formId) {
    $(formId).trigger('reset');
}

function setDataToSingleSelect2(selectId, data) {
    // $(selectId).select2({ data: data, placeholder: placeholder });
    $(selectId).select2('data', data).change();
}

/**
 *
 * @param separator Por ejemplo: ','.
 * @param str Eje: '1234, 5678, 91011'
 * @returns {*[]} Lista con los números de cuenta. Eje: ['1234', '5678', '91011'].
 */
function getAccountNumbersFromString(separator, str) {
    return str.split(separator).map(function (item) { return item.trim(); });
}

function listStringToAccountObject(accounts, callback) {
    const obj = {};
    if (!isDefined(accounts) || accounts.length !== 8) return null;

    $.each(accounts, function (index, value) {
        if (index >= 2) {
            if (index===2) obj.CTA_1 = value;
            if (index===3) obj.CTA_2 = value;
            if (index===4) obj.CTA_3 = value;
            if (index===5) obj.CTA_4 = value;
            if (index===6) obj.CTA_5 = value;
            if (index===7) {
                obj.CTA_6 = value;
                if (isFunction(callback)) callback(obj);
            }
        }
    });
}

function decodeDataFromContaAccountVal(accounts, callback) {
    const obj = {};
    if (!isDefined(accounts) || accounts.length !== 8) return null;

    $.each(accounts, function (index, value) {
        if(index===0) { obj.COD_CIA = value; }
        if(index===1) { obj.CENTRO_COSTO = value; }
        if (index >= 2) {
            if (index===2) obj.CTA_1 = value;
            if (index===3) obj.CTA_2 = value;
            if (index===4) obj.CTA_3 = value;
            if (index===5) obj.CTA_4 = value;
            if (index===6) obj.CTA_5 = value;
            if (index===7) {
                obj.CTA_6 = value;
                if (isFunction(callback)) callback(obj);
            }
        }
    });
}

function getMaxValueFromJqDtColumn(dt, column) {
    return dt.column(column).data().max();
}

function isValueInObjectList(value, list) {
    return list.some(function (item) { return item.id === value });
}

function initCalendar(id, format) {
    $(id).datetimepicker({
        format: isDefined(format) ? format : CONSTANTS.defaults.date.formats.date,
        locale: 'es',
        allowInputToggle: true
    });
}

function initTipoDoctoSelect(id, placeholder) {
    // initSelect2Paginated(id, '/DmgDoctos/GetToSelect2', 'Tipo de documento');
    initSelect2Paginated(id, '/DmgDoctos/GetToSelect2', placeholder);
}

function setSelect2Focus(id) {
    $(id).select2('open');
}