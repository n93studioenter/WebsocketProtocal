using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsocketProtocal.Models;
namespace WebsocketProtocal.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        public ActionResult Index()
        {
            siyosane_uwb_prototypeEntities db = new siyosane_uwb_prototypeEntities();
            ViewBag.ListDevice = db.tb_Device.ToList();
            return View();
        }
        public ActionResult IndexTag()
        {
            return View();
        }
        public JsonResult getDevicebyname(string DeviceName)
        {
            var getsplit = DeviceName.Split(' ');
            var db = new siyosane_uwb_prototypeEntities();
            var tb_Device = db.tb_Device.ToList().Where(m => m.DeviceID.Contains(getsplit[0].Trim())).FirstOrDefault();
            return Json(tb_Device, JsonRequestBehavior.AllowGet);
        }
    }
}