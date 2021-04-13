using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using WebDstu.Database;
using WebDstu.Models;

namespace WebDstu.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index(string login, string password)
        {

            return View();
        }


        [HttpPost]
        public ActionResult Movie(string action, int id)
        {
            if (Startup.saveds.ContainsKey(action))
            {
                Startup.saveds[action].SortId = id;
                //Startup.saveds.OrderBy(p => p.Value.SortId);

                //var sortedUsers = Startup.saveds.OrderBy(p => p.Value.SortId);
                Startup.saveds = Startup.saveds.OrderBy(pair => pair.Value.SortId).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> getUpdates()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                List<DSTUSaved> list = db.Saved.ToList();
                Startup.saveds.Clear();

                foreach (var item in list)
                {
                    Startup.saveds.Add(item.Action, item);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<ActionResult> UpdateDSTU()
        {
            //Startup.saveds.OrderBy(p => p.Value.SortId);

            await using (DatabaseContext db = new DatabaseContext())
            {
               foreach (var item in db.Saved.ToList())
               {
                   db.Remove(item);
               }

               await db.SaveChangesAsync();
            }

            await using (DatabaseContext db = new DatabaseContext())
            {
               foreach (var item in Startup.saveds)
               {
                   await db.Saved.AddAsync(item.Value);
               }

               await db.SaveChangesAsync();
            }

            await using (DatabaseContext db = new DatabaseContext())
            {
               Startup.saveds.Clear();
               foreach (var item in db.Saved.ToList())
               {
                   Startup.saveds.Add(item.Action, item);
               }
            }


            Startup.saveds = Startup.saveds.OrderBy(pair => pair.Value.SortId).ToDictionary(pair => pair.Key, pair => pair.Value);

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Upper()
        {
            Logger.Input("Update bot");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddAction()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddAction(string action = "", string desc = "")
        {
            Startup.saveds.Add(action, new DSTUSaved() { Action = action, OptionsJson = desc, SubActions = "null" });
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddSubAction(string addToAction)
        {
            ViewBag.AddToAction = addToAction;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddSubAction(string addToAction = "", string action = "", string desc = "")
        {
            if (Startup.saveds.ContainsKey(addToAction))
            {
                Dictionary<string, string> subActions;

                DSTUSaved item = Startup.saveds[addToAction];

                if (item.SubActions != "null")
                {
                    subActions = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.SubActions);
                }
                else
                {
                    subActions = new Dictionary<string, string>();
                }

                subActions.Add(action, desc);
                Startup.saveds[addToAction].SubActions = JsonConvert.SerializeObject(subActions);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<ActionResult> Delete(int id)
        {
            foreach (var item in Startup.saveds)
            {
                if (item.Value.Id == id)
                {
                    Startup.saveds.Remove(item.Key);
                    Startup.deletes.Add(id);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> EditSubAction(string action = "", string actionn = "", string subAction = "")
        {
            ViewBag.action = actionn;
            ViewBag.subAction = subAction;

            if (Startup.saveds.ContainsKey(actionn))
            {
                DSTUSaved item = Startup.saveds[actionn];

                if (item.SubActions != "null")
                {
                    Dictionary<string, string> subActions = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.SubActions);

                    if (subActions.ContainsKey(subAction))
                    {
                        ViewBag.SubActionDesc = subActions[subAction];
                    }
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EditSubAction(string action = "", string subAction = "", string desc = "", string er = "")
        {
            if (Startup.saveds.ContainsKey(action))
            {
                DSTUSaved item = Startup.saveds[action];

                if (item.SubActions != "null")
                {
                    Dictionary<string, string> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.SubActions);

                    if (list.ContainsKey(subAction))
                    {
                        list[subAction] = desc;
                        Startup.saveds[action].SubActions = JsonConvert.SerializeObject(list);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<ActionResult> EditAction(string action = "", string actionn = "", string test = "")
        {
            if (Startup.saveds.ContainsKey(actionn))
            {
                ViewBag.Action = actionn;
                ViewBag.Desc = Startup.saveds[actionn].OptionsJson;
                return View();
            }

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<ActionResult> EditAction(string action = "", string newActionValue = "", string desc = "", string er = "")
        {
            if (Startup.saveds.ContainsKey(action))
            {
                if (action != newActionValue)
                {
                    Startup.saveds.Add(newActionValue, Startup.saveds[action]);
                    Startup.saveds.Remove(action);

                    Startup.saveds[newActionValue].Action = newActionValue;
                    Startup.saveds[newActionValue].OptionsJson = desc;

                }
                else
                {
                    Startup.saveds[action].OptionsJson = desc;
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DeleteSubAction(string tab = "", string subAction = "")
        {

            if (Startup.saveds.ContainsKey(tab))
            {
                if (Startup.saveds[tab].SubActions != "null")
                {
                    Dictionary<string, string> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(Startup.saveds[tab].SubActions);

                    if (list.ContainsKey(subAction))
                    {
                        list.Remove(subAction);

                        Startup.saveds[tab].SubActions = JsonConvert.SerializeObject(list);
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }
        
    }
}
