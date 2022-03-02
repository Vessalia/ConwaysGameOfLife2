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

        int screenWidth;
        int screenHeight;

        int VAO;

        float rot = 0;

        int positionVBO;
        int texCoordVBO;
        int EBO;

        Texture readTexture;
        Texture writeTexture;

        int readFrameBuffer;
        int writeFrameBuffer;

        Vector3 position = new Vector3(0.0f, 0.0f, 3.0f);
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        float yaw = -MathF.PI / 2;
        float pitch = 0;

        float speed = 4f;

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




            readTexture = new Texture(3840, 2160);
            readFrameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, readFrameBuffer);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, readTexture.handle, 0);

            writeTexture = new Texture(3840, 2160);
            writeFrameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, writeFrameBuffer);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, writeTexture.handle, 0);




            Title = "Conway's Game of Life 2.0";

            screenWidth = ClientRectangle.Size.X;
            screenHeight = ClientRectangle.Size.Y;

            GL.Enable(EnableCap.DepthTest);

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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();
            shader.SetInt("state", 0);
            shader.SetVec2("size", new Vector2(readTexture.width, readTexture.height));

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = Matrix4.Identity;
            Matrix4 projection = Matrix4.Identity;

            shader.SetMat4("model", model);
            shader.SetMat4("view", view);
            shader.SetMat4("projection", projection);

            GL.BindVertexArray(VAO);
            GL.Viewport(0, 0, readTexture.width, readTexture.height);

            readTexture.Use(TextureUnit.Texture0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, writeFrameBuffer);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.Viewport(0, 0, screenWidth, screenHeight);

            model = Matrix4.CreateRotationY(rot);
            view = Matrix4.LookAt(position, position + front, up);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4, screenWidth / screenHeight, 0.1f, 100f);

            rot += MathF.PI / 180;

            shader.SetMat4("model", model);
            shader.SetMat4("view", view);
            shader.SetMat4("projection", projection);

            writeTexture.Use(TextureUnit.Texture0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapTextureFaces();

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                position += front * speed * (float)e.Time; //Forward 
            }

            if (KeyboardState.IsKeyDown(Keys.S))
            {
                position -= front * speed * (float)e.Time; //Backwards
            }

            if (KeyboardState.IsKeyDown(Keys.A))
            {
                position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time; //Left
            }

            if (KeyboardState.IsKeyDown(Keys.D))
            {
                position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time; //Right
            }

            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                position += up * speed * (float)e.Time; //Up 
            }

            if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                position -= up * speed * (float)e.Time; //Down
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            CursorGrabbed = true;
            CursorVisible = false;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            CursorGrabbed = false;
            CursorVisible = true;

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            float sensitivity = 0.002f;

            if(MouseState.IsButtonDown(MouseButton.Button1))
            {
                yaw += e.DeltaX * sensitivity;
                pitch -= e.DeltaY * sensitivity;

                if (pitch > MathF.PI / 2)
                {
                    pitch = MathF.PI / 2;
                }
                else if (pitch < -MathF.PI / 2)
                {
                    pitch = -MathF.PI / 2;
                }

                front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
                front.Y = MathF.Sin(pitch);
                front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);
                front = Vector3.Normalize(front);
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            screenWidth = e.Width;
            screenHeight = e.Height;
            
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
