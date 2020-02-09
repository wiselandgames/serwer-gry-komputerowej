using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ManagerSieci : MonoBehaviour
{
    public static ManagerSieci instancja;

    public GameObject prefabrykatGracza;
    private void Awake()
    {
        if (instancja == null)
        {
            instancja = this;
        }
        else if (instancja != this)
        {
            Debug.Log("Instancja już istnieje... zniszcz instancje!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        #if UNITY_EDITOR
        Debug.Log("Zbuduj projekt aby uruchomić serwer!");
        #else
            Serwer.Start(18, 24842);
        #endif
    }

    public Gracz InstancjujGracza()
    {

        return Instantiate(prefabrykatGracza, Vector3.zero, Quaternion.identity).GetComponent<Gracz>();
    }


}
