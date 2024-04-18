using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using   EcommerceChitarre.Models;

namespace EcommerceChitarre.Controllers
{
    
    public class OrdArtsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: OrdArts
        [Authorize(Roles = "Amministratore")]
        public ActionResult Index()
        {
            var ordArt = db.OrdArt.Include(o => o.Articoli).Include(o => o.Ordini);
            return View(ordArt.ToList());
        }

        // GET: OrdArts/Details/5
        [Authorize(Roles = "Amministratore,Cliente")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ordineWithArticoli = db.OrdArt
                .Include(o => o.Ordini)
                .Include(o => o.Ordini.Users)
                .Include(o => o.Articoli)
                .Where(o => o.Ordine_ID == id).ToList();

            if (ordineWithArticoli == null)
            {
                return HttpNotFound();
            }
            return View(ordineWithArticoli);      
        }

        // GET: OrdArts/Create
        [Authorize(Roles = "Amministratore,Cliente")]
        public ActionResult Create()
        {
            ViewBag.Articolo_ID = new SelectList(db.Articoli, "Articolo_ID", "Nome");
            ViewBag.Ordine_ID = new SelectList(db.Ordini, "Ordine_ID", "Indirizzo");
            return View();
        }

        // POST: OrdArts/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente,Amministratore")]
        public ActionResult Create(OrdArt ordArt)
        {
            if (ModelState.IsValid)
            {
                // Salva l'ordine nella tabella Ordini
                db.Ordini.Add(ordArt.Ordini);
                db.SaveChanges();

                // Ottieni l'Ordine_ID appena creato
                int newOrdineID = ordArt.Ordini.Ordine_ID;

                // Assegna l'Ordine_ID appena creato al record della tabella OrdArt
                ordArt.Ordine_ID = newOrdineID;

                // Aggiungi il record della tabella OrdArt
                db.OrdArt.Add(ordArt);
                db.SaveChanges();

                return RedirectToAction("Details", "OrdArts", new { id = newOrdineID });
            }

            return View(ordArt);
        }


        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public ActionResult AggiornaQuantita(int articoloId, string operazione)
        {
            // Recupera l'elenco degli articoli attualmente nel carrello dal cookie
            var artsCart = new List<ArtCart>();
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                artsCart = JsonConvert.DeserializeObject<List<ArtCart>>(cartJson);
            }

            // Trova l'articolo con l'ID specificato nell'elenco
            var articolo = artsCart.FirstOrDefault(a => a.Articolo.Articolo_ID == articoloId);
            if (articolo != null)
            {
                // Aggiorna la quantità in base all'operazione
                if (operazione == "incrementa")
                {
                    articolo.Quantita++;
                }
                else if (operazione == "decrementa" && articolo.Quantita > 1)
                {
                    articolo.Quantita--;
                }
            }

            // Aggiorna il cookie del carrello con l'elenco aggiornato degli articoli
            var cartCookie = new HttpCookie("Carrello" + User.Identity.Name);
            cartCookie.Values["User"] = HttpUtility.UrlEncode(JsonConvert.SerializeObject(artsCart));
            cartCookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cartCookie);

            // Reindirizza alla vista del carrello aggiornata
            return RedirectToAction("Cart");
        }

        // Crea Cookie Carrello

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public ActionResult AddToCart(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var articolo = db.Articoli.FirstOrDefault(o => o.Articolo_ID == id);
            if (articolo == null)
            {
                return HttpNotFound();
            }

            // Inizializza artsCart
            List<ArtCart> artsCart = new List<ArtCart>();

            // Creazione del cookie
            HttpCookie cartCookie;

            // Verifica se il cookie "Carrello" esiste già
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                // Se esiste, aggiorna direttamente il valore
                cartCookie = Request.Cookies["Carrello" + User.Identity.Name];
                // Decodifica il valore del cookie e riempie la lista
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                artsCart = JsonConvert.DeserializeObject<List<ArtCart>>(cartJson);
            }
            else
            {
                // Altrimenti, crea un nuovo cookie solo se non esiste già
                cartCookie = new HttpCookie("Carrello" + User.Identity.Name);
                cartCookie.Values["User"] = User.Identity.Name;
            }
            // Verifica se l'articolo è già presente nel carrello
            var existingCartItem = artsCart.FirstOrDefault(item => item.Articolo.Articolo_ID == articolo.Articolo_ID);
            if (existingCartItem != null)
            {
                // Se l'articolo è già presente, incrementa la quantità
                existingCartItem.Quantita++;
            }
            else
            {
                // Aggiungi l'articolo al carrello
                var artCart = new ArtCart()
                {
                    Articolo = new Articoli()
                    {
                        Articolo_ID = articolo.Articolo_ID,
                        Nome = articolo.Nome,
                        Prezzo = articolo.Prezzo,
                        Img = articolo.Img,
                        Tempo_Cons = articolo.Tempo_Cons,
                        Dettagli = articolo.Dettagli,
                    },
                    Quantita = 1,
                    User_Id = Convert.ToInt32(User.Identity.Name),
                };
                artsCart.Add(artCart);
            }

            // Serializza la lista e aggiorna il valore del cookie
            cartCookie["User"] = HttpUtility.UrlEncode(JsonConvert.SerializeObject(artsCart));
            cartCookie.Expires = DateTime.Now.AddDays(1);

            // Aggiunge o aggiorna il cookie nella risposta
            Response.Cookies.Add(cartCookie);

            return RedirectToAction("Index", "Articoli");
        }

        [HttpGet]
        [Authorize(Roles = "Cliente")]
        public ActionResult Cart()
        {
            List<ArtCart> userArtCart = new List<ArtCart>();

            // Verifica se il cookie "Carrello" esiste già
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                var userId = Convert.ToInt32(User.Identity.Name);

                // Decodifica il valore del cookie e riempie la lista
                var artsCart = JsonConvert.DeserializeObject<List<ArtCart>>(cartJson);

                // Filtra solo gli articoli relativi all'utente attuale
                userArtCart = artsCart.Where(a => a.User_Id == userId).ToList();
                ViewBag.UserCart = userArtCart;
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrderFromCart(Ordini ordArt)
        {
            
            if (ModelState.IsValid)
            {
                
                ordArt.CostoCons = 4;
                ordArt.User_ID = Convert.ToInt32(User.Identity.Name);

                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                var userId = Convert.ToInt32(User.Identity.Name);

                var artsCart = JsonConvert.DeserializeObject<List<ArtCart>>(cartJson);
                var userArtCart = artsCart.Where(a => a.User_Id == userId).ToList();

                decimal totale = 0;

                foreach (var art in userArtCart)
                {
                    totale += (art.Quantita * art.Articolo.Prezzo);
                }
                ordArt.Totale = totale + 4;

                ordArt.Stato = "Preparazione";
                ordArt.Data = DateTime.Today;

                db.Ordini.Add(ordArt);
                db.SaveChanges();

                int newOrdineID = ordArt.Ordine_ID;

                foreach (var art in userArtCart)
                {
                    var newOrdArt = new OrdArt();
                    newOrdArt.Articolo_ID = art.Articolo.Articolo_ID;
                    newOrdArt.Ordine_ID = newOrdineID;
                    newOrdArt.Quantita = Convert.ToInt32(art.Quantita);
                    db.OrdArt.Add(newOrdArt);
                }

                db.SaveChanges();

                HttpCookie userCookie = Request.Cookies["Carrello" + User.Identity.Name];
                if (userCookie != null)
                {
                    userCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(userCookie);
                }

                return RedirectToAction("Details", "OrdArts", new { id = newOrdineID });

            }
          
                return RedirectToAction("Cart");
            

           






        }


        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public ActionResult SvuotaCarrello()
        {
            // Rimuovi il cookie del carrello per l'utente corrente
            HttpCookie userCookie = Request.Cookies["Carrello" + User.Identity.Name];
            if (userCookie != null)
            {
                userCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userCookie);
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public ActionResult RimuoviDallCarrello(int articoloId)
        {
            // Recupera l'elenco degli articoli attualmente nel carrello dal cookie
            var artsCart = new List<ArtCart>();
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                artsCart = JsonConvert.DeserializeObject<List<ArtCart>>(cartJson);
            }

            // Trova l'articolo con l'ID specificato nell'elenco e rimuovilo
            var articoloToRemove = artsCart.FirstOrDefault(a => a.Articolo.Articolo_ID == articoloId);
            if (articoloToRemove != null)
            {
                artsCart.Remove(articoloToRemove);
            }

            // Aggiorna il cookie del carrello con l'elenco aggiornato degli articoli
            var cartCookie = new HttpCookie("Carrello" + User.Identity.Name);
            if (artsCart.Any()) // Verifica se ci sono ancora articoli nel carrello
            {
                cartCookie.Values["User"] = HttpUtility.UrlEncode(JsonConvert.SerializeObject(artsCart));
                cartCookie.Expires = DateTime.Now.AddDays(1);
            }
            else
            {
                // Se il carrello è vuoto, elimina completamente il cookie
                cartCookie.Expires = DateTime.Now.AddDays(-1);
            }
            Response.Cookies.Add(cartCookie);

            // Reindirizza alla vista del carrello aggiornata
            return RedirectToAction("Cart");
        }



        // GET: OrdArts/Delete/5
        [Authorize(Roles = "Amministratore,Cliente")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdArt ordArt = db.OrdArt.Find(id);
            if (ordArt == null)
            {
                return HttpNotFound();
            }
            return View(ordArt);
        }

        // POST: OrdArts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore,Cliente")]
        public ActionResult DeleteConfirmed(int id)
        {
            OrdArt ordArt = db.OrdArt.Find(id);
            db.OrdArt.Remove(ordArt);
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
