﻿using NNCQ.Domain.Application;
using NNCQ.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NNCQ.Web.Controllers.Application
{
    public class SystemWorkTaskController:Controller
    {
        private readonly IEntityRepository<SystemWorkTask> _Service;

        public SystemWorkTaskController(IEntityRepository<SystemWorkTask> service)
        {
            this._Service = service;
        }

        public ActionResult Index() 
        {
            return View("../../Views/Admin/Common/Index");
        }
    }
}