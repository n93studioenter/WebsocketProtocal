using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsocketProtocal.Models;
using static WebSocketServer;

namespace WebsocketProtocal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //test thuật toán tam giác
            var getpoint = Helper.getDeviceLocation(2000,7000,3000,0,8,0,0,0,7); 
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public JsonResult findPointTag(string data)
        {
            List<Tag> tags = JsonConvert.DeserializeObject<List<Tag>>(data);
            return Json(tags, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getDevicebyname(string DeviceName)
        {
            var getsplit = DeviceName.Split(' ');
            var db = new siyosane_uwb_prototypeEntities();
            var tb_Device = db.tb_Device.ToList().Where(m => m.DeviceID.Contains(getsplit[0].Trim())).FirstOrDefault();
            return Json(tb_Device, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}