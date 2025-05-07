namespace CmsPages
{
    public static class CmsPagesDomainErrorCodes
    {
        public const string PageNotFound = "CmsPages:PageNotFound";
        public const string PageCreationFailed = "CmsPages:PageCreationFailed";
        public const string PageCreationFailedExistingRoute = "CmsPages:PageCreationFailedRouteAlreadyExists";
        public const string PageUpdateFailed = "CmsPages:PageUpdateFailed";
        public const string PageUpdateFailedExistingRoute = "CmsPages:PageUpdateFailedRouteAlreadyExists";
    }
}