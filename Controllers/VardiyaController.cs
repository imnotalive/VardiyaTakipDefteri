using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VardiyaTakipDefteri.Models;

namespace VardiyaTakipDefteri.Controllers
{
    public class VardiyaController : Controller
    {
        private POLY_QDMSEntities db = new POLY_QDMSEntities();

        [AllowAnonymous]
        public ActionResult Giris()
        {

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Giris(Vardiya_Kullanici Kullanici)
        {


            var teyit = db.Vardiya_Kullanici.FirstOrDefault(a => a.KullaniciAdi == Kullanici.KullaniciAdi && a.Sifre == Kullanici.Sifre);
            if (teyit != null)
            {
                FormsAuthentication.SetAuthCookie(teyit.KullaniciAdi, false);
                return RedirectToAction("Index", "Vardiya");
            }
            else
            {
                ViewBag.mesaj = "GEÇERSİZ KULLANICI ADI VEYA ŞİFRE !";
                return View();
            }


        }



        [AllowAnonymous]
        public ActionResult VardiyaResimEkleme(int? id)

        {

            Vardiya_Defter qdms_MusteriSikayet = db.Vardiya_Defter.Find(id);
            ViewBag.SikayetId = new SelectList(db.Vardiya_Defter, "ID", "Cari");
            return View();
        }
        [HttpPost]
        public ActionResult VardiyaResimEkleme(int? id, Vardiya_Resim cokluResims, List<HttpPostedFileBase> ImagePath)
        {

            Vardiya_Defter fisDetays = db.Vardiya_Defter.FirstOrDefault(x => x.ID == id);
            if (ModelState.IsValid)
            {
                foreach (var item in ImagePath)

                    if (item.ContentLength > 0)
                    {
                        var image = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/Image"), image);
                        item.SaveAs(path);
                        cokluResims.VardiyaId = fisDetays.ID;
                        cokluResims.ImagePath = "/Image/" + image;
                        db.Vardiya_Resim.Add(cokluResims);
                        db.SaveChanges();
                    }
                return RedirectToAction("Index");
            }

            ViewBag.SikayetId = new SelectList(db.Vardiya_Defter, "ID", "Cari", cokluResims.VardiyaId);
            return View(cokluResims);

        }
        [AllowAnonymous]
     
        public ActionResult Cikis()
        {
            FormsAuthentication.SignOut();

            return View("Giris");
        }
        public ActionResult Index(int? ara)
        {
            List<Vardiya_DefterAna> fisAna = db.Vardiya_DefterAna.OrderByDescending(x => x.Tarih).ThenBy(x=>x.VardiyaSaatDeger).ToList();
            return View(fisAna);
           
            //List<Vardiya_DefterAna> fisAna = db.Vardiya_DefterAna.OrderByDescending(a=>a.Tarih.ToString()).ToList();
            //Vardiya_DefterAna fisAna = db.Vardiya_DefterAna.OrderByDescending(a => a.Tarih).ToList();
           

        }

        //public ActionResult Indexs(string ara)
        //{
        //    using (POLY_QDMSEntities entites = new POLY_QDMSEntities())
        //    {
        //        List<Vardiya> model = new List<Vardiya>();

        //        foreach (Vardiya_DefterAna customer in entites.Vardiya_DefterAna.OrderByDescending(a => a.Tarih))
        //        {
        //            model.Add(new Vardiya
        //            {
        //                ID = customer.ID,
        //                Tarih = customer.Tarih,
        //                VardiyaSaati = customer.VardiyaSaati,
        //                VardiyaAmiri = customer.VardiyaAmiri,
        //                VardiyaAmiriYrd = customer.VardiyaAmirYardimcisi,

        //                Orders = entites.Vardiya_Defter.Where(o => o.DefterId == customer.ID).OrderByDescending(o => o.VardiyaTarihi).Take(9).ToList()
        //            });
        //        }
        //        //var data = GetEmployees(search);

        //        return View(model);
        //    }
        //}
        //Vardiya _mobjModel = new Vardiya();
        //public ActionResult Filter(string description)
        //{
        //    var schedules = _mobjModel.Orders.ToList();

        //    if (!string.IsNullOrEmpty(description))
        //    {
        //        schedules = schedules.Where(s => s.Makine.ToLower().Contains(description.ToLower())).ToList();
        //    }

        //    return PartialView("Index", schedules);
        //}
        [AllowAnonymous]
        public ActionResult Ara(int? ara)
        {
            int? department_id = Convert.ToInt32(ara);
            var qq = (from e in db.Vardiya_DefterAna
                      join d in db.Vardiya_Defter on e.ID equals d.DefterId
                      where e.ID == department_id
                      select new Vardiya
                      {
                          ID = e.ID,

                          Tarih = e.Tarih,
                          VardiyaAmiri = e.VardiyaAmiri,
                          VardiyaSaati = e.VardiyaSaati,
                          VardiyaAmiriYrd = e.VardiyaAmirYardimcisi,

                      }).ToList();
            return PartialView("Index", qq);

        }

        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vardiya_Defter vardiya_Defter = db.Vardiya_Defter.Find(id);
            if (vardiya_Defter == null)
            {
                return HttpNotFound();
            }
            return View(vardiya_Defter);
        }


        public ActionResult Create()
        {
            //ViewBag.VardiyaSaatiId = new SelectList(db.Vardiya_Saat, "ID", "VardiyaSaati");

            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vardiya_DefterAna defterAna)
        {

            if (db.Vardiya_DefterAna.Any(a => a.Tarih == defterAna.Tarih && a.VardiyaSaati == defterAna.VardiyaSaati))
            {
                Response.Write("<script> alert('AYNI TARİHTEKİ AYNI VARDİYA SAATİ EKLENEMEZ');</script>");
                return RedirectToAction("Index");
                //ViewBag.mesaj = "dsafasfasfas";

                //return RedirectToAction("Index");
            }
            ////else if (db.Vardiya_DefterAna.Any(a => a.Tarih != defterAna.Tarih && a.VardiyaSaati==defterAna.VardiyaSaati))
            ////{
            ////    Response.Write("<script>alert('TARİHE EKLENEMEZ');</script>");
            ////    return RedirectToAction("Index");
            ////}
            else if (ModelState.IsValid) 
            {

                if (defterAna.VardiyaSaati == "23:00-07:00")
                {
                    defterAna.VardiyaSaatDeger = 3;
                    db.Vardiya_DefterAna.Add(defterAna);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                if (defterAna.VardiyaSaati == "07:00-15:00")
                {
                    defterAna.VardiyaSaatDeger = 2;
                    db.Vardiya_DefterAna.Add(defterAna);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                if (defterAna.VardiyaSaati == "15:00-23:00")
                {
                    defterAna.VardiyaSaatDeger = 1;
                    db.Vardiya_DefterAna.Add(defterAna);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            //ViewBag.VardiyaSaatiId = new SelectList(db.Vardiya_Saat, "ID", "VardiyaSaati", defterAna.VardiyaSaatiId);
            return View();

        }
        public ActionResult Detaylar(int? id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vardiya_DefterAna vardiya_Defter = db.Vardiya_DefterAna.Find(id);
            if (vardiya_Defter == null)
            {
                return HttpNotFound();
            }
            return View(vardiya_Defter);
        }
        [AllowAnonymous]
        public ActionResult Edit(int? id, Vardiya_Defter fisDetay)
        {

            Vardiya_DefterAna fis;

            //ViewBag.BolumID = new SelectList(db.Vardiya_Bolum, "BolumID", "Bolum");
            //ViewBag.MakineID = new SelectList(db.Vardiya_BolumMakine, "MakineID", "Makineler");
            List<Vardiya_Bolum> CountryList = db.Vardiya_Bolum.ToList();
            ViewBag.BolumID = new SelectList(CountryList, "BolumID", "Bolum"); 
            List<Vardiya_BolumMakine> CountryLists = db.Vardiya_BolumMakine.ToList();
            ViewBag.MakineID = new SelectList(CountryLists, "MakineID", "Makineler");
            fis = db.Vardiya_DefterAna.FirstOrDefault(x => x.ID == id);
            fisDetay.DefterId = fis.ID;
            //if (id == null)
            //{
            //    Vardiya_Defter vardiya_Defter = db.Vardiya_Defter.Find(id);
            //}
            Vardiya_Defter vardiya_Defter = db.Vardiya_Defter.FirstOrDefault(a => a.DefterId == id);
            //Vardiya_Defter vardiya_Defter = db.Vardiya_Defter.Find(id);

            //if (vardiya_Defter == null)
            //{
            //    return HttpNotFound();
            //}


            return View(vardiya_Defter);
        }
        [AllowAnonymous]



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vardiya_DefterAna fisAna, Vardiya_Defter fisDetay, int? id)
        {
        

            if (fisDetay != null)
            {
                int sayac = 0;
                Vardiya_DefterAna fis;
                int result = 0;

                fis = db.Vardiya_DefterAna.FirstOrDefault(x => x.ID == id);

                if (fis == null)
                {
                    fis = new Vardiya_DefterAna()
                    {

                        VardiyaAmiri = fis.VardiyaAmiri,
                        VardiyaAmirYardimcisi = fis.VardiyaAmirYardimcisi,
                        TamamlandiMi = false

                    };
                    sayac++;
                    db.Vardiya_DefterAna.Add(fis);
                    result = db.SaveChanges();

                }

                fisDetay.Vardiya_DefterAna = fis;
                fisDetay.DefterId = fis.ID;
                fisDetay.VardiyaTarihi = fis.Tarih;
                

                db.Vardiya_Defter.Add(fisDetay);
                db.SaveChanges();


              


                return RedirectToAction(nameof(Edit));
            }

         

          
            return View(fisDetay);
        }

        public JsonResult GetStateList(int BolumID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Vardiya_BolumMakine> StateList = db.Vardiya_BolumMakine.Where(x => x.MakineID == BolumID).OrderBy(x => x.Makineler).ToList();
            return Json(StateList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult makineGetir(int p)
        {
            var makine = (from x in db.Vardiya_BolumMakine
                          join y in db.Vardiya_Bolum on x.Vardiya_Bolum.BolumID equals y.BolumID
                          where x.Vardiya_Bolum.BolumID == p

                          select new
                          {
                              Text = x.Makineler,
                              Value = x.MakineID.ToString()
                          }).ToList();
            return Json(makine, JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public ActionResult EditDetay(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vardiya_Defter vardiya_Defter = db.Vardiya_Defter.Find(id);


            if (vardiya_Defter == null)
            {
                return HttpNotFound();
            }

            return View(vardiya_Defter);
        }
        [HttpPost]
        public ActionResult EditDetay(int? id, Vardiya_Defter defter, Vardiya_DefterAna fis)
        {
            Vardiya_Defter fisDetay = db.Vardiya_Defter.FirstOrDefault(x => x.ID == id);
            if (ModelState.IsValid)
            {


                fisDetay.Aciklama = defter.Aciklama;
                fisDetay.Makine = defter.Makine;
                fisDetay.WinderNo = defter.WinderNo;
                fisDetay.KanalFirinPozisyon = defter.KanalFirinPozisyon;
                fisDetay.DevreCikisSaati = defter.DevreCikisSaati;
                fisDetay.DevredenCiktiMi = defter.DevredenCiktiMi;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fisDetay);
        }


        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Vardiya_Defter vardiya_Defter = db.Vardiya_Defter.Find(id);
        //    if (vardiya_Defter == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(vardiya_Defter);
        //}


        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Vardiya_Defter vardiya_Defter = db.Vardiya_Defter.Find(id);
        //    db.Vardiya_Defter.Remove(vardiya_Defter);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public ActionResult Delete(int? id, Vardiya_Defter fisDetay)
        {
          
            

            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            Vardiya_Defter denemes = db.Vardiya_Defter.Find(id);

            if (denemes == null)
            {
                return HttpNotFound();
            }
            return View(denemes);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
          
            Vardiya_Defter denemes = db.Vardiya_Defter.Find(id);
          
            db.Vardiya_Defter.Remove(denemes);
            db.SaveChanges();
            return RedirectToAction("Index", "Vardiya");
        }
        public ActionResult VardiyaRapor()
        {
            return View(new TarihRapor());
        }
        [HttpPost]
        public ActionResult VardiyaRapor(TarihRapor model)
        {


            return VardiyaRaporModeliOlustur(model);
        }


        [HttpPost]
        public FileResult VardiyaRaporModeliOlustur(TarihRapor gelenModel)
        {

            var model = new TarihRapor()
            {

            };

            model.BaslangicTarihiDateTime = gelenModel.BaslangicTarihiDateTime;
            model.BitisTarihiDateTime = gelenModel.BitisTarihiDateTime;

            if (model.BaslangicTarihiDateTime != null && model.BitisTarihiDateTime != null)
            {
                model.VardiyaSureModel = db.Vardiya_Defter
                    .Where(a => a.VardiyaTarihi >= (model.BaslangicTarihiDateTime) && a.VardiyaTarihi <= (model.BitisTarihiDateTime)).OrderBy(a => a.VardiyaTarihi).ToList();
            }
            else
            {
                ViewBag.Error = "VERİ YOK";
            }
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[]{

                new DataColumn("Tarih"),
                new DataColumn("Vardiya Amiri"),
                new DataColumn("Vardiya Amir Yrd"),
                new DataColumn("Vardiya Saati"),


                    new DataColumn("Winder No"),
                        new DataColumn("Kanal-Fırın-Pozisyon"),
                             new DataColumn("Bölüm"),
                new DataColumn("Makine"),
                            new DataColumn("Açıklama"),
                             new DataColumn("Devreden Çıktı Mı"),
                           new DataColumn("Devreden Çıkış Saati"),



            });
            var liste = from list in db.Vardiya_Defter select list;



            if (model.VardiyaSureModel != null)
            {

                foreach (var list in model.VardiyaSureModel)
                {
                    dt.Rows.Add(
                        list.VardiyaTarihi.GetValueOrDefault().ToString("dd.MM.yyyy"), list.Vardiya_DefterAna.VardiyaAmiri, list.Vardiya_DefterAna.VardiyaAmirYardimcisi, list.Vardiya_DefterAna.VardiyaSaati, list.WinderNo, list.KanalFirinPozisyon, list.Bolum, list.Makine, list.Aciklama, list.DevredenCiktiMi ? "EVET" : "HAYIR", list.DevreCikisSaati);
                }

            }
            else
            {
                ViewBag.Error = "VERİ YOK";
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var W_WorkSheet = wb.Worksheets.Add(dt, "VardiyaDefteri");
                //W_WorkSheet.Columns().AdjustToContents(); // Sütunların içerigine göre sütünları genişletir
                W_WorkSheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);


                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("dd MMMM yyyy") + "-" + "VardiyaDefteri" + ".xlsx");
                }


            }

        }

        [AllowAnonymous]
        public ActionResult EditTrafo(int? id, Vardiya_TrafoKmprsr fisDetay)
        {

            Vardiya_DefterAna fis;
      

            fis = db.Vardiya_DefterAna.FirstOrDefault(x => x.ID == id);
            fisDetay.VardiyaDefteriAnaId = fis.ID;
         
            Vardiya_TrafoKmprsr vardiya_Defter = db.Vardiya_TrafoKmprsr.FirstOrDefault(a => a.VardiyaDefteriAnaId == id);
     

            return View(vardiya_Defter);
        }
        [AllowAnonymous]



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTrafo(Vardiya_DefterAna fisAna, Vardiya_TrafoKmprsr fisDetay, int? id)
        {


            if (fisDetay != null)
            {
                int sayac = 0;
                Vardiya_DefterAna fis;
                int result = 0;

                fis = db.Vardiya_DefterAna.FirstOrDefault(x => x.ID == id);

                if (fis == null)
                {
                    fis = new Vardiya_DefterAna()
                    {

                        VardiyaAmiri = fis.VardiyaAmiri,
                        VardiyaAmirYardimcisi = fis.VardiyaAmirYardimcisi,
                        TamamlandiMi = false

                    };
                    sayac++;
                    db.Vardiya_DefterAna.Add(fis);
                    result = db.SaveChanges();

                }

                fisDetay.Vardiya_DefterAna = fis;
                fisDetay.VardiyaDefteriAnaId = fis.ID;



                db.Vardiya_TrafoKmprsr.Add(fisDetay);
                result = db.SaveChanges();





                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        //[AllowAnonymous]
        //public ActionResult TrafoDuzenle(int? id, Vardiya_TrafoKmprsr fisDetay)
        //{

        //    Vardiya_DefterAna fis;


        //    fis = db.Vardiya_DefterAna.FirstOrDefault(x => x.ID == id);
        //    fisDetay.VardiyaDefteriAnaId = fis.ID;

        //    Vardiya_TrafoKmprsr vardiya_Defter = db.Vardiya_TrafoKmprsr.FirstOrDefault(a => a.VardiyaDefteriAnaId == id);



        //    return View(vardiya_Defter);
        //}
        //[AllowAnonymous]



        //[HttpPost]

        //public ActionResult TrafoDuzenle(Vardiya_DefterAna fisAna, Vardiya_TrafoKmprsr fisDetay, int? id)
        //{

        //    if (fisDetay != null)
        //    {
        //        int sayac = 0;
        //        Vardiya_DefterAna fis;
        //        int result = 0;

        //        fis = db.Vardiya_DefterAna.FirstOrDefault(x => x.ID == id);

        //        if (fis == null)
        //        {
        //            fis = new Vardiya_DefterAna()
        //            {

        //                VardiyaAmiri = fis.VardiyaAmiri,
        //                VardiyaAmirYardimcisi = fis.VardiyaAmirYardimcisi,
        //                TamamlandiMi = false

        //            };
        //            sayac++;
        //            db.Vardiya_DefterAna.Add(fis);
        //            result = db.SaveChanges();

        //        }

        //        fisDetay.Vardiya_DefterAna = fis;
        //        fisDetay.VardiyaDefteriAnaId = fis.ID;



        //        db.Vardiya_TrafoKmprsr.Add(fisDetay);
        //        result = db.SaveChanges();





        //        return RedirectToAction(nameof(Edit));
        //    }
        //    return View(fisDetay);
        //}
        //public ActionResult TrafoDuzenle(int? id,Vardiya_TrafoKmprsr trafo)
        //{

        //    Vardiya_DefterAna fis;
        //    fis = db.Vardiya_DefterAna.FirstOrDefault(x => x.ID == id);
        //    trafo.VardiyaDefteriAnaId = trafo.ID;

        //    Vardiya_TrafoKmprsr vardiya_Defter = db.Vardiya_TrafoKmprsr.Find(id);



        //    ViewBag.VardiyaDefteriAnaId = new SelectList(db.Vardiya_DefterAna, "ID", "VardiyaAmiri", vardiya_TrafoKmprsr.VardiyaDefteriAnaId);


        //    return View(vardiya_Defter);
        //}

        public ActionResult TrafoDuzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vardiya_TrafoKmprsr vardiya_TrafoKmprsr = db.Vardiya_TrafoKmprsr.Find(id);
            if (vardiya_TrafoKmprsr == null)
            {
                return HttpNotFound();
            }
            ViewBag.VardiyaDefteriAnaId = new SelectList(db.Vardiya_DefterAna, "ID", "VardiyaAmiri", vardiya_TrafoKmprsr.VardiyaDefteriAnaId);
            return View(vardiya_TrafoKmprsr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrafoDuzenle(Vardiya_TrafoKmprsr trafo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trafo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VardiyaDefteriAnaId = new SelectList(db.Vardiya_DefterAna, "ID", "VardiyaAmiri", trafo.VardiyaDefteriAnaId);
            return View(trafo);


        }


    }
}
