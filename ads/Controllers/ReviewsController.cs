using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ads.Models;
using Microsoft.AspNet.Identity;

namespace ads.Controllers
{
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews
        public async Task<ActionResult> Index()
        {
            var reviews = db.Reviews.Include(r => r.UserRecipient).Include(r => r.UserSender);
            return View(await reviews.ToListAsync());
        }

        // GET: Reviews/About/jkjk-sdsa
        public async Task<ActionResult> About(string id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser rec = await db.Users.Where(u => u.Id == id).FirstAsync();
            if(rec==null)
            {
                return HttpNotFound();
            }
            var reviews = await db.Reviews.Where(r=>r.idRecipient==id).ToListAsync();
            if (User.Identity.IsAuthenticated && User.Identity.GetUserId() == id)
            {
                var notread = reviews.Where(r => r.isRead == false).ToList();
                for (int i = 0; i < notread.Count; i++)
                {
                    notread[i].isRead = true;
                    db.Entry(notread[i]).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
            return View("Index",reviews);
        }

        // GET: Reviews/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        public ActionResult Create()
        {
            ViewBag.idRecipient = new SelectList(db.ApplicationUsers, "Id", "Email");
            ViewBag.idSender = new SelectList(db.ApplicationUsers, "Id", "Email");
            return View();
        }

        // POST: Reviews/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "text,idSender,idRecipient")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.datetime = DateTime.Now;
                review.isRead = false;
                db.Reviews.Add(review);
                await db.SaveChangesAsync();
                return RedirectToAction("Index","Ads",null);
            }
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.idRecipient = new SelectList(db.ApplicationUsers, "Id", "Email", review.idRecipient);
            ViewBag.idSender = new SelectList(db.ApplicationUsers, "Id", "Email", review.idSender);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,text,datetime,isRead,idSender,idRecipient")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idRecipient = new SelectList(db.ApplicationUsers, "Id", "Email", review.idRecipient);
            ViewBag.idSender = new SelectList(db.ApplicationUsers, "Id", "Email", review.idSender);
            return View(review);
        }
        [Authorize(Roles ="Admin")]
        // GET: Reviews/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Review review = await db.Reviews.FindAsync(id);
            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
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
