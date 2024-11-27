'use strict';

overrideIds({
    addButtonId: 'usersAddButtonId',
    tableId: 'usersDataTableId',
    gridButtonsId: 'usersDataTableActionsContainerId',
    formId: 'usersFormHtmlId',
    formValuesId: 'usersFormId',
    btnSaveId: 'usersFormAddButtonId',
    btnSaveTextId: 'usersFormAddButtonTextId',
});

API.tableURL = '/Users/GetUsersForDt';
API.saveOneURL = '/Users/SaveOrUpdate';
API.getOneURL = '/Users/GetOneUser?userId={userId}';
API.deleteOne = '/Users/DeleteOne?userId={userId}';

let companyAccessDialog = null;

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
                    'url': API.tableURL,
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    {'data': 'Id'},
                    {'data': 'UserName'},
                    {'data': 'Email'},
                    {'data': 'FirstName'},
                    {'data': 'LastName'},
                    {
                        'data': 'Active',
                        render: function (data, type, row) {
                            // return data ? 'Activo' : 'Inactivo';
                            return data ? '<center><span class="badge badge-success">ACTIVO</span></center>'
                                : '<center><span class="badge badge-danger">INACTIVO</span></center>';
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
                            let buttons = gridButtons.replace('{data}',Base64.encode($.toJSON(row)));
                            buttons = (row.Id == $('#currentUserId').val())
                                ? buttons.replace('{canDelete}', 'disabled')
                                : buttons.replace('{canDelete}', '');
                            return buttons;
                        }
                    }
                ]
            });
        },
        function () {
            bindButtons(function (action, data) {
                if (action === 'edit') { overrideShowForm(data.Id); }
                // DON'T CHANGE TO !== IT ONLY WORKS WITH !=
                if (action === 'delete'&& data.Id != $('#currentUserId').val()) { overrideDeleteOne(data.Id); }
                if (action === 'company') { showCompanyAccessDialog(data); }
            });
        }
    );
}

function overrideShowForm(id) {
    showForm(id, function (isEditing) {
        overrideFormValidation();
        initAsyncSelect2('SelRoles', '/Roles/GetToSelect2', 'Asignar roles',true);
        initAsyncSelect2(
            'SelCias',
            '/Cias/GetToSelect2',
            'Asignar empresas',
            true,
        );

        $('#SelCias').on('select2-removing', function (e) {
            if (isEditing) {
                const currentDataLength = $('#SelCias').select2('data').length;
                if (currentDataLength === 1) {
                    e.preventDefault();
                    showToast(false, 'El usuario debe pertenecer a al menos una empresa.');
                }
            } else {
                if (e.choice.id === $('#codCia').val()) {
                    e.preventDefault();
                    showToast(false, 'El usuario no puede ser creado sin pertenecer a una empresa.');
                }
            }
        });

        if (isEditing) {
            $('#Active').removeAttr('disabled');
            watchChangePasswordCheckbox();

            $('#Id').val(id);
            $(`#${initValues.formAddButtonTextId}`).text('Editar');
            overrideLoadOne(id);
        } else {
            $('#changePassword').prop('checked', true).prop('disabled', true);
            $('#Password').prop('disabled', false);
            $('#RepeatPassword').prop('disabled', false);
            const currentCiaCode = $('#codCia').val();
            const currentCiaName = $('#ciaName').val();
            $('#SelCias').select2('data', [{id: currentCiaCode, text: currentCiaName}]).change();
        }
    });
}

function watchChangePasswordCheckbox() {
    $('#changePassword').change(function () {
        const isChecked = $(this).is(':checked');
        $('#Password').prop('disabled', !isChecked);
        $('#RepeatPassword').prop('disabled', !isChecked);

        if (!isChecked) {
            $('#Password').val('');
            $('#RepeatPassword').val('');
        }
    });
}

function overrideFormValidation() {
    initFormValidation({
        rules: {
            Id: { required: false },
            FirstName: { required: true, minlength: 3, maxlength: 256 },
            LastName: { required: true, minlength: 3, maxlength: 256 },
            UserName: { required: true, minlength: 2, maxlength: 256 },
            Email: { required: true, email: true, minlength: 6, maxlength: 256 },
            Password: {
                required: { depends: function() { return !($('#Id').val()!=='') || $('#changePassword').is(':checked'); } },
                minlength: 6,
                maxlength: 256
            },
            RepeatPassword: {
                required: { depends: function() { return !($('#Id').val()!=='') || $('#changePassword').is(':checked'); } },
                minlength: 6,
                maxlength: 256,
                equalTo: '#Password',
            },
            selRoles: { required: false },
            SelCias: { required: false },
        },
        showErrorsCb: function (errorMap, errorList, validator) {
            validator.defaultShowErrors();
        },
        submitHandlerCb: function (form, event) {
            doSave({});
        }
    });
}

function overrideLoadOne(id) {
    loadOne({
        url: API.getOneURL.replaceAll('{userId}', id),
        success: function (data) {
            if(data.success){ setFormData(data.data); }
        }
    });
}

function setFormData(data) {
    $('#FirstName').val(data.FirstName);
    $('#LastName').val(data.LastName);
    $('#UserName').val(data.UserName);
    $('#Email').val(data.Email);
    $('#Active').prop('checked', data.Active ?? false);
    $('#SelRoles').select2('data', data.SelRoles).change();
    $('#SelCias').select2('data', data.SelCias).change();
    $('input[type=hidden][name="OldRoles[]"]').val(data.OldRoles);
    $('input[type=hidden][name="OldCias[]"]').val(data.OldCias);
}

function overrideDeleteOne(id) {
    deleteOne({
        message: '¿Esta seguro que desea eliminar este usuario?',
        url: API.deleteOne.replaceAll('{userId}', id),
        success: function (data) {
            showToast(data.success, data.message);
            if(data.success){ dtTable.ajax.reload(); }
        }
    });
}

function showCompanyAccessDialog(user) {
    companyAccessDialog = bootbox.dialog({
        title: `Editar acceso por empresa<br/><small class="text-secondary">${user.FirstName} ${user.LastName} (${user.Email})</small>`,
        message: $('#userCiasFormHtmlId').val(),
        className: initValues.formModalSize,
        onEscape: true
    });

    validateCurrentCompanyRolesForm();

    loadOne({
        url: API.getOneURL.replaceAll('{userId}', user.Id),
        success: function (data) {
            if(data.success){
                // initAsyncSelect2('SelCiaCompanyAccess', '/Cias/GetToSelect2', 'Asignar empresas',false);
                initLocalSelect2('SelCiaCompanyAccess', data.data.SelCias, 'Asignar empresa', false);
                initAsyncSelect2(
                    'SelRolesCompanyAccess',
                    '/Roles/GetToSelect2',
                    'Asignar roles',
                    true,
                    function (term, page) {
                        return {
                            q: term,
                            codCia: $('#SelCiaCompanyAccess').val()
                        };
                    }
                );
                setCompanyAccessFormInitialData(user);
                listenCompanySelect(user);
            }
        }
    });
}

function setCompanyAccessFormInitialData(data) {
    $('#UserIdCompanyAccess').val(data.Id);

    const currentCiaCode = $('#codCia').val();
    const currentCiaName = $('#ciaName').val();

    setSelect2Data('#SelCiaCompanyAccess', {id: currentCiaCode, text: currentCiaName});
    getCurrentCompanyRoles(data.Id, currentCiaCode);
}

function listenCompanySelect(data) {
    $('#SelCiaCompanyAccess').on('change', function () {
        if ($('#SelCiaCompanyAccess').val() === '') {
            readOnlySelect2('#SelRolesCompanyAccess', true);
            setSelect2Data('#SelRolesCompanyAccess', null);
        } else {
            readOnlySelect2('#SelRolesCompanyAccess', false);
            getCurrentCompanyRoles(data.Id, $('#SelCiaCompanyAccess').val());
        }
    });
}

function getCurrentCompanyRoles(userId, ciaCode) {
    GET({
        url: `/Users/GetUserRolesByCia?userId=${userId}&codCia=${ciaCode}`,
        success: function (data) {
            if (data.success) {
                setSelect2Data('#SelRolesCompanyAccess', data.data.roles);
                $('input[type=hidden][id="OldRolesCompanyAccess[]"]').val(data.data.oldRoles);
            }
        }
    });
}

function validateCurrentCompanyRolesForm() {
    $('#userCompanyAccessFormId').validate({ // number: true
        rules: {
            UserId: { required: true },
            SelCia: { required: true },
            SelRoles: { required: false },
        },
        showErrors: function(errorMap, errorList) {
            this.defaultShowErrors();
        },
        submitHandler: function (form, event) {
            UpdateUserCompanyAccess();
        }
    });
}

function UpdateUserCompanyAccess() {
    isLoading('#usersFormCompanyAccessAddButtonId', true);
    const formData = $('#userCompanyAccessFormId').serialize();

    POST({
        url: '/Users/UpdateCompanyAccess',
        data: formData,
        success: function (data) {
            showToast(data.success, data.message);
            if(data.success) {
                hideForm(companyAccessDialog);
                reloadTable();
            }
        }
    });
}