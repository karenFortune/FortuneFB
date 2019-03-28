
$(document).ready(function () {
    $(function () {
        $('#selectGenero').change(function () {
            var text = $(this).find('#selectGenero option:selected').text();
            $.ajax('@Url.Action("List", "POSummary")', {
                data: { Genero: text },
                method: 'GET',
                success: function (result) {
                    var html = '';
                    html += '<table class="table" id="tablaTallas"><thead>' +
                        '  <tr>' +

                        '<th>Talla</th>' +
                        ' <th>Cantidad</th>' +
                        ' <th>Extras</th>' +
                        ' <th>Ejemplos</th>' +
                        '</tr>' +
                        '</thead><tbody>';
                    $.each(result, function (key, item) {
                        html += '<tr>';
                        //html += '<td><input type="text" name="talla" id="f-talla" class="form-control" /></td>';
                        html += '<td><input type="text" id="f-talla" class="form-control" value="' + item.CatTallaItem.Talla + '"/></td>';
                        html += '<td><input type="text" name="l-cantidad" id="l-cantidad" class="form-control l-name01 "  /></td>';
                        html += '<td><input type="text" name="e-extras" id="e-extras" class="form-control e-name01 " value="' + 0 + '"/></td>';
                        html += '<td><input type="text" name="s-ejemplo" id="s-ejemplo" class="form-control s-name01 "  value="' + 0 + '"/></td>';
                        html += '<td><button type="button" id="btnDelete" class="deleteTalla btn btn btn-danger btn-xs" value="4">Eliminar</button></td>';
                        html += '</tr>';

                    });
                    html += '</tbody> </table>';
                    $('#listaTalla').html(html);
                },
                error: function (errormessage) {
                    alert(errormessage.responseText);
                },
            }).done(function (data) {

            });
        });
    });

});