using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorColorController : MonoBehaviour
{
    [SerializeField] private GameObject[] Floor;

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
            //Color color = new Color(1f, 0.5f, 0.5f);
            //ChangeFloorColor(1, 0, color);
            //SceneManager.LoadScene("PacManMap");
        //}
    }

    public void ChangeFloorColor(int row, int col, Color color)
    {
        Renderer renderer = Floor[(row * 20) + col].GetComponent<Renderer>();
        renderer.material.color = new Color(color.r, color.g, color.b, 1f);
    }

    public void ClearColor()
    {
        for(int i = 0; i < Floor.Length; i++)
        {
            Renderer renderer = Floor[i].GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
