using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;

public class ScoreReading : MonoBehaviour
{
    private string path = "Assets/Save/Score.txt";
    private ArrayList data = new ArrayList();
    private StreamReader reader;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Parse()
    {
        reader = new StreamReader(path);
        string tmp;
        while((tmp = reader.ReadLine()) != null)
        {
            data.Add(tmp);
        }
    }

    public ArrayList getScoreData()
    {
        Parse();
        return data;
    }
}
