using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wine_Shop_Inventory_Application.Models
{
    public class UserViewModel
    {
    }

    public class CurrentUser
    {
        public int UserId { get; set; }
        public int? RoleId { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}