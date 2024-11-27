'use strict';

var selectedElms = [];
var permissionsIds = [];
var rolePermissions = [];

overrideIds({
    addButtonId: 'rolesAddButtonId',
    tableId: 'rolesDataTableId',
    gridButtonsId: 'rolesDataTableActionsContainerId',
    formId: 'rolesFormHtmlId',
    formValuesId: 'rolesFormId',
    btnSaveId: 'rolesFormAddButtonId',
    btnSaveTextId: 'rolesFormAddButtonTextId',
});

overrideMessages({
    saveTitle: 'Agregar rol',
    editTitle: 'Modificar rol'
});

API.tableURL = '/Roles/GetRolesForDt?ciaCod={ciaCod}';
API.saveOneURL = '/Roles/SaveOrUpdate';
API.getOneURL = '/Roles/GetOneRole?roleId={roleId}';
API.deleteOne = '/Roles/DeleteOneRole?roleId={roleId}';

$(document).ready(function () {
    initAsyncSelect2(
        'companyFilter',
        '/Cias/GetToSelect2',
        'Filtrar por empresa',
        false,
        undefined,
        undefined,
        false
    );
    // const currentCiaCode = $('#companyFilter').val();
    const currentCiaCode = $('#codCia').val();
    const currentCiaName = $('#ciaName').val();
    setSelect2Data('#companyFilter', {id: currentCiaCode, text: currentCiaName});
    listenFilterChanges();

    overridePrepareButtons();
    overrideInitDt();
});

function listenFilterChanges() {
    $('#companyFilter').on('change', function () {
        const newUrl = API.tableURL.replaceAll('{ciaCod}', $(this).val());
        reloadTableWithNewURL(newUrl);
    });
}

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
                    'url': API.tableURL.replaceAll('{ciaCod}', $('#companyFilter').val()),
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    {'data': 'Id'},
                    {'data': 'Name'},
                    {
                        'data': 'Active',
                        render: function (data, type, row) {
                            return data ? 'Activo' : 'Inactivo';
                        }
                    },
                    {
                        'data': 'CreatedAt',
                        render: function (data, type, row) {
                            return moment(data).format('DD/MM/YYYY HH:mm A');
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
                if (action === 'edit') { overrideShowForm(data.Id); }
                if (action === 'delete') { deleteRecord(data.Id); }
            });
        }
    );
}

function overrideShowForm(id) {
    showForm(id, function (isEditing) {
        overrideFormValidation();
        $('#CodCiaFromForm').val($('#companyFilter').val());

        if (isEditing) {
            $('#Id').val(id);
            $(`#${initValues.formAddButtonTextId}`).text('Editar');
            overrideLoadOne(id);
        } else {
            loadPermissions();
        }
    });
}

function overrideFormValidation() {
    initFormValidation({
        rules: {
            CodCia: { required: false, minlength: 3, maxlength: 3 },
            Id: { required: false },
            idsPermissions: { required: false },
            Name: { required: true, minlength: 3, maxlength: 256 },
            Active: { required: false },
        },
        showErrorsCb: function (errorMap, errorList, validator) {
            validator.defaultShowErrors();
        },
        submitHandlerCb: function (form, event) {
            selectedElms = $('#permissionsJsTree').jstree('get_selected', true);

            if (selectedElms.length > 0) {
                $.each(selectedElms, function() { permissionsIds.push(this.id); });
                var unds = $('.jstree-undetermined');
                $.each(unds,function(idx,obj){ permissionsIds.push($(obj).parent().parent().attr('id')); });
                $('#idsPermissions').val(permissionsIds.join(','));

                selectedElms = [];
                permissionsIds = [];

                doSave({});
            }
            else showToast(false, 'Seleccione al menos un permiso para el rol')
        }
    });
}

function overrideSave() {
    doSave({
        success: function (data) {
            if (data.success) savePermissionsForRole(roleId);
        }
    });
}

function savePermissionsForRole(roleId) {
    $.each(selectedElms, function() { permissionsIds.push(this.id); });
    var unds = $('.jstree-undetermined');
    $.each(unds,function(idx,obj){ permissionsIds.push($(obj).parent().parent().attr('id')); });
    $('#idsPermissions').val(permissionsIds.join(','));

    selectedElms = [];
    permissionsIds = [];
    const formData = $(`#${initValues.formID}`).serialize();

    POST({
        url: '/permission/save',
        data: formData
    });
}

function overrideLoadOne(id) {
    loadOne({
        url: API.getOneURL.replaceAll('{roleId}', id),
        success: function (data) {
            if(data.success){
                setFormData(data.data);
                loadPermissions(id);
            }
        }
    });
}

function setFormData(data) {
    $('#Name').val(data.Name);
    if (data.Active) { $('#Active').prop('checked', true) }
}

function deleteRecord(id){
    bootbox.confirm(
        '¿Esta seguro que desea eliminar este rol?', 
        function(result) {
            if(result){
                $.ajax({
                    url: API.deleteOne.replaceAll('{roleId}', id),
                    type:'DELETE',
                    success:function(data){
                        showToast(data.success, data.message);
                        if(data.success){
                            dtTable.ajax.reload();
                        }
                    }
                });
            }
        }
    );
}

function loadPermissions(id){
    if(id===undefined) id = 0;
    $(`#${initValues.formAddButtonId}`).prop('disabled', true);

    $.ajax({
        url: '/Permission/GetPermissions?roleId={roleId}'.replaceAll('{roleId}', id),
        type:'GET',
        success:function(data){
            if(data.success){
                $(`#${initValues.formAddButtonId}`).prop('disabled', false);

                $('#permissionsJsTree').jstree({
                    core: { data: data.data },
                    checkbox : { keep_selected_style : false },
                    plugins : ['wholerow', 'checkbox', 'noClose']
                }).bind('loaded.jstree', function (event, data) {
                    selectedElms = [];
                });
            }
            $('#lblLoading').hide();
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            $(`#${initValues.formAddButtonId}`).prop('disabled', false);
            $('#lblLoading').hide();
        }
    });
}