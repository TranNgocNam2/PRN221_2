using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace FolderManagement
{
    public class Image
    {
        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                // Lưu trữ ảnh vào MemoryStream với định dạng BMP
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                // Tạo đối tượng BitmapImage từ MemoryStream
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        // Lưu trữ đối tượng BitmapImage vào một tệp tin hình ảnh (JPEG)
        public static void saveBitmapImageToFile(Object[] args)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            BitmapImage image = (BitmapImage)(args[0]);
            string filePath = Convert.ToString(args[1]);

            encoder.Frames.Add(BitmapFrame.Create(image));

            // Sử dụng FileStream để lưu trữ tệp tin hình ảnh
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        // Chuyển đổi đường dẫn hình ảnh thành đối tượng ImageSource
        public static ImageSource ConvertToImageSource(string imagePath)
        {
            try
            {
                // Tạo đối tượng BitmapImage từ đường dẫn hình ảnh
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();

                // Ép kiểu BitmapImage thành ImageSource và trả về
                ImageSource imageSource = bitmapImage as ImageSource;
                return imageSource;
            }
            catch (Exception ex)
            {
                // In ra thông báo lỗi nếu có vấn đề xảy ra
                Console.WriteLine("Error converting image to ImageSource: " + ex.Message);
                return null;
            }
        }

        public static System.Drawing.Icon GetFolderIcon()
        {
            // Kích thước của biểu tượng (ở đây là 32x32)
            const uint SHGFI_ICON = 0x100;
            const uint SHGFI_SMALLICON = 0x1;
            const uint SHGFI_LARGEICON = 0x0;

            // Đường dẫn đến thư mục
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            SHFILEINFO shfi = new SHFILEINFO();
            IntPtr hImgSmall = SHGetFileInfo(folderPath, 0, ref shfi, (uint)Marshal.SizeOf(shfi), SHGFI_ICON | SHGFI_SMALLICON);

            // Tạo một đối tượng Icon từ handle của biểu tượng
            System.Drawing.Icon folderIcon = System.Drawing.Icon.FromHandle(shfi.hIcon);

            return folderIcon;
        }

        // Cấu trúc SHFILEINFO
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        // Hàm SHGetFileInfo
        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

    }
}
