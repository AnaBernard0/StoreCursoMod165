using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StoreCursoMod165.Models
{
    public class Order
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Order º")]
        public string Number { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Time")]
        public DateTime Time { get; set; }

        [ForeignKey("CustomerID")]
        [ValidateNever]
        public Customer Customer { get; set; }

        [Display(Name = "Customer")]
        [Required]
        public int CustomerID { get; set; }

        [ForeignKey("ProductID")]
        [ValidateNever]
        public Product Product { get; set; }

        [Display(Name = "Product")]
        [Required]
        public int ProductID { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Informations")]
        public string Informations { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, 5000)]
        [Precision(18, 2)]
        [Required]
        public decimal TotalValue { get; set; }

        [ForeignKey("StatusID")]
        [ValidateNever]
        public Status Status { get; set; }

        [Display(Name = "Status")]
        [Required]
        public int StatusID { get; set; }


        [Required]
        [Display(Name = "IsPaid")]
        public bool IsPaid { get; set; } 
    }
}
