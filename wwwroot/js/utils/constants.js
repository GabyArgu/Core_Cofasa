const DOCUMENT_TYPES = [
    { id: 'FA', text: 'Facturación' },
    { id: 'CC', text: 'Cuentas por Cobrar' },
    { id: 'CP', text: 'Cuentas por Pagar' },
    { id: 'CH', text: 'Caja Chica' },
    { id: 'CB', text: 'Control Bancario' },
    { id: 'CO', text: 'Compras' },
    { id: 'CG', text: 'Contabilidad General' },
    { id: 'CI', text: 'Control de Inventarios' },
    { id: 'CN', text: 'Control de Nómina' },
];

const CONSTANTS = {
    defaults: {
        currency: {
            code: 'USD',
            symbol: '$',
        },
        date: {
            formats: {
                dateTime: 'DD/MM/YYYY HH:mm A',
                date: 'DD/MM/YYYY',
                year: 'YYYY',
                month: 'MM',
            }
        },
        select: {
            estadoPoliza: [
                { id: 'G', text: 'Grabada' },
                { id: 'R', text: 'Revisada' }
            ],
            reportType: [
                { id: 'HDC', text: 'Histórico de cuenta' },
                { id: 'BDC', text: 'Balance de comprobación' },
                { id: 'BGR', text: 'Balance General' },
                /*{ id: 'DMA', text: 'Diario Mayor Auxiliar' },*/
                { id: 'ER', text: 'Estado de Resultados' } 
            ],
            reportLevel: [
                { id: '1', text: 'Nivel 1' },
                { id: '2', text: 'Nivel 2' },
                { id: '3', text: 'Nivel 3' },
                { id: '4', text: 'Nivel 4' },
                { id: '5', text: 'Nivel 5' },
                { id: '6', text: 'Nivel 6' }
            ],
            documentType: DOCUMENT_TYPES
        },
        documentTypes: DOCUMENT_TYPES,
    }
}