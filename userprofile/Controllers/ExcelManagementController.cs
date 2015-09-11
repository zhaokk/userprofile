using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;

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

        public string importXml(excelModel newmodel) {


            return "";
        }
	}
}