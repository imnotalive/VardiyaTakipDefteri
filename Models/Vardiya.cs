using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardiyaTakipDefteri.Models
{
    public class Vardiya
    {

        public Vardiya()
    {

            Trafos = new List<Vardiya_TrafoKmprsr>();
    }

        public int ID { get; set; }
        public DateTime? Tarih { get; set; } 
        
        public string VardiyaAmiri { get; set; }
        public string VardiyaAmiriYrd { get; set; }
        public string VardiyaSaati { get; set; }  
        public int Trafo1 { get; set; }   
        
        public int Trafo2 { get; set; }    
        public int Trafo3 { get; set; }    
        public int Trafo4 { get; set; }   
        public int Trafo5 { get; set; }    
        public int Trafo6 { get; set; }    
        public int Trafo7 { get; set; }   
        public string Yedek { get; set; }  
        public string Calisan { get; set; }  
        //public int MakineId { get; set; }
        public string MakineId { get; set; }
        public List<Vardiya_Defter> Orders { get; set; }
        public List<Vardiya_TrafoKmprsr> Trafos { get; set; }
    }

}