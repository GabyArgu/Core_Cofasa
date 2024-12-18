'use strict';

overrideIds({
    addButtonId: 'repositoryAddButtonId',
    tableId: 'repositoryDataTableId',
    gridButtonsId: 'repositoryDataTableActionsContainerId',
    // formId: 'repositoryForm',
    formValuesId: 'repositoryForm',
    btnSaveId: 'repositoryFormAddButton',
    btnSaveTextId: 'repositoryFormAddButtonText',
});

API.tableURL = '/Repository/GetForDt?doctoType={doctoType}';
API.saveOneURL = '/Repository/SaveOrUpdateOrDeleteOne';
API.getOneURL = '/Repository/GetOneBy?codCia={codCia}&period={period}&doctoType={doctoType}&numPoliza={numPoliza}';
initValues.formModalSize = 'modalExtraLarge';

let reportDialog = null;
let importFromFileDialog = null;

// USADOS PARA MAYORIZAR PARTIDAS AL SALIR DEL DIALOG
var temp_PARTIDA_DIFF = null;
var temp_COD_CIA = null;
var temp_PERIODO = null;
var temp_TIPO_DOCTO = null;
var temp_NUM_POLIZA = null;

$(document).ready(function () {
    overridePrepareButtons();
    overrideInitDt();
    initTipoDoctoSelect('tipoDoctoFilter', 'Filtrar por tipo de documento');
    listenDoctoTypeChanges();
});

function listenDoctoTypeChanges() { 
    $('#tipoDoctoFilter').on('change', function () {
        const newUrl = API.tableURL.replaceAll('{doctoType}', $(this).val());
        reloadTableWithNewURL(newUrl);
    });
}

function overridePrepareButtons() {
    prepareButtons(function () {
        $(`#${initValues.addButtonId}`).click(function(){ showDetRepoGrid(); });
        $('#repositoryImportButton').click(function(){ showImportFileDialog(); });
    });
}

function overrideInitDt() {
    initDt(
        function (table) {
            dtTable = table.DataTable({
                processing: true,
                serverSide: true,
                filter: true,
                'ajax': {
                    'url': API.tableURL.replaceAll('{doctoType}', ''),
                    'type': 'POST',
                    'datatype': 'JSON',
                },
                'createdRow': function(row, data, dataIndex) {
                    // Verifica si ambos valores son 0 o nulos/indefinidos
                    if ((data.DiferenciaCargoAbono === 0 || isUndefinedNullOrEmpty(data.DiferenciaCargoAbono)) &&
                    (data.TOTAL_POLIZA === 0 || isUndefinedNullOrEmpty(data.TOTAL_POLIZA))) {
                        // Si ambos son 0, agrega la clase 'hasDifference'
                        $(row).addClass('hasDifference');
                    }
                    // Si solo 'DiferenciaCargoAbono' tiene valor diferente de 0, se agrega la clase
                    else if (!isUndefinedNullOrEmpty(data.DiferenciaCargoAbono) && data.DiferenciaCargoAbono !== 0) {
                        $(row).addClass('hasDifference');
                    }
                },
                'rowCallback': function(row, data, index) {
                    $(row).find('td:eq(5)').addClass('break-word');
                    $(row).find('td:eq(6)').addClass('text-right');
                    $(row).find('td:eq(7)').addClass('text-right');
                },
                'columns': [
                    // {'data': 'COD_CIA'},
                    // {sortable: false, searchable: false, visible: false, 'data': 'FECHA_CAMBIO'},
                    { searchable: false, 'data': 'RowNum',},
                    {
                        'data': 'FECHA',
                        // sortable: false,
                        render: function (data, type, row) {
                            return moment(data).format(CONSTANTS.defaults.date.formats.date);
                        }
                    },
                    {'data': 'NUM_POLIZA'},
                    {'data': 'NOMBRE_DOCTO'},
                    {
                        'data': 'STAT_POLIZA',
                        render: function (data, type, row) {
                            return data === 'G'
                                ? 'Grabada'
                                : 'Revisada';
                        }
                    },
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
                        sortable: false, searchable: false,
                        render: function (data, type, row) {
                            let buttons = gridButtons.replace('{data}',Base64.encode($.toJSON(row)));
                            if (!isUndefinedNullOrEmpty(row.DiferenciaCargoAbono) && row.DiferenciaCargoAbono !== 0) {
                                return buttons.replaceAll('{mayorizar_style}', 'display:none;');
                                
                            }else if ((row.DiferenciaCargoAbono === 0 || isUndefinedNullOrEmpty(row.DiferenciaCargoAbono)) &&
                            (row.TOTAL_POLIZA === 0 || isUndefinedNullOrEmpty(row.TOTAL_POLIZA))) {
                                // Si ambos son 0, oculta el botón
                                return buttons.replaceAll('{mayorizar_style}', 'display:none;');
                            }
                            else {
                                return buttons;
                            }
                        }
                    }
                ]
            });
        },
        function () {
            bindButtons(function (action, data) {
                if (action === 'asientos') { showDetRepoGrid(data); }
                if (action === 'deleterepo') { saveUpdateOrDeleteHeader('ELIMINAR', data); }
                // if (action === 'capitalizerepo') { capitalizeAccounts(data); }
                if (action === 'capitalizerepo') { capitalizeAccounts(data.COD_CIA, data.PERIODO, data.TIPO_DOCTO, data.NUM_POLIZA); }
                if (action === 'repoprint') { makeReport(data); }
                if (action === 'excelprint') { makeExcel(data); }
            });
        }
    );
}

function makeReport(data) {
    const codCia = data.COD_CIA;
    const tipoDocto = data.TIPO_DOCTO;
    const numPoliza = data.NUM_POLIZA;
    const periodo = data.PERIODO;
    const url = `/Reports/PolizaDiarioOMayor?codCia=${codCia}&tipoDocto=${tipoDocto}&numPoliza=${numPoliza}&periodo=${periodo}&reportType=diario`;
    window.open(url, '_blank');
}

function makeExcel(data) {
    const codCia = data.COD_CIA;
    const tipoDocto = data.TIPO_DOCTO;
    const numPoliza = data.NUM_POLIZA;
    const periodo = data.PERIODO;
    const url = `/Reports/ExportarPolizaDiarioOMayorExcel?codCia=${codCia}&tipoDocto=${tipoDocto}&numPoliza=${numPoliza}&periodo=${periodo}&reportType=diario`;
    window.open(url, '_blank');
}

function overrideFormValidation(action) {
    initFormValidation({
        showErrorsCb: function (errorMap, errorList, validator) {
            validator.defaultShowErrors();
            // hideFieldsErrorsMessages(['FECHA', 'PERIODO', 'ANIO', 'MES']);
        },
        submitHandlerCb: function (form, event) {
            saveUpdateOrDeleteHeader(action);
        }
    });
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
    // $('#TOTAL_POLIZA').val(data.TOTAL_POLIZA);
}

function initDetailSelectsAndCalendars() {
    initTipoDoctoSelect('selTIPO_DOCTO', 'Tipo de documento');

    $('#selSTAT_POLIZA').select2({ data: CONSTANTS.defaults.select.estadoPoliza });
    $('#PERIODO_container').datetimepicker({ format: CONSTANTS.defaults.date.formats.year, locale: 'es', allowInputToggle: true });
    $('#ANIO_container').datetimepicker({ format: CONSTANTS.defaults.date.formats.year, locale: 'es', allowInputToggle: true });
    $('#MES_container').datetimepicker({ viewMode: 'months', format: CONSTANTS.defaults.date.formats.month, locale: 'es', allowInputToggle: true });
    $('#FECHA_container').datetimepicker({ format: CONSTANTS.defaults.date.formats.date, locale: 'es', allowInputToggle: true });

    $('#FECHA_container').on('change.datetimepicker', function () {
        const dateStr = $('#FECHA').val();
        // $('#NUM_REFERENCIA').val(dateStr.replaceAll('/', ''));
        $('#NUM_REFERENCIA').val(dateStr);
        $('#PERIODO, #ANIO').val(getYearFromDate(dateStr));
        $('#MES').val(getMonthFromDate(dateStr));
    });

    $('#cleanDetRepoButton').on('click', function () {
        cleanDetRepoForm();
    });

    // FORMULARIO DE LA TABLA HIJA: DET_REPOSITORY
    watchCargoAndAbonoChanges();

    initSelect2Paginated(
        'CENTRO_COSTO',
        '/CentroCosto/GetToSelect2',
        'Centro costo'
    );

    initSelect2Paginated(
        'CENTRO_CUENTA',
        '/CentroCuenta/GetToSelect2',
        'Cuenta contable',
        false,
        function(term, page) {
            return {
                codCia: $('#COD_CIA').val(),
                centroCosto: $('#CENTRO_COSTO').select2('val'),
                q: term,
                page: page || 1,
                pageSize: 10
            };
    });

    $('#CENTRO_COSTO').on('change', function () { validateDetRepoFormSel2(); });
}

function showDetRepoGrid(data) {
    const isEditing = !(typeof (data) === 'undefined' || data === '' || data===null);
    IS_ADDING = !isEditing;

    if (isEditing) {
        detRepoId.codCia = data.COD_CIA;
        detRepoId.period = data.PERIODO;
        detRepoId.doctoType = data.TIPO_DOCTO;
        detRepoId.numPoliza = data.NUM_POLIZA;
        setTempPartidaDiffValues(data.COD_CIA, data.PERIODO, data.TIPO_DOCTO, data.NUM_POLIZA);
        console.log(detRepoId);
    }

    const TITLE = isEditing ? 'Editar asientos contables' : 'Agregar asientos contables';

    detRepoDialog = bootbox.dialog({
        title: TITLE,
        message: $(detRepoGridHtmlId).val(),
        className: 'modalExtraLarge',
    });

    detRepoDialog.on('hidden.bs.modal', function(e){
        IS_ADDING = false;
        DT_LOCAL_DATA = [];
        reloadTable();
        //if (!isUndefinedNullOrEmpty(temp_PARTIDA_DIFF) && temp_PARTIDA_DIFF === 0) {
        //capitalizeAccounts(temp_COD_CIA, temp_PERIODO, temp_TIPO_DOCTO, temp_NUM_POLIZA);
        
        //}
    });

    initDetailSelectsAndCalendars();
    centroCuentaNumbersToInputs();

    if (isEditing) {
        $(`#${initValues.formAddButtonTextId}`).text('Actualizar encabezado');
        $('#OPERACION').val('MODIFICAR');

        readOnlyInput('#NUM_POLIZA', true);
        readOnlySelect2('#selTIPO_DOCTO', true);
        hideElement('#repositoryFormAddAllButton');
        overrideLoadOne(data.COD_CIA, data.PERIODO, data.TIPO_DOCTO, data.NUM_POLIZA);

        // $('#det_COD_CIA').val(data.COD_CIA);
        $('#det_PERIODO').val(data.PERIODO);
        $('#det_TIPO_DOCTO').val(data.TIPO_DOCTO);
        $('#det_NUM_POLIZA').val(data.NUM_POLIZA);
    } else {
        prepareDetRepoButtonsOffline();
        readOnlySelect2('#CENTRO_CUENTA', true);

        $('#repositoryFormAddButton').hide();
        $('#repositoryFormAddAllButton').on('click', function () {
            saveAll(initValues.formID);
        });
    }

    detRepoPrepareButtons();
    detRepoInitGrid();
    overrideFormValidation(isEditing ? 'MODIFICAR' : 'AGREGAR');

    // PARA EL GRID DE DETALLE
    detRepoStartValidation();
    // watchListOfAccountNumbers(listOfAccountsForContaRepoDetRepo);
    listenEscKey();
    // set timeout
    setTimeout(function () {
        setSelect2Focus(isEditing ? '#CENTRO_COSTO' : '#selSTAT_POLIZA');
    }, 300);
}

function watchCargoAndAbonoChanges() {
    $('#CARGO, #ABONO').on('keyup', function () {
        const cargo = $('#CARGO').val()
        const abono = $('#ABONO').val()

        if ((isUndefinedNullOrEmpty(cargo) || cargo=='0') && (isUndefinedNullOrEmpty(abono) || abono=='0')) {
            $('#CARGO, #ABONO').removeAttr('readonly');
        }
        else if (isUndefinedNullOrEmpty(cargo)) {
            $('#CARGO').prop('readonly', true);
            $('#CARGO').val(0);
        }
        else if (isUndefinedNullOrEmpty(abono)) {
            $('#ABONO').prop('readonly', true);
            $('#ABONO').val(0);
        }
    });
}

function saveUpdateOrDeleteHeader(action, data) {
    const formData = $(`#${initValues.formID}`).serialize();
    $('#OPERACION').val(action);

    if (action === 'AGREGAR' || action === 'MODIFICAR') {
        callSaveUpdateOrDeleteHeader(formData);
    } else if (action === 'ELIMINAR') {
        showConfirm({
            message: '¿Está seguro de eliminar el registro?',
            result: function (result) {
                if(!result) return;
                const newFormData = `COD_CIA=${data.COD_CIA}&NUM_REFERENCIA=${data.NUM_REFERENCIA}
                    &OPERACION=ELIMINAR&PERIODO=${data.PERIODO}&ANIO=${data.ANIO}&MES=${data.MES}
                    &STAT_POLIZA=${data.STAT_POLIZA}&NUM_POLIZA=${data.NUM_POLIZA}&TIPO_DOCTO=${data.TIPO_DOCTO}
                    &FECHA=${data.FECHA}&CONCEPTO=${data.CONCEPTO}`;
                callSaveUpdateOrDeleteHeader(newFormData);
            }
        });
    }
}

function callSaveUpdateOrDeleteHeader(formData, callback, errorCallback) {
    POST({
        url: API.saveOneURL,
        data: formData,
        success: function (data) {
            if (isFunction(callback)) callback(data);
            else {
                isLoading(`#${initValues.btnSaveId}`, false);
                showToast(data.success, data.message);
                if(data.success) { reloadTable(); }
            }
        },
        error: function (err) {
            if (isFunction(errorCallback)) errorCallback(err);
            else {
                showToast(false, 'Ocurrió un error al procesar la solicitud');
                isLoading(`#${initValues.btnSaveId}`, false);
            }
        }
    });
}

function setTempPartidaDiffValues(codCIA, periodo, tipoDOCTO, numPOLIZA) {
    if (isUndefinedNullOrEmpty(codCIA) || isUndefinedNullOrEmpty(periodo) || isUndefinedNullOrEmpty(tipoDOCTO)
        || isUndefinedNullOrEmpty(numPOLIZA)) {
        temp_COD_CIA = null;
        temp_PERIODO = null;
        temp_TIPO_DOCTO = null;
        temp_NUM_POLIZA = null;
    } else {
        temp_COD_CIA = codCIA;
        temp_PERIODO = periodo;
        temp_TIPO_DOCTO = tipoDOCTO;
        temp_NUM_POLIZA = numPOLIZA;
    }
}

function capitalizeAccounts(codCIA, periodo, tipoDOCTO, numPOLIZA) {
    showConfirm({
        message: '¿Mayorizar este asiento contable?',
        result: function (result) {
            if(!result) return;
            showLoadingDialog(true, 'Mayorizando asientos, por favor espere ...');
            const formData= `codCia=${codCIA}&periodo=${periodo}&tipoDocto=${tipoDOCTO}&numPoliza=${numPOLIZA}`;

            $.ajax({
                url: '/Repository/CapitalizeAccounts',
                type: 'POST',
                data: formData,
                success:function(data){
                    showLoadingDialog(false, '');
                    showToast(data.success, data.message);
                    if(data.success) { reloadTable(); }
                    setTempPartidaDiffValues();
                },
                error: function (error) {
                    showToast(false, 'Ocurrió un error al procesar la solicitud');
                    // isLoading('#btnUpdateContaParams', false);
                    showLoadingDialog(false, '');
                    setTempPartidaDiffValues();
                }
            });
        }
    });
}

function showImportFileDialog() {
    const title = 'Importar asientos contables';
    const message = $('#importRepoFormHtml').val();

    importFromFileDialog = bootbox.dialog({
        title: `<div class="row">
                    <div class="col-6">
                        <h4 class="modal-title">${title}</h4>
                    </div>
                    <div class="col-6">
                        <div class="btn-toolbar float-right" role="toolbar" aria-label="">
<!--                            <div class="btn-group mr-2" role="group" aria-label="First group">-->
<!--                                <button type="button" class="btn btn-primary" id="printReport">-->
<!--                                    <i class="fa fa-print"></i>&nbsp;Imprimir-->
<!--                                </button>-->
<!--                            </div>-->
                            <div class="btn-group mr-2" role="group" aria-label="Second group">
                                <button type="button" class="btn btn-success" id="downloadCsvFormat">
<!--                                    <i class="fa fa-arrow-down"></i>&nbsp;Descargar formato-->
                                    <i class="fa fa-file-excel-o"></i>&nbsp;Descargar formato
                                </button>
                            </div>
                        </div>
                    </div>
                </div>`,
        message: message,
        onEscape: true,
        className: 'modalMedium',
    });

    listenFileDrop();
}
