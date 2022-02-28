using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.IO;
using System.Text;

namespace ConwaysGameOfLife2
{
    class Shader
    {
        public readonly int handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            int vertexShader;
            int fragmentShader;

            string vertexShaderSource;

            using(StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                vertexShaderSource = reader.ReadToEnd();
            }

            string fragmentShaderSource;

            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                fragmentShaderSource = reader.ReadToEnd();
            }

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.CompileShader(vertexShader);
            GL.CompileShader(fragmentShader);

            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            GL.LinkProgram(handle);

            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(handle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }

        public void SetBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(handle, name), value ? 1: 0);
        }

        public void SetInt(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(handle, name), value);
        }

        public void SetFloat(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(handle, name), value);
        }

        public void SetVec2(string name, Vector2 value)
        {
            GL.Uniform2(GL.GetUniformLocation(handle, name), value);
        }

        public void SetVec2(string name, float x, float y)
        {
            GL.Uniform2(GL.GetUniformLocation(handle, name), x, y);
        }

        public void SetVec3(string name, Vector3 value)
        {
            GL.Uniform3(GL.GetUniformLocation(handle, name), value);
        }

        public void SetVec3(string name, float x, float y, float z)
        {
            GL.Uniform3(GL.GetUniformLocation(handle, name), x, y, z);
        }

        public void SetVec4(string name, Vector4 value) 
        {
            GL.Uniform4(GL.GetUniformLocation(handle, name), value);
        }

        public void SetVec4(string name, float x, float y, float z, float w)
        {
            GL.Uniform4(GL.GetUniformLocation(handle, name), x, y, z, w);
        }

        public void SetMat4(string name, Matrix4 mat) 
        {
            GL.UniformMatrix4(GL.GetUniformLocation(handle, name), false, ref mat);
        }
    }
}
