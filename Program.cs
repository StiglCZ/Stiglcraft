
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;


class Program{
    public static float DisplayVal = 0;
    Game game = new();
    Camera3D camera;
    float gravity = 0.1f, renderDistance = 1000;
    public void Init(string[] args){
        game.Start();
        DisableCursor();
        camera = game.GetCamera();
        game.GenerateTerain(128,50);
        for(int i =0; i < Block.blocks.Count;i++){
            if(Block.blocks[i].type == BlockType.dirt)
                if(Block.blocks[i].position.Y <= 1){
                    Block.blocks[i].blockColor = Color.BROWN;
                }else Block.blocks[i].blockColor = Color.DARKGREEN;
            else if(Block.blocks[i].type == BlockType.water)
                Block.blocks[i].blockColor = Color.DARKBLUE;
            else if(Block.blocks[i].type == BlockType.stone){
                Block.blocks[i].blockColor = Color.GRAY;
                if(Block.blocks[i].position.Y >= 20){
                    Block.blocks[i].blockColor = Color.RAYWHITE;
                }
            }
        }
        while(!WindowShouldClose())
            Update();
        EnableCursor();
    }
    vector pos = new(){X = 0, Y = 0, Z = 0};
    vectori chunkCoordinates = new(){X = 0, Y =0};
    bool grounded = false;
    public void Update(){
        if(IsMouseButtonReleased(0)){
            game.BrakBlock(camera);
        }
        //Calculating gravity
        Vector3 grav = new();
        if(!grounded)
            grav.Z -= gravity;
        //Updates camera
        game.UpdateC(camera);
        //Movemnt and rotation
        unsafe{
            fixed(Camera3D* cam = &camera){
                UpdateCameraPro(cam,game.Movement() + grav,game.sensitivity * new Vector3(GetMouseDelta().X*0.05f,GetMouseDelta().Y*0.05f,0),0);
            }
        }
        //Begin rendering process
        BeginDrawing();
        ClearBackground(Color.BLUE);
        BeginMode3D(camera);
        //Draws water!
        DrawPlane(new Vector3(32,1f,32),new Vector2(64,64),Color.DARKBLUE);
        grounded = false;
        
        foreach(Block block in Block.blocks){
            //Vector3 one = block.position.export();
            //float distance =Math.Abs(Vector2.Subtract(new Vector2(one.X,one.Z)/2,new Vector2(camera.position.X,camera.position.Z)).Length());
            //if(distance < renderDistance /*&& Opts.IsBlockVisible(block.position.export(),Vector3.One/2,camera)*/)
                DrawCube(new Vector3(block.position.X /2,block.position.Y /2,block.position.Z /2),0.5f,0.5f,0.5f,block.blockColor);   
            //DrawCubeWires(new Vector3(block.position.X /2,block.position.Y /2,block.position.Z /2),0.5f,0.5f,0.5f,block.blockColor);   
            if((int)(block.position.X /2 - block.position.X %2) == (int)camera.position.X && (int)(block.position.Z /2 -0.5f) == (int)camera.position.Z &&
            (int)(block.position.Y /2 - block.position.X %2) == (int)camera.position.Y){
                grounded = true;
            }
        }
        EndMode3D();
        //Draws interesting thingies
        DrawText($"X: {(int)(camera.position.X*2)}\nY: {(int)(camera.position.Y*2)}\nZ: {(int)(camera.position.Z*2)}",5,1,15,Color.YELLOW); 
        DrawText(GetFPS() + "FPS",1200,0,15,Color.YELLOW);
        DrawCircle(640,360,5,Color.BLACK);
        DrawText($"Seed: {game.seed}",1150,700,15,Color.YELLOW);DrawText($"Dev Preview! Build v0.1",0,700,15,Color.YELLOW);
        EndDrawing();
    }
    public static void Main(string[] args)=> new Program().Init(args);
    /*public void CreateMap(){
        vectori chunkCoordinates2 = new(){
            X = (int)((camera.position.X - camera.position.X % 8) / 8),
            Z = (int)((camera.position.Z - camera.position.Z % 8) / 8),
            Y = 0};
        if(chunkCoordinates2.Z == chunkCoordinates.Z && chunkCoordinates2.X == chunkCoordinates.X)return;
        chunkCoordinates.X = chunkCoordinates2.X;
        chunkCoordinates.Z = chunkCoordinates2.Z;
        pos.X = chunkCoordinates.X * 16;
        pos.Z = chunkCoordinates.Z * 16;
        Block.blocks.Clear();
        Random r = new();
        vector bpos = new(){X = 0, Y = 0, Z = 0};
        BlockType bt = BlockType.stone;
        bpos = pos;
        for(int i =0; i < 16;i++){
            pos.X++;
            for(int y =0; y < 16;y++){
                pos.Z ++;
                for(int z =0; z < 30;z++){
                    pos.Y++;  
                    //bt = (BlockType)r.Next(0,3);
                    List<int> options = new();
                    if(pos.Y > 27){
                        options.Add(0);options.Add(0);options.Add(0);options.Add(0);
                    }if(pos.Y > 24){
                        options.Add(2);
                    }if(pos.Y < 28){
                        options.Add(1);options.Add(1);
                    }bt = (BlockType)options[r.Next(0,options.Count)];
                    Block.blocks.Add(new Block(){type = bt, position = pos});
                }
                pos.Y = 0;
            }
            pos.Z = bpos.Z;
        }
        pos.X = bpos.X;
    }*/
}
