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


using System.Data;
using System.Data.Entity;
using System.IO;

using System.Net;

using System.Data.OleDb;


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
                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            // If we got this far something failed, redisplay form
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
                var db = new Raoconnection();

                var user = db.AspNetUsers.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);


                if (model.Roles[0].Selected)
                {
                    user.isAdmin = true;
                    var roleMap = db.AspNetRoles.First(m => m.Name=="Admin");
                    user.AspNetRoles.Add(roleMap);

                }
                if (model.Roles[1].Selected)
                {
                    user.isPlayer = true;
                    var roleMap = db.AspNetRoles.First(m => m.Name == "Player"); //change to  == "Player"
                    user.AspNetRoles.Add(roleMap);

                }
                if (model.Roles[2].Selected)
                {
                    user.isReferee = true;
                    var roleMap = db.AspNetRoles.First(m => m.Name == "Referee");
                    user.AspNetRoles.Add(roleMap);

                }
                if (model.Roles[3].Selected)
                {
                    user.isOrganizer = true;
                    var roleMap = db.AspNetRoles.First(m => m.Name == "Organizer"); //change to  == "Organizer"
                    user.AspNetRoles.Add(roleMap);
                   
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
            
            var db = new Raoconnection();
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
        public ActionResult showProfile(string id) { 
         var db=new ApplicationDbContext();
            
            var user=db.Users.First(u=>u.UserName==id);
            var model=new logindetialViewModel(user);
            return View(model);
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
        //public async Task<Boolean> createUserFromExcel(RegisterViewModel listofUser, string type)
        //{
        //    Boolean result = false;
        //    switch (type)
        //    {
        //        case "Referee":
        //           result= await createListOfReferee(listofUser);
        //            break;
        //        default:
        //            break;
        //    }
        //    // If we got this far, something failed, redisplay form
        //    return result;
        //}
        public async Task<Boolean> createListOfReferee(RegisterViewModel model)
        {
            Boolean success = true;
            var db = new Raoconnection();
            string location = @"~\userprofile\default.png";


            model.photoDir = location;

            if (ModelState.IsValid)
            {
                var user = model.GetUser();
                
                    user.city = "wollongong";
                
                var j= 0;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (true)
                {
                    var storedUser = db.AspNetUsers.First(u => u.UserName == model.UserName);
                    var idManager = new IdentityManager();
                    switch (model.Roles)
                    {
                        case "Referee":
                            REFEREE refComeWithUser = model.optionalRe.re;
                            refComeWithUser.sport = "Soccor";
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

                    // await SignInAsync(user, isPersistent: false);


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
        public void importXml()
        {

            var excel = Request.Files[0];
            string type = Request.Form["type"];

            if (excel.ContentType == "application/vnd.ms-excel" || excel.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                try
                {
                    string fileName = Path.Combine(Server.MapPath("~/Excel"), Guid.NewGuid().ToString() + Path.GetExtension(excel.FileName));

                    excel.SaveAs(fileName);
                    String conString = " ";
                    string ext = Path.GetExtension(excel.FileName);
                    if (ext.ToLower() == ".xls")
                    {
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (ext.ToLower() == ".xlsx")
                    {
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    string query = "select * from [Sheet1$]";
                    OleDbConnection con = new OleDbConnection(conString);
                    if (con.State == System.Data.ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    OleDbCommand cmd = new OleDbCommand(query, con);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    da.Fill(ds);
                    da.Dispose();
                    con.Close();
                    con.Dispose();
                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }

                    var test = ds.Tables[0].Rows;
                    using (Raoconnection db = new Raoconnection())
                    {
                        switch (type)
                        {
                            case "sport":
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    string newname = row["name"].ToString();
                                    if (db.SPORTs.Find(newname) == null)
                                    {
                                        db.SPORTs.Add(new SPORT { name = newname });

                                    }
                                }
                                break;
                            case "user":
                                RegisterViewModel modelfromExcel;
                                List<RegisterViewModel> listuser = new List<RegisterViewModel>();
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    string username = row["UserName"].ToString();
                                    modelfromExcel = new RegisterViewModel();
                                    modelfromExcel.UserName = username;
                                    modelfromExcel.Password = row["Password"].ToString();
                                    modelfromExcel.ConfirmPassword = modelfromExcel.Password;
                                    modelfromExcel.FirstName = row["firstName"].ToString();
                                    modelfromExcel.LastName = row["lastName"].ToString();
                                    modelfromExcel.phoneNum = int.Parse(row["phoneNum"].ToString());
                                    modelfromExcel.Email = row["email"].ToString();
                                    modelfromExcel.residentLoc = new ResidentLoc();
                                    modelfromExcel.optionalRe = new REFEREEqualViewModel();
                                    modelfromExcel.optionalRe.re = new REFEREE();
                                    modelfromExcel.optionalRe.re.status = int.Parse(row["status"].ToString());
                                    modelfromExcel.optionalRe.re.status = int.Parse(row["rating"].ToString());
                                    modelfromExcel.residentLoc.street = row["street"].ToString();
                                    modelfromExcel.residentLoc.state = row["state"].ToString();
                                    modelfromExcel.residentLoc.country = row["country"].ToString();
                                    modelfromExcel.residentLoc.postcode = int.Parse(row["postcode"].ToString());
                                    modelfromExcel.ffaNum = int.Parse(row["ffanum"].ToString());
                                    modelfromExcel.dob = row["dob"].ToString();
                                    modelfromExcel.Roles = "Referee";
                                    modelfromExcel.optionalRe.re.distTravel = int.Parse(row["distTravel"].ToString());
                                    modelfromExcel.residentLoc.city = row["city"].ToString();

                                    modelfromExcel.optionalRe.re.maxGames = int.Parse(row["maxGames"].ToString());

                                 //   this.createListOfReferee(modelfromExcel);
                                    this.Register(modelfromExcel);
                                    modelfromExcel.optionalRe.re.sport = "Soccer";
                                    //    var storedUser = db.AspNetUsers.First(u => u.UserName == modelfromExcel.UserName).Id;
                                    //      modelfromExcel.optionalRe.re.userId = storedUser;

                                    //      db.REFEREEs.Add(modelfromExcel.optionalRe.re);

                                    //      db.SaveChanges();
                                }


                                break;
                            default:
                                break;
                        }

                        db.SaveChanges();
                    }

                    if (System.IO.File.Exists(fileName))
                    {
                        //  System.IO.File.Delete(fileName);
                    }
                }
                catch (Exception)
                {


                    throw;
                }
            }
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