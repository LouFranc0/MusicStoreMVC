using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EcommerceChitarre.Models;

namespace EcommerceChitarre.Controllers
{ 
    public class ArticoliController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Articoli
        public ActionResult Index()
        {
            return View(db.Articoli.ToList());
        }

        // GET: Articoli/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // GET: Articoli/Create
        [Authorize(Roles = "Amministratore")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: Articoli/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore")]

        public ActionResult Create([Bind(Include = "Articolo_ID,Nome,Img,Prezzo,Tempo_Cons,Dettagli")] Articoli articoli)
        {
            if (ModelState.IsValid)
            {
                db.Articoli.Add(articoli);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(articoli);
        }

        // GET: Articoli/Edit/5
        [Authorize(Roles = "Amministratore")]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore")]
        public ActionResult Edit([Bind(Include = "Articolo_ID,Nome,Img,Prezzo,Tempo_Cons,Dettagli")] Articoli articoli)
        {
            if (ModelState.IsValid)
            {
                db.Entry(articoli).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(articoli);
        }

        // GET: Articoli/Delete/5
        [Authorize(Roles = "Amministratore")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore")]
        public ActionResult DeleteConfirmed(int id)
        {
            Articoli articolo = db.Articoli.Find(id);

            // Trova e rimuovi tutte le voci correlate nella tabella OrdArt
            var ordArtCorrelati = db.OrdArt.Where(oa => oa.Articolo_ID == id);
            foreach (var ordArt in ordArtCorrelati)
            {
                db.OrdArt.Remove(ordArt);
            }

            // Rimuovi l'ordine dalla tabella Ordini
            db.Articoli.Remove(articolo);

            // Salva le modifiche
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
