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
      //  [HttpPost]
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
                                    modelfromExcel.dob = row["dob"].ToString();
                                    modelfromExcel.Roles = "Referee";
                                    modelfromExcel.optionalRe.re.distTravel = int.Parse(row["distTravel"].ToString());


                                    modelfromExcel.optionalRe.re.maxGames = int.Parse(row["maxGames"].ToString());
                                    new AccountController().createUserFromExcel(modelfromExcel, "Referee");
                                    modelfromExcel.optionalRe.re.sport = "Soccer";
                                    var storedUser = db.AspNetUsers.First(u => u.UserName == modelfromExcel.UserName).Id;
                                    modelfromExcel.optionalRe.re.userId = storedUser;

                                    db.REFEREEs.Add(modelfromExcel.optionalRe.re);

                                    db.SaveChanges();
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
         public  void importXmll() {

            //var excel = Request.Files[0];
            string type = Request.Form["type"];
           

                try {
                   // string fileName = Path.Combine(Server.MapPath("~/Excel"), Guid.NewGuid().ToString() + Path.GetExtension(excel.FileName));
                    string fileName = "C:\\Users\\kang\\Source\\Repos\\userprofile\\userprofile\\Excel\\17c0a4f5-2d53-4fa2-829d-5a4acd2598b6.xls";
                   
                    String conString = " ";
                    string ext = Path.GetExtension(fileName);
                    if (ext.ToLower()==".xls")

                    {
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\""; 
                    }
                    else if(ext.ToLower()==".xlsx")
                    {
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\""; 
                    }
                    string query = "select * from [Sheet1$]";
                    OleDbConnection con = new OleDbConnection(conString);
                    if (con.State == System.Data.ConnectionState.Closed) {
                        con.Open();
                    }
                    OleDbCommand cmd = new OleDbCommand(query, con);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    
                    da.Fill(ds);
                    da.Dispose();
                    con.Close();
                    con.Dispose();
                   
                    var test = ds.Tables[0].Rows;
                    using (Raoconnection db=new Raoconnection())
                    { switch ("user")
                        {
                        case "sport":
                          foreach(DataRow row in ds.Tables[0].Rows){
                        string newname = row["name"].ToString();
                        if (db.SPORTs.Find(newname) == null) {
                            db.SPORTs.Add(new SPORT { name = newname });

                        }
                       }
                       break;
                        case "user":
                            RegisterViewModel modelfromExcel;
                            List<RegisterViewModel> listuser = new List<RegisterViewModel>();
                          foreach (DataRow row in ds.Tables[0].Rows) {
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
                              modelfromExcel.dob = row["dob"].ToString();
                              modelfromExcel.Roles = "Referee";
                              modelfromExcel.optionalRe.re.distTravel = int.Parse(row["distTravel"].ToString());
                              
                              
                              modelfromExcel.optionalRe.re.maxGames = int.Parse(row["maxGames"].ToString());
                              new AccountController().createUserFromExcel(modelfromExcel, "Referee");
                              modelfromExcel.optionalRe.re.sport = "Soccer";
                              var storedUser = db.AspNetUsers.First(u => u.UserName == modelfromExcel.UserName).Id;
                              modelfromExcel.optionalRe.re.userId = storedUser;
                            
                              db.REFEREEs.Add(modelfromExcel.optionalRe.re);
                              
                              db.SaveChanges();
                          }
                             

                          break;
                        default:
                          break;
                        }
                    
                    db.SaveChanges();
                    }

                    if (System.IO.File.Exists(fileName)) {
                      //  System.IO.File.Delete(fileName);
                    }
                }
                catch(Exception) {

                    throw;
                }
            }
            
            
        }
	}


