$(document).on("click", ".classAdd", function () {
    var rowCount = $('.data-Talla').length + 1;
    var tallasdiv = '<tr class="data-Talla">' +
        '<td width="250"><input type="text"  name="f-talla" id="f-talla" class="form-control talla" autocomplete="off" /></td>' +
        '<td width="250"><input type="text" name="l-cantidad" id="l-cantidad" class="form-control numeric numCantTall"  /></td>' +
        '<td width="250"><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Delete</button></td>' +
        '</tr>';
    $('#tablaTallasPPK').append(tallasdiv);
});

$(document).on("click", ".classAdd", function () {
    var rowCount = $('.data-Talla').length + 1;
    var tallasdiv = '<tr class="data-Talla">' +
        '<td width="250"><input type="text"  name="f-talla" id="f-talla" class="form-control talla" autocomplete="off" /></td>' +
        '<td width="250"><input type="text" name="l-cantidad" id="l-cantidad" class="form-control numeric numCantTall"  /></td>' +
        '<td width="250"><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Delete</button></td>' +
        '</tr>';
    $('#tablaTallasBulk').append(tallasdiv);
});

$(document).on("click", ".deleteTalla", function () {
    $(this).closest("tr").remove();
});