using System;
using System.Collections.Generic;

namespace BookSwap.Models
{
    public class ProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ImagePath { get; set; }
        public List<LendHistory> LendBooks { get; set; }   
        public bool IsCurrentUser { get; set; }  

    }
}
