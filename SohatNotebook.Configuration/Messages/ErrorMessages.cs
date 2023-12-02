namespace SohatNotebook.Configuration.Messages;

public static class ErrorMessages
{
    public static class Generic
    {
        public static string SomethingWentWrong = "Something went wrong, please try again later.";
        public static string UnableToProcess  = "Unable to process request";
        public static string InvalidPayload  = "Invalid payload.";
        public static string TypeBadRequest  = "Bad Request";
        public static string TypeNotFound  = "Not Found";
    }
    public static class Profile
    {
        public static string UserNotFound  = "User not found.";
    }

    public static class Users
    {
        public static string UserNotFound = "User not found.";
    }
}
