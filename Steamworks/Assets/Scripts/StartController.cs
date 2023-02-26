using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(string scene)
    {
        GameObject.Find("LocalPlayer").GetComponent<PlayerObjectController>().StartGame(scene);
    }
}
