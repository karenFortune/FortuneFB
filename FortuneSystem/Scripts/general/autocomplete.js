$(document).ready(function () {

    //Autocomplete de Estilos Items
    $("#ItemDescripcion_ItemEstilo").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/POSummary/Autocomplete_Item_Estilo",
                type: "POST",
                dataType: "json",
                data: { keyword: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.Estilo,
                            value: item.Estilo,
                            descripcion: item.Descr,
                            id: item.Id

                        };
                    }));
                }
            });
        },
        select: function (event, ui) {
            var estilo = ui.item.value;
            var nuevoEstilo = estilo.trim();
            var numero = validarEstilo(nuevoEstilo);  
            var item = $("#ItemDescripcion_Descripcion").val(ui.item.descripcion);
            if (item !== "") {
                $('#nuevoEstilo').attr('disabled', true);
            } else {
                $('#nuevoEstilo').attr('disabled', false);
            }
        },
        minLength: 1,
        response: function (event, ui) {
            if (!ui.content.length) {               
                var alert = alertify.alert("Message", 'No style results found.').set('label', 'Aceptar');
                alert.set({ transition: 'zoom' });
                alert.set('modal', false);
            } 
        }
    });

    //Autocomplete codigo de color 
    $("#CatColores_CodigoColor").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/POSummary/Autocomplete_Color",
                type: "POST",
                dataType: "json",
                data: { keyword: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.CodigoColor,
                            value: item.IdColor,
                            color: item.Color,
                            id: item.Id
                        };
                    }));
                },
                error: function () {
                }
            });
        },
        select: function (event, ui) {
            var color = $("#CatColores_DescripcionColor").val(ui.item.color);
            $("#IdColor").val(ui.item.id);
            if (color !== "") {
                $('#nuevoColor').attr('disabled', true);
            } else {
                $('#nuevoColor').attr('disabled', false);
            }

        },
        minLength: 1,
        response: function (event, ui) {
            if (!ui.content.length) {
                var alert = alertify.alert("Message", 'No color results found.').set('label', 'Aceptar');
                alert.set({ transition: 'zoom' });
                alert.set('modal', false);
            }
        }
    });   


});