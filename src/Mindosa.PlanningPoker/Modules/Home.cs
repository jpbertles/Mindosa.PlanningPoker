using Mindosa.PlanningPoker.Infrastructure.Models;
using Mindosa.PlanningPoker.Infrastructure.Repositories;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.ModelBinding;

namespace Mindosa.PlanningPoker.Modules
{
    public class Home: NancyModule
    {
        private IGroupRepository _groupRepository;

        public Home(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;


            Get["/"] = ctx =>
                {
                    return View["index"];
                };

            Post["/GetStarted"] = ctx =>
                {
                    var postData = this.Bind<GettingStartedModel>();
                    if (string.IsNullOrWhiteSpace(postData.GroupName))
                    {
                        postData.GroupName = _groupRepository.CreateGroup();
                    }
                    
                    var response = Response.AsRedirect("/" + postData.GroupName);


                    if (!string.IsNullOrWhiteSpace(postData.UserName))
                    {
                        response.WithCookie("username", postData.UserName);
                    }

                    return response;
                };

            Get["/{groupName}"] = ctx =>
                {
                    return View["group", ctx.groupName.ToString()];
                };
        }
    }
}