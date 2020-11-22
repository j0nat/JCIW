using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

// CODE TAKEN FROM https://docs.microsoft.com/en-us/samples/xamarin/monodroid-samples/android50-camera2basic/
// 25.09.2020

namespace Client_Mobile.Android
{
	[Activity (Label = "Camera2Basic", 
		ConfigurationChanges = ConfigChanges.Orientation,
		MainLauncher = false, Icon = "@mipmap/ic_launcher")]
	public class CameraActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ActionBar.Hide ();

			if (bundle == null)
			{
				SetContentView(Resource.Layout.camera_layout);
				FragmentManager.BeginTransaction().Replace(Resource.Id.container, Camera2BasicFragment.NewInstance()).Commit();
			}
		}
	}
}


