using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class Controller : MonoBehaviour
{
    int[,] level;
    public GameObject wall;
    public GameObject ground;

    // Start is called before the first frame update
    void Start()
    {
        level = LevelGenerator.Generate(20, 20);
        LevelGenerator.PrintLevel(level);
        for (int i = 0; i < level.GetLength(0); i++)
        {
            for (int j = 0; j < level.GetLength(1); j++)
            {
                if (level[i, j] == 1)
                {
                    Debug.Log("Ground");
                    Instantiate(ground, new Vector3(i, 0, j), new Quaternion());
                }
                else if (level[i, j] == 0)
                {
                    Debug.Log("Ground");
                    Instantiate(wall, new Vector3(i, 0, j), new Quaternion());
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
