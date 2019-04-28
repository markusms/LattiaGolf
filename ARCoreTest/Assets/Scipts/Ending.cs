using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    //private GameObject textEnding;
    public InstantiateObjectOnTouch gcScript;

    // Start is called before the first frame update
    void Start()
    {
        gcScript = GameObject.FindObjectOfType<InstantiateObjectOnTouch>();
        //textEnding = GameObject.Find("EndingText");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ball"))
            SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex+1);
        //GUI.Button(new Rect(10, 50, 100, 30), "Ended");
        //textEnding.SetActive(true);

    }

    public void FindScript()
    {
        gcScript = GameObject.FindObjectOfType<InstantiateObjectOnTouch>();
    }
}
