'use strict';

let detRepoDataTable;
let detRepoDialog = null;

const detRepoGridHtmlId = '#dmgDetalleGridHtml';
const detRepoDataTableId = '#dmgDetalleDataTable';

const detRepoAPI = {
    TABLE: '/DmgDetalle/GetForDt?codCia={codCia}&period={period}&doctoType={doctoType}&numPoliza={numPoliza}',
    DETAIL: '',
};

const detRepoId = {
    codCia: null,
    period: null,
    doctoType: null,
    numPoliza: null,
};

function dmgDetalleInitGrid() {
    detRepoDataTable = $(detRepoDataTableId)
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
                const totCargos = api.column(10).data().reduce(function (a, b) { return a + b; }, 0);
                const totAbonos = api.column(11).data().reduce(function (a, b) { return a + b; }, 0);
                $(api.column(10).footer()).html(formatMoney(totCargos));
                $(api.column(11).footer()).html(formatMoney(totAbonos));
            },
            'columns': [
                { sortable: false, searchable: false, 'data': 'CORRELAT' },
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
                }
            ]
        });

    $(detRepoDataTableId)
        .removeClass('display')
        .addClass('table table-bordered table-hover dataTable');
}
