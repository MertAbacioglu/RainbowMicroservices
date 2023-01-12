﻿using FreeCourse.Services.Catalog.Dtos.Category;
using FreeCourse.Services.Catalog.Dtos.Feature;

namespace FreeCourse.Services.Catalog.Dtos.Course
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Picture { get; set; }
        public decimal Price { get; set; }
        public FeatureDto Feature { get; set; }
        public CategoryDto Category { get; set; }
    }
}