namespace ProjBlog.Models
{
    using System;
    using System.Collections.Generic;

    public interface IEntity
    {
        int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
