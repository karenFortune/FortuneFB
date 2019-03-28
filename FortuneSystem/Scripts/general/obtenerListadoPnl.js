$(document).ready(function () {
    var ID = $("#IdPedido").val();
    buscar_estilos(ID);
    $("#div_tabla_pnl").css("visibility", "hidden");
});

function probar(id) {
    $('#tabless tr').on('click', function (e) {
        $('#tabless tr').removeClass('highlighted');
        $(this).addClass('highlighted');
    });
    //obtener_tallas_item(id);
}

$(document).on("dblclick", "#tabless tr", function () {
    var row = this.rowIndex;
    var numEstilo = $('#tabless tr:eq(' + row + ') td:eq(0)').html();
    //var estilo = $('#tabless tr:eq(' + row + ') td:eq(2)').html();
    obtener_tallas_item(numEstilo);
});


$(document).on("input", ".numeric", function () {
    this.value = this.value.replace(/\D/g, '');
});

$(document).on("input", ".number", function () {
    this.value = this.value.replace(/\D/g, '');
});

//Registrar Batch

function registrarBatchPNL() {
    var nColumnas = $("#tablePnl tr:last td").length;

    var r = 0; var c = 0; var i = 0; var cadena = new Array(nColumnas - 1);
    for (var x = 0; x < nColumnas - 1; x++) {
        cadena[x] = '';
    }
    var nFilas = $("#tablePnl tbody>tr").length;
    r = 0;
    $('#tablePnl tbody>tr').each(function () {
        if (r > 0) {
            c = 0;
            $(this).find("input").each(function () {
                $(this).closest('td').find("input").each(function () {
                    cadena[c] += this.value + "*";
                    c++;
                });


            });
        }
        r++;
    });
    var error = 0;
    $('#tablePnl').find('td.printed').each(function (i, el) {

        var valor = $(el).children().val();

        if (valor === '') {
            error++;
            $(el).children().css('border', '2px solid #e03f3f');

        } else {

            $(el).children().css('border', '');
        }
    });

    var turno = $('#PNL_Turnos option:selected').val();
    if (turno === "0") {
        error++;
        $('#PNL_Turnos').css('border', '2px solid #e03f3f');
    }
    else {
        $('#PNL_Turnos').css('border', '');
    }

    var maquina = $("#PNL_Maquinas option:selected").val();
    if (maquina === "0") {
        error++;
        $('#PNL_Maquinas').css('border', '2px solid #e03f3f');
    }
    else {
        $('#PNL_Maquinas').css('border', '');
    }
    enviarListaTallaPnl(cadena, error);
}

var estiloId;
function enviarListaTallaPnl(cadena, error) {
    var idTurno = $("#PNL_Turnos option:selected").val();
    var idMaquina = $("#PNL_Maquinas option:selected").val();
    var idStatus = $("input[name='PNL.EstadoPallet']:checked").val();
    var comentario = $("#PNL_Comentarios").val();
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/PNL/Obtener_Lista_Tallas_Pnl",
            datatType: 'json',
            data: JSON.stringify({ ListTalla: cadena, TurnoID: idTurno, EstiloID: estiloId, MaquinaID: idMaquina, StatusID: idStatus, Comentarios: comentario }),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The batch was correctly registered.', 'success', 5, null);
                $('.number').val('0');
                obtener_tallas_item(estiloId);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                showError(xhr.status, xhr.responseText);
                if (data.error === 1) {
                    alertify.notify('Check.', 'error', 5, null);
                }
            }
        });
    }
}


//Actualizar información de un batch
var batchID;
function actualizarBatchPNL() {
    var nColumnas = $("#tablePnl tr:last td").length;

    var r = 0; var c = 0; var i = 0; var cadena = new Array(nColumnas - 1);
    for (var x = 0; x < nColumnas - 1; x++) {
        cadena[x] = '';
    }
    var nFilas = $("#tablePnl tbody>tr").length;
    r = 0;
    $('#tablePnl tbody>tr').each(function () {
        if (r > 0) {
            c = 0;
            $(this).find("input").each(function () {
                $(this).closest('td').find("input").each(function () {
                    cadena[c] += this.value + "*";
                    c++;
                });
            });
        }
        r++;
    });
    var error = 0;
    $('#tablePnl').find('td').each(function (i, el) {

        var valor = $(el).children().val();

        if ($(el).children().val() === '') {
            error++;
            $(el).children().css('border', '2px solid #e03f3f');
        } else {
            $(el).children().css('border', '');
        }
    });

    var turno = $('#PNL_Turnos option:selected').val();
    if (turno === "0") {
        error++;
        $('#PNL_Turnos').css('border', '2px solid #e03f3f');
    }
    else {
        $('#PNL_Turnos').css('border', '');
    }

    var maquina = $("#PNL_Maquinas option:selected").val();
    if (maquina === "0") {
        error++;
        $('#PNL_Maquinas').css('border', '2px solid #e03f3f');
    }
    else {
        $('#PNL_Maquinas').css('border', '');
    }
    enviarListaTallaBatchPnl(cadena, error, batchID);
}

function enviarListaTallaBatchPnl(cadena, error, batchID) {
    var idTurno = $("#PNL_Turnos option:selected").val();
    var idMaquina = $("#PNL_Maquinas option:selected").val();
    var idStatus = $("input[name='PNL.EstadoPallet']:checked").val();
    var comentario = $("#PNL_Comentarios").val();
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/PNL/Actualizar_Lista_Tallas_Batch",
            datatType: 'json',
            data: JSON.stringify({
                ListTalla: cadena, TurnoID: idTurno, EstiloID: estiloId, IdBatch: batchID, MaquinaID: idMaquina, StatusID: idStatus, Comentarios: comentario
            }),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The batch was modified correctly.', 'success', 5, null);
                $('.number').val('0');
                obtener_tallas_item(estiloId);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                showError(xhr.status, xhr.responseText);
                if (data.error === 1) {
                    alertify.notify('Check.', 'error', 5, null);
                }
            }
        });
    }
}

function buscar_estilos(ID) {
    var tempScrollTop = $(window).scrollTop();
    $.ajax({
        url: "/Pedidos/Lista_Estilos_PO/" + ID,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var lista_estilo = jsonData.Data.listaItem;

            $.each(lista_estilo, function (key, item) {
                html += '<tr  onclick="probar(' + item.IdItems + ')">';
                html += '<td>' + item.IdItems + '</td>';
                html += '<td>' + item.EstiloItem + '</td>';
                html += '<td>' + item.ItemDescripcion.Descripcion + '</td>';
                html += '<td>' + item.CatColores.CodigoColor + '</td>';
                html += '<td>' + item.CatColores.DescripcionColor + '</td>';
                html += '<td>' + item.Cantidad + '</td>';
                html += '<td>' + item.Price + '</td>';
                html += '<td>' + item.Total + '</td>';
               // html += '<td><a href="#" onclick="obtener_tallas_item(' + item.IdItems + ');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Sizes"></a></td>';
                html += '</tr>';
            });
            if (Object.keys(lista_estilo).length === 0) {
                html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No styles were found for the PO.</td></tr>';

            }
            $('.tbody').html(html);
            $("#div_estilos_orden").css("visibility", "visible");
            $(window).scrollTop(tempScrollTop);
        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}
var listaPO;

function obtener_tallas_item(IdEstilo) {
    var tempScrollTop = $(window).scrollTop();
    $("#panelPNL").css('display', 'inline');
    $("#loading").css('display', 'inline');
    estiloId = IdEstilo;
    obtener_tallas_PO(IdEstilo);
    $.ajax({
        url: "/Pedidos/Lista_Tallas_Estilo_Pnl/" + IdEstilo,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var estilos = jsonData.Data.estilos;
            var EstiloDescription;
            var lista_estilo_Desc = jsonData.Data.listaTalla;
            $.each(lista_estilo_Desc, function (key, item) {

                EstiloDescription = item.DescripcionEstilo;

            });  
            var lista_Datos_Staging = jsonData.Data.listDatosStaging;
            var datoColor="";
            var datoPais="";
            var datoPorc="";    
            $.each(lista_Datos_Staging, function (key, item) {
                
                datoColor = item.NombreColor;
                datoPais = item.Pais;
                datoPorc = item.Porcentaje;

            });
            var datoColor2 = datoColor === "" ? "N/A" : datoColor;
            var datoPais2 = datoPais === "" ? "N/A" : datoPais;
            var datoPorc2 = datoPorc === "" ? "N/A" : datoPorc;

            if (estilos !== '') {
                $("#div_datos_staging").html("<h3>Color:" + $.trim(datoColor2) + " ---" + " País:" + $.trim(datoPais2) + " --- %:" + $.trim(datoPorc2) + "</h3>");
                $("#div_estilo").html("<h2>Item: " + estilos + "-" + $.trim(EstiloDescription) + "</h2>");
                $("#div_estilo").show();
                $("#div_datos_staging").show();
            } else {
                $("#div_estilo").hide();
                $("#div_datos_staging").hide(); 
            }

            var lista_estilo = jsonData.Data.listaTalla;
            listaEstiloPO = lista_estilo;
            html += '<tr> <th>  </th>';
            $.each(lista_estilo, function (key, item) {

                html += '<th>' + item.Talla + '</th>';

            });
            html += '<th> Total </th>';      
            html += '</tr><tr><td>PO Quantity</td>';
            var cantidadesPO = 0;
            var cadena_cantidades = "";
            $.each(lista_estilo, function (key, item) {

                html += '<td class="total" >' + item.Cantidad + '</td>';
                cantidadesPO += item.Cantidad;
                cadena_cantidades += "*" + item.Cantidad;
            });
            var cantidades_array = cadena_cantidades.split('*');
            html += '<td>' + cantidadesPO + '</td>';
            html += '</tr>';
            var numTallas = 0;
            $.each(lista_estilo, function (key, item) {
                numTallas++;
            });
            html += '</tr><tr><td>Staging Quantity</td>';
            var cantidades = 0;
            var lista_Staging = jsonData.Data.listTallaStaging;//listaStaging.length;
            if (lista_Staging.length === 0) {
                var total = 0;
                for (var v = 0; v < numTallas; v++) {

                    html += '<td>' + total + '</td>';
                    cantidades += total;
                }

            }
            $.each(lista_Staging, function (key, item) {
                
                html += '<td>' + item.total + '</td>';
                
                cantidades += item.total;
            });
            html += '<td>' + cantidades + '</td>';
            html += '</tr><tr><td>PrintShop Quantity</td>';
            var cantidadesPrint = 0;
            var lista_Batch = jsonData.Data.listaTallasTotalBatch;
            var listaTBatch = 0;
            $.each(lista_Batch, function (key, item) {
                listaTBatch++;
            });
            if (listaTBatch === 0) {
                lista_Batch = lista_estilo;
            } else {
                lista_Batch;
            }
            $.each(lista_Batch, function (key, item) {
                if (listaTBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                cantidadesPrint += item;
            });
            html += '<td>' + cantidadesPrint + '</td>';
            html += '</tr><tr><td>PNL Quantity</td>';
            var cantidadesPnl = 0;
            var lista_Batch_PNL = jsonData.Data.listaTallasTotalPnlBatch;
            var listaTBatchPnl = 0;
            $.each(lista_Batch_PNL, function (key, item) {
                listaTBatchPnl++;
            });
            if (listaTBatchPnl === 0) {
                lista_Batch_PNL = lista_estilo;
            } else {
                lista_Batch_PNL;
            }
            $.each(lista_Batch_PNL, function (key, item) {
                if (listaTBatchPnl === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                cantidadesPnl += item;
            });
            html += '<td>' + cantidadesPnl + '</td>';
            html += '</tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Printed</td>';
            var cantidadesPrinted = 0;
            var lista_Batch_Printed = jsonData.Data.listaTallasTotalPBatchPNL;
            var listaPBatch = 0;
            $.each(lista_Batch_Printed, function (key, item) {
                listaPBatch++;
            });
            if (listaPBatch === 0) {
                lista_Batch_Printed = lista_estilo;
            } else {
                lista_Batch_Printed;
            }
            $.each(lista_Batch_Printed, function (key, item) {
                if (listaPBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                // cantidadesPrinted += item;
            });
            html += '</tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ MisPrint</td>';
            var cantidadesMisPrintB = 0;
            var lista_Batch_MP = jsonData.Data.listaTallasTotalMBatchPnl;
            var listaMPBatch = 0;
            $.each(lista_Batch_MP, function (key, item) {
                listaMPBatch++;
            });
            if (listaMPBatch === 0) {
                lista_Batch_MP = lista_estilo;
            } else {
                lista_Batch_MP;
            }
            $.each(lista_Batch_MP, function (key, item) {
                if (listaMPBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                // cantidadesMisPrintB += item;
            });
            html += '</tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Defect</td>';
            var cantidadesDefectB = 0;
            var lista_Batch_Defect = jsonData.Data.listaTallasTotalDBatchPnl;
            var listaDefBatch = 0;
            $.each(lista_Batch_Defect, function (key, item) {
                listaDefBatch++;
            });
            if (listaDefBatch === 0) {
                lista_Batch_Defect = lista_estilo;
            } else {
                lista_Batch_Defect;
            }
            $.each(lista_Batch_Defect, function (key, item) {
                if (listaDefBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                // cantidadesDefectB += item;
            });
            html += '</tr>';
            html += '</tr><tr><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+ Repair</td>';
            var cantidadesRepairB = 0;
            var lista_Batch_Repair = jsonData.Data.listaTallasTotalRBatch;
            var listaRepBatch = 0;
            $.each(lista_Batch_Repair, function (key, item) {
                listaRepBatch++;
            });
            if (listaRepBatch === 0) {
                lista_Batch_Repair = lista_estilo;
            } else {
                lista_Batch_Repair;
            }
            $.each(lista_Batch_Repair, function (key, item) {
                if (listaRepBatch === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }

                // cantidadesRepairB += item;
            });
            html += '</tr>';
            html += '<tr><td>+/-</td>';
            var totales = 0;
            var i = 1;
            var sumaTotal = 0;
            $.each(lista_Batch_PNL, function (key, item) {
                if (listaTBatchPnl === 0) {
                    item = 0;
                }
                var resta = parseFloat(cantidades_array[i]) - parseFloat(item);
                if (resta === 0) {
                    html += '<td class="restaPnl" style="color:black;">' + resta + '</td>';
                } else if (resta >= 0) {
                    html += '<td class="restaPnl" style="color:blue;">' + resta + '</td>';
                } else {
                    html += '<td class="restaPnl" style="color:red;">' + resta + '</td>';
                }
                i++;
                sumaTotal += resta;
            });
            html += '<td>' + sumaTotal + '</td>';
            html += '</tr>';

            if (Object.keys(lista_estilo).length === 0) {
                html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No sizes were found for the style.</td></tr>';
            }
            $('.tbodys').html(html);
            $("#consultaTalla").css("visibility", "visible");
            $("#div_estilo").css("visibility", "visible");
            $("#div_tabla_pnl").css("visibility", "visible");
            $("#div_datos_staging").css("visibility", "visible");
            $("#arte").css("visibility", "visible");
            obtenerImagenPNL(estilos);
            obtenerImagenArte(estilos);
            obtener_bacth_estilo_PNL(IdEstilo);
            if (sumaTotal !== 0) {
                obtenerTallas_Pnl(IdEstilo);
            }
            
            //obtenerIdEstilo(IdEstilo);
            $("#loading").css('display', 'none');
            $(window).scrollTop(tempScrollTop);
        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}

$(document).on('click', '#registrarNuevo', function () {
    obtenerTallas_Pnl(estiloId);
});
var size;
function obtener_bacth_estilo_PNL(IdEstilo) {
    var tempScrollTop = $(window).scrollTop();
    //  $("#loading").css('display', 'inline');
    $.ajax({
        url: "/PNL/Lista_Batch_Estilo/" + IdEstilo,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';

            var estilos = jsonData.Data.estilos;
            var cargoUser = jsonData.Data.cargoUser;
            if (estilos !== '') {
                $("#div_estilo_batch").html("<h2>BATCH REVIEW <h4> STATUS (C-complete / I-incomplete)</h4> </h2> ");
                $("#div_estilo_batch").show();
            } else {
                //$("#div_estilo_batch").hide();
            }
            var lista_batch = jsonData.Data.listaTalla;
            var numBatch = lista_batch.length;
            if (numBatch === 0) {
                // $("#div_tabla_talla").hide();
            }
            html += '<tr> <th>   </th>';
            $.each(lista_batch, function (key, item) {
                size = item.Batch;
            });
            if (numBatch === 0) {
                $.each(size, function (key, item) {
                    // html += '<th>' + item.Talla + '</th>';
                });
            } else {
                $.each(size, function (key, item) {
                    html += '<th>' + item.Talla + '</th>';
                });
            }

            html += '<th> Total </th>';
            html += '<th> User </th>';
            html += '<th> Comments </th>';
            html += '<th> Turn </th>';
            html += '<th> Machine </th>';
            html += '<th> User Modif </th>';
            html += '<th> Status </th>';
            html += '<th> Actions </th>';
            html += '</tr>';


            $.each(lista_batch, function (key, item) {
                html += '<tr><td>Pallet-' + item.IdBatch + '</td>';

                var cantidad = 0;
                $.each(item.Batch, function (key, i) {

                    html += '<td class="total" >' + i.Total + '</td>';
                    cantidad += i.Total;
                });
                html += '<td>' + cantidad + '</td>';
                html += '<td>' + item.NombreUsr + '</td>';
                if (item.Comentarios === '') {
                    item.Comentarios = 'N/A';
                    html += '<td>' + item.Comentarios + '</td>';
                } else {
                    html += '<td>' + item.Comentarios + '</td>';
                }
                if (item.TipoTurno === 1) {
                    html += '<td>First Turn</td>';
                } else {
                    html += '<td>Second Turn</td>';
                }
                html += '<td>' + item.NombreMaquina + '</td>';
                html += '<td>' + item.NombreUsrModif + '</td>';
                html += '<td>' + item.Status + '</td>';

                html += '<td><a href="#" onclick="obtenerTallas_Batch_PNL(' + item.IdBatch + ',' + item.TipoTurno + ',' + item.Maquina + ',' + item.IdPnl + ',\'' + item.Status + '\');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Details Bacth"></a></td>';
                html += '</tr>';


            });
            if (Object.keys(lista_batch).length === 0) {
                html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No batches were found for the style.</td></tr>';

            }
            $('.tbodyBatch').html(html);
            $("#div_estilo_batch").css("visibility", "visible");
            //$("#loading").css('display', 'none');
            $(window).scrollTop(tempScrollTop);

        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
    calcular_Restantes();
}

function obtener_tallas_PO(IdEstilo) {
    $.ajax({
        url: "/Pedidos/Lista_Tallas_Pnl_Estilo/" + IdEstilo,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';

            listaPO = jsonData.Data.listaTalla;
        },
        error: function (errormessage) { alert(errormessage.responseText); }

    });

}

function obtenerIdEstilo(IdEstilo) {
    $.ajax({
        url: "/Arte/ObtenerIdEstilo/" + IdEstilo,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        success: function () {
        },
        error: function (errormessage) { alert(errormessage.responseText); }

    });

}

function obtenerImagenPNL(nombreEstilo) {
    $('#imagenPNL').attr('src', '/Arte/ConvertirImagenPNLEstilo?nombreEstilo=' + nombreEstilo);
}

function obtenerImagenArte(nombreEstilo) {
    $('#imagenArte').attr('src', '/Arte/ConvertirImagenArteEstilo?nombreEstilo=' + nombreEstilo);
}

function ConfirmRev(a) {
    alertify.confirm("Are you sure you want to modify the batch?", function (result) {
        actualizarBatchPNL();
    }).set({
        title: "Confirmation"
    });
}
//Muestra el formulario de registro para las tallas correspondientes del batch
function obtenerTallas_Pnl(idEstilo) {
    var tempScrollTop = $(window).scrollTop();
    //$("#loading").css('display', 'inline');
    CalcularTotalPNL();
    calcular_Restantes();
    $("#PNL_Turnos").val(0);
    $("#PNL_Maquinas").val(0);
    $("#PNL_Comentarios").val('');
    $.ajax({
        url: "/Pedidos/Lista_Tallas_Pnl_Estilo/" + idEstilo,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var estilos = jsonData.Data.estilos;
            if (estilos !== '') {
                $("#div_estilo_pnl").html("<h2> Register new batch</h2>");
                $("#div_estilo_pnl").show();
                $("#registarBatch").hide();
                $("#modificarBatch").hide();
                $("#guardarBatch").show();
                $("#registrarNuevo").hide();
            } else {
                $("#div_estilo_pnl").hide();
            }

            var lista_estilo = jsonData.Data.listaTalla;
            var list = lista_estilo.length;
            if (list === 0) {
                lista_estilo = listaEstiloPO;
            }
            html += '</tr><tr><td>PO </td>';
            var cantidadesPO = 0;
            var cadena_cantidades = "";
            $.each(listaPO, function (key, item) {

                cadena_cantidades += "*" + item.Cantidad;
            });
            var cantidades_array = cadena_cantidades.split('*');
            var lista_Batch = jsonData.Data.listaPrint;
            var listaBat = lista_Batch.length;
            if (listaBat === 0) {
                lista_Batch = listaPO;
            } else {
                lista_Batch;
            }
            var totales = 0;
            var i = 1;
            $.each(lista_Batch, function (key, item) {
                if (listaBat === 0) {
                    html += '<td id="po"><input type="text" id="po" class="txtDes form-control cantPO"  value="' + item.Cantidad + '"/></td>';
                } else {
                    var resta = parseFloat(cantidades_array[i]) - parseFloat(item);
                    html += '<td id="po"><input type="text" id="po" class="txtDes form-control cantPO"  value="' + resta + '"/></td>';
                    i++;
                }

                cantidadesPO += item.Cantidad;
            });

            html += '<th> QTY </th>';
            html += '<tr > <th>  </th>';
            //*************************************************
            var lista_estilo_Tallas = jsonData.Data.listaEstiloTallas;
            $.each(lista_estilo_Tallas, function (key, item) {

                html += '<td><input type="text" id="talla" class="txtDes form-control talla" value="' + item.Talla + '"/></td>';

            });

            html += '<th> Total </th>';
            html += '</tr><tr><td>Printed</td>';
            var cantidades = 0;

            $.each(lista_estilo_Tallas, function (key, item) {
                item.Printed = item.Talla;
                item.Printed = 0;
                html += '<td class="printed"><input type="text" id="cantidad" class="txt form-control print numeric" onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
                cantidades += item.Printed;
            });
            html += '<td><input type="text" id="totalP" class="form-control number "  value="' + cantidades + '" readonly/></td>';
            html += '</tr><tr><td>MisPrint</td>';
            var misPrintCant = 0;
            $.each(lista_estilo_Tallas, function (key, item) {

                item.MisPrint = item.Talla;
                item.MisPrint = 0;
                html += '<td ><input type="text" id="misprint" class="txt form-control mp numeric" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';


                misPrintCant += item.MisPrint;
            });
            html += '<td><input type="text" id="totalM" class="form-control number totalM" value="' + misPrintCant + '" readonly/></td>';
            html += '</tr><tr ><td class="dato">Defect</td>';
            var defectCant = 0;

            $.each(lista_estilo_Tallas, function (key, item) {

                item.Defect = item.Talla;
                item.Defect = 0;
                html += '<td ><input type="text" id="defect" class="txt form-control def numeric " onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';


                defectCant += item.Defect;
            });
            html += '<td><input type="text" id="totalD" class="form-control number totalD" value="' + defectCant + '" readonly/></td>';
            html += '</tr><tr ><td class="dato">Repair</td>';
            var repairCant = 0;

            $.each(lista_estilo_Tallas, function (key, item) {

                item.Repair = item.Talla;
                item.Repair = 0;
                html += '<td ><input type="text" id="repair" class="txt form-control rep numeric " onChange="calcular_Repair()" value="' + item.Repair + '"/></td>';


                repairCant += item.Repair;
            });
            html += '<td><input type="text" id="totalR" class="form-control number totalR" value="' + repairCant + '" readonly/></td>';
            html += '</tr><tr ><td class="total">+/-</td>';
            var total = 0;
            $.each(lista_estilo_Tallas, function (key, item) {
                html += ' <div class="span7">';
                item.Defect = item.Talla;
                item.Defect = 0;
                html += '<td ><input type="text" id="falt" class="form-control number totalFal" value="' + item.Defect + '" readonly/></td>';

                html += ' </div>';
                total = cantidades + misPrintCant + defectCant + repairCant;
            });
            html += '<td><input type="text" id="totalF" class="form-control number totalF" value="' + total + '" readonly/></td>';
            html += '</tr>';


            $('.tbodyprint').html(html);
            $("#div_estilo_pnl").css("visibility", "visible");
            // $("#loading").css('display', 'none');
            $(window).scrollTop(tempScrollTop);

        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}


function obtenerTallas_Batch_PNL(idBatch, idTurno, idMaquina, idPrintShop, idStatus) {
    // var tempScrollTop = $(window).scrollTop(); 
    $("#PNL_Turnos").val(idTurno);
    $("#PNL_Maquinas").val(idMaquina);
    if (idStatus === "C") {
        $("input[name='PNL.EstadoPallet'][value='true']").prop("checked", true);
    } else {
        $("input[name='PNL.EstadoPallet'][value='false']").prop("checked", true);
    }

    $('#PNL_Turnos').css('border', '');
    $('#PNL_Maquinas').css('border', '');

    batchID = idBatch;
    var actionData = "{'idEstilo':'" + estiloId + "','idBatch':'" + idBatch + "'}";
    $.ajax({
        url: "/PNL/Lista_Tallas_Pnl_IdEstilo_IdBatch/",
        type: "POST",
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var estilos = jsonData.Data.estilos;
            if (estilos !== '') {
                $("#div_estilo_pnl").html("<h2> Details Batch </h2>");
                $("#div_estilo_pnl").show();
                $("#registarBatch").show();
                $("#guardarBatch").hide();
                $("#modificarBatch").show();
                $("#registrarNuevo").show();
            } else {
                $("#div_estilo_pnl").hide();
            }

            var lista_estilo = jsonData.Data.listaTalla;
            var list = lista_estilo.length;
            if (lista_estilo === 0) {
                lista_estilo = listaEstiloPO;
            } else {
                lista_estilo;

            }
            html += '</tr><tr><td>PO </td>';
            var cantidadesPO = 0;
            var cadena_cantidades = "";
            $.each(listaPO, function (key, item) {

                cadena_cantidades += "*" + item.Cantidad;
            });
            var cantidades_array = cadena_cantidades.split('*');
            var lista_Batch = jsonData.Data.listaPrint;
            var listaBat = lista_Batch.length;
            if (listaBat === 0) {
                lista_Batch = listaPO;
            } else {
                lista_Batch;
            }
            var totales = 0;
            var i = 1;
            $.each(lista_Batch, function (key, item) {
                if (listaBat === 0) {
                    html += '<td id="po"><input type="text" id="po" class="txtDes form-control cantPO"  value="' + item.Cantidad + '"/></td>';
                } else {
                    var resta = parseFloat(cantidades_array[i]) - parseFloat(item);
                    html += '<td id="po"><input type="text" id="po" class="txtDes form-control cantPO"  value="' + resta + '"/></td>';
                    i++;
                }

                cantidadesPO += item.Cantidad;
            });
            html += '<th> QTY </th>';
            html += '<tr > <th>  </th>';
            $.each(lista_estilo, function (key, item) {
                html += '<td><input type="text" id="talla" class="txtDes form-control talla" value="' + item.Talla + '"/></td>';

            });
            html += '<th> Total </th>';
            $.each(lista_estilo, function (key, item) {
                identificador = item.IdPrintShop;

            });
            html += '</tr><tr><td>Printed</td>';
            var cantidades = 0;
            $.each(lista_estilo, function (key, item) {
                if (list === 0) {
                    item.Printed = 0;
                    html += '<td><input type="text" id="cantidad" class="txt form-control print numeric" onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
                } else {

                    html += '<td><input type="text" id="cantidad" class="txt form-control print numeric"  onChange="calcular_Printed()" value="' + item.Printed + '"/></td>';
                }

                cantidades += item.Printed;
            });
            html += '<td><input type="text" id="totalP" class="form-control number"  value="' + cantidades + '" readonly/></td>';
            html += '</tr><tr><td>MisPrint</td>';
            var misPrintCant = 0;
            $.each(lista_estilo, function (key, item) {
                if (list === 0) {
                    item.MisPrint = 0;
                    html += '<td ><input type="text" id="misprint" class="txt form-control mp numeric" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';
                } else {

                    html += '<td > <input type="text" id="misprint" class=" txt form-control mp numeric" onChange="calcular_MisPrint()" value="' + item.MisPrint + '"/></td>';
                }

                misPrintCant += item.MisPrint;
            });
            html += '<td><input type="text" id="totalM" class="form-control number totalM" value="' + misPrintCant + '" readonly/></td>';
            html += '</tr><tr ><td class="dato">Defect</td>';
            var defectCant = 0;
            $.each(lista_estilo, function (key, item) {
                if (list === 0) {
                    item.Defect = 0;
                    html += '<td ><input type="text" id="defect" class="txt form-control def numeric " onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';
                } else {

                    html += '<td ><input type="text" id="defect" class="txt form-control def numeric" onChange="calcular_Defect()" value="' + item.Defect + '"/></td>';
                }

                defectCant += item.Defect;
            });
            html += '<td><input type="text" id="totalD" class="form-control number totalD" value="' + defectCant + '" readonly/></td>';
            html += '</tr><tr ><td class="dato">Repair</td>';
            var repairCant = 0;
            $.each(lista_estilo, function (key, item) {
                if (list === 0) {
                    item.Repair = 0;
                    html += '<td ><input type="text" id="repair" class="txt form-control rep numeric " onChange="calcular_Repair()" value="' + item.Repair + '"/></td>';
                } else {

                    html += '<td ><input type="text" id="repair" class="txt form-control rep numeric" onChange="calcular_Repair()" value="' + item.Repair + '"/></td>';
                }

                repairCant += item.Repair;
            });
            html += '<td><input type="text" id="totalR" class="form-control number totalR" value="' + repairCant + '" readonly/></td>';
            /*  html += '</tr><tr ><td class="total">+/-</td>';
              var total = 0;
              $.each(lista_estilo, function (key, item) {
                  html += ' <div class="span7">';
                  if (list === 0) {
                      item.Defect = 0;
                      html += '<td ><input type="number" id="falt" class="form-control totalFalt" value="' + item.Defect + '" readonly/></td>';
                  } else {
                      item.Defect = 0;
                      html += '<td ><input type="number" id="falt" class="form-control totalFalt" value="' + item.Defect + '" readonly/></td>';
                  }
                  html += ' </div>';
                  total = cantidades + misPrintCant + defectCant + repairCant;
              });*/
            html += '</tr><tr ><td class="total">+/-</td>';
            var total = 0;
            $.each(lista_estilo, function (key, item) {
                html += ' <div class="span7">';
                item.Defect = item.Talla;
                item.Defect = 0;
                html += '<td ><input type="text" id="falt" class="form-control number totalFal" value="' + item.Defect + '" readonly/></td>';

                html += ' </div>';
                total = cantidades + misPrintCant + defectCant + repairCant;
            });
            html += '<td><input type="text" id="totalF" class="form-control number totalF" value="' + total + '" readonly/></td>';
            html += '</tr>';

            $('.tbodyprint').html(html);
            $("#div_estilo_pnl").css("visibility", "visible");
            CalcularTotalBatchPNL();
            CalcularTotalPNL();
            calcular_Restantes();
            // $(window).scrollTop(tempScrollTop); 
        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}