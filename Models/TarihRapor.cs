using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VardiyaTakipDefteri.Models
{
    public class TarihRapor
    {


    
            public TarihRapor()
            {
                BaslangicTarihiDateTime = DateTime.Now.AddDays(-1);
                BitisTarihiDateTime = DateTime.Now;
                VardiyaSureModel = new List<Vardiya_Defter>();
            }

      

            [DataType(DataType.Date)]

            public DateTime BaslangicTarihiDateTime { get; set; }


            public DateTime BitisTarihiDateTime { get; set; }

            public List<Vardiya_Defter> VardiyaSureModel { get; set; }
       
    }
}