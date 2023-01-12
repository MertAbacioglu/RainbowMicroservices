using FreeCourse.Services.Catalog.Dtos.Feature;

namespace FreeCourse.Services.Catalog.Dtos.Course
{
    public class CourseUpdateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public FeatureDto Feature { get; set; }
        public int CategoryId { get; set; }
        public string Picture { get; set; }
    }
}
