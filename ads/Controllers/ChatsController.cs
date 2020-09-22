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
using PagedList.Mvc;
using PagedList;

namespace ads.Controllers
{
    public class ChatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Chats
        public async Task<ActionResult> Index()
        {
            var chats = db.Chats.Include(c => c.Ad);
            return View(await chats.ToListAsync());
        }

        // GET: Chats/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Chat chat = await db.Chats.FindAsync(id);
            var idUser = User.Identity.GetUserId();
            if (chat == null || db.ChatUser.Where(cu=>cu.idUser==
           idUser && cu.idChat==chat.id).First()==null)
            {
                return HttpNotFound();
            }
            var notread = await db.Messages.Where(m => m.idChat == chat.id
            && m.idSender != idUser && m.isRead == false).ToListAsync();
            for(int i=0;i<notread.Count;i++)
            {
                notread[i].isRead = true;
                db.Entry(notread[i]).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return View(chat);
        }

        // GET: Chats/Create
        public ActionResult Create()
        {
            ViewBag.idAd = new SelectList(db.Ads, "id", "title");
            return View();
        }

        // POST: Chats/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,title,idAd")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                db.Chats.Add(chat);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idAd = new SelectList(db.Ads, "id", "title", chat.idAd);
            return View(chat);
        }

        // GET: Chats/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            ViewBag.idAd = new SelectList(db.Ads, "id", "title", chat.idAd);
            return View(chat);
        }

        // POST: Chats/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,title,idAd")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chat).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idAd = new SelectList(db.Ads, "id", "title", chat.idAd);
            return View(chat);
        }

        // GET: Chats/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            return View(chat);
        }

        // POST: Chats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Chat chat = await db.Chats.FindAsync(id);
            db.Chats.Remove(chat);
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
