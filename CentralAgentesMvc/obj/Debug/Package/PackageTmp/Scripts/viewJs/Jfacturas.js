$(document).ready(function () {
    ConfigSearchControls('dvDesde', 'dvHasta', 'AgenteID', 'ddlAgentes', 'AgenteName');
    ConfigBootstapTable('tblInvoicesFC', app.getFacturas, uri.facturas);
    ConfigBootstapTable('tblEndosos', app.getEndosos, uri.endosos);
});

var app = {
    getFacturas: function (number, size) {
        var agID = $("#AgenteID").val();
        if (agID == undefined) {
            agID = uri.agenteId;
        }

        var params = {
            fechaInicio: $("#FechaDesde").val(),
            fechaFinal: $("#FechaHasta").val(),
            agenteID: agID,
            polizaID: $("#PolizaID").val(),
            clienteID: $("#ClienteID").val(),
            offset: number,
            limit: size
        };

        // Extraigo los datos via Json
        GetDataTable('tblInvoicesFC', uri.facturas, params);
        GetDataTable('tblEndosos', uri.endosos, params);
    },
    getEndosos: function (number, size) {
        var agID = $("#AgenteID").val();
        if (agID == undefined) {
            agID = uri.agenteId;
        }

        var params = {
            fechaInicio: $("#FechaDesde").val(),
            fechaFinal: $("#FechaHasta").val(),
            agenteID: agID,
            polizaID: $("#PolizaID").val(),
            clienteID: $("#ClienteID").val(),
            offset: number,
            limit: size
        };

        // Extraigo los datos via Json
        GetDataTable('tblEndosos', uri.endosos, params);
     }
}

/*---------------------------------------------------------------------*/
/* Formateo de la columna de iconos para Facturas                      */
/*---------------------------------------------------------------------*/
function fieldFormatterFCT(value, row, index) {
    return [
                '<a class="pdf" href="javascript:void(0)" title="Visualizar PDF"><i class="invoice img_pdf"></i></a>',
                '<a class="xml" href="javascript:void(0)" title="Descargar XML"><i class="invoice img_xml"></i></a>',
                '<a class="100' + row.Css100 + '" href="javascript:void(0)" title="Visualizar Cobertura 100"><i class="invoice img_100' + row.Css100 + '"></i></a>',
                '<a class="sob" href="javascript:void(0)" title="Visualizar Seguro Obligatorio"><i class="invoice img_sob"></i></a>',
                ].join('');
};


function fieldFormatterEnd(value, row, index) {
    return [
                '<a class="pdf" href="javascript:void(0)" title="Visualizar PDF"><i class="invoice img_pdf"></i></a>'
                ].join('');
};

$('#NombreAsegurado').typeahead({
    source: function (query, process) {
        return $.get(uri.listaCliente, { query: query }, function (data) {
            return process(data);
        });
    },
    items: 15,
    updater: function (item) {
        $("#ClienteID").val(item.id);
        return item;
    }
});