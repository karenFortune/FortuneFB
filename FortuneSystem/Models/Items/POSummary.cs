using FortuneSystem.Models.Pedidos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Catalogos;
using System.Web.ModelBinding;
using FortuneSystem.Models.Item;

namespace FortuneSystem.Models.POSummary
{

    public class POSummary
    {

        [Display(Name = "ID")]
        public int IdItems { get; set; }

        [Required(ErrorMessage = "Ingrese el estilo del Item.")]
        [Display(Name = "STYLE")]
        public string EstiloItem { get; set; }
        public int IdEstilo { get; set; }
        public virtual ItemDescripcion ItemDescripcion { get; set; }

        public List<ItemDescripcion> ListaItems { get; set; }

        [Required]
        [Display(Name = "COLOR")]
        public string IdColor { get; set; }
        public int ColorId { get; set; }
        public virtual CatColores CatColores { get; set; }

        public List<CatColores> ListaColores { get; set; }

        [Required]
        [Display(Name = "QTY")]
        public int Cantidad { get; set; }
        [Required]
        [Display(Name = "QTY")]
        public string CantidadT { get; set; }

        [Required]
        [Display(Name = "PRICE")]
        [RegularExpression("[0-9]\\d{0,9}(\\.\\d{1,3})?%?$", ErrorMessage = "The Price must contain only numbers(0.35 o 2.5)")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        // [DisplayFormat(DataFormatString = "{0:n2}")]
        public double Precio { get; set; }

        [Required]
        [Display(Name = "PRICE")]
        public string Price { get; set; }
        [Display(Name = "PO#")]
        [Column("ID_PEDIDOS")]
        [ForeignKey("PO_SUMMARY")]
        public virtual int PedidosId { get; set; }
        public virtual OrdenesCompra Pedidos { get; set; }

        [Display(Name = "GENDER")]
        [Column("ID_GENDER")]
        [ForeignKey("ID_GENDER")]

        public virtual string IdGenero { get; set; }
        public virtual CatGenero CatGenero { get; set; }
        public List<CatGenero> ListaGeneros { get; set; }

        public List<CatGenero> ListarTallasPorGenero { get; set; }

        public List<ItemTalla> ListarTallasPorEstilo { get; set; }

        public virtual ItemTalla ItemTalla { get; set; }
        public int Id_Genero { get; set; }
        [Display(Name = "FABRIC")]
        [Column("ID_TELA")]
        [ForeignKey("ID")]
        public int IdTela { get; set; }
        public List<CatTela> ListaTelas { get; set; }
        public virtual CatTela CatTela { get; set; }
        [Display(Name = "TYPE OF SHIRT")]
        public string TipoCamiseta { get; set; }
        public int IdCamiseta { get; set; }
        public CatTipoCamiseta CatTipoCamiseta { get; set; }


        public List<CatTipoCamiseta> ListaTipoCamiseta { get; set; }

        public List<CatTallaItem> ListaTallas { get; set; }

        public CatEspecialidades CatEspecialidades { get; set; }
        public int IdEspecialidad { get; set; }
        public List<CatEspecialidades> ListaEspecialidades { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.####}")]
        [Display(Name = "TOTAL")]
        public string Total { get; set; }

        public List<OrdenesCompra> ListaPO { get; set; }
        public virtual IMAGEN_ARTE ImagenArte { get; set; }
        public string ExtensionArte { get; set; }
        public HttpPostedFileBase FileArte { get; set; }
        [Display(Name = "ART IMAGE")]
        public byte[] ImgArte { get; set; }

        public string NombreEstilo { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }







    }
}