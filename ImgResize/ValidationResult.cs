using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImgResize
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            ErrorMessages = new List<string>();
        }

        public List<string> ErrorMessages { get; private set; }

        public void AddErrorMessage(string message)
        {
            ErrorMessages.Add(message);
        }
    }
}
