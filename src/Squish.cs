// Copyright 2009, 2022 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace fshwrite
{
    internal static class Squish
    {
        internal static byte[] CompressImage(Bitmap color, Bitmap alpha, int flags)
        {
            byte[] pixelData = CreatePixelDataBuffer(color, alpha);

            int width = color.Width;
            int height = color.Height;

            // Compute size of compressed block area, and allocate
            int blockCount = ((width + 3) / 4) * ((height + 3) / 4);
            int blockSize = ((flags & (int)SquishCompFlags.kDxt1) != 0) ? 8 : 16;

            // Allocate room for compressed blocks
            byte[] blockData = new byte[blockCount * blockSize];

            // Invoke squish::CompressImage() with the required parameters
            CompressImageWrapper(pixelData, width, height, blockData, flags);

            // Return our block data to caller..
            return blockData;
        }

        private static unsafe void CompressImageWrapper(byte[] rgba, int width, int height, byte[] blocks, int flags)
        {
            fixed (byte* RGBA = rgba)
            {
                fixed (byte* Blocks = blocks)
                {
                    if (Is64bit())
                    {
                        Squish_64.SquishCompressImage(RGBA, width, height, Blocks, flags);
                    }
                    else
                    {
                        Squish_32.SquishCompressImage(RGBA, width, height, Blocks, flags);
                    }
                }
            }
        }

        private static unsafe byte[] CreatePixelDataBuffer(Bitmap color, Bitmap alpha)
        {
            int width = color.Width;
            int height = color.Height;

            byte[] pixelData = new byte[width * height * 4];

            BitmapData colorData = color.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData alphaData = null;

            try
            {
                byte* colorScan0 = (byte*)colorData.Scan0;
                int colorStride = colorData.Stride;
                int destStride = width * 4;

                fixed (byte* destScan0 = pixelData)
                {
                    if (alpha != null)
                    {
                        alphaData = alpha.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                        byte* alphaScan0 = (byte*)alphaData.Scan0;
                        int alphaStride = alphaData.Stride;

                        for (int y = 0; y < height; y++)
                        {
                            byte* colorPtr = colorScan0 + (y * colorStride);
                            byte* alphaPtr = alphaScan0 + (y * alphaStride);
                            byte* dest = destScan0 + (y * destStride);

                            for (int x = 0; x < width; x++)
                            {
                                // The color bitmap is BGR and the destination is RGB.
                                dest[0] = colorPtr[2];
                                dest[1] = colorPtr[1];
                                dest[2] = colorPtr[0];
                                dest[3] = alphaPtr[0];

                                colorPtr += 4;
                                alphaPtr += 4;
                                dest += 4;
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < height; y++)
                        {
                            byte* colorPtr = colorScan0 + (y * colorStride);
                            byte* dest = destScan0 + (y * destStride);

                            for (int x = 0; x < width; x++)
                            {
                                // The color bitmap is BGR and the destination is RGB.
                                dest[0] = colorPtr[2];
                                dest[1] = colorPtr[1];
                                dest[2] = colorPtr[0];
                                dest[3] = 255;

                                colorPtr += 4;
                                dest += 4;
                            }
                        }
                    }
                }
            }
            finally
            {
                color.UnlockBits(colorData);
                if (alpha != null)
                {
                    alpha.UnlockBits(alphaData);
                }
            }

            return pixelData;
        }

        private static bool Is64bit()
        {
            return IntPtr.Size == 8 ? true : false;
        }

        private sealed class Squish_32
        {
            [DllImport("Squish_Win32.dll")]
            internal static extern unsafe void SquishCompressImage(byte* rgba, int width, int height, byte* blocks, int flags);
        }

        private sealed class Squish_64
        {
            [DllImport("squish_x64.dll")]
            internal static extern unsafe void SquishCompressImage(byte* rgba, int width, int height, byte* blocks, int flags);
        }
    }
}
