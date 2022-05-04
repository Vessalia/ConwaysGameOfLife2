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
        Shader conwayShader;
        Shader defaultShader;
        Shader lightlampShader;

        int screenWidth;
        int screenHeight;

        int cubeVAO;
        int quadVAO;
        int lampVAO;

        float rot = 0;

        int positionVBO;
        int quadVBO;
        int lampVBO;

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

        Vector3 lightPos = new Vector3(1.2f, 2.0f, 1.0f);
        Vector3 lightColour = new Vector3(1.0f, 1.0f, 1.0f);

        float[] vertices = { // xCoord, yCoord, zCoord, texXCoord, texYCoord, normX, normY, normZ
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  0.0f,  0.0f, -1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  0.0f,  0.0f,  1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f, -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  0.0f, -1.0f,  0.0f,
                                                  
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,  0.0f,  1.0f,  0.0f
        };

        float[] quad =
        {
             -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
              1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
             -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,

             -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
              1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
              1.0f,  1.0f, 0.0f, 1.0f, 1.0f
        };

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        protected override void OnLoad()
        {
            conwayShader = new Shader("Shaders/shader.vert.glsl", "Shaders/conway.frag.glsl");
            defaultShader = new Shader("Shaders/shader.vert.glsl", "Shaders/shader.frag.glsl");
            lightlampShader = new Shader("Shaders/lightlamp.vert.glsl", "Shaders/lightlamp.frag.glsl");




            cubeVAO = GL.GenVertexArray();
            GL.BindVertexArray(cubeVAO);

            positionVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(defaultShader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(defaultShader.GetAttribLocation("aPosition"));

            GL.VertexAttribPointer(defaultShader.GetAttribLocation("aTexCoord"), 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(defaultShader.GetAttribLocation("aTexCoord"));

            GL.VertexAttribPointer(defaultShader.GetAttribLocation("aNormal"), 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(defaultShader.GetAttribLocation("aNormal"));




            quadVAO = GL.GenVertexArray();
            GL.BindVertexArray(quadVAO);

            quadVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, quad.Length * sizeof(float), quad, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(conwayShader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(conwayShader.GetAttribLocation("aPosition"));

            GL.VertexAttribPointer(conwayShader.GetAttribLocation("aTexCoord"), 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(conwayShader.GetAttribLocation("aTexCoord"));




            lampVAO = GL.GenVertexArray();
            GL.BindVertexArray(lampVAO);

            lampVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, lampVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(conwayShader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(conwayShader.GetAttribLocation("aPosition"));




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
            conwayShader.Dispose();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteBuffer(positionVBO);
            GL.DeleteBuffer(quadVBO);
            GL.DeleteVertexArray(cubeVAO);
            GL.DeleteVertexArray(quadVAO);

            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            lightlampShader.Use();

            lightlampShader.SetVec3("lightColour", lightColour);

            Matrix4 lampMatrix = Matrix4.Identity;
            lampMatrix *= Matrix4.CreateScale(0.2f);
            lampMatrix *= Matrix4.CreateTranslation(lightPos);

            lightlampShader.SetMat4("model", lampMatrix);
            lightlampShader.SetMat4("view", Matrix4.LookAt(position, position + front, up));
            lightlampShader.SetMat4("projection", Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4, screenWidth / screenHeight, 0.1f, 100f));

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 5);




            conwayShader.Use();
            conwayShader.SetInt("state", 0);
            conwayShader.SetVec2("size", new Vector2(readTexture.width, readTexture.height));

            GL.BindVertexArray(lampVAO);
            GL.Viewport(0, 0, screenWidth, screenHeight);

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = Matrix4.Identity;
            Matrix4 projection = Matrix4.Identity;

            conwayShader.SetMat4("model", model);
            conwayShader.SetMat4("view", view);
            conwayShader.SetMat4("projection", projection);

            GL.BindVertexArray(quadVAO);
            GL.Viewport(0, 0, readTexture.width, readTexture.height);

            readTexture.Use(TextureUnit.Texture0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, writeFrameBuffer);
            GL.DrawArrays(PrimitiveType.Triangles, 0, quad.Length / 5);




            defaultShader.Use();
            defaultShader.SetInt("tex", 0);

            defaultShader.SetVec3("lightColour", lightColour);
            defaultShader.SetVec3("lightPos", lightPos);
            defaultShader.SetVec3("viewPos", position);

            GL.BindVertexArray(cubeVAO);
            GL.Viewport(0, 0, screenWidth, screenHeight);

            model = Matrix4.CreateRotationY(rot);
            view = Matrix4.LookAt(position, position + front, up);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4, screenWidth / screenHeight, 0.1f, 100f);

            rot += MathF.PI / 1800;

            defaultShader.SetMat4("model", model);
            defaultShader.SetMat4("view", view);
            defaultShader.SetMat4("projection", projection);

            writeTexture.Use(TextureUnit.Texture0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 5);




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
