namespace Forum.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Category
    {
        public Category()
        {
            this.Topics = new List<Topic>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display (Name = "Category Name")]
        public string Name { get; set; }
        
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Updated Date")]
        public DateTime LastUpdatedDate { get; set; }

        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public List<Topic> Topics { get; set; }

        [NotMapped]
        [Display(Name = "Number Comments")]
        public int NumberTopics => Topics.Count;

        public bool IsAuthor(string id)
        {
            return this.Author.UserName.Equals(id);
        }

        public bool IsTopicAuthor(string id)
        {
            return this.Author.UserName.Equals(id);
        }
    }
}
