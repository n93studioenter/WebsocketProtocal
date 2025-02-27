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
        public ActionResult FakeDistance()
        {
            return View();
        }
        public ActionResult Index()
        {
           
            var db = new siyosane_uwb_prototypeEntities();
            ViewBag.lstSite = db.tb_Site.Where(m => m.ID == 1).FirstOrDefault();
            ViewBag.listfloor = db.tb_Floorplan.Where(m => m.SiteID == 1).FirstOrDefault();
            return View();
        }
        public string CalculateDistances(float fX, float fY, float node1X, float node1Y, float node2X, float node2Y, float node3X, float node3Y)
        {
            // Tính khoảng cách từ F đến A
            float distanceToA = CalculateDistance(fX, fY, node1X, node1Y);
            // Tính khoảng cách từ F đến B
            float distanceToB = CalculateDistance(fX, fY, node2X, node2Y);
            // Tính khoảng cách từ F đến C
            float distanceToC = CalculateDistance(fX, fY, node3X, node3Y);
            return distanceToA + "|" + distanceToB + "|" + distanceToC;
        }
        private static float CalculateDistance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
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