using System.Text.Json.Serialization;

namespace TodoList.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        [JsonIgnore]
        public Category? ParentCategory { get; set; } 

        [JsonIgnore]
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();
    }

}