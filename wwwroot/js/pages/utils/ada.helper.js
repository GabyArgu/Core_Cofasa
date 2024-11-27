(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
        typeof define === 'function' && define.amd ? define(factory) :
            global.ADA= factory();

} (this,function() {
    'use strict';
    
    var ADA = {}

    // var tinylib = (function(el) {
    //     var obj={}
    //     var myelement=el
    //
    //     obj.setColor=(color)=>{
    //         myelement.style.color=color;
    //     }
    //     return obj;
    // })

    var safeParseInt = function safeParseInt(value) {
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

    let dtTable;
    let gridButtons;
    let dialog;
    let initValues = {
        tableId: '', // DATA TABLE.
        formId: '', // TEXTAREA QUE CONTIENE EL HTML DEL FORMULARIO.
        formValuesId: '', // ID DEL FORMULARIO.
        addButtonId: '', // ID DEL BOTON AGREGAR.
        gridButtonsId: '', // ID DEL TEXTAREA QUE CONTIENE LOS BOTONES DE ACCION PARA EL GRID.
        btnSaveId: '',
        btnSaveTextId: '',
        formModalSize: '',
    }

    let API = {
        tableURL: '',
        getOneURL: '',
        saveOneURL: '',
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
        if (isDefined(initValues.tableId)) initValues.tableId = tableId;
        if (isDefined(initValues.formId)) initValues.formId = formId;
        if (isDefined(initValues.formValuesId)) initValues.formValuesId = formValuesId;
        if (isDefined(initValues.addButtonId)) initValues.addButtonId = addButtonId;
        if (isDefined(initValues.gridButtonsId)) initValues.gridButtonsId = gridButtonsId;
        if (isDefined(initValues.btnSaveId)) initValues.btnSaveId = btnSaveId;
        if (isDefined(initValues.btnSaveTextId)) initValues.btnSaveTextId = btnSaveTextId;
    }

    function prepareButtons(callback) {
        const bodyButtons = $(`#${initValues.gridButtonsId}`).val();
        const tags = $('<div/>');
        tags.append(bodyButtons);

        gridButtons = '<center>' + tags.html() + '<center>';
        if (isFunction(callback)) callback();
    }

    function bindButtons(callback) {
        $(`#${initValues.tableId} tbody tr td button`).unbind('click').on('click', function (event) {
            if (event.preventDefault) { event.preventDefault(); }
            if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

            const obj = JSON.parse(Base64.decode($(this).parent().attr('data-row')));
            const action = $(this).attr('data-action');

            if (isFunction(callback)) callback(action, obj);
        });
    }

    function initDt(completeInit, completeDraw) {
        dtTable = $(`#${initValues.tableId}`)
            .on('draw.dt', function () {
                drawRowNumbers(`#${initValues.tableId}`, dtTable);
                setTimeout(function () {
                    if (isFunction(completeDraw)) completeDraw();
                }, 500);
            });

        $(`#${initValues.tableId}`)
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
            message: $(`#${initValues.formId}`).val().replaceAll('ttextarea', 'textarea'),
            className: initValues.formModalSize,
            onEscape: true
        });

        if (isFunction(callback)) callback(isEditing);
    }

    function buildRulesForValidators(result) {
        let data = {};
        const inputsList = [];

        $(`#${initValues.formValuesId} input, #${initValues.formValuesId} select,
        #${initValues.formValuesId} textarea`).each(function() { inputsList.push($(this)) });

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

    function initFormValidation({showErrorsCb, submitHandlerCb}) {
        buildRulesForValidators(function (data) {
            $(`#${initValues.formValuesId}`).validate({
                rules: data,
                showErrors: function(errorMap, errorList) { if (isFunction(showErrorsCb)) showErrorsCb(errorMap, errorList, this) },
                submitHandler: function (form, event) { if (isFunction(submitHandlerCb)) submitHandlerCb(form, event); }
            });
        });
    }

    function hideForm() {
        if (typeof (dialog)!=='undefined' && dialog!==null) {
            dialog.modal('hide');
        }
    }

    function reloadTable() {
        if (typeof (dtTable)!=='undefined' && dtTable!==null) {
            dtTable.ajax.reload();
        }
    }

    function isFunction(val) {
        return (typeof val === 'function')
    }

    function isDefined(val) {
        return !(typeof val === 'undefined')
    }

    function doSave({success, error}) {
        isLoading(`#${initValues.addButtonId}`, true);
        const formData = $(`#${initValues.formValuesId}`).serialize();

        POST({
            url: API.saveOneURL,
            data: formData,
            success: function (data) {
                isLoading(`#${initValues.addButtonId}`, false);
                showToast(data.success, data.message);

                if(data.success) {
                    hideForm();
                    reloadTable();
                }

                if (isFunction(success)) success(data);
            },
            error: function (err) {
                showToast(false, 'Ocurrió un error al procesar la solicitud');
                isLoading(`#${initValues.addButtonId}`, false);

                if (isFunction(error)) error(err)
            }
        });
    }

    function loadOne({type, data, success, error}) {
        if (!isDefined(type) || type === 'GET') {
            GET({
                url: API.getOneURL,
                success: function (data) {
                    if (isFunction(success)) success(data);
                },
                error: function (err) {
                    console.log(err);
                    showToast(false, 'Ocurrió un error al obtener los detalles del tipo de documento');
                    if (isFunction(error)) error(err);
                }
            });
        } else if (isDefined(type) && type === 'POST') {
            POST({
                url: API.getOneURL,
                data: data,
                success: function (data) {
                    if (isFunction(success)) success(data);
                },
                error: function (err) {
                    console.log(err);
                    showToast(false, 'Ocurrió un error al obtener los detalles del tipo de documento');
                    if (isFunction(error)) error(err);
                }
            });
        }
    }
    
    return ADA;
}))

// (function (global, factory) {
//     typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
//         typeof define === 'function' && define.amd ? define(factory) :
//             global.ADA = factory()
// }(this, (function () {
//    
// })));