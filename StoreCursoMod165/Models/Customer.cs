using System.ComponentModel.DataAnnotations;

namespace StoreCursoMod165.Models
{
    public class Customer
    {
        public int ID { get; set; }

        [StringLength(255)]
        [Display(Name="Name")]
        [Required]
        public string Name { get; set; }

        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Required]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [Display(Name="Birthday")]
        [Required]
        public DateOnly Birthday { get; set; }
        [Display(Name="Address")]
        public string Address { get; set; }
        [Display(Name="City")]
        public string City { get; set; }

        [StringLength(110)]
        [Display(Name = "PostalCode")]
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
