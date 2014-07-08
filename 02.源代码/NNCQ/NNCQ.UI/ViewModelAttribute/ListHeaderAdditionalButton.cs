﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNCQ.UI.ViewModelAttribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ListHeaderAdditionalButton:Attribute
    {
        public string DisplayName { get; set; }
        public string OnClickFunction { get; set; }
        public int Width { get; set; }

        public ListHeaderAdditionalButton( string name, string clickFunction,int width) 
        {
            DisplayName = name;
            OnClickFunction = clickFunction;
            Width = width;
        }
    }
}
