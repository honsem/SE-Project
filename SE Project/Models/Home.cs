using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE_Project.Models
{
    public class Home
    {
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public string URL { get; set; }
        public List<MenuItem> Children { get; set; }
    } 

    public class MenuItemViewModel
    {
        public List<MenuItem> MenuItems { get; set; }
    }
}