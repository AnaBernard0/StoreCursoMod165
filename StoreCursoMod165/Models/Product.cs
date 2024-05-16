using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreCursoMod165.Models
{
    public class Product
    {
        public int ID { get; set; }

        [StringLength(255)]
        [Required]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, 5000)]
        [Precision(18, 2)]
        [Required]
        public decimal Price { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Weight")]
        public string Weight { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Quantity")]
        public string Quantity { get; set; }

        [ForeignKey("CategoryID")]
        [ValidateNever]
        public Category Category { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int CategoryID { get; set; }
    }
}
