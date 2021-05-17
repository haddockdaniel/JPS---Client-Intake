using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JurisUtilityBase
{
    public class ExceptionHandler
    {
        public ExceptionHandler()
        {

        }

        public string errorMessage { get; set; }
        public string errorSolution { get; set; }
    }
}
