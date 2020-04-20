using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DefaultExecutionOrder(1)]
public class EditorModeStencilWriter : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private Shader maskShader;
    private static Material maskMaterial;
    private static CommandBuffer commands;
    private static Camera[] cameras;

    private void Awake()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        if (maskMaterial == null)
        {
            if (this.maskShader == null)
            {
                this.maskShader = Shader.Find("Hidden/CarverMask");
            }

            if (this.maskShader != null)
            {
                maskMaterial = new Material(this.maskShader);
            }
        }

        if (commands != null)
        {
            return;
        }

        commands = new CommandBuffer {name = "Fill Stencil Buffer"};
        commands.Blit(EditorGUIUtility.whiteTexture, BuiltinRenderTextureType.CameraTarget, maskMaterial);
        cameras = SceneView.GetAllSceneCameras().Concat(Camera.allCameras).Select(
            cam =>
            {
                cam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commands);
                return cam;
            }).ToArray();
    }

    private void OnDisable()
    {
        if (cameras == null)
        {
            return;
        }

        foreach (var cam in cameras)
        {
            if (cam == null)
            {
                continue;
            }

            cam.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, commands);
        }

        cameras = null;
        commands = null;
    }

#else
    private void Awake()
    {
        Destroy(this);
    }
#endif
}