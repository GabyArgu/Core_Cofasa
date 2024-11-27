'use strict';

var ciasGridButtons = '';
var tableCias;
var dialogCias = null;

const listOfAccountsCia = [
    {ac1: 'CTA_1RESUL_ACT', ac2: 'CTA_2RESUL_ACT', ac3: 'CTA_3RESUL_ACT', ac4: 'CTA_4RESUL_ACT', ac5: 'CTA_5RESUL_ACT', ac6: 'CTA_6RESUL_ACT', name: 'NOM_RESUL_ACT'},
    {ac1: 'CTA_1RESUL_ANT', ac2: 'CTA_2RESUL_ANT', ac3: 'CTA_3RESUL_ANT', ac4: 'CTA_4RESUL_ANT', ac5: 'CTA_5RESUL_ANT', ac6: 'CTA_6RESUL_ANT', name: 'NOM_RESUL_ANT'},
    {ac1: 'CTA_1RESUL_ANT', ac2: 'CTA_2RESUL_ANT', ac3: 'CTA_3RESUL_ANT', ac4: 'CTA_4RESUL_ANT', ac5: 'CTA_5RESUL_ANT', ac6: 'CTA_6RESUL_ANT', name: 'NOM_RESUL_ANT'},
    {ac1: 'CTA_1PER_GAN', ac2: 'CTA_2PER_GAN', ac3: 'CTA_3PER_GAN', ac4: 'CTA_4PER_GAN', ac5: 'CTA_5PER_GAN', ac6: 'CTA_6PER_GAN', name: 'NOM_PER_GAN'},
    {ac1: 'INGRESO_CTA1', ac2: 'INGRESO_CTA2', ac3: 'INGRESO_CTA3', ac4: 'INGRESO_CTA4', ac5: 'INGRESO_CTA5', ac6: 'INGRESO_CTA6', name: 'NOM_INGRESO'},
    {ac1: 'INGRESO_CTA1', ac2: 'INGRESO_CTA2', ac3: 'INGRESO_CTA3', ac4: 'INGRESO_CTA4', ac5: 'INGRESO_CTA5', ac6: 'INGRESO_CTA6', name: 'NOM_INGRESO'},
    {ac1: 'GASTO_CTA1', ac2: 'GASTO_CTA2', ac3: 'GASTO_CTA3', ac4: 'GASTO_CTA4', ac5: 'GASTO_CTA5', ac6: 'GASTO_CTA6', name: 'NOM_GASTO'},
    {ac1: 'GASTO_CTA1', ac2: 'GASTO_CTA2', ac3: 'GASTO_CTA3', ac4: 'GASTO_CTA4', ac5: 'GASTO_CTA5', ac6: 'GASTO_CTA6', name: 'NOM_GASTO'},
    {ac1: 'COSTO_CTA1', ac2: 'COSTO_CTA2', ac3: 'COSTO_CTA3', ac4: 'COSTO_CTA4', ac5: 'COSTO_CTA5', ac6: 'COSTO_CTA6', name: 'NOM_COSTO'},
    {ac1: 'COSTO_CTA1', ac2: 'COSTO_CTA2', ac3: 'COSTO_CTA3', ac4: 'COSTO_CTA4', ac5: 'COSTO_CTA5', ac6: 'COSTO_CTA6', name: 'NOM_COSTO'}
];

$(document).ready(function () {
    prepareCiaButtons();
    initGrid();
});

function prepareCiaButtons() {
    const bodyButtons = $('#ciasGridButtons').val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    $('#addCiaBtn').click(function(){ showCiasForm(); });
    ciasGridButtons = '<center>' + tags.html() + '<center>';
}

function bindCiasButtons() {
    $('#ciasDt tbody tr td button').unbind('click').on('click', function (event) {
       if (event.preventDefault) { event.preventDefault(); }
       if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

        const obj = JSON.parse(Base64.decode($(this).parent().attr('data-row')));
        const action = $(this).attr('data-action');

        if (action === 'edit') { showCiasForm(obj.cod); }
       // if (action === 'delete') { deleteRecord(obj.id); }
       if (action === 'copy') { showCiasForm(obj.cod, 1); }
    });
}

function initGrid() {
    tableCias = $('#ciasDt')
        .on('draw.dt', function () {
            drawRowNumbers('#ciasDt', tableCias);
            setTimeout(function(){bindCiasButtons();},500);
        })
        .DataTable({
            'language': { url: '/plugins/jquery.dataTables/1.13.7/i18n/es-MX.json' },
            'ajax': {
                'url': '/Cias/GetCias',
                'type': 'GET',
                'datatype': 'JSON',
            },
            'columns': [
                { 'data': 'cod' },
                { 'data': 'cod' },
                { 'data': 'razonSocial' },
                { 'data': 'nomComercial' },
                { 'data': 'direcEmpresa' },
                {
                    sortable: false, searchable: false,
                    render: function (data, type, row) {
                        return ciasGridButtons.replace('{data}',Base64.encode($.toJSON(row)));
                    }
                }
            ]
        });

    $('#ciasDt')
        .removeClass('display')
        .addClass('table table-bordered table-hover dataTable');
}

function startCiasValidation() {
    $('#ciasForm').validate({ // number: true
        rules: {
            // COD_CIA: { required: true, minlength: 3, maxlength: 3 },
            RAZON_SOCIAL: { required: true, minlength: 3, maxlength: 60 },
            NOM_COMERCIAL: { required: true, minlength: 3, maxlength: 60 },
            DIREC_EMPRESA: { required: true, minlength: 3, maxlength: 100 },
            
            // NOM_CORTO: { required: false, minlength: 3, maxlength: 100 }, // TODO: NO ESTA EN EL MODELO
            TELEF_EMPRESA: { required: false, minlength: 3, maxlength: 30 },
            NIT_EMPRESA: { required: false, minlength: 3, maxlength: 25 },
            NUMERO_PATRONAL: { required: false, minlength: 3, maxlength: 15 },
            // COD_MONEDA: { required: true, minlength: 3, maxlength: 100 }, // TODO: TABLA FORANEA
            PERIODO: { required: false, digits: true },
            MES_CIERRE: { required: false, digits: true },
            MES_PROCESO: { required: false, digits: true },
            // INICIALES: { required: true, minlength: 3, maxlength: 100 }, // TODO: NO ESTA EN EL MODELO
            
            IVA_PORC: { required: false, number: true }, // float
            ND_IVA: { required: false, minlength: 3, maxlength: 6 },
            FD_IVA: { required: false }, // Fecha
            TASA_CAM: { required: false, number: true },
            ISR_PORC: { required: false, number: true },
            ND_ISR: { required: false, minlength: 3, maxlength: 6 },
            FD_ISR: { required: false }, // Fecha
            VAL_MIN_DEPRECIAR: { required: false, number: true }, // float

            CTA_1RESUL_ACT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_2RESUL_ACT').val()!=='' || $('#CTA_3RESUL_ACT').val()!==''
                            || $('#CTA_4RESUL_ACT').val()!=='' || $('#CTA_5RESUL_ACT').val()!==''
                            || $('#CTA_6RESUL_ACT').val()!=='';
                    }
                }
            },
            CTA_2RESUL_ACT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ACT').val()!=='' || $('#CTA_3RESUL_ACT').val()!==''
                            || $('#CTA_4RESUL_ACT').val()!=='' || $('#CTA_5RESUL_ACT').val()!==''
                            || $('#CTA_6RESUL_ACT').val()!=='';
                    }
                }
            },
            CTA_3RESUL_ACT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ACT').val()!=='' || $('#CTA_2RESUL_ACT').val()!==''
                            || $('#CTA_4RESUL_ACT').val()!=='' || $('#CTA_5RESUL_ACT').val()!==''
                            || $('#CTA_6RESUL_ACT').val()!=='';
                    }
                },
            },
            CTA_4RESUL_ACT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ACT').val()!=='' || $('#CTA_2RESUL_ACT').val()!==''
                            || $('#CTA_3RESUL_ACT').val()!=='' || $('#CTA_5RESUL_ACT').val()!==''
                            || $('#CTA_6RESUL_ACT').val()!=='';
                    }
                },
            },
            CTA_5RESUL_ACT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ACT').val()!=='' || $('#CTA_2RESUL_ACT').val()!==''
                            || $('#CTA_3RESUL_ACT').val()!=='' || $('#CTA_4RESUL_ACT').val()!==''
                            || $('#CTA_6RESUL_ACT').val()!=='';
                    }
                },
            },
            CTA_6RESUL_ACT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ACT').val()!=='' || $('#CTA_2RESUL_ACT').val()!==''
                            || $('#CTA_3RESUL_ACT').val()!=='' || $('#CTA_4RESUL_ACT').val()!==''
                            || $('#CTA_5RESUL_ACT').val()!=='';
                    }
                },
            },
            NOM_RESUL_ACT: { required: false }, // TODO: VIENE DE OTRA TABLA

            CTA_1RESUL_ANT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_2RESUL_ANT').val()!=='' || $('#CTA_3RESUL_ANT').val()!==''
                            || $('#CTA_4RESUL_ANT').val()!=='' || $('#CTA_5RESUL_ANT').val()!==''
                            || $('#CTA_6RESUL_ANT').val()!=='';
                    }
                }
            },
            CTA_2RESUL_ANT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ANT').val()!=='' || $('#CTA_3RESUL_ANT').val()!==''
                            || $('#CTA_4RESUL_ANT').val()!=='' || $('#CTA_5RESUL_ANT').val()!==''
                            || $('#CTA_6RESUL_ANT').val()!=='';
                    }
                }
            },
            CTA_3RESUL_ANT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ANT').val()!=='' || $('#CTA_2RESUL_ANT').val()!==''
                            || $('#CTA_4RESUL_ANT').val()!=='' || $('#CTA_5RESUL_ANT').val()!==''
                            || $('#CTA_6RESUL_ANT').val()!=='';
                    }
                }
            },
            CTA_4RESUL_ANT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ANT').val()!=='' || $('#CTA_2RESUL_ANT').val()!==''
                            || $('#CTA_3RESUL_ANT').val()!=='' || $('#CTA_5RESUL_ANT').val()!==''
                            || $('#CTA_6RESUL_ANT').val()!=='';
                    }
                }
            },
            CTA_5RESUL_ANT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ANT').val()!=='' || $('#CTA_2RESUL_ANT').val()!==''
                            || $('#CTA_3RESUL_ANT').val()!=='' || $('#CTA_4RESUL_ANT').val()!==''
                            || $('#CTA_6RESUL_ANT').val()!=='';
                    }
                }
            },
            CTA_6RESUL_ANT: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1RESUL_ANT').val()!=='' || $('#CTA_2RESUL_ANT').val()!==''
                            || $('#CTA_3RESUL_ANT').val()!=='' || $('#CTA_4RESUL_ANT').val()!==''
                            || $('#CTA_5RESUL_ANT').val()!=='';
                    }
                }
            },
            NOM_RESUL_ANT: { required: false }, // TODO: VIENE DE OTRA TABLA

            CTA_1PER_GAN: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_2PER_GAN').val()!=='' || $('#CTA_3PER_GAN').val()!==''
                            || $('#CTA_4PER_GAN').val()!=='' || $('#CTA_5PER_GAN').val()!==''
                            || $('#CTA_6PER_GAN').val()!=='';
                    }
                }
            },
            CTA_2PER_GAN: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1PER_GAN').val()!=='' || $('#CTA_3PER_GAN').val()!==''
                            || $('#CTA_4PER_GAN').val()!=='' || $('#CTA_5PER_GAN').val()!==''
                            || $('#CTA_6PER_GAN').val()!=='';
                    }
                }
            },
            CTA_3PER_GAN: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1PER_GAN').val()!=='' || $('#CTA_2PER_GAN').val()!==''
                            || $('#CTA_4PER_GAN').val()!=='' || $('#CTA_5PER_GAN').val()!==''
                            || $('#CTA_6PER_GAN').val()!=='';
                    }
                }
            },
            CTA_4PER_GAN: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1PER_GAN').val()!=='' || $('#CTA_2PER_GAN').val()!==''
                            || $('#CTA_3PER_GAN').val()!=='' || $('#CTA_5PER_GAN').val()!==''
                            || $('#CTA_6PER_GAN').val()!=='';
                    }
                }
            },
            CTA_5PER_GAN: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1PER_GAN').val()!=='' || $('#CTA_2PER_GAN').val()!==''
                            || $('#CTA_3PER_GAN').val()!=='' || $('#CTA_4PER_GAN').val()!==''
                            || $('#CTA_6PER_GAN').val()!=='';
                    }
                }
            },
            CTA_6PER_GAN: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#CTA_1PER_GAN').val()!=='' || $('#CTA_2PER_GAN').val()!==''
                            || $('#CTA_3PER_GAN').val()!=='' || $('#CTA_4PER_GAN').val()!==''
                            || $('#CTA_5PER_GAN').val()!=='';
                    }
                }
            },
            NOM_PER_GAN: { required: false }, // TODO: VIENE DE OTRA TABLA

            INGRESO_CTA1: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#INGRESO_CTA2').val()!=='' || $('#INGRESO_CTA3').val()!==''
                            || $('#INGRESO_CTA4').val()!=='' || $('#INGRESO_CTA5').val()!==''
                            || $('#INGRESO_CTA6').val()!=='';
                    }
                }
            },
            INGRESO_CTA2: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#INGRESO_CTA1').val()!=='' || $('#INGRESO_CTA3').val()!==''
                            || $('#INGRESO_CTA4').val()!=='' || $('#INGRESO_CTA5').val()!==''
                            || $('#INGRESO_CTA6').val()!=='';
                    }
                }
            },
            INGRESO_CTA3: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#INGRESO_CTA1').val()!=='' || $('#INGRESO_CTA2').val()!==''
                            || $('#INGRESO_CTA4').val()!=='' || $('#INGRESO_CTA5').val()!==''
                            || $('#INGRESO_CTA6').val()!=='';
                    }
                }
            },
            INGRESO_CTA4: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#INGRESO_CTA1').val()!=='' || $('#INGRESO_CTA2').val()!==''
                            || $('#INGRESO_CTA3').val()!=='' || $('#INGRESO_CTA5').val()!==''
                            || $('#INGRESO_CTA6').val()!=='';
                    }
                }
            },
            INGRESO_CTA5: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#INGRESO_CTA1').val()!=='' || $('#INGRESO_CTA2').val()!==''
                            || $('#INGRESO_CTA3').val()!=='' || $('#INGRESO_CTA4').val()!==''
                            || $('#INGRESO_CTA6').val()!=='';
                    }
                }
            },
            INGRESO_CTA6: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#INGRESO_CTA1').val()!=='' || $('#INGRESO_CTA2').val()!==''
                            || $('#INGRESO_CTA3').val()!=='' || $('#INGRESO_CTA4').val()!==''
                            || $('#INGRESO_CTA5').val()!=='';
                    }
                }
            },
            NOM_INGRESO: { required: false }, // TODO: VIENE DE OTRA TABLA

            GASTO_CTA1: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#GASTO_CTA2').val()!=='' || $('#GASTO_CTA3').val()!==''
                            || $('#GASTO_CTA4').val()!=='' || $('#GASTO_CTA5').val()!==''
                            || $('#GASTO_CTA6').val()!=='';
                    }
                }
            },
            GASTO_CTA2: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#GASTO_CTA1').val()!=='' || $('#GASTO_CTA3').val()!==''
                            || $('#GASTO_CTA4').val()!=='' || $('#GASTO_CTA5').val()!==''
                            || $('#GASTO_CTA6').val()!=='';
                    }
                }
            },
            GASTO_CTA3: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#GASTO_CTA1').val()!=='' || $('#GASTO_CTA2').val()!==''
                            || $('#GASTO_CTA4').val()!=='' || $('#GASTO_CTA5').val()!==''
                            || $('#GASTO_CTA6').val()!=='';
                    }
                }
            },
            GASTO_CTA4: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#GASTO_CTA1').val()!=='' || $('#GASTO_CTA2').val()!==''
                            || $('#GASTO_CTA3').val()!=='' || $('#GASTO_CTA5').val()!==''
                            || $('#GASTO_CTA6').val()!=='';
                    }
                }
            },
            GASTO_CTA5: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#GASTO_CTA1').val()!=='' || $('#GASTO_CTA2').val()!==''
                            || $('#GASTO_CTA3').val()!=='' || $('#GASTO_CTA4').val()!==''
                            || $('#GASTO_CTA6').val()!=='';
                    }
                }
            },
            GASTO_CTA6: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#GASTO_CTA1').val()!=='' || $('#GASTO_CTA2').val()!==''
                            || $('#GASTO_CTA3').val()!=='' || $('#GASTO_CTA4').val()!==''
                            || $('#GASTO_CTA5').val()!=='';
                    }
                }
            },
            NOM_GASTO: { required: false }, // TODO: VIENE DE OTRA TABLA

            COSTO_CTA1: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#COSTO_CTA2').val()!=='' || $('#COSTO_CTA3').val()!==''
                            || $('#COSTO_CTA4').val()!=='' || $('#COSTO_CTA5').val()!==''
                            || $('#COSTO_CTA6').val()!=='';
                    }
                }
            },
            COSTO_CTA2: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#COSTO_CTA1').val()!=='' || $('#COSTO_CTA3').val()!==''
                            || $('#COSTO_CTA4').val()!=='' || $('#COSTO_CTA5').val()!==''
                            || $('#COSTO_CTA6').val()!=='';
                    }
                }
            },
            COSTO_CTA3: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#COSTO_CTA1').val()!=='' || $('#COSTO_CTA2').val()!==''
                            || $('#COSTO_CTA4').val()!=='' || $('#COSTO_CTA5').val()!==''
                            || $('#COSTO_CTA6').val()!=='';
                    }
                }
            },
            COSTO_CTA4: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#COSTO_CTA1').val()!=='' || $('#COSTO_CTA2').val()!==''
                            || $('#COSTO_CTA3').val()!=='' || $('#COSTO_CTA5').val()!==''
                            || $('#COSTO_CTA6').val()!=='';
                    }
                }
            },
            COSTO_CTA5: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#COSTO_CTA1').val()!=='' || $('#COSTO_CTA2').val()!==''
                            || $('#COSTO_CTA3').val()!=='' || $('#COSTO_CTA4').val()!==''
                            || $('#COSTO_CTA6').val()!=='';
                    }
                }
            },
            COSTO_CTA6: {
                digits: true,
                required: {
                    depends: function() {
                        return $('#COSTO_CTA1').val()!=='' || $('#COSTO_CTA2').val()!==''
                            || $('#COSTO_CTA3').val()!=='' || $('#COSTO_CTA4').val()!==''
                            || $('#COSTO_CTA5').val()!=='';
                    }
                }
            },
            NOM_COSTO: { required: false, minlength: 3, maxlength: 100 }, // TODO: VIENE DE OTRA TABLA

            FEC_ULT_CIE: { required: false },
            DUP_DET_PARTIDAD: { required: false },
            APLICA_POLIZA_IMPUESTO: { required: false },
            CONTA_X_CLIENTE: { required: false },
        },
        messages:{
            // COD_CIA: 'Este campo es obligatorio',
            RAZON_SOCIAL: 'Este campo es obligatorio',
            NOM_COMERCIAL: 'Este campo es obligatorio',
            DIREC_EMPRESA: 'Este campo es obligatorio',
        },
        showErrors: function(errorMap, errorList) {
            this.defaultShowErrors();
            $('#CTA_1RESUL_ACT-error, #CTA_2RESUL_ACT-error, #CTA_3RESUL_ACT-error, #CTA_4RESUL_ACT-error,' +
                '#CTA_5RESUL_ACT-error, #CTA_6RESUL_ACT-error, #CTA_1RESUL_ANT-error, #CTA_2RESUL_ANT-error,' +
                '#CTA_3RESUL_ANT-error, #CTA_4RESUL_ANT-error, #CTA_5RESUL_ANT-error, #CTA_6RESUL_ANT-error,' +
                '#CTA_1PER_GAN-error, #CTA_2PER_GAN-error, #CTA_3PER_GAN-error, #CTA_4PER_GAN-error,' +
                '#CTA_5PER_GAN-error, #CTA_6PER_GAN-error, #INGRESO_CTA1-error, #INGRESO_CTA2-error,' +
                '#INGRESO_CTA3-error, #INGRESO_CTA4-error, #INGRESO_CTA5-error, #INGRESO_CTA6-error, #GASTO_CTA1-error,' +
                '#GASTO_CTA2-error, #GASTO_CTA3-error, #GASTO_CTA4-error, #GASTO_CTA5-error, #GASTO_CTA6-error,' +
                '#COSTO_CTA1-error, #COSTO_CTA2-error, #COSTO_CTA3-error, #COSTO_CTA4-error, #COSTO_CTA5-error,' +
                '#COSTO_CTA6-error, #TASA_CAM-error, #VAL_MIN_DEPRECIAR-error, #IVA_PORC-error, #ND_IVA-error,' +
                '#ISR_PORC-error, #ND_ISR-error').hide();
        },
        submitHandler: function (form, event) {
            isLoading('#btnSaveCia', true);
            if ($('#IS_COPYING').val()!=='') { copyCia(); }
            else { saveCia(); }
        }
    });
}

function showCiasForm(id, makeCopy) {
    const isCopying = !(typeof (makeCopy) === 'undefined');
    const isEditing = !(typeof (id) === 'undefined' || id === 0) && !isCopying;
    var title = '';

    if (isEditing) {
        title = 'Editar compañía';
    } else if (isCopying) {
        title = 'Copiar compañía';
    } else {
        title = 'Agregar compañía';
    }

    dialogCias = bootbox.dialog({
        title: title,
        message: $('#ciasFormBody').val(),
        className: 'modalLarge',
        onEscape: true
    });

    startCiasValidation();
    initAsyncSelect2('COD_MONEDA', '/Currency/GetCurrenciesToS2','', false);

    if (isEditing) {
        $('#COD_CIA').val(id);
        $('#btnSaveCiaText').text('Editar');
        loadOneCia(id);
    }
    if (isCopying) {
        $('#IS_COPYING').val(id);
        $('#btnSaveCiaText').text('Copiar');
        loadOneCia(id);
    }

    $('#FD_IVA').datetimepicker({format: 'DD/MM/yyyy', locale: 'es'});
    $('#FD_ISR').datetimepicker({format: 'DD/MM/yyyy', locale: 'es'});
    $('#FEC_ULT_CIE_container').datetimepicker({format: 'DD/MM/yyyy', locale: 'es', allowInputToggle: true});

    // watchAccountNumbers();
    watchListOfAccountNumbers(listOfAccountsCia);
}

function saveCia(){
    const formData = $('#ciasForm').serialize();

    $.ajax({
        url: '/Cias/SaveCia',
        type:'POST',
        data: formData,
        success:function(data){
            showToast(data.success, data.message);
            isLoading('#btnSaveCia', false);

            if(data.success){
                dialogCias.modal('hide');
                tableCias.ajax.reload();
            }
        },
        error: function (error) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading('#btnSaveCia', false);
        }
    });
}

function copyCia(){
    const formData = $('#ciasForm').serialize();

    $.ajax({
        url: '/Cias/CopyCia',
        type:'POST',
        data: formData,
        success:function(data){
            showToast(data.success, data.message);
            isLoading('#btnSaveCia', false);

            if(data.success){
                dialogCias.modal('hide');
                tableCias.ajax.reload();
            }
        },
        error: function (error) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading('#btnSaveCia', false);
        }
    });
}

function loadOneCia(id) {
    $.ajax({
        url: '/Cias/GetCiaById?ciaCod=' + id,
        type:'GET',
        success:function(data){
            if(data.success){
                setDataToCiaForm(data.data);
            }
        },
        error: function (error) {
            console.log(error);
            showToast(false, 'Ocurrió un error al obtener los detalles de la empresa');
        }
    });
}

function setDataToCiaForm(data) {
    $('#RAZON_SOCIAL').val(data.razonSocial);
    $('#NOM_COMERCIAL').val(data.nomComercial);
    $('#DIREC_EMPRESA').val(data.direcEmpresa);

    // $('#NOM_CORTO').val(data.); // TODO: No existe en la tabla cias.
    $('#TELEF_EMPRESA').val(data.telefEmpresa);
    $('#NIT_EMPRESA').val(data.nitEmpresa);
    $('#NUMERO_PATRONAL').val(data.numeroPatronal);
    $('#PERIODO').val(data.periodo);
    $('#MES_CIERRE').val(data.mesCierre);
    $('#MES_PROCESO').val(data.mesProceso);
    // $('#INICIALES').val(data.mesProceso); // TODO: No existe en la tabla cias.

    $('#IVA_PORC').val(data.ivaPorc);
    $('#ND_IVA').val(data.ndIva);
    // $('#FD_IVA').val(data.fdIva); // FECHA
    $('#TASA_CAM').val(data.tasaCam);
    $('#ISR_PORC').val(data.isrPorc);
    $('#ND_ISR').val(data.ndIsr);
    // $('#FD_ISR').val(data.fdIsr); // FECHA
    $('#VAL_MIN_DEPRECIAR').val(data.valMinDepreciar);

    $('#CTA_1RESUL_ACT').val(data.cta1ResulAct);
    $('#CTA_2RESUL_ACT').val(data.cta2ResulAct);
    $('#CTA_3RESUL_ACT').val(data.cta3ResulAct);
    $('#CTA_4RESUL_ACT').val(data.cta4ResulAct);
    $('#CTA_5RESUL_ACT').val(data.cta5ResulAct);
    $('#CTA_6RESUL_ACT').val(data.cta6ResulAct);
    // $('#NOM_RESUL_ACT').val(data.);
    if (needAccountName(data.cta1ResulAct, data.cta2ResulAct, data.cta3ResulAct, data.cta4ResulAct, data.cta5ResulAct, data.cta6ResulAct)) {
        makeAccountNameReq(
            '#CTA_1RESUL_ACT',
            '#CTA_2RESUL_ACT',
            '#CTA_3RESUL_ACT',
            '#CTA_4RESUL_ACT',
            '#CTA_5RESUL_ACT',
            '#CTA_6RESUL_ACT',
            '#NOM_RESUL_ACT'
        );
    }

    $('#CTA_1RESUL_ANT').val(data.cta1ResulAnt);
    $('#CTA_2RESUL_ANT').val(data.cta2ResulAnt);
    $('#CTA_3RESUL_ANT').val(data.cta3ResulAnt);
    $('#CTA_4RESUL_ANT').val(data.cta4ResulAnt);
    $('#CTA_5RESUL_ANT').val(data.cta5ResulAnt);
    $('#CTA_6RESUL_ANT').val(data.cta6ResulAnt);
    // $('#NOM_RESUL_ANT').val(data.);
    if (needAccountName(data.cta1ResulAnt, data.cta2ResulAnt, data.cta3ResulAnt, data.cta4ResulAnt, data.cta5ResulAnt, data.cta6ResulAnt)) {
        makeAccountNameReq(
            '#CTA_1RESUL_ANT',
            '#CTA_2RESUL_ANT',
            '#CTA_3RESUL_ANT',
            '#CTA_4RESUL_ANT',
            '#CTA_5RESUL_ANT',
            '#CTA_6RESUL_ANT',
            '#NOM_RESUL_ANT'
        );
    }

    $('#CTA_1PER_GAN').val(data.cta1PerGan);
    $('#CTA_2PER_GAN').val(data.cta2PerGan);
    $('#CTA_3PER_GAN').val(data.cta3PerGan);
    $('#CTA_4PER_GAN').val(data.cta4PerGan);
    $('#CTA_5PER_GAN').val(data.cta5PerGan);
    $('#CTA_6PER_GAN').val(data.cta6PerGan);
    // $('#NOM_PER_GAN').val(data.);
    if (needAccountName(data.cta1PerGan, data.cta2PerGan, data.cta3PerGan, data.cta4PerGan, data.cta5PerGan, data.cta6PerGan)) {
        makeAccountNameReq(
            '#CTA_1PER_GAN',
            '#CTA_2PER_GAN',
            '#CTA_3PER_GAN',
            '#CTA_4PER_GAN',
            '#CTA_5PER_GAN',
            '#CTA_6PER_GAN',
            '#NOM_PER_GAN'
        );
    }

    $('#INGRESO_CTA1').val(data.ingresoCta1);
    $('#INGRESO_CTA2').val(data.ingresoCta2);
    $('#INGRESO_CTA3').val(data.ingresoCta3);
    $('#INGRESO_CTA4').val(data.ingresoCta4);
    $('#INGRESO_CTA5').val(data.ingresoCta5);
    $('#INGRESO_CTA6').val(data.ingresoCta6);
    // $('#NOM_INGRESO').val(data.);
    if (needAccountName(data.ingresoCta1, data.ingresoCta2, data.ingresoCta3, data.ingresoCta4, data.ingresoCta5, data.ingresoCta6)) {
        makeAccountNameReq(
            '#INGRESO_CTA1',
            '#INGRESO_CTA2',
            '#INGRESO_CTA3',
            '#INGRESO_CTA4',
            '#INGRESO_CTA5',
            '#INGRESO_CTA6',
            '#NOM_INGRESO'
        );
    }

    $('#GASTO_CTA1').val(data.gastoCta1);
    $('#GASTO_CTA2').val(data.gastoCta2);
    $('#GASTO_CTA3').val(data.gastoCta3);
    $('#GASTO_CTA4').val(data.gastoCta4);
    $('#GASTO_CTA5').val(data.gastoCta5);
    $('#GASTO_CTA6').val(data.gastoCta6);
    // $('#NOM_GASTO').val(data.);
    if (needAccountName(data.gastoCta1, data.gastoCta2, data.gastoCta3, data.gastoCta4, data.gastoCta5, data.gastoCta6)) {
        makeAccountNameReq(
            '#GASTO_CTA1',
            '#GASTO_CTA2',
            '#GASTO_CTA3',
            '#GASTO_CTA4',
            '#GASTO_CTA5',
            '#GASTO_CTA6',
            '#NOM_GASTO'
        );
    }

    $('#COSTO_CTA1').val(data.costoCta1);
    $('#COSTO_CTA2').val(data.costoCta2);
    $('#COSTO_CTA3').val(data.costoCta3);
    $('#COSTO_CTA4').val(data.costoCta4);
    $('#COSTO_CTA5').val(data.costoCta5);
    $('#COSTO_CTA6').val(data.costoCta6);
    // $('#NOM_COSTO').val(data.);
    if (needAccountName(data.costoCta1, data.costoCta2, data.costoCta3, data.costoCta4, data.costoCta5, data.costoCta6)) {
        makeAccountNameReq(
            '#COSTO_CTA1',
            '#COSTO_CTA2',
            '#COSTO_CTA3',
            '#COSTO_CTA4',
            '#COSTO_CTA5',
            '#COSTO_CTA6',
            '#NOM_COSTO'
        );
    }
    
    // CHECKBOX's
    if (data.dupDetPartidad === '1') { $('#DUP_DET_PARTIDAD').prop('checked', true) }
    // $('#APLICA_POLIZA_IMPUESTO').val(data.); // TODO: No esta en la tabla Cias.
    // $('#CONTA_X_CLIENTE').val(data.); // TODO: No esta en la tabla Cias.

    // NO ESTAN
    // $('#FECH_ULT').val(data.fechUlt);
    // $('#TASA_IVA').val(data.tasaIva);
    // $('#MESES_CHQ').val(data.mesesChq);
    // $('#PRB_PORC').val(data.prbPorc);

    // Objeto de select2
    $('#COD_MONEDA').select2('data', data.monedaSelect2).change();

    // Calendarios
    if (data.fdIva!=null) $('#FD_IVA').val(moment(data.fdIva).format('DD/MM/yyyy')); // FECHA
    if (data.fdIsr!=null) $('#FD_ISR').val(moment(data.fdIsr).format('DD/MM/yyyy')); // FECHA
    if (data.fecUltCie!=null) $('#FEC_ULT_CIE').val(moment(data.fecUltCie).format('DD/MM/yyyy')); // FECHA
}