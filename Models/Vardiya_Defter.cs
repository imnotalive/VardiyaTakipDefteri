//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VardiyaTakipDefteri.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vardiya_Defter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vardiya_Defter()
        {
            this.Vardiya_Resim = new HashSet<Vardiya_Resim>();
        }
    
        public int ID { get; set; }
        public Nullable<System.DateTime> VardiyaTarihi { get; set; }
        public string Aciklama { get; set; }
        public bool DevredenCiktiMi { get; set; }
        public string DevreCikisSaati { get; set; }
        public Nullable<int> DefterId { get; set; }
        public Nullable<int> WinderNo { get; set; }
        public string KanalFirinPozisyon { get; set; }
        public string Bolum { get; set; }
        public string Makine { get; set; }
        public Nullable<int> MakineID { get; set; }
        public Nullable<int> BolumID { get; set; }
        public string Taraf { get; set; }
    
        public virtual Vardiya_BolumMakine Vardiya_BolumMakine { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vardiya_Resim> Vardiya_Resim { get; set; }
        public virtual Vardiya_DefterAna Vardiya_DefterAna { get; set; }
    }
}
