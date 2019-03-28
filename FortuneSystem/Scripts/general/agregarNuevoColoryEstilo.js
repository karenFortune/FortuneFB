$(document).ready(function () {
    //Agregar nuevo color para Items
    $('#nuevoColor').click(function (e) {
        var colorEstilo = $("#CatColores_CodigoColor").val();
        var colorDesc = $("#CatColores_DescripcionColor").val();
        $.ajax('@Url.Action("Registrarcolor", "Colores")', {
            data: { CodigoColor: colorEstilo, DescColor: colorDesc },
            method: 'GET'
        }).done(function (data) {
        });
        $("#nuevoColor").prop("disabled", true);
        alertify.notify('Se registro correctamente el color.', 'success', 10, null);
    });

     //Agregar nuevo estilo para Items
    $('#nuevoEstilo').click(function (e) {
        var itemEstilo = $("#ItemDescripcion_ItemEstilo").val();
        var estiloDesc = $("#ItemDescripcion_Descripcion").val();
        $.ajax('@Url.Action("RegistrarEstilo", "Items")', {
            data: { ItemEstilo: itemEstilo, DescEstilo: estiloDesc },
            method: 'GET'
        }).done(function (data) {
        });
        $("#nuevoEstilo").prop("disabled", true);
        alertify.notify('Se registro correctamente el estilo.', 'success', 10, null);
    });
});