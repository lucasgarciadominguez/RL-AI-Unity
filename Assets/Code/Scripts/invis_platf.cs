using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invis_platf : MonoBehaviour
{
    private Renderer rend;

    int shaderProperty;
    public AnimationCurve fadeIn;
    public float spawnEffectTime = 2;
    bool enableEffect = false;
    float timer = 0;
    public float pause = 1;

    private bool visibleInicial;
    private BoxCollider colider;
    private bool player_on;
    private bool platformVisible=true;
    private bool isIncreasing=false;

    private void Start()
    {
        colider = GetComponent<BoxCollider>();
        rend = GetComponentInChildren<Renderer>();  //TODO que no se hagan invisiubles en un inicio
        shaderProperty = Shader.PropertyToID("_Dissolve");

        visibleInicial = rend.enabled; // Almacena el estado inicial de visibilidad
        rend.enabled = false;

        //// Verificar si el MeshRenderer tiene al menos dos materiales
        //if (rend != null && rend.materials.Length >= 2)
        //{
        //    // Obtener los materiales
        //    material1 = rend.materials[0];
        //    material2 = rend.materials[1];

        //    // Imprimir los nombres de los materiales en la consola
        //    Debug.Log("Material 1: " + material1.name);
        //    Debug.Log("Material 2: " + material2.name);
        //}
    }
    private void Update()
    {
        if (enableEffect)
        {
            // Actualizar el valor de disolución
            if (isIncreasing)
            {
                timer += Time.deltaTime * 1;
                if (timer >= 1.0f)
                {
                    timer = 1.0f;
                    isIncreasing = false;
                    enableEffect = false; // Desactivar el efecto después de un ciclo completo
                    rend.enabled = false;

                }
            }
            else
            {
                timer -= Time.deltaTime * 1;
                if (timer <= 0.0f)
                {
                    timer = 0.0f;
                    isIncreasing = true;
                    enableEffect = false; // Desactivar el efecto después de un ciclo completo
                }
            }

            // Iterar a través de todos los materiales del Renderer y aplicar el efecto disolver
            foreach (Material material in rend.materials)
            {
                material.SetFloat(shaderProperty, timer);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        player_on = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        player_on = false;
    }
    public void Activar()
    {
        if (player_on == false)
        {
            //rend.enabled = !rend.enabled; // Cambia el estado de visibilidad al opuesto del actual
            enableEffect = true;

            rend.enabled = true;
            platformVisible = false;
            if (isIncreasing)
            {
                timer = 0.0f;

            }
            else
            {
                timer = 1.0f;
            }


        }

    }

    public void RestaurarEstadoInicial()
    {
        if (player_on == false)
        {
            rend.enabled = visibleInicial; // Restaura el estado inicial de visibilidad
        }
    }
}
