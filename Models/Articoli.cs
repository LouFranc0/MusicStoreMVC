namespace EcommerceChitarre.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Articoli")]
    public partial class Articoli
    {
        public Articoli()
        {
            OrdArt = new HashSet<OrdArt>();
        }

        [Key]
        public int Articolo_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [StringLength(255)]
        public string Img { get; set; }

        public decimal Prezzo { get; set; }

        [StringLength(50)]
        public string Tempo_Cons { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Dettagli { get; set; }

        public virtual ICollection<OrdArt> OrdArt { get; set; }
    }
}
