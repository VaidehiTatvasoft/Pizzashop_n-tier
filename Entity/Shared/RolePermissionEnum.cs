namespace Entity.Shared;


public class RolePermissionEnum
{
    public enum Permission
    {
        // Users
        Users_CanView = 1,
        Users_CanEdit = 2,
        Users_CanDelete = 3,

        // RolesAndPermissions
        RolesAndPermissions_CanView = 4,
        RolesAndPermissions_CanEdit = 5,
        RolesAndPermissions_CanDelete = 6,

        // Menu
        Menu_CanView = 7,
        Menu_CanEdit = 8,
        Menu_CanDelete = 9,

        // TablesAndSections
        TablesAndSections_CanView = 10,
        TablesAndSections_CanEdit = 11,
        TablesAndSections_CanDelete = 12,

        // TaxesAndFees
        TaxesAndFees_CanView = 13,
        TaxesAndFees_CanEdit = 14,
        TaxesAndFees_CanDelete = 15,

        // Orders
        Orders_CanView = 16,
        Orders_CanEdit = 17,
        Orders_CanDelete = 18,

        // Customers
        Customers_CanView = 19,
        Customers_CanEdit = 20,
        Customers_CanDelete = 21
    }
}
