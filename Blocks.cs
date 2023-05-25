#pragma warning disable
using System.Numerics;
using Raylib_cs;
struct vector{
    public Vector3 export(){
        return new Vector3(X,Y,Z);
    }
    public float X, Y, Z;
}
struct vectori{
    public int X, Y, Z;
}
enum BlockType{
    dirt,
    stone,
    water
}
class Block{
    public static List<Block> blocks = new();
    public BlockType type;
    public Color blockColor;
    public vector position;

}
