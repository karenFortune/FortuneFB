function obtenerPalletBulkAct(batchID) {
    var r = 0; var c = 0; var i = 0; var cadena = new Array(6);
    cadena[0] = ''; cadena[1] = ''; cadena[2] = ''; cadena[3] = ''; cadena[4] = ''; cadena[5] = '';
    var nFilas = $("#tablaTallasPallet tbody>tr").length;
    var nColumnas = $("#tablaTallasPallet tr:last td").length;
    $('#tablaTallasPallet tbody>tr').each(function () {
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
    error = 0;
    $('#tablaTallasPallet').find('td.cBox').each(function (i, el) {
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
    enviarListaTallaPalletAct(cadena, error, batchID);
}

function obtenerPalletPPKAct(batchID) {
    var r = 0; var c = 0; var i = 0; var cadena = new Array(5);
    cadena[0] = ''; cadena[1] = ''; cadena[2] = ''; cadena[3] = ''; cadena[4] = '';
    var nFilas = $("#tablaTallasPallet tbody>tr").length;
    var nColumnas = $("#tablaTallasPallet tr:last td").length;
    $('#tablaTallasPallet tbody>tr').each(function () {
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
    $('#tablaTallasPallet').find('td.cBox').each(function (i, el) {
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


    var numCaja = $("#Packing_CantBox").val();
    if (numCaja === "") {
        error++;
        $('#Packing_CantBox').css('border', '2px solid #e03f3f');
    }
    else {
        $('#Packing_CantBox').css('border', '');
    }
    enviarListaTallaPalletAct(cadena, error, batchID);
}