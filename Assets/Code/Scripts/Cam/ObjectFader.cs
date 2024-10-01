using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    Player_controler player_Controler;
    [SerializeField]
    LayerMask layerPlayer;
    [SerializeField]
    LayerMask layerEnemySensors;
    [SerializeField]
    LayerMask layerWalls;

    Dictionary<Material, Color> originalEmissionColors = new Dictionary<Material, Color>();

    public int framesEntreLlamadas = 10; // Número de frames entre llamadas
    private int contadorFrames = 0;

    private void Awake()
    {
        player_Controler = FindObjectOfType<Player_controler>();
    }

    void LateUpdate()
    {
        LogicFadeObjects(transform.position);

    }

    void LogicFadeObjects(Vector3 positionSendRay)
    {
        Vector3 playerPosition = new Vector3(player_Controler.transform.position.x,
            player_Controler.transform.position.y + 1, player_Controler.transform.position.z);
        Vector3 dir = playerPosition - positionSendRay;

        Ray ray = new Ray(positionSendRay, dir);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                if (layerWalls == (layerWalls | (1 << hit.transform.gameObject.layer))
                    && (layerPlayer != (layerPlayer | (1 << hit.transform.gameObject.layer))))
                {
                    float r = Random.value;
                    float g = Random.value;
                    float b = Random.value;
                    Debug.DrawRay(positionSendRay, dir, new Color(r, g, b));
                    MeshRenderer meshRenderer = hit.transform.gameObject.GetComponent<MeshRenderer>();
                    StartCoroutine(FadeObjects(meshRenderer));
                }
                else if (layerPlayer == (layerPlayer | (1 << hit.transform.gameObject.layer)))
                {
                    //Debug.Log("over");
                }
            }
        }
    }
    IEnumerator FadeObjects(MeshRenderer meshRenderer)
    {
        //originalEmissionColors.Clear();

        foreach (Material material in meshRenderer.materials)
        {
            if (material.IsKeywordEnabled("_EMISSION"))
            {
                if (!originalEmissionColors.ContainsKey(material))
                {
                    originalEmissionColors[material] = material.GetColor("_EmissionColor");
                }

                material.SetOverrideTag("RenderType", "Transparent");

                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;



                // Guardar el color de emisión original
                Color originalEmissionColor = originalEmissionColors[material];

                // Obtener el brillo del color de emisión
                float brightness = originalEmissionColor.maxColorComponent;

                // Crear un nuevo color con el brillo reducido
                Color newEmissionColor = originalEmissionColor / brightness * 0.26f;

                // Asignar el nuevo color de emisión al material
                material.SetColor("_EmissionColor", newEmissionColor);
            }
            else
            {
                material.SetOverrideTag("RenderType", "Transparent");

                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;



                material.color = new Color(1, 1, 1, 0.26f);
            }

        }
        yield return new WaitForSeconds(.5f);
        foreach (Material material in meshRenderer.materials)
        {

            material.SetOverrideTag("RenderType", "");

            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;

            //material.SetShaderPassEnabled("SHADOWCASTER", true);
            // Restaurar el color de emisión original si el material tenía emisión
            if (originalEmissionColors.ContainsKey(material))
            {
                 material.SetColor("_EmissionColor", originalEmissionColors[material]);
                originalEmissionColors.Remove(material);
            }

            // Restaurar el color opaco original
            material.color = Color.white;
            //material.color = new Color(1, 1, 1, 1);
        }
    }
}
