// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace TQM.Backoffice.Domain.Enums;

public enum PermissionEnum : short
{
    [Display(GroupName = "Sale", Name = "POST", Description = "Update Sale Status")]
    SaleSubmitOrder = 0x001,
    [Display(GroupName = "Sale", Name = "GET", Description = "Get Sale Test")]
    GetSaleTest2 = 0x005,

    [Display(GroupName = "Task", Name = "GET", Description = "Get Action Alert")]
    GetActionAlert = 0x002,
    [Display(GroupName = "AdminVerify", Name = "POST", Description = "GetSaleListConfirm")]
    GetSaleListConfirm = 0x003,
    [Display(GroupName = "AdminVerify", Name = "GET", Description = "SaleInquiry")]
    SaleInquiry = 0x004,
    [Display(GroupName = "AdminVerify", Name = "GET", Description = "GetAdminVerifyMaster")]
    GetAdminVerifyMaster = 0x006,
    [Display(GroupName = "AdminVerify", Name = "POST", Description = "UpdateSalePaymentEasyLending")]
    UpdateSalePaymentEasyLending = 0x007
}
