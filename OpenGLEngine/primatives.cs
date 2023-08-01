
using OpenTK.Mathematics;

namespace OpenGLEngine
{
    public static class Shapes
    {
        public static Cube AddCubeToGame(Game game, Vector3 dimensions, Vector3 position, Vector3 rotation)
        {
            float[] vertices =
            {
                -1f*(dimensions.X/2f),  1f*(dimensions.Y/2f),   -1f*(dimensions.Z/2f),      0f, 1f,
                1f*(dimensions.X/2f),   1f*(dimensions.Y/2f),   -1f*(dimensions.Z/2f),      1f, 1f,
                1f*(dimensions.X/2f),   1f*(dimensions.Y/2f),   1f*(dimensions.Z/2f),       1f, 0f,
                -1f*(dimensions.X/2f),  1f*(dimensions.Y/2f),   1f*(dimensions.Z/2f),       0f, 0f,

                -1f*(dimensions.X/2f),  -1f*(dimensions.Y/2f),  -1f*(dimensions.Z/2f),      1f, 1f,
                1f*(dimensions.X/2f),   -1f*(dimensions.Y/2f),  -1f*(dimensions.Z/2f),      1f, 0f,
                1f*(dimensions.X/2f),   -1f*(dimensions.Y/2f),  1f*(dimensions.Z/2f),       0f, 0f,
                -1f*(dimensions.X/2f),  -1f*(dimensions.Y/2f),  1f*(dimensions.Z/2f),       0f, 1f
            };
            uint[] indices =
            {
                3, 0, 1,
                3, 2, 1,

                6, 5, 4,
                6, 7, 4,

                1, 2, 6,
                1, 5, 6,

                3, 0, 4,
                3, 7, 4,

                7, 3, 2,
                7, 6, 2,

                0, 1, 5,
                0, 4, 5
            };
            Cube cube = new Cube(game.getDefaultShader(), vertices, indices);
            cube.setLocation(position.X, position.Y, position.Z);
            cube.setRotation(rotation.X, rotation.Y, rotation.Z);
            game.addDrawableObject(cube); ;
            return cube;
        }
    }

    public class Cube : Entity
    {
        internal Cube(Shader shader, float[] vertices, uint[] indices) : base(shader, vertices, indices, new (string, int)[] { ("aPosition", 3), ("aTexPos", 2) }, "Cube") 
        {
            this.loadTextures("C:\\Users\\brian\\source\\repos\\OpenGLEngine\\OpenGLEngine\\Textures\\", new string[] { "container.jpg"});
        }
    }
}
