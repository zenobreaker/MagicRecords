using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class UpdateShaderProperties : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.hasChanged)
        {
            Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();

            foreach (var r in renderers)
            {
                Material m;
#if UNITY_EDITOR
                m = r.sharedMaterial;
#else
                m = r.material;
#endif                
                if (m == null)
                    return;
                if (string.Compare(m.shader.name, "Shader Graphs/ToonRamp") == 0)
                {

                    m.SetVector("_LightDir", transform.forward);
                }
            }
        }
    }
}
