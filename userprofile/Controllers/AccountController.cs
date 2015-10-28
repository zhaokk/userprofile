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
using System.Reflection;
using System.Net;

namespace userprofile.Controllers
{
   
    public class AccountController : Controller
    {

		public ActionResult makeAdmin(int? userID) {
			int uID = userID.Value;
			string user = userID.ToString();
			Raoconnection db = new Raoconnection();
			if (db.AspNetUsers.First(o=> o.Id == user).AspNetRoles.Where(role => role.Id == "1").Count() == 0)
				new IdentityManager().AddUserToRole(user,"Admin");
			else
				new IdentityManager().RemoveUserFromRole(user, "Admin");
			return Json(userID);
		}

        [Authorize(Roles = "Admin,Organizer")]
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
     //   no authorise due to self edit
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            Boolean shouldFail = false;
            if (ModelState.IsValid)
            {
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);

                if (checkUsername(model.UserName) && user.UserName != model.UserName)
                {
                    ModelState.AddModelError("Username", "Username alredy exists");
                    shouldFail = true;
                }
                if (checkEmail(model.Email) && user.email != model.Email)
                {
                    ModelState.AddModelError("Email", "Email already registered");
                    shouldFail = true;
                }
                if (checkFFA(model.ffaNum) && user.ffaNum != model.ffaNum)
                {
                    ModelState.AddModelError("ffaNum", "FFA number alredy in system");
                    shouldFail = true;
                }

                if (!shouldFail)
                {

                    // Update the user data:
                    user.firstName = model.FirstName;
                    user.lastName = model.LastName;
                    user.email = model.Email;
                    user.country = model.country;

                    user.dob = model.dob;
                    user.phoneNum = model.phoneNum;
                    user.state = model.state;
                    user.street = model.street;
                    Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    await Db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            // If we got this far something failed, redisplay form
            return View(model);
        }

        public ActionResult SelfEdit()
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == User.Identity.Name);
            var model = new EditUserViewModel(user);
            
            return View("Edit",model);
        }



        [Authorize(Roles = "Admin,Organizer")]
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

        public ActionResult Details(String id)
        {
            if (id == null)
            {
                return HttpNotFound();
            } 
            var db = new Raoconnection();


            var user = db.AspNetUsers.First(u => u.UserName == id);
            var playerIn = db.PLAYERs.Where(teams => teams.userId == user.Id && teams.status > 0).ToList();
            var managerOfteams = db.TEAMs.Where(teams => teams.managerId == user.Id && teams.status > 0).ToList();
            //List<TOURNAMENT> organizerOfTournaments = db.TOURNAMENTs.Where(t => t.AspNetUser.Id == id && t.status > 0).ToList();
			List<TOURNAMENT> organizerOfTournaments = db.TOURNAMENTs.Where(t => t.AspNetUsers.Where(u => u.Id == user.Id).Count() > 0 && t.status > 0).ToList();

            if (user == null)
            {
                return HttpNotFound();
            }
            var combined = new Tuple<AspNetUser, List<PLAYER>, List<TEAM>, List<TOURNAMENT>>(user, playerIn, managerOfteams, organizerOfTournaments) { };

            return View(combined);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
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
            user.status = 0;

            //Db.Entry(user).State = EntityState.Modified;
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
        [Authorize(Roles = "Admin,Organizer")]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var db = new Raoconnection();

                var user = db.AspNetUsers.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);

                //admin
                if (model.Roles[0].Selected)
                {
                    var roleMap = db.AspNetRoles.First(m => m.Name=="Admin");
                    user.AspNetRoles.Add(roleMap);

                }
                else
                {
                    user.AspNetRoles.Remove(db.AspNetRoles.First(m => m.Name == "Admin"));
                    db.SaveChanges();
                }

                //player
                if (model.Roles[1].Selected)
                {
                    var roleMap = db.AspNetRoles.First(m => m.Name == "Player"); 
                    user.AspNetRoles.Add(roleMap);

                }
                else
                {
                    user.AspNetRoles.Remove(db.AspNetRoles.First(m => m.Name == "Player"));
                    db.SaveChanges();
                }

                //referee
                if (model.Roles[2].Selected)
                {
                    var roleMap = db.AspNetRoles.First(m => m.Name == "Referee");
                    user.AspNetRoles.Add(roleMap);

                }
                else
                {
                    user.AspNetRoles.Remove(db.AspNetRoles.First(m => m.Name == "referee"));
                    db.SaveChanges();
                }

                //organizer
                if (model.Roles[3].Selected)
                {
                    var roleMap = db.AspNetRoles.First(m => m.Name == "Organizer"); 
                    user.AspNetRoles.Add(roleMap);
                   
                }
                else
                {
                    user.AspNetRoles.Remove(db.AspNetRoles.First(m => m.Name == "Organizer"));
                    db.SaveChanges();
                }

                //organizer
                if (model.Roles[4].Selected)
                {
                    var roleMap = db.AspNetRoles.First(m => m.Name == "Manager");
                    user.AspNetRoles.Add(roleMap);

                }
                else
                {
                    user.AspNetRoles.Remove(db.AspNetRoles.First(m => m.Name == "Manager"));
                    db.SaveChanges();
                }


                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                int x = db.SaveChanges();


                return RedirectToAction("Index");
            }
            return View();
        }


        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
			UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
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
            var db = new Raoconnection();
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "name", "name");
            RegisterViewModel RVM = new RegisterViewModel(db);
            return View(RVM);
        }

       public ActionResult RegisterNew()
       {
           var db = new Raoconnection();
           ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
           ViewBag.tournament = new SelectList(db.TOURNAMENTs, "name", "name");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        //handles self register, so no authenticate
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            Boolean shouldFail = false;
            var db = new Raoconnection();
			//ViewBag.sport = new SelectList(db.SPORTs, "name", "name"); DISABLED
			RegisterViewModel RVM = new RegisterViewModel(db);
            string location = @"~\userprofile\default.png";
            REFEREE newref=new REFEREE();
            /*if (model.upload != null) DISABLED UPLOAD
            {
                string[] split = model.upload.FileName.Split('.');
                string newfilename = model.UserName + '.' + split[1];

                model.upload.SaveAs(Server.MapPath(@"~\userprofile\" + newfilename));
                 location = @"~\userprofile\" + newfilename;

            }
			 
			 */
            model.photoDir = location;


            if (ModelState.IsValid)
            {
                var user = model.GetUser();


                if (checkUsername(user.UserName))
                {
                    ModelState.AddModelError("Username", "Username alredy exists");
                    shouldFail = true;
                }
                if (checkEmail(user.email))
                {
                    ModelState.AddModelError("Email", "email already registered");
                    shouldFail = true;
                }
				if (user.ffaNum != 0) {
					if (checkFFA(user.ffaNum)) {
						ModelState.AddModelError("ffaNum", "FFA number alredy in system");
						shouldFail = true;
					}
				}
                if (!shouldFail)
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var storedUser = db.AspNetUsers.First(u => u.UserName == model.UserName);
						if (storedUser.ffaNum == 0)
							storedUser.ffaNum = null;

                        var client = new WebClient();
                        //var content = client.DownloadString("http://www.smsglobal.com/http-api.php?action=sendsms&user=hy8e6w5k&password=sbn74Yrw&&from=TM&to=61"+storedUser.phoneNum+"&text=Welcome&nbsp;to&nbsp;Tournament&nbsp;manager&nbsp;"+storedUser.UserName);


                        //ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
						/*
                        var idManager = new IdentityManager();
                        switch (model.Roles)
                        {
                            case "Referee":
                                REFEREE refComeWithUser = model.optionalRe.re;

                                refComeWithUser.userId = storedUser.Id;
                                refComeWithUser.USERQUALs.Clear();
                                foreach (var qual in model.optionalRe.srqvm.quals)
                                {
                                    QUALIFICATION thequal = db.QUALIFICATIONS.First(q => q.name == qual.qualName);

                                    if (qual.Selected == true)
                                    {
                                        USERQUAL newQual = new USERQUAL();
                                        newQual.qualificationId = thequal.qualificationId;
                                        refComeWithUser.USERQUALs.Add(newQual);
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
						*/
						storedUser.status = 1;
						await db.SaveChangesAsync();

                        bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
                     if (val1) //is logged in
                     {
                         return RedirectToAction("Index", "Account");
                     }
                     else //not logged in, log them in
                     {
                         LoginViewModel lvm = new LoginViewModel();
                         lvm.Password = model.Password;
                         lvm.UserName = model.UserName;
                         lvm.RememberMe = true;
                         await Login(lvm, "/Account/Index");
                         //return RedirectToAction("Index", "Account");
                     }

                        
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(result.ToString());
                        AddErrors(result);
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public ActionResult showProfile(string id)
        {
            var db = new ApplicationDbContext();

            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (val1) //is logged in
            {
                var user = db.Users.First(u => u.UserName == id);
                var model = new logindetialViewModel(user);
                return View(model);
            }
            else //not logged in
            {
                
            }
            return View();
        }

        private Boolean checkUsername(String name)
        {
            var db = new Raoconnection();

            if (db.AspNetUsers.Any(user => user.UserName == name))
            {
                return true;
            }
            return false;
        }
        private Boolean checkFFA(Nullable<int> ffa)
        {
            var db = new Raoconnection();

            if (db.AspNetUsers.Any(user => user.ffaNum == ffa))
            {
                return true;
            }
            return false;
        }
        private Boolean checkEmail(String email)
        {
            var db = new Raoconnection();

            if (db.AspNetUsers.Any(user => user.email == email))
            {
                return true;
            }
            return false;
        }

        public ActionResult showhead_Icon(string id)
        {
            var db = new ApplicationDbContext();

            var user = db.Users.First(u => u.UserName == id);
            var model = new logindetialViewModel(user);
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
        public async Task<Boolean> createUserFromExcel(RegisterViewModel listofUser, string type)
        {
            Boolean result = false;
            switch (type)
            {
                case "Referee":
                    result = await createListOfReferee(listofUser);
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
            var db = new Raoconnection();
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
                            refComeWithUser.userId = storedUser.Id;

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

                }
                else
                {
                    success = false; ;
                }
            }

            return success;

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