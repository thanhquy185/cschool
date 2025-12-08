public class RoleDetailRowFormat
{
    public int FunctionId { get; set; }
    public string FunctionName { get; set; }
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }

    public bool AllowView { get; set; }
    public bool AllowCreate { get; set; }
    public bool AllowUpdate { get; set; }
    public bool AllowDelete { get; set; }
}
