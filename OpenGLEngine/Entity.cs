using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace OpenGLEngine
{
    public class Entity
    {
        List<Texture> textures;
        Shader shader;
        int vertexBufferObject;
        int vertexArrayObject;
        int ElementBufferObject;
        int numIndices;
        Vector3 position;
        Vector3 rotation;
        Matrix4 worldViewMat;
        bool moved;
        string name;
        public static Entity createEntity(Shader shader, float[] data, uint[] indices, (string, int)[] vertAttr, string name)
        {
            int vbo = GL.GenBuffer();
            /*
             StaticDraw: the data will most likely not change at all or very rarely.
            DynamicDraw: the data is likely to change a lot.
            StreamDraw: the data will change every time it is drawn.
             */
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int sum = 0;
            for (int i = 0; i < vertAttr.Length; i++)
            {
                sum += vertAttr[i].Item2;
            }
            Console.WriteLine($"Sum: {sum}");
            int offset = 0;
            for (int i = 0; i < vertAttr.Length; i++)
            {
                int pos = shader.GetAttribLocation(vertAttr[i].Item1);
                Console.WriteLine($"aPosition Location: {pos}");
                GL.VertexAttribPointer(pos, vertAttr[i].Item2, VertexAttribPointerType.Float, false, sum * sizeof(float), offset);
                GL.EnableVertexAttribArray(pos);
                offset += vertAttr[i].Item2 * sizeof(float);
            }

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            
            shader.Use();
            
            return new Entity(name, vao, vbo, ebo, indices.Length, shader);
        }
        public void loadTextures(string textureFolderPath, string[] textureNames)
        {
            if (textures == null)
                textures = new List<Texture>();
            for( int i = 0; i < textureNames.Length; i++)
            {
                Texture tex = new Texture($"{textureFolderPath}{textureNames[i]}", TextureUnit.Texture0+i);
                tex.Use(TextureUnit.Texture0 + i);
                textures.Add(tex);
            }
        }
        internal Entity(Shader shader, float[] vertices, uint[] indices, (string, int)[] vertAttr, string name)
        {
            this.shader = shader;
            int vbo = GL.GenBuffer();
            /*
             StaticDraw: the data will most likely not change at all or very rarely.
            DynamicDraw: the data is likely to change a lot.
            StreamDraw: the data will change every time it is drawn.
             */
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int sum = 0;
            for (int i = 0; i < vertAttr.Length; i++)
            {
                sum += vertAttr[i].Item2;
            }
            int offset = 0;
            for (int i = 0; i < vertAttr.Length; i++)
            {
                int pos = shader.GetAttribLocation(vertAttr[i].Item1);
                GL.VertexAttribPointer(pos, vertAttr[i].Item2, VertexAttribPointerType.Float, false, sum * sizeof(float), offset);
                GL.EnableVertexAttribArray(pos);
                offset += vertAttr[i].Item2 * sizeof(float);
            }

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader.Use();
            vertexArrayObject = vao;
            vertexBufferObject = vbo;
            vertexBufferObject = ebo;
            numIndices = indices.Length;
            this.shader = shader;
            this.name = name;
            moved = true;
        }
        private Entity(string name, int vao, int vbo, int ebo, int ni, Shader shader) 
        { 
            vertexArrayObject = vao;
            vertexBufferObject= vbo;
            vertexBufferObject = ebo;
            numIndices = ni;
            this.shader = shader;
            this.name = name;
            moved = true;
        }
        public Matrix4 setModelMat()
        {
            if (moved)
            {
                worldViewMat = Matrix4.Identity * Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.CreateTranslation(position.X, position.Y, position.Z);
                moved = false;
            }
            GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "model"), true, ref worldViewMat);
            return worldViewMat;
        }

        internal void draw(Camera c)
        {
            foreach(Texture texture in textures)
            {
                texture.Use(texture.unit);
            }
            shader.Use();
            GL.BindVertexArray(vertexArrayObject);
            
            setModelMat();
            setViewMat(c.GetViewMatrix());
            setProjectionMat(c.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);
        }
        public void setLocation(float x, float y, float z)
        {
            position = new Vector3(x, y, z);
            moved = true;
        }

        public void setRotation(float xaxis, float yaxis, float zaxis)
        {
            rotation = new Vector3(xaxis, yaxis, zaxis);
            moved = true;
        }
        public void setProjectionMat(Matrix4 projection)
        {
            
            GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "projection"), true, ref projection);
        }
        public void setViewMat(Matrix4 view)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "view"), true, ref view);
        }
        public void Dispose()
        {
            shader.Dispose();
        }
    }
    public class Shader : IDisposable
    {
        public int Handle;
        private bool disposedValue = false;
        public Shader(string vertexPath, string fragmentPath)
        {
            //Compiles the shaders at runtime
            int VertexShader, FragmentShader;
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            //Links the shaders into one program that runs n the gpu
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            //the compiled data is copied to the shader program when you link it. So we are releasing the memory here
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }
        ~Shader()
        {
            if (disposedValue == false)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }
        public void SetInt(string name, int value)
        {

            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, value);
        }
    }

    public class Texture
    {
        public int Handle;
        public TextureUnit unit;
        public Texture(string path, TextureUnit unit)
        {
            Handle = GL.GenTexture();
            this.unit = unit;
            Use(unit);
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Load the image.
            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
