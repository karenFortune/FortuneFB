$(document).ready(function () {
    var ID = $("#IdPedido").val();
    buscar_estilos(ID);
    $("#div_tabla_packing").css('display', 'none');

});


$(document).on("click", "#btnDone", function () {
    window.location.reload();
});

function probar(id) {
    $('#tabless tr').on('click', function (e) {
        $('#tabless tr').removeClass('highlighted');
        $(this).addClass('highlighted');
    });   
}

$(document).on("dblclick", "#tabless tr", function () {
    var row = this.rowIndex;
    var numEstilo = $('#tabless tr:eq(' + row + ') td:eq(0)').html();
    //var estilo = $('#tabless tr:eq(' + row + ') td:eq(2)').html();
    obtenerListaTallas(numEstilo);
});

$(document).on("input", ".numeric", function () {
    this.value = this.value.replace(/\D/g, '');
});

//Autocomplete tallas
$(function () {
    var list_datalist = Array();
    $.ajax({
        url: '/Tallas/Lista_Tallas',
        type: 'GET',
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                var html = "";
                list_datalist.push(data[i].Talla);
            }
        }
    });
    var availableTags = list_datalist;
    $(document).on("focus keyup", "input.talla", function (event) {
        $(this).autocomplete({
            source: availableTags,
            select: function (event, ui) {
                event.preventDefault();
                this.value = ui.item.label;
            },
            focus: function (event, ui) {
                event.preventDefault();
                this.value = ui.item.label;
            }
        });
    });
});

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
                //html += '<td><a href="#" onclick="obtenerListaTallas(' + item.IdItems + ');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Sizes"></a></td>';
                html += '</tr>';
            });
            if (Object.keys(lista_estilo).length === 0) {
                html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No styles were found for the PO.</td></tr>';

            }
            $('.tbody').html(html);
            $("#div_estilos_orden").css("display", "inline"); 
            $(window).scrollTop(tempScrollTop);
        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
}

var estiloId;
var tipoEmp = "";
var datosPO = "";
var tUnidades = 0;
function obtenerListaTallas(EstiloId) {
    $("#loading").css('display', 'inline');
    $("#panelHotTopic").css('display', 'inline');
    
    estiloId = EstiloId;
    $.ajax({
        url: "/Packing/Lista_Tallas_HT_Por_Estilo/" + EstiloId,
        method: 'POST',
        dataType: "json",
        success: function (jsonData) {
            //var listaT = jsonData.Data.listaTalla;
            //var listaPacking = jsonData.Data.listaPackingS;
            var listaPO = jsonData.Data.lista;
            var listaPBulk = jsonData.Data.listaPTBulk;
            var listaEPPK = jsonData.Data.listaEmpPPK;
            var listaPPPK = jsonData.Data.listaPTPPK;
            var listaTCajas = jsonData.Data.listaTotalCajasPack;
            //var listaTCajas = jsonData.Data.listaCajasT;
            var html = '';
            var estilos = jsonData.Data.estilos;
            $("#btnAdd").hide();
            $("#nuevaTalla").hide();
               $("#div_estilo_ht").html("<h3>QUALITY OF SIZES</h3>");
                    html += '<tr> <th width="30%"> Size </th>';
                $.each(listaPO, function (key, item) {
                    html += '<th>' + item.Talla + '</th>';
                });
                html += '<th width="30%"> Total </th>';
                html += '</tr><tr><td width="30%">1rst QTY</td>';
                var cantidades = 0;
            var cadena_cantidades = "";
            var lPO = listaPO.length;         
              
                $.each(listaPO, function (key, item) {
                    html += '<td class="calidad">' + item.Cantidad + '</td>';
                    cantidades += item.Cantidad;
                    cadena_cantidades += "*" + item.Cantidad;
                });
                var cantidades_array = cadena_cantidades.split('*');
                html += '<td>' + cantidades + '</td>';

            var cantidadesEmpBulk = 0;
            html += '</tr><tr><td width="30%">Bulk - #Pieces</td>';
            var listaTBatchBulk = 0;
            $.each(listaTCajas, function (key, item) {
                listaTBatchBulk++;
            });
          
            $.each(listaPBulk, function (key, item) {
                if (listaTBatchBulk === 0) {
                    item = 0;
                    html += '<td>' + item + '</td>';
                } else {
                    html += '<td>' + item + '</td>';
                }                
                cantidadesEmpBulk += item;              
            });
            
            html += '<td>' + cantidadesEmpBulk + '</td>';
            html += '</tr><tr><td width="30%">Packages</td>';
            var cantidades_PBulk = "";          
            var listaTBatch = 0;
            $.each(listaTCajas, function (key, item) {
                listaTBatch++;
            });
            if (listaTBatch === 0) {
                listaTCajas = listaPBulk;
            } else {
                listaTCajas;
            }
            $.each(listaTCajas, function (key, item) {
                if (listaTBatch === 0) {
                    item = 0;
                    html += '<td class="qtyBulk">' + item + '</td>';
                } else {
                    html += '<td class="qtyBulk">' + item + '</td>';
                }                  
                cantidades_PBulk += "*" + item;
            });
            var cantidades_array_pbulk = cantidades_PBulk.split('*');
           
            var total = 0;            
            var cantidadesEmpPPK = 0;
            var listaTallasPPK = listaEPPK.lenght;
            var totalTallas = 0;
            
            var cont = 0;
            $.each(listaEPPK, function (key, item) {
                cont = cont + 1;
                    html += '</tr><tr><td width="30%">PPK - #Ratio- PO#' + item.NumberPO+'</td>';
                  
                $.each(item.ListaEmpaque, function (key, i) {
                    html += '<td>' + i.Ratio + '</td>';
                    cantidadesEmpPPK += i.Ratio;               
                });
                //html += '<td>' + cantidadesEmpPPK + '</td>';
                html += '</tr><tr id="empaque' + cont + '" class="empaque"><td width="30%">Packages</td>';
                $.each(item.ListaEmpaque, function (key, i) {

                       html += '<td class="qtyPPK">' + i.TotalRatio + '</td>';
             
                });
                html += '</tr>';
            });

            html += '<tr><td width="30%">+/-</td>';
            var totales = 0;
            var i = 1;           
            var sum = 0;
            $.each(listaTCajas, function (key, item) {
                var valorCalidad = $(".calidad").parent("tr").find("td").eq(i).text();
                var valorQtyBulk = $(".qtyBulk").parent("tr").find("td").eq(i).text();
                var valorQtyPPK = $(".qtyPPK").parent("tr").find("td").eq(i).text();                
                var qtyPO = parseInt(valorCalidad);             
                var qtyValorB = parseInt(valorQtyBulk);
                var qtyValorP = parseInt(valorQtyPPK);
                if (isNaN(qtyPO)) {
                    qtyPO = 0;
                }
                if (isNaN(qtyValorB)) {
                    qtyValorB = 0;
                }
                if (isNaN(qtyValorP)) {
                    qtyValorP = 0;
                }
                sum = qtyValorB + qtyValorP;
                if (isNaN(sum)) {
                    sum = 0;
                }
                var resta = sum - qtyPO;
                if (isNaN(resta)) {
                    resta = 0;
                }
               // var resta = (parseFloat(cantidades_array[i]) - parseFloat(item))
                if (resta === 0) {
                    html += '<td class="faltante" style="color:black;">' + resta + '</td>';
                } else if (resta >= 0) {
                    html += '<td class="faltante" style="color:blue;">' + resta + '</td>';
                } else {
                    html += '<td class="faltante" style="color:red;">' + resta + '</td>';
                }
               // html += '<td class="faltante" ></td>';
                i++;
            });
            html += '</tr>';             

            $('.tbodyPHT').html(html);
            
           /* var nColumnas = $("#tablePackingHT tr:last td").length;
            var totalRows = $("#tablePackingHT tr").length;
             for (var v = 1; v < lPO+1; v++) {
                 datosPO += "*" + $('#tablePackingHT tr:eq(1) td:eq(' + v + ')').html();
              }

            var temp = "";
            var arrayCantidades = new Array();
          
            var mArray = new Array();
            
            for (var z = 0; z < totalRows; z++) {
                var valor = $('#tablePackingHT tr:eq(' + z + ') td:eq(0)').html();
                var contener = "";
                if ( valor !== undefined) {
                    contener = valor.includes('Packages');
                }               
                if (contener === true) {
                    for (var j = 1; j < lPO+1; j++) {
                        temp += "*" + $('#tablePackingHT tr:eq(' + z + ') td:eq(' + j + ')').html();
                    }
                    arrayCantidades.push(temp);
                    temp = "";
                }                
            }
            mArray[arrayCantidades.lenght];
            for (var x = 0; x < arrayCantidades.length; x++) {               
                var cantidadesV = arrayCantidades[x].split("*");
                    mArray[x] = cantidadesV.map(function (x) {
                        return parseInt(x, 10);
                    });
            }

            var iDatosPO = datosPO.split("*");
            var resultPO = iDatosPO.map(function (x) {
                return parseInt(x, 10);
            });
            //var l = 0;
            var valorFalt = 0;
            var val = parseInt(resultPO.length);
            for (var l = 1; l < val; l++) {
                for (var r = 0; r < arrayCantidades.length; r++) {
                    var totalC = mArray[r][l] - resultPO[l];
                    resultPO[l] = totalC;
                }
                $('#tablePackingHT tr:eq(' + (totalRows - 1) + ') td:eq(' + l + ')').html(resultPO[l]);
                valorFalt = $(".faltante").parent("tr").find("td").eq(l).text();
                var qtyFalt = parseInt(valorFalt);

                if (qtyFalt === 0) {
                    $(".faltante").css('color', '1px solid black');
                } else if (qtyFalt >= 0) {
                    $(".faltante").css('color', '1px solid blue');
                } else {
                    $(".faltante").css('color', '1px solid #e03f3f');
                }
            }*/
          
            

            
            
            if (listaPBulk.length === 0 && listaEPPK.length === 0) {
                if (listaPBulk.length !== 0) {
                    $("#btnNext").prop("disabled", false);
                } else {
                    $("#btnNext").prop("disabled", true);
                }
                TallasEmpaqueBulkHT(EstiloId);
            } else {
                $("#grupoBotones").hide();                 
                $("#div_titulo_Register").css("display", "inline"); 
                $("#div_titulo_Register").html("<h3>REGISTRATION OF PALLET</h3>");
                $('label[for="Packing_CantBox"]').hide();
                $("#numeroCajas").hide();
                $('label[for="Packing_CantidadPPKS"]').hide();
                $("#Packing_CantidadPPKS").hide();
                $("#opcionesRegistro").css("display", "inline");
                $('label[id="numTotalUnitLabel"]').hide();
                $("#numTotalUnit").hide(); 
                $("#div_titulo_Bulk").css("display", "none");
                $("#opciones").css("display", "none");
                obtener_bacth_estilo(estiloId);
            }            
      
            $("#consultaTallaHT").css("visibility", "visible");
            $("#arte").css("display", "inline-block");
            obtenerImagenPNL(estilos);
            obtenerImagenArte(estilos);
           // $("#loading").css('display', 'none');
            setTimeout(function () { $("#loading").css('display', 'none'); }, 3000);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    }).done(function (data) {

    });
}

function TallasEmpaqueBulkHT(idEst) {
    var actionData = "{'idEst':'" + idEst + "'}";
    $.ajax({
        url: "/Packing/Lista_Tallas_Empaque_HT_Registrar_Por_Estilo/",
        type: 'POST',
        dataType: "json",
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        success: function (jsonData) {
            var listaT = jsonData.Data.listaTalla;
            var listaPO = jsonData.Data.lista;
            var listaPacking = jsonData.Data.listaPackingS;
            var html = '';
            $("#btnAddP").show();           
            $('#btnNuevoPPK').hide();
                $("#btnNuevo").prop("disabled", true);               
            $("#btnDone").prop("disabled", true);     
            $('label[id="numTotalUnitLabel"]').hide();
            $("#numTotalUnit").hide(); 
            $('#listaTallaBatchHT').css("display", "none"); 
            $('#listaBatchHT').css("display", "none");  
                $("#div_titulo_Bulk").html("<h3>REGISTRATION OF TYPE OF PACKAGING - BULK</h3>");
            $("#div_titulo_Bulk").css("display", "inline"); 
            $("#opciones").css("display", "inline");
                $("#div_titulo_Register").css("display", "none");
                $("#opcionesRegistro").css("display", "none");
                          
            
                html += '<table class="table" id="tablaTallasBulkHT"><thead>';
                html += '<tr><th>Size</th>' +
                    ' <th>QTY#</th>' +
                    ' <th>CARTONS 50PCS#</th>' +
                    ' <th>PARTIAL#</th>' +
                    ' <th>TOTALCARTONS#</th>' +
                    '</tr>' +
                '</thead><tbody>';
            var cont = 0;
            $.each(listaPO, function (key, item) {
                    cont = cont + 1;
                    html += '<tr id="pallet' + cont + '" class="pallet">';
                    html += '<td width="20%"><input type="text" id="f-talla" class="form-control talla" value="' + item.Talla + '" readonly/></td>';
                    html += '<td width="20%"><input type="text" name="l-cantidad" id="l-cantidad" class="form-control numeric qty " onkeyup="obtTotalCartones(' + cont + ')" value="' + 0 + '"  /></td>';
                    html += '<td width="20%"><input type="text" name="l-cartones" id="l-cartones" class="form-control numeric cart " value="' + 0 + '"  readonly/></td>';
                    html += '<td width="20%"><input type="text" name="l-partial" id="l-partial" class="form-control numeric part " value="' + 0 + '"  readonly/></td>';
                html += '<td width="20%"><input type="text" name="l-totCartones" id="l-totCartones" class="form-control numeric tcart " value="' + 0 + '"  readonly/></td>';
                   // html += '<td width="250"><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Delete</button></td>';
                    html += '</tr>';
                });
                html += '</tbody> </table>';    
                html += '<button type="button" id="nuevoEmpaqueBulkHT" class="btn btn-success btn-md pull-right btn-sm"><span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span> Save</button>';               
                $('#listaTallaPHT').html(html);
            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    }).done(function (data) {
    });
}
var cadenaCantidad = ""; 
function RegistrarEmpaqueBulkHT(nPO, tEmpaque) {
    if (nPO === "" || nPO === null) {
        nPO = 0;
    }
    var actionData = "{'estiloId':'" + estiloId + "','nPO':'" + nPO + "','tEmpaque':'" + tEmpaque + "'}";
    $.ajax({
        url: "/Packing/Lista_Tallas_Empaque_HT_Por_Estilo/",
        type: 'POST',
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var listaT = jsonData.Data.listaTalla;
            var listaPO = jsonData.Data.lista;
            var listaPacking = jsonData.Data.listaPackingS;
            var listaPackingBox = jsonData.Data.listaPackingBox;
            var html = '';          

            html += '<table class="table" id="tablaTallasPalletHT"><thead>';
            html += '<tr><th>Size</th>' +
                ' <th>Box#</th>' +
                ' <th>QTY</th>' +
                ' <th>TOTALCARTONS#</th>' +
                '</tr>' +
                '</thead><tbody>';
            if (listaPackingBox.length === 0) {
                listaPackingBox = listaPacking;
            } else {
                listaPackingBox;
            }
       
            if (listaPackingBox.length !== 0) {
            var totalTallas = listaPackingBox.length;
                var tallaTemp = listaPackingBox[0].Talla;
                var cajaTemp = 0;
                if (listaPackingBox[0].PackingM !== null) {
                    cajaTemp = parseFloat(listaPackingBox[0].PackingM.CantBox);
                }
                var cantTemp = parseFloat(listaPackingBox[0].Cantidad);
                var cartonsTemp = parseFloat(listaPackingBox[0].TotalCartones);
                var i = 0;
                var cont = 0;
            $.each(listaPackingBox, function (key, item) {
                    i++;
                if (tallaTemp === item.Talla) {
                    if (item.PackingM !== null)
                    {
                        cajaTemp += parseFloat(item.PackingM.CantBox);
                    } 
                   
                } else {
                    cadenaCantidad += cajaTemp + "*";
                    cont = cont + 1;
                    html += '<tr id="pallet' + cont + '" class="pallet">';
                    html += '<td width="20%"><input type="text" id="f-talla" class="form-control talla" value="' + tallaTemp + '" readonly/></td>';
                    if (cajaTemp === 0) {
                        html += '<td width="20%" class="bCajas"><input type="text" name="l-cajas" id="l-cajas" class="form-control numeric caja " onkeyup="obtTotalCartonesBulk(' + cont + ')"  value="' + cajaTemp + '"  /></td>';
                    } else {
                        html += '<td width="20%" class="bCajas"><input type="text" name="l-cajas" id="l-cajas" class="form-control numeric caja " value="' + cajaTemp+ '"  readonly/></td>';
                    }
                    html += '<td width="20%"><input type="text" name="l-cantidad" id="l-cantidad" class="form-control numeric qtyBox " value="' + cantTemp + '"  readonly/></td>';
                    html += '<td width="20%"><input type="text" name="l-cartons" id="l-cartons" class="form-control numeric cartones " value="' + cartonsTemp + '"  readonly/></td>';
                    html += '</tr>';
                    tallaTemp = item.Talla;
                    if (item.PackingM !== null) {
                        cajaTemp = parseFloat(item.PackingM.CantBox);
                    } 
                    cantTemp = parseFloat(item.Cantidad);
                    cartonsTemp = parseFloat(item.TotalCartones);
                }
                if (i === totalTallas) {
                    cadenaCantidad += cajaTemp + "*";
                    html += '<tr>';
                    html += '<td width="20%"><input type="text" id="f-talla" class="form-control talla" value="' + tallaTemp + '" readonly/></td>';
                    if (cajaTemp === 0) {
                        html += '<td width="20%" class="bCajas"><input type="text" name="l-cajas" id="l-cajas" class="form-control numeric caja " value="' + cajaTemp + '"  /></td>';
                    } else {
                        html += '<td width="20%" class="bCajas"><input type="text" name="l-cajas" id="l-cajas" class="form-control numeric caja " value="' + cajaTemp + '"  readonly/></td>';
                    }
                    html += '<td width="20%"><input type="text" name="l-cantidad" id="l-cantidad" class="form-control numeric qtyBox " value="' + cantTemp + '"  readonly/></td>';
                    html += '<td width="20%"><input type="text" name="l-cartons" id="l-cartons" class="form-control numeric cartones " value="' + cartonsTemp + '"  readonly/></td>';
                    html += '</tr>';
                }
                });
            }

            html += '</tbody> </table>';
            html += '<button type="button" id="guardarBulkHT" class="btn btn-success btn-md pull-right btn-sm"><span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span> Pallet</button>';
            $('#listaTallaPHT').html(html);
            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    }).done(function (data) {
    });
}
$(document).on("keyup", "input.cantCajas", function () {   
    obtTotalPiezasPPK();
});
var cadenaCantidadPPK = "";
function RegistrarEmpaquePPKHT(nPO, tEmpaque) {
    if (nPO === "" || nPO === null) {
        nPO = 0;
    } 
    var totalUnidades = 0;
    var actionData = "{'estiloId':'" + estiloId + "','nPO':'" + nPO + "','tEmpaque':'" + tEmpaque + "'}";
    $.ajax({
        url: "/Packing/Lista_Tallas_Empaque_HT_Por_Estilo/",
        type: 'POST',
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {
            var listaT = jsonData.Data.listaTalla;
            var listaPO = jsonData.Data.lista;
            var listaPacking = jsonData.Data.listaPackingS;
            var listaPackingBox = jsonData.Data.listaPackingBox;
            var html = '';

            html += '<table class="table" id="tablaTallasPalletHT"><thead>';
            html += '<tr><th>Size</th>' +
                ' <th>Ratio#</th>' +
                ' <th>Pieces#</th>' +
                ' <th>QtyPieces#</th>' +
                ' <th>TotalPieces#</th>' +
                '</tr>' +
                '</thead><tbody>';
            if (listaPackingBox.length === 0) {
                listaPackingBox = listaPacking;
            } else {
                listaPackingBox;
            }

            if (listaPackingBox.length !== 0) {
                var totalTallas = listaPackingBox.length;
                var tallaTemp = listaPackingBox[0].Talla;
                var cajaTemp = 0;
                var totalPiezas = 0;
                var piezasTemp = 0;                
                if (listaPackingBox[0] !== null) {
                    cajaTemp = parseFloat(listaPackingBox[0].Ratio);
                    tUnidades = parseFloat(listaPackingBox[0].TotalUnits);
                  
                 }
                if (listaPackingBox[0].PackingM !== null) {
                    piezasTemp = parseFloat(listaPackingBox[0].PackingM.TotalPiezas);
                }
               // var cantTemp = parseFloat(listaPackingBox[0].Cantidad);
                var i = 0;
                var cont = 0;
                $.each(listaPackingBox, function (key, item) {
                    i++;
                    if (tallaTemp === item.Talla) {                   
                          cajaTemp = parseFloat(item.Ratio);
                        if (item.PackingM !== null) {
                            piezasTemp += parseFloat(item.PackingM.TotalPiezas);
                        }

                    } else {
                        cadenaCantidad += cajaTemp + "*";
                        cont = cont + 1;
                        html += '<tr id="pallet' + cont + '" class="pallet">';
                        html += '<td width="20%"><input type="text" id="f-talla" class="form-control talla" value="' + tallaTemp + '" readonly/></td>';           
                        html += '<td width="20%"><input type="text"  name="l-ratio" id="l-ratio" class="form-control numeric ratio " onkeyup="obtTotalPiezasPPK(' + cont + ')"  value="' + cajaTemp + '"  readonly/></td>';                     
                        html += '<td width="20%"><input type="text" name="l-piezas" id="l-piezas" class="form-control numeric piezas " value="' + 0 + '"  readonly/></td>';
                        html += '<td width="20%"><input type="text" name="l-tpiezas" id="l-tpiezas" class="form-control numeric tpiezas " value="' + piezasTemp + '"  readonly/></td>';
                        html += '<td width="20%"><input type="text" name="l-cantTP" id="l-cantTP" class="form-control numeric cantTP " value="' + 0 + '"  readonly/></td>';
                        html += '</tr>';
                        if (item.PackingM !== null) {
                            piezasTemp = parseFloat(item.PackingM.TotalPiezas);
                           
                        }
                        tallaTemp = item.Talla;
                        cajaTemp = parseFloat(item.Ratio);
                    }
                    if (i === totalTallas) {
                        cadenaCantidad += cajaTemp + "*";
                        cont = cont + 1;
                        html += '<tr id="pallet' + cont + '" class="pallet">';
                        html += '<td width="20%"><input type="text" id="f-talla" class="form-control talla" value="' + tallaTemp + '" readonly/></td>';
                        html += '<td width="20%"><input type="text"  name="l-ratio" id="l-ratio" class="form-control numeric ratio " onkeyup="obtTotalPiezasPPK(' + cont + ')"  value="' + cajaTemp + '"  readonly/></td>';
                        html += '<td width="20%"><input type="text" name="l-piezas" id="l-piezas" class="form-control numeric piezas " value="' + 0 + '"  readonly/></td>';
                        html += '<td width="20%"><input type="text" name="l-tpiezas" id="l-tpiezas" class="form-control numeric tpiezas " value="' + piezasTemp + '"  readonly/></td>';
                        html += '<td width="20%"><input type="text" name="l-cantTP" id="l-cantTP" class="form-control numeric cantTP " value="' + 0 + '"  readonly/></td>';
                        html += '</tr>';
                    }
                });
            }
            html += '<tr><td width="20%">+/-</td>';
            html += '<td width="20%"></td>';
            html += '<td width="20%"></td>';
            html += '<td width="20%"></td>';
            html += '<td width="20%"><input type="text" name="l-cantTU" id="l-cantTU" class="form-control numeric cantTU " value="' + totalUnidades + '"  readonly/></td>';
            html += '</tr>';


            html += '</tbody> </table>';
            html += '<button type="button" id="guardarBulkHT" class="btn btn-success btn-md pull-right btn-sm"><span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span> Pallet</button>';
            $('#listaTallaPHT').html(html);
            $("#numTotalUnit").val(tUnidades);
           
           
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    }).done(function (data) {
    });
}

function ConfirmEmpaqueBulk(a) {
    var confirm = alertify.confirm('Confirmation', 'Do you want to register a new type of packaging Bulk ?', null, null).set('labels', { ok: 'Accept', cancel: 'Cancel' });
    confirm.set('closable', false); 
    confirm.set('onok', function () {
        //crearTallasBulk();
        limpiarFormBulk();
        $("#btnNuevo").prop("disabled", true);
        $("#btnNext").prop("disabled", true);
        $("#btnDone").prop("disabled", true);
        $("#nuevoEmpaqueBulkHT").prop("disabled", false);
       // alertify.success('The packaging was registered correctly.');
    });
    confirm.set('oncancel', function () { 

    });
}

function ConfirmEmpaquePPK(a) {
    var confirm = alertify.confirm('Confirmation', 'Do you want to register a new type of packaging PPK ?', null, null).set('labels', { ok: 'Accept', cancel: 'Cancel' });
    confirm.set('closable', false);
    confirm.set('onok', function () {
        //crearTallasBulk();
        limpiarFormPPK();
        $("#btnNuevoPPK").prop("disabled", true);
        $("#btnNext").prop("disabled", true);
        $("#btnDone").prop("disabled", true);
        $("#nuevoEmpaquePPKHT").prop("disabled", false);
        // alertify.success('The packaging was registered correctly.');
    });
    confirm.set('oncancel', function () {

    });
}

function limpiarFormBulk() {
    $('#tablaTallasBulkHT tbody>tr').each(function () {      
        $(this).find("input.qty").each(function () {
            $(this).closest('td').find("input.qty").each(function () {
              var valor= $(this).val(0);
            });        
        }); 
    });
    $('#tablaTallasBulkHT tbody>tr').each(function () {
        $(this).find("input.cart").each(function () {
            $(this).closest('td').find("input.cart").each(function () {
                var valor = $(this).val(0);
            });
        });
    });
    $('#tablaTallasBulkHT tbody>tr').each(function () {
        $(this).find("input.part").each(function () {
            $(this).closest('td').find("input.part").each(function () {
                var valor = $(this).val(0);
            });
        });
    });
    $('#tablaTallasBulkHT tbody>tr').each(function () {
        $(this).find("input.tcart").each(function () {
            $(this).closest('td').find("input.tcart").each(function () {
                var valor = $(this).val(0);
            });
        });
    });
    
    $('#Packing_PackingTypeSize_FormaEmpaque').val(0);
    $('#Packing_PackingTypeSize_NumberPO').val('');
}

function limpiarFormPPK() {
    $('#tablaTallasPPKHT tbody>tr').each(function () {
        $(this).find("input.qty").each(function () {
            $(this).closest('td').find("input.qty").each(function () {
                var valor = $(this).val(0);
            });
        });
    });
    //$('#Packing_PackingTypeSize_FormaEmpaque').val(0)
    $('#Packing_PackingTypeSize_NumberPO').val('');
    $("#Packing_PackingTypeSize_TotalUnits").val(0);
}
//Registrar tallas Bulk HT
//function crearTallasBulk() {
    $(document).on("click", "#nuevoEmpaqueBulkHT", function () {
        var r = 0; var c = 0; var i = 0; var cadena = new Array(5);
        cadena[0] = ''; cadena[1] = ''; cadena[2] = ''; cadena[3] = ''; cadena[4] = '';
        var nFilas = $("#tablaTallasBulkHT tbody>tr").length;
        var nColumnas = $("#tablaTallasBulkHT tr:last td").length;
        $('#tablaTallasBulkHT tbody>tr').each(function () {
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
        var error = 0;
        $('#tablaTallasBulkHT').find('td').each(function (i, el) {
            var valor = $(el).children().val();
            if ($(el).children().val() === '') {
                error++;
                $(el).children().css('border', '2px solid #e03f3f');

            } else {
                $(el).children().css('border', '1px solid #cccccc');
            }
        });
        var formaEmpaque = $("#Packing_PackingTypeSize_FormaEmpaque option:selected").val();
        if (formaEmpaque === "0") {
            error++;
            $('#Packing_PackingTypeSize_FormaEmpaque').css('border', '2px solid #e03f3f');
        }
        else {
            $('#Packing_PackingTypeSize_FormaEmpaque').css('border', '');
        }

        var numberPO = $("#Packing_PackingTypeSize_NumberPO").val();
        if (numberPO === "" ) {
            error++;
            $('#Packing_PackingTypeSize_NumberPO').css('border', '2px solid #e03f3f');
        }
        else {
            $('#Packing_PackingTypeSize_NumberPO').css('border', '');
        }

        enviarListaTallaBulkHT(cadena, error);
    });
//}


function enviarListaTallaBulkHT(cadena, error) {
    var idFormaP = $("#Packing_PackingTypeSize_FormaEmpaque option:selected").val();
    var idNumberPo = $("#Packing_PackingTypeSize_NumberPO").val();
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/Packing/Obtener_Lista_Tallas_Packing_Bulk_HT",
            datatType: 'json',
            data: JSON.stringify({ ListTalla: cadena, EstiloID: estiloId, FormaPackID: idFormaP, NumberPOID: idNumberPo }),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The type of packaging is registered correctly.', 'success', 5, null);
                $("#btnNuevo").prop("disabled", false);
                $("#btnNext").prop("disabled", false);
                $("#btnDone").prop("disabled", false);
                $("#nuevoEmpaqueBulkHT").prop("disabled", true);
                // obtenerListaTallas(estiloId);
            },
            error: function (xhr, ajaxOptions, thrownError) {
               
            }
        });
    }
}


$(document).on("click", "#btnNext", function () {
    $('#Packing_PackingTypeSize_NumberPO').val('');
    $("#Packing_PackingTypeSize_TotalUnits").val(0);
    TallasEmpaquePPKHT(estiloId);
});

$(document).on("click", "#btnNewP", function () {
    $('#Packing_PackingTypeSize_NumberPO').val('');
    $("#Packing_PackingTypeSize_TotalUnits").val(0);
    TallasEmpaqueBulkHT(estiloId);
});

function TallasEmpaquePPKHT(idEst) {
    var actionData = "{'idEst':'" + idEst + "'}";
    $.ajax({
        url: "/Packing/Lista_Tallas_Empaque_HT_Registrar_Por_Estilo/",
        type: 'POST',
        dataType: "json",
        data: actionData,
        contentType: "application/json;charset=UTF-8",
        success: function (jsonData) {
            var listaT = jsonData.Data.listaTalla;
            var listaPO = jsonData.Data.lista;
            var listaPacking = jsonData.Data.listaPackingS;
            var html = '';
                $("#btnAddP").show();
                $("#btnNuevoPPK").show();
                $("#btnNuevo").hide();
                $("#btnNuevo").prop("disabled", true);
                $("#btnNext").hide();
                $("#btnDone").prop("disabled", true);
                $("#btnNuevoPPK").prop("disabled", true);
                $('#Packing_PackingTypeSize_FormaEmpaque').hide();
            $('label[for="Packing_PackingTypeSize_FormaEmpaque"]').hide();
            $("#opciones").css("display", "inline");
            $('#listaTallaBatchHT').css("display", "none"); 
            $('#listaBatchHT').css("display", "none"); 
            $('#opcionTotal').css("display", "inline");    
            $('label[id="numTotalUnitLabel"]').hide();
            $("#numTotalUnit").hide(); 
                $("#div_titulo_Bulk").html("<h3>REGISTRATION OF TYPE OF PACKAGING - PPK</h3>");
                $("#div_titulo_Bulk").css("display", "inline"); 

                html += '<table class="table" id="tablaTallasPPKHT"><thead>';
                html += '<tr><th>Size</th>' +
                    ' <th>Ratio</th>' +
                    '</tr>' +
                    '</thead><tbody>';
                $.each(listaPO, function (key, item) {
                    html += '<tr>';
                    html += '<td width="250"><input type="text" id="f-talla" class="form-control talla" value="' + item.Talla + '" readonly/></td>';
                    html += '<td width="250"><input type="text" name="l-cantidad" id="l-cantidad" class="form-control numeric qty " value="' + 0 + '"  /></td>';
                  //  html += '<td width="250"><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Delete</button></td>';
                    html += '</tr>';
                });
                html += '</tbody> </table>';
                html += '<button type="button" id="nuevoEmpaquePPKHT" class="btn btn-success btn-md pull-right btn-sm"><span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span>Save</button>';
                $('#listaTallaPHT').html(html);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    }).done(function (data) {
    });
}
//Registrar tallas PPK

$(document).on("click", "#nuevoEmpaquePPKHT", function () {
    var r = 0; var c = 0; var i = 0; var cadena = new Array(2);
    cadena[0] = ''; cadena[1] = '';
    var nFilas = $("#tablaTallasPPKHT tbody>tr").length;
    var nColumnas = $("#tablaTallasPPKHT tr:last td").length;
    $('#tablaTallasPPKHT tbody>tr').each(function () {
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
    var error = 0;
    $('#tablaTallasPPKHT').find('td').each(function (i, el) {
        var valor = $(el).children().val();
        if ($(el).children().val() === '' ) {
            error++;
            $(el).children().css('border', '2px solid #e03f3f');

        } else {
            $(el).children().css('border', '1px solid #cccccc');
        }
    });

    

    var numberPO = $("#Packing_PackingTypeSize_NumberPO").val();
    if (numberPO === "") {
        error++;
        $('#Packing_PackingTypeSize_NumberPO').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_NumberPO').css('border', '');
    }

    var tUnits = $("#Packing_PackingTypeSize_TotalUnits").val();
    if (tUnits === "") {
        error++;
        $('#Packing_PackingTypeSize_TotalUnits').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_PackingTypeSize_TotalUnits').css('border', '');
    }

    enviarListaTallaPPKHT(cadena, error);
});


function enviarListaTallaPPKHT(cadena, error) {
    var idNumberPo = $("#Packing_PackingTypeSize_NumberPO").val();
    var totalUnits = $("#Packing_PackingTypeSize_TotalUnits").val();
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/Packing/Obtener_Lista_Tallas_Packing_PPK_HT",
            datatType: 'json',
            data: JSON.stringify({ ListTalla: cadena, EstiloID: estiloId, NumberPOID: idNumberPo, NumberTotU: totalUnits }),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The type of packaging is registered correctly.', 'success', 5, null);
                $("#btnNext").prop("disabled", false);
                $("#btnDone").prop("disabled", false);
                $("#btnNuevoPPK").prop("disabled", false);                
                $("#nuevoEmpaquePPKHT").prop("disabled", true);
                //obtenerListaTallas(estiloId);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                
            }
        });
    }
}

$(function () {
    $("#btnAddP").hide();
    $("#nuevoEmpaque").hide();
    $("#nuevoEmpaquePPK").hide();
    $("#registrarNuevo").hide();
    $('#TipoEmp').change(function () {
        var selectedText = $(this).find("option:selected").text();
        var selectedValue = $(this).val();
        var html = '';
        var npo = $("#numeroPOHT").val();
        var tEmpaque = $("#TipoEmp option:selected").val();
        
        if (selectedValue === "1") {
            $('label[for="Packing_CantBox"]').hide();
            $("#numeroCajas").hide();
            $('label[for="Packing_CantidadPPKS"]').hide();
            $("#Packing_CantidadPPKS").hide();
            $('label[id="numTotalUnitLabel"]').hide();
            $("#numTotalUnit").hide(); 
            if (npo === "") {
                npo = 0;
            }
            RegistrarEmpaqueBulkHT(npo, tEmpaque);
            
            
        } else if (selectedValue === "2") {
            $('label[for="Packing_CantBox"]').show();
            $("#numeroCajas").show();
            $("#numeroCajas").val(0);
            $('label[for="Packing_CantidadPPKS"]').show();
            $("#Packing_CantidadPPKS").show();
            $("#Packing_CantidadPPKS").val(0); 
            $('label[id="numTotalUnitLabel"]').show();
            $("#numTotalUnit").show();
            $("#numTotalUnit").val(0);
            if (npo === "") {
                npo = 0;
            }
            RegistrarEmpaquePPKHT(npo, tEmpaque);
            
        }
    });
});

$(document).on('change', '#numeroPOHT', function () {
    var npo = $("#numeroPOHT").val();
    if (npo === "") {
        npo = 0;
    }
    var tEmpaque = $("#TipoEmp option:selected").val();
    if (tEmpaque === "1") {
        RegistrarEmpaqueBulkHT(npo, tEmpaque);
    } else if (tEmpaque === "2") {         
        RegistrarEmpaquePPKHT(npo, tEmpaque);
    }
    
});

$(document).on("click", "#guardarBulkHT", function () {
    var tEmpaque = $("#TipoEmp option:selected").val();
    if (tEmpaque === "1") {
        obtenerPalletBulkHT();
    } else if (tEmpaque === "2") {
        obtenerPalletBulkHT();
    }
       
});
function limpiarTableBulkHT() {
    $('#tablaTallasPalletHT tbody>tr').each(function () {
        $(this).find("input.qty").each(function () {
            $(this).closest('td').find("input.qty").each(function () {
                var valor = $(this).val(0);
            });
        });
    });
    $('#TipoEmp').val(0);
    $('#Packing_Turnos').val(0);    
    $('#numeroPOHT').val('');
    $("#tablaTallasPalletHT").hide();
    $("#guardarBulkHT").hide();
    
}
function obtenerPalletBulkHT() {
    cadenaCantidad;
    var cantidades = cadenaCantidad.split("*");
    var r = 0; var c = 0; var i = 0; var x = 0; var cadena = new Array(5);
    cadena[0] = ''; cadena[1] = ''; cadena[2] = ''; cadena[3] = ''; cadena[4] = '';
    var nFilas = $("#tablaTallasPalletHT tbody>tr").length;
    var nColumnas = $("#tablaTallasPalletHT tr:last td").length;
    $('#tablaTallasPalletHT tbody>tr').each(function () {
        r = 0;
        c = 0;
        $(this).find("input").each(function () {
            $(this).closest('td').find("input").each(function () {
                if (c === 1) {
                    if (cantidades[x] === "0") {
                        cadena[c] += this.value + "*";
                    } else {
                        cadena[c] += 0 + "*";
                    }
                    x++;
                } else {
                    cadena[c] += this.value + "*";
                }                  
                c++;
            });
            r++;
        });
    });
    error = 0;
    $('#tablaTallasPalletHT').find('td.cBox').each(function (i, el) {
        var valor = $(el).children().val();
        if ($(el).children().val() === '' || $(el).children().val() === '0') {
            error++;
            $(el).children().css('border', '2px solid #e03f3f');

        } else {
            $(el).children().css('border', '1px solid #cccccc');
        }
    }); 
    $('#tablaTallasPalletHT').find('td.bCajas').each(function (i, el) {
        var valor = $(el).children().val();
        if ($(el).children().val() === '') {
            error++;
            $(el).children().css('border', '2px solid #e03f3f');

        } else {
            $(el).children().css('border', '1px solid #cccccc');
        }
    });
    var tipoTurno = $("#Packing_Turnos option:selected").val();
    if (tipoTurno === "0") {
        error++;
        $('#Packing_Turnos').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_Turnos').css('border', '');
    }

    var tEmpaque = $("#TipoEmp option:selected").val();
    if (tEmpaque === "0") {
        error++;
        $('#TipoEmp').css('border', '2px solid #e03f3f');
    }
    else {
        $('#TipoEmp').css('border', '');
    }

    var npo = $("#numeroPOHT").val();
    if (npo === "") {
        error++;
        $('#numeroPOHT').css('border', '2px solid #e03f3f');
    }
    else {
        $('#numeroPOHT').css('border', '');
    }    

    enviarListaTallaPalletHT(cadena, error);
}

function enviarListaTallaPalletHT(cadena, error) {
    var idTipoTurno = $("#Packing_Turnos option:selected").val();
    var numPO = $("#numeroPOHT").val();
    var tipoEmpaque = $('#TipoEmp option:selected').text();
    var numPPK;
    var numCaja;
    if (tipoEmpaque === "PPK") {
        numPPK = $("#Packing_CantidadPPKS").val();
        numCaja = $("#numeroCajas").val();
    } else if (tipoEmpaque === "BULK") {
        numPPK = 0;
        numCaja = 0;
    }
    if (error !== 0) {
        var alert = alertify.alert("Message", 'All fields are required.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
    } else {
        $.ajax({
            url: "/Packing/Obtener_Lista_Tallas_Packing_HT_Pallet",
            datatType: 'json',
            data: JSON.stringify({
                ListTalla: cadena, EstiloID: estiloId, TipoTurnoID: idTipoTurno, NumeroPO: numPO, TipoEmpaque: tipoEmpaque,
                NumeroCaja: numCaja, NumeroPPK: numPPK
            }),
            cache: false,
            type: 'POST',
            contentType: 'application/json',
            success: function (data) {
                alertify.set('notifier', 'position', 'top-right');
                alertify.notify('The packaging was registered correctly.', 'success', 5, null);
                obtenerListaTallas(estiloId);
                limpiarTableBulkHT();
                

            },
            error: function (xhr, ajaxOptions, thrownError) {
                /*showError(xhr.status, xhr.responseText);
                if (data.error === 1) {
                    alertify.notify('Check.', 'error', 5, null);
                }*/
            }
        });
    }
}

function obtener_bacth_estilo(IdEstilo) {
    var tempScrollTop = $(window).scrollTop();
    //  $("#loading").css('display', 'inline');
    $.ajax({
        url: "/Packing/Lista_Batch_HT_Estilo/",
        type: "POST",
        data: JSON.stringify({ id: IdEstilo }),
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (jsonData) {

            var lista_batch = jsonData.Data.listaPO;
            var numBatch = 0;
            $.each(lista_batch, function (key, item) {
                numBatch++;
            });
            if (numBatch === 0) {
                // $("#div_tabla_talla").hide();
            } else {
                var html = '';
                var estilos = jsonData.Data.estilos;
                if (estilos !== '') {
                    $("#div_titulo_Bulk").html("<h3>BATCH REVIEW </h3>");
                    $("#div_titulo_Bulk").css("display", "inline"); 
                } else {
                    //$("#div_estilo_batch").hide();
                }
                html += '<table class="table table-sm table-striped table-hover" id="tablaTallasBulk"><thead>';
                html += '<tr> <th>   </th>';
                var tipoEmpaque;
                $.each(lista_batch, function (key, item) {
                    size = item.Batch;
                    tipoEmpaque = item.TipoEmpaque;
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

                // 
                //html += '<th> Total </th>';
       
                html += '<th> Box# </th>';               
                html += '<th> Type Packing </th>';
                html += '<th> PO# </th>';
                html += '<th> User </th>';
                html += '<th> Turn </th>';
                html += '<th> User Modif </th>';
                //html += '<th> Actions </th>';
                html += '</tr>';

                html += '</thead><tbody>';
                $.each(lista_batch, function (key, item) {
                    html += '<tr><td>Pallet-' + item.IdBatch + '</td>';

                    var cantidad = 0;
                    if (item.TipoEmpaque === 1) {
                        $.each(item.Batch, function (key, i) {
                            html += '<td class="total" >' + i.CantBox + " BOX" +'</td>';             
                        });
                    } else {
                        $.each(item.Batch, function (key, i) {
                            html += '<td class="total" >' + i.TotalPiezas + " PCS" +'</td>';
                            //html += '<td class="total" >' + i.TotalPiezas + '</td>';               
                        });
                    }
                    if (item.TipoEmpaque === 2) {
                        $.each(item.Batch, function (key, i) {
                            if (key === 1) {
                                html += '<td>' + i.CantBox + '</td>';
                            }

                        });
                    } else {
                        $.each(item.Batch, function (key, i) {
                            if (key === 1) {
                                html += '<td>-</td>';
                            }

                        });
                    }

                    if (item.TipoEmpaque === 1) {
                        html += '<td>BULK</td>';
                    } else if (item.TipoEmpaque === 2) {
                        html += '<td>PPK</td>';
                    } else {
                        html += '<td>ASSORTMENT</td>';
                    }
                    html += '<td>' + item.NumberPO + '</td>';
                    html += '<td>' + item.NombreUsr + '</td>';
                    if (item.TipoTurno === 1) {
                        html += '<td>1rst Turn</td>';
                    } else {
                        html += '<td>2nd Turn</td>';
                    }
                    html += '<td>' + item.NombreUsrModif + '</td>';

                   // html += '<td><a href="#" onclick="obtenerTallas_Batch(' + item.IdBatch + ',' + item.TipoTurno + ',' + item.IdPacking + ',' + item.TipoEmpaque /*+ ',\'' + item.Status + '\'*/ + ');" class = "btn btn-default glyphicon glyphicon-search l1s" style = "color:black; padding:0px 5px 0px 5px;" Title = "Details Bacth"></a></td>';
                    html += '</tr>';

                });
                if (Object.keys(lista_batch).length === 0) {
                    html += '<tr class="odd"><td valign="middle" align="center" colspan="10" class="dataTables_empty">No batches were found for the style.</td></tr>';

                }
                html += '</tbody> </table>';
                $('#listaTallaBatchHT').html(html);
                $('#listaTallaBatchHT').css("display", "inline");        
                // $("#loading").css('display', 'none');
                $(window).scrollTop(tempScrollTop);
            }


        },
        error: function (errormessage) { alert(errormessage.responseText); }
    });
    //calcular_Restantes();
}


