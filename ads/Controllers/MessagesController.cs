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
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;

namespace ads.Controllers
{
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //// GET: Messages
        public async Task<ActionResult> Index()
        {
            var messages = db.Messages.Include(m => m.UserSender);
            return View(await messages.ToListAsync());
        }

        //// GET: Dialogs
        //public async Task<ActionResult> Dialogs()
        //{
        //    string idUser = User.Identity.GetUserId();
        //    ApplicationUser user = await UserManager.FindByIdAsync(idUser);
        //    //Ad ad = 
        //    var mes = await db.Messages.Where(m => m.idRecipient == idUser
        //    || m.idSender == idUser).GroupBy(m => m..ToListAsync();
        //    DialogViewModel dialogs = new DialogViewModel() { User = user, Messages = mes };
        //    var messages = db.Messages.Include(m => m.Ad).Include(m => m.UserRecipient).Include(m => m.UserSender);
        //    await messages.ToListAsync();
        //    return View(dialogs);
        //}

        // GET: Messages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: Messages/Create
        public ActionResult Create()
        {
            //ViewBag.idAd = new SelectList(db.Ads, "id", "title");
            //  ViewBag.idRecipient = new SelectList(db.ApplicationUsers, "Id", "Email");
            // ViewBag.idSender = new SelectList(db.ApplicationUsers, "Id", "Email");
            return View();
        }

        // POST: Messages/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWithAd([Bind(Include = "text")] Message message,
            int? idAd)
        {
                if (User.Identity.IsAuthenticated)
                {

                if (ModelState.IsValid)
                {
                    var idSender = User.Identity.GetUserId();
                    message.idSender = idSender;
                    message.isRead = false;
                    message.datetime = DateTime.Now;

                    Chat Chat;
                
                        bool isExistChat = await db.ChatUser.Where
                            (chat => chat.Chat.idAd == idAd && chat.idUser == idSender).CountAsync() > 0;

                        if (isExistChat)
                        {
                            var ChatUser = await db.ChatUser.Where
                            (chat => chat.Chat.idAd == idAd && chat.User.Id == idSender).FirstAsync();
                            Chat = ChatUser.Chat;
                        }
                        else
                        {
                            var Ad = await db.Ads.FindAsync(idAd);
                            Chat = new Chat();
                            Chat.idAd = idAd;
                            db.Chats.Add(Chat);
                            await db.SaveChangesAsync();

                            ChatUser chatUser1 = new ChatUser();
                            ChatUser chatUser2 = new ChatUser();

                            chatUser1.idChat = Chat.id;
                            chatUser1.idUser = Ad.idUser;

                            chatUser2.idChat = Chat.id;
                            chatUser2.idUser = idSender;

                            db.ChatUser.Add(chatUser1);
                            db.ChatUser.Add(chatUser2);

                            await db.SaveChangesAsync();
                        }
                    message.idChat = Chat.id;
                    db.Messages.Add(message);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index","ChatUsers",new { });
                }
                return View("",new Ad{ id=(int)idAd});
            }
            return RedirectToAction("Login", "Account", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "text")] Message message,
           int? idChat)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var idSender = User.Identity.GetUserId();
                    message.idSender = idSender;
                    message.isRead = false;
                    message.datetime = DateTime.Now;

                    Chat Chat;
                    
                    Chat = new Chat { id = (int)idChat };

                    message.idChat = Chat.id;
                    db.Messages.Add(message);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "ChatUsers", new { });

                }
                return RedirectToAction("Login", "Account", null);
            }
            return View(message);
        }
        // GET: Messages/GetContac
        [Authorize]
        public ActionResult GetContact()
        {
            //ViewBag.idAd = new SelectList(db.Ads, "id", "title");
            //  ViewBag.idRecipient = new SelectList(db.ApplicationUsers, "Id", "Email");
            // ViewBag.idSender = new SelectList(db.ApplicationUsers, "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> CreateToAdmin([Bind(Include = "text, title")] MessageToAdminViewModel message)
        {
            if (ModelState.IsValid)
            { 
                Message newMessage = new Message();
            var idSender = User.Identity.GetUserId();
            newMessage.idSender = idSender;
            newMessage.isRead = false;
            newMessage.datetime = DateTime.Now;

            Chat newChat = new Chat();
            newChat.title = message.title;

            db.Chats.Add(newChat);
            await db.SaveChangesAsync();

            ChatUser chatUser1 = new ChatUser();
            ChatUser chatUser2 = new ChatUser();

            chatUser1.idChat = newChat.id;
            chatUser1.idUser = idSender;

            string admin = ConfigurationManager.AppSettings.Get("AdminToConnection");
            chatUser2.idChat = newChat.id;
            chatUser2.idUser = admin;

            db.ChatUser.Add(chatUser1);
            db.ChatUser.Add(chatUser2);

            await db.SaveChangesAsync();


            newMessage.idChat = newChat.id;
            db.Messages.Add(newMessage);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
            return View(message);
        }

        //public async Task<ActionResult> Create([Bind(Include = "text")] Message message,
        //  int? idAd)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var idSender = User.Identity.GetUserId();
        //        message.idSender = idSender;
        //        message.isRead = false;
        //        message.datetime = DateTime.Now;

        //        Chat Chat;

        //        bool isExistChat = await db.ChatUser.Where
        //            (chat => chat.Chat.idAd == idAd && chat.User.Id == idSender).CountAsync() > 0;
        //        if (isExistChat)
        //        {
        //            var ChatUser = await db.ChatUser.Where
        //            (chat => chat.Chat.idAd == idAd && chat.User.Id == idSender).FirstAsync();
        //            Chat = ChatUser.Chat;
        //        }
        //        else
        //        {
        //            var Ad = await db.Ads.FindAsync(idAd);
        //            Chat = new Chat();
        //            Chat.idAd = idAd;
        //            db.Chats.Add(Chat);
        //            await db.SaveChangesAsync();

        //            ChatUser chatUser1 = new ChatUser();
        //            ChatUser chatUser2 = new ChatUser();

        //            chatUser1.idChat = Chat.id;
        //            chatUser1.idUser = Ad.idUser;

        //            chatUser2.idChat = Chat.id;
        //            chatUser2.idUser = idSender;

        //            db.ChatUser.Add(chatUser1);
        //            db.ChatUser.Add(chatUser2);

        //            await db.SaveChangesAsync();
        //        }


        //        message.idChat = Chat.id;
        //        db.Messages.Add(message);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");

        //    }

        //    return RedirectToAction("~/Views/Account/Login.cshtml");
        //    //if (ModelState.IsValid)
        //    //{

        //    //}

        //    //ViewBag.idAd = new SelectList(db.Ads, "id", "title", message.idAd);
        //    //ViewBag.idRecipient = new SelectList(db.ApplicationUsers, "Id", "Email", message.idRecipient);
        //    //ViewBag.idSender = new SelectList(db.ApplicationUsers, "Id", "Email", message.idSender);
        //    //return View(message);
        //}
        // GET: Messages/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Message message = await db.Messages.FindAsync(id);
        //    if (message == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.idAd = new SelectList(db.Ads, "id", "title", message.idAd);
        //    ViewBag.idRecipient = new SelectList(db.ApplicationUsers, "Id", "Email", message.idRecipient);
        //    ViewBag.idSender = new SelectList(db.ApplicationUsers, "Id", "Email", message.idSender);
        //    return View(message);
        //}

        //// POST: Messages/Edit/5
        //// Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        //// сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "id,text,datetime,isRead,idAd,idSender,idRecipient")] Message message)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(message).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.idAd = new SelectList(db.Ads, "id", "title", message.idAd);
        //    ViewBag.idRecipient = new SelectList(db.ApplicationUsers, "Id", "Email", message.idRecipient);
        //    ViewBag.idSender = new SelectList(db.ApplicationUsers, "Id", "Email", message.idSender);
        //    return View(message);
        //}

        // GET: Messages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Message message = await db.Messages.FindAsync(id);
            db.Messages.Remove(message);
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
