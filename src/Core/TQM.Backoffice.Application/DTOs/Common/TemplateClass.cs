using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQM.Backoffice.Application.DTOs.Common
{
    public class TemplateClass
    {
        public static DateTime defaultDateTime = new DateTime(1900, 1, 1);

        [Serializable]
        public class ErrorTemplate
        {
            public List<string> Message { get; set; }
            public Boolean IsOK { get; set; }

            public ErrorTemplate()
            {
                this.Message = new List<string>();
                this.IsOK = false;
            }
        }
    }
}
