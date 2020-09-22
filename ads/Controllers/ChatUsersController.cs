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
using PagedList;

namespace ads.Controllers
{
    public class ChatUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ChatUsers
        [Authorize]
        public async Task<ActionResult> Index(int? page)
        {
            int pageNumber = (page ?? 1);
            var CurrentUser = User.Identity.GetUserId();
            var groupchat = await db.ChatUser.Where(chatus => chatus.idUser == CurrentUser).Select(chat =>
            new DialogViewModel
            {
                Chatt = chat.Chat,
                CountMess = chat.Chat.Messages.Where(mes => mes.isRead == false &&
                mes.idSender!=CurrentUser).Count(),
                Chaters = chat.Chat.ChatUsers.Where(c => c.idUser != CurrentUser).ToList()
            }).ToListAsync();

            //var chatUser = db.ChatUser.Where(chatus => chatus.idUser==CurrentUser)
            //    .Include(c => c.Chat).Include(c => c.User);

            return View(groupchat.ToPagedList(pageNumber, 6));
        }

        // GET: ChatUsers/Details/5
        public async Task<ActionResult> Details(int? idChat)
        {
            if (idChat == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ChatUser chatUser = await db.ChatUser.FindAsync(idChat);
            if (chatUser == null)
            {
                return HttpNotFound();
            }
            return View(chatUser);
        }

        // GET: ChatUsers/Create
        public ActionResult Create()
        {
            ViewBag.idChat = new SelectList(db.Chats, "id", "title");
            ViewBag.idUser = new SelectList(db.ApplicationUsers, "Id", "Email");
            return View();
        }

        // POST: ChatUsers/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idChat,idUser")] ChatUser chatUser)
        {
            if (ModelState.IsValid)
            {
                db.ChatUser.Add(chatUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idChat = new SelectList(db.Chats, "id", "title", chatUser.idChat);
            ViewBag.idUser = new SelectList(db.ApplicationUsers, "Id", "Email", chatUser.idUser);
            return View(chatUser);
        }

        // GET: ChatUsers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatUser chatUser = await db.ChatUser.FindAsync(id);
            if (chatUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.idChat = new SelectList(db.Chats, "id", "title", chatUser.idChat);
            ViewBag.idUser = new SelectList(db.ApplicationUsers, "Id", "Email", chatUser.idUser);
            return View(chatUser);
        }

        // POST: ChatUsers/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idChat,idUser")] ChatUser chatUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chatUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idChat = new SelectList(db.Chats, "id", "title", chatUser.idChat);
            ViewBag.idUser = new SelectList(db.ApplicationUsers, "Id", "Email", chatUser.idUser);
            return View(chatUser);
        }

        // GET: ChatUsers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatUser chatUser = await db.ChatUser.FindAsync(id);
            if (chatUser == null)
            {
                return HttpNotFound();
            }
            return View(chatUser);
        }

        // POST: ChatUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ChatUser chatUser = await db.ChatUser.FindAsync(id);
            db.ChatUser.Remove(chatUser);
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
