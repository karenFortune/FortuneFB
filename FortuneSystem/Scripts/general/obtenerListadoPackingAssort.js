var idPedido;
$(document).ready(function () {
    var ID = $("#IdPedido").val();
    idPedido = ID;
    buscar_estilos(ID);
    $("#div_tabla_packing").css("visibility", "hidden");

});

function probar() {
    $('#tabless tr').on('click', function (e) {
        $('#tabless tr').removeClass('highlighted');
        $(this).addClass('highlighted');
    });
}

$(document).on("keyup", "input.tcart", function () {
    obtTotalPiezasRatioAssort();
});

$(document).on("keyup", "input.rat", function () {
    obtTotalPiezasRatioAssort();
});

$(document).on("input", ".numeric", function () {
    this.value = this.value.replace(/\D/g, '');
});
var idCargo;
$(document).on("click", "#btnAssort", function () {
    $("#panelPacking").css('display', 'inline');
    $.ajax({
        url: "/Packing/Listado_Packing_Assort/",
        type: 'POST',
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var num = jsonData.Data.totalRegistros;
            var cargo = jsonData.Data.cargoUser;
            var tPiezasEstilos = jsonData.Data.numTPSyle;
            var tPiezasPack = jsonData.Data.numTPack;
            idCargo = cargo;
            $("#arte").css('display', 'none');
           // if (tPiezasPack <= tPiezasEstilos) {                
                if (num !== 0) {
                    if (cargo === 1 || cargo === 9) {
                        $("#loading").css('display', 'inline');
                        $("#packBPPK").hide();
                        $("#packAssort").show();
                        $('#agregarNuevoPack').show();
                        $("#div_titulo_Registro").css('display', 'inline');
                        $("#opcionesAssort").css('display', 'none');
                        $("#opcionesPAssort").css('display', 'inline');
                        $("#regAssort").css('display', 'none');
                        $("#consultaTalla").css('height', '1040px');
                        $("#consultaTalla").css('width', '121%');
                        $("#consultaTalla").css("visibility", "visible");
                        $("#nuevoPalletAssort").hide();
                        $("#opcionesRegPallet").css('display', 'none');
                        $("#div_titulo_Register_pallet").css('display', 'none');
                        $("#loading").css('display', 'none');
                        ActualizarSelectPackingName();
                    } else {
                        $("#loading").css('display', 'inline');
                        $("#packBPPK").hide();
                        $("#packAssort").show();
                        $('#agregarNuevoPack').hide();
                        $("#nuevoPalletAssort").hide();
                        $("#div_titulo_Registro").css('display', 'inline');
                        $("#opcionesPAssort").css('display', 'inline');
                        $("#regAssort").css('display', 'none');
                        $("#consultaTalla").css("visibility", "visible");
                        $("#consultaTalla").css('height', '1040px');
                        $("#consultaTalla").css('width', '121%');
                        $("#loading").css('display', 'none');
                    }

                } else {
                    if (cargo === 1 || cargo === 9) {
                        cargarPanelAssort();
                    } else {
                        $("#loading").css('display', 'inline');
                        $("#packBPPK").hide();
                        $("#packAssort").show();
                        $('#agregarNuevoPack').hide();
                        $("#nuevoPalletAssort").hide();
                        $("#panelNoEstilos").css('display', 'inline');
                        $("#imgPanel").css('cursor', 'none');
                        $("#consultaTalla").css('height', '1040px');
                        $("#arte").css('display', 'none');
                        $("#consultaTalla").css('width', '121%');
                        $("#consultaTalla").css("visibility", "visible");
                        $("#loading").css('display', 'none');
                    }

                }
           // }

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    }).done(function (data) {

    }); 
    
});

function ConfirmEmpaqueAssort() {
    var confirm = alertify.confirm('Confirmation', 'Do you want to register a new style for the packaging ?', null, null).set('labels', { ok: 'Accept', cancel: 'Cancel' });
    confirm.set('closable', false);
    confirm.set('onok', function () {
       
        limpiarFormaAssort();       
     
    });
    confirm.set('oncancel', function () {
        limpiarRegisAssort();
        $('#selectEstilos').parent().hide();
        $('label[for="Packing_PackingTypeSize_PackingName"]').hide();
        $('#Packing_PackingTypeSize_PackingName').hide();
        $('label[for="Packing_PackingTypeSize_AssortName"]').hide();
        $('#Packing_PackingTypeSize_AssortName').hide();
        $("#div_titulo_Assort").hide();
        $('label[for="Packing_PackingTypeSize_Cantidad"]').hide();
        $("#Packing_PackingTypeSize_Cantidad").hide();
        $('label[for="Packing_PackingTypeSize_Cartones"]').hide();
        $("#Packing_PackingTypeSize_Cartones").hide();
        $('label[for="Packing_PackingTypeSize_TotalUnits"]').hide();
        $("#Packing_PackingTypeSize_TotalUnits").hide();
        $("#opcionesPAssort").css('display', 'inline');
        ActualizarSelectPackingName();
        //$('#registroPiezasModal').modal('show'); 
        
    });
}

function ConfirmNewAssort() {
    $("#Packing_PackingAssort_CantCartons").css('border', '1px solid #cccccc');
    $('#nuevoPalletAssort').prop("disabled", false);
    $('#Packing_PackingAssort_Turnos').val(0);
    $('#Packing_PackingAssort_Turnos').css('border', '2px solid #e03f3f');
    $('#selectEstilos').find('option:not(:first)').remove();
    var confirm = alertify.confirm('Confirmation', 'Do you want to register a new style for the packaging ?', null, null).set('labels', { ok: 'Accept', cancel: 'Cancel' });
    confirm.set('closable', false);
    confirm.set('onok', function () {

        $.ajax({
            url: "/Packing/ListadoEstilos/" + idPedido,
            method: 'POST',
            dataType: "json",
            success: function (jsonData) {
                var html = '';
                var listaEstilos = jsonData.Data.listEstilo;
                var block = jsonData.Data.Block;
                var nameAssort = jsonData.Data.AssortBlock;
                $.each(listaEstilos, function (key, item) {
                    html += '<option  value="' + item.IdSummary + '">' + item.ItemEstilo + '</option>';
                });
                $('#selectEstilos').append(html);
                $('#selectEstilos').parent().show();
                $('label[for="Packing_PackingTypeSize_PackingName"]').show();
                $('#Packing_PackingTypeSize_PackingName').show();
                $('#Packing_PackingTypeSize_PackingName').val(block); 
                $('label[for="Packing_PackingTypeSize_AssortName"]').show();
                $('#Packing_PackingTypeSize_AssortName').show();
                $('#Packing_PackingTypeSize_AssortName').val(nameAssort);
                $("#div_titulo_Assort").show();
                $("#div_titulo_Assort").html("<h3>REGISTRATION OF TYPE OF PACKAGING- ASSORTMENT</h3>");
                $("#opcionesAssort").css('display', 'inline');              
                $("#opcionesPAssort").css('display', 'none');
                $("#lTallasAssort").css('display', 'none');
                $("#batch").css('display', 'none');
                $("#div_titulo_Registro").css('display', 'none');
                $("#regAssort").css('display', 'none');
                $("#opcionesRegPallet").css('display', 'none');
                $("#div_titulo_Register_pallet").css('display', 'none'); 
                $("#nuevoPalletAssort").hide(); 
                $('label[for="Packing_PackingTypeSize_Cantidad"]').show();
                $("#Packing_PackingTypeSize_Cantidad").show();
                $("#Packing_PackingTypeSize_Cantidad").val(0);
                $('label[for="Packing_PackingTypeSize_Cartones"]').show();
                $("#Packing_PackingTypeSize_Cartones").show();
                $("#Packing_PackingTypeSize_Cartones").val(0);
                $('label[for="Packing_PackingTypeSize_TotalUnits"]').show();
                $("#Packing_PackingTypeSize_TotalUnits").show();
                $("#Packing_PackingTypeSize_TotalUnits").val(0);
                limpiarRegisAssort();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        }).done(function (data) {

        });
    });
    confirm.set('oncancel', function () {  
 
    });
}

function ActualizarSelectPackingName() {
    $('#selectPackingName').find('option:not(:first)').remove();
        $.ajax({
            url: "/Packing/ListadoPackingRegistrados/" + idPedido,
            method: 'POST',
            dataType: "json",
            success: function (jsonData) {
                var html = '';
                var listaEstilos = jsonData.Data.listEstilo;
                
                $.each(listaEstilos, function (key, item) {
                    html += '<option  value="' + item.PackingRegistrado + '">' + item.PackingRegistrado + '</option>';
                });
                $('#selectPackingName').append(html);
                $('#selectPackingName').parent().show();                
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        }).done(function (data) {

        }); 
}


function limpiarFormaAssort() {
    $('#tablaTallasAssort tbody>tr').each(function () {
        $(this).find("input.rat").each(function () {
            $(this).closest('td').find("input.rat").each(function () {
                var valor = $(this).val(0);
            });
        });
    });
    var tEstilo = $("#selectEstilos option:selected").val();
    $("#selectEstilos option[value='" + tEstilo+"']").remove();
    $("#selectEstilos").val('').trigger("change");
    $('#Packing_PackingTypeSize_PackingName').prop('readonly', true);
    $('#Packing_PackingTypeSize_AssortName').prop('readonly', true);
    $('#Packing_PackingTypeSize_Cantidad').prop('readonly', true);
    $('#Packing_PackingTypeSize_Cartones').prop('readonly', true);
    
}

function limpiarRegisAssort() {
    $('#tablaTallasAssort tbody>tr').each(function () {
        $(this).find("input.rat").each(function () {
            $(this).closest('td').find("input.rat").each(function () {
                var valor = $(this).val(0);
            });
        });
    });
  
    $("#selectEstilos").val('').trigger("change");
    $('#Packing_PackingTypeSize_PackingName').prop('readonly', false);
    $('#Packing_PackingTypeSize_AssortName').prop('readonly', false);
    $('#Packing_PackingTypeSize_Cantidad').prop('readonly', false);
    $('#Packing_PackingTypeSize_Cartones').prop('readonly', false);
    $('#agregarNuevoPack').show();
    
    
}
$(function () {
    $('#selectEstilos').change(function () {
        var selectedText = $(this).find("option:selected").text();
        var selectedValue = $(this).val();
        var html = '';
        var tEstilo = $("#selectEstilos option:selected").val();
        if (tEstilo !== "") {
            obtenerTallas(parseInt(tEstilo));
            $("#regAssort").show();
        } else {
            $("#regAssort").hide();
        }
        
    });

    $('#selectPackingName').change(function () {
        $("#Packing_PackingAssort_CantCartons").css('border', '1px solid #cccccc');
        $('#Packing_PackingAssort_Turnos').css('border', '1px solid #cccccc');
        var selectedText = $(this).find("option:selected").text();
        var selectedValue = $(this).val();
        var html = '';
        var namePack = $("#selectPackingName option:selected").val();
        if (namePack !== "") {
            obtenerListaTallasAssort(namePack); 
            
            //$("#regAssort").show();
        } else {
           // $("#regAssort").hide();
        }

    });
});

$(document).on("click", "#nuevaQty", function () {
    var error = 0;
    var numQty = $("#Packing_PackingTypeSize_Cantidad").val();
    var numCart = $("#Packing_PackingTypeSize_Cartones").val();
    var tUnits = $("#Packing_PackingTypeSize_TotalUnits").val();
    var nombrePack = $("#Packing_PackingTypeSize_PackingName").val();

    if (numQty === "0") {
        error++;
        $('#Packing_PackingTypeSize_Cantidad').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_Cantidad').css('border', '');
    }  
    if (numCart === "0") {
        error++;
        $('#Packing_PackingTypeSize_Cartones').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_Cartones').css('border', '');
    } 
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        var actionData = "{'qty':'" + numQty + "','cart':'" + numCart + "','totalUnits':'" + tUnits + "','packName':'" + nombrePack + "'}";
        $.ajax({
            url: "/Packing/ActualizarPackingAssort/",
            type: 'POST',
            data: actionData,
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (jsonData) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The quantities of the packing were registered correctly.', 'success', 5, null);
                $('#registroPiezasModal').modal('hide');
                 $("#Packing_PackingTypeSize_Cantidad").val(0);
                 $("#Packing_PackingTypeSize_Cartones").val(0);
                $("#Packing_PackingTypeSize_TotalUnits").val(0);
                $("#opcionesPAssort").css('display', 'inline');
                ActualizarSelectPackingName();
                
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        }).done(function (data) {
        });
    }
});

function cargarPanelAssort() {
    $("#loading").css('display', 'inline');
    $("#packBPPK").hide();
    $("#packAssort").show();
    $('#agregarNuevoPack').hide();
    $("#div_titulo_Assort").html("<h3>REGISTRATION OF TYPE OF PACKAGING- ASSORTMENT</h3>");
    $("#consultaTalla").css('height', '1040px');
    $("#consultaTalla").css('width', '121%');
    $("#opcionesAssort").css('display', 'inline');
    $("#opcionesPAssort").css('display', 'none');
    $("#div_titulo_Registro").css('display', 'none');
    $("#regAssort").css('display', 'none');
    $("#consultaTalla").css("visibility", "visible");
    $("#nuevoPalletAssort").hide();    
    $("#loading").css('display', 'none');
}  
var numBlock = 0;
var totalCartones = 0;
var totalPiezas = 0; 
var totalPiezasFaltantes = 0;
var totalCartonesFaltantes = 0;
function obtenerListaTallasAssort(namePack) {
   
    var actionData = "{'packingName':'" + namePack + "'}";
    $.ajax({
        url: "/Packing/Lista_Estilos_Empaque_Assort/",
        type: 'POST',
        dataType: "json",
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        success: function (jsonData) {
            var html = '';
            var listaEstilos = jsonData.Data.listaPackingBox;
            var cartonesFaltantes = jsonData.Data.numTotalCartonesFalt;
            totalCartonesFaltantes = cartonesFaltantes;
            var piezasFaltantes = jsonData.Data.numTotalPiezasFat;
            totalPiezasFaltantes = piezasFaltantes;
            $("#div_titulo_Registro").css('display', 'inline');
            $("#regAssort").css('display', 'inline');
            $("#div_titulo_Registro").html("<h3>REGISTRATION OF ASSORTMENT</h3>");
            html += '<table class="table" id="tablaAssort"  width="100%"><thead>';
            html += '<tr>'+
                 '<th style="visibility:hidden;"> </th> ' +
                '<th> STYLE#</th> ' +
                '<th> STYLE NAME</th> ' +
                ' <th>COLOR</th>' +
                ' <th>SIZES</th>' +
                ' <th>RATIO</th>' +                
                //' <th>ACTIONS</th>' +
                '</tr>' +
                '</thead><tbody>';
            var cont = 0;
            $.each(listaEstilos, function (key, item) {
                totalCartones = item.TotalCartones;
                totalPiezas = item.Pieces;
                numBlock = item.IdBlockPack;
                cont = cont + 1;
                html += '<tr id="pallet' + cont + '" class="pallet">';
                html += '<td style="visibility:hidden; width:1px;"><input type="text" id="f-id" class="form-control" value="' + item.IdSummary + '" /></td>';
                html += '<td>' + item.ItemDescripcion.ItemEstilo + '</td>';
                html += '<td>' + item.ItemDescripcion.Descripcion + '</td>';
                html += '<td>' + item.ItemDescripcion.CatColores.CodigoColor + '</td>';
                html += '<td>' + item.TallasGrl + '</td>';
                html += '<td>' + item.Ratios + '</td>';//onkeyup="obtTotalCartones(' + (cont) + ')"
              // 
              //  
               // html += '<td><a href="#" onclick="obtenerTallas(' + item.IdSummary + ');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Sizes"></a></td>';
                //html += '<td width="250"><button type="button" id="btnTallas" class="agregarTalla btn btn btn-success btn-xs">Tallas</button></td>';
                // html += '<td width="250"><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Delete</button></td>';
                html += '</tr>';
            });
            html += '</tbody> </table>';
           
            $('#regAssort').html(html);
        
           
            obtenerValoresTallas(namePack);
            obtener_bacth_Assort();
            if (idCargo === 1 || idCargo === 9) {
                if (cartonesFaltantes !== 0) {
                    formularioRegistroPallet(totalCartones, totalPiezas);
                }
                else {
                    formularioRegistroPalletA(totalCartones, totalPiezas);
                }
                
            } else {
               formularioOpcionesCantPack(totalCartones, totalCartonesFaltantes);
            }
           // $("#consultaTalla").css("visibility", "visible");
           // $("#loading").css('display', 'none');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    }).done(function (data) {

    });
}


function formularioRegistroPallet(totalCartones, totalPiezas) {
    $("#div_titulo_Register_pallet").css('display', 'inline');
    $("#div_titulo_Register_pallet").html("<h3>PALLET</h3>");
    $("#opcionesRegPallet").css('display', 'inline');
    $('#numeroTotalCart').val(totalCartones);
    $('#numeroTotalFaltCart').val(totalCartonesFaltantes);
    $('#Packing_PackingTypeSize_Pieces').val(totalPiezas);
    $('#Packing_PackingAssort_CantCartons').val(0);
    $('#Packing_PackingAssort_TotalPiezas').val(0);
    $('#Packing_PackingAssort_TotalPiezasFalt').val(totalPiezasFaltantes);
    $("#nuevoPalletAssort").show(); 
}


function formularioRegistroPalletA(totalCartones, totalPiezas) {
    $("#div_titulo_Register_pallet").css('display', 'inline');
    $("#div_titulo_Register_pallet").html("<h3>PALLET</h3>");
    $("#opcionesRegPallet").css('display', 'inline');
    $('#numeroTotalCart').val(totalCartones);
    $('#numeroTotalFaltCart').val(totalCartonesFaltantes);
    $('label[for="Packing_PackingTypeSize_Pieces"]').hide();
    $('#Packing_PackingTypeSize_Pieces').hide();
    $('label[for="Packing_PackingAssort_CantCartons"]').hide();
    $('#Packing_PackingAssort_CantCartons').hide();
    $('label[for="Packing_PackingAssort_TotalPiezas"]').hide();
    $('#Packing_PackingAssort_TotalPiezas').hide();
    $('#Packing_PackingAssort_TotalPiezasFalt').val(totalPiezasFaltantes);
    $("#nuevoPalletAssort").hide();
    $('#nuevoPalletAssort').prop("disabled", true);
    $(".turn").css('display', 'none');
   /* $('label[for="Packing_PackingAssort_Turnos"]').hide();
    $('#Packing_PackingAssort_Turnos').hide();*/
}

function formularioOpcionesCantPack(totalCartones, totalCartonesFaltantes) {

    $("#opcionesCantidadesPack").css('display', 'inline');
    $('#nTotalCart').attr('value', totalCartones);
    $('#nTotalFaltCart').attr('value', totalCartonesFaltantes);
    $('#nTotalPcsFalt').attr('value', totalPiezasFaltantes);


}

$(document).on("click", "#nuevoPalletAssort", function () {
    var error = 0;
    var block = numBlock;
    var numCart = $("#Packing_PackingAssort_CantCartons").val();
    var numTotalCart = $("#numeroTotalCart").val();
    var nTotalCart = $("#numeroTotalFaltCart").val();
    var totalPcs = $("#Packing_PackingAssort_TotalPiezas").val();
    var nombrePack = $("#selectPackingName option:selected").val();
    var totalPiezasFalt = $('#Packing_PackingAssort_TotalPiezasFalt').val();
    var resultadoCartones = parseInt(nTotalCart) - parseInt(numCart);
    var cantidadFinalCart = $('#numeroTotalFaltCart').val(parseInt(resultadoCartones));
    var resultadoPiezas = parseInt(totalPiezasFalt) - totalPcs;
    var cantidadFinalPiezas = $('#Packing_PackingAssort_TotalPiezasFalt').val(parseInt(resultadoPiezas));
    if (numCart === "0" || numCart === "") {
        error++;
        $('#Packing_PackingAssort_CantCartons').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingAssort_CantCartons').css('border', '');
    }

    if (parseInt(resultadoCartones) === 0 ) {
        $('label[for="Packing_PackingTypeSize_Pieces"]').hide();
        $('#Packing_PackingTypeSize_Pieces').hide();
        $('label[for="Packing_PackingAssort_CantCartons"]').hide();
        $('#Packing_PackingAssort_CantCartons').hide();
        $('label[for="Packing_PackingAssort_TotalPiezas"]').hide();
        $('#Packing_PackingAssort_TotalPiezas').hide();
        $(".turn").css('display', 'none');
        $("#nuevoPalletAssort").hide();
    }

    var tipoTurno = $("#Packing_PackingAssort_Turnos option:selected").val();
    if (tipoTurno === "0") {
        error++;
        $('#Packing_PackingAssort_Turnos').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingAssort_Turnos').css('border', '');
    }   
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        var actionData = "{'TipoTurnoID':'" + tipoTurno + "','NumCartones':'" + numCart + "','numTotalP':'" + totalPcs + "','packName':'" + nombrePack + "','numBlock':'" + block + "','idPedido':'" + parseInt(idPedido) + "'}";
        $.ajax({
            url: "/Packing/Obtener_Lista_Tallas_Packing_Assort_Pallet/",
            type: 'POST',
            data: actionData,
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (jsonData) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The pallet was correctly registered.', 'success', 5, null);
                $("#Packing_PackingAssort_CantCartons").val(0);
                $("#Packing_PackingAssort_TotalPiezas").val(0);
                $('#Packing_PackingAssort_Turnos').val(0); 
                obtener_bacth_Assort();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        }).done(function (data) {
        });
    }
});

function obtener_bacth_Assort() {
    $("#batch").css('display', 'inline');
    var tempScrollTop = $(window).scrollTop();
    var actionData = "{'numBlock':'" + numBlock + "','idPedido':'" + parseInt(idPedido) + "'}";
    //  $("#loading").css('display', 'inline');
    $.ajax({
        url: "/Packing/Lista_Batch_Assort/",
        type: 'POST',
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var html = '';
            var estilos = jsonData.Data.estilos;
            var lista_batch = jsonData.Data.listaTalla;
            if (estilos !== '') {
                $("#div_batch_assort").html("<h3>BATCH REVIEW</h3>");
            } else {
                //$("#div_estilo_batch").hide();
            }
            html += '<table class="table table-sm table-striped table-hover" id="tablaBatchAssort"><thead>';
            html += '<tr> <th>   </th>';
            var tipoEmpaque;
            html += '<th> Cartons </th>';
            html += '<th> Pieces </th>';
            html += '<th> Type Packing </th>';
            html += '<th> User </th>';
            html += '<th> Turn </th>';
            html += '<th> User Modif </th>';
            //html += '<th> Actions </th>';
            html += '</tr>';

            html += '</thead><tbody>';
            $.each(lista_batch, function (key, item) {
                html += '<tr><td>Pallet-' + item.IdBatch + '</td>';
                html += '<td>' + item.CantCartons + '</td>';   
                html += '<td>' + item.TotalPiezas + '</td>'; 
                html += '<td>ASSORTMENT</td>';
                html += '<td>' + item.NombreUsr + '</td>';
                if (item.TipoTurno === 1) {
                    html += '<td>1rst Turn</td>';
                } else {
                    html += '<td>2nd Turn</td>';
                }
                html += '<td>' + item.NombreUsrModif + '</td>';
                html += '</tr>';
            });
            if (Object.keys(lista_batch).length === 0) {
                html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No batches were found for the style.</td></tr>';
            }
            html += '</tbody> </table>';
            $('#listaBatchAssort').html(html);
            $('#listaBatchAssort').show();
            //$("#div_titulo").css("visibility", "visible");
            // $("#loading").css('display', 'none');
      

        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}

var EstiloID;
var listCantTalla;
var listaPsc;
function obtenerTallas(IdSummary) {
    EstiloID = IdSummary;
    var actionData = "{'estiloId':'" + IdSummary +  "'}";
    $.ajax({
        url: "/Packing/Lista_Tallas_Empaque_Assort/",
        type: 'POST',
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var listaT = jsonData.Data.listaTalla;
            var listaPO = jsonData.Data.lista;           
            var listaPackingBox = jsonData.Data.listaPackingBox;
            listaPsc = jsonData.Data.listaPackingS;
            listCantTalla = jsonData.Data.listCantTalla;
            var html = '';
            
            html += '<table class="table" id="tablaTallasAssort"><thead>';
            html += '<tr><th>Size</th>' +
                ' <th>Ratio</th>' +
                ' <th>Pieces</th>' +
                ' <th></th>' +
                '</tr>' +
                '</thead><tbody>';
            var cont = 0;
            $.each(listaPO, function (key, item) {
              
                html += '<tr id="pallet' + cont + '" class="pallet">';
                html += '<td width="250"><input type="text" id="f-talla" class="form-control talla" value="' + item.Talla + '" readonly/></td>';
                html += '<td width="250"><input type="text" name="l-ratio" id="l-ratio" class="form-control numeric rat" value="' + 0 + '"  /></td>';
                html += '<td width="250"><input type="text" name="l-piecesAssort" id="l-piecesAssort'+cont+'" class="form-control numeric ratPieces" value="' + 0 + '"  readonly/></td>';
                html += '<td width="250"><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Delete</button></td>';
                html += '</tr>';
                cont = cont + 1;
            });
            html += '</tbody> </table>';
            html += '<button type="button" id="nuevoEmpaqueAssort" class="btn btn-success btn-md pull-right btn-sm"><span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span> ASSORTMENT</button>';
            $('#regAssort').html(html);
           // $('#tallasModal').modal('show'); 
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    }).done(function (data) {
    });
     
}

//Registrar tallas ASSORT
$(document).on("click", "#nuevoEmpaqueAssort", function () {
    var error = 0;
    var r = 0; var c = 0; var i = 0; var cadena = new Array(3);
    cadena[0] = ''; cadena[1] = ''; cadena[2] = '';
    var nFilas = $("#tablaTallasAssort tbody>tr").length;
    var nColumnas = $("#tablaTallasAssort tr:last td").length;
    $('#tablaTallasAssort tbody>tr').each(function () {
        r = 0;
        c = 0;
        $(this).find("input").each(function () {
            $(this).closest('td').find("input").each(function () {
                cadena[c] += this.value + "*";
                c++;
            });
            r++;
        });
    });
    var cadena_cantidades_psc = "";
    var cantidades_array_psc = "";
    $('#tablaTallasAssort tbody>tr').each(function () {
        $(this).find(".ratPieces").each(function () {
            $(this).closest('td').find(".ratPieces").each(function () {
                cadena_cantidades_psc += this.value + "*";
                cantidades_array_psc = cadena_cantidades_psc.split('*');
            });
        });
    });
    var cadena_cantidades_Talla = "";
    var cantidades_array_Talla = "";
    $.each(listCantTalla, function (key, item) {
        cadena_cantidades_Talla += item.Cantidad + "*";
        cantidades_array_Talla = cadena_cantidades_Talla.split('*');
    });
    var errorT = 0;
    $.each(listaPsc, function (key, item) {
        var sum = parseInt(cantidades_array_psc[i]) + parseInt(item.Calidad);
        var result = parseInt(cantidades_array_Talla[i]);
        var nombre = "#l-piecesAssort" + i;
        if (parseInt(sum) >= parseInt(result)) {
            errorT++;
            $(nombre).css('border', '2px solid #e03f3f');
        } else {
            $(nombre).css('border', '1px solid #cccccc');
        }
        i++;
    });
    $('#tablaTallasAssort').find('td').each(function (i, el) {
        var valor = $(el).children().val();
        if ($(el).children().val() === '' || $(el).children().val() === '0') {
            error++;
            $(el).children().css('border', '2px solid #e03f3f');

        } else {
            $(el).children().css('border', '1px solid #cccccc');
        }
    });

    var namePack = $("#Packing_PackingTypeSize_PackingName").val();
    if (namePack === "") {
        error++;
        $('#Packing_PackingTypeSize_PackingName').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_PackingName').css('border', '');
    }  

    var nameAssort = $("#Packing_PackingTypeSize_AssortName").val();
    if (nameAssort === "") {
        error++;
        $('#Packing_PackingTypeSize_AssortName').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_AssortName').css('border', '');
    } 
    var numQty = $("#Packing_PackingTypeSize_Cantidad").val();
    if (numQty === "" || numQty === "0") {
        error++;
        $('#Packing_PackingTypeSize_Cantidad').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_Cantidad').css('border', '');
    } 
    var numCart = $("#Packing_PackingTypeSize_Cartones").val();
    if (numCart === "" || numCart === "0") {
        error++;
        $('#Packing_PackingTypeSize_Cartones').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_Cartones').css('border', '');
    } 
    enviarListaTallaAssort(cadena, error, errorT);
});


function enviarListaTallaAssort(cadena, error, errorT) {
    var packName = $("#Packing_PackingTypeSize_PackingName").val();
    var nameAssort = $("#Packing_PackingTypeSize_AssortName").val();
    var numQty = $("#Packing_PackingTypeSize_Cantidad").val();
    var numCart = $("#Packing_PackingTypeSize_Cartones").val();
    var tUnits = $("#Packing_PackingTypeSize_TotalUnits").val();
       if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
       } else if (errorT !== 0) {
           var alertE = alertify.alert("Message", 'Review the quantities of the packaging exceed the order.').set('label', 'Aceptar');
           alertE.set({ transition: 'zoom' });
           alertE.set('modal', false);
       } else {
        $.ajax({
            url: "/Packing/Obtener_Lista_Tallas_Packing_Assort",
            datatType: 'json',
            data: JSON.stringify({
                ListTalla: cadena, EstiloID: EstiloID, PackingName: packName, AssortName: nameAssort, NumQty: numQty,
                NumCart: numCart, TotalUnidades: tUnits}),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The type of packaging is registered correctly.', 'success', 5, null);
                $("#packBPPK").hide();
                $("#packAssort").show();
                $("#opcionesAssort").css('display', 'inline');
                $("#consultaTalla").css("visibility", "visible"); 
                ConfirmEmpaqueAssort();
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
function obtenerValoresTallas(namePack){
    $("#loading").css('display', 'inline');
    $("#lTallasAssort").css('display', 'inline');
    var actionData = "{'numBlock':'" + numBlock + "','idPedido':'" + parseInt(idPedido) + "','namePack':'" + namePack + "'}";
    $.ajax({
        url: "/Packing/Lista_Tallas_Assort_Por_Estilo/",
        type: 'POST',
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var listaPO = jsonData.Data.listaPO;
            var listaCantidadesPO = jsonData.Data.lista;
            var listaRatios = jsonData.Data.listRatio;
            var nCartones = jsonData.Data.numCartones;
            var html = '';
            $("#div_titulo_Tallas_assort").html("<h3>QUALITY OF SIZES</h3>");
            html += '<tr> <th width="30%"> Size </th>';
            $.each(listaPO, function (key, item) {
                html += '<th>' + item.Talla + '</th>';
            });
            html += '<th width="30%"> Total </th>';
            html += '</tr><tr><td width="30%">1rst QTY</td>';
            var cantidades = 0;
            var cadena_cantidades = "";        

            $.each(listaCantidadesPO, function (key, item) {
                html += '<td class="calidad">' + item.Cantidad + '</td>';
                cantidades += item.Cantidad;
                cadena_cantidades += "*" + item.Cantidad;
            });
            var cantidades_array = cadena_cantidades.split('*');
            html += '<td>' + cantidades + '</td>';
            var cantidadesEmpAssort = 0;
            html += '</tr><tr><td width="30%">Packages Cartons# =' + nCartones +'</td>';
            $.each(listaRatios, function (key, item) {
                html += '<td>' + item.TotalPiezas + '</td>';
                cantidadesEmpAssort += item.TotalPiezas;
            });
            html += '<td>' + cantidadesEmpAssort + '</td>';
            var sumaTotal = 0;
            html += '</tr><tr><td width="30%">+/-</td>';
            var totales = 0;
            var i = 1;
            $.each(listaRatios, function (key, item) {
                if (listaRatios === 0) {
                    item.TotalPiezas = 0;
                }
                var resta = parseInt(cantidades_array[i]) - parseInt(item.TotalPiezas);
                html += '<td class="faltante">' + resta + '</td>';
                i++;
                sumaTotal += resta;
            });
            html += '<td>' + sumaTotal + '</td>';
            html += '</tr>';
            $('.tbodyTallasAssort').html(html);
            $("#loading").css('display', 'none');
    },
    error: function (errormessage) {
        alert(errormessage.responseText);
    }
        }).done(function (data) {

    });
}
