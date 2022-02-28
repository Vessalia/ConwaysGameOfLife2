using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace ConwaysGameOfLife2
{
    public class Game : GameWindow
    {
        Shader shader;

        int VAO;

        int positionVBO;
        int texCoordVBO;
        int EBO;

        Texture readTexture;
        Texture writeTexture;

        int readFrameBuffer;
        int writeFrameBuffer;

        float[] verticies = 
        {
            -1.0f, -1.0f, 0.0f, //Bottom-left vertex
             1.0f, -1.0f, 0.0f, //Bottom-right vertex
            -1.0f,  1.0f, 0.0f, //Top-left vertex
             1.0f,  1.0f, 0.0f  //Top-right vertex
        };

        float[] UV =
        {
            0.0f, 0.0f,
            1.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f
        };

        uint[] indices =
        {
            0, 1, 2,
            2, 1, 3
        };

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        protected override void OnLoad()
        {
            shader = new Shader("Shaders/shader.vert.glsl", "Shaders/shader.frag.glsl");

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);




            positionVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verticies.Length * sizeof(float), verticies, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            texCoordVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, UV.Length * sizeof(float), UV, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(shader.GetAttribLocation("aTexCoord"), 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);




            readTexture = new Texture(ClientRectangle.Size.X, ClientRectangle.Size.Y);
            readFrameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, readFrameBuffer);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, readTexture.handle, 0);

            writeTexture = new Texture(ClientRectangle.Size.X, ClientRectangle.Size.Y);
            writeFrameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, writeFrameBuffer);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, writeTexture.handle, 0);




            Title = "Conway's Game of Life 2.0";

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            shader.Dispose();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteBuffer(positionVBO);
            GL.DeleteBuffer(texCoordVBO);
            GL.DeleteVertexArray(VAO);

            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();
            shader.SetInt("state", 0);
            shader.SetVec2("size", new Vector2(ClientRectangle.Size.X, ClientRectangle.Size.Y));
            readTexture.Use(TextureUnit.Texture0);

            GL.BindVertexArray(VAO);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, writeFrameBuffer);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            
            writeTexture.Use(TextureUnit.Texture0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapTextureFaces();

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            
            base.OnResize(e);
        }

        private void SwapTextureFaces()
        {
            Texture temp = writeTexture;
            writeTexture = readTexture;
            readTexture = temp;

            int tempBuffer = writeFrameBuffer;
            writeFrameBuffer = readFrameBuffer;
            readFrameBuffer = tempBuffer;
        }
    }
}
