i have this model for Permission of the user 
public partial class AppUserFormPermission
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FormId { get; set; }
    public bool AllowRead { get; set; }
    public bool AllowWrite { get; set; }
    public bool? AllowDelete { get; set; }
    public bool? AllowAll { get; set; }
    public bool? AllowModify { get; set; }
    public bool DownTime { get; set; }
}

this is the data in my table 
dffd1b83-9359-4f69-883b-a23247084ed3	15fae783-d72d-468b-b75c-e388dc0f9b7e	d48679a0-f8ba-4658-bb9e-c564e95da013	True	True	True	True	True	False

every form has there FormId , and UserId of the , check if the user has the permission of the page , if the permission then he can open the page but if not then shows access denied
