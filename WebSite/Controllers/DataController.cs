﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advokates_CRM_BL_Models.Inputs;
using Advokates_CRM_BL_Models.Outputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM.BL.Helpers;

namespace Avokates_CRM.Controllers
{
    [Authorize]
    public class DataController : BaseController
    {
        private readonly IDataLayer dl;      // dataLayer
        public DataController(IDataLayer dataLayer)      // в Startup : AddScoped<IDataLayer, DataLayer>();
        {
            dl = dataLayer;
        }

        public IActionResult Test([FromBody] BaseAuth_In auth)
        {
            var c = HttpContext.Request;
            if (HelperSecurity.IsTokenValid(auth.Token))
            {
                GetCasesList_Out result = dl.GetCasesList(auth.Token);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());

        }

        [Authorize(Roles = "admin, director")]
        public IActionResult CreateInvite(string email)
        {          
            ResultBase result = dl.CreateInvite(GetToken(), email);
            return Ok(result);
        }

        // Создание нового юзера по пригласительному токену
        [AllowAnonymous]
        public IActionResult CreateUserByInvite(Registration_In value)
        {
            Registration_Out result = dl.CreateUserByInvite(value);
            if (result.Status == ResultBase.StatusOk)
            {
                HttpContext.Session.SetString("token", result.JWT);
            }
            return Ok(result);
        }


//----------------------------------------------    ДЕЛО    -------------------------------------------------------
        #region CASE

        public IActionResult GetCasesList()
        {            
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                GetCasesList_Out result = dl.GetCasesList(token);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());
        }

        public IActionResult NewCaseGetModel()
        {
            string token = GetToken();
            NewCaseGetModel_Out result = dl.NewCaseGetModel(token);
            return Ok(result);
        }

        #region CASE_FIGURANTS

        public IActionResult AddNewFigurantToCase(NewCase_In figurant, Guid caseUid, string privateKey)
        {
            string token = GetToken();
            ResultBase result = dl.AddNewFigurantToCase(token, figurant, caseUid, privateKey);
            return Ok(result);
        }
        public IActionResult RemoveFigurantFromCase( Guid caseUid, Guid figurantUid)
        {
            string token = GetToken();
            ResultBase result = dl.RemoveFigurantFromCase(token, caseUid, figurantUid);
            return Ok(result);
        }

        #endregion

        public IActionResult GetCaseNotes(Guid caseUid, string privateKey)
        {
            string token = GetToken();
            GetCaseNotes_Out result = dl.GetCaseNotes(token, caseUid, privateKey);
            return Ok(result);
        }

        #region NOTE

        public IActionResult AddNewNoteToCase(NewNote_In note, IFormFile[] files, Guid caseUid, string privateKey)
        {
            string token = GetToken();
            ResultBase result = dl.AddNewNoteToCase(token, note, files, caseUid, privateKey);
            return Ok(result);
        }

        public IActionResult RemoveNoteFromCase(Guid caseUid, Guid noteUid)
        {
            string token = GetToken();
            ResultBase result = dl.RemoveNoteFromCase(token, caseUid, noteUid);
            return Ok(result);
        }
        
        #endregion



        public IActionResult CreateNewCase(NewCase_In value)
        {
            value.Token = GetToken();
            ResultBase result = dl.CreateNewCase(value);
            return Ok(result);
        }
        public IActionResult GetCaseInfo(int id,string privateKey)
        {
            string token = GetToken();
            GetCase_Out result = dl.GetCase(token, id, privateKey);
            return Ok(result);
        }

        public IActionResult GrantAccessToCase(Guid userUid, Guid caseUid, string privateKey)
        {
            string token = GetToken();
            ResultBase result = dl.GrantAccessToCase(token, userUid, caseUid, privateKey);
            return Ok(result);
        }

        public IActionResult RemoveAccessToCase(Guid userUid, Guid caseUid)
        {
            string token = GetToken();
            ResultBase result = dl.RemoveAccessToCase(token, userUid, caseUid);
            return Ok(result);
        }

#endregion

//-------------------------------------------------------------------------------------------------------------------------------------

        public IActionResult GetMainPage()
        {
            GetMainPage_Out result = dl.GetMainPage(GetToken());
            return Ok(result);
        }

        #region CABINET

        public IActionResult GetCabinetInfo()
        {
            string token = GetToken();
            GetCabinetInfo_Out result  = dl.GetCabinetInfo(token);
            return Ok(result);

        }

        public IActionResult CabinetInfoSaveChanges(GetCabinetInfo_Out cabinetInfo)
        {
            string token = GetToken();
            ResultBase result = dl.CabinetInfoSaveChanges(token, cabinetInfo);
            return Ok(result);
        }

        #endregion
    }
}