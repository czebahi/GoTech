﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GOTech.Models;
using Microsoft.AspNet.Identity.Owin;

/* 
 * Name: Jo Lim
 * Date: Apr 2, 2019
 * Last Modified: Apr 2, 2019
 * Description: This controller provides CRUD function of Employees (ApplicationUser)
 * */

namespace GOTech.Controllers
{
    [Authorize(Roles ="Admin")]
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        // This no-parameter controller must exist
        public EmployeesController() { }

        public EmployeesController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

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

        // GET: Employees
        public ActionResult Index()
        {
            var users = db.Users.AsQueryable();

            // Find employees among the application users. Employees DO have positionId
            var employees = from employee in users
                                            where employee.PositionId != null
                                            select employee;
            return View(employees.Include(x=>x.Position));
        }

        // GET: Employees/Details/5
        public async Task<ActionResult> Details(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByNameAsync(email);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvinceName = db.Provinces.FirstOrDefault(x => x.ProvinceId == user.ProvinceId).ProvinceName;
            return View(user);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByNameAsync(email);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title", user.PositionId);
            ViewBag.ProvinceId = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", user.ProvinceId);
            return View(user);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,LastName,PositionId,HiringDate,Address,City,ProvinceId,PostalCode,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index"); 
            }
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title", user.PositionId);
            ViewBag.ProvinceId = new SelectList(db.Provinces, "ProvinceId", "ProvinceName", user.ProvinceId);
            return View(user);
        }

        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed([Bind(Include = "Email")] ApplicationUser user)
        {
            var employee = await UserManager.FindByEmailAsync(user.Email);
            if (employee != null)
            {
                await UserManager.RemoveFromRoleAsync(employee.Id, "Employee");
                await UserManager.DeleteAsync(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
