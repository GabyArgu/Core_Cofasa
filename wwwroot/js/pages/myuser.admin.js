'use strict';

let APIMyProfile = {
    tableURL: '',
    getOneURL: '',
    saveOneURL: '',
    deleteOne: '',
}

APIMyProfile.save = '/Users/SaveOrUpdate';
APIMyProfile.get = '/Users/GetOneUser?userId={userId}';

$(document).ready(function () {
    $("#editMyUser").click(function () { showEditProfile($('#myProfileId').val()) });
});

let myProfileDialog;

function showEditProfile(id) {
    myProfileDialog = bootbox.dialog({
        title: "Editar perfil",
        message: $("#editMyUserFormHtmlId").val().replaceAll('ttextarea', 'textarea'),
        className: "",
        onEscape: true
    });
    overrideMyProfileFormValidation();

    initAsyncSelect2('SelRolesMyProfile', '/Roles/GetToSelect2', 'Asignar roles', true);
    initAsyncSelect2('SelCiasMyProfile', '/Cias/GetToSelect2', 'Asignar empresas', true);

    $('#MyProfileActive').removeAttr('disabled');
    watchChangePasswordCheckboxProfile();

    $('#MyProfileId').val(id);
    loadMyProfile(id);
}

function watchChangePasswordCheckboxProfile() {
    $('#changePasswordMyProfile').change(function () {
        const isChecked = $(this).is(':checked');
        $('#PasswordMyProfile').prop('disabled', !isChecked);
        $('#RepeatPasswordMyProfile').prop('disabled', !isChecked);

        if (!isChecked) {
            $('#PasswordMyProfile').val('');
            $('#RepeatPasswordMyProfile').val('');
        }
    });
}

function initMyProfileFormValidation({ rules, showErrorsCb, submitHandlerCb }) {
    $("#myProfileFormId").validate({
        ignore: [],
        rules: rules,
        showErrors: function (errorMap, errorList) { if (isFunction(showErrorsCb)) showErrorsCb(errorMap, errorList, this) },
        submitHandler: function (form, event) { if (isFunction(submitHandlerCb)) submitHandlerCb(form, event); }
    });
}

function overrideMyProfileFormValidation() {
    initMyProfileFormValidation({
        rules: {
            MyProfileId: { required: true },
            FirstNameMyProfile: { required: true, minlength: 3, maxlength: 256 },
            LastNameMyProfile: { required: true, minlength: 3, maxlength: 256 },
            UserNameMyProfile: { required: true, minlength: 2, maxlength: 256 },
            EmailMyProfile: { required: true, email: true, minlength: 6, maxlength: 256 },
            PasswordMyProfile: {
                required: { depends: function () { return !($('#MyProfileId').val() !== '') || $('#changePasswordMyProfile').is(':checked'); } },
                minlength: 6,
                maxlength: 256
            },
            RepeatPasswordMyProfile: {
                required: { depends: function () { return !($('#MyProfileId').val() !== '') || $('#changePasswordMyProfile').is(':checked'); } },
                minlength: 6,
                maxlength: 256,
                equalTo: '#PasswordMyProfile',
            },
        },
        showErrorsCb: function (errorMap, errorList, validator) {
            validator.defaultShowErrors();
        },
        submitHandlerCb: function (form, event) {
            doSaveMyProfile({});
        }
    });
}

function loadMyProfile(id) {
    $.ajax({
        url: APIMyProfile.get.replaceAll('{userId}', id),
        type: 'GET',
        success: function (data) {
            if (data.success) { setMyProfileFormData(data.data); }
        },
        error: function (err) {
            console.log(error);
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            if (isFunction(error)) error(err);
        }
    });
}

function setMyProfileFormData(data) { 
    $('#FirstNameMyProfile').val(data.FirstName);
    $('#LastNameMyProfile').val(data.LastName);
    $('#UserNameMyProfile').val(data.UserName);
    $('#EmailMyProfile').val(data.Email);
    $('#ActiveMyProfile').prop('checked', data.Active ?? false);
    $('#SelRolesMyProfile').select2('data', data.SelRoles).change();
    $('#SelCiasMyProfile').select2('data', data.SelCias).change();
    $('input[type=hidden][id="OldRolesMyProfile"]').val(data.OldRoles);
    $('input[type=hidden][id="OldCiasMyProfile"]').val(data.OldCias);
}

function doSaveMyProfile({ success, error }) {
    const formData = $("#myProfileFormId").serialize();
    completeDoSaveMyProfile({ success, error, formData });
}

function completeDoSaveMyProfile({ success, error, formData }) {
    isLoading("#myProfileFormSaveButtonId", true);

    $.ajax({
        url: APIMyProfile.save,
        type: 'POST',
        data: formData,
        success: function (data) {
            isLoading("#myProfileFormSaveButtonId", false);
            showToast(data.success, data.message);

            if (data.success) {
                myProfileDialog.modal("hide");
            }

            if (isFunction(success)) success(data);
        },
        error: function (err) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading("#myProfileFormSaveButtonId", false);

            if (isFunction(error)) error(err)
        }
    });
}
