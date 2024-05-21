﻿using System.ComponentModel.DataAnnotations;

namespace StoreCursoMod165.Models
{
    public class Category
    {
        public int ID { get; set; }

        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [StringLength(255)]
        [Required]
        public string Description { get; set; }
    }
}
