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
    }
}