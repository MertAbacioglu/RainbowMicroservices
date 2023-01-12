﻿using FreeCourse.Services.Catalog.Dtos.Feature;

namespace FreeCourse.Services.Catalog.Dtos.Course
{
    public class CourseCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public decimal Price { get; set; }
        public FeatureDto Feature { get; set; }
        public string CategoryId { get; set; }
        public string Picture { get; set; }

    }
}
