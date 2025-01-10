'use strict';

var tableDmgCieCierre;
var cieCierreGridSelect = '';

$(document).ready(function () {
    initPeriodParamFormValidation();
    initCalendars();
    initDmgCieCierreGrid();
    prepareCieCierreSelect()
});

function initCalendars() {
    $('#Period_container').datetimepicker({format: CONSTANTS.defaults.date.formats.year, locale: 'es', allowInputToggle: true});
    $('#StartMonth_container').datetimepicker({viewMode: 'months', format: 'MM', locale: 'es', allowInputToggle: true});
    $('#FinishMonth_container').datetimepicker({viewMode: 'months', format: 'MM', locale: 'es', allowInputToggle: true});

    loadCurrentCiaPeriodParams();
}

function loadCurrentCiaPeriodParams() {
    $.ajax({
        url: `/Params/GetPeriodParams?ciaCod=${$('#CodCia').val()}&period=${new Date().getFullYear()}`,
        type:'GET',
        success:function(data){
            if(data.success){
                setPeriodParamsFormData(data.data);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function setPeriodParamsFormData(data) {
    if (data!=null) {
        $('#btnSavePeriodParamText').text('Actualizar');

        if (data.Period!=null) $('#Period').val(moment(data.Period).format(CONSTANTS.defaults.date.formats.year));
        if (data.StartMonth!=null) $('#StartMonth').val(moment(data.StartMonth).format(CONSTANTS.defaults.date.formats.month));
        if (data.FinishMonth!=null) $('#FinishMonth').val(moment(data.FinishMonth).format(CONSTANTS.defaults.date.formats.month));
        if (data.Status!=null) $('#Status').val(data.Status).change();
    }
}

function initPeriodParamFormValidation() {
    $('#periodParamsForm').validate({
        rules: {
            CodCia: {required: false, minlength: 3, maxlength: 3},
            Period: {required: true},
            StartMonth: {required: true},
            FinishMonth: {required: true},
            Status: {required: true},
            PROVSEGURO: {required: false}, // TODO: NO EXISTE EN LA DB
        },
        showErrors: function (errorMap, errorList) {
            this.defaultShowErrors();
            hideFieldsErrorsMessages(['Period', 'StartMonth', 'FinishMonth']);
        },
        submitHandler: function (form, event) {
            isLoading('#btnSavePeriodParam', true);
            savePeriodParams();
        }
    });
}

function savePeriodParams(){
    const formData = $('#periodParamsForm').serialize();

    $.ajax({
        url: '/Params/SavePeriodParams',
        type: 'POST',
        data: formData,
        success:function(data){
            isLoading('#btnSavePeriodParam', false);
            showToast(data.success, data.message);
            if(data.success) {
                tableDmgCieCierre.ajax.reload();
                loadCurrentCiaPeriodParams();
            }
        },
        error: function (error) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading('#btnUpdateContaParams', false);
        }
    });
}

function prepareCieCierreSelect() {
    const bodyButtons = $('#cieCierreGridSelect').val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    cieCierreGridSelect = '<center>' + tags.html() + '<center>';
}

function bindPeriodParamsSelect() {
    $('#dmgCieCierreDt tbody tr td select').unbind('change').on('change', function (event) {
        if (event.preventDefault) { event.preventDefault(); }
        if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

        const obj = JSON.parse(Base64.decode($(this).attr('data-row')));
        callCieCierreUpdateStatus(obj, $(this).val());
    });
}

function initDmgCieCierreGrid() {
    tableDmgCieCierre = $('#dmgCieCierreDt')
        .on('draw.dt', function () {
            drawRowNumbers('#dmgCieCierreDt', tableDmgCieCierre);
            setTimeout(function(){bindPeriodParamsSelect();},500);
        })
        .DataTable({
            // 'pageLength': 12,
            searching: false,
            paging: false,
            'ajax': {
                'url': `/Params/GetDmgCieCierreForDt?ciaCod=${$('#CodCia').val()}&period=${new Date().getFullYear()}`,
                'type': 'GET',
                'datatype': 'JSON',
            },
            'columns': [
                { 'data': 'CIE_CODCIA' },
                { 'data': 'CIE_ANIO' },
                {
                    'data': 'CIE_MES',
                    render: function (data, type, row, meta) {
                        return moment(data, 'MM').format('MMMM');
                    }
                },
                {
                    'data': 'CIE_ESTADO',
                    render: function (data, type, row, meta) {
                        if (row.CIE_ESTADO==='A') return 'Activo';
                        else if (row.CIE_ESTADO==='C') return 'Concluido';
                        //else if (row.CIE_ESTADO==='S') return 'Suspendido';
                        else return '';
                    }
                },
                {
                    sortable: false, searchable: false,
                    render: function (data, type, row, meta) {
                        return cieCierreGridSelect
                            .replaceAll('{codCia}', `${row.CIE_CODCIA}_${meta.row}`)
                            .replaceAll('{data}', Base64.encode($.toJSON(row)))
                            .replaceAll('{selectednull}', row.CIE_ESTADO===''||row.CIE_ESTADO===null ? '' : 'selected')
                            .replaceAll('{selecteda}', row.CIE_ESTADO==='A' ? 'selected' : '')
                            .replaceAll('{selectedc}', row.CIE_ESTADO==='C' ? 'selected' : '')
                            .replaceAll('{selecteds}', row.CIE_ESTADO==='S' ? 'selected' : '');
                    }
                }
            ]
        });

    $('#dmgCieCierreDt')
        .removeClass('display')
        .addClass('table table-bordered table-hover dataTable');
}

// Función para recargar el DataTable con un año específico
function reloadDmgCieCierreGrid(year) {
    const ciaCod = $('#CodCia').val();
    const period = year || new Date().getFullYear(); // Usa el año actual si no se selecciona ninguno.

    // Actualiza la URL del DataTable y recarga los datos
    tableDmgCieCierre.ajax.url(`/Params/GetDmgCieCierreForDt?ciaCod=${ciaCod}&period=${period}`).load();
}

// Configuración inicial y eventos
$(document).ready(function () {
    initDmgCieCierreGrid(); // Inicializa la tabla

    // Evento para filtrar por año
    $('#YearFilter').on('change', function () {
        const selectedYear = $(this).val();
        reloadDmgCieCierreGrid(selectedYear);
    });
});

function callCieCierreUpdateStatus(data, newStatus) {
    const formData = `CIE_CODCIA=${data.CIE_CODCIA}&CIE_CODIGO=${data.CIE_CODIGO}&CIE_ESTADO=${newStatus}`;

    $.ajax({
        url: '/Params/ChangeDmgCieCierreStatus',
        type:'POST',
        data: formData,
        success:function(data){
            showToast(data.success, data.message);

            if(data.success){
                tableDmgCieCierre.ajax.reload();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}