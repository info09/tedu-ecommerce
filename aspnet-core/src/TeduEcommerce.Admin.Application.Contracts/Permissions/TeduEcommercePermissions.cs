﻿namespace TeduEcommerce.Permissions;

public static class TeduEcommercePermissions
{
    public const string GroupName = "TeduEcommerce";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public const string SystemGroupName = "TeduEcomAdminSystem";
    public const string CatalogGroupName = "TeduEcomAdminCatalog";

    public static class Role
    {
        public const string Default = SystemGroupName + ".Role";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class User
    {
        public const string Default = SystemGroupName + ".User";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Product
    {
        public const string Default = CatalogGroupName + ".Product";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string AttributeManage = Default + ".Attribute";

    }

    public static class Attribute
    {
        public const string Default = CatalogGroupName + ".Attribute";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
}
