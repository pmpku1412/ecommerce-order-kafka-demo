using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQM.Backoffice.Application.Helpers;

    public class CommonVariable
{
    }

    public class PaymentVariable
{
    public static decimal VatRate { get; set; } = Convert.ToDecimal( 0.07 );
    public static decimal DutyRate { get; set; } = Convert.ToDecimal( 0.4 );
}



