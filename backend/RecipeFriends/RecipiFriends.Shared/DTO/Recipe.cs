using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeFriends.Shared.DTO
{
    public class Recipe
    {
        public int Id { get; set; }
        
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Content { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
