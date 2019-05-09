using Android.App;

namespace iDatech.WifiP2p.Poc.Permissions
{
    /// <summary>
    /// Handles permission related operations.
    /// </summary>
    public interface IPermissionService
    {
        #region Methods

        /// <summary>
        /// Indicates if a permission is granted for the current app.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <returns><c>true</c> if granted, <c>false</c> otherwise.</returns>
        bool IsPermissionGranted(string permission);

        /// <summary>
        /// Are a series of permissions all granted ?
        /// </summary>
        /// <param name="permissions">The list of permissions.</param>
        /// <returns><c>true</c> if granted, <c>false</c> otherwise.</returns>
        bool IsPermissionGroupGranted(string[] permissions);

        /// <summary>
        /// Request a specified permission for the app.
        /// </summary>
        /// <param name="permissionGroupId">The permission group ID (user defined).</param>
        /// <param name="permissions">The permissions.</param>
        /// <remarks>Check for the result of those permission requirements with <see cref="Activity.OnRequestPermissionsResult(int, string[], Android.Content.PM.Permission[])"/></remarks>
        void RequestPermissions(int permissionGroupId, string[] permissions);

        /// <summary>
        /// Request a specified permission for the app.
        /// </summary>
        /// <param name="permissionGroupId">The permission ID (user defined).</param>
        /// <param name="permissions">The permissions.</param>
        /// <param name="rationaleMessage">The message to display to the user when additional information is necessary for the permission.</param>
        /// <remarks>Check for the result of those permission requirements with <see cref="Activity.OnRequestPermissionsResult(int, string[], Android.Content.PM.Permission[])"/></remarks>
        void RequestPermission(int permissionGroupId, string[] permissions, string rationaleMessage);

        #endregion Methods
    }
}