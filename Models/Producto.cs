using System.ComponentModel.DataAnnotations;

namespace CrudProductosOracle19c.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "el nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "maximo 100 caracteres!")]
        [Display(Name = "nombre del Producto")]
        public string Nombre { get; set; } = string.Empty;

        [Range(0.01 , 999999999, ErrorMessage = "el precio debe ser mayor que 0.")]
        [Display(Name = "precio (COP)")]
        public decimal Precio { get; set; }

        [Range(0, 1000000, ErrorMessage = "el stock no puede ser negativo!")]
        [Display(Name = "unidades disponibles")]
        public int Stock { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "categoria")]
        public string Categoria { get; set; } = "general";

        [Display(Name = "fecha de creacion")]
        public DateTime FechaCreacion { get; set; }
    }
}
