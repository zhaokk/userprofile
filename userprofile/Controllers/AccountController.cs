using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using userprofile.Models;

namespace userprofile.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        [Authorize(Roles = "Admin,Referee")]
        public ActionResult Index(){
            var Db = new ApplicationDbContext();
            var users = Db.Users;
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }

            return View(model);
        
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return View(model);
        }
   
        [HttpPost]
     //   [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);
                // Update the user data:
                user.firstName = model.FirstName;
                user.lastName = model.LastName;
                user.email = model.Email;
                user.country = model.country;

                user.dob = model.dob;
                user.phoneNum = model.phoneNum;
                user.state = model.state;
                user.street = model.street;
                user.streetNumber = model.streetNumber;
                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
     // [Authorize(Roles = "Admin")]
        public ActionResult SelfEdit()
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == User.Identity.Name);
            var model = new EditUserViewModel(user);
            
            return View("Edit",model);
        }
     


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id = null)
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == id);
            if (user.photoDir != @"~\userprofile\default.png") {
                string fullPath = Request.MapPath(user.photoDir);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            Db.Users.Remove(user);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }
       [Authorize(Roles = "Admin")]
        public ActionResult UserRoles(string id)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserRolesViewModel(user);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        idManager.AddUserToRole(user.Id, role.RoleName);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }


        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
      [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Getroles()
        {
            var context = new ApplicationDbContext();
            var allUsers = context.Users.ToList();
            var allRoles = context.Roles.ToList();
            ViewBag.Roles = new MultiSelectList(allRoles, "name", "name");
            return View();
        }

        //
        // GET: /Account/Register
     //   [Authorize(Roles = "Admin")]
       [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            var db = new Entities();
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            RegisterViewModel RVM = new RegisterViewModel(db);
            return View(RVM);
        }
        public ActionResult show()
        {
            var db = new ApplicationDbContext();
            var user = db.Users;
            return View(user);
        }
        //
        // POST: /Account/Register
        [HttpPost]
     //   [Authorize(Roles = "Admin")]
       [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var db = new Entities();
            string location = @"~\userprofile\default.png";
            REFEREE newref=new REFEREE();
            if (model.upload != null)
            {
                string[] split = model.upload.FileName.Split('.');
                string newfilename = model.UserName + '.' + split[1];

                model.upload.SaveAs(Server.MapPath(@"~\userprofile\" + newfilename));
                 location = @"~\userprofile\" + newfilename;

            }
            model.photoDir = location;

            if (ModelState.IsValid)
            {
                var user = model.GetUser();
               
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var storedUser = db.AspNetUsers.First(u => u.UserName == model.UserName);
                    var idManager = new IdentityManager();
                    switch (model.Roles) { 
                        case "Referee":
                            REFEREE refComeWithUser = model.optionalRe.re;
                            refComeWithUser.ID = storedUser.Id;
                            refComeWithUser.QUALIFICATIONS.Clear();
                            foreach (var qual in model.optionalRe.srqvm.quals)
                            {
                                QUALIFICATION thequal = db.QUALIFICATIONS.First(q => q.name == qual.qualName);

                                if (qual.Selected == true)
                                {
                                    refComeWithUser.QUALIFICATIONS.Add(thequal);
                                }

                            }
                            idManager.AddUserToRole(storedUser.Id, model.Roles);
                            refComeWithUser.maxGames = 4;
                            db.REFEREEs.Add(refComeWithUser);
                            db.SaveChanges();
                            break;
                        case "Admin":
                            idManager.AddUserToRole(storedUser.Id, model.Roles);
                            break;
                        default:
                            break;
                    
                    
                    }
                   
                    await SignInAsync(user, isPersistent: false);
                    
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public async Task<Boolean> createUserFromExcel(RegisterViewModel listofUser,string type)
        {
            Boolean result=false;
            switch (type) { 
                case"Referee":
                   result=await createListOfReferee(listofUser);
                    break;
                default:
                    break;
            }
            // If we got this far, something failed, redisplay form
            return result;
        }
        public async Task<Boolean> createListOfReferee(RegisterViewModel model)
        {
            Boolean success = true;
            var db = new Entities();
            string location = @"~\userprofile\default.png";
          

                model.photoDir = location;

                if (ModelState.IsValid)
                {
                    var user = model.GetUser();

                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var storedUser = db.AspNetUsers.First(u => u.UserName == model.UserName);
                        var idManager = new IdentityManager();
                        switch (model.Roles)
                        {
                            case "Referee":
                                REFEREE refComeWithUser = model.optionalRe.re;
                                refComeWithUser.sport = "Soccor";
                                refComeWithUser.ID = storedUser.Id;
                              
                                idManager.AddUserToRole(storedUser.Id, model.Roles);
                                refComeWithUser.maxGames = 4;
                                
                             //   db.REFEREEs.Add(refComeWithUser);
                              //  db.SaveChanges();
                                var i = 0;
                                break;
                           
                            default:
                                success = false;
                                break;


                        }

                        // await SignInAsync(user, isPersistent: false);

                        
                    }
                    else
                    {
                        success = false; ;
                    }
                }
                


            
            return success;
            
        }
       
        public ActionResult showProfile(string id) { 
         var db=new ApplicationDbContext();
            
            var user=db.Users.First(u=>u.UserName==id);
            var model=new logindetialViewModel(user);
            return View(model);
        }
       
        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

      
     
      

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

  
        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
      

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        #endregion
    }
}