'use strict';

overrideIds({
    tableId: 'dmgPolizaDataTableId',
    gridButtonsId: 'dmgPolizaDataTableActionsContainerId',
});

API.tableURL = '/DmgPoliza/GetForDt?fechaInicio={fechaInicio}&fechaFin={fechaFin}&doctoType={doctoType}';
API.getOneURL = '/DmgPoliza/GetOneBy?codCia={codCia}&period={period}&doctoType={doctoType}&numPoliza={numPoliza}';
initValues.formModalSize = 'modalExtraLarge';

$(document).ready(function () {
    overridePrepareButtons();
    overrideInitDt();
    initCalendarFilters();
    initTipoDoctoSelect('tipoDoctoFilter', 'Filtrar por tipo de documento');
    listenDoctoTypeChanges();
});

function listenDoctoTypeChanges(id) {
    $('#tipoDoctoFilter').on('change', function () {
        const newUrl = API.tableURL
            .replaceAll('{fechaInicio}', getStartDate())
            .replaceAll('{fechaFin}', getFinishDate())
            .replaceAll('{doctoType}', getDoctoType());
        reloadTableWithNewURL(newUrl);
    });
}

function getStartDate() {
    return $('#FECHA-INICIO').val();
}

function getFinishDate() {
    return $('#FECHA-FIN').val();
}

function getDoctoType() {
    return $('#tipoDoctoFilter').val();
}

function overridePrepareButtons() {
    prepareButtons(function () {});
}

function overrideInitDt() {
    initDt(
        function (table) {
            dtTable = table.DataTable({
                processing: true,
                serverSide: true,
                filter: true,
                'ajax': {
                    'url': API.tableURL
                        .replaceAll('{fechaInicio}', '')
                        .replaceAll('{fechaFin}', '')
                        .replaceAll('{doctoType}', ''),
                    'type': 'POST',
                    'datatype': 'JSON',
                },
                'rowCallback': function(row, data, index) {
                    $(row).find('td:eq(4)').addClass('break-word');
                    $(row).find('td:eq(5)').addClass('text-right');
                    $(row).find('td:eq(6)').addClass('text-right');
                },
                'columns': [
                    { searchable: false, 'data': 'RowNum',},
                    {
                        // sortable: false,
                        'data': 'FECHA',
                        render: function (data, type, row) {
                            return moment(data).format(CONSTANTS.defaults.date.formats.date);
                        }
                    },
                    {'data': 'NUM_POLIZA'},
                    { 'data': 'NOMBRE_DOCTO',},
                    {'data': 'CONCEPTO'},
                    {
                        'data': 'TOTAL_POLIZA',
                        render: function (data, type, row) {
                            return formatMoney(data);
                        }
                    },
                    {
                        'data': 'DiferenciaCargoAbono',
                        render: function (data, type, row) {
                            return formatMoney(data);
                        }
                    },
                    {
                        'data': 'Asiento_Impreso',
                        render: function (data, type, row) {
                            return data === 'S'
                                ? `<center><span class="badge badge-success">IMPRESO</span></center>`
                                : `<center><span class="badge badge-danger">NO IMPRESO</span></center>`;
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
                if (action === 'detail') { showDetRepoGrid(data); }
                if (action === 'uncapitalize') { uncapitalizeAccounts(data); }
                if (action === 'report') { makeReport(data); }
            });
        }
    );
}

function makeReport(data) {
    const codCia = data.COD_CIA;
    const tipoDocto = data.TIPO_DOCTO;
    const numPoliza = data.NUM_POLIZA;
    const periodo = data.PERIODO;
    const url = `/Reports/PolizaDiarioOMayor?codCia=${codCia}&tipoDocto=${tipoDocto}&numPoliza=${numPoliza}&periodo=${periodo}&reportType=mayor`;
    window.open(url, '_blank');
    setTimeout(function () {
        reloadTable();
    }, 4000);
}

function overrideLoadOne(codCia, period, doctoType, numPoliza) {
    loadOne({
        url: API.getOneURL
            .replaceAll('{codCia}', codCia)
            .replaceAll('{period}', period)
            .replaceAll('{doctoType}', doctoType)
            .replaceAll('{numPoliza}', numPoliza),
        success: function (data) {
            if(data.success){ setFormData(data.data); }
        }
    });
}

function setFormData(data) {
    $('#COD_CIA').val(data.COD_CIA);
    $('#NUM_REFERENCIA').val(data.NUM_REFERENCIA);
    $('#selSTAT_POLIZA').select2('data', data.selSTAT_POLIZA).change();
    $('#NUM_POLIZA').val(data.NUM_POLIZA);
    $('#selTIPO_DOCTO').select2('data', data.selTIPO_DOCTO).change();
    $('#FECHA').val(moment(data.FECHA).format(CONSTANTS.defaults.date.formats.date));
    $('#PERIODO').val(moment(data.PERIODO).format(CONSTANTS.defaults.date.formats.year));
    $('#ANIO').val(moment(data.ANIO).format(CONSTANTS.defaults.date.formats.year));
    $('#MES').val(moment(data.MES).format(CONSTANTS.defaults.date.formats.month));
    $('#CONCEPTO').val(data.CONCEPTO);
    $('#TOTAL_POLIZA').val(data.TOTAL_POLIZA);
}

function showDetRepoGrid(data) {
    detRepoId.codCia = data.COD_CIA;
    detRepoId.period = data.PERIODO;
    detRepoId.doctoType = data.TIPO_DOCTO;
    detRepoId.numPoliza = data.NUM_POLIZA;

    detRepoDialog = bootbox.dialog({
        title: 'Asientos contables',
        message: $(detRepoGridHtmlId).val(),
        onEscape: true,
        className: 'modalExtraLarge',
    });

    initSelect2Paginated(
        'selTIPO_DOCTO',
        '/DmgDoctos/GetToSelect2',
        'Tipo de documento'
    );

    $('#selSTAT_POLIZA').select2({ data: CONSTANTS.defaults.select.estadoPoliza });
    readOnlyInput('#selSTAT_POLIZA', true);
    readOnlySelect2('#selTIPO_DOCTO', true);
    $('#det_PERIODO').val(data.PERIODO);
    $('#det_TIPO_DOCTO').val(data.TIPO_DOCTO);
    $('#det_NUM_POLIZA').val(data.NUM_POLIZA);

    overrideLoadOne(data.COD_CIA, data.PERIODO, data.TIPO_DOCTO, data.NUM_POLIZA);
    dmgDetalleInitGrid();
}

/**
 *
 * @param data
 */
function uncapitalizeAccounts(data) {
    showConfirm({
        message: '¿Desmayorizar este asiento contable?',
        result: function (result) {
            if(!result) return;
            showLoadingDialog(true, 'Desmayorizando cuentas ...');
            const formData= `codCia=${data.COD_CIA}&periodo=${data.PERIODO}&tipoDocto=${data.TIPO_DOCTO}&numPoliza=${data.NUM_POLIZA}`;

            $.ajax({
                url: '/DmgPoliza/UncapitalizeAccounts',
                type: 'POST',
                data: formData,
                success:function(data){
                    // isLoading('#btnSavePeriodParam', false);
                    showLoadingDialog(false);
                    showToast(data.success, data.message);
                    if(data.success) { reloadTable(); }
                },
                error: function (error) {
                    showToast(false, 'Ocurrió un error al procesar la solicitud');
                    // isLoading('#btnUpdateContaParams', false);
                    showLoadingDialog(false);
                }
            });
        }
    });
}

function initCalendarFilters() {
    $('#FECHA-INICIO_container').datetimepicker({ format: CONSTANTS.defaults.date.formats.date, locale: 'es', allowInputToggle: true });
    $('#FECHA-FIN_container').datetimepicker({ format: CONSTANTS.defaults.date.formats.date, locale: 'es', allowInputToggle: true });

    $('#FECHA-INICIO_container').on('change.datetimepicker', function () {
        if (getStartDate() === '') {
            const newUrl = API.tableURL
                .replaceAll('{fechaInicio}', '')
                .replaceAll('{fechaFin}', '')
                .replaceAll('{doctoType}', getDoctoType());

            reloadTableWithNewURL(newUrl);
        }
        changeMinDateTempusdominus(getStartDate());
    });

    $('#FECHA-FIN_container').on('change.datetimepicker', function () {
        const finishDateStr = getFinishDate();
        const startDateStr = finishDateStr === '' ? '' : getStartDate();

        const newUrl = API.tableURL
            .replaceAll('{fechaInicio}', startDateStr)
            .replaceAll('{fechaFin}', finishDateStr)
            .replaceAll('{doctoType}', getDoctoType());

        reloadTableWithNewURL(newUrl);
    });
}

function changeMinDateTempusdominus(minDate) {
    if (minDate === '') {
        $('#FECHA-FIN_container').datetimepicker('minDate', null);
        disableInput('#FECHA-FIN', true);
        return;
    }

    $('#FECHA-FIN_container').datetimepicker('minDate', minDate);
    disableInput('#FECHA-FIN', false);
}