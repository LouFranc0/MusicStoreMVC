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
    public class UsersController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Users
        [Authorize(Roles = "Amministratore")]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
       

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "User_ID,Nome,Cognome,Email,Password,Ruolo")] Users users)
        {
            var userDb = db.Users.Where(u => u.Email == users.Email).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (userDb == null)
                {
                    db.Users.Add(users);
                    db.SaveChanges();
                    TempData["message"] = "Account creato con successo";
                    return RedirectToAction("Login","Auth");
                }
                else
                {
                    TempData["mesError"] = "L'email esiste già";
                    return View(users);
                }
            }

            return View(users);
        }


        // GET: Users/Edit/5
        [Authorize(Roles = "Cliente, Amministratore")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente, Amministratore")]
        public ActionResult Edit([Bind(Include = "User_ID,Nome,Cognome,Email,Password,Ruolo")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Cliente")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
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
