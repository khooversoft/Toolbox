// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Security;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Khooversoft.AspMvc
{
    public class HmacAuthenticateAttribute : ActionFilterAttribute
    {
        public HmacAuthenticateAttribute()
        {
        }

        public bool AllowAnonymous { get; set; }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            HmacIdentity hmac = context.HttpContext.Items.Get<HmacIdentity>();
            if (hmac != null)
            {
                return base.OnActionExecutionAsync(context, next);
            }

            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (context.Controller == null || controllerActionDescriptor == null)
            {
                if (!AllowAnonymous)
                {
                    context.Result = new ContentResult { StatusCode = (int)HttpStatusCode.Forbidden };
                }

                return base.OnActionExecutionAsync(context, next);
            }

            HmacAuthenticateAttribute attr;

            // Check method
            attr = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(HmacAuthenticateAttribute), false)
                .OfType<HmacAuthenticateAttribute>()
                .FirstOrDefault();

            if (attr?.AllowAnonymous == true)
            {
                return base.OnActionExecutionAsync(context, next);
            }

            // Check controller
            attr = context.Controller.GetType()
                .GetCustomAttributes(typeof(HmacAuthenticateAttribute), false)
                .OfType<HmacAuthenticateAttribute>()
                .FirstOrDefault();

            if (attr?.AllowAnonymous == true)
            {
                return base.OnActionExecutionAsync(context, next);
            }

            context.Result = new ContentResult { StatusCode = (int)HttpStatusCode.Forbidden };
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
