using System.ComponentModel.DataAnnotations;

namespace StoreCursoMod165.Models
{
    public class Status
    {
        public int ID { get; set; }
        [Display(Name = "Name of Status")]
        public string NameofStatus { get; set; }
    }
}
