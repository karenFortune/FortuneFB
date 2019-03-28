$(document).ready(function () {

$("#formPO").validate({
    rules:
        {
            PO: {
                required: true,
                validname: true,
                minlength: 4
            }
        },
    messages:
        {
            PO: {
                required: "Please Enter User Name",
                validname: "Name must contain only alphabets and space",
                minlength: "Your Name is Too Short"
            }
        },
    errorPlacement: function (error, element) {
        $(element).closest('.form-group').find('.help-block').html(error.html());
    },
    highlight: function (element) {
        $(element).closest('.form-group').removeClass('has-success').addClass('has-error');
    },
    unhighlight: function (element, errorClass, validClass) {
        $(element).closest('.form-group').removeClass('has-error').addClass('has-success');
        $(element).closest('.form-group').find('.help-block').html('');
    },

    submitHandler: function () {
  
        $.ajax({
            url: "/Pedidos/RegistrarPO",
            data: ("#formPO").serialize(),
            cache: false,
            type: 'POST',
            async: true,
            success: function (data) {
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });

        alert('ok');
    }
    });


});