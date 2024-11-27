let ACCOUNTS = {};

$(document).ready(function () {
    initFilters();
    validateReportFilters();
});

function initFilters() {
    $('#selReportType').select2({ data: CONSTANTS.defaults.select.reportType, placeholder: 'Tipo de reporte' });

    $('#selReportLevel').select2({ data: CONSTANTS.defaults.select.reportLevel, placeholder: 'Nivel de cuenta' });

    initSelect2Paginated(
        'CENTRO_COSTO',
        '/CentroCosto/GetToSelect2',
        'Centro costo'
    );

    initSelect2Paginated(
        'selCuentaContable',
        '/CentroCuenta/GetToSelect2',
        'Cuenta contable',
        false,
        function (term, page) {
            return {
                codCia: $('#codCia').val(),
                centroCosto: $('#CENTRO_COSTO').select2('val'),
                q: term,
                page: page || 1,
                pageSize: 10
            };
        });

    initCalendar('#startDateContainer');
    initCalendar('#finishDateContainer');

    readOnlySelect2('#CENTRO_COSTO', true);
    readOnlySelect2('#selCuentaContable', true);
    readOnlyInput('#startDate', true);
    readOnlyInput('#finishDate', true);
    readOnlySelect2('#selReportLevel', true);
    disableInput('#btnGenerateReport', true);
    disableInput('#btnGenerateExcel', true);

    $('#selReportType').on('change', function () {
        if ($(this).val() !== '') { watchFilters($(this).val()); }
        else { clearFiltersForm(); }
    });

    $('#CENTRO_COSTO').on('change', function () {
        // if ($(this).val() !== '') {
        //     setSelect2Data('#selCuentaContable', null);
        // }
        setSelect2Data('#selCuentaContable', null);
    });

    $('#selCuentaContable').on('change', function () {
        if ($(this).val() !== '') {
            const dataList = getAccountNumbersFromString('|', $(this).val());
            decodeDataFromContaAccountVal(dataList, function (dataObj) {
                ACCOUNTS = dataObj;
            });
        } else {
            ACCOUNTS = {};
        }
    });
}

function validateReportFilters() {
    $('#reportsFilters').validate({
        rules: {
            selReportType: { required: true }, // Select2
            CENTRO_COSTO: { required: { depends: function () { return $('#selReportType').val() === 'HDC'; } } },
            selCuentaContable: { required: { depends: function () { return $('#selReportType').val() === 'HDC'; } } },
            startDate: { required: true }, // DateTimePicker
            finishDate: { required: { depends: function () { return $('#selReportType').val() !== 'BGR'; } } },
            selReportLevel: { required: { depends: function () { return $('#selReportType').val() === 'BDC'; }}}
        },
            showErrors: function (errorMap, errorList) {
                this.defaultShowErrors();
                hideFieldsErrorsMessages(['selReportType', 'CENTRO_COSTO', 'selCuentaContable', 'startDate', 'finishDate']);
            },
            submitHandler: function (form, event) {
                makeReport();
            }
    });
}

function clearFiltersForm(
    clearReportType = false,
    clearCentroCosto = true,
    clearCuentaContable = true,
    clearStartDate = true,
    clearFinishDate = true,
    clearReportLevel = true
) {
    if (clearReportType) {
        readOnlySelect2('#selReportType', true);
        setSelect2Data('#selReportType', null);
    }
    if (clearCentroCosto) {
        readOnlySelect2('#CENTRO_COSTO', true);
        setSelect2Data('#CENTRO_COSTO', null);
    }
    if (clearCuentaContable) {
        readOnlySelect2('#selCuentaContable', true);
        setSelect2Data('#selCuentaContable', null);
    }
    if (clearStartDate) { $('#startDate').val(''); }
    if (clearFinishDate) { $('#finishDate').val(''); }
    if (clearReportLevel) {
        readOnlySelect2('#selReportLevel', true);
        setSelect2Data('#selReportLevel', null);
    }
}

function watchFilters(selectedReport) {
    switch (selectedReport) {
        case 'HDC': // Histórico de Cuenta
            clearFiltersForm();
            changeFiltersStatus(
                false, // selCentroCostoReadOnly
                false, // selCuentaContableReadOnly
                false, // startDateReadOnly
                false, // finishDateReadOnly
                true,   // selReportLevelReadOnly
                false, // btnGenerateReportDisable
                false, // btnGenerateExcelDisable
            );
            break;
        case 'BDC': // Balance de Comprobación
            clearFiltersForm();
            changeFiltersStatus(
                true,  // selCentroCostoReadOnly
                true,  // selCuentaContableReadOnly
                false, // startDateReadOnly
                false, // finishDateReadOnly
                false,  // selReportLevelReadOnly
                false, // btnGenerateReportDisable
                false, // btnGenerateExcelDisable
            );
            break;
        case 'BGR': // Balance General
            clearFiltersForm();
            changeFiltersStatus(
                true,  // selCentroCostoReadOnly (disable CentroCosto)
                true,  // selCuentaContableReadOnly (disable CuentaContable)
                false, // startDateReadOnly (enable startDate as unique date required)
                true,  // finishDateReadOnly (disable finishDate)
                true,   // selReportLevelReadOnly
                false, // btnGenerateReportDisable
                false, // btnGenerateExcelDisable
            );
            break;
        case 'DMA': // Diario Mayor
            clearFiltersForm();
            changeFiltersStatus(
                false, // selCentroCostoReadOnly (enable CentroCosto)
                false, // selCuentaContableReadOnly (disable CuentaContable for DMA)
                false, // startDateReadOnly (enable startDate for Diario Mayor)
                false, // finishDateReadOnly (enable finishDate for Diario Mayor)
                true,  // selReportLevelReadOnly
                false, // btnGenerateReportDisable
                false, // btnGenerateExcelDisable
            );
            break;
        case 'ER': // Estado de Resultados
            clearFiltersForm();
            changeFiltersStatus(
                true,  // selCentroCostoReadOnly (disable CentroCosto)
                true,  // selCuentaContableReadOnly (enable CuentaContable)
                false, // startDateReadOnly (enable startDate)
                false, // finishDateReadOnly (enable finishDate)
                true,  // selReportLevelReadOnly
                false, // btnGenerateReportDisable
                false, // btnGenerateExcelDisable
            );
            break;
        default:
            clearFiltersForm();
            changeFiltersStatus(
                true,  // selCentroCostoReadOnly
                true,  // selCuentaContableReadOnly
                true,  // startDateReadOnly
                true,  // finishDateReadOnly
                true,  // selReportLevelReadOnly
                true,  // btnGenerateReportDisable
                true,  // btnGenerateExcelDisable
            );
            break;
    }
}

function changeFiltersStatus(
    selCentroCostoReadOnly,
    selCuentaContableReadOnly,
    startDateReadOnly,
    finishDateReadOnly,
    selReportLevelReadOnly,
    btnGenerateReportDisable,
    btnGenerateExcelDisable
) {
    readOnlySelect2('#CENTRO_COSTO', selCentroCostoReadOnly);
    readOnlySelect2('#selCuentaContable', selCuentaContableReadOnly);
    readOnlyInput('#startDate', startDateReadOnly); // Habilitar o deshabilitar campo de fecha de inicio
    readOnlyInput('#finishDate', finishDateReadOnly); // Habilitar o deshabilitar campo de fecha de fin
    readOnlySelect2('#selReportLevel', selReportLevelReadOnly);
    disableInput('#btnGenerateReport', btnGenerateReportDisable);
    disableInput('#btnGenerateExcel', btnGenerateExcelDisable);

    if (!startDateReadOnly) {
        $('#startDate').removeAttr('disabled'); // Asegurarse de que el campo esté habilitado si no es de solo lectura
    } else {
        $('#startDate').attr('disabled', 'disabled'); // Deshabilitar si es de solo lectura
    }

    if (!finishDateReadOnly) {
        $('#finishDate').removeAttr('disabled'); // Asegurarse de que el campo esté habilitado si no es de solo lectura
    } else {
        $('#finishDate').attr('disabled', 'disabled'); // Deshabilitar si es de solo lectura
    }
}


function makeReport() {
    const codCia = $('#codCia').val();
    const startDate = $('#startDate').val();
    const finishDate = $('#finishDate').val();
    const level = $('#selReportLevel').val();

    if ($('#selReportType').val() === 'HDC') { // Histórico de Cuenta
        const cuentaContable = $('#CENTRO_COSTO').val() || '00-000-00'; // Ajusta esto según tus necesidades
        const CTA1 = ACCOUNTS.CTA_1 || 1;
        const CTA2 = ACCOUNTS.CTA_2 || 0;
        const CTA3 = ACCOUNTS.CTA_3 || 0;
        const CTA4 = ACCOUNTS.CTA_4 || 0;
        const CTA5 = ACCOUNTS.CTA_5 || 0;
        const CTA6 = ACCOUNTS.CTA_6 || 0;

        // Formatea las fechas
        const formattedStartDate = formatDate(startDate);
        const formattedFinishDate = formatDate(finishDate);

        const url = `/Reports/HistoricoDecuenta?codCia=${codCia}&cuentaContable=${cuentaContable}&startDate=${formattedStartDate}&finishDate=${formattedFinishDate}&cta1=${CTA1}&cta2=${CTA2}&cta3=${CTA3}&cta4=${CTA4}&cta5=${CTA5}&cta6=${CTA6}`;
        window.open(url, '_blank');
    }

    if ($('#selReportType').val() === 'BDC') {
        const url = `/Reports/BalanceComprobacion?codCia=${codCia}&startDate=${startDate}&finishDate=${finishDate}&level=${level}`;
        window.open(url, '_blank');
    }

    if ($('#selReportType').val() === 'BGR') { // Balance General
        const formattedStartDate = formatDate(startDate); // Formatear solo para Balance General
        const grupoCta = 'ACTIVO';//??? no necesario
        const subGrupo = 'CORRIENTE';//??? no necesario

        const url = `/Reports/BalanceGral?fecha=${formattedStartDate}&codCia=${codCia}&grupoCta=${grupoCta}&subGrupo=${subGrupo}`;
        window.open(url, '_blank');
    }

    if ($('#selReportType').val() === 'DMA') { 
        const cuentaContable = $('#CENTRO_COSTO').val() || '00-000-00'; // Ajusta esto según tus necesidades
        const CTA1 = ACCOUNTS.CTA_1 || 1;
        const CTA2 = ACCOUNTS.CTA_2 || 0;
        const CTA3 = ACCOUNTS.CTA_3 || 0;
        const CTA4 = ACCOUNTS.CTA_4 || 0;
        const CTA5 = ACCOUNTS.CTA_5 || 0;
        const CTA6 = ACCOUNTS.CTA_6 || 0;

        // Formatea las fechas
        const formattedStartDate = formatDate(startDate);
        const formattedFinishDate = formatDate(finishDate);

        const url = `/Reports/DiarioMayor?codCia=${codCia}&cuentaContable=${cuentaContable}&startDate=${formattedStartDate}&finishDate=${formattedFinishDate}&cta1=${CTA1}&cta2=${CTA2}&cta3=${CTA3}&cta4=${CTA4}&cta5=${CTA5}&cta6=${CTA6}`;
        window.open(url, '_blank');
    }

    if ($('#selReportType').val() === 'ER') { // Estado de Resultados
        const formattedStartDate = formatDate(startDate);
        const formattedFinishDate = formatDate(finishDate);

        const url = `/Reports/EstadosResultados?codCia=${codCia}&fechaInicio=${formattedStartDate}&fechaFin=${formattedFinishDate}`;
        window.open(url, '_blank');
    }
}

function makeExcel() {
    const codCia = $('#codCia').val();
    const startDate = $('#startDate').val();
    const finishDate = $('#finishDate').val();
    const level = $('#selReportLevel').val();

    if ($('#selReportType').val() === 'HDC') { // Histórico de Cuenta
        const cuentaContable = $('#CENTRO_COSTO').val() || '00-000-00'; // Ajusta esto según tus necesidades
        const CTA1 = ACCOUNTS.CTA_1 || 1;
        const CTA2 = ACCOUNTS.CTA_2 || 0;
        const CTA3 = ACCOUNTS.CTA_3 || 0;
        const CTA4 = ACCOUNTS.CTA_4 || 0;
        const CTA5 = ACCOUNTS.CTA_5 || 0;
        const CTA6 = ACCOUNTS.CTA_6 || 0;

        // Formatea las fechas
        const formattedStartDate = formatDate(startDate);
        const formattedFinishDate = formatDate(finishDate);

        const url = `/Reports/ExportarHistoricoCuentaExcel?codCia=${codCia}&cuentaContable=${cuentaContable}&startDate=${formattedStartDate}&finishDate=${formattedFinishDate}&cta1=${CTA1}&cta2=${CTA2}&cta3=${CTA3}&cta4=${CTA4}&cta5=${CTA5}&cta6=${CTA6}`;
        window.open(url, '_blank');
    }

    if ($('#selReportType').val() === 'BDC') {
        const url = `/Reports/ExportarBalanceComprobacionExcel?codCia=${codCia}&startDate=${startDate}&finishDate=${finishDate}&level=${level}`;
        window.open(url, '_blank');
    }

    if ($('#selReportType').val() === 'BGR') { // Balance General
        const formattedStartDate = formatDate(startDate); // Formatear solo para Balance General
        const grupoCta = 'ACTIVO';//??? no necesario
        const subGrupo = 'CORRIENTE';//??? no necesario

        const url = `/Reports/ExportarBalanceGralExcel?fecha=${formattedStartDate}&codCia=${codCia}&grupoCta=${grupoCta}&subGrupo=${subGrupo}`;
        window.open(url, '_blank');
    }

    if ($('#selReportType').val() === 'DMA') {
        const cuentaContable = $('#CENTRO_COSTO').val() || '00-000-00'; // Ajusta esto según tus necesidades
        const CTA1 = ACCOUNTS.CTA_1 || 1;
        const CTA2 = ACCOUNTS.CTA_2 || 0;
        const CTA3 = ACCOUNTS.CTA_3 || 0;
        const CTA4 = ACCOUNTS.CTA_4 || 0;
        const CTA5 = ACCOUNTS.CTA_5 || 0;
        const CTA6 = ACCOUNTS.CTA_6 || 0;

        // Formatea las fechas
        const formattedStartDate = formatDate(startDate);
        const formattedFinishDate = formatDate(finishDate);

        const url = `/Reports/DiarioMayor?codCia=${codCia}&cuentaContable=${cuentaContable}&startDate=${formattedStartDate}&finishDate=${formattedFinishDate}&cta1=${CTA1}&cta2=${CTA2}&cta3=${CTA3}&cta4=${CTA4}&cta5=${CTA5}&cta6=${CTA6}`;
        window.open(url, '_blank');
    }

    if ($('#selReportType').val() === 'ER') { // Estado de Resultados
        const formattedStartDate = formatDate(startDate);
        const formattedFinishDate = formatDate(finishDate);

        const url = `/Reports/ExportarEstadoResultadosExcel?codCia=${codCia}&fechaInicio=${formattedStartDate}&fechaFin=${formattedFinishDate}`;
        window.open(url, '_blank');
    }
}

// Función para formatear la fecha en formato 'yyyy-MM-dd'
function formatDate(dateString) {
    const [dia, mes, anio] = dateString.split('/');
    // Crear un nuevo objeto Date (Nota: los meses en JavaScript van de 0 a 11, por lo que restamos 1 al mes)
    const date = new Date(anio, mes - 1, dia);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}


function printFromHTML(html) {
    var printWindow = window.open('', '_blank');
    printWindow.document.open();
    printWindow.document.write(html);
    printWindow.document.close();
    printWindow.print();
}

function printDiv(divIdOrClass) { // .bootbox-body
    var content = $(divIdOrClass).html();
    var printWindow = window.open('', '_blank');
    printWindow.document.open();
    printWindow.document.write('<html><head><title>Print</title></head><body>' + content + '</body></html>');

    setTimeout(function () {
        printWindow.print();
        printWindow.document.close();
    }, 500);
}

function downloadPDF(containerId, fileName) {
    // window.jsPDF = window.jspdf.jsPDF;
    // var pdf = new jsPDF('p', 'pt', 'letter');
    // pdf.addHTML(document.getElementById(containerId), function() {
    //     pdf.save(fileName + '.pdf');
    // });
    var doc = new jsPDF();
    var content = $(`#${containerId}`).html(); // Get HTML content
    doc.fromHTML(content, 15, 15); // Convert HTML to PDF
    doc.save(fileName + '.pdf'); // Save the PDF file
}

function renderPDF(url, canvasContainer) {
    var { pdfjsLib } = globalThis;

    // The workerSrc property shall be specified.
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://mozilla.github.io/pdf.js/build/pdf.worker.mjs';

    function renderPage(page) {

        let viewport = page.getViewport({ scale: .5 })
        const DPI = 72;
        const PRINT_OUTPUT_SCALE = DPI / 72;
        const scale = canvasContainer.clientWidth / viewport.width;
        const canvas = document.createElement('canvas')

        const ctx = canvas.getContext('2d')
        viewport = page.getViewport({ scale })

        canvas.width = Math.floor(viewport.width * PRINT_OUTPUT_SCALE);
        canvas.height = Math.floor(viewport.height * PRINT_OUTPUT_SCALE);
        canvas.style.width = '100%';

        canvas.style.transform = 'scale(1,1)';
        canvas.style.transformOrigin = '0% 0%';

        const canvasWrapper = document.createElement('div');

        canvasWrapper.style.width = '100%';
        canvasWrapper.style.height = '100%';

        canvasWrapper.appendChild(canvas);

        const renderContext = {
            canvasContext: ctx,
            viewport,
        }

        canvasContainer.appendChild(canvasWrapper)

        page.render(renderContext)
    }

    function renderPages(pdfDoc) {
        for (let num = 1; num <= pdfDoc.numPages; num += 1)
            pdfDoc.getPage(num).then(renderPage)
    }

    pdfjsLib.disableWorker = true
    pdfjsLib.getDocument(url).promise.then(renderPages)
}

function makeRender(url, canvasContainer) {
    // If absolute URL from the remote server is provided, configure the CORS
    // header on that server.
    //     var url = `/Reports/PolizaDiario?codCia=${codCia}&tipoDocto=${tipoDocto}&numPoliza=${numPoliza}&periodo=${periodo}`;

    // Loaded via <script> tag, create shortcut to access PDF.js exports.
    var { pdfjsLib } = globalThis;

    // The workerSrc property shall be specified.
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://mozilla.github.io/pdf.js/build/pdf.worker.mjs';
    //     pdfjsLib.GlobalWorkerOptions.workerSrc = 'plugins/mozilla.pdf.js/pdf.worker.mjs';

    // Asynchronous download of PDF
    var loadingTask = pdfjsLib.getDocument(url);
    loadingTask.promise.then(function (pdf) {
        console.log('PDF loaded');

        // Fetch the first page
        var pageNumber = 1;
        pdf.getPage(pageNumber).then(function (page) {
            console.log('Page loaded');

            let viewport = page.getViewport({ scale: .5 })
            const DPI = 72;
            const PRINT_OUTPUT_SCALE = DPI / 72;
            const scale = canvasContainer.clientWidth / viewport.width;
            // const canvas = document.createElement('canvas')

            // var scale = 1.5;
            // var viewport = page.getViewport({scale: scale});
            // var canvas = canvasContainer;
            // var context = canvas.getContext('2d');
            // canvas.height = viewport.height;
            // canvas.width = viewport.width;

            const canvas = document.createElement('canvas');
            const context = canvas.getContext('2d')
            viewport = page.getViewport({ scale })
            canvas.width = Math.floor(viewport.width * PRINT_OUTPUT_SCALE);
            canvas.height = Math.floor(viewport.height * PRINT_OUTPUT_SCALE);
            canvas.style.width = '100%';
            canvas.style.transform = 'scale(1,1)';
            canvas.style.transformOrigin = '0% 0%';

            const canvasWrapper = document.createElement('div');
            canvasWrapper.style.width = '100%';
            canvasWrapper.style.height = '100%';
            canvasWrapper.appendChild(canvas);

            // Render PDF page into canvas context
            var renderContext = {
                canvasContext: context,
                viewport: viewport
            };

            canvasContainer.appendChild(canvasWrapper)

            var renderTask = page.render(renderContext);
            renderTask.promise.then(function () {
                console.log('Page rendered');
            });
        });
    }, function (reason) {
        // PDF loading error
        console.error(reason);
    });
}