using NetCom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snakes_Client
{
    /// <summary>
    /// Represent a 2D texture.
    /// </summary>
    public class Texture
    {

        /// <summary>
        /// Contains directions and their coresponding rotations.
        /// </summary>
        private static readonly Dictionary<Direction, Int32> rotations = new Dictionary<Direction, Int32>()
        {
            { Direction.Down, 180 },
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Left, 270 },
        };

        /// <summary>
        /// Bitmap source of texture.
        /// </summary>
        public BitmapSource Source { get; private set; }

        /// <summary>
        /// Color that has been applied to texture.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Create new texture.
        /// </summary>
        /// <param name="path">Path to the texture file.</param>
        /// <param name="color">Color that will be aplied to texture.</param>
        public Texture(String path, Color color)
        {
            Source = new BitmapImage(new Uri(Path.GetFullPath(path), UriKind.Absolute));
            Color = color;
            ChangeColor();
        }

        /// <summary>
        /// Get texture rotated by specific angle.
        /// </summary>
        /// <param name="angle">Specific angle.</param>
        /// <returns>Texturte rotated by specific angle.</returns>
        public TransformedBitmap GetRotation(Int32 angle)
        {
            Console.WriteLine(Source);

            return new TransformedBitmap(Source, new System.Windows.Media.RotateTransform(angle));
        }
 //           => new TransformedBitmap(Source, new System.Windows.Media.RotateTransform(angle));

        /// <summary>
        /// Get texture rotated in specific direction.
        /// </summary>
        /// <param name="direction">Specific direction.</param>
        /// <returns>Texture rotat4ed in specifc direction.</returns>
        public TransformedBitmap GetRotation(Direction direction)
            => GetRotation(rotations[direction]);

        /// <summary>
        /// Change color of the texture.
        /// </summary>
        private void ChangeColor()
        {
            if (Color == Colors.White)
                return;

            Double[] hsvTransform = RgbToHsv(Color);
            WriteableBitmap bitmap = new WriteableBitmap(Source);

            // Copy the pixels
            Int32 stride = bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
            Byte[] pixels = new Byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            // Edit the pixels
            for (Int32 i = 0; i < pixels.Length; i += 4)
            {
                // To HSV
                Double[] hsv = RgbToHsv(Color.FromArgb(0, pixels[i], pixels[i + 1], pixels[i + 2]));
                // HSV Transform
                hsv[0] = hsvTransform[0];
                hsv[1] = hsvTransform[1];
                // To RGB
                Color color = HsvToRgb(hsv);
                pixels[i] = color.B;
                pixels[i + 1] = color.G;
                pixels[i + 2] = color.R;
            }

            // Write pixels back
            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixels, stride, 0);
            Source = bitmap;
        }

        /// <summary>
        /// Convert RGB color format to HSV color format.
        /// </summary>
        /// <param name="color">Color in RGV color format.</param>
        /// <returns>Array containg color in HSV color format in this order: { hue, saturation, value }.</returns>
        public static Double[] RgbToHsv(Color color)
        {
            Double[] hsv = new Double[3];
            Double red = color.R / 255d;
            Double green = color.G / 255d;
            Double blue = color.B / 255d;
            Double max = Math.Max(red, Math.Max(green, blue));
            Double min = Math.Min(red, Math.Min(green, blue));
            Double delta = max - min;

            if (delta > 0)
            {
                if (max == red)
                    hsv[0] = (green - blue) / delta;
                else if (max == green)
                    hsv[0] = 2d + (blue - red) / delta;
                else
                    hsv[0] = 4d + (red - green) / delta;

                hsv[0] *= 60d;
                if (hsv[0] < 0d)
                    hsv[0] += 360d;
                else if (hsv[0] >= 360d)
                    hsv[0] -= 360d;
            }
            else
                hsv[0] = 0;
            hsv[1] = max <= 0 ? 0 : delta / max;
            hsv[2] = max;

            return hsv;
        }

        /// <summary>
        /// Converts HSV color format to RGB color format.
        /// </summary>
        /// <param name="hsv">Array containg color in HSV color format in this order: { hue, saturation, value }.</param>
        /// <returns>Color in RGB color format.</returns>
        public static Color HsvToRgb(params Double[] hsv)
        {
            Int32 hi = (Int32)Math.Floor(hsv[0] / 60d);
            hi = (hi < 0) ? (hi % 6 + 6) % 6 : hi % 6;
            Double f = hsv[0] / 60d - Math.Floor(hsv[0] / 60d);

            Byte v = (Byte)((Math.Max(hsv[2], 0d)) * 255);
            Byte p = (Byte)((Math.Max(hsv[2] * (1d - hsv[1]), 0d)) * 255);
            Byte q = (Byte)((Math.Max(hsv[2] * (1d - f * hsv[1]), 0d)) * 255);
            Byte t = (Byte)((Math.Max(hsv[2] * (1d - (1d - f) * hsv[1]), 0d)) * 255);

            return hi switch
            {
                0 => Color.FromArgb(255, v, t, p),
                1 => Color.FromArgb(255, q, v, p),
                2 => Color.FromArgb(255, p, v, t),
                3 => Color.FromArgb(255, p, q, v),
                4 => Color.FromArgb(255, t, p, v),
                _ => Color.FromArgb(255, v, p, q),
            };
        }

        public static Color ConvertColor(System.Drawing.Color color)
            => Color.FromArgb(color.A, color.R, color.G, color.B);

        public static System.Drawing.Color ConvertColor(Color color)
            => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

    }
}
