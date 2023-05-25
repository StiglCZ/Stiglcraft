using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;
class Game{
    public float speed = 16f, sensitivity = 4f;
    public void Start(){
        SetTraceLogLevel(TraceLogLevel.LOG_NONE);
        SetTargetFPS(120);
        SetExitKey(KeyboardKey.KEY_ESCAPE);
        InitWindow(1280,720,"Game");
    }
    public void UpdateC(Camera3D camera){
        unsafe{
            UpdateCamera(&camera,CameraMode.CAMERA_FIRST_PERSON);
        }
    }
    public Vector3 Movement(){
        Vector3 movement = Vector3.Zero;
        if(IsKeyDown(KeyboardKey.KEY_W))
            movement += Vector3.UnitX;
        else if(IsKeyDown(KeyboardKey.KEY_S))
            movement -= Vector3.UnitX;
        if(IsKeyDown(KeyboardKey.KEY_A))
            movement -= Vector3.UnitY;
        else if(IsKeyDown(KeyboardKey.KEY_D))
            movement += Vector3.UnitY;
        if(IsKeyDown(KeyboardKey.KEY_SPACE))
            movement += Vector3.UnitZ;
        else if(IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
            movement -= Vector3.UnitZ;
        return movement * GetFrameTime() * speed;
    }
    public Camera3D GetCamera(){
        Camera3D camera3D = new();
        camera3D.fovy = 60f;
        camera3D.position = Vector3.Zero;
        camera3D.target = Vector3.UnitX * 2;
        camera3D.up = Vector3.UnitY * 2;
        camera3D.projection = CameraProjection.CAMERA_PERSPECTIVE;
        return camera3D;
    }
    public int seed;
    public void GenerateTerain(int size, int height){
        //int size = 128;
        //int height = 50;
        Block.blocks.Clear();
        Random r = new();
        seed = r.Next();
        FastNoiseLite fnl = new(seed);
        fnl.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        float[] noiseData = new float[size * size];
        int index = 0;
        for (int y = 0; y < size; y++){
            for (int x = 0; x < size; x++){
                noiseData[index++] = fnl.GetNoise(x, y);
            }
        }
        int[] noise = new int[size * size];
        for(int i =0; i < size* size;i++){
            noise[i] = DecimalToRange(noiseData[i],height);
        }
        for(int i =0; i < noise.Length;i++){
            int x = (i- i%size)/size;
            int y = noise[i];
            int z = i % size;
            BlockType bt = BlockType.stone;
            if(y > 6){
                bt = BlockType.stone;
            }else{
                bt = BlockType.dirt;
            }
            Block.blocks.Add(new Block(){type = bt, position = new vector(){X = x, Y = y, Z = z}});
        }
    }
    public void BrakBlock(Camera3D camera){
            unsafe{
                Vector3 cameraPosition = camera.position;
                Vector3 cameraDirection = GetCameraForward(&camera);
                Ray ray;
                ray.position = cameraPosition;
                ray.direction = cameraDirection;
                float distance = 2;
                int index = -1;
                for (int i =0; i < Block.blocks.Count;i++){
                    Block block = Block.blocks[i];
                    //Vector3 boxMin = Vector3.Divide(new Vector3(block.position.X,block.position.Y,block.position.Z), 2);
                    //Vector3 boxMax = Vector3.Add(boxMin, Vector3.One/2); 
                    //RayCollision rc = GetRayCollisionBox(ray,new BoundingBox(boxMin,boxMax));
                    RayCollision rc = GetRayCollisionSphere(ray,new Vector3(block.position.X /2,block.position.Y /2,block.position.Z /2),0.5f);
                    if(rc.hit && rc.distance < distance){
                        //Program.DisplayVal = rc.distance;
                        index = i;
                        distance = rc.distance;
                    }
                }
                if(index > -1)
                    Block.blocks.RemoveAt(index);
            }
    }
    public static int DecimalToRange(float decimalValue,int height){
        decimalValue = Math.Max(Math.Min(decimalValue, 1), 0);
        float decimalRange = 1 - 0;
        return (int)(((decimalValue - 0) / decimalRange) * height);
    }
}