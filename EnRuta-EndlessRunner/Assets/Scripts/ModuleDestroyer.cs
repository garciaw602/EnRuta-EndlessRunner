// ModuleDestroyer.cs (Adjunto al Static_Module_Destroyer en la escena)

using UnityEngine;

public class ModuleDestroyer : MonoBehaviour
{
    // Esta funci�n se llama cuando un Collider (el m�dulo) entra en el Trigger (el destructor).
    void OnTriggerEnter(Collider other)
    {
        // El objeto que tiene el script RoadModule es el objeto ra�z que queremos destruir.
        RoadModule module = other.GetComponent<RoadModule>();

        // Si el RoadModule se encuentra en el objeto que colision�, lo destruimos.
        if (module != null)
        {
            // other.gameObject es la ra�z del prefab RoadModule_01.
            Destroy(other.gameObject);
        }

        // OPCIONAL: Si el Trigger del m�dulo est� en un objeto hijo y el RoadModule est� en el padre,
        // necesitamos buscar el componente RoadModule en el padre.
        else if (other.transform.parent != null)
        {
            module = other.transform.parent.GetComponent<RoadModule>();
            if (module != null)
            {
                // Destruimos el objeto padre (RoadModule_01).
                Destroy(other.transform.parent.gameObject);
            }
        }
    }
}