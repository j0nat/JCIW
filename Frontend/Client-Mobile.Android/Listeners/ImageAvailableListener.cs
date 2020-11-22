using Android.Graphics;
using Android.Media;
using Java.IO;
using Java.Lang;
using Java.Nio;

// CODE TAKEN FROM https://docs.microsoft.com/en-us/samples/xamarin/monodroid-samples/android50-camera2basic/
// 25.09.2020

namespace Client_Mobile.Android.Listeners
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        public ImageAvailableListener(Camera2BasicFragment fragment, File file)
        {
            if (fragment == null)
                throw new System.ArgumentNullException("fragment");
            if (file == null)
                throw new System.ArgumentNullException("file");

            owner = fragment;
            this.file = file;
        }

        private readonly File file;
        private readonly Camera2BasicFragment owner;

        //public File File { get; private set; }
        //public Camera2BasicFragment Owner { get; private set; }

        public void OnImageAvailable(ImageReader reader)
        {
            owner.mBackgroundHandler.Post(new ImageSaver(reader.AcquireNextImage(), file, owner));
        }

        // Saves a JPEG {@link Image} into the specified {@link File}.
        private class ImageSaver : Java.Lang.Object, IRunnable
        {
            // The JPEG image
            private Image mImage;

            // The file we save the image into.
            private File mFile;

            private Camera2BasicFragment owner;

            public ImageSaver(Image image, File file, Camera2BasicFragment fragment)
            {
                if (image == null)
                    throw new System.ArgumentNullException("image");
                if (file == null)
                    throw new System.ArgumentNullException("file");

                mImage = image;
                mFile = file;
                owner = fragment;
            }

            public void Run()
            {
                ByteBuffer buffer = mImage.GetPlanes()[0].Buffer;

                //int rotation = 0;
                int rotation = (int)owner.Activity.WindowManager.DefaultDisplay.Rotation;

                System.Diagnostics.Debug.WriteLine("PHONE ROTATION: " + rotation);
             //   System.Diagnostics.Debug.WriteLine(owner.GetOrientation(rotation).ToString());

                byte[] bytes = new byte[buffer.Remaining()];
                buffer.Get(bytes);

                Bitmap bmp = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                Bitmap rotated = resizeAndRotate(bmp, owner.GetOrientation(rotation * -1));

                byte[] bitmapData;
                using (var stream = new System.IO.MemoryStream())
                {
                    rotated.Compress(Bitmap.CompressFormat.Jpeg, 70, stream);
                    bitmapData = stream.ToArray();
                }

                AndroidFunctions.ByteArrayLastImageCaptured = bitmapData;

                owner.FinishActivity();
            }

            public Bitmap resizeAndRotate(Bitmap image, int orientation)
            {
                var matrix = new Matrix();
                matrix.PostRotate(orientation);
                return Bitmap.CreateBitmap(image, 0, 0, image.Width, image.Height, matrix, true);
            }


        }
    }
}