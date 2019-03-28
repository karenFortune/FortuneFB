function sumar() {
    var total = 0;
    //valor = parseInt(valor);
    var numPiezas = $(".tpieces").val();
    var numCartones = $(".tcart").val();
    total = parseInt(numPiezas) * parseInt(numCartones);
    $(".tcu").val(total);
}

function obtenerCantidades(listaPsc) {
    $.each(listaPsc, function (key, item) {
        item.TotalPieces;
    });
}

function sumarAssort() {
    var total = 0;
    //valor = parseInt(valor);
    var totalCart = $("#numeroTotalCart").val();
    var numPiezas = $(".cantTPcs").val();
    var numCartones = $(".cantCartons").val();
    var numCartonesFalt = $(".tcFalt").val();
    var numCartFalt = $("#numeroTotalFaltCart").val();
    var nombreC = ".cantCartons"; 
    var nombreCF = ".tcFalt";
    total = parseInt(numCartones) * parseInt(numPiezas);
    $(".totalPcs").val(total);

    if (parseInt(numCartones) > parseInt(numCartFalt)) {
            $(nombreC).css('border', '2px solid #e03f3f');
            var alertC = alertify.alert("Message", 'The number of cartons exceeds the total packaging.').set('label', 'Aceptar');
            alertC.set({ transition: 'zoom' });
            alertC.set('modal', false);
            $('#nuevoPalletAssort').prop("disabled", true);
        }
        else {
            $(nombreC).css('border', '1px solid #cccccc');
            $('#nuevoPalletAssort').prop("disabled", false);
    }
}

function calcular_Printed() {
    importe_total = 0;
    var error = 0;
    $(".print").each(
        function (index, value) {

            var input = $(this);
            var print = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numPrint = parseInt(print);
            var valores;
            importe_total = importe_total + eval($(this).val());

            if (numPrint > numPO) {
                input.css('border', '2px solid #e03f3f');
                var alert = alertify.alert("Message", 'The value of Print is greater than the missing PO or zero.').set('label', 'Aceptar');
                alert.set({ transition: 'zoom' });
                alert.set('modal', false);
                error++;       
            } 
            else {
                input.css('border', '1px solid #cccccc');
            }

        }
    );

    if (error !== 0) {
        $('#guardarBatch').prop("disabled", true);
        $('#modificarBatch').prop("disabled", true);
    } else {
        $('#guardarBatch').prop("disabled", false);
        $('#modificarBatch').prop("disabled", false);
    }

    $("#totalP").val(importe_total);
    CalcularTotal();
    CalcularTotalPNL();
    CalcularTotalBatchPNL();
    calcular_TotalG();
    CalcularTotalBatch();
    calcular_Restantes();
}

function calcular_TotalPiezas(index) {
    importe_total = 0;
    var error = 0;
      $(".cantBox").each(
        function (index, value) {

            var input = $(this);
            var cantidadBox = eval($(this).val());
           // var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPiezas = parseInt($("#l-piezas").val());
            var numCajas = parseInt(cantidadBox);
            var valores;
            importe_total = numCajas * numPiezas;

            /*if (numPrint > numPO || numPrint === 0) {
                input.css('border', '2px solid #e03f3f');
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }*/         

        }
    );
    $(".totalPiezas").val(importe_total);
  
}

function obtTotalMat(index) {
    var error = 0;
    var valorCalidad = $(".calidad").parent("tr").find("td").eq(index).text();
    var valorCajas = $(".cajasQty").parent("tr").find("td").eq(index).text();
    var valorFaltante = $(".faltante").parent("tr").find("td").eq(index).text();
    var faltanteQty = parseInt(valorFaltante) * -1;
    var input = $(".totalPiezas");
    var pCalidad = parseInt(valorCalidad);  
    var nombreT = "#pallet" + index + " .totalPiezas";
    var nombreC = "#pallet" + index + " .cantCajas";
    var nombreP = "#pallet" + index + " .cant"; 
    var nombreB = "#pallet" + index + " .totBox";
    var nombreTF = "#pallet" + index + " .totFaltantes";
    var nombrePartial = "#pallet" + index + " .totBoxPartial";
  
    var numCajas = parseInt($(nombreC).val()); 
    var totalCajas = $(nombreB).val();
    var nCajas = 0;
    var resta = 0;
    var numTotalBox = 0;
    var totalBox;
    var tot = 0;
    var totalPartial = 0;
    var totalPiezasGrl = 0;
    var resultado = 0;
    var numTotalCajas = $(nombreB).val();   
    if (numCajas <= totalCajas) {
        if (numCajas !== 0) {
            var numPiezas = $(nombreP).val();
            var cantBox = parseInt(numPiezas);
            if (faltanteQty < cantBox) {
                numTotalBox = faltanteQty / numPiezas;
                totalBox = Math.floor(numTotalBox);
                if (numTotalBox < 1) {
                    totalBox = 1;
                }
                var partial = parseInt(totalBox);
                $(nombreT).val(faltanteQty);

                $(nombrePartial).val(faltanteQty);
                nCajas = parseInt(valorCajas) + parseInt(numCajas);
                resta = parseInt(numTotalCajas) - nCajas;
                $(nombreTF).val(resta);
            } else {
                numTotalBox = faltanteQty / numPiezas;
                totalBox = Math.floor(numTotalBox);
                tot = numCajas * numPiezas;
                $(nombreT).val(tot);
                if (faltanteQty > parseInt(tot)) {
                    $(nombrePartial).val(0);
                    if ($(nombrePartial).val() === "0") {
                        $(nombrePartial).prop('readonly', true);
                    } else {
                        $(nombrePartial).prop('readonly', false);
                    }
                    tot = numCajas * numPiezas;
                    resultado = parseInt(tot);
                    $(nombreT).val(resultado);
                    if (parseInt(valorCajas) === 0) {
                        nCajas = parseInt(valorCajas) + parseInt(numCajas);
                    } else {
                        nCajas = parseInt(valorCajas);
                    }
                   
                   // resta = parseInt(numTotalCajas) - nCajas;
                    resta = numTotalCajas - numCajas;
                    $(nombreTF).val(resta);
                } else {                 
                        tot = (numCajas - 1) * numPiezas;
                        totalPiezasGrl = faltanteQty - tot;
                        resultado = parseInt(tot) + parseInt(totalPiezasGrl);
                    $(nombreT).val(resultado);
                  
                    if (numTotalBox % 1 === 0) {
                        $(nombrePartial).val(0);
                    } else {
                        $(nombrePartial).val(totalPiezasGrl);
                    }
                    
                    if ($(nombrePartial).val() === "0") {
                        $(nombrePartial).prop('readonly', true);
                    } else {
                        $(nombrePartial).prop('readonly', false);
                    }
                    if (faltanteQty > cantBox) {
                        //nCajas = parseInt(valorCajas) + parseInt(numCajas);
                        resta = numTotalCajas - numCajas;
                        $(nombreTF).val(resta);
                    } else {
                       // nCajas = parseInt(valorCajas) + parseInt(numCajas);
                       // resta = parseInt(numTotalCajas) - nCajas;
                        resta = numTotalCajas - numCajas;
                        $(nombreTF).val(resta);
                    }
                                      
                 
                }                
            }
        } else {
            $(nombreT).val(0);
            $(nombrePartial).val(0);
            if ($(nombrePartial).val() === "0") {
                $(nombrePartial).prop('readonly', true);
            } else {
                $(nombrePartial).prop('readonly', false);
            }
            $(nombreTF).val(totalCajas);
        }
        $(nombreC).css('border', '1px solid #cccccc');
        $("#nuevoPallet").prop('disabled', false);
    } else {
        var alert = alertify.alert("Message", 'The number of boxes exceeds the allowed boxes.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
        $(nombreC).css('border', '2px solid #e03f3f');
        $("#nuevoPallet").prop('disabled', true);
    }
    
    
    
    /*if (resta < 0) {
        $(nombreC).css('border', '2px solid #e03f3f');
        error++;
    } else {
        $(nombreC).css('border', '1px solid #cccccc');
       
    }

    if (error !== 0) {
        $("#nuevoPallet").prop('disabled', true);
    }
    else {
        $("#nuevoPallet").prop('disabled', false);
    }*/
    /*if (tot > pCalidad ) {
        $(nombreT).css('border', '2px solid #e03f3f');
        error++;
        $("#nuevoPallet").prop('disabled', true);
    } else {
        $(nombreT).css('border', '1px solid #cccccc');
        $("#nuevoPallet").prop('disabled', false);
    }*/
   
}

function ActualizarPiezasPackingBulk(index) {
    var error = 0;
    var valorCalidad = $(".calidad").parent("tr").find("td").eq(index).text();
    var pCalidad = parseInt(valorCalidad); 
    var nombreT = "#pallet" + index + " .totalPiezas";
    var nombrePartial = "#pallet" + index + " .totBoxPartial";
    var nombreP = "#pallet" + index + " .cant"; 
    var numPartial = $(nombrePartial).val();
    var totalPiezas = $(nombreT).val();
    var numTPiezas = 0;
    var resultado = 0;
    var numPiezas = $(nombreP).val();
    var numTotalBox = pCalidad / numPiezas;
    var totalBox = Math.floor(numTotalBox);
    var cantCajas = totalBox * numPiezas;
    var totalPiezasGrl = pCalidad - parseInt(cantCajas) ;

    if (numPartial > pCalidad || numPartial > totalPiezasGrl) {        
        var alert = alertify.alert("Message", 'The quantity of the partial exceeds the number of pieces of the order.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
        $(nombrePartial).css('border', '2px solid #e03f3f');
        $("#nuevoPallet").prop('disabled', true);
        error++;
    } else {
        $(nombrePartial).css('border', '1px solid #cccccc');
        $("#nuevoPallet").prop('disabled', false);
        numTPiezas = $(nombrePartial).val();
        resultado = parseInt(cantCajas) + parseInt(numTPiezas);
        $(nombreT).val(resultado); 
      
              
    }
}


function obtTotalCartones(index) {
    var error = 0;
    var valorCalidad = $(".calidad").parent("tr").find("td").eq(index).text();
    var input = $(".totalPiezas");
    var pCalidad = parseInt(valorCalidad);

    var nombreT = "#pallet" + index + " .cart";
    var nombreC = "#pallet" + index + " .qty";
    var nombrePart = "#pallet" + index + " .part";
    var tcartones = "#pallet" + index + " .tcart";
    var numCajas = $(nombreC).val();
    var tot = numCajas / 50;
    var tCajas = Math.floor(tot);
    var mult = tCajas * 50;
    var result = numCajas - mult;
    var descripcion = tCajas + "ctnx50" + "1x" + result;
    var numCartones = tCajas + 1;
    $(nombreT).val(tCajas);
    $(nombrePart).val(result);
    $(tcartones).val(numCartones);
    if (tot > pCalidad) {
        $(nombreT).css('border', '2px solid #e03f3f');
        error++;
        $("#nuevoPallet").prop('disabled', true);
    } else {
        $(nombreT).css('border', '1px solid #cccccc');
        $("#nuevoPallet").prop('disabled', false);
    }

}

function obtTotalPiezas(numBoxPPK) {
    var error = 0;
    var numFilas = $("#tablaTallasPallet tbody>tr").length;
    var valorQty = $(".calidad").parent("tr").find("td").eq(1).text();
    var valorRatio = $(".numRatio").parent("tr").find("td").eq(1).text();
    var pQty = parseInt(valorQty);
    var numRatio = parseInt(valorRatio);
    var numTotalCart = pQty / numRatio;
    var restar;
    if ($(".cantBox").val() === "") {
        $(".cantBox").val(0);
    }
    $("#Packing_TotalCartonsPPK").val(numTotalCart);
    if (parseInt(numBoxPPK) === 0) {
        $("#Packing_TotalCartonesFaltPPK").val(numTotalCart);
    } else {
        restar = parseInt(numTotalCart) - parseInt(numBoxPPK);
        $("#Packing_TotalCartonesFaltPPK").val(restar);
    } 
    for (var i = 1; i <= numFilas; i++) {
        var input = $(".totalPiezas");
        var nombreT = "#pallet" + i + " .totalPiezas";
        var nombreC = "#pallet" + i + " .cantBox";
        var nombreP = "#pallet" + i + " .cant";
        /*$('#tablaTallasPallet tr').each(function () {
            var valor = "td" + " #pallet" + i;
            var customerId = $(input).find(nombreT).eq(i).html();
            var kksj = $(nombreP).val();
        });*/
        var valorCalidad = $(".calidad").parent("tr").find("td").eq(i).text();

        var pCalidad = parseInt(valorCalidad);

        /* var nombreC = $(".cantBox").val();"#pallet" + i + " .cantBox";*/

        var numCajas = $(".cantBox").val(); //$(nombreC).val();
        var numPiezas = $(nombreP).val();
        var tot = numCajas * numPiezas;
        var numCartons = $("#Packing_TotalCartonsPPK").val();
        /*var numMBoxPPK = $("#Packing_TotalCartonesFaltPPK").val();
         */

        $(nombreT).val(tot);
        if (tot > pCalidad) {
            $(nombreT).css('border', '2px solid #e03f3f');
            error++;
            $("#nuevoPallet").prop('disabled', true);
        } else {
            $(nombreT).css('border', '1px solid #cccccc');
            $("#nuevoPallet").prop('disabled', false);
        }

        if (parseInt(numCajas) > restar) {
            $(".cantBox ").css('border', '2px solid #e03f3f');
            error++;
            $("#nuevoPallet").prop('disabled', true);
        } else {
            $(".cantBox ").css('border', '1px solid #cccccc');
            $("#nuevoPallet").prop('disabled', false);
        }

    }
    var numMBoxPPK = $("#Packing_TotalCartonesFaltPPK").val();
    //var resta = parseInt(numMBoxPPK) - parseInt(numBoxPPK);
    var resta = parseInt(numMBoxPPK) - parseInt(numCajas);
    if (resta !== 0) {
        $("#Packing_TotalCartonesFaltPPK").val(resta);
    }


}

function obtTotalPiezasRatioAssort() {
    var error = 0;
    var numFilas = $("#tablaTallasAssort tbody>tr").length;
    for (var i = 0; i <= numFilas; i++) {
        var nombreT = "#pallet" + i + " .ratPieces";
        var nombreC = "#pallet" + i + " .tcart";
        var nombreR = "#pallet" + i + " .rat";
        var numCajas = $(".tcart").val(); //$(nombreC).val();
        var numRatio = $(nombreR).val();
        var tot = parseInt(numCajas) * parseInt(numRatio);
        $(nombreT).val(tot);
    }


}

function obtTotalPiezasPPK() {
    var error = 0;
    var totalUnidades = 0;
    var valorUnidades = 0;
    var nTotalUnits = parseInt($("#numTotalUnit").val(), 10);
    var numFilas = $("#tablaTallasPalletHT tbody>tr").length;
    for (var i = 1; i <= numFilas-1; i++) {
        var input = $(".piezas");
        var nombreP = "#pallet" + i + " .piezas";
        var nombreR = "#pallet" + i + " .ratio";
        var nombreTP = "#pallet" + i + " .tpiezas";
        var totalG = "#pallet" + i + " .cantTP"; 

        /*$('#tablaTallasPallet tr').each(function () {
            var valor = "td" + " #pallet" + i;
            var customerId = $(input).find(nombreT).eq(i).html();
            var kksj = $(nombreP).val();
        });*/
        var valorCalidad = $(".calidad").parent("tr").find("td").eq(i).text();

        var pCalidad = parseInt(valorCalidad);

        /* var nombreC = $(".cantBox").val();"#pallet" + i + " .cantBox";*/
        var numCajas = $(".cantCajas").val(); //$(nombreC).val();
        var numPPK = $(".cantPPK").val();
        var numRatio = $(nombreR).val();
        var numPiezas = $(nombreP).val();
        var numTotalPiezas = $(nombreTP).val();
        var totalRatio = parseInt(numPPK) * parseInt(numRatio);       
        var tot = numCajas * totalRatio;
        var sumaTotalPiezas = parseInt(tot) + parseInt(numTotalPiezas);
        totalUnidades += parseInt(sumaTotalPiezas);
        $(nombreP).val(tot);
        $(totalG).val(sumaTotalPiezas);
            valorUnidades = parseInt(totalUnidades); 
    }
    $(".cantTU").val(parseInt(valorUnidades));
    var valtotalPiezas = parseInt($(".cantTU").val(), 10);
    if (valtotalPiezas > nTotalUnits ) {
        //$(nombreT).css('border', '2px solid #e03f3f');
        var alert = alertify.alert("Message", 'The total of pieces exceeds the number of units of the po for packaging.').set('label', 'Aceptar');
        alert.set({ transition: 'zoom' });
        alert.set('modal', false);
        error++;
        $("#guardarBulkHT").prop('disabled', true);
    } else {
        //  $(nombreT).css('border', '1px solid #cccccc');
        $("#guardarBulkHT").prop('disabled', false);
    }

}

function obtTotalCartonesBulk() {
    var error = 0;
    var numFilas = $("#tablaTallasPalletHT tbody>tr").length;
    for (var i = 1; i <= numFilas; i++) {
        var input = $(".piezas");
        var nombreCajas = "#pallet" + i + " .caja";
        var nombreCartones = "#pallet" + i + " .cartones";
        var valorCalidad = $(".calidad").parent("tr").find("td").eq(i).text();
        var pCalidad = parseInt(valorCalidad);

        var numCajas = $(nombreCajas).val();
        var numCartones = $(nombreCartones).val();       
        if (parseInt(numCajas) > parseInt(numCartones)) {
            $(nombreCajas).css('border', '2px solid #e03f3f');
            error++;
          //  var alert = alertify.alert("Message", 'The number of boxes must be less than the total number of cartons. '/* + numCartones +'.'*/).set('label', 'Aceptar');
          //  alert.set({ transition: 'zoom' });
          //  alert.set('modal', false);
           // $("#guardarBulkHT").prop('disabled', true);
         } else {
            $(nombreCajas).css('border', '1px solid #cccccc');
            //$("#guardarBulkHT").prop('disabled', false);
         }
    }

    if (error !== 0) {
        $("#guardarBulkHT").prop('disabled', true);
    } else {
        $("#guardarBulkHT").prop('disabled', false);
    }
     

}

function calcular_MisPrint() {
    importe_total = 0;
    var error = 0;
    $(".mp").each(
        function (index, value) {
            var input = $(this);
            var Misprint = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numMisPrint = parseInt(Misprint);
            var valores;
            importe_total = importe_total + eval($(this).val());
            if (numMisPrint > numPO) {
                input.css('border', '2px solid #e03f3f');
                var alert = alertify.alert("Message", 'The value of MisPrint is greater than the missing PO.').set('label', 'Aceptar');
                alert.set({ transition: 'zoom' });
                alert.set('modal', false);
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }
        }
    );

    if (error !== 0) {
        $('#guardarBatch').prop("disabled", true);
        $('#modificarBatch').prop("disabled", true);
    } else {
        $('#guardarBatch').prop("disabled", false);
        $('#modificarBatch').prop("disabled", false);
    }
    $("#totalM").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    CalcularTotalPNL();
    CalcularTotalBatchPNL();
    calcular_Restantes();
}

function calcular_Defect() {
    importe_total = 0;
    var error = 0;
    $(".def").each(
        function (index, value) {
            var input = $(this);
            var def = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numDefect = parseInt(def);
            var valores;
            importe_total = importe_total + eval($(this).val());

            if (numDefect > numPO ) {
                input.css('border', '2px solid #e03f3f');
                var alert = alertify.alert("Message", 'The value of Defect is greater than the missing PO.').set('label', 'Aceptar');
                alert.set({
                    transition: 'zoom'
                });
                alert.set('modal', false);
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }
        }
    );
    $("#totalD").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    CalcularTotalPNL();
    CalcularTotalBatchPNL();
    calcular_Restantes();
}

function calcular_Repair() {
    importe_total = 0;
    var error = 0;
    $(".rep").each(
        function (index, value) {
            var input = $(this);
            var rep = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numRepair = parseInt(rep);
            var valores;
            importe_total = importe_total + eval($(this).val());

            if (numRepair > numPO) {
                input.css('border', '2px solid #e03f3f');
                var alert = alertify.alert("Message", 'The value of Repair is greater than the missing PO.').set('label', 'Aceptar');
                alert.set({
                    transition: 'zoom'
                });
                alert.set('modal', false);
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }
        }
    );
    $("#totalR").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    CalcularTotalPNL();
    CalcularTotalBatchPNL();
    calcular_Restantes();
}

function calcular_TotalG() {
    importe_total = 0;
    var print = parseInt($("#totalP").val());
    var misPrint = parseInt($("#totalM").val());
    var defecto = parseInt($("#totalD").val());
    var repair = parseInt($("#totalR").val());
    importe_total = print + misPrint + defecto + repair;
    $("#totalF").val(parseInt(importe_total));

}



function CalcularTotalGeneral() {
    var sum = 0;
    $(".total").each(function () {
        sum += parseFloat($(this).text());
    });
    $('#sum').text(sum);
}
function CalcularTotal() {
    var sumQ = [];
    var nColumnas = $("#tablePrint tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFal").eq(i - 1).val(faltante);


        });

    }


}

function CalcularTotalPNL() {
    var sumQ = [];
    var nColumnas = $("#tablePnl tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFal").eq(i - 1).val(faltante);


        });

    }


}

function CalcularTotalBatch() {
    var sumQ = [];
    var nColumnas = $("#tablePrint tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFalt").eq(i - 1).val(valor);


        });

    }


}

function CalcularTotalBatchPNL() {
    var sumQ = [];
    var nColumnas = $("#tablePnl tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFalt").eq(i - 1).val(valor);


        });

    }


}

function calcular_Restantes() {
    var error = 0;
    $(".totalFal").each(
        function (index, value) {
            var input = $(this);
            var print = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numPrint = parseInt(print);
            var valores;


            if (numPO >= numPrint) {
                input.css('background-color', '#f97878');

            } 

            if (numPrint === 0) {
                input.css('background-color', '#eee');
                
            } 

        }
    );
}