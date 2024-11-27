'use strict';

var orgininalHtml = null;
var loadingDialog;

$(document).ready(function () {
    httpInterceptor();
    getCias();
    listenCiasChanges();
    fixMenuSelected();
});

function getCias() {
}

function listenCiasChanges() {
    $('#selCias').change(function () {
        $.ajax({
            type: 'GET',
            url: '/Home/ChangeCia?newCiaCode=' + $(this).val(),
            success: function (data) {
                location.reload();
            },
            error: function (response) {
                console.log('Failed');
            }
        });
    });
}

function fixMenuSelected() {
    const url = window.location.href.toLowerCase();

    $('ul.nav-sidebar a').filter(function () { return this.href.toLowerCase()===url; })
        .addClass('active')
        .closest('.parent')
        .addClass('menu-open')
        .children('a')
        .addClass('active');
}

function drawRowNumbers(selector, table) {
    if (typeof (table) == 'undefined') return;

    $.each($(selector + ' tbody tr td:first-child'), function (idx, obj) {
        if ($(obj).hasClass('dataTables_empty')) return;
        $(obj).addClass('text-center').html((idx++) + 1);
    });
}

/**
 * @author Edgar Mejía
 * @param elementId string of btn to manipulate with # like: #btn-id
 * @param isLoading condition to show/hide loading animation
 * Both params are required
 *
 */
function isLoading(elementId, isLoading){
    $(elementId).prop({disabled : isLoading});

    orgininalHtml = (isLoading) ? $(elementId).html() : orgininalHtml;
    const newHtml = '<i class="fa fa-spinner fa-spin fa-fw" id="loading-spinner"></i> Cargando ...';

    return $(elementId).html((isLoading) ? newHtml : orgininalHtml);
}

function showLoadingDialog(isLoading, message) {
    let newMessage = (typeof message === 'undefined' || message===null || message==='')
        ? 'Por favor espere un momento ...' : message;

    return (isLoading) ? loadingDialog = bootbox
            .dialog({
                message: `<p class="text-center mb-0"><i class="fa fa-spin fa-cog"></i> ${newMessage}</p>`,
                closeButton: false
            })
        : loadingDialog.modal('hide');
}

function showToast(success, message) {
    if(typeof message==='undefined' || message===null){
        message = 'No permitido';
    }
    
    var style = success ? {
        background: 'rgb(0, 176, 155)',
        transform: 'translate(0px, 0px); top: 15px',
        fontSize: '1.2em'
    } : {
        background: 'rgb(255, 95, 109)',
        transform: 'translate(0px, 0px); top: 15px',
        fontSize: '1.2em'
    };

    Toastify({
        text: message,
        // className: type,
        position: 'center',
        style: style,
    }).showToast();
}

function httpInterceptor() {
    // $(document).ajaxStart(function () {
    //     showLoadingDialog(true);
    // });
    //
    // $(document).ajaxStop(function () {
    //     showLoadingDialog(false);
    // });
    $(document).ajaxError(function(event, jqxhr, settings, thrownError) {
        if (jqxhr.status === 401) {
            console.log('Redirecting to login page');
            showToast(false, 'Su sesión ha expirado, por favor inicie sesión nuevamente');
            window.location.href = '/Security/Login';
        }
    });
}