using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;
using System.Data.OleDb;

namespace userprofile.Controllers
{
    public class ExcelManagementController : Controller
    {
        //
        // GET: /ExcelManagement/
        public ActionResult Index()
        {


            return View();
        }
        public FileResult Download()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"C:\Users\kang\Source\Repos\userprofile\userprofile\Excel\sampleExcel.xls");
            string fileName = "sampleExcel.xls";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        //  [HttpPost]
        public string importXml()
        {
            var message = "";
            var inall="";
            var i = 0;
            var success = 0;
            var fails = 0;
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
                            case "Match":
                                MATCH newMatch;
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    i++;

                                    
                                }
                                break;
                            case "user":
                                RegisterViewModel modelfromExcel;
                                List<RegisterViewModel> listuser = new List<RegisterViewModel>();
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    i++;
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
                                    modelfromExcel.optionalRe.re.rating = int.Parse(row["rating"].ToString());
                                    modelfromExcel.residentLoc.street = row["street"].ToString();
                                    modelfromExcel.residentLoc.city = row["city"].ToString();
                                    modelfromExcel.residentLoc.state = row["state"].ToString();
                                    modelfromExcel.residentLoc.country = row["country"].ToString();
                                    modelfromExcel.residentLoc.postcode = int.Parse(row["postcode"].ToString());
                                    modelfromExcel.dob = row["dob"].ToString();
                                    modelfromExcel.Roles = "Referee";
                                    modelfromExcel.optionalRe.re.distTravel = int.Parse(row["distTravel"].ToString());


                                    modelfromExcel.optionalRe.re.maxGames = int.Parse(row["maxGames"].ToString());
                                    
                                    if (db.AspNetUsers.FirstOrDefault(u => u.UserName == modelfromExcel.UserName) == null)
                                    {
                                        if (db.AspNetUsers.FirstOrDefault(u => u.email == modelfromExcel.Email) == null)
                                        {
                                            new AccountController().createUserFromExcel(modelfromExcel, "Referee");
                                            message += "success at row" + i + "<br>";
                                            success++;
                                        }
                                       
                                        message += "duplicate email address at row" + i + "<br>";
                                        fails++;
                                    }
                                    else
                                    {
                                        message += "duplicate user name at row "+i+ "<br>";
                                        fails++;
                                    }

                                    //  var storedUser = db.AspNetUsers.First(u => u.UserName == modelfromExcel.UserName).Id;
                                    //modelfromExcel.optionalRe.re.userId = storedUser;

                                    // db.REFEREEs.Add(modelfromExcel.optionalRe.re);

                                    //  db.SaveChanges();
                                }
                                if (success == 0) {
                                    inall = "<h1>none user inserted</h1>";
                                    inall += message;
                                }
                                else
                                {
                                    inall = "<h1>" + success + "user successfully inserted </h1>";
                                    inall += "<h2>" + fails + "user is not valide</h2>";
                                    inall += message;
                                }

                                break;
                            default:
                                return "fail";
                                break;
                        }

                        db.SaveChanges();
                    }

                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }
                    return inall;
                }
                catch (Exception)
                {

                    return "fail";
                    throw;
                }
            }
            return "fail";
        }
        public string importXmll()
        {

            //var excel = Request.Files[0];
            return "hahah";
        }
    
}
}


