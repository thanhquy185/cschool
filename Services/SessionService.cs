namespace Services;

public static class SessionService
{
    public static UserModel currentUserLogin { get; set; }
    public static bool IsLoggedIn => currentUserLogin != null;

    public static void Logout()
    {
        currentUserLogin = null;
    }
}
