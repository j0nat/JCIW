using System;

namespace JCIW.Data.Drawing
{
    /// <summary>
    /// This class is used to store image information.
    /// </summary>
    public class ImageSource
    {
        public byte[] Image { get; set; }
        public Vector2 ImageSize { get; set; }
        public IntPtr Texture { get; set; }
        public object MGTexture { get; set; }
    }
}
