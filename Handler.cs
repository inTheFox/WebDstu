using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json;

namespace WebDstu
{
    public class Handler
    {
        public Action<string, System.Dynamic.ExpandoObject> onDataUpdate;

        public dynamic DynamicData = new System.Dynamic.ExpandoObject();
        delegate void CallbackUpdate(dynamic update);

        public async void StartHandler()
        {
            this.onDataUpdate = (string updateType, System.Dynamic.ExpandoObject obj) =>
            {
                switch (updateType)
                {
                    case "ActionUpdate":
                        CallbackUpdate callback = (dynamic update) => { return; };
                        dynamic args = obj;
                        dynamic updatedAction = args.Action;
                        dynamic changesList = args.Changes;


                        if (args.Delete)
                        {
                            callback = DeleteAction;
                        }
                        if (args.UpdateDescription)
                        {
                            callback = UpdateAction;
                        }
                        if (args.CreateAction)
                        {
                            callback = CreateNewAction;
                        }
                        if (args.AddSubAction)
                        {
                            callback = AddSubAction;
                        }

                        callback(args.Changes);
                        DynamicData = obj;
                       
                        break;
                    default:
                        return;
                    ////////
                }
            };
        }

        public void DeleteAction(dynamic update)
        {
            Console.WriteLine("DeleteAction");
            Console.WriteLine(JsonConvert.SerializeObject(update));
        }
        public void UpdateAction(dynamic update)
        {
            Console.WriteLine("UpdateAction");
            Console.WriteLine(JsonConvert.SerializeObject(update));
        }
        public void CreateNewAction(dynamic update)
        {
            Console.WriteLine("CreateNewAction");
            Console.WriteLine(JsonConvert.SerializeObject(update));
        }
        public void AddSubAction(dynamic update)
        {
            Console.WriteLine("AddSubAction");
            Console.WriteLine(JsonConvert.SerializeObject(update));
        }


    }
}
