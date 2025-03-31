using OpenTK.Graphics.OpenGL4;

namespace EasyImGui.NET.Windowing.OpenTK;
public static unsafe class EasyTextureOpenTK {

    public static unsafe uint CreateTexture(byte* ptr, int width, int height,
        PixelInternalFormat iFormat = PixelInternalFormat.Rgba,
        PixelFormat format = PixelFormat.Rgba,
        PixelType pixelType = PixelType.UnsignedByte) {
        GL.GenTextures(1, out uint texture);
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, iFormat, width, height, 0, format, pixelType, (IntPtr)ptr);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        return texture;
    }

    public static uint CreateTexture(ReadOnlySpan<byte> bytes, int width, int height, string name, int channels = 4) {
        uint tex = 0;
        fixed (byte* ptr = bytes) {
            tex = CreateTexture(ptr, width, height);
        }
        return tex;
    }

}
