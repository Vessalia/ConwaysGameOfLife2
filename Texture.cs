using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace ConwaysGameOfLife2
{
    class Texture
    {
        public readonly int handle;

        public readonly int width;
        public readonly int height;

        public Texture(int width, int height)
        {
            handle = GL.GenTexture();

            Use(TextureUnit.Texture0);

            this.width = width;
            this.height = height;

            byte[] arr = new byte[4 * width * height];
            Random r = new Random();
            for(int i = 0; i < arr.Length; i += 4)
            {
                arr[i    ] = (byte)r.Next(0, 255);
                arr[i + 1] = (byte)r.Next(0, 255);
                arr[i + 2] = (byte)r.Next(0, 255);
                arr[i + 3] = 255;
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, arr);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
    }
}
