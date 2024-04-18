using EcommerceChitarre.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceChitarre.Controllers
{

    public class OrdineDTO
    {
        public int Ordine_ID { get; set; }
        public string Indirizzo { get; set; }
        public string Note { get; set; }
        public DateTime? Data { get; set; }
        public string Stato { get; set; }
        public decimal? Totale { get; set; }
        public decimal? CostoCons { get; set; }
        public int User_ID { get; set; }
    }
    public class AdminController : Controller
    {
        
        private DBContext db = new DBContext();
        
        

        [HttpGet]

        public ActionResult OrdiniEvasi()
        {

            return View();
        }

        [HttpGet]
        public JsonResult GetOrdiniEvasi()
        {
            var order = db.Ordini
               .Where(o => o.Stato == "Evaso").Select(o => new OrdineDTO
               {
                   Ordine_ID = o.Ordine_ID,
                   Indirizzo = o.Indirizzo,
                   Note = o.Note,
                   Data = o.Data,
                   Stato = o.Stato,
                   Totale = o.Totale,
                   CostoCons = o.CostoCons,
                   User_ID = o.User_ID
               }).ToList();

            return Json(order, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public ActionResult TotaleIncassato()
        {
            return View();

        }

        [HttpGet]
        public JsonResult GetTotaleIncassato(DateTime data)
        {
            var ordiniDTO = db.Ordini
                .Where(o => o.Data.HasValue)
                .AsEnumerable() // Esegue la query fino a questo punto
                .Where(o => o.Data.Value.Date == data.Date) // Filtra sulla data in memoria
                .Select(o => new OrdineDTO
                {
                    Ordine_ID = o.Ordine_ID,
                    Indirizzo = o.Indirizzo,
                    Note = o.Note,
                    Data = o.Data,
                    Stato = o.Stato,
                    Totale = o.Totale,
                    CostoCons = o.CostoCons,
                    User_ID = o.User_ID
                }).ToList();

            return Json(ordiniDTO, JsonRequestBehavior.AllowGet);
        }


    }
}