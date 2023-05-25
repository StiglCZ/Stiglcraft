using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
class Opts{
     public static bool IsBlockVisible(Vector3 blockPosition, Vector3 blockSize,Camera3D camera){
        // Calculate the block's bounding box corners
        Vector3 blockMin = blockPosition - blockSize / 2f;
        Vector3 blockMax = blockPosition + blockSize / 2f;

        // Calculate the frustum planes
        Plane[] frustumPlanes = new Plane[6];
        Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(camera.position, camera.target, Vector3.UnitY);
        //Matrix4x4 projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 3f, (float)Raylib.GetScreenWidth() / Raylib.GetScreenHeight(), 0.1f, 1000f);
        Matrix4x4 projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 2f, (float)GetScreenWidth() / GetScreenHeight(), 0.1f, 1000f);
        Matrix4x4 viewProjectionMatrix = Matrix4x4.Multiply(viewMatrix, projectionMatrix);
        
        // Left plane
        frustumPlanes[0] = new Plane(
            viewProjectionMatrix.M14 + viewProjectionMatrix.M11,    
            viewProjectionMatrix.M24 + viewProjectionMatrix.M21,
            viewProjectionMatrix.M34 + viewProjectionMatrix.M31,
            viewProjectionMatrix.M44 + viewProjectionMatrix.M41
        );

        // Right plane
        frustumPlanes[1] = new Plane(
            viewProjectionMatrix.M14 - viewProjectionMatrix.M11,
            viewProjectionMatrix.M24 - viewProjectionMatrix.M21,
            viewProjectionMatrix.M34 - viewProjectionMatrix.M31,
            viewProjectionMatrix.M44 - viewProjectionMatrix.M41
        );

        // Bottom plane
        frustumPlanes[2] = new Plane(
            viewProjectionMatrix.M14 + viewProjectionMatrix.M12,
            viewProjectionMatrix.M24 + viewProjectionMatrix.M22,
            viewProjectionMatrix.M34 + viewProjectionMatrix.M32,
            viewProjectionMatrix.M44 + viewProjectionMatrix.M42
        );

        // Top plane
        frustumPlanes[3] = new Plane(
            viewProjectionMatrix.M14 - viewProjectionMatrix.M12,
            viewProjectionMatrix.M24 - viewProjectionMatrix.M22,
            viewProjectionMatrix.M34 - viewProjectionMatrix.M32,
            viewProjectionMatrix.M44 - viewProjectionMatrix.M42
        );

        // Near plane
        frustumPlanes[4] = new Plane(
            viewProjectionMatrix.M13,
            viewProjectionMatrix.M23,
            viewProjectionMatrix.M33,
            viewProjectionMatrix.M43
        );

        // Far plane
        frustumPlanes[5] = new Plane(
            viewProjectionMatrix.M14 - viewProjectionMatrix.M13,
            viewProjectionMatrix.M24 - viewProjectionMatrix.M23,
            viewProjectionMatrix.M34 - viewProjectionMatrix.M33,
            viewProjectionMatrix.M44 - viewProjectionMatrix.M43
        );
        for (int i = 0; i < 6; i++)
        {
            if (GetDistanceToPoint(frustumPlanes[i], blockMin) > 0f)
                continue;

            if (GetDistanceToPoint(frustumPlanes[i], new Vector3(blockMin.X, blockMin.Y, blockMax.Z)) > 0f)
                continue;

            if (GetDistanceToPoint(frustumPlanes[i], new Vector3(blockMin.X, blockMax.Y, blockMin.Z)) > 0f)
                continue;

            if (GetDistanceToPoint(frustumPlanes[i], new Vector3(blockMin.X, blockMax.Y, blockMax.Z)) > 0f)
                continue;

            if (GetDistanceToPoint(frustumPlanes[i], new Vector3(blockMax.X, blockMin.Y, blockMin.Z)) > 0f)
                continue;

            if (GetDistanceToPoint(frustumPlanes[i], new Vector3(blockMax.X, blockMin.Y, blockMax.Z)) > 0f)
                continue;

            if (GetDistanceToPoint(frustumPlanes[i], new Vector3(blockMax.X, blockMax.Y, blockMin.Z)) > 0f)
                continue;

            if (GetDistanceToPoint(frustumPlanes[i], blockMax) > 0f)
                continue;

            // If none of the block's corners are behind the current plane, it's not visible
            return false;
        }

        // If all corner tests passed, the block is visible
        return true;
        
    }
    

     static float GetDistanceToPoint(Plane plane, Vector3 point){
        return Vector3.Dot(plane.Normal, point) + plane.D;
    }

}