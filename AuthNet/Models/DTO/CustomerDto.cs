﻿using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.DTO
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
