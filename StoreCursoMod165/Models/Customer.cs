using System.ComponentModel.DataAnnotations;

namespace StoreCursoMod165.Models
{
    public class Customer
    {
        public int ID { get; set; }

        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Required]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateOnly Birthday { get; set; }
        public string Address { get; set; }

        public string City { get; set; }

        [StringLength(110)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Fiscal Number")]
        public string NIF { get; set; }

        [StringLength(20)]
        [Display(Name = "Customer Number")]
        public string CustomerNumber { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
