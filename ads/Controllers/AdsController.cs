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
using System.Web.Security;
using PagedList.Mvc;
using PagedList;

namespace ads.Controllers
{
    public class AdsController : Controller 
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private Random rnd = new Random();
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

        // GET: Ads
        public async Task<ActionResult> Index(int? id, int? page)
        {
            int pageNumber = (page ?? 1);
            var ads = await db.Ads.OrderByDescending(a=>a.datetime).Where(a=>a.AdStatus.name== "Активное")
                .Include(a => a.AdStatus).Include(a => a.City)
                .Include(a => a.Subject).Include(a=>a.Image).Include(a => a.User).ToListAsync();
            var subjects = await db.Subjects.Select(s => new SubjectsViewModel
            {
                Name = s.name,
                Id = s.id,
                Count = s.Ads.Count()
            }).ToListAsync();

            ViewBag.categories = subjects;
            ViewBag.catgid = null;
            if (id!=null)
            {
                ads = ads.Where(ad => ad.idSubject == id).ToList();
                ViewBag.catgid = id;
            }
            return View(ads.ToPagedList(pageNumber,6));
        }

        // GET: Ads
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminIndex(int? id, int? page)
        {
                int pageNumber = (page ?? 1);
                var ads = await db.Ads.OrderByDescending(a => a.datetime).Include(a => a.AdStatus).Include(a => a.City)
                    .Include(a => a.Subject).Include(a => a.Image).Include(a => a.User).ToListAsync();
                var subjects = await db.AdStatuses.Select(s => new StatusViewModel
                {
                    Name = s.name,
                    Id = s.id,
                    Count = s.Ads.Count()
                }).ToListAsync();

                ViewBag.categories = subjects;
                ViewBag.catgid = null;
                if (id != null)
                {
                    ads = ads.Where(ad => ad.idStatusAd == id).ToList();
                    ViewBag.catgid = id;
                }
                return View(ads.ToPagedList(pageNumber, 6));
        }

        //GET: Ads/UserAds/fduf94
        public async Task<ActionResult> UserAds(string id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            int pageNumber = (page ?? 1);
            ViewBag.id = user.Id;
            var ads = await db.Ads.Where(ad => ad.idUser == id).Include(a => a.AdStatus)
                .Include(a => a.City).Include(a => a.Subject).Include(a=>a.Image).Include(a => a.User)
                .ToListAsync();
            return View(ads.ToPagedList(pageNumber,9));
        }

        // GET: Ads/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.buttonEdit = false;
            ViewBag.buttonDelete = false;
            ViewBag.formMessage = true;
            ViewBag.idAd = ad.id;
            var reviews = await db.Reviews.Where(r => r.idRecipient == ad.idUser).ToListAsync();
            ViewBag.Reviews = reviews.Take(10).ToList();
            if (User.Identity.IsAuthenticated)
            {
                bool isAdmin = UserManager.IsInRole(User.Identity.GetUserId(), "Admin");
                bool isOwner = (User.Identity.GetUserId() == ad.idUser);
                bool isAdActivate = ad.AdStatus.name == "Активное";
                if (isOwner)
                {
                    ViewBag.buttonEdit = true;
                    ViewBag.buttonDelete = true;
                    ViewBag.formMessage = false;


                    if (isAdActivate)
                    {
                        ViewBag.buttonEdit = false;
                    }
                }

                if ( isAdmin)
                {
                    ViewBag.buttonEdit = true;
                    ViewBag.buttonDelete = true;
                }
            }
            return View(ad);
        }

        // GET: Ads/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.idSubject = new SelectList(db.Subjects, "id", "name");
            return View();
        }

        // POST: Ads/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "title,text,idSubject,prise")] Ad ad, 
            HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                ad.idUser = User.Identity.GetUserId();
                Image image = new Image();
                ad.AdStatus = await db.AdStatuses.Where(status => status.name == "Неактивное")?.FirstAsync();
                ad.City = await db.Cities.Where(city => city.name == "Неизвестно")?.FirstAsync();
                ad.datetime = DateTime.Now;
                db.Ads.Add(ad);

                await UploadImage(upload, ad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idStatusAd = new SelectList(db.AdStatuses, "id", "name", ad.idStatusAd);
            ViewBag.idCity = new SelectList(db.Cities, "id", "name", ad.idCity);
            ViewBag.idSubject = new SelectList(db.Subjects, "id", "name", ad.idSubject);
            ViewBag.idUser = new SelectList(db.Users, "Id", "Email", ad.idUser);
            return View(ad);
        }

        // GET: Ads/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            bool isAdmin = UserManager.IsInRole(User.Identity.GetUserId(),"Admin");
            bool isOwner = (User.Identity.GetUserId() == ad.idUser);
            ViewBag.EditStatus = false;
            if (ad == null || (isAdmin==false && isOwner==false ))
            {
                return HttpNotFound();
            }

            ViewBag.idStatusAd = new SelectList(db.AdStatuses, "id", "name", ad.idStatusAd);
            ViewBag.idCity = new SelectList(db.Cities, "id", "name", ad.idCity);
            ViewBag.idSubject = new SelectList(db.Subjects, "id", "name", ad.idSubject);
            if(isAdmin)
            {
                ViewBag.EditStatus = true;
            }
            return View(ad);
        }

        // POST: Ads/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,title,text,idImage,idSubject,idStatusAd,idCity,datetime,prise,idUser")] Ad ad,
              string action, HttpPostedFileBase upload )
        {
                if (ModelState.IsValid)
                {
                    db.Entry(ad).State = EntityState.Modified;
                    if (upload != null) await DeleteImage(ad.idImage);
                    await UploadImage(upload, ad);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            ViewBag.idStatusAd = new SelectList(db.AdStatuses, "id", "name", ad.idStatusAd);
            ViewBag.idCity = new SelectList(db.Cities, "id", "name", ad.idCity);
            ViewBag.idSubject = new SelectList(db.Subjects, "id", "name", ad.idSubject);
            return View(ad);
        }

        private async Task<Image> DeleteImage(int? id)
        {
            if(id==null)
            {
                return null;
            }
            Image img = await db.Images.FindAsync(id);
            System.IO.File.Delete(Server.MapPath("~/Content/userfiles/" + img.id + img.name));
            var adImage = await db.Ads.Where(ad => ad.idImage == id).FirstAsync();
            adImage.idImage = null;
            db.Images.Remove(img);
            await db.SaveChangesAsync();
            return img;
        }

        private async Task<Image> UploadImage(HttpPostedFileBase upload, Ad ad)
        {
            Image img = new Image();
            if (upload != null)
            {
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                img.name = fileName;
                img.idAd = ad.id;
                db.Images.Add(img);
                await db.SaveChangesAsync();
                upload.SaveAs(Server.MapPath("~/Content/userfiles/" + img.id + fileName));
                ad.idImage = img.id;
            }
            return img;
        }
        // GET: Ads/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            var isAdmin = User.IsInRole("Admin");
            var isOwner = User.Identity.GetUserId() == ad.idUser;
            if (ad == null || (isAdmin == false && isOwner == false))
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
                Ad ad = await db.Ads.FindAsync(id);
                var chats = await db.Chats.Where(c => c.idAd == ad.id).ToListAsync();
                for(int i=0;i<chats.Count;i++)
                    {
                chats[i].idAd = null;
               db.Entry(chats[i]).State = EntityState.Modified;
                await db.SaveChangesAsync();
                    }
                db.Ads.Remove(ad);
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
