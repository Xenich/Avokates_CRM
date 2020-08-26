﻿using Avokates_CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Models.Outputs
{
    public class ResultBase
    {
        public string Status { get; set; }
        public List<ErrorMessageResult> ErrorMessages { get; set; }

        public static string StatusOk = "ok";
        public static string StatusBad = "bad";
        public static string StatusDBError = "dbError";
        public static string BadToken = "Доступ к данным по данному токену отсутствует";
    }
}
