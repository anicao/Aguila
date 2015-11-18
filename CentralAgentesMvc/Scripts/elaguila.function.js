var FristLogin = false;

// Función para imprimir la cotización
function RefreshCalculoCotizacion() {
    $.get(window.jsUrls.Cotizar, {
        ejecutaCalculo: true
    }, function (msg) {
        $("#dvRefreshCalculo").html(msg);

        // Refrescar el grid de Desglose por Vehiculo
        RefreshDetalleTables('tblDesglosePago', 'RefreshCalculoVehiculos', 'Cotizacion');

        // Refrescar la pestaña de formas de pago
        RefreshDetalleTables('tblFormasPago', 'RefreshCalculoFormasPagos', 'Cotizacion');
    });

    // Colocar como activa la pestaña de importe
    $('#tabColor a[href="#tbImporteCtz"]').tab('show');

    // Busco errores
    $.get(window.jsUrls.WarningsC, function (warnings) {
        var message = "";
        $.each(warnings, function (key, value) {
            message += "<li class='smallText'>" + value + "</li>";
        });
        if (message.length > 0) {
            $('#dvBodyInfo').html("<div><ul>" + message + "</ul></div>");
            $('#dvWinInfo').show();
            //$('#QuiereCederComision').prop('checked', value == 1);
            $('#CedeComision').hide();
        }
    });
}

/*-------------------------------------------------------------*/
/* Función para mostrar mensaje de espera                      */
/*-------------------------------------------------------------*/
function ShowWait() {
    var msje = '<div class="loading">'
                 + '    <img style="height: 80px; margin:30px;" src="../Content/images/loading.gif" alt="Loading.."/>'
                 + '    <h4 style="color:gray; font-weight:normal;">Porfavor espere....</h4>'
                 + '</div>';

    $.blockUI({
        message: msje
            , baseZ: 2000
            , css: {
                border: "none"
                , top: ($(window).height() - 300) / 2 + 'px'
                , left: ($(window).width() - 300) / 2 + 'px'
                , backgroundColor: '#000'
                , color: '#000'
                , width: '200px'
                , '-webkit-border-radius': '10px'
                , '-moz-border-radius': '10px'
            }
    });
}

/*-------------------------------------------------------------*/
/* Función para levantar ventana SrPago                 */
/*-------------------------------------------------------------*/
function ShowPagoWindow(msje, titleWin, exitoso, origen) {
    var $message = '';
    var dialog = new BootstrapDialog({
        title: titleWin,
        message: function (dialogRef) {
            if (exitoso) {
                $message = $('<div>' + msje + '<img src="../Content/images/BIENVENIDACOT.jpg" style="width: 550px; height: 370px;"></img></div>' 
//+
//                    '<script type="text/javascript">' +
//                    '/* <![CDATA[ */' +
//                    'var google_conversion_id = 960553754;' +
//                    'var google_conversion_language = "en";' +
//                    'var google_conversion_format = "3";' +
//                    'var google_conversion_color = "ffffff";' +
//                    'var google_conversion_label = "MQIPCMD21VgQmsaDygM";' +
//                    'var google_remarketing_only = false;' +
//                    '/* ]]> */' +
//                    '</script>' +
//                    '<script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js">' +
//                    '</script>' +
//                    '<noscript>' +
//                    '<div style="display:inline;">' +
//                    '<img height="1" width="1" style="border-style:none;" alt="" src="//www.googleadservices.com/pagead/conversion/960553754/?label=MQIPCMD21VgQmsaDygM&amp;guid=ON&amp;script=0"/>' +
//                    '</div>' +
//                    '</noscript>'
);
            } else {
                $message = $('<div>' + msje + '</div><!-- Google Code for Registro Conversion Page -->');
            }
            var $button = $('<div style="padding: 5px; text-align: right; border-top: 1px solid #E5E5E5;"><button class="btn btn-default">OK</button></div>');
            if (exitoso) {
                $button.on('click', {
                    dialogRef: dialogRef
                }, function (event) {
                    if (origen == "") {
                        window.location.replace("https://elaguila.com.mx");
                    } else {
                        location.href = window.jsUrls.home;
                    }
                });
            } else {
                $button.on('click', {
                    dialogRef: dialogRef
                }, function (event) {
                    event.data.dialogRef.close();
                });
            }
            $message.append($button);

            return $message;
        },
        cssClass: 'Pago',
        draggable: false,
        closable: false
    });
    dialog.realize();
    dialog.getModalHeader().css('background-color', '#C91D3B');
    dialog.open();
}

/*-------------------------------------------------------------*/
/* Función para levantar ventana de alertas                    */
/*-------------------------------------------------------------*/
function ShowAlertWindow(msje, titleWin, icon) {
    if (icon == undefined) icon = "img-alert"
    var $textAndPic = $('<div id="alerts"><img class="' + icon + '">' + msje + '</div>');
    BootstrapDialog.alert({
        type: BootstrapDialog.TYPE_DANGER,
        title: titleWin,
        message: $textAndPic,
        closable: false,
        draggable: true
    });
}
/*----------------------------------------------------------------------------*/
/*     Función para las acciones de los botones de las tablas y ventanas      */
/*----------------------------------------------------------------------------*/
function ShowWinHtml(title, dialogClass, iconClass, aButtons, html) {
    var $textAndPic = $('<div></div>');
    $textAndPic.append(html);
    if (title == "Datos Generales de la Cotización" || title == "Datos Generales de la Renovación") {
        var controls = $textAndPic.find("#dataExtraPoliza_statusFinal").val();
        if (title == "Datos Generales de la Renovación") {
            var segEstatus = $textAndPic.find("#dataExtraPoliza_segEstatus").val();
            if (controls == "E" && segEstatus == "REN") {
                aButtons[0].cssClass = "btn btn-sm btn-primary disabled";
                aButtons[2].cssClass = "btn btn-sm btn-primary disabled";
            }
        } else {
            if (controls == "E" && controls == "e") {
                aButtons[0].cssClass = "btn btn-sm btn-primary disabled";
                aButtons[2].cssClass = "btn btn-sm btn-primary disabled";
            }
         }
        
     }
    var isClosable = true;
    if (aButtons.length == 1)
        isClosable = false;

    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_PRIMARY,
        title: "<span class='" + iconClass + "'></span> " + title,
        message: $textAndPic,
        closable: isClosable,
        closeByBackdrop: false,
        closeByKeyboard: false,
        draggable: true,
        cssClass: dialogClass,
        buttons: aButtons
    });
};

/*----------------------------------------------------------*/
/* Función para pintar el borde de controles con errores    */
/*----------------------------------------------------------*/
function OnPaintErrorsAndSuccess(elements) {
    // Muestro tooltip con errores de validación
    $.each(elements.errores, function (key, value) {
        var group = $("#" + key + "").closest(".form-validator");
        if (group.length > 0) {

            group.removeClass("has-error");
            $("#" + key + "").qtip('destroy', true);

            if (value != '') {
                group.addClass("has-error").removeClass("has-success");

                var corners = ['left center', 'right center'];
                var flipIt = $("#" + key + "").parents('span.right').length > 0;
                $("#" + key + "").qtip({
                    content: {
                        text: value[value.length - 1].ErrorMessage
                    },
                    position: {
                        my: corners[flipIt ? 1 : 0],
                        at: corners[flipIt ? 0 : 1],
                        viewport: $("#" + key + "")
                    },
                    show: {
                        ready: false
                    },
                    overwrite: false,
                    style: {
                        classes: 'qtip-default qtip qtip-red qtip-shadow qtip-rounded'
                    }
                });
            }
        }
        var group2 = $("input[name$='" + key + "']").closest(".form-validator");
        if (group2.length > 0) {
            if (value != '') {
                group2.addClass("has-error").removeClass("has-success");

                var corners2 = ['left center', 'right center'];
                var flipIt2 = $("input[name$='" + key + "']").parents('span.right').length > 0;
                $("input[name$='" + key + "']").not('.valid').qtip({
                    content: {
                        text: value[value.length - 1].ErrorMessage
                    },
                    position: {
                        my: corners2[flipIt2 ? 1 : 0],
                        at: corners2[flipIt2 ? 0 : 1],
                        viewport: $("input[name$='" + key + "']")
                    },
                    show: {
                        ready: false
                    },
                    overwrite: false,
                    style: {
                        classes: 'qtip-default qtip qtip-red qtip-shadow qtip-rounded'
                    }
                });
            } else {
                group2.removeClass("has-error");
                $("input[name$='" + key + "']").qtip('destroy', true);
            }
        }
    });

    // Levanto ventana con errores de operación
    var message = "";
    $.each(elements.validaciones, function (key, value) {
        if (value != "ERROR")
            message += "<li class='smallText'>" + value + "</li>";
    });

    if (message.length > 0) {
        $('#dvBodyError').html("<div><ul>" + message + "</ul></div>");
        $('#dvWinErrors').show();
    }
}
/*-------------------------------------------------------------*/
/* Función para levantar ventana para Pagos */
/*-------------------------------------------------------------*/
function PagosWin(title, url) {
    $.get(url, function (result) {
        var $textAndPic = $('<div></div>');
        $textAndPic.append(result);

        BootstrapDialog.show({
            type: BootstrapDialog.TYPE_PRIMARY,
            title: "<span class='fa fa-cubes fa-2x'></span> " + title,
            message: $textAndPic,
            closable: false,
            draggable: true,
            buttons: [{
                label: 'Generar Pago',
                cssClass: 'btn btn-primary',
                action: function (dialog) {
                    dialog.close();
                }
            }, {
                label: 'Cerrar',
                cssClass: 'btn btn-default',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    });
}
/*-------------------------------------------------------------*/
/* Función para levantar ventana de En Construccion            */
/*-------------------------------------------------------------*/
function constrWin(title, url) {
    $.get(url, function (result) {
        var $textAndPic = $('<div></div>');
        $textAndPic.append(result);

        BootstrapDialog.show({
            type: BootstrapDialog.TYPE_PRIMARY,
            title: "<span class='fa fa-cubes fa-2x'></span> " + title,
            message: $textAndPic,
            closable: false,
            draggable: true,
            buttons: [{
                label: 'Aceptar',
                cssClass: 'btn btn-primary',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    });
};
/*-------------------------------------------------------------*/
/* Función para levantar ventana para Cotización express       */
/*-------------------------------------------------------------*/
function CotizaExpressWin(title, url) {
    var ids = (title == "Express" ? "0" : title == "Publicos" ? "1" : "2");
    $.get(url, {
        id: ids
    }, function (result) {
        var $textAndPic = $('<div></div>');
        $textAndPic.append(result);

        BootstrapDialog.show({
            type: BootstrapDialog.TYPE_PRIMARY,
            title: "<span class='fa fa-file-text'></span> Cotizar " + title,
            message: $textAndPic,
            closable: false,
            draggable: true,
            buttons: [{
                id: 'Cot',
                label: 'Cotiza',
                cssClass: 'btn btn-primary',
                action: function (dialog) {
                    ShowWait();
                    $.ajax({
                        type: "POST",
                        url: $("#formaExp").attr("action"),
                        data: $("#formaExp").serialize(),
                        success: function (msg) {
                            ShowWait();
                            ShowCotizarWin("0");
                            $.unblockUI();
                        },
                        error: function (e) {
                            $.unblockUI();
                            alert(e.responseText);
                        }
                    });
                }
            }, {
                label: 'Cancelar',
                cssClass: 'btn btn-default',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    });
}
/*----------------------------------------------------------*/
/* Función para configurar los objetos cotizar express      */
/*----------------------------------------------------------*/
function ConfigExpressControls(ctrlFchCac, ctrltxtEdad, ctrlDtTxt) {
    // Configuro las fechas
    if (ctrlFchCac != null) {
        $("#" + ctrlFchCac + " .input-group.date").datepicker({
            format: "dd/mm/yyyy",
            orientation: "top auto",
            todayBtn: "linked",
            clearBtn: true,
            language: "es",
            todayHighlight: true,
            autoclose: true
        }).on("changeDate", function (ev) {
            var edad = new Date().getFullYear() - ev.date.getFullYear();
            $("#" + ctrltxtEdad + "").val(edad)
        });
    }
    // Cambia texto de fecha cuando modifican el textbox de edad
    if (ctrltxtEdad != null) {
        $("#" + ctrltxtEdad + "").on("change", function () {
            var d = new Date();
            $("#" + ctrlDtTxt + "").val(d.getUTCDate() + "/" + d.getMonth() + "/" + (d.getFullYear() - $("#" + ctrltxtEdad + "").val()));
            $("#" + ctrlFchCac + " .input-group.date").datepicker('setDate', d.getUTCDate() + "/" + d.getMonth() + "/" + (d.getFullYear() - $("#" + ctrltxtEdad + "").val()));
        });
    }
}
/*-------------------------------------------------------------*/
/* Función para validar solo numeros                           */
/*-------------------------------------------------------------*/
function ValidaNumero(ctrlTxt) {
    $("#" + ctrlTxt + "").keydown(function (event) {
        if (event.shiftKey) {
            event.preventDefault();
        }

        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9) { } else {
            if (event.keyCode < 95) {
                if (event.keyCode < 48 || event.keyCode > 57) {
                    event.preventDefault();
                }
            } else {
                if (event.keyCode < 96 || event.keyCode > 105) {
                    event.preventDefault();
                }
            }
        }
    });
}
/*-------------------------------------------------------------*/
/* Función para validar solo letras                            */
/*-------------------------------------------------------------*/
function SoloLetras(ctrlTxt) {
    $("#" + ctrlTxt + "").bind('keyup blur', function () {
        var node = $(this);
        node.val(node.val().replace(/[^a-zñA-ZÑ\s]/g, ''));
    });
}
/*-------------------------------------------------------------*/
/* Función para generar los caracteres del Captcha             */
/*-------------------------------------------------------------*/
function RandomCaptcha() {
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (var i = 0; i < 8; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}

/*-------------------------------------------------------------*/
/* Función para regenerar los caracteres el Captcha            */
/*-------------------------------------------------------------*/
function RecargaCaptcha(s) {
    s = s + "?random=" + RandomCaptcha();
    $.ajax({
        type: "POST",
        url: s,
        contentType: "image/jpeg",
        cache: true,
        success: function (data) {
            $.unblockUI();
            var img = $('#CaptchaImg');
            img.attr('src', s);
        },
        error: function (e) {
            $.unblockUI();
            ShowAlertWindow(e.responseText, 'Error...');
        }
    });
}
/*-------------------------------------------------------------*/
/* Función para levantar ventana para los reportes de Agentes  */
/*-------------------------------------------------------------*/
function ReportesAgentesWin(title, url) {
    var id = (title == "Liquidaciones" ? "0" : title == "Movimientos" ? "1" : title == "Renovaciones" ? "2" : "3");

    $.get(url, {
        rptID: id
    }, function (result) {
        var buttons = [{
            label: 'Generar',
            cssClass: 'btn btn-primary',
            action: function (dialog) {
                GeneraReportesAgentes();
            }
        }, {
            label: 'Cancelar',
            cssClass: 'btn btn-default',
            action: function (dialog) {
                dialog.close();
            }
        }];
        ShowWinHtml(title, '', 'fa fa-file-text', buttons, result);
    });
}

/*-------------------------------------------------------------*/
/* Función para levantar ventana para Perfil del Usuario       */
/*-------------------------------------------------------------*/
function UserProfileWin(url) {
    var titles = "";
    if (FristLogin) {
        titles = "Actualiza tus datos";
    } else {
        titles = "Perfil del Usuario";
    }
    $.get(window.jsUrls.editaPerfil, function (result) {
        var buttons = [{
            label: 'Aceptar',
            cssClass: 'btn btn-sm btn-primary',
            action: function (dialog) {
                var form = $('#frmUserData');
                form.validate();
                if (!form.valid()) return false;

                $.ajax({
                    type: "POST",
                    url: form.attr("action"),
                    data: form.serialize(),
                    success: function (msg) {
                        if (msg.success == undefined) {
                            $.growl.notice({
                                title: titles,
                                message: '<div id="alerts"><img class="img-sucess">Información actualizada satisfactoriamente</div>',
                                duration: 30000,
                                size: "large"
                            });
                            dialog.close();
                            FristLogin = false;
                        } else {
                            OnPaintErrorsAndSuccess(msg);
                        }
                    },
                    error: function (e) {
                        ShowAlertWindow(e.responseText, 'Error...');
                    }
                });
            }
        }, {
            label: 'Cancelar',
            cssClass: 'btn btn-sm btn-default',
            action: function (dialog) {
                dialog.close();
            }
        }];

        ShowWinHtml(titles, '', 'fa fa-user fa-lg', buttons, result);
    });
}

/*----------------------------------------------------------*/
/* Función para configurar los objetos de consulta          */
/*----------------------------------------------------------*/
function ConfigSearchControls(ctrlFchIni, ctrlFchFin, ctrlAgID, ctrlDdlAge, ctrlAgeNm) {
    // Configuro las fechas
    if (ctrlFchIni != null) {
        $("#" + ctrlFchIni + " .input-group.date").datepicker({
            format: "dd/mm/yyyy",
            orientation: "top auto",
            todayBtn: "linked",
            clearBtn: true,
            language: "es",
            todayHighlight: true,
            autoclose: true
        }).on("changeDate", function (ev) {
            $("#" + ctrlFchFin + " .input-group.date").find('input').val('').change()
            $("#" + ctrlFchFin + " .input-group.date").datepicker('remove');

            if (ev.date != undefined) {
                var nowTemp = new Date(ev.date.valueOf());
                $("#" + ctrlFchFin + " .input-group.date").datepicker({
                    format: "dd/mm/yyyy",
                    orientation: "top auto",
                    language: "es",
                    todayHighlight: true,
                    autoclose: true,
                    clearBtn: true,
                    startDate: nowTemp
                });
            }
        });
    }

    // Cambia el objeto seleccionado en el dropdown cuando modifican el textbox
    if (ctrlAgID != null) {
        $("#" + ctrlAgID + "").on("change", function () {
            $("#" + ctrlDdlAge + "").val($("#" + ctrlAgID + "").val());
            if (ctrlAgeNm != null) {
                $("#" + ctrlAgeNm + "").val($("#" + ctrlDdlAge + "" + " option:selected")[0].innerHTML);
            }
        });

        // Captura el ID seleccionado del DropDown y lo asigna al textbox
        $("#" + ctrlDdlAge + "").on("change", function () {
            $("#" + ctrlAgID + "").val($("#" + ctrlDdlAge + "" + " option:selected").val());
            if (ctrlAgeNm != null) {
                $("#" + ctrlAgeNm + "").val($("#" + ctrlDdlAge + "" + " option:selected")[0].innerHTML);
            }
        });
    }
}
/*----------------------------------------------------------*/
/* Función para dibujar un gráfico                          */
/*----------------------------------------------------------*/
function DrawGraph(divName, s1, s2, ticks, lbl1, lbl2) {
    var plot = $.jqplot(divName, [s1, s2], {
        animate: !$.jqplot.use_excanvas,
        animateReplot: true,

        // The "seriesDefaults" option is an options object that will
        // be applied to all series in the chart.
        seriesDefaults: {
            renderer: $.jqplot.BarRenderer,
            pointLabels: {
                show: true
            },
            rendererOptions: {
                fillToZero: true
            }
        },
        // Custom labels for the series are specified with the "label"
        // option on the series option.  Here a series option object
        // is specified for each series.
        series: [{
            label: '&nbsp;' + lbl1 + '&nbsp;'
        }, {
            label: '&nbsp;' + lbl2 + '&nbsp;'
        }, ],
        // Show the legend and put it outside the grid, but inside the
        // plot container, shrinking the grid to accomodate the legend.
        // A value of "outside" would not shrink the grid and allow
        // the legend to overflow the container.
        legend: {
            show: true,
            placement: 'outsideGrid'
        },
        axes: {
            // Use a category axis on the x axis and use our custom ticks.
            xaxis: {
                renderer: $.jqplot.CategoryAxisRenderer,
                ticks: ticks
            },
            // Pad the y axis just a little so bars can get close to, but
            // not touch, the grid boundaries.  1.2 is the default padding.
            yaxis: {
                pad: 1.05,
                tickOptions: {
                    formatString: '%#.2f'
                }
            }
        }
    });
}
/*----------------------------------------------------------*/
/* Función para recuperar datos via json                    */
/*----------------------------------------------------------*/
function GetDataTable(tableID, urlData, params) {
    var $table = $('#' + tableID + '');
    return $.get(urlData, params, function (data) {
        var myData = {
            total: 0,
            rows: {}
        };
        if (data.length > 0) {
            myData = {
                total: data[0].total,
                rows: data
            };
        }
        $table.bootstrapTable('load', myData);
    });
}

/*----------------------------------------------------------*/
/* Función para configurar tabla                            */
/*----------------------------------------------------------*/
function ConfigBootstapTable(tableID, pageChgFunc, urlRefresh, clkFunc, dblClkFunc, srchFunc) {
    $('#' + tableID + '').bootstrapTable()
        .on('page-change.bs.table', function (e, number, size) {
            pageChgFunc(number, size);
        })
        .on('click', 'tbody tr', function (event) {
            $(this).addClass('highlight').siblings().removeClass('highlight');
        });

    if (urlRefresh != undefined && urlRefresh != null) {
        $('#' + tableID + '').bootstrapTable('refresh', {
            url: urlRefresh
        });
    };

    if (clkFunc != undefined && clkFunc != null) {
        $('#' + tableID + '').on('click-row.bs.table', function (e, row, $element) {
            clkFunc(e, row, $element);
        });
    }

    if (dblClkFunc != undefined && dblClkFunc != null) {
        $('#' + tableID + '').on('dbl-click-row.bs.table', function (e, row, $element) {
            dblClkFunc(e, row, $element);
        });
    }

    if (srchFunc != undefined && srchFunc != null) {
        $('#' + tableID + '').on('search.bs.table', function (e, text) {
            srchFunc(e, text);
        });
    }
}
/*----------------------------------------------------------*/
/* Función para configurar el highlight de la tabla         */
/*----------------------------------------------------------*/
function SetHighLightClickInTables(tableID) {
    $('#' + tableID + '').bootstrapTable();
    $('#' + tableID + '').on('click', 'tbody tr', function (event) {
        $(this).addClass('highlight').siblings().removeClass('highlight');
    });
}
/*----------------------------------------------------------*/
/* Función para asociar datos a la tabla                    */
/*----------------------------------------------------------*/
function SetDataInTables(tableID, data) {
    $('#' + tableID + '').bootstrapTable('load', data);
}

/*----------------------------------------------------------*/
/* Función para formatear de números a formato de monedas   */
/*----------------------------------------------------------*/
function moneyFormatter(value) {
    var num = '$' + value.toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    return num;
}

/*----------------------------------------------------------*/
/* Función para presentar checkbox en las columnas          */
/*----------------------------------------------------------*/
function checkFormatter(value, row, index) {
    return [
        '<input class="checkbox-sm" type="checkbox" ' + (value ? "checked" : "") + ' disabled=true />',
    ].join('');
};

/*----------------------------------------------------------*/
/* Función para formatear de json date a fechas javascript  */
/*----------------------------------------------------------*/
function dateFormatter(value, row) {
    var date = new Date(value.match(/\d+/)[0] * 1);
    return date.toLocaleDateString();
}

/*----------------------------------------------------------*/
/* Función para formatear datos de tarjetas                 */
/*----------------------------------------------------------*/
function tcFormatter(value) {
    var num = value.substring(0, 4) + ' - **** - **** - ' + value.substring(value.length - 4);
    return num;
}

/*----------------------------------------------------------*/
/* Función para formatear datos de código de seguridad      */
/*----------------------------------------------------------*/
function blackFormatter(value) {
    var num = '***' + value.substring(value.length - 1);
    return num;
}

/*----------------------------------------------------------*/
/* Función para formatear fecha vigencia de Tarjetas        */
/*----------------------------------------------------------*/
function vgFormatter(value) {
    var lng = (value.length > 7 ? 3 : 0);
    var num = value.substring(lng);
    return num;
}

/*----------------------------------------------------------*/
/* Función para eliminar registros                          */
/*----------------------------------------------------------*/
function DeleteObject(title, msg, iconClass, urlData, params, objRefresh) {
    BootstrapDialog.confirm({
        title: "<span class='" + iconClass + "'></span> " + title,
        message: '<div id="alerts"><img class="img-quest">' + msg + '</div>',
        btnOKLabel: 'Si',
        btnCancelLabel: 'No',
        callback: function (result) {
            if (result) {
                $.get(urlData, params, function (data) {
                    if (data == "OK") {
                        RefreshDetalleTables(objRefresh.Control, objRefresh.Action);
                        $.growl.notice({
                            title: title,
                            message: '<div id="alerts"><img class="img-sucess">Información eliminada</div>',
                            duration: 5000,
                            size: "large"
                        });
                    }
                });
            }
        }
    });
}

/*----------------------------------------------------------------------------*/
/*     Función para las acciones de los botones de las tablas y ventanas      */
/*----------------------------------------------------------------------------*/
function ShowDataInfo(titleWin, url, params, html) {
    var buttons = null;
    var dialogCls = null;
    var iconClass = null;

    // Uso el Id de póliza si es renovación
    if (params.Modulo == "Renovaciones") params.id = $("#poliza").val();

    switch (titleWin) {
        case "Datos Generales de la Cotización":
            {
                window.RefreshDT = true;
                dialogCls = 'big-dialog';
                iconClass = 'fa fa-file-text-o fa-lg';

                buttons = [{
                    label: 'Guardar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        return $.ajax({
                            type: "POST",
                            url: window.jsUrls.SaveData,
                            data: {
                                modulo: params.modulo
                            },
                            success: function (msg) {

                                if (msg.success == undefined) {
                                    // Cambio el Nro de la cotización
                                    $("#cotizacionID").val(msg);

                                    // Refresco el control izquierdo de Detalle General
                                    $.get(window.jsUrls.RefViewDG, function (result) {
                                        $("#fsGeneral").html(result);
                                    });
                                    $('#CedeComision').hide();

                                    // Refresco el control de Complementos
                                    $.get(window.jsUrls.RefViewCP, function (result) {
                                        $("#dvRefrescable").html(result);
                                    });

                                    // Refresco el grid de Cotizaciones / Renovaciones
                                    if ($('#dvTablaCalculo') == undefined) {
                                        LoadData(1, 10);
                                    }

                                    var txt = (params.isnew ? "Cotización creada con el No.: " : "Cotización actualizada. No.: ") + msg;
                                    $.growl.notice({
                                        title: "Cotizaciones",
                                        message: '<div id="alerts"><img class="img-sucess">' + txt + '</div>',
                                        duration: 30000,
                                        size: "large"
                                    });

                                    // Activo el botón de Pagar
                                    var boton = $('#BtnPagoAgentes');
                                    if (boton != undefined)
                                        boton.prop('disabled', false);
                                    var impr = $('#Imprimir');
                                    impr.val("true");
                                } else {
                                    // impresion es true
                                    var impr = $('#Imprimir');
                                    impr.val("false");
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cotizar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        RefreshCalculoCotizacion();
                    }
                }, {
                    label: 'Imprimir',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        var impr = $('#Imprimir');
                        if ($("#cotizacionID").val() != 0 & impr.val() == "false") {
                            $("button:contains('Guardar')").click();
                            PrintCotizacion($("#cotizacionID").val());
                        } else {
                            PrintCotizacion($("#cotizacionID").val());
                        }
                    }
                }, {
                    label: 'Cerrar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        location.href = window.jsUrls.reloadCotizacion;

                        window.RefreshDT = false;
                        $('#dvWinErrors').hide();
                        $('#dvWinInfo').hide();
                    }
                }];
                break;
            }
        case "Datos Generales de la Renovación":
            {
                window.RefreshDT = true;
                dialogCls = 'big-dialog';
                iconClass = 'fa fa-file-text-o fa-lg';
                buttons = [{
                    label: 'Guardar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        $.ajax({
                            type: "POST",
                            url: window.jsUrls.SaveData,
                            data: {
                                modulo: params.modulo
                            },
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    // Cambio el Nro de la cotización
                                    $("#poliza").val(msg);

                                    // Refresco el control izquierdo de Detalle General
                                    $.get(window.jsUrls.RefViewDG, function (result) {
                                        $("#fsGeneral").html(result);
                                    });
                                    $('#CedeComision').hide();
                                    // Refresco el control de Complementos
                                    $.get(window.jsUrls.RefViewCP, function (result) {
                                        $("#dvRefrescable").html(result);
                                    });

                                    // Refresco el grid de Cotizaciones / Renovaciones
                                    LoadData(1, 10);

                                    var txt = (params.isnew ? "Renovación creada con el No.: " : "Renovación actualizada. No.: ") + msg;
                                    $.growl.notice({
                                        title: "Renovaciones",
                                        message: '<div id="alerts"><img class="img-sucess">' + txt + '</div>',
                                        duration: 30000,
                                        size: "large"
                                    });
                                    var boton = $('#BtnPagoAgentes');
                                    if (boton != undefined)
                                        boton.prop('disabled', false);
                                    var impr = $('#Imprimir');
                                    impr.val("true");
                                } else {
                                    // impresion es true
                                    var impr = $('#Imprimir');
                                    impr.val("false");
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cotizar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        RefreshCalculoCotizacion();
                    }
                }, {
                    label: 'Imprimir',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        var impr = $('#Imprimir');
                        if (impr.val() == "false") {
                            $("button:contains('Guardar')").click();
                            PrintRenovacion($("#poliza").val());
                        } else {
                            PrintRenovacion($("#poliza").val());
                        }
                    }
                }, {
                    label: 'Cerrar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                        window.RefreshDT = false;
                    }
                }];
                break;
            }
        case "Nueva Cotización":
            { }
        case "Editar Documento":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");
                window.RefreshGT = true;
                dialogCls = 'middle-dialog';
                iconClass = 'fa fa-file-o fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmGenData');
                        form.validate();
                        if (!form.valid()) return false;

                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (window.RefreshDT) {
                                    RefreshDetalleTables('tblVehiculos', 'RefreshCars');

                                    // Refresco el control izquierdo de Detalle General
                                    $.get(window.jsUrls.RefViewDG, function (result) {
                                        $("#fsGeneral").html(result);
                                    });
                                    $('#CedeComision').hide();
                                }
                                if (msg.success == undefined) {
                                    dialog.close();
                                    if (params.isnew == true) {
                                        ShowDataInfo('Nuevo Conductor', '', {
                                            id: params.id,
                                            modulo: params.Modulo,
                                            isnew: params.isnew
                                        }, msg);
                                    }
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                        window.RefreshGT = false;
                    }
                }];
                break;
            }
        case "Nuevo Conductor":
            { }
        case "Editar Conductor":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");

                dialogCls = 'small-dialog';
                iconClass = 'fa fa-users fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmDriver');
                        form.validate();
                        if (!form.valid()) return false;

                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();
                                    if (window.RefreshDT) {
                                        RefreshDetalleTables('tblConductores', 'RefreshDrivers');
                                    }
                                    if (params.isnew == true) {
                                        window.DriversAdded--;

                                        // Pregunto si desean agregar mas conductores
                                        if (window.DriversAdded > 0) {
                                            BootstrapDialog.confirm({
                                                message: '<div id="alerts"><img class="img-quest">¿ Desea agregar otro Conductor ?</div>',
                                                btnOKLabel: 'Si',
                                                btnCancelLabel: 'No',
                                                callback: function (result) {
                                                    if (result) {
                                                        ShowDataInfo('Nuevo Conductor', window.jsUrls.AddDriver, {
                                                            id: params.id,
                                                            modulo: params.Modulo,
                                                            isnew: params.isnew
                                                        });
                                                    } else {
                                                        ShowDataInfo('Nuevo Vehículo', '', {
                                                            id: params.id,
                                                            modulo: params.Modulo,
                                                            isnew: params.isnew
                                                        }, msg);
                                                        window.DriversAdded = 0;
                                                    }
                                                }
                                            });
                                        } else {
                                            ShowDataInfo('Nuevo Vehículo', '', {
                                                id: params.id,
                                                modulo: params.Modulo,
                                                isnew: params.isnew
                                            }, msg);
                                        }
                                    }
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                        if (params.isnew) {
                            ShowDataInfo('Datos Generales de la Cotización', window.jsUrls.DtGeneral, {
                                id: params.id,
                                modulo: params.Modulo
                            });
                        }
                    }
                }];
                break;
            }
        case "Nuevo Vehículo":
            { }
        case "Editar Vehículo":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");
                dialogCls = 'middle-dialog';
                iconClass = 'fa fa-car fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmCar');
                        form.validate();
                        if (!form.valid()) return false;

                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();
                                    if (window.RefreshDT) {
                                        RefreshDetalleTables('tblVehiculos', 'RefreshCars');

                                        // Refresco el control izquierdo de Detalle General
                                        $.get(window.jsUrls.RefViewDG, function (result) {
                                            $("#fsGeneral").html(result);
                                        });
                                        $('#CedeComision').hide();
                                    }
                                    if (params.isnew == true) {
                                        window.CarsAdded--;

                                        // Pregunto si desean agregar mas vehiculos
                                        if (window.CarsAdded > 0) {
                                            BootstrapDialog.confirm({
                                                message: '<div id="alerts"><img class="img-quest">¿ Desea agregar otro Vehículo ?</div>',
                                                btnOKLabel: 'Si',
                                                btnCancelLabel: 'No',
                                                callback: function (result) {
                                                    if (result) {
                                                        ShowDataInfo('Nuevo Vehículo', window.jsUrls.AddCar, {
                                                            id: params.id,
                                                            modulo: params.Modulo,
                                                            isnew: params.isnew
                                                        });
                                                    } else {
                                                        ShowDataInfo('Datos Generales de la Cotización', '', {
                                                            id: params.id,
                                                            modulo: params.Modulo
                                                        }, msg);
                                                        window.CarsAdded = 0;
                                                    }
                                                }
                                            });
                                        } else {
                                            ShowDataInfo('Datos Generales de la Cotización', '', {
                                                id: params.id,
                                                modulo: params.Modulo
                                            }, msg);
                                        }
                                    }
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                        $('#dvWinErrors').hide();
                        if (params.isnew) {
                            ShowDataInfo('Datos Generales de la Cotización', window.jsUrls.DtGeneral, {
                                id: params.id,
                                modulo: params.Modulo
                            });
                        }
                    }
                }];
                break;

            }
        case "Nuevo Prospecto":
            { }
        case "Editar Prospecto":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");
                dialogCls = 'small-dialog';
                iconClass = 'fa fa-bookmark-o fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmProspect');
                        form.validate();
                        if (!form.valid()) return false;

                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();
                                    if (window.RefreshPP) {
                                        RefreshDetalleTables('tblProspectos', 'RefreshProspects');
                                        window.RefreshPP = true;
                                    }
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }
        case "Modificar Complemento":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");
                dialogCls = 'small-dialog';
                iconClass = 'fa fa-book fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmComplement');
                        form.validate();
                        if (!form.valid()) return false;

                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();
                                    $("#dvRefrescable").html(msg);
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }
        case "Desglose de Pagos":
            { }
        case "Editar Desglose de Pagos":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");
                dialogCls = 'small-dialog';
                iconClass = 'fa fa-cubes fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmDesglosePago');
                        form.validate();
                        if (!form.valid()) return false;

                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();

                                    // Controlar los objetos a actualizar en el html
                                    var tpCot = $('#hdTipoCotizacion');
                                    if ((tpCot != undefined) && (tpCot.val() == 'EXPRESS')) {
                                        RefrescaPartialExpress();
                                    } else {
                                        RefreshDetalleTables('tblTarjetasP', 'RefreshDesglosePagos');
                                        TotalizaPagos();
                                    }
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }
        case "Datos del Pago":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");
                dialogCls = 'small-dialog dialog-bottom';
                iconClass = 'fa fa-cubes fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmDesglosePago');
                        form.validate();
                        if (!form.valid()) return false;

                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();

                                    // Controlar los objetos a actualizar en el html
                                    var tpCot = $('#hdTipoCotizacion');
                                    if ((tpCot != undefined) && (tpCot.val() == 'EXPRESS')) {
                                        RefrescaPartialExpress();
                                    } else {
                                        RefreshDetalleTables('tblTarjetasP', 'RefreshDesglosePagos');
                                        TotalizaPagos();
                                    }
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }
        case "Modificar Seguimiento":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");
                dialogCls = 'small-dialog';
                iconClass = 'fa fa-clock-o fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmTracking');
                        form.validate();
                        if (!form.valid()) return false;

                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();
                                    $("#dvRefreshTracking").html(msg);
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }
        case "Nueva Dirección":
            { }
        case "Editar Dirección":
            {

                dialogCls = 'small-dialog';
                iconClass = 'fa fa-street-view fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmAddress');
                        form.validate();
                        if (!form.valid()) return false;
                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();

                                    // Controlar los objetos a actualizar en el html
                                    var tpCot = $('#hdTipoCotizacion');
                                    if ((tpCot != undefined) && (tpCot.val() == 'EXPRESS'))
                                        RefrescaPartialExpress();
                                    else
                                        RefreshGeneralTables('tblAddress', 'RefreshAddress');
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }

        case "Dirección de Facturación":
            {
                // Desctivo el botón de Pagar
                var boton = $('#BtnPagoAgentes');
                if (boton != undefined)
                    boton.prop('disabled', true);
                // impresion es falso
                var impr = $('#Imprimir');
                if (impr != undefined)
                    impr.val("false");
                dialogCls = 'small-dialog dialog-bottom';
                iconClass = 'fa fa-street-view fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmAddress');
                        form.validate();
                        if (!form.valid()) return false;
                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();

                                    // Controlar los objetos a actualizar en el html
                                    var tpCot = $('#hdTipoCotizacion');
                                    if ((tpCot != undefined) && (tpCot.val() == 'EXPRESS'))
                                        RefrescaPartialExpress();
                                    else
                                        RefreshGeneralTables('tblAddress', 'RefreshAddress');
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        if (document.getElementById("hdSteps")) {
                            $("#hdSteps").val(2);
                        }
                        dialog.close();
                    }
                }];
                break;
            }
        case "Nuevo Teléfono":
            { }
        case "Editar Teléfono":
            {
                dialogCls = 'small-dialog';
                iconClass = 'fa fa-phone fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmPhone');
                        form.validate();
                        if (!form.valid()) return false;
                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();
                                    RefreshGeneralTables('tblPhones', 'RefreshPhones');
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }
        case "Consulta de Vehículos":
            {
                dialogCls = 'small-dialog';
                iconClass = 'fa fa-car fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var row = {
                            Clave: $('#HiddenID').val(),
                            Descripcion: $('#HiddenDesc').val()
                        };
                        SelectedCarDblClk(0, row, 0);
                    }
                }, {
                    id: 'btnCloseSearch',
                    label: 'Cerrar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }
        case "Serie y Placa Vehículo":
            {
                titleWin = titleWin + " " + params.descVeh;
                dialogCls = 'xsmall-dialog dialog-bottom'
                iconClass = 'fa fa-tags fa-lg';
                buttons = [{
                    label: 'Aceptar',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        var form = $('#frmSeriePlaca');
                        form.validate();
                        //alert(form.serialize(true));
                        if (!form.valid()) return false;
                        $.ajax({
                            type: "POST",
                            url: form.attr("action"),
                            data: form.serialize(true),
                            success: function (msg) {
                                if (msg.success == undefined) {
                                    dialog.close();
                                    RefrescaPartialExpress();
                                } else {
                                    OnPaintErrorsAndSuccess(msg);
                                }
                            },
                            error: function (e) {
                                ShowAlertWindow(e.responseText, 'Error...');
                            }
                        });
                    }
                }, {
                    label: 'Cancelar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        if (document.getElementById("hdSteps")) {
                            $("#hdSteps").val(3);
                        }
                        dialog.close();
                    }
                }];
                break;
            }
        case "Pagar Cotización":
            {
                dialogCls = 'small-dialog';
                iconClass = 'fa fa-money fa-lg';
                buttons = [{
                    label: 'Aplicar Pago',
                    cssClass: 'btn btn-sm btn-primary',
                    action: function (dialog) {
                        PagoAgentes(dialog);
                    }
                }, {
                    label: 'Cerrar',
                    cssClass: 'btn btn-sm btn-default',
                    action: function (dialog) {
                        dialog.close();
                    }
                }];
                break;
            }
    }

    // Llamo a la ventana
    if (html == undefined) {
        return $.get(url, params, function (result) {
            ShowWinHtml(titleWin, dialogCls, iconClass, buttons, result);
        });
    } else {
        ShowWinHtml(titleWin, dialogCls, iconClass, buttons, html);
    }

}
$(function () {
    if (FristLogin) {
        UserProfileWin('/Account/UserProfile');
    }
    /*-------------------------------------------------------------*/
    /* Función para mostrar imagen de espera en los submit         */
    /*-------------------------------------------------------------*/
    $("form").on("submit", function () {
        if ($(this).valid()) {
            ShowWait();
        }
    });

    // Don't allow browser caching of forms
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

    /*-------------------------------------------------------------*/
    /* Run this function for all validation error messages         */
    /*-------------------------------------------------------------*/
    $('.field-validation-error').each(function () {
        // Get the name of the element the error message is intended for
        // Note: ASP.NET MVC replaces the '[', ']', and '.' characters with an
        // underscore but the data-valmsg-for value will have the original characters
        var inputElem = '#' + $(this).attr('data-valmsg-for').replace('.', '_').replace('[', '_').replace(']', '_');

        var corners = ['left center', 'right center'];
        var flipIt = $(inputElem).parents('span.right').length > 0;

        // Hide the default validation error
        $(this).hide();

        // Show the validation error using qTip
        $(inputElem).filter(':not(.valid)').qtip({
            content: {
                text: $(this).text()
            },
            position: {
                my: corners[flipIt ? 1 : 0],
                at: corners[flipIt ? 0 : 1],
                viewport: $(window)
            },
            show: {
                ready: true
            },
            hide: false,
            style: {
                classes: 'qtip-red qtip-shadow qtip-rounded',
                widget: true
            }
        });
    });

    /*-------------------------------------------------------------*/
    /* Función para mostrar el ValidationSumary                    */
    /*-------------------------------------------------------------*/
    $('.validation-summary-errors').each(function () {
        if ($(this).text() != "\n") {
            $(this).hide();
            ShowAlertWindow($(this).text(), "Validaciones");
        }
    });
    /*---------------------------------------------------------------------*/
    /* Eventos a ejecutar en el click  Facturas / Servicios en Línea       */
    /*---------------------------------------------------------------------*/
    window.operateEventsFCT = {
        'click .pdf': function (e, value, row, index) {
            if (row.Serie != null) {
                var html = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">' + '<html xmlns="http://www.w3.org/1999/xhtml">' + '<head>' + '    <title>Visor PDF</title>' + '    <style type="text/css">' + '        body { width: 100%; height: 100%; }' + '        .mycss { width: 98%; height: 800px;}' + '    </style>' + '</head>' + '<body>' + "    <object class='mycss' data='" + row.LnkPDF + "'>" + "        <embed src='" + row.LnkPDF + "' type='application/pdf' />" + "    </object>" + '</body>' + '</html>';

                // If so, then this is Chrome
                if (navigator.userAgent.search("Chrome") >= 0) {
                    window.open(row.LnkPDF, '_self', "resizable=1, status=1, scrollbars=1; height:100%; width:100%;")
                } else {
                    var wnd = window.open('', '_blank', 'resizable=1, status=1, scrollbars=1; height:100%; width:100%;')
                    wnd.document.write(html);
                }
            }
        },
        'click .xml': function (e, value, row, index) {
            if (row.Serie != null) {
                window.open(row.LnkXML, '_self', "resizable=1, status=1, scrollbars=1; height:100%; width:100%;")
            }
        },
        'click .100': function (e, value, row, index) {
            if ((row.Lnk100 != null) && (row.Lnk100 != '')) {
                var url = window.jsUrls.RefViewer + row.Lnk100;
                window.open(url, "rpt100", "resizable=1, status=1, scrollbars=1; height:100%; width:100%;");
            } else {
                ShowAlertWindow("La póliza no tiene Cobertura 100", "Cobertura...");
            }
        },
        'click .sob': function (e, value, row, index) {
            if ((row.LnkSOB != null) && (row.LnkSOB != '')) {
                var url = window.jsUrls.RefViewer + row.LnkSOB;
                window.open(url, "rptSob", "resizable=1, status=1, scrollbars=1; height:100%; width:100%;");
            } else {
                ShowAlertWindow("La póliza no tiene Seguro Obligatorio", "Seguro...");
            }
        }
    };

});