using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JurisUtilityBase
{
    public class BillingField
    {
        public string name { get; set; } // name of clibillingfield (on client table)
        public int length { get; set; } // max length of field
        public string text { get; set; } // actual text that is in textbox and goes into field

        public string whichBox { get; set; } // track which billing field is in which box. Currently BillingField 05 can be in richtextbox 2 or 4 or 5...depends on whats defined in sysparam

        public bool isRequired { get; set; } //specifically for UDFs

        public string UDFtype { get; set; }
    }
}
