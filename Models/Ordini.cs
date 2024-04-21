namespace EcommerceChitarre.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ordini")]
    public partial class Ordini
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ordini()
        {
            OrdArt = new HashSet<OrdArt>();
        }

        [Key]
        public int Ordine_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Indirizzo { get; set; }

        [StringLength(250)]
        public string Note { get; set; }

       
        [DataType(DataType.DateTime)]
        public DateTime? Data {  get; set; }


        [StringLength(50)]
        public string Stato { get; set; }

       
        public decimal? Totale { get; set; }


        public decimal? CostoCons { get; set; }

        public int User_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdArt> OrdArt { get; set; }  

        public virtual Users Users { get; set; }
    }
}
