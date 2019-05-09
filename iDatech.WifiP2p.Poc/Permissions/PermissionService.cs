using Android.App;
using Android.Content.PM;
using System;
using System.Linq;

namespace iDatech.WifiP2p.Poc.Permissions
{
    /// <summary>
    /// Default implementation of <see cref="IPermissionService"/>
    /// </summary>
    public class PermissionService : IPermissionService
    {
        #region Instance variables

        /// <summary>
        /// The activity used to create the service.
        /// </summary>
        readonly private Activity m_Activity;

        #endregion Instance variables

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="activity">The calling activity.</param>
        public PermissionService(Activity activity)
        {
            m_Activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// <see cref="IPermissionService.IsPermissionGranted(string)"/>
        /// </summary>
        public bool IsPermissionGranted(string permission)
        {
            return m_Activity.CheckSelfPermission(permission) == (int)Permission.Granted;
        }

        /// <summary>
        /// <see cref="IPermissionService.IsPermissionGroupGranted(string[])"/>
        /// </summary>
        public bool IsPermissionGroupGranted(string[] permissions)
        {
            if (permissions == null)
            {
                throw new ArgumentNullException(nameof(permissions));
            }

            foreach(string permission in permissions)
            {
                if (!IsPermissionGranted(permission))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// <see cref="IPermissionService.RequestPermissions(int, string[])"/>
        /// </summary>
        public void RequestPermissions(int permissionGroupId, string[] permissions)
        {
            RequestPermission(permissionGroupId, permissions, m_Activity.GetString(Resource.String.default_permission_rationale_message));
        }

        /// <summary>
        /// <see cref="IPermissionService.RequestPermission(int, string, string)"/>
        /// </summary>
        public void RequestPermission(int permissionGroupId, string[] permissions, string rationaleMessage)
        {
            if (permissions == null)
            {
                throw new ArgumentNullException(nameof(permissions));
            }

            bool allGranted = true;
            string neededPermission = permissions.First();

            foreach (string permission in permissions)
            {
                if (!IsPermissionGranted(permission))
                {
                    allGranted = false;
                    neededPermission = permission;
                    break;
                }
            }

            if (allGranted)
            {
                return;
            }

            // If user has already denied the permission once, show a more explicit message.
            if (m_Activity.ShouldShowRequestPermissionRationale(neededPermission))
            {
                // Create an alert
                AlertDialog.Builder alert = new AlertDialog.Builder(m_Activity);
                alert.SetTitle("Permissions requises");
                alert.SetMessage(rationaleMessage);
                alert.SetPositiveButton("Autoriser", (e, args) =>
                {
                    m_Activity.RequestPermissions(permissions, permissionGroupId);
                });

                alert.SetNegativeButton("Refuser", (e, args) => { });

                Dialog dialog = alert.Create();
                dialog.Show();

                return;
            }

            m_Activity.RequestPermissions(permissions, permissionGroupId);
        }        

        #endregion Methods
    }
}