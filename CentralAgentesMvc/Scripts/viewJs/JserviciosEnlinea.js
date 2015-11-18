var poliza = 0;
var serviciosEnLinea = {
    DataTablesConfig: {
        "autoWidth": false,
        language: {
            lengthMenu: "_MENU_ Registros por página",
            zeroRecords: "No se encontró información",
            info: "Mostrando  página _PAGE_ de _PAGES_ (_MAX_ Registros encontrados)",
            infoEmpty: "No se encontró información",
            infoFiltered: "(filtered from _MAX_ total records)",
            search: "Buscar:",
            paginate: {
                first: "<<",
                last: ">>",
                next: ">",
                previous: "<"
            }
        }
    },
    fnConsultaSiniestros: function () {
        var SiniestrosParams = {
            polizaID: $("#PolizaID").val(),
            solicitudID: 0,
            offset: 1,
            limit: 1000
        }
        $.ajax({
            url: actions.ShowSiniestroAction,
            data: SiniestrosParams
        }).success(function (data) {
            var datas = data;
            $("#tblContainerSiniestros").html(data);

        }).then(function () {
            var conFSiniestro = {
                "autoWidth": false,
                language: {
                    lengthMenu: "_MENU_ Registros por página",
                    zeroRecords: "No se han registrado siniestros",
                    info: "Mostrando  página _PAGE_ de _PAGES_ (_MAX_ Registros encontrados)",
                    infoEmpty: "No se han registrado siniestros",
                    infoFiltered: "(filtered from _MAX_ total records)",
                    search: "Buscar:",
                    paginate: {
                        first: "<<",
                        last: ">>",
                        next: ">",
                        previous: "<"
                    }
                }
            }
            var table = $("#tblSiniestrosSL").DataTable(conFSiniestro);
            table.columns.adjust().draw();
            //$("#tblSiniestrosSL").find("th").width(10);

        });
    },
    GetDataInvoices: function (number, size) {
        poliza = $("#PolizaID").val();
        var params = {
            polizaID: $("#PolizaID").val(),
            solicitudID: $("#SolicitudID").val(),
            offset: number,
            limit: size
        };
        $.ajax({
            url: actions.GetDataServiceAction,
            data: params
        }).success(function (data) {
            var datas = data;
            $("#tblContainerFacturas").html(data);

        }).then(function () {

            serviciosEnLinea.DataTablesConfig.language.infoEmpty = "No hay facturas registradas";
            serviciosEnLinea.DataTablesConfig.language.zeroRecords = "No hay facturas registradas";

            var configs = serviciosEnLinea.DataTablesConfig;
            var table = $("#tblFacturacionSl").DataTable(configs);
            var s = $("#tblFacturacionSl");
            if (s.find('td').html() == "No hay facturas registradas") {
                alert("No se encontraron registros con la póliza y solicitud proporcionadas");
                window.location.href = "https://elaguila.com.mx:8000/es/facturasypolizas";
            }
            table.columns.adjust().draw();
            serviciosEnLinea.fnConsultaSiniestros();
            serviciosEnLinea.getpendientesPago(1, 10);
            //$("#tblSiniestrosSL").find("th").width(10);
            $(".ini").html($("#dates").val());
            $(".name").html($("#nombreAsegurado").html())
        });
        //ConfigBootstapTable('tblInvoicesSL', serviciosEnLinea.GetDataInvoices, actions.GetDataServiceAction);
        //        var s = GetDataTable('tblInvoicesSL', actions.GetDataServiceAction, params);
        //        s.success(function (data) {
        //            $(".ini").html(data[0].dFIniVig + ' - ' + data[0].dFFinVig);
        //        }).then(function () {
        //            serviciosEnLinea.fnConsultaSiniestros();
        //            serviciosEnLinea.getpendientesPago(1,10);
        //        });
    },
    getpendientesPago: function (number, size) {
        var recibosParams = {
            polizaID: poliza,
            offset: number,
            limit: size
        }
        $.ajax({
            url: actions.pagosPendientesAction,
            data: recibosParams
        }).success(function (data) {
            var datas = data;
            $("#tblContainerPendientes").html(data);

        }).then(function () {
            var conFPendientes = {
                "autoWidth": false,
                lengthMenu: [[12, 25, 50, -1], [12, 25, 50, "All"]],
                language: {
                    lengthMenu: "_MENU_ Registros por página",
                    zeroRecords: "No hay pendientes de pago",
                    info: "Mostrando  página _PAGE_ de _PAGES_ (_MAX_ Registros encontrados)",
                    infoEmpty: "No hay pendientes de pago",
                    infoFiltered: "(filtered from _MAX_ total records)",
                    search: "Buscar:",
                    paginate: {
                        first: "<<",
                        last: ">>",
                        next: ">",
                        previous: "<"
                    }
                }
            }
            var table = $("#tblPendientesPagoSL").DataTable(conFPendientes);
            table.columns.adjust().draw();

        });
    }
}
var siniestro = {
    detalle: function (siniestroId) {
        $.ajax({
            url: actions.siniestroDetails,
            data: { "siniestroId": siniestroId },
            context: document.body
        }).done(function (data) {
            var datas = data;
            $("#siniestroModal").html(data);
            $("#detalle").modal();
        });
    }
}

$(document).ready(function () {
    jQuery.ajaxSetup({
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.responseText.replace(/[^a-z0-9]/gi, '') == "ErrInterno") {
                $.unblockUI();
                ShowAlertWindow("Error en proceso favor de intentar de nuevo", 'Error...');
            } else {
                location.href = window.jsUrls.outSession
            }

        },
        statusCode: {
            302: function () {
                alert("Vuelve a iniciar session");
            }
        }
    });


    /*---------------------------------------------------------------------*/
    /* Formateo de la columna de iconos Servicios en Línea                 */
    /*---------------------------------------------------------------------*/
    fieldFormatterSL = function (value, row, index) {
        if (row.nRecibo == 1) {
            return [
                '<a class="pdf" href="javascript:void(0)" title="Visualizar PDF"><i class="invoice img_pdf"></i></a>',
                '<a class="xml" href="javascript:void(0)" title="Descargar XML"><i class="invoice img_xml"></i></a>',
                '<a class="100' + row.Css100 + '" href="javascript:void(0)" title="Visualizar Cobertura 100"><i class="invoice img_100' + row.Css100 + '"></i></a>',
                '<a class="sob" href="javascript:void(0)" title="Visualizar Seguro Obligatorio"><i class="invoice img_sob"></i></a>',
                ].join('');
        }
        else {
            return [
                '<a class="pdf" href="javascript:void(0)" title="Visualizar PDF"><i class="invoice img_pdf"></i></a>',
                '<a class="xml" href="javascript:void(0)" title="Descargar XML"><i class="invoice img_xml"></i></a>',
                ].join('');
        }
    };


    serviciosEnLinea.GetDataInvoices(1, 100);
});

var uAg = navigator.userAgent.toLowerCase();
var isMobile = !!uAg.match(/android|iphone|ipad|ipod|blackberry|symbianos/i);

if (isMobile)
    $('#footer').hide();
