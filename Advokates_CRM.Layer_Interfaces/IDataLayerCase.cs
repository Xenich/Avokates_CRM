﻿using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using System;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface IDataLayerCase
    {
        GetCase_Out GetCase(string token, int caseId, string privateKey);
        ResultBase CreateNewCase(NewCase_In inputValue);
        NewCaseGetModel_Out NewCaseGetModel(string token);
        GetCaseNotes_Out GetCaseNotes(string token, Guid caseUid, string privateKey);
        ResultBase GrantAccessToCase(string token, Guid userUid, Guid caseUid, string privateKey);
        ResultBase RemoveAccessToCase(string token, Guid userUid, Guid caseUid);

    }
}
