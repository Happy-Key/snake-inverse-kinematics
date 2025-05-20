using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    public SnakeGameManager gm;
    public TMPro.TextMeshProUGUI text;

    private void Update()
    {
        text.text = "Score: " + gm.score;
    }
}
