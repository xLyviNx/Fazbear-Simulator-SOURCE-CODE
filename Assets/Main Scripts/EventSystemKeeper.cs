using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemKeeper : MonoBehaviour
{

    public static EventSystemKeeper Instance;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
