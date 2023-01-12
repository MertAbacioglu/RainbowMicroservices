namespace FreeCourse.Services.Catalog.Settings
{
    public interface IDatabaseSetting
    {
        string CourseCollectionName { get; set; }
        string CategoryCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }

    }
}
