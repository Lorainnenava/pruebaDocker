namespace MyApp.Presentation.MiddlewaresAndFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequiredPermissionAttribute : Attribute
    {
        public string Permission { get; }

        public RequiredPermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }

}
