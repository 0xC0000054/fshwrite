// Copyright 2009, 2022 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace fshwrite
{
    internal static class Squish
    {
        internal static byte[] CompressImage(Bitmap image, int flags)
        {
            byte[] pixelData = new byte[image.Width * image.Height * 4];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int Offset = (y * image.Width * 4) + (x * 4);

                    pixelData[Offset + 0] = pixel.R;
                    pixelData[Offset + 1] = pixel.G;
                    pixelData[Offset + 2] = pixel.B;
                    pixelData[Offset + 3] = pixel.A;
                }
            }

            // Compute size of compressed block area, and allocate
            int blockCount = ((image.Width + 3) / 4) * ((image.Height + 3) / 4);
            int blockSize = ((flags & (int)SquishCompFlags.kDxt1) != 0) ? 8 : 16;

            // Allocate room for compressed blocks
            byte[] blockData = new byte[blockCount * blockSize];

            // Invoke squish::CompressImage() with the required parameters
            CompressImageWrapper(pixelData, image.Width, image.Height, blockData, flags);

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
