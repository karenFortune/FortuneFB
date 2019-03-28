
//Botones para la recepcion de PO
$("#siguienteInfoItems").on("click",function(){
	 $('.nav-tabs a[href="#items"]').tab('show');
   
});

$("#siguienteInfoTallas").on("click", function () {
    $('.nav-tabs a[href="#tallas"]').tab('show');

});

$("#anteriorPO").on("click",function(){
	 $('.nav-tabs a[href="#po"]').tab('show');
});

$("#anteriorItems").on("click", function () {
    $('.nav-tabs a[href="#items"]').tab('show');
});