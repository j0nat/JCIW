
using Android.App;
using Android.Content;
using Android.OS;

// CODE TAKEN FROM https://docs.microsoft.com/en-us/samples/xamarin/monodroid-samples/android50-camera2basic/
// 25.09.2020

namespace Client_Mobile.Android
{
    public class ErrorDialog : DialogFragment
    {
        private static readonly string ARG_MESSAGE = "message";
        private static Activity mActivity;

        private class PositiveListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public void OnClick(IDialogInterface dialog, int which)
            {
                mActivity.Finish();
            }
        }

        public static ErrorDialog NewInstance(string message)
        {
            var args = new Bundle();
            args.PutString(ARG_MESSAGE, message);
            return new ErrorDialog { Arguments = args };
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            mActivity = Activity;
            return new AlertDialog.Builder(mActivity)
                .SetMessage(Arguments.GetString(ARG_MESSAGE))
                .SetPositiveButton("Ok", new PositiveListener())
                .Create();
        }
    }
}