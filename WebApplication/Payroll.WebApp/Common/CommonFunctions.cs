using Newtonsoft.Json;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.CommonDto;
using System.Drawing;
using System.Text;

namespace Payroll.WebApp.Common
{
    public static class CommonFunctions
    {
        public static Bitmap GenerateCaptchaImage(string captchaCode)
        {
            int width = 200;
            int height = 50;
            var random = new Random();
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);

            // Create a background with random noise
            graphics.Clear(System.Drawing.Color.Transparent);
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);
                bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)));
            }

            // Draw the captcha code
            var font = new System.Drawing.Font("Arial", 18, FontStyle.Regular);
            var brush = new SolidBrush(System.Drawing.Color.DarkBlue);

            // Draw each character with random rotation and position
            for (int i = 0; i < captchaCode.Length; i++)
            {
                int x = 20 + i * 30;
                int y = random.Next(10, 20);
                float angle = random.Next(-30, 30);
                graphics.TranslateTransform(x, y);
                graphics.RotateTransform(angle);
                graphics.DrawString(captchaCode[i].ToString(), font, brush, 0, 0);
                graphics.RotateTransform(-angle);
                graphics.TranslateTransform(-x, -y);
            }

            // Add random lines
            for (int i = 0; i < 5; i++)
            {
                int x1 = random.Next(0, width);
                int y1 = random.Next(0, height);
                int x2 = random.Next(0, width);
                int y2 = random.Next(0, height);
                graphics.DrawLine(new Pen(System.Drawing.Color.DarkBlue), x1, y1, x2, y2);
            }
            return bitmap;
        }
        #region Added By Harshida To generate Image
        public static Bitmap GenerateProfileImage(string username, int width, int height)
        {

            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Random background colors
                Color[] colors = { Color.LightGray };
                Random random = new Random();
                Color backgroundColor = colors[random.Next(colors.Length)];
                // Fill background
                graphics.Clear(backgroundColor);
                // Draw the first letter
                var font = new Font("Arial", 48, FontStyle.Bold);
                var brush = new SolidBrush(Color.White);
                string firstLetter = username.Substring(0, 1).ToUpper();
                SizeF textSize = graphics.MeasureString(firstLetter, font);
                float x = (width - textSize.Width) / 2;
                float y = (height - textSize.Height) / 2;
                graphics.DrawString(firstLetter, font, brush, x, y);
            }
            return bitmap;
        }
        #endregion

    }

}
