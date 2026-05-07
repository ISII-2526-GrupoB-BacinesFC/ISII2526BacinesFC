using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.Models
{
    public class Model
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del modelo es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Nombre del modelo")]
        public string NameModel { get; set; }
    }
}