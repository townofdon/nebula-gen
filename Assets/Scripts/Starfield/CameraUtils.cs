
using UnityEngine;

static class CameraUtils
{

    static Camera cachedCamera;

    public static Camera GetMainCamera()
    {
        if (cachedCamera != null) return cachedCamera;
        return Camera.main;
    }
}
